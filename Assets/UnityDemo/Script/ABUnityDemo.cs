using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class ABUnityDemo : MonoBehaviour
{
    private string abFilePath;
    private string manifestFilePath;

    private void Awake()
    {
        abFilePath = Application.dataPath + "/UnityDemo/AssetBundles/unitydemo.unity3d";
        manifestFilePath = Application.dataPath + "/UnityDemo/AssetBundles/AssetBundles.manifest";
    }

    private void Start()
    {
        //第一种加载
        //StartCoroutine(LoadFromMemoryAsync(abFilePath));
        //第二种加载
        //LoadFromFile();

        StartCoroutine(InstantiateObject());
    }


    //AssetBundle.LoadFromMemoryAsync
    IEnumerator LoadFromMemoryAsync(string path)
    {
        AssetBundleCreateRequest createRequest = AssetBundle.LoadFromMemoryAsync(File.ReadAllBytes(path));
        yield return createRequest;
        AssetBundle bundle = createRequest.assetBundle;
        var prefab = bundle.LoadAsset<GameObject>("AbAsset");
        Instantiate(prefab);
    }

    //AssetBundle.LoadFromFile
    public void LoadFromFile()
    {
        var myLoadedAssetBundle = AssetBundle.LoadFromFile(abFilePath);

        if (myLoadedAssetBundle == null)
        {
            Debug.Log("Failed to load AssetBundle!");
            return;
        }
        var prefab = myLoadedAssetBundle.LoadAsset<GameObject>("AbAsset");
        Instantiate(prefab);
    }
    
    //UnityWebRequestAssetBundle
    IEnumerator InstantiateObject()
    {
        string uri = "http://127.0.0.1/AssetBundle/unitydemo.unity3d";
        UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(uri, 0);
        yield return request.SendWebRequest();
        AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(request);
        GameObject cube = bundle.LoadAsset<GameObject>("AbAsset");
        //GameObject sprite = bundle.LoadAsset<GameObject>("Sprite");
        Instantiate(cube);
        //Instantiate(sprite);
    }

    //加载 AssetBundle 清单
    public void LoadManifest()
    {
        AssetBundle assetBundle = AssetBundle.LoadFromFile(manifestFilePath);
        AssetBundleManifest manifest = assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
    }

    public void LoadAssetsByManifest(string assetBundlePath)
    {
        AssetBundle assetBundle = AssetBundle.LoadFromFile(manifestFilePath);
        AssetBundleManifest manifest = assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        string[] dependencies = manifest.GetAllDependencies("assetBundle"); //传递想要依赖项的捆绑包的名称。
        foreach(string dependency in dependencies)
        {
            AssetBundle.LoadFromFile(Path.Combine(assetBundlePath, dependency));
        }
    }
}
