using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Majic.CM
{
    public class AssetBundleManager : MonoBehaviour
    {
        public class LoadingABTask
        {
            public string fromABPath;
            public string path;
            public int taskCount;
        }
        public class AssetBundleInfo
        {
            public int refValue = 1;
            public AssetBundle abAsset = null;
        }
        public const string AssetFront = "Assets/WorkAssets/";
        private Dictionary<string, string[]> depsDic = new Dictionary<string, string[]>();
        private Dictionary<string, Dictionary<string, int>> loadingABRef = new Dictionary<string, Dictionary<string, int>>();
        private List<LoadingABTask> loadingABTaskList = new List<LoadingABTask>();
        private string[] GetABDependencies(string abName)
        {
            if (depsDic.ContainsKey(abName) == false)
            {
                //        result = YQPackageManagerEX.mBaseManifest:GetAllDependencies(abName)

                //        self.depsDic[abName] = result;
            }
            return depsDic[abName];
        }
        private Dictionary<string, AssetBundleInfo> loadedABDic = new Dictionary<string, AssetBundleInfo>();
        private void SaveAssetBundle(string abFullPath, AssetBundle ab)
        {
            if(loadedABDic.ContainsKey(abFullPath))
            {
                loadedABDic[abFullPath].refValue += 1;
            }else
            {
                AssetBundleInfo info = new AssetBundleInfo();
                info.refValue = 1;
                info.abAsset = ab;
            }
        }
        private AssetBundle GetLoadedAB(string fullABPath, bool includeDyingAB = false)
        {
            if (loadedABDic.ContainsKey(fullABPath))
            {
                AssetBundleInfo info = loadedABDic[fullABPath];
                SaveAssetBundle(fullABPath, info.abAsset);
                return info.abAsset;
            }else
            {
                if(includeDyingAB)
                {
                    AssetBundle dyingAB = AssetUnLoadManager.Instance.TryGetAB(fullABPath);
                    if(dyingAB == null)
                    {
                        return null;
                    }else
                    {
                        SaveAssetBundle(fullABPath, dyingAB);
                        return dyingAB;
                    }
                }
            }
            return null;
        }
        private string GetRealABPath(string name)
        {
            string fullABPath = PathManager.GetPDABPath() + name;
            bool exist = FileManager.FileExist(fullABPath);

             if( exist == false)
            {
                fullABPath = PathManager.GetSAABPath() + name;
            }
            return fullABPath;
        }

        public AssetBundle LoadAssetBundleSync(string abName)
        {
            string lowerPath = abName.ToLower();
            string needFullABPath = GetRealABPath(abName);
            AssetBundle needLoadedAsset = GetLoadedAB(needFullABPath);
            if (needLoadedAsset == null)
            {
                string[] dependencies = GetABDependencies(lowerPath);
                if(dependencies != null)
                {
                    int count = dependencies.Length;
                    for( int i = 0; i <  count;i++)
                    {
                        string depABName = dependencies[i];
                        string fullABPath = GetRealABPath(depABName);
                        AssetBundle loadedAsset = GetLoadedAB(fullABPath);

                        if(loadedAsset == null)
                        {
                            AssetBundle ab = AssetLoadController.Instance.LoadAssetBundleSync(fullABPath);
                            SaveAssetBundle(fullABPath, ab);
                        }
                    }
                }
            }
            string abPath = GetRealABPath(abName);
            AssetBundle asset = AssetLoadController.Instance.LoadAssetBundleSync(abPath);
            SaveAssetBundle(abPath, asset);
            return asset;
        }
        
        //--fromABPath 主ab
        //--fullABPath 依赖ab
        private void OnLoadAB(string fromABPath, string fullABPath)
        {
            if(loadingABRef.ContainsKey(fullABPath))
            {
                var loadingABRefItem = loadingABRef[fullABPath];
                loadingABRefItem[fromABPath] = 1;
            }else
            {
                Dictionary<string, int> loadingABRefItem = new Dictionary<string, int>();

                loadingABRefItem[fromABPath] = 1;

                loadingABRef[fullABPath] = loadingABRefItem;
                AssetLoadController.Instance.LoadAssetBundleAsync(fullABPath);
            }
        }

        private void LoadAssetBundleWithCallBackAsync(string fromABPath, OnCallBackSObject callBack = null)
        {
            AssetLoadController.Instance.LoadAssetBundleAsyncWithCallBack(fromABPath, (string callBackPath, Object asset) =>
            {
                SaveAssetBundle(callBackPath, asset as AssetBundle);
                if (callBack != null)
                {
                    callBack(callBackPath, asset);
                }
            }
            );
        }
        //-可以同时启用多个load ab ,应该能够保证path不会相同
        public void LoadABResAsync(string path)
        {
            if(string.IsNullOrEmpty(path))
            {
                return;
            }
            int taskCount = 0;

            string lowerPath = path.ToLower();

            string abName = YQPackageManagerEX.Instance.GetABName(lowerPath);
            if(string.IsNullOrEmpty(abName))
            {
                Debug.LogError("cant find abName= by path =" + lowerPath);
                return;
            }
            string fromABPath = GetRealABPath(abName);

            AssetBundle loadedAsset = GetLoadedAB(fromABPath);
            if(loadedAsset != null)
            {
                AssetLoadController.Instance.LoadAssetAsync(loadedAsset, AssetFront + path, path);
                return;
            }
            string[] dependencies = GetABDependencies(abName);
            if(dependencies != null)
            {
                int count = dependencies.Length;
                for(int  i = 0; i <  count;i++)
                {
                    string depABName = dependencies[i];

                    string fullABPath = GetRealABPath(depABName);

                    AssetBundle loadedAssetDep = GetLoadedAB(fullABPath, true);
                    if(loadedAssetDep == null)
                    {
                        OnLoadAB(fromABPath, fullABPath);
                        taskCount = taskCount + 1;
                    }
                }
            }
            if(taskCount == 0)//依赖全都没卸载
            {
                bool isHasDyingAB = AssetUnLoadManager.Instance.IsHasDyingAB(fromABPath);
                if(isHasDyingAB)//-主ab也没卸载
                {
                    AssetBundle loadedDying = GetLoadedAB(fromABPath, true);

                    AssetLoadController.Instance.LoadAssetAsync(loadedDying, AssetFront + path, path);

                    return;
                }else //依赖部分卸载，那也要同时卸载主ab
                {
                    AssetBundle loadedDying = AssetUnLoadManager.Instance.TryGetAB(fromABPath);
                    if(loadedDying != null)
                    {
                        loadedDying.Unload(true);
                    }
                }
                taskCount = taskCount + 1;
                OnLoadAB(fromABPath, fromABPath);

                LoadingABTask task = new LoadingABTask();
                task.fromABPath = fromABPath;
                task.path = path;
                task.taskCount = taskCount;
                loadingABTaskList.Add(task);
            }
        }
        //--会存在，加载同一个不同path，但是同一个ab，不存在同一个ab，同一个path
        public void RejeistAction()
        {
            //--每一个依赖ab加载完之后，都要给主ab ref + 1
            AssetLoadController.Instance.OnABCallBackAction((string callBackPath, Object obj) =>
            {
                if(loadingABRef.ContainsKey(callBackPath) == false)
                {
                    return;
                }
                AssetBundle asset = obj as AssetBundle;
                Dictionary<string, int> loadingABRefItem = loadingABRef[callBackPath];//记录的是所有的ab
                var itor = loadingABRefItem.GetEnumerator();
                while (itor.MoveNext())
                {
                    var iten = itor.Current;
                    string taskPath = iten.Key;
                    SaveAssetBundle(callBackPath, asset);
                    int newCount = loadingABTaskList.Count;
                    for(int i = newCount - 1; i >= 0;i --)
                    {
                        LoadingABTask itemTask = loadingABTaskList[i];
                        if(itemTask.fromABPath.Equals(taskPath))
                        {
                            itemTask.taskCount -= 1;
                            if(itemTask.taskCount == 0) //当依赖加载完之后再加载ab中的东西
                            {
                                string path = itemTask.path;
                                loadingABTaskList.RemoveAt(i);
                                AssetBundle abAsset = loadedABDic[taskPath].abAsset;
                                if(abAsset == null)
                                {
                                    //debugErrorLog("abAsset== nil", path)
                                }else
                                {
                                    AssetLoadController.Instance.LoadAssetAsync(abAsset, AssetFront + path, path);
                                }
                            }
                        }
                        
                    }
                }
                loadingABRef.Remove(callBackPath);
            });
        }

        public void UnloadAB(string abName, string abFullPath, string assetName, bool isNeedCheckDeps)
        {
            if(string.IsNullOrEmpty(abName) || string.IsNullOrEmpty(abFullPath) || string.IsNullOrEmpty(assetName))
            {
                return;
            }
            if(loadedABDic.ContainsKey(abFullPath) == false)
            {
                return;
            }
            AssetBundleInfo item = loadedABDic[abFullPath];
            item.refValue -= 1;
            if(item.refValue == 0)
            {
                loadedABDic.Remove(abFullPath);
                AssetUnLoadManager.Instance.AddAB(abFullPath, item.abAsset);
                if(isNeedCheckDeps)//依赖的依赖不卸载
                {
                    string[] dependencies = GetABDependencies(abName);
                    int count = dependencies.Length;
                    for(int i = 0; i < count;i++)
                    {
                        string depABName = dependencies[i];
                        string fullABPath = GetRealABPath(depABName);
                        UnloadAB(depABName, fullABPath, null, false);
                    }
                }
            }
        }

        public int GetABCount()
        {
            return loadedABDic.Count;
        }

        public void Cleanup()
        {
            var itor = loadedABDic.GetEnumerator();
            while (itor.MoveNext())
            {
                AssetBundle abAsset = itor.Current.Value.abAsset;
                if(abAsset != null)
                {
                    abAsset.Unload(true);
                }
            }
            depsDic.Clear();

            loadedABDic.Clear();
            loadingABRef.Clear();
            loadingABTaskList.Clear();
        }
        private static volatile AssetBundleManager _AssetBundleManager;
      

        private Dictionary<int, Coroutine> coroutineDic = new Dictionary<int, Coroutine>();
        public static AssetBundleManager Instance
        {
            get
            {
                return _AssetBundleManager;
            }
        }

        private void Awake()
        {
            _AssetBundleManager = this;
        }

      
    }
}

