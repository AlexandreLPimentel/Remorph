using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

[Serializable]
public struct MyPair
{
    public MyDictionary.Platform platform;
    public GameObject prefab;
}

[ExecuteInEditMode]
public class LevelEditor : MonoBehaviour
{
    public static LevelEditor instance;

    public List<MyPair> platformMapping = new List<MyPair>();

    private Dictionary<MyDictionary.Platform, GameObject> platformAssetMap = new Dictionary<MyDictionary.Platform, GameObject>();
    
    public void Start()
    {
        instance = this;
        foreach (var kvp in platformMapping)
        {
            platformAssetMap[kvp.platform] = kvp.prefab;
        }
    }

    public GameObject getPlatformObject(MyDictionary.Platform platform)
    {
        GameObject ret;
        platformAssetMap.TryGetValue(platform, out ret);
        return ret;
    }

    public void buttonClicked(MyDictionary.Platform platform)
    {
        print("Clicked " + platform);
    }
}
