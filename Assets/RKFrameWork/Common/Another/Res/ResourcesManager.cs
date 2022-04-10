using System.Collections.Generic;
using UnityEngine;

namespace Majic.CM
{
    //    string path, Object obj, int guid
    public delegate void LoadResCallBack(string path, Object obj, int guid);
    public class ResourcesManager : MonoBehaviour
    {
        private class LoadingTask
        {
            public int idIndex;
            public int guid;
            public string path;
            public LoadResCallBack callBack;
        }
        private int guid = 0;
        private int loadingn = 0;
        private int maxloadingn = 999999;

        private Dictionary<string, LoadingTask> loadingTasksDic = new Dictionary<string, LoadingTask>();
        private Dictionary<string, int> assetRefs = new Dictionary<string, int>();
        private Dictionary<string, Object> assetDic = new Dictionary<string, Object>();
        private Dictionary<Object, int> assetObjDic = new Dictionary<Object, int>();

        private List<LoadingTask> loadTasks = new List<LoadingTask>();
        private List<LoadingTask> cacheTasks = new List<LoadingTask>();
        private Dictionary<string, Object> resDic = new Dictionary<string, Object>();
        private Dictionary<int, string> guid2name = new Dictionary<int, string>();

        public bool HasObject(string path)
        {
            return assetDic.ContainsKey(path);
        }
        private int NewGuid(string assetName)
        {
            guid++;
            return guid;
        }

        private void AddRef(string assetName, int refn = 1)
        {
            if(assetRefs.ContainsKey(assetName) ==false)
            {
                assetRefs[assetName] = 0;
            }
            assetRefs[assetName] += refn;
        }
        private void LoadFinish(string path, Object asset)
        {
            if(asset == null)
            {
                Debug.LogError("xlr Load asset error:" + path);
                return;
            }
            loadingTasksDic.Remove(path);
            assetDic[path] = asset;
            assetObjDic[asset] = 1;
            int newCount = loadTasks.Count;
            if(newCount > 0)
            {
                for(int i = newCount - 1; i >= 0;i --)
                {
                    LoadingTask itemTask = loadTasks[i];
                    if(itemTask != null && itemTask.path.Equals(path))
                    {
                        loadTasks.RemoveAt(i);
                        cacheTasks.Add(itemTask);
                    }
                }
            }
            int count = cacheTasks.Count;
            if(count > 0)
            {
                for (int i = count - 1; i >= 0; i--)
                {
                    LoadingTask itemTask = cacheTasks[i];
                    cacheTasks.RemoveAt(i);
                    if (MacroDefinition.UNITY_EDITOR())
                    {
                        CommonUtils.ReplaceShaders(asset);
                    }
                    itemTask.callBack(path, asset, itemTask.guid);
                }
            }
        }
        private void GetABNameByKey(string key, string abNameResult, string pathResult)
        {
            string abName = YQPackageManagerEX.Instance.GetABName(key);
            if(string.IsNullOrEmpty(abName) == false)
            {
                string path = PathManager.GetPDABPath() + abName;
                if(FileManager.FileExist(path) == false)
                {
                    path = PathManager.GetSAABPath() + abName;
                }
                pathResult = path;
                abNameResult = abName;
            }
        }

        //isNoLoaded: 是否还没有加载，，没有进行task
        private bool DeRef(string assetName, bool isNoLoaded)
        {
            int count = 0;
            if(assetRefs.ContainsKey(assetName))
            {
                count = assetRefs[assetName];
            }
            count = count - 1;
            assetRefs[assetName] = count;
            if(count > 0)
            {
                return true;
            }
            assetRefs.Remove(assetName);
            Object obj = assetDic[assetName];
            assetDic.Remove(assetName);
            assetObjDic.Remove(obj);
            if (BaseConfig.RES_FROM_AB && isNoLoaded)
            {
                string lowerPath = assetName.ToLower();

                string abName = null;
                string abFullPath = null;
                GetABNameByKey(lowerPath, abName, abFullPath);
                AssetBundleManager.Instance.UnloadAB(abName, abFullPath, assetName, true);
            }
            return false;
        }
        private LoadingTask GetOneLoadTask()
        {
            int count = loadTasks.Count;
            for (int i = 0; i < count; i++)
            {
                LoadingTask task = loadTasks[i];
                if (loadingTasksDic.ContainsKey(task.path) == false)
                {
                    return task;
                }
            }
            return null;
        }
        private void LoadResAsyncNext()
        {
            if(loadingn > maxloadingn)
            {
                return;
            }
            int count = loadTasks.Count;
            if(count == 0)
            {
                return;
            }
            LoadingTask task = GetOneLoadTask();
            if (task == null)
            {
                return;
            }
            string path = task.path;

            int idIndex = -1;
            if(BaseConfig.RES_FROM_AB)
            {
                AssetBundleManager.Instance.LoadABResAsync(path);
            }else
            {
                idIndex = AssetLoadController.Instance.LoadResourceAsync(path);
            }
            LoadingTask loadingTask = new LoadingTask();
            loadingTask.idIndex = idIndex;
            loadingTask.guid = task.guid;
            loadingTasksDic[path] = loadingTask;
        }
        public void StopLoadResAsyncByGUID(int guid)
        {
            int newCount = loadTasks.Count;

            for (int i = newCount - 1; i >= 0; i--)
            {
                LoadingTask itemTask = loadTasks[i];
                if (itemTask != null && itemTask.guid == guid)
                {
                    loadTasks.RemoveAt(i);
                }
            }
            var itor = loadingTasksDic.GetEnumerator();
            while (itor.MoveNext())
            {
                var value = itor.Current;
                LoadingTask loadingTask = value.Value;
                if (loadingTask != null && loadingTask.guid == guid)
                {
                    int idIndex = loadingTask.idIndex;
                    AssetLoadController.Instance.StopCoroutineByIndex(idIndex);
                    loadingTasksDic.Remove(value.Key);
                    break;
                }
            }
            LoadResAsyncNext();
        }

        public Object LoadResSync(string path)
        {
            Object asset = null;
            int guid = NewGuid(path);
            guid2name[guid] = path;
            if (BaseConfig.RES_FROM_AB)
            {
                AssetBundle assetBundle = AssetBundleManager.Instance.LoadAssetBundleSync(AssetBundleManager.AssetFront +path);

                asset = assetBundle.LoadAsset(path);
                assetDic[path] = asset;
                AddRef(path, 1);
            }
            else
            {
                asset = AssetLoadController.Instance.LoadResSync(path);
                if(asset == null)
                {
                    Debug.LogError("xlr==LoadResSync null:" + path);
                }
                assetDic[path] = asset;
                AddRef(path, 1);
            }
            if (MacroDefinition.UNITY_EDITOR())
            {
                CommonUtils.ReplaceShaders(asset);
            }
            return asset;
        }
        public int LoadResAsync(string path, LoadResCallBack callBack)
        {
            if(assetDic.ContainsKey(path))
            {
                AddRef(path, 1);
                int guid = NewGuid(path);
                guid2name[guid] = path;
                callBack(path, assetDic[path], guid);
                return guid;
            }
            LoadingTask loadTask = new LoadingTask();
            loadTask.path = path;
            loadTask.callBack = callBack;
            loadTask.guid = NewGuid(path); ;
            loadTasks.Add(loadTask);
            LoadResAsyncNext();
            guid2name[guid] = path;
            AddRef(path, 1);
            return guid;
        }
        public void Cleanup()
        {
            loadingn = 0;
            guid = 1;
            maxloadingn = 999999;
            assetDic.Clear();
            assetRefs.Clear();
            loadTasks.Clear();
            loadingTasksDic.Clear();
            guid2name.Clear();
            cacheTasks.Clear();
            assetObjDic.Clear();
        }
        private void OnLoadAssetCallBack(string callBackPath, Object asset)
        {
            LoadFinish(callBackPath, asset);
            LoadResAsyncNext();
        }

        public void UnloadGuid(int guid)
        {
            if(guid2name.ContainsKey(guid) == false)
            {
                return;
            }
            string path = guid2name[guid];
            guid2name.Remove(guid);
            int newCount = loadTasks.Count;
            if(newCount > 0)
            {
                for (int i = newCount - 1; i >= 0; i--)
                {
                    LoadingTask itemTask = loadTasks[i];
                    if (itemTask != null && itemTask.guid == guid)
                    {
                        loadTasks.RemoveAt(i);
                        DeRef(path, true);
                    }
                }
            }
            DeRef(path, false);
        }
        private void UpdateLoadingCount()
        {
            int level = DeviceLevelManager.Instance.GetDeviceLevel();
            if(level == 1)
            {
                maxloadingn = 4;
            }else if(level == 2)
            {
                maxloadingn = 6;
            }
            else if (level == 3)
            {
                maxloadingn = 8;
            }
        }

        public void Startup()
        {
            loadingn = 0;
            maxloadingn = 999999;
            guid = 1;
            UpdateLoadingCount();
            if(BaseConfig.RES_FROM_AB)
            {
                AssetBundleManager.Instance.RejeistAction();

                AssetLoadController.Instance.OnABAssetCallBackAction(OnLoadAssetCallBack);
            }else
            {
                AssetLoadController.Instance.OnResCallBackAction(OnLoadAssetCallBack);
            }
        }



        private static volatile ResourcesManager _ResourcesManager;

        public static ResourcesManager Instance
        {
            get
            {
                return _ResourcesManager;
            }
        }
        private void Awake()
        {
            _ResourcesManager = this;
        }
    }
}

