using System.Threading.Tasks;
using GLTFast;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Transformers;

public static class GLTFUtils
{
    public static async Task<GameObject> InstantiateARObjectFromGltf(GltfImport gltf)
    {
        GameObject newObject = new();
        await gltf.InstantiateMainSceneAsync(newObject.transform);
        ARTransformer aRTransformer = newObject.AddComponent<ARTransformer>();
        XRGrabInteractable xRGrabInteractable = newObject.GetOrAddComponent<XRGrabInteractable>();
        ARObjectManager aRObjectManager = newObject.AddComponent<ARObjectManager>();
        BoxCollider boxCollider = newObject.AddComponent<BoxCollider>();
        Rigidbody rigidbody = newObject.GetOrAddComponent<Rigidbody>();
        newObject.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);

        newObject.SetActive(false);

        return newObject;
    }
    public static async Task<GameObject> InstantiateARObjectFromGltf(GltfImport gltf, Vector3 position, Quaternion rotation)
    {
        var newObject = await InstantiateARObjectFromGltf(gltf);
        newObject.transform.position = position;
        newObject.transform.rotation = rotation;
        return newObject;
    }
}
