

using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using System.Collections.Generic;

public class ExportTools
{
    public static void ShowSuccessDialog(bool isSuccess,string target,string exportPath )
    {
        if (isSuccess)
        {
            if (EditorUtility.DisplayDialog(target, "success", "打开导出文件夹", "关闭"))
            {
                System.Diagnostics.Process.Start(Path.GetFullPath(exportPath));
            }
        }
    }
    public static bool BuildPlayer(string exportPath, string[] BUILD_SCENES, BuildTarget buildTarget, BuildOptions _BuildOptions)
    {
        try
        {
            FileManager.DeleteFile(exportPath);
            FileManager.DeleteDirectory(exportPath);

            EditorUserBuildSettings.exportAsGoogleAndroidProject = false;
            EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;

            EditorUserBuildSettings.androidBuildSubtarget = MobileTextureSubtarget.Generic;
            EditorUserBuildSettings.androidETC2Fallback = AndroidETC2Fallback.Quality16Bit;
            BuildReport result = BuildPipeline.BuildPlayer(BUILD_SCENES, exportPath, buildTarget, _BuildOptions);
            if (result.summary.result != BuildResult.Succeeded)
            {
                Debug.Log("Build___失败" + result);
                EditorUtility.DisplayDialog("Error", "Build___失败" + result, "ok");
                return false;
            }
        }
        catch (System.Exception e)
        {
            Debug.Log("Build___失败" + e);
            EditorUtility.DisplayDialog("Error", "Build___失败" + e, "ok");
            return false;
        }
        return true;
    }

    public static void PackUnpackLua()
    {
        //string fromPath = PathManager.BaseLuaPath + "/unpack";
        //FileManager.DeleteFile(PathManager.StreamingAssetsUnpackLuaPath);
        //CombineLuaProcesscs.CombineLua(fromPath, PathManager.StreamingAssetsUnpackLuaPath, true);
    }
    public static void RemovePackUnpackLua()
    {
        //FileManager.DeleteFile(PathManager.StreamingAssetsUnpackLuaPath);
    }

    public static void PackLuaFile()
    {
        //FileManager.DeleteFile(PathManager.StreamingAssetsNeedPackLuaPath);
        //CombineLuaProcesscs.Clear();
        //string fromPath = PathManager.BaseLuaPath + "/AppCore";
        //CombineLuaProcesscs.PushLua(fromPath, false);
        //fromPath = PathManager.BaseLuaPath + "/AppLogic";
        //CombineLuaProcesscs.PushLua(fromPath, false);
        //fromPath = PathManager.BaseLuaPath + "/LuaData";
        //CombineLuaProcesscs.PushLua(fromPath, false);
        //fromPath = PathManager.BaseLuaPath + "/LuaDataB";
        //CombineLuaProcesscs.PushLua(fromPath, false);
        //fromPath = PathManager.BaseLuaPath + "/SlotsData";
        //CombineLuaProcesscs.PushLua(fromPath, false);
        //fromPath = PathManager.BaseLuaPath + "/SlotsDataB";
        //CombineLuaProcesscs.PushLua(fromPath, false);
        //CombineLuaProcesscs.FileLua(PathManager.StreamingAssetsNeedPackLuaPath);
    }

    public static void RemovePackLuaFile()
    {
        //FileManager.DeleteFile(PathManager.StreamingAssetsNeedPackLuaPath);
    }

    public static void OnCompressResourcePng()
    {
        string resourcePath = Path.Combine(Application.dataPath, "Resources");
        OnCompressPng(resourcePath);
        AssetDatabase.Refresh();
    }

    public static void OnCompressPng(string path)
    {
        path = PathUtil.Replace(path);
        DirectoryInfo direction = new DirectoryInfo(path);
        FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);

        for (int i = 0; i < files.Length; i++)
        {
            FileInfo item = files[i];
            if (item != null && item.Name.EndsWith(".png"))
            {
                EditorTools.OnCompressPngs(item.FullName);
            }
        }
    }
    
    public static void EncryptedData()
    {
        //if (FileManager.DirectoryExist(PathManager.EncryptedDatapath))
        //{
        //    string tempOutPath = Application.dataPath + "/../tempDataPath";
        //    FileManager.DeleteDirectory(tempOutPath);
        //    FileManager.CreateDirectory(tempOutPath);
        //    //将resource的unpack资源拷贝回来
        //    FileManager.CopyDirectory(PathManager.EncryptedDatapath, tempOutPath);

        //    EncryptedData(PathManager.EncryptedDatapath);
        //}
    }
    //public static void BackEncryptedData()
    //{
    //    string tempOutPath = Application.dataPath + "/../tempDataPath";
    //    FileManager.DeleteDirectory(PathManager.EncryptedDatapath);
    //    FileManager.CreateDirectory(PathManager.EncryptedDatapath);
    //    //将resource的unpack资源拷贝回来
    //    FileManager.CopyDirectory(tempOutPath, PathManager.EncryptedDatapath);
    //}
    static void EncryptedData(string path)
    {
        DirectoryInfo folder = new DirectoryInfo(path);
        FileSystemInfo[] files = folder.GetFileSystemInfos();
        int length = files.Length;
        for (int i = 0; i < length; i++)
        {
            FileSystemInfo item = files[i];
            if (item == null)
            {
                continue;
            }
            string fullName = item.FullName;
            if (fullName.EndsWith(".svn") == false)
            {
                bool isDirectoryInfo = item is DirectoryInfo;
                if (isDirectoryInfo)
                {
                    EncryptedData(fullName);
                }
                else
                {
                    string endName = Path.GetExtension(fullName);
                    if (endName.EndsWith(".txt"))
                    {
                        EncryptedDataItem(fullName);
                    }
                }
            }
        }
    }

    static void EncryptedDataItem(string path)
    {
        FileCatchData _FileCatchData = FileManager.ReadAllBytes(path);
        byte[] data = (byte[])_FileCatchData.data;
        data = AESManager.Encrypt(YQPackageManagerEX.KEY, data);
        FileManager.DeleteFile(path);
        FileManager.WriteAllBytes(path, data);
    }

    public static void PackPBFile()
    {
        //FileManager.CopyDirectory(PathManager.BasePBPath, Application.streamingAssetsPath);
        //string path = PathManager.StreamingAssetsPBPath;
        //FileCatchData _FileCatchData = FileManager.ReadAllBytes(path);
        //byte[] data = (byte[])_FileCatchData.data;
        //data = AESManager.Encrypt(YQPackageManagerEX.KEY, data);
        //FileManager.DeleteFile(path);
        //FileManager.WriteAllBytes(path, data);
    }

    public static void RemovePBInStreamingAssets()
    {
        //string path = PathManager.StreamingAssetsPBPath;
        //FileManager.DeleteFile(path);
    }

    public static void RemoveManifest(string path)
    {
        string[] _allFiles = FileManager.GetAllFilesInFolder(path);
        foreach(var item in _allFiles)
        {
            if(item.EndsWith(".manifest") || item.EndsWith(".manifest.meta"))
            {
                FileManager.DeleteFile(item);
            }
        }
    }
    public static void RemoveMeta(string path)
    {
        string[] _allFiles = FileManager.GetAllFilesInFolder(path);
        foreach (var item in _allFiles)
        {
            if (item.EndsWith(".meta"))
            {
                FileManager.DeleteFile(item);
            }
        }
    }
    public static bool IsReal(string filePath)
    {
        return filePath.EndsWith(".meta") == false &&
            filePath.EndsWith(".DS_Store") == false &&
            filePath.EndsWith(".git") == false &&
            filePath.EndsWith(".gitignore") == false &&
            filePath.EndsWith(".vscode") == false &&
            filePath.EndsWith(".cs") == false &&
             filePath.EndsWith(".svn") == false;
    }
    public static string AddVersion(string version)
    {
        if(string.IsNullOrEmpty(version))
        {
            return null;
        }
        string[]  data = version.Split('.');
        if(data.Length == 0)
        {
            return null;
        }
        int lastNum = int.Parse(data[data.Length - 1]);
        lastNum = lastNum + 1;
        StringBuilder builder = new StringBuilder();
        for(int i = 0;i < data.Length;i ++)
        {
            if(i == data.Length)
            {
                builder.Append(lastNum.ToString());
            }else
            {
                builder.Append(data[i]);
                builder.Append(".");
            }
        }
        return builder.ToString();
    }
    private static Dictionary<string, string> prefabMd5Dic = new Dictionary<string, string>();

    private static Dictionary<string, int> saveFileMd5s = new Dictionary<string, int>();
    public static string GetFilesMD5(string[] files)
    {
        StringBuilder sb = new StringBuilder();
        saveFileMd5s.Clear();
        
        for (int i = 0; i < files.Length; i++)
        {
            string filePath = files[i];
            sb.Append("-->");
            string fileMd5 = MakeOneFileMd5(filePath);
            string depsMd5 = OnMaleDepsMd5(filePath);
            sb.Append(fileMd5);
            sb.Append(depsMd5);
            sb.Append("<--");
        }
        return sb.ToString();
    }
    

    private static string GetPrefabMd5(string fullPath)
    {
        if(prefabMd5Dic.ContainsKey(fullPath))
        {
            return prefabMd5Dic[fullPath];
        }
        return MakeOneFileMd5(fullPath);
    }
    public static string OnMaleDepsMd5(string filePath)
    {
        StringBuilder sb = new StringBuilder();
        string[] deps = AssetDatabase.GetDependencies(filePath);
        foreach (string dep in deps)
        {
            if (dep.EndsWith(BuildABCommand.prefabExt))
            {
                //Debug.Log("filePath==" + filePath + "===" + dep);
                if(saveFileMd5s.ContainsKey(dep) == false)
                {
                    string md5 = GetPrefabMd5(dep);
                    sb.Append(md5);
                    saveFileMd5s[dep] = 1;
                }
               
                if (filePath.ToLower().Equals(dep.ToLower()) == false)
                {
                    string depsMd5 = OnMaleDepsMd5(dep);
                    sb.Append(depsMd5);
                }
            }
        }
        return sb.ToString();
    }
    public static string MakeOneFileMd5(string filePath)
    {
        StringBuilder sb = new StringBuilder();
        System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
        FileStream fileStream = new FileStream(filePath, FileMode.Open);
        sb.Append("-");
        sb.Append(filePath);
        sb.Append("|");
        byte[] retVal = md5.ComputeHash(fileStream);
        for (int k = 0; k < retVal.Length; k++)
        {
            sb.Append(retVal[k].ToString("x2"));
        }
        fileStream.Close();
        sb.Append("#");
        //Debug.Log("filePath==" + filePath);
        var lines = new List<string>(File.ReadAllLines(filePath + ".meta"));
        for (int j = 0; j < lines.Count; j++)
        {
            if (lines[j].Contains("timeCreated"))
            {
                lines.RemoveAt(j);
                break;
            }
        }
        var metacontent = string.Join("\n", lines.ToArray());
        retVal = md5.ComputeHash(Encoding.Default.GetBytes(metacontent));
        for (int k = 0; k < retVal.Length; k++)
        {
            sb.Append(retVal[k].ToString("x2"));
        }
        sb.Append("=");
        return sb.ToString();
    }

}
