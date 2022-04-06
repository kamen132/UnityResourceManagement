using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AB_UnityWebRequestAssetBundle : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    IEnumerator InstantiateObject(string assetBundleName)
    {
        string uri = "file:///" + Application.dataPath + "/AssetBundles/" + assetBundleName;
        UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(uri, 0);
        yield return request.SendWebRequest();
        AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(request);
        GameObject cube = bundle.LoadAsset<GameObject>("Cube");
        GameObject sprite = bundle.LoadAsset<GameObject>("Sprite");
        Instantiate(cube);
        Instantiate(sprite);
    }
}

