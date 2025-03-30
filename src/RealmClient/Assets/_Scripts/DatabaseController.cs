using UnityEngine;
using PocketBaseSdk;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using System.Collections.Generic;
using GLTFast;
using System.Linq;
using Unity.VisualScripting;

public class DatabaseController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public PocketBase pb;
    public Sprite sampleSprite;
    public ModelStore modelStore;

    private async void Awake()
    {
        pb = new PocketBase("https://pocketbase.midnightstudio.me", "en-US", AsyncAuthStore.PlayerPrefs);
        if (pb.AuthStore == null || !pb.AuthStore.IsValid())
        {
            SceneManager.LoadScene(0);
        }
        await updateModels();
    }

    private async void Start()
    {

    }

    public void Logout()
    {
        pb.AuthStore.Clear();
        SceneManager.LoadScene(0);
    }

    public async Task<List<AnchorDTO>> GetAllARObjects()
    {
        var rawAnchors = await pb.Collection("ar_objects").GetFullList();
        List<AnchorDTO> anchors = new List<AnchorDTO>();
        foreach (RecordModel a in rawAnchors)
        {
            anchors.Add(AnchorDTO.FromRecord(a));
        }
        return anchors;
    }

    public async Task<AnchorDTO> GetARObjectFromAnchorId(string anchorId)
    {
        var anchors = await GetAllARObjects();
        foreach (AnchorDTO anchorDTO in anchors)
        {
            if (anchorDTO.AnchorId == anchorId)
                return anchorDTO;
        }
        return new AnchorDTO();
    }

    public async Task loadAllModels()
    {
        Debug.Log("SHOULD NOT EXECUTE");
        List<RecordModel> modelRecords = await pb.Collection("models").GetFullList(filter: $"creator=\"{pb.AuthStore.Model.Id}\"||creator=\"9xj9tsw0c9010du\"");
        List<ModelDTO> models = modelRecords.ConvertAll(ModelDTO.FromRecord);

        for (int i = 0; i < models.Count; i++)
        {
            var uri = pb.Files.GetUrl(modelRecords[i], models[i].Model);
            var g = new GltfImport();
            bool success = await g.Load(uri);
            if (!success)
            {
                Debug.Log($"failed to load model: {models[i].Name}");
            }
            GameObject prefabObject = await GLTFUtils.InstantiateARObjectFromGltf(g);
            modelStore.modelObjects.Add(models[i].Id, prefabObject);
            modelStore.modelData.Add(models[i].Id, models[i]);
        }

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
        Camera previewCam = cameraObject.AddComponent<Camera>();
        Camera main = Camera.main;
        // previewCam.gameObject.transform.position = new Vector3(10000, 10000);
        // main.gameObject.transform.position = new Vector3(10000, 10000);

        // previewCam.gameObject.transform.rotation = Quaternion.identity;
        // main.gameObject.transform.rotation = Quaternion.identity;

        int spriteSize = 128;
        RenderTexture renderTexture = new(spriteSize, spriteSize, 24);
        previewCam.targetTexture = renderTexture;

        RenderTexture.active = renderTexture;

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

            GameObject previewObject = new();
            await g.InstantiateMainSceneAsync(previewObject.transform);
            previewObject.SetActive(true);

            previewObject.transform.position = previewCam.gameObject.transform.position + previewCam.gameObject.transform.forward * 5;
            previewCam.transform.LookAt(previewObject.transform);

            Debug.Log($"UPDATE MODELS (5.{i}.3)");

            Texture2D texture = new Texture2D(spriteSize, spriteSize, TextureFormat.RGB24, false);
            Rect rect = new Rect(0, 0, spriteSize, spriteSize);

            previewCam.Render();
            // await Task.Delay(2000);


            texture.ReadPixels(rect, 0, 0);
            texture.Apply();


            Debug.Log($"UPDATE MODELS (5.{i}.4)");
            Sprite sprite = Sprite.Create(texture, rect, Vector2.zero);
            Debug.Log($"UPDATE MODELS (5.{i}.5)");
            Debug.Log($"modelId: {models[i].ID}, sprite: {sprite == null}");

            Destroy(previewObject);
            // modelStore.sprites.Add(models[i].ID, sprite);
            modelStore.sprites.TryAdd(models[i].ID, sprite);
            if (models[i].ID == "vjz118q86az7otl")
            {
                modelStore.sprites.TryAdd(models[i].ID, sampleSprite);
            }
            modelStore.modelObjects.Add(models[i].ID, prefabObject);
            modelStore.modelData.Add(models[i].ID, models[i]);
            Debug.Log($"UPDATE MODELS (5.{i}.6)");

        }

        modelStore.initialized = true;

        Debug.Log("UPDATE MODELS (6) (Final)");
    }

}
