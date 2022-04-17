using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using UnityEngine;
public class ResourceTest:MonoBehaviour
{
    private void Start()
    {
        TestLoadAB();
    }

    private void TestLoadAB()
    {
        AssetBundle ab = AssetBundle.LoadFromFile(Application.streamingAssetsPath+"/data");
        TextAsset textAsset = ab.LoadAsset<TextAsset>("AssetBundleConfig");
        MemoryStream stream = new MemoryStream(textAsset.bytes);
        BinaryFormatter bf = new BinaryFormatter();
        AssetBundleConfig config = (AssetBundleConfig)bf.Deserialize(stream);
        stream.Close();
        string path = "Assets/GameData/Prefab/RKPrefab.prefab";
        ABBase abBase = null;
        uint crc = Crc32.GetCrc32(path);
        for (int i = 0; i < config.ABList.Count; i++)
        {
            if (config.ABList[i].Crc==crc)
            {
                abBase = config.ABList[i];
            }
        }

        AssetBundle assetBundle = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/" + abBase.ABName);
        GameObject obj = GameObject.Instantiate(assetBundle.LoadAsset<GameObject>(abBase.AssetName));
    }
}
