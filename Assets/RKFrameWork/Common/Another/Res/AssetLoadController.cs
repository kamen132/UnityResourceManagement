using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Majic.CM
{
    public class AssetLoadController : MonoBehaviour
    {
        private static volatile AssetLoadController _AssetLoadController;
        private OnCallBackSObject onResCallBack = null;
        private OnCallBackSObject onABCallBack = null;
        private OnCallBackSObject onABAssetCallBack = null;
        private const string BasePath = "Assets/WorkAssets/";
        private int idIndex = 0;
        private Dictionary<int, Coroutine> coroutineDic = new Dictionary<int, Coroutine>();
        public static AssetLoadController Instance
        {
            get
            {
                return _AssetLoadController;
            }
        }

        private void Awake()
        {
            _AssetLoadController = this;
        }

        public void OnResCallBackAction(OnCallBackSObject callBack)
        {
            onResCallBack = callBack;
        }
        public void OnABCallBackAction(OnCallBackSObject callBack)
        {
            onABCallBack = callBack;
        }
        public void OnABAssetCallBackAction(OnCallBackSObject callBack)
        {
            onABAssetCallBack = callBack;
        }

        public Object LoadResSync(string path)
        {
            return LoadResSync(path, typeof(Object));
        }

        public Object LoadResSync(string path, System.Type systemTypeInstance)
        {
            path = BasePath + path;
            Object asset = null;
#if UNITY_EDITOR
            asset = AssetDatabase.LoadAssetAtPath(path, systemTypeInstance);
#endif
            return asset;
        }

        //load from resource asynchronous
        public int LoadResourceAsync(string fileName)
        {
            return LoadResourceAsync(fileName, typeof(Object));
        }

        public int LoadResourceAsync(string filePath, System.Type systemTypeInstance)
        {
            idIndex++;
            Coroutine coroutine = StartCoroutine(OnLoadResourceAsync(idIndex, filePath, systemTypeInstance));
            coroutineDic[idIndex] = coroutine;
            return idIndex;
        }

        IEnumerator OnLoadResourceAsync(int index, string filePath, System.Type systemTypeInstance)
        {
            string path = BasePath + filePath;
            Object asset = null;
#if UNITY_EDITOR
            asset = AssetDatabase.LoadAssetAtPath(path, systemTypeInstance);
#endif
            yield return null;
            coroutineDic.Remove(index);
            if (onResCallBack != null)
            {
                onResCallBack(filePath, asset);
            }
        }

        public AssetBundle LoadAssetBundleSync(string assetBundlePath)
        {
            return AssetBundle.LoadFromFile(assetBundlePath, 0, (ulong)YQPackageManagerEX.bundleOffSize);
        }

        public Object OnLoadAssetSync(AssetBundle assetBundle, string assetName)
        {
            return assetBundle.LoadAsset(assetName);
        }
        public int PreLoadAssetBundleAsync(string assetBundlePath, OnCallBackSObject callBack)
        {
            idIndex++;
            var coroutine = StartCoroutine(OnPreLoadAssetBundleAsync(idIndex, assetBundlePath, callBack));
            coroutineDic[idIndex] = coroutine;
            return idIndex;
        }

        IEnumerator OnPreLoadAssetBundleAsync(int index, string assetBundlePath, OnCallBackSObject callBack)
        {
            AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(assetBundlePath, 0, (ulong)YQPackageManagerEX.bundleOffSize);
            yield return request;
            coroutineDic.Remove(index);
            if (callBack != null)
            {
                callBack(assetBundlePath, request.assetBundle);
            }
        }

        public int LoadAssetBundleAsyncWithCallBack(string assetBundlePath, OnCallBackSObject callBack)
        {
            idIndex++;
            var coroutine = StartCoroutine(OnLoadAssetBundleAsyncWithCallBack(idIndex, assetBundlePath, callBack));
            coroutineDic[idIndex] = coroutine;
            return idIndex;
        }

        IEnumerator OnLoadAssetBundleAsyncWithCallBack(int index, string assetBundlePath, OnCallBackSObject callBack)
        {
            AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(assetBundlePath, 0, (ulong)YQPackageManagerEX.bundleOffSize);
            yield return request;
            coroutineDic.Remove(index);
            if (callBack != null)
            {
                callBack(assetBundlePath, request.assetBundle);
            }
        }
        public int LoadAssetBundleAsync(string assetBundlePath)
        {
            idIndex++;
            var coroutine = StartCoroutine(OnLoadAssetBundleAsync(idIndex, assetBundlePath));
            coroutineDic[idIndex] = coroutine;
            return idIndex;
        }

        IEnumerator OnLoadAssetBundleAsync(int index, string assetBundlePath)
        {
            AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(assetBundlePath, 0, (ulong)YQPackageManagerEX.bundleOffSize);
            yield return request;
            coroutineDic.Remove(index);
            if (onABCallBack != null)
            {
                onABCallBack(assetBundlePath, request.assetBundle);
            }
        }
        public int LoadAssetAsync(AssetBundle assetBundle, string assetName, string assetPath)
        {
            if (assetBundle == null)
            {
                Debug.LogError("error:LoadAB is null __assetName:" + assetName + "  path:" + assetPath);
            }
            idIndex++;
            var coroutine = StartCoroutine(OnLoadAssetAsync(idIndex, assetBundle, assetName, assetPath, null));
            coroutineDic[idIndex] = coroutine;
            return idIndex;
        }

        IEnumerator OnLoadAssetAsync(int index, AssetBundle assetBundle, string assetName, string assetPath, System.Type systemTypeInstance)
        {
            AssetBundleRequest request = null;
            if (systemTypeInstance == null)
            {
                request = assetBundle.LoadAssetAsync(assetName);
            }
            else
            {
                request = assetBundle.LoadAssetAsync(assetName, systemTypeInstance);
            }
            yield return request;
            coroutineDic.Remove(index);
            if (onABAssetCallBack != null)
            {
                Object asset = request.asset;
                onABAssetCallBack(assetPath, asset);
            }
        }

        public void Clear()
        {
            OnStopAllCoroutines();
            onResCallBack = null;
            onABCallBack = null;
            onABAssetCallBack = null;
            coroutineDic.Clear();
        }

        public void OnStopAllCoroutines()
        {
            StopAllCoroutines();
        }
        public void UnloadUnusedAssets()
        {
            StartCoroutine(OnUnloadUnusedAssets());
        }

        IEnumerator OnUnloadUnusedAssets()
        {
            yield return null;
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
        }

        public int ReadPersistentDataPathTexture(string path, OnCallBackSTexture2D callBack)
        {
            idIndex++;
            var coroutine = StartCoroutine(OnReadPersistentDataPathTexture(path, callBack));
            coroutineDic[idIndex] = coroutine;
            return idIndex;
        }
        IEnumerator OnReadPersistentDataPathTexture(string path, OnCallBackSTexture2D callBack)
        {
            System.Uri uri = new System.Uri(path);
            using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(uri))
            {
                yield return www.SendWebRequest();
                Texture2D tex = DownloadHandlerTexture.GetContent(www);
                if (callBack != null)
                {
                    callBack(path, tex);
                }
            }
        }

        public void StopCoroutineByIndex(int index)
        {
            if (coroutineDic.ContainsKey(index))
            {
                var coroutine = coroutineDic[index];
                if (coroutine != null)
                {
                    StopCoroutine(coroutine);
                    coroutineDic.Remove(index);
                }
            }
        }
    }
}

