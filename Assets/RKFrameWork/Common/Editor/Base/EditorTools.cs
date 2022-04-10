using System.IO;
using UnityEditor;
using UnityEngine;

public class EditorTools
{
    [MenuItem("Tools/Clear all caches", false, 1)]
    static void ClearDownloadAndDataCahces()
    {
        PlayerPrefs.DeleteAll();
        string path = PathManager.GetRootDataPath;
        if (Directory.Exists(path))
        {
            Directory.Delete(path, true);
        }
    }
    [MenuItem("Tools/Open Base DIR", false, 2)]
    static void OpenBaseFolder()
    {
        string FolderPath = PathManager.GetRootDataPath;
        System.Diagnostics.Process.Start(FolderPath);
    }

    //[MenuItem("Tools/Compress pngs", false, 4)]
    static void CompressPngs()
    {
        UnityEngine.Object[] arr = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.TopLevel);
        for (int i = 0; i < arr.Length; i++)
        {
            string path = Application.dataPath + "/../" + AssetDatabase.GetAssetPath(arr[i]);
            if (Directory.Exists(path))
            {
                DirectoryInfo direction = new DirectoryInfo(path);
                FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);
                for (int j = 0; j < files.Length; j++)
                {
                    FileInfo item = files[j];
                    if (item.Name.EndsWith(".png"))
                    {
                        OnCompressPngs(item.FullName);
                    }
                }
            }
            else
            {
                OnCompressPngs(path);
            }
        }
        AssetDatabase.Refresh();
    }

    public static void ClearAssetBundlesName()
    {
        int length = AssetDatabase.GetAllAssetBundleNames().Length;
        string[] oldAssetBundleNames = new string[length];
        for (int i = 0; i < length; i++)
        {
            oldAssetBundleNames[i] = AssetDatabase.GetAllAssetBundleNames()[i];
        }
        for (int j = 0; j < oldAssetBundleNames.Length; j++)
        {
            AssetDatabase.RemoveAssetBundleName(oldAssetBundleNames[j], true);
        }
    }
    public static BuildTarget GetBuildTargetByName(string name)
    {
        BuildTarget target = BuildTarget.Android;
        if (name == "ios")
        {
            target = BuildTarget.iOS;
        }
        else if (name == "android")
        {
            target = BuildTarget.Android;
        }
        else
        {
            target = BuildTarget.StandaloneWindows;
        }
        return target;
    }


    public static void OnCompressPngs(string path)
    {
#if UNITY_EDITOR_OSX
        string shell = Application.dataPath + "/../../command/pngquant/pngquant.sh";
        CommandLineTools.MacCommand(shell, path);
#elif UNITY_EDITOR_WIN
        string commond = Application.dataPath + "/../../command/pngquant/pngquant.exe";
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.Append(@"--force --verbose --ordered --speed=1 ");
        //sb.Append(@"--quality=98-100 ");
        sb.Append(@"--quality=95-100 ");
        sb.Append(@" --ext=.png ");
        sb.Append(@path);

        sb.Replace("/", "\\");
        CommandLineTools.WinCommand(commond, sb.ToString());
#endif
    }
    public static void SetPlatformTextureSettings(TextureImporter importer, string platform, int maxTextureSize, TextureImporterFormat textureFormat, bool allowsAlphaSplitting)
    {
        TextureImporterPlatformSettings platfromSettings = new TextureImporterPlatformSettings();
        platfromSettings.name = platform;
        platfromSettings.maxTextureSize = maxTextureSize;
        platfromSettings.format = textureFormat;
        platfromSettings.compressionQuality = 50;
        platfromSettings.allowsAlphaSplitting = allowsAlphaSplitting;
        platfromSettings.overridden = true;
        importer.SetPlatformTextureSettings(platfromSettings);
    }
    //[MenuItem("Tools/生成Lua配置")]
    //private static void XlsToLua()
    //{
    //    string path = Application.dataPath + "/../../command/xls2lua/";
    //    path = path.Replace("/", "\\");
    //    CreateShellExProcess("xls2lua.sh", string.Empty, path);
    //    UnityEngine.Debug.Log("所生成的配置位于 <b><color=green>Assets/Resources/appdata/LuaScripts/Data</color></b>\n如果生成失败可能是由于：\n未安装Python2.7；\nxls文件路径错误；");
    //    AssetDatabase.Refresh();
    //}

}