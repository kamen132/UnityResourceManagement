using System.IO;
using UnityEngine;

public class PathManager
{
    public const string zipCacheSub = ".cmtemp";
    public const string zipDirectorySub = "_cmtemp";
    //public static string PackSourcePath = Application.dataPath + "/Resources";//pack 的源目录
    //public static string unPackSourcePath = Application.dataPath + "/Resources/unpack";//unpack 的源目录
    //public static string EncryptedDatapath = Application.dataPath + "/Resources/appdata/res/data";//加密数据 的源目录
    //public static string Packsoundpath = Application.dataPath + "/Resources/appdata/res/un_pack_sound";

    public static string PACKAGE_FILE_SIGN = "PictureBook";//打资源包时候用到

    //public static string unpackPath = "unpack";//resource下，不打包的路径

    //public static string base_lua_scripts_Path = "LuaScripts";
    //public static string BaseLuaPath = Application.dataPath + "/../LuaScripts";
    //public static string StreamingAssetsLuaPath = Application.streamingAssetsPath + "/LuaScripts";

    //public static string BasePBPath = Application.dataPath + "/../LuaScripts/LuaPb";

    public static string streamingAssetsPath = Application.streamingAssetsPath;
    public static string persistentDataPath = Application.persistentDataPath;
    public static string StreamingAssetsVersionPath = streamingAssetsPath + "/version.json";

    //public static string StreamingAssetsPBPath = Application.streamingAssetsPath + "/lua.pb";
    //public static string StreamingAssetsUnpackLuaPath = Application.streamingAssetsPath + "/unpack_lua.txt";
    //public static string StreamingAssetsNeedPackLuaPath = Application.streamingAssetsPath + "/need_pack_lua.txt";
    public static string StreamingAssetsABPath = streamingAssetsPath + "/max";
    //public static string StreamingAssetsMachinesABPath = Application.streamingAssetsPath + "/machines";
    //public static string StreamingAssetsDepInfoPath = Application.streamingAssetsPath + "/dps.txt";

    private static string PersistentDataUnpackLuaPath = string.Empty;
    private static string PersistentDataNeedPackLuaPath = string.Empty;
    //public static string PersistentDataLuaPatchPath = Application.persistentDataPath + "/LuaPatch";

    //public const string save_sound_path_name = "un_pack_sound";
    //public const string download_unpackLuaName = "unpack_info.info";//下载的 unpack lua data name

    public static string SALocaleLanguage = streamingAssetsPath + "/Config/Common/Locale_{0}.bytes";
    public static string SADllBytesPath = streamingAssetsPath + "/HotFixProjectDll.bytes";
    public static string SADllPDBPath = streamingAssetsPath + "/HotFixProject.pdb";
    public static string SAMetaPath = streamingAssetsPath + "/Config/Common/Meta.bytes";

    private static string downLoadABPath = streamingAssetsPath;

    private static string PDABPath = null;
    private static string PDDepInfoPath = null;
    private static string PDManifestPath = null;
    public static void OnUpdate()
    {
        downLoadABPath = streamingAssetsPath;
        PDABPath = downLoadABPath + "/max/";
        PDDepInfoPath = downLoadABPath + "/dps.txt";
        PDManifestPath = downLoadABPath + "/max/max";
    }
    public static void UpdatePersistentDataUnpackLuaPath(string AppVersion)
    {
        PersistentDataUnpackLuaPath = Application.persistentDataPath + "/AB_" + AppVersion + "/unpack_lua.txt";
    }

    public static string GetPersistentDataUnpackLuaPath()
    {
        return PersistentDataUnpackLuaPath;
    }

    public static void UpdatePersistentDataNeedPackLuaPath(string AppVersion)
    {
        PersistentDataNeedPackLuaPath = Application.persistentDataPath + "/AB_" + AppVersion + "/need_pack_lua.txt";
    }

    public static string GetPersistentDataNeedPackLuaPath()
    {
        return PersistentDataNeedPackLuaPath;
    }
    public static string GetPDDepInfoPath()
    {
        return PDDepInfoPath;
    }

    public static string GetPDManifestPath()
    {
        return PDManifestPath;
    }

    public static string GetPDABPath()
    {
        return PDABPath;
    }
    public static string GetSAABPath()
    {
        return StreamingAssetsABPath;
    }
    public static string GetRootDataPath
    {
        get
        {
            string dir = Path.Combine(Application.persistentDataPath);

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            return dir;
        }
    }



}
