using UnityEngine;
using PocketBaseSdk;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using GLTFast;
using System.Linq;

namespace Realm
{
    public class DatabaseController : MonoBehaviour
    {
        public static PocketBase pb;

        public ModelStore modelStore;

        private async void Awake()
        {

        }

        private async void Start()
        {
            pb = new PocketBase("https://pocketbase.midnightstudio.me", "en-US", AsyncAuthStore.PlayerPrefs);
            await updateModels();
        }

        // public static void Logout()
        // {
        //     pb.AuthStore.Clear();
        //     SceneManager.LoadScene(0);
        // }

        //==========================Authentication==========================

        public static bool IsLoggedIn()
        {
            return pb.AuthStore != null && pb.AuthStore.IsValid();
        }

        public static bool IsValidUser()
        {
            if (!IsLoggedIn())
                return false;
            if (pb.AuthStore.Model["type"].ToString() == "organization")
            {
                if (pb.AuthStore.Model["verifiedAsOrg"].ToString() == "False")
                    return false;
            }
            return true;
        }

        public static void Logout()
        {
            pb.AuthStore.Clear();
        }

        public static async Task<RecordAuth> AuthenticateUser(string email, string password)
        {
            var userData = await pb.Collection("users").AuthWithPassword(email, password);
            Debug.Log("REALM: User " + userData.Record.Email);
            return userData;
        }

        public static async Task<bool> CreateNewUser(
            string name,
            string email,
            string password,
            string type,
            string organizationId = null
        )
        {
            UserCreateDTO data = new()
            {
                Name = name,
                Email = email,
                Password = password,
                PasswordConfirm = password,
                Type = type,
                OrganizationId = organizationId
            };

            try
            {
                var record = await pb.Collection("users").Create(data);
                if (type != "organization")
                    await AuthenticateUser(email, password);
            }
            catch (Exception e)
            {
                Debug.LogError(e.InnerException.ToString());
                return false;
            }

            return true;
        }

        public static string GetCurrentUserOrganizationId()
        {
            return pb.AuthStore.Model["organizationId"].ToString();
        }

        public static string GetCurrentUserName()
        {
            return pb.AuthStore.Model["name"].ToString();
        }

        //============================AR Objects============================

        public static async Task<List<AnchorDTO>> GetAllARObjects()
        {
            var rawAnchors = await pb.Collection("ar_objects").GetFullList();
            List<AnchorDTO> anchors = new();
            foreach (RecordModel anchor in rawAnchors)
            {
                anchors.Add(AnchorDTO.FromRecord(anchor));
            }
            return anchors;
        }

        public static async Task<AnchorDTO> GetARObjectFromAnchorId(string anchorId)
        {
            var anchors = await GetAllARObjects();
            foreach (AnchorDTO anchorDTO in anchors)
            {
                if (anchorDTO.AnchorId == anchorId)
                    return anchorDTO;
            }
            return new AnchorDTO();
        }

        //==============================Tours===============================

        public static async Task<bool> CreateTour(
            string name,
            string description,
            double startLatitude,
            double startLongitude
        )
        {
            TourDTO data = new()
            {
                Name = name,
                Description = description,
                OrganizationId = GetCurrentUserOrganizationId(),
                StartLatitude = startLatitude,
                StartLongitude = startLongitude
            };

            try
            {
                var record = await pb.Collection("tours").Create(data);
            }
            catch (Exception e)
            {
                Debug.LogError(e.InnerException.ToString());
                return false;
            }

            return true;
        }

        public static async Task<List<TourDTO>> GetAllTours()
        {
            var rawTours = await pb.Collection("tours").GetFullList();
            List<TourDTO> tours = new();
            foreach (RecordModel tour in rawTours)
            {
                tours.Add(TourDTO.FromRecord(tour));
            }
            return tours;
        }

        public static async Task<TourDTO> GetTourFromId(string tourId)
        {
            var tours = await GetAllTours();
            foreach (TourDTO tourDTO in tours)
            {
                if (tourDTO.Id == tourId)
                    return tourDTO;
            }
            return new TourDTO();
        }

        public static async Task<bool> UpdateTourFromId(
            string tourId,
            string name,
            string description,
            double startLatitude,
            double startLongitude
        )
        {
            try
            {
                Debug.Log(tourId);
                var record = await pb.Collection("tours").Update(tourId,
                body: new TourDTO()
                {
                    Name = name,
                    Description = description,
                    OrganizationId = GetCurrentUserOrganizationId(),
                    StartLatitude = startLatitude,
                    StartLongitude = startLongitude
                });
            }
            catch (Exception e)
            {
                Debug.LogError(e.InnerException.ToString());
                return false;
            }

            return true;
        }

        //==========================Organizations===========================

        public static async Task<List<OrganizationDTO>> GetAllOrganizations()
        {
            var rawOrganizations = await pb.Collection("organizations").GetFullList();
            List<OrganizationDTO> organizations = new();
            foreach (RecordModel organization in rawOrganizations)
            {
                organizations.Add(OrganizationDTO.FromRecord(organization));
            }
            return organizations;
        }

        public static async Task<OrganizationDTO> GetOrganizationFromId(string organizationId)
        {
            var organizations = await GetAllOrganizations();
            foreach (OrganizationDTO organizationDTO in organizations)
            {
                if (organizationDTO.Id == organizationId)
                    return organizationDTO;
            }
            return new OrganizationDTO();
        }

        public async Task updateModels()
        {
            Debug.Log("UPDATE MODELS (1)");

            if (modelStore.modelData == null)
            {
                modelStore.modelData = new();
            }
            if (modelStore.modelObjects == null)
            {
                modelStore.modelObjects = new();
            }
            if (modelStore.sprites == null)
            {
                modelStore.sprites = new();
            }


            ResultList<RecordModel> l = await pb.Collection("models").GetList(1, 1, fields: "id");
            Debug.Log($"total items: {l.TotalItems}, existing items: {modelStore.modelData.Count}");
            if (modelStore.modelData.Count >= l.TotalItems)
            {
                return;
            }

            Debug.Log("UPDATE MODELS (2)");

            // get all models made by the user or ARC team (id = 9xj9tsw0c9010du)
            List<RecordModel> notPresent = await pb.Collection("models").GetFullList(filter: $"creator=\"{pb.AuthStore.Model.Id}\" || creator=\"9xj9tsw0c9010du\"");//, fields: "id");
            Debug.Log("UPDATE MODELS (2.1)");
            notPresent = notPresent.Where(m => !modelStore.modelData.ContainsKey(m.Id)).ToList();
            Debug.Log("UPDATE MODELS (2.2)");

            List<RecordModel> modelRecords = new List<RecordModel>();
            Debug.Log("UPDATE MODELS (2.3)");
            List<ModelDTO> models = new List<ModelDTO>();
            Debug.Log("UPDATE MODELS (2.4)");

            Debug.Log("UPDATE MODELS (3)");

            foreach (var m in notPresent)
            {
                // var record = await pb.Collection("models").GetOne(m.Id);
                modelRecords.Add(m);
                models.Add(ModelDTO.FromRecord(m));
                Debug.Log($"model record loaded with ID: {models.Last().ID}");
            }

            Debug.Log("UPDATE MODELS (4)");

            GameObject cameraObject = new();
            // Camera previewCam = cameraObject.AddComponent<Camera>();
            // Camera main = Camera.main;
            // previewCam.gameObject.transform.position = new Vector3(10000, 10000);
            // main.gameObject.transform.position = new Vector3(10000, 10000);

            // previewCam.gameObject.transform.rotation = Quaternion.identity;
            // main.gameObject.transform.rotation = Quaternion.identity;

            int spriteSize = 128;
            // RenderTexture renderTexture = new(spriteSize, spriteSize, 24);
            // previewCam.targetTexture = renderTexture;

            // RenderTexture.active = renderTexture;

            Debug.Log("UPDATE MODELS (5)");

            Debug.Log($"model count: {models.Count}");

            for (int i = 0; i < models.Count; i++)
            {
                var uri = pb.Files.GetUrl(modelRecords[i], models[i].Model);
                var g = new GltfImport();
                Debug.Log($"UPDATE MODELS (5.{i}.1)");

                bool success = await g.Load(uri);
                if (!success)
                {
                    Debug.Log($"failed to load model: {models[i].Name}");
                }
                Debug.Log($"UPDATE MODELS (5.{i}.2)");

                GameObject prefabObject = await GLTFUtils.InstantiateARObjectFromGltf(g);

                // GameObject previewObject = new();
                // await g.InstantiateMainSceneAsync(previewObject.transform);
                // previewObject.SetActive(true);

                // previewObject.transform.position = previewCam.gameObject.transform.position + previewCam.gameObject.transform.forward * 5;
                // previewCam.transform.LookAt(previewObject.transform);

                // Debug.Log($"UPDATE MODELS (5.{i}.3)");

                // Texture2D texture = new Texture2D(spriteSize, spriteSize, TextureFormat.RGB24, false);
                // Rect rect = new Rect(0, 0, spriteSize, spriteSize);

                // previewCam.Render();
                // // await Task.Delay(2000);


                // texture.ReadPixels(rect, 0, 0);
                // texture.Apply();


                Debug.Log($"UPDATE MODELS (5.{i}.4)");
                // Sprite sprite = Sprite.Create(texture, rect, Vector2.zero);
                Debug.Log($"UPDATE MODELS (5.{i}.5)");
                // Debug.Log($"modelId: {models[i].ID}, sprite: {sprite == null}");

                // Destroy(previewObject);
                // modelStore.sprites.Add(models[i].ID, sprite);
                // modelStore.sprites.TryAdd(models[i].ID, sprite);

                modelStore.modelObjects.Add(models[i].ID, prefabObject);
                modelStore.modelData.Add(models[i].ID, models[i]);
                Debug.Log($"UPDATE MODELS (5.{i}.6)");

            }

            modelStore.initialized = true;

            Debug.Log("UPDATE MODELS (6) (Final)");
        }
    }
}