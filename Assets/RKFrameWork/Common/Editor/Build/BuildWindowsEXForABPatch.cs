using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public partial class BuildWindows
{
    public static int MaxPatchCount = 20;
    public static int TestMaxPatchCount = 5;

    static bool MakeABDiff(BuildTarget buildTarget, string env, AssetBundleBuild[] _AssetBundleBuild, bool isSilentABChange)
    {
        Debug.Log("xxx MakeABDiff===");
        string SavePath = Application.dataPath + "/../../../../SaveData/" + env;
        FileManager.CreateDirectory(SavePath);
        string versionPath = SavePath + "/version.json";
        if (FileManager.FileExist(versionPath) == false)
        {
            string fromPath = Application.dataPath + "/../../command/build/version.json";
            FileManager.CopyFile(fromPath, versionPath);
        }
        string versionData = (string)FileManager.ReadAllText(versionPath).data;
        JSONObject versionJson = new JSONObject(versionData);
        int appVersion = (int)versionJson["appVersion"].i;
        int abVersion = (int)versionJson["abVersion"].i;
        string baseResPath = SavePath + "/AB/" + appVersion + "_0";
        abVersion++;
        string resPath = SavePath + "/AB/" + appVersion + "_" + abVersion.ToString();
        FileManager.DeleteDirectory(resPath);
        versionJson.SetField("abVersion", abVersion);

        Debug.Log("xxx abVersion===" + abVersion);
        FileManager.DeleteDirectory(resPath);
        FileManager.CreateDirectory(resPath);
        FileManager.CopyDirectory(PathManager.StreamingAssetsABPath, resPath + "/max");
        //FileManager.CopyFile(PathManager.StreamingAssetsDepInfoPath, resPath + "/dps.txt");
        //FileManager.CopyFile(PathManager.StreamingAssetsPBPath, resPath + "/lua.pb");
        //FileManager.CopyFile(PathManager.StreamingAssetsUnpackLuaPath, resPath + "/unpack_lua.txt");
        //FileManager.CopyFile(PathManager.StreamingAssetsNeedPackLuaPath, resPath + "/need_pack_lua.txt");
        ExportTools.RemoveManifest(resPath);
        ExportTools.RemoveMeta(resPath);
        CreateFileInfo(buildTarget, resPath, env, _AssetBundleBuild);
        bool isChange = MakeDiffFileInfo(buildTarget, env);
        Debug.Log("xxx isChange==" + isChange);
        if (isChange || isSilentABChange)
        {
            FileManager.WriteAllText(versionPath, versionJson.ToString());
            CreateDiffABPatchZIP(buildTarget, env, isChange, isSilentABChange);
        }
        return isChange == true || isSilentABChange;
    }

    static void CreateFileInfo(BuildTarget buildTarget, string resPath, string env, AssetBundleBuild[] _AssetBundleBuild)
    {
        string SavePath = Application.dataPath + "/../../../../SaveData/" + env;
        string fileInfoPath = SavePath + "/FileInfo";
        FileManager.CreateDirectory(fileInfoPath);

        JSONObject json = new JSONObject();
        foreach (AssetBundleBuild build in _AssetBundleBuild)
        {
            JSONObject item = new JSONObject();
            string[] assetNames = build.assetNames;
            string md5s = ExportTools.GetFilesMD5(assetNames);
            json.AddField(build.assetBundleName, md5s);
        }
        string fileinfoName = fileInfoPath + "/" + PathUtil.GetFolderName(resPath) + ".json";
        FileManager.WriteAllText(fileinfoName, json.ToString());
    }
    static bool MakeDiffFileInfo(BuildTarget buildTarget, string env)
    {
        string SavePath = Application.dataPath + "/../../../../SaveData/" + env;
        FileManager.CreateDirectory(SavePath);
        string versionPath = SavePath + "/version.json";
        string versionData = (string)FileManager.ReadAllText(versionPath).data;
        JSONObject versionJson = new JSONObject(versionData);
        int appVersion = (int)versionJson["appVersion"].i;
        int abVersion = (int)versionJson["abVersion"].i;

        abVersion = abVersion + 1;

        string abDir = SavePath + "/AB/" + appVersion + "_" + abVersion;
        string baseFileInfoJsonPath = SavePath + "/FileInfo/" + appVersion + "_0.json";
        string targetFileInfoJsonPath = SavePath + "/FileInfo/" + appVersion + "_" + abVersion + ".json";
        //string targetDiffPath = SavePath + "/DiffFiles/" + appVersion + "_" + abVersion;


        string baseFileInfoJsonStr = (string)FileManager.ReadAllText(baseFileInfoJsonPath).data;
        string targetFileInfoJsonStr = (string)FileManager.ReadAllText(targetFileInfoJsonPath).data;

        JSONObject baseFileInfoJson = new JSONObject(baseFileInfoJsonStr);
        JSONObject targetFileInfoJson = new JSONObject(targetFileInfoJsonStr);
        Dictionary<string, string> baseFileInfoDic = baseFileInfoJson.ToDictionary();
        Dictionary<string, string> targetFileInfo = targetFileInfoJson.ToDictionary();
        //bool isChange = false;
        JSONObject resultJson = new JSONObject();

        foreach (var item in targetFileInfo)
        {
            string name = item.Key;
            string md5 = item.Value;
            if (name.Equals("need_pack_lua.txt") || IsSilentAB(name))
            {
                continue;
            }
            if (baseFileInfoDic.ContainsKey(name) == false)
            {
                //CreateOneDiff(resultJson, name, md5, abDir, targetDiffPath);
                Debug.Log("isChange1==" + name);
                //isChange = true;
                return true;
            }
            else
            {
                if (baseFileInfoDic[name].Equals(md5) == false)
                {
                    //CreateOneDiff(resultJson, name, md5, abDir, targetDiffPath);
                    Debug.Log("isChange2==" + name);
                    return true;
                }
            }
        }
        bool isChange1 = IsOtherChange(SavePath, appVersion, abVersion, abVersion - 1, "lua.pb");
        bool isChange2 = IsOtherChange(SavePath, appVersion, abVersion, abVersion - 1, "unpack_lua.txt");
        //bool isChange3 = IsOtherChange(SavePath, appVersion, abVersion, abVersion - 1, "need_pack_lua.txt");
        return isChange1 || isChange2;
    }

    static bool IsOtherChange(string SavePath, int appVersion, int nowABVersion, int oldABVersion, string name)
    {
        string oldUnPackPath = string.Format("{0}/AB/{1}_{2}/{3}", SavePath, appVersion, oldABVersion, name);
        string nowUnPackPath = string.Format("{0}/AB/{1}_{2}/{3}", SavePath, appVersion, nowABVersion, name);
        string oldMd5 = FileUtils.GetMD5HashFromFile(oldUnPackPath);
        string nowMd5 = FileUtils.GetMD5HashFromFile(nowUnPackPath);
        return oldMd5.Equals(nowMd5) == false;
    }

    static void CreateOneDiff(JSONObject resultJson, string name, string md5, string abDir, string targetDiffPath)
    {
        JSONObject itemJson = new JSONObject();
        itemJson.AddField("p", name);
        itemJson.AddField("m", md5);
        //itemJson.AddField("s", item["size"].i);
        resultJson.Add(itemJson);
        FileManager.CreateDirectory(targetDiffPath);
        string fromFilePath = Path.Combine(abDir, name);
        string targetFilePath = Path.Combine(targetDiffPath, name);
        string fonder = PathUtil.GetFilesParentFolder(targetFilePath);
        FileManager.CreateDirectory(fonder);
        FileManager.CopyFile(fromFilePath, targetFilePath);
    }

    static void CreateDiffABPatchZIP(BuildTarget buildTarget, string env, bool isChange, bool isSilentABChange)
    {
        string SavePath = Application.dataPath + "/../../../../SaveData/" + env;
        FileManager.CreateDirectory(SavePath);
        string versionPath = SavePath + "/version.json";
        string versionData = (string)FileManager.ReadAllText(versionPath).data;
        JSONObject versionJson = new JSONObject(versionData);
        int appVersion = (int)versionJson["appVersion"].i;
        int abVersion = (int)versionJson["abVersion"].i;
        if (abVersion == 0)
        {
            return;
        }
        string abDir = SavePath + "/AB/" + appVersion + "_" + abVersion;

        string fileInfoJsonPath = SavePath + "/FileInfo/" + appVersion;
        string nowFileInfoJsonPath = fileInfoJsonPath + "_" + abVersion + ".json";

        string saveDir = SavePath + "/ABPatchZip/" + appVersion + "_" + abVersion;


        JSONObject abPatchIndexs = new JSONObject();
        JSONObject abPatchMd5s = new JSONObject();

        bool isAllBuild = false;
        for (int i = abVersion - 1; i >= (abVersion - 1 - MaxPatchCount); i--)
        {
            if (i < 0)
            {
                break;
            }
            if (i == 0)
            {
                isAllBuild = true;
            }
            string filePath = fileInfoJsonPath + "_" + i + ".json";

            Dictionary<string, string> diffInfo = MakeABPatchInfo(nowFileInfoJsonPath, filePath);

            string zipPath = saveDir + "/" + appVersion + "_" + abVersion + "-" + i + ".zip";
            CheckOtherChange(SavePath, appVersion, abVersion, i, zipPath);
            MakeABPatchZip(diffInfo, zipPath, abDir, isChange, isSilentABChange);
            long size = FileUtils.GetFileSize(zipPath);
            abPatchIndexs.SetField(i.ToString(), size);
            abPatchMd5s.SetField(i.ToString(), FileUtils.GetMD5HashFromFile(zipPath));
        }
        if (isSilentABChange || isChange)//强制更新dps
        {
            //FileManager.CopyFile(PathManager.StreamingAssetsDepInfoPath, abDir + "/dps.txt");
        }
        if (isChange)
        {
            FileManager.CopyFile(PathManager.StreamingAssetsABPath + "/max", abDir + "/max/max");
        }
        if (isAllBuild == false)
        {
            string savezipPath = saveDir + "/" + appVersion + "_" + abVersion + "-0.zip";
            FileUtils.CompressZip(abDir, savezipPath, 8, null);
            long size0 = FileUtils.GetFileSize(savezipPath);
            abPatchIndexs.SetField("0", size0);
            abPatchMd5s.SetField("0", FileUtils.GetMD5HashFromFile(savezipPath));
        }
        versionJson.SetField("abPatchIndexs", abPatchIndexs);
        versionJson.SetField("abPatchMd5s", abPatchMd5s);
        versionJson.SetField("savezipPath", abPatchMd5s);
        FileManager.WriteAllText(versionPath, versionJson.ToString());
    }

    static Dictionary<string, string> MakeABPatchInfo(string nowABJsonPath, string oldABJsonPath)
    {
        Dictionary<string, string> result = new Dictionary<string, string>();

        string oldDataStr = (string)FileManager.ReadAllText(oldABJsonPath).data;
        JSONObject oldDataJson = new JSONObject(oldDataStr);

        string nowDataStr = (string)FileManager.ReadAllText(nowABJsonPath).data;
        JSONObject nowDataJson = new JSONObject(nowDataStr);

        Dictionary<string, string> nowFileInfoDic = nowDataJson.ToDictionary();
        Dictionary<string, string> oldFileInfoDic = oldDataJson.ToDictionary();
        foreach (var item in nowFileInfoDic)
        {
            string name = item.Key;
            string md5 = item.Value;
            if (oldFileInfoDic.ContainsKey(name) == false)
            {
                result[name] = md5;
            }
            else
            {
                string oldMd5 = oldFileInfoDic[name];
                if (oldMd5.Equals(md5) == false)
                {
                    Debug.Log("change ab:" + name + " old:" + oldMd5 + " new:" + md5);
                    result[name] = md5;
                }
            }
        }
        return result;
    }

    static void CheckOtherChange(string SavePath, int appVersion, int nowABVersion, int oldABVersion, string savezipPath)
    {
        string fonder = PathUtil.GetFilesParentFolder(savezipPath);
        string fileName = Path.GetFileNameWithoutExtension(savezipPath);
        string tempDir = fonder + "/" + fileName + "/";


        string unpackLua = "unpack_lua.txt";
        if (IsOtherChange(SavePath, appVersion, nowABVersion, oldABVersion, unpackLua))
        {
            string targetPath = tempDir + unpackLua;
            string nowUnPackPath = string.Format("{0}/AB/{1}_{2}/{3}", SavePath, appVersion, nowABVersion, unpackLua);
            FileManager.CopyFile(nowUnPackPath, targetPath);
        }
        string luaPb = "lua.pb";
        if (IsOtherChange(SavePath, appVersion, nowABVersion, oldABVersion, luaPb))
        {
            string targetPath = tempDir + luaPb;
            string nowPbPath = string.Format("{0}/AB/{1}_{2}/{3}", SavePath, appVersion, nowABVersion, luaPb);
            FileManager.CopyFile(nowPbPath, targetPath);
        }
    }
    static void MakeABPatchZip(Dictionary<string, string> diffInfo, string savezipPath, string abDir, bool isChange, bool isSilentABChange)
    {
        string fonder = PathUtil.GetFilesParentFolder(savezipPath);
        string fileName = Path.GetFileNameWithoutExtension(savezipPath);
        string tempDir = fonder + "/" + fileName;
        FileManager.CreateDirectory(tempDir);
        if (isChange)
        {
            foreach (var item in diffInfo)
            {
                string path = abDir + "/max/" + item.Key;
                string targetPath = tempDir + "/max/" + item.Key;
                FileManager.CopyFile(path, targetPath);
            }
            FileManager.CopyFile(PathManager.StreamingAssetsABPath + "/max", tempDir + "/max/max");//如果change必然会change max
        }
        if (isSilentABChange || isChange)//强制更新dps
        {
            //public static string StreamingAssetsDepInfoPath = Application.streamingAssetsPath + "/dps.txt";
            string dpsPath = tempDir + "/dps.txt";
            //Debug.Log("MakeABPatchZip====" + dpsPath + "---" + PathManager.StreamingAssetsDepInfoPath);
            //FileManager.CopyFile(PathManager.StreamingAssetsDepInfoPath, dpsPath);
        }
        FileUtils.CompressZip(tempDir, savezipPath, 8, null);
        //FileManager.DeleteDirectory(tempDir);
    }
    static bool IsSilentAB(string name)
    {
        foreach(string key in SilentDownload.DownloadList)
        {
            if(name.Contains(key.ToLower()))
            {
                return true;
            }
        }
        return false;
    }
    //-------------------------------------------
    static void MakeLuaPatch(BuildTarget buildTarget, string env, bool isChange)
    {
        Debug.Log("start MakeLuaPatch");
        string SavePath = Application.dataPath + "/../../../../SaveData/" + env;
        FileManager.CreateDirectory(SavePath);
        string versionPath = SavePath + "/version.json";

        string versionData = (string)FileManager.ReadAllText(versionPath).data;
        JSONObject versionJson = new JSONObject(versionData);
        int appVersion = (int)versionJson["appVersion"].i;
        //int abVersion = (int)versionJson["abVersion"].i;
        int luaVersion = (int)versionJson["luaVersion"].i;

        string lastLuaDataPath = SavePath + "/BSLuaFile/" + appVersion + "_" + luaVersion + "/need_pack_lua.txt";
        //string nowLuaDataPath = PathManager.StreamingAssetsNeedPackLuaPath;
        //string lastMD5 = FileUtils.GetMD5HashFromFile(lastLuaDataPath);
        //string nowMD5 = FileUtils.GetMD5HashFromFile(nowLuaDataPath);
        //if (lastMD5.Equals(nowMD5))
        //{
        //    return;
        //}
        //luaVersion++;
        //string copyluaDataPath = SavePath + "/BSLuaFile/" + appVersion + "_" + luaVersion + "/need_pack_lua.txt";
        //FileManager.CopyFile(nowLuaDataPath, copyluaDataPath);

        //string nowABDir = SavePath + "/AB/" + appVersion + "_" + luaVersion + 1;

        //Debug.Log("md5 change " + lastMD5 + "--" + nowMD5);

        //JSONObject luaPatchIndexs = new JSONObject();
        //JSONObject luaPatchMd5s = new JSONObject();

        //for (int i = luaVersion - 1; i >= (luaVersion - 1 - MaxPatchCount); i--)
        //{
        //    if (i < 0)
        //    {
        //        break;
        //    }

        //    string patchFilePath = MakeOneLuaPatch(appVersion, luaVersion, i, luaVersion, SavePath);
        //    long size = FileUtils.GetFileSize(patchFilePath);
        //    luaPatchIndexs.SetField(i.ToString(), size);
        //    luaPatchMd5s.SetField(i.ToString(), FileUtils.GetMD5HashFromFile(patchFilePath));
        //}

        //long nowSize = FileUtils.GetFileSize(copyluaDataPath);
        //luaPatchIndexs.SetField(luaVersion.ToString(), nowSize);
        //luaPatchMd5s.SetField(luaVersion.ToString(), FileUtils.GetMD5HashFromFile(copyluaDataPath));

        //versionJson.SetField("luaPatchIndexs", luaPatchIndexs);
        //versionJson.SetField("luaPatchMd5s", luaPatchMd5s);
        //versionJson.SetField("luaVersion", luaVersion);
        //FileManager.WriteAllText(versionPath, versionJson.ToString());
        //Debug.Log("start MakeLuaPatch over");
    }

    static string MakeOneLuaPatch(int appVersion, int luaVersion, int baseABVersion, int targetABVersion, string SavePath)
    {
        shell = Application.dataPath + "/../../command/build/BsDiff.sh";
        string patchPath = SavePath + "/BSLuaPatch/" + appVersion + "_" + luaVersion;
        FileManager.CreateDirectory(patchPath);

        string baseABPath = SavePath + "/BSLuaFile/" + appVersion + "_" + baseABVersion + "/need_pack_lua.txt";
        string targetABPath = SavePath + "/BSLuaFile/" + appVersion + "_" + targetABVersion + "/need_pack_lua.txt";

        string patchFilePath = patchPath + "/" + targetABVersion + "_" + baseABVersion;
        string args = baseABPath + " " + targetABPath + " " + patchFilePath;
        Debug.Log("args==" + args);
        CommandLineTools.MacCommand(shell, args);
        return patchFilePath;



    }

}
