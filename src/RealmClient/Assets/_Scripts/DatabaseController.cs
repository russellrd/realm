using UnityEngine;
using PocketBaseSdk;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using System.Collections.Generic;
using GLTFast;
using System.Linq;

public class DatabaseController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public PocketBase pb;
    public ModelStore modelStore;

    private void Start()
    {
        pb = new PocketBase("https://pocketbase.midnightstudio.me", "en-US", AsyncAuthStore.PlayerPrefs);
        if (pb.AuthStore == null || !pb.AuthStore.IsValid())
        {
            SceneManager.LoadScene(0);
        }
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
            modelStore.modelObjects.Add(models[i].Id, g);
            modelStore.modelData.Add(models[i].Id, models[i]);
        }

    }

    public async void updateModels()
    {
        ResultList<RecordModel> l = await pb.Collection("models").GetList(1, 1, fields: "id");
        if (modelStore.modelData.Count >= l.TotalItems)
        {
            return;
        }

        // get all models made by the user or ARC team (id = 9xj9tsw0c9010du)
        List<RecordModel> notPresent = await pb.Collection("models").GetFullList(filter: $"creator={pb.AuthStore.Model.Id} || creator=\"9xj9tsw0c9010du\"", fields: "id");
        notPresent = notPresent.Where(m => !modelStore.modelData.ContainsKey(m.Id)).ToList();

        List<RecordModel> modelRecords = new List<RecordModel>();
        List<ModelDTO> models = new List<ModelDTO>();

        foreach (var m in notPresent)
        {
            var record = await pb.Collection("models").GetOne(m.Id);
            modelRecords.Add(record);
            models.Add(ModelDTO.FromRecord(record));
        }



        for (int i = 0; i < models.Count; i++)
        {
            var uri = pb.Files.GetUrl(modelRecords[i], models[i].Model);
            var g = new GltfImport();
            bool success = await g.Load(uri);
            if (!success)
            {
                Debug.Log($"failed to load model: {models[i].Name}");
            }
            modelStore.modelObjects.Add(models[i].Id, g);
            modelStore.modelData.Add(models[i].Id, models[i]);
        }
    }
}
