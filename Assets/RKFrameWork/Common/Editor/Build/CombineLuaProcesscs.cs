using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class CombineLuaProcesscs
{
    private static List<string> luaPathList = new List<string>();
    private static Dictionary<string, byte[]> luaData = new Dictionary<string, byte[]>();
    private static bool isContainsUnpackPath = false;

    //public static void CopyLuaToStreamingAssets()
    //{
    //    FileManager.CopyDirectory(PathManager.BaseLuaPath, PathManager.StreamingAssetsLuaPath);
    //}
    //public static void RMLuaToStreamingAssets()
    //{
    //    FileManager.DeleteDirectory(PathManager.StreamingAssetsLuaPath);
    //}
    public static void Clear()
    {
        luaPathList.Clear();
        luaData.Clear();
    }
    public static void PushLua(string fromPath, bool _isContainsUnpackPath)
    {
        isContainsUnpackPath = _isContainsUnpackPath;
        if(FileManager.DirectoryExist(fromPath))
        {
            PackLua(fromPath);
        }
        
    }

    //默认是不包括unpack path
    public static void CombineLua(string fromPath, string targetPath, bool _isContainsUnpackPath)
    {
        isContainsUnpackPath = _isContainsUnpackPath;
        luaPathList.Clear();
        luaData.Clear();
        PackLua(fromPath);
        FileLua(targetPath);
    }

    static void ReadLua()
    {
        //string fileName = "lua_" + BuildConfig.PACKAGE_NAME + BuildConfig.RES_VERSION;
        //string filePath = Path.Combine(BuildConfig.ResourceOutPutPath, BuildConfig.BuildTarget + "/" + fileName);
        //FileCatchData _FileCatchData = FileManager.ReadAllBytes(filePath);
        //byte[] data = (byte[])_FileCatchData.data;
        //Debug.Log("data   " + data.Length / 1024);
    }

    static void PackLua(string path)
    {
        if(FileManager.DirectoryExist(path) == false)
        {
            return;
        }
        //DirectoryInfo folder = new DirectoryInfo(path);
        //FileSystemInfo[] files = folder.GetFileSystemInfos();
        //int length = files.Length;
        //for (int i = 0; i < length; i++)
        //{
        //    FileSystemInfo item = files[i];
        //    if (item == null)
        //    {
        //        continue;
        //    }
        //    string fullName = item.FullName;
        //    if (fullName.EndsWith(".svn") == false)
        //    {
        //        bool isDirectoryInfo = item is DirectoryInfo;
        //        if (isDirectoryInfo && isContainsUnpackPath == false && fullName.Contains(PathManager.unpackPath))
        //        {
        //            continue;
        //        }
        //        if (isDirectoryInfo)
        //        {
        //            PackLua(fullName);
        //        }
        //        else
        //        {
        //            string endName = Path.GetExtension(fullName);
        //            if (endName.EndsWith(".lua") && endName.Contains("LuaDebug") == false)
        //            {
        //                luaPathList.Add(fullName);
        //            }
        //        }
        //    }
        //}
    }

    //static void CollectLuaData()
    //{
    //    int count = luaPathList.Count;
    //    for (int i = 0; i < count; i++)
    //    {
    //        string path = luaPathList[i];
    //        path = FileManager.RemoveFileExtension(path);
    //        string key = GetLuaScriptsPath(path);
    //        FileCatchData _FileCatchData = FileManager.ReadAllBytes(path);
    //        byte[] data = (byte[])_FileCatchData.data;
    //        data = AESManager.Encrypt(YQPackageManagerEX.KEY, data);
    //        luaData[key] = data;
    //    }
    //}

    public static void FileLua(string targetPath)
    {
        string dirPath = Path.GetDirectoryName(targetPath);
        FileManager.CreateDirectory(dirPath);
        int count = luaPathList.Count;
        Debug.Log("xxx FileLua===" + count);
        StringBuilder testStr = new StringBuilder();
        if (count == 0)
        {
            Debug.Log("error, no lua file");
            return;
        }
        try
        {
            FileStream fileStream = new System.IO.FileStream(targetPath, System.IO.FileMode.Append);
            FileUtils.WriteInt(fileStream, count);
            for (int i = 0; i < count; i++)
            {
                string path = luaPathList[i];
                path = PathUtil.Replace(path);
                string key = PathUtil.RemoveFileExtension(path);
                key = GetLuaScriptsPath(key);
                key = key.Replace("/", ".");
                key = key.ToLower();
                testStr.Append(key);
                testStr.Append("----");
                FileUtils.WriteString(fileStream, key);
                FileCatchData _FileCatchData = FileManager.ReadAllBytes(path);
                byte[] data = (byte[])_FileCatchData.data;
                data = AESManager.Encrypt(YQPackageManagerEX.KEY, data);
                FileUtils.WriteInt(fileStream, data.Length);
                fileStream.Write(data, 0, data.Length);
            }
            fileStream.Close();
        }
        catch (Exception e)
        {
            Debug.Log("e====" + e);
        }
        //Debug.Log("testStr.ToString()====" + testStr.ToString());
        //string NamesPath = Path.Combine(dirPath, "names.txt");
        //FileManager.DeleteFile(NamesPath);
        //FileManager.WriteAllText(NamesPath, testStr.ToString());
        //return filePath;
        //Debug.Log("dirPath==" + dirPath);
        //System.Diagnostics.Process.Start(Path.GetFullPath(dirPath));
    }
    static string GetLuaScriptsPath(string fullPath)
    {
        //Debug.Log("fullPath===" + fullPath);
        if (string.IsNullOrEmpty(fullPath))
        {
            return null;
        }
        return null;
        //string subName = PathManager.base_lua_scripts_Path;
        //int startIndex = fullPath.LastIndexOf(subName);
        //if (startIndex == -1)
        //{
        //    return null;
        //}
        //startIndex = startIndex + PathManager.base_lua_scripts_Path.Length + 1;
        //string sourceAssetPath = fullPath.Substring(startIndex);
        //sourceAssetPath = PathUtil.Replace(sourceAssetPath);

        //return sourceAssetPath;
    }
}
