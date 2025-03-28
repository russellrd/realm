using GLTFast;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Transformers;

public static class GLTFUtils
{
    public static GameObject InstantiateARObjectFromGltf(GltfImport gltf)
    {
        GameObject newObject = new();
        gltf.InstantiateMainSceneAsync(newObject.transform);
        ARTransformer aRTransformer = newObject.AddComponent<ARTransformer>();
        XRGrabInteractable xRGrabInteractable = newObject.GetOrAddComponent<XRGrabInteractable>();
        Rigidbody rigidbody = newObject.GetOrAddComponent<Rigidbody>();
        Debug.Log($"Rigid body props:\nmass: {rigidbody.mass}\n");
        Debug.Log($"AR transformer props:\nscale sensitivity: {aRTransformer.scaleSensitivity}\nelastic break limit: {aRTransformer.elasticBreakLimit}\n");
        Debug.Log($"XR grab interactable props:\nattach ease-in time: {xRGrabInteractable.attachEaseInTime}\n");
        return newObject;
    }
    public static GameObject InstantiateARObjectFromGltf(GltfImport gltf, Vector3 position, Quaternion rotation)
    {
        GameObject newObject = new();
        gltf.InstantiateMainSceneAsync(newObject.transform);
        ARTransformer aRTransformer = newObject.AddComponent<ARTransformer>();
        XRGrabInteractable xRGrabInteractable = newObject.GetOrAddComponent<XRGrabInteractable>();
        Rigidbody rigidbody = newObject.GetOrAddComponent<Rigidbody>();
        newObject.transform.position = position;
        newObject.transform.rotation = rotation;
        Debug.Log($"Rigid body props:\nmass: {rigidbody.mass}\n");
        Debug.Log($"AR transformer props:\nscale sensitivity: {aRTransformer.scaleSensitivity}\nelastic break limit: {aRTransformer.elasticBreakLimit}\n");
        Debug.Log($"XR grab interactable props:\nattach ease-in time: {xRGrabInteractable.attachEaseInTime}\n");
        return newObject;
    }
}
