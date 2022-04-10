using Majic.CM;
using UnityEngine;


public enum ResourceType
{
    Prefab,
	Material,
	Shader,
	Texture,
	TextAsset,
    AudioClip,
	Other
}

public delegate void AsyncLoadCallback(IAsyncResourceRequest request);

public interface ILongRunTask
{
    bool isCompleted
    {
        get;
    }

    float progress
    {
        get;
    }

    void Dispose();
}

public interface IAsyncResourceRequest
{
	string id
	{
		get;
	}

	AsyncLoadCallback callback
	{
		get;
		set;
	}

	bool isDone
	{
		get;
	}

	Object asset
	{
		get;
	}

	void Dispose();
}

public class ResourceFacade
{
    public const string PREFAB_PATH = "Prefabs/{0}.prefab";
    private static ResourceFacade _instance;
	public delegate bool FilterAssetDeleagate(string assetId);
	public FilterAssetDeleagate needFilterPreload;
	//private static List<Object> _autoUnloadList = new List<Object>();

    public static ResourceFacade instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new ResourceFacade();
            }
            return _instance;
        }
    }

	public static Object Instantiate(Object original)
	{
        return Object.Instantiate(original);
	}

	public static void Destroy(Object obj)
	{
        _instance.DoDestroy(obj);
	}

	public static void Destroy(Object obj, float t)
	{
        _instance.DoDestroy(obj, t);
	}




    public bool HasPrefab(string localPath)
    {
       
        return ResourcesManager.Instance.HasObject(string.Format(PREFAB_PATH, localPath));
    }

    //, string prefix = "", bool prewarm = false
    public Object LoadObject(string localPath, ResourceType type = ResourceType.Other)
    {
        Object obj = ResourcesManager.Instance.LoadResSync(localPath);

        if (type == ResourceType.Prefab)
        {
            //return CreateObject(res);
        }
        if(localPath.Contains(".") == false)
        {
            Debug.LogError("xlr===LoadObject:  " + localPath);
        }
        
        return obj;
    }

    public GameObject LoadPrefab(string localPath, bool prewarm = false)
    {
        Object obj = ResourcesManager.Instance.LoadResSync(string.Format(PREFAB_PATH, localPath));
        if (prewarm == false)
        {
            return Object.Instantiate(obj) as GameObject;
        }
        return obj as GameObject;
    }

	public GameObject LoadPrefabWithoutInstantiate(string localPath)
	{
        return LoadObject(localPath) as GameObject;
    }


    public Texture2D LoadTexture2D(string localPath)
    {
        return LoadObject(localPath + ".png", ResourceType.Texture) as Texture2D;
    }

    public TextAsset LoadTextAsset(string localPath)
    {
        return LoadObject(localPath + ".bytes", ResourceType.TextAsset) as TextAsset;
    }

    public Material LoadMaterial(string localPath)
    {
        return LoadObject(localPath + ".mat", ResourceType.Material) as Material;
    }

    public int LoadPrefabAsync(string localPath, LoadResCallBack callBack)
    {
        return LoadObjectAsync(string.Format(PREFAB_PATH, localPath), callBack);
    }
    public int LoadObjectAsync(string localPath, LoadResCallBack  callBack)
    {
        if (localPath.Contains(".") == false)
        {
            Debug.LogError("xlr===LoadObjectAsync:  " + localPath);
        }

        return ResourcesManager.Instance.LoadResAsync(localPath, callBack);
        //ResourceType resType = ResourceType.Other;
        //bool create = false;
        //if (type == typeof(GameObject))
        //{
        //    create = true;
        //    resType = ResourceType.Prefab;
        //}
        //else if (type == typeof(Material))
        //{
        //    resType = ResourceType.Material;
        //}
        //else if (type == typeof(Shader))
        //{
        //    resType = ResourceType.Shader;
        //}
        //else if (type == typeof(TextAsset))
        //{
        //    resType = ResourceType.TextAsset;
        //}
        //else if (type == typeof(Texture))
        //{
        //    resType = ResourceType.Texture;
        //}
        //else if (type == typeof(AudioClip))
        //{
        //    resType = ResourceType.AudioClip;
        //}
        //string name = prefix + localPath;
        //AsyncResourceLoadRequest request = new AsyncResourceLoadRequest(name, create, prewarm);
        //_asyncContextList.Add(new AsyncLoadRequestContext(name, resType, request));
        //Resource res =  ResourceManager.Instance.LoadAsync(name, resType, request.OnAsyncLoadComplete);  

        //return request;
    }


    public void Unload(Object obj)
    {
        DoDestroy(obj);
    }
    public void Unload(GameObject gameObject)
    {
        DoDestroy(gameObject);
    }
    public void Unload(Texture2D texture2D)
    {
        DoDestroy(texture2D);
    }
    public void Unload(TextAsset textAsset)
    {
        DoDestroy(textAsset);
    }
    public void Unload(Material material)
    {
        DoDestroy(material);
    }

	public void UnloadPrewarmedPrefab(string path)
	{
	}



    private void DoDestroy(Object obj, float delayTime = 0)
    {

        //if (obj.GetType() == typeof(GameObject))
        //{
        //    if (delayTime > 0)
        //    {
        //        Object.Destroy(obj, delayTime);
        //    }
        //    else
        //    {
        //        Object.Destroy(obj);
        //    }
        //}
    }

    
}
