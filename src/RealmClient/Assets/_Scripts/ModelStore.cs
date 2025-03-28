using System.Collections.Generic;
using GLTFast;
using UnityEngine;

public class ModelStore : MonoBehaviour
{
    public Dictionary<string, ModelDTO> modelData;
    public Dictionary<string, GltfImport> modelObjects;
}
