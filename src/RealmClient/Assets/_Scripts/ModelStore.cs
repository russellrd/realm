using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ModelStore : MonoBehaviour
{
    public Dictionary<string, ModelDTO> modelData;
    public Dictionary<string, GameObject> modelObjects;
    public Dictionary<string, Sprite> sprites;
    public bool initialized = false;

    public List<String> getKeys()
    {
        var keys = modelData.Keys.ToList();
        keys.Sort();
        return keys;
    }
}
