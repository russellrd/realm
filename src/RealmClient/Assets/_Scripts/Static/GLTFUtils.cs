using System.Threading.Tasks;
using GLTFast;
using UnityEngine;

public static class GLTFUtils
{
    public static async Task<GameObject> InstantiateARObjectFromGltf(GltfImport gltf)
    {
        var arObject = (GameObject)UnityEngine.GameObject.Instantiate(Resources.Load("ARObject"), Vector3.zero, Quaternion.identity);
        await gltf.InstantiateMainSceneAsync(arObject.transform);
        var world = arObject.transform.Find("world").gameObject;
        var model = world.transform.GetChild(0).gameObject;
        var boxCollider = model.AddComponent<BoxCollider>();
        Vector3 offsetPos = model.transform.position;
        offsetPos.y = boxCollider.center.y + boxCollider.size.y / 2;
        model.transform.position = offsetPos;

        arObject.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);

        arObject.SetActive(false);

        return arObject;
    }
    public static async Task<GameObject> InstantiateARObjectFromGltf(GltfImport gltf, Vector3 position, Quaternion rotation)
    {
        var newObject = await InstantiateARObjectFromGltf(gltf);
        newObject.transform.position = position;
        newObject.transform.rotation = rotation;
        return newObject;
    }
}
