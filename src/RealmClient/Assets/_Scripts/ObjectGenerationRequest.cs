using System;
using System.Collections;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using GLTFast;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;

public class ObjectGenerationRequest
{
    public GltfImport gltf;
    public string creationName;
    private string url = "http://72.200.100.237:11077/generate";
    private ObjectGenRequestDTO data;
    private string filepath;

    public ObjectGenerationRequest(ObjectGenRequestDTO dat, string n)
    {
        creationName = n;
        data = dat;
        filepath = $"{Application.persistentDataPath}/{creationName}.glb";
    }

    public async Task promptGeneration()
    {

        UnityWebRequest req = UnityWebRequest.Post(url, JsonConvert.SerializeObject(data), "application/json");

        // req.downloadHandler = new DownloadHandlerFile(filepath);
        req.downloadHandler = new DownloadHandlerBuffer();
        await req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.ConnectionError || req.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log($"{req.error} : {req.downloadHandler.text}");

        }
        else
        {
            Debug.Log(req.downloadHandler.text);
            loadModel(req.downloadHandler.data);
        }
    }

    private async void loadModel(byte[] data)
    {
        gltf = new GltfImport();
        bool success = await gltf.LoadGltfBinary(
            data
            // The URI of the original data is important for resolving relative URIs within the glTF
            // new Uri(filePath)
            );
        if (!success)
        {
            Debug.Log("RIP");
        }
        else
        {
            Debug.Log("GLB LOADED");
        }
    }
}