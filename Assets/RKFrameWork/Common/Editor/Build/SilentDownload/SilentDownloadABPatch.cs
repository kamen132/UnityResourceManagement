using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public partial class SilentDownload
{
    public static bool MakeSilentABPatch(BuildTarget buildTarget, string env)
    {
        Dictionary<string, bool> abvs = new Dictionary<string, bool>();

        bool finalIsChange = false;
        foreach (string key in DownloadList)
        {
            abvs.Clear();
            AssetBundleBuild[] data = PreBuildSlientAB(buildTarget, env, key, abvs);
            bool isChange = false;
            bool isNewKey = IsNewKey(env, key);
            Debug.Log("isNewKey:" + env + "--" + key + "--" + isNewKey);
            if (isNewKey)
            {
                isChange = true;
                ExeSingleSilentAB(buildTarget, env, key, abvs);
                CreateSilentABInfo(buildTarget, env, key, false, data);
            }
            else
            {
                isChange = MakeSingleSilentAB(buildTarget, env, key, abvs, data);
                Debug.Log("MakeSilentABPatch:" + isChange);
            }
            if(isChange)
            {
                finalIsChange = true;
            }
        }
        return finalIsChange;
    }
    
    static bool IsNewKey(string env, string key)
    {
        string SavePath = Application.dataPath + "/../../../../SaveData/" + env;

        string versionPath = SavePath + "/version.json";
        string versionData = (string)FileManager.ReadAllText(versionPath).data;
        JSONObject versionJson = new JSONObject(versionData);
        return versionJson.HasField(key) == false;
    }
    public static bool MakeSingleSilentAB(BuildTarget buildTarget, string env, string key, Dictionary<string, bool> abvs, AssetBundleBuild[] _AssetBundleBuild)
    {
        string SavePath = Application.dataPath + "/../../../../SaveData/" + env;

        string versionPath = SavePath + "/version.json";
        string versionData = (string)FileManager.ReadAllText(versionPath).data;
        JSONObject versionJson = new JSONObject(versionData);
        int appVersion = (int)versionJson["appVersion"].i;
        int keyVersion = 0;
        if (versionJson.HasField(key))
        {
            keyVersion = (int)versionJson[key].i;
        }
        keyVersion++;
        string abPath = string.Format("{0}/SilentAB/{1}/{2}_{3}", SavePath, key, appVersion, keyVersion);
        FileManager.DeleteDirectory(abPath);
        //FileManager.CopyDirectory(PathManager.StreamingAssetsABPath, abPath);
        string[] files = FileManager.GetAllFilesInFolder(PathManager.StreamingAssetsABPath);
        foreach (string file in files)
        {
            string fileName = Path.GetFileName(file);
            if (abvs.ContainsKey(fileName))
            {
                string targetFile = abPath + "/" + fileName;
                FileManager.FileMove(file, targetFile);
            }
        }
        //string depInfoPath = string.Format("{0}/{1}_deps.txt", PathManager.StreamingAssetsABPath, key.ToLower());
        //string depInfoTargetPath = string.Format("{0}/{1}_deps.txt", abPath, key.ToLower());
        //FileManager.FileMove(depInfoPath, depInfoTargetPath);
        CreateSilentABInfo(buildTarget, env, key, true, _AssetBundleBuild);
        bool isChange = MakeDiffFileInfo(buildTarget, env, key);
        if(isChange)
        {
            versionJson.SetField(key, keyVersion);
            FileManager.WriteAllText(versionPath, versionJson.ToString());
            Debug.Log("env:" + env);
            Debug.Log("env2:" + versionJson.ToString());
            CreateDiffSlientABPatchZIP(buildTarget, env, key);
        }
        return isChange;
    }

    static bool MakeDiffFileInfo(BuildTarget buildTarget, string env, string key)
    {
        string SavePath = Application.dataPath + "/../../../../SaveData/" + env;
        FileManager.CreateDirectory(SavePath);
        string versionPath = SavePath + "/version.json";
        string versionData = (string)FileManager.ReadAllText(versionPath).data;
        JSONObject versionJson = new JSONObject(versionData);
        int appVersion = (int)versionJson["appVersion"].i;
        if(versionJson.HasField(key) == false)
        {
            return true;
        }
        int keyVersion = (int)versionJson[key].i;
        
        string lastABPath = string.Format("{0}/SilentAB/{1}/{2}_{3}", SavePath, key, appVersion, keyVersion);
        string newABPath = string.Format("{0}/SilentAB/{1}/{2}_{3}", SavePath, key, appVersion, keyVersion + 1);

        string lastJsonPath = string.Format("{0}/SilentInfos/{1}/{2}_{3}.json", SavePath, key, appVersion, keyVersion);
        string newJsonPath = string.Format("{0}/SilentInfos/{1}/{2}_{3}.json", SavePath, key, appVersion, keyVersion + 1);

        string lastJson = (string)FileManager.ReadAllText(lastJsonPath).data;
        string newJson = (string)FileManager.ReadAllText(newJsonPath).data;

        string diffABPath = string.Format("{0}/SilentABDiff/{1}/{2}_{3}/", SavePath, key, appVersion, keyVersion + 1);
        
        JSONObject lastJsonObject = new JSONObject(lastJson);
        JSONObject newJsonObject = new JSONObject(newJson);
        
        Dictionary<string, string> lastFileInfoDic = lastJsonObject.ToDictionary();
        Dictionary<string, string> nowFileInfo = newJsonObject.ToDictionary();
        bool isChange = false;
        foreach (var item in nowFileInfo)
        {
            string name = item.Key;
            string md5 = item.Value;
            if (lastFileInfoDic.ContainsKey(name) == false)
            {
                //CopyOneDiffAB(name, md5, lastABPath, diffABPath);
                Debug.Log("isChange1==" + key + name);
                isChange = true;
            }else
            {

                if (lastFileInfoDic[name].Equals(md5) == false)
                {
                    //CopyOneDiffAB(name, md5, lastABPath, diffABPath);
                    Debug.Log("isChange2==" + key + name);
                    isChange = true;
                }
            }
        }
        Debug.Log("xxx isChange==" + key +  isChange);
        if (isChange == false)
        {
            return false;
        }
        //string diffABJsonInfoPath = string.Format("{0}/SilentABDiffInfo/{1}/{2}_{3}.json", SavePath, key, appVersion, keyVersion + 1);
        //FileManager.CreateDirectory(PathUtil.GetFilesParentFolder(diffABJsonInfoPath));
        //FileManager.WriteAllText(diffABJsonInfoPath, resultJson.ToString());
        return true;
    }
    static void CopyOneDiffAB(string name, string md5, string abDir, string diffPath)
    {
        FileManager.CreateDirectory(diffPath);
        string fromFilePath = Path.Combine(abDir, name);
        string targetFilePath = Path.Combine(diffPath, name);
        FileManager.CopyFile(fromFilePath, targetFilePath);
    }

    static void CreateDiffSlientABPatchZIP(BuildTarget buildTarget, string env, string key)
    {
        string SavePath = Application.dataPath + "/../../../../SaveData/" + env;

        string versionPath = SavePath + "/version.json";
        string versionData = (string)FileManager.ReadAllText(versionPath).data;
        JSONObject versionJson = new JSONObject(versionData);
        int appVersion = (int)versionJson["appVersion"].i;
        int keyVersion = (int)versionJson[key].i;
        if (keyVersion == 0)
        {
            return;
        }

        string diffABPath = string.Format("{0}/SilentAB/{1}/{2}_{3}", SavePath, key, appVersion, keyVersion);
        string nowABJsonInfoPath = string.Format("{0}/SilentInfos/{1}/{2}_{3}.json", SavePath, key, appVersion, keyVersion);
        JSONObject infoIndexs = new JSONObject();
        JSONObject infoMd5s = new JSONObject();

        for (int i = keyVersion - 1; i >= (keyVersion - 1 - BuildWindows.MaxPatchCount); i--)
        {
            if (i < 0)
            {
                break;
            }
            string filePath = string.Format("{0}/SilentInfos/{1}/{2}_{3}.json", SavePath, key, appVersion, i);

            Dictionary<string, string> diffInfo = MakeABPatchInfo(nowABJsonInfoPath, filePath);
            string zipPath = string.Format("{0}/SilentABZip/{1}/{2}_{3}/{4}_{5}.zip", SavePath, key, appVersion, keyVersion, keyVersion, i);
            MakeABPatchZip(diffInfo, zipPath, diffABPath, env);
            long size = FileUtils.GetFileSize(zipPath);
            infoIndexs.SetField(i.ToString(), size);
            infoMd5s.SetField(i.ToString(), FileUtils.GetMD5HashFromFile(zipPath));
        }

        string nowZipPath = string.Format("{0}/SilentABZip/{1}/{2}_{3}/{4}_{5}.zip", SavePath, key, appVersion, keyVersion, keyVersion, keyVersion);
        Debug.Log("CreateDiffSlientABPatchZIP---" + diffABPath + "--" + nowZipPath);
        FileUtils.CompressZip(diffABPath, nowZipPath, 8, null);
        long nowSize = FileUtils.GetFileSize(nowZipPath);
        infoIndexs.SetField(keyVersion.ToString(), nowSize);
        infoMd5s.SetField(keyVersion.ToString(), FileUtils.GetMD5HashFromFile(nowZipPath));

        versionJson.SetField(key + "_info", infoIndexs);
        versionJson.SetField(key + "_info_md5", infoMd5s);
        FileManager.WriteAllText(versionPath, versionJson.ToString());
    }
    static Dictionary<string, string> MakeABPatchInfo(string newABJsonPath, string oldABJsonPath)
    {
        Debug.Log("MakeABPatchInfo=1==" + newABJsonPath);
        Debug.Log("MakeABPatchInfo=2==" + oldABJsonPath);
        Dictionary<string, string> result = new Dictionary<string, string>();
        string oldDataStr = (string)FileManager.ReadAllText(oldABJsonPath).data;
        JSONObject oldDataJson = new JSONObject(oldDataStr);

        string newDataStr = (string)FileManager.ReadAllText(newABJsonPath).data;
        JSONObject newDataJson = new JSONObject(newDataStr);
        Dictionary<string, string> oldFileInfoDic = oldDataJson.ToDictionary();
        Dictionary<string, string> nowFileInfoDic = newDataJson.ToDictionary();
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
                if (oldFileInfoDic[name].Equals(md5) == false)
                {
                    result[name] = md5;
                }
            }
        }
        return result;
    }

    static void MakeABPatchZip(Dictionary<string, string> diffInfo, string savezipPath, string diffABPath, string env)
    {
        string fonder = PathUtil.GetFilesParentFolder(savezipPath);
        string fileName = Path.GetFileNameWithoutExtension(savezipPath);
        string tempDir = fonder + "/" + fileName;
        FileManager.CreateDirectory(tempDir);
        foreach (var item in diffInfo)
        {
            string path = diffABPath + "/" + item.Key;
            string targetPath = tempDir + "/" + item.Key;
            Debug.Log("MakeABPatchZip===" + path + "--" + targetPath);
            FileManager.CopyFile(path, targetPath);
        }
        Debug.Log("CompressZip===" + tempDir + "--" + savezipPath);
        FileUtils.CompressZip(tempDir, savezipPath, 8, null);
        FileManager.DeleteDirectory(tempDir);
    }
}
