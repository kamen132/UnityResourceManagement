using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public partial class SilentDownload
{
    public static string[] DownloadList = new string[] { "DazzlingPin", "UIClub" , "UILounge" };
    static List<string> allShaderFiles = new List<string>();
    static List<string> allTmpFiles = new List<string>();
    static AssetBundleBuild shaderAssetBundleBuild;
    static AssetBundleBuild tmpAssetBundleBuild;

    public static void GetABNames(string key, Dictionary<string, string> newABNameDic, Dictionary<string, string> depsInfo)
    {
        Dictionary<string, Dictionary<string, bool>> allDepsDic = new Dictionary<string, Dictionary<string, bool>>();
        Dictionary<string, bool> allNeedFiles = CollectAllNeedFiles(key);

        foreach (var abFile in allNeedFiles)
        {
            string assetFilePath = PathUtil.GetAssetPath(abFile.Key);
            string[] deps = AssetDatabase.GetDependencies(assetFilePath, true);
            allDepsDic[assetFilePath] = new Dictionary<string, bool>();
            foreach (string dep in deps)
            {
                if (dep.Contains("Package") == false
                    && dep.EndsWith(".cs") == false)
                {
                    allDepsDic[assetFilePath][dep] = true;
                }
            }
        }

        BuildABCommand.RemoveSubPrefab(allDepsDic);
        Dictionary<string, string> result = BuildABCommand.GenABNames(allDepsDic);

        Debug.Log("总资源数为 " + result.Count);
        
        BuildABCommand.OnDoABName(result, newABNameDic, depsInfo);
        List<string> removeKeys = new List<string>();
        foreach(var item in newABNameDic)
        {
            if(item.Key.Contains(key.ToLower()) == false)
            {
                removeKeys.Add(item.Key);
            }
        }
        foreach(string item in removeKeys)
        {
            newABNameDic.Remove(item);
            depsInfo.Remove(item);
        }
    }
    static AssetBundleBuild[] PreBuildSlientAB(BuildTarget buildTarget, string env, string key, Dictionary<string, bool> abvs)
    {
        var startTime = CommonUtils.GetTickCount();
        Dictionary<string, string> newABNameDic = new Dictionary<string, string>();
        Dictionary<string, string> depsInfo = new Dictionary<string, string>();
        GetABNames(key, newABNameDic, depsInfo);
        OnDoAnotherABName(key, ref newABNameDic, ref depsInfo);
        //MakeDepInfo(key, newABNameDic);

        //MakeShaderAB();
        //MakeTMPAB();

        AssetBundleBuild[] data = MakeAssetBundleBuild(newABNameDic, abvs);
        //FileManager.DeleteDirectory(PathManager.StreamingAssetsABPath);
        //FileManager.CreateDirectory(PathManager.StreamingAssetsABPath);
        //BuildABCommand.MakeAssetBundles(data, buildTarget, PathManager.StreamingAssetsABPath);
        //RemoveRedundantResources();
        
        Debug.Log("OnStartBuildSlienatAB OVER use time " + (CommonUtils.GetTickCount() - startTime));
        return data;
    }
    
    public static void ExeSilentAB(BuildTarget buildTarget, string env)
    {
        Dictionary<string, bool> abvs = new Dictionary<string, bool>();

        foreach(string key in DownloadList)
        {
            abvs.Clear();
            var data = PreBuildSlientAB(buildTarget, env, key, abvs);
            ExeSingleSilentAB(buildTarget, env, key, abvs);
            CreateSilentABInfo(buildTarget, env, key, false, data);
        }
        
    }
    public static void ExeSingleSilentAB(BuildTarget buildTarget, string env, string key, Dictionary<string, bool> abvs)
    {
        string SavePath = Application.dataPath + "/../../../../SaveData/" + env;
        
        string versionPath = SavePath + "/version.json";
        string versionData = (string)FileManager.ReadAllText(versionPath).data;
        JSONObject versionJson = new JSONObject(versionData);
        int appVersion = (int)versionJson["appVersion"].i;
        int keyVersion = 0;
        versionJson.SetField(key, keyVersion);
       
        string abPath = string.Format("{0}/SilentAB/{1}/{2}_{3}", SavePath, key, appVersion, keyVersion);

        string[] files = FileManager.GetAllFilesInFolder(PathManager.StreamingAssetsABPath);
        foreach(string file in files)
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
        //FileManager.CopyDirectory(PathManager.StreamingAssetsABPath, abPath);

        string zipPath =  string.Format("{0}/SilentABZip/{1}/{2}_{3}/{4}_{5}.zip", SavePath, key, appVersion, keyVersion, keyVersion, keyVersion);
        FileManager.CreateDirectory(PathUtil.GetFilesParentFolder(zipPath));
        FileUtils.CompressZip(abPath, zipPath, 8, null);

        JSONObject canPatchVersionList = new JSONObject();
        long size = FileUtils.GetFileSize(zipPath);
        canPatchVersionList.SetField(keyVersion.ToString(), size);
        versionJson.SetField(key + "_info", canPatchVersionList);
        FileManager.WriteAllText(versionPath, versionJson.ToString());
    }
    static void CreateSilentABInfo(BuildTarget buildTarget, string env, string key, bool isUpdate, AssetBundleBuild[] _AssetBundleBuild)
    {
        string SavePath = Application.dataPath + "/../../../../SaveData/" + env;
        string versionPath = SavePath + "/version.json";
        string versionData = (string)FileManager.ReadAllText(versionPath).data;
        JSONObject versionJson = new JSONObject(versionData);
        int appVersion = (int)versionJson["appVersion"].i;
        int keyVersion = 0;
        if(versionJson.HasField(key))
        {
            keyVersion = (int)versionJson[key].i;
        }
        if (isUpdate)
        {
            keyVersion++;
        }
        string fileInfoPath = SavePath + "/SilentInfos/" + key;
        FileManager.CreateDirectory(fileInfoPath);
        string abPath = string.Format("{0}/SilentAB/{1}/{2}_{3}", SavePath, key, appVersion, keyVersion);
        Debug.Log("CreateSilentABInfo=abPath=" + abPath);
        JSONObject json = new JSONObject();
        foreach (AssetBundleBuild build in _AssetBundleBuild)
        {
            JSONObject item = new JSONObject();
            string[] assetNames = build.assetNames;
            string md5s = ExportTools.GetFilesMD5(assetNames);
            json.AddField(build.assetBundleName, md5s);
        }
        //string[] allFiles = FileManager.GetAllFilesInFolder(abPath);
        //foreach (string files in allFiles)
        //{
        //    JSONObject item = new JSONObject();
        //    string filePath = PathUtil.Replace(files);
        //    string md5 = FileUtils.GetMD5HashFromFile(filePath);
        //    string localPath = filePath.Replace(abPath + "/", "");
        //    item.AddField("p", localPath);
        //    item.AddField("md5", md5);
        //    json.Add(item);
        //}
        string jsonPath = string.Format("{0}/SilentInfos/{1}/{2}_{3}.json", SavePath, key, appVersion, keyVersion);
        Debug.Log("CreateSilentABInfo=jsonPath=" + jsonPath);
        FileManager.WriteAllText(jsonPath, json.ToString());
    }
    private static Dictionary<string, bool> CollectAllNeedFiles(string key)
    {
        var abfiles = new Dictionary<string, bool>(1024 * 64);
        foreach (var item in JenkinsConfig.NeedPackDirectorys)
        {
            string endSuffix = item.Key;
            List<string> directoryList = item.Value;
            foreach (var directoryPath in directoryList)
            {
                foreach (var path in Directory.GetFiles(Path.Combine(Application.dataPath, directoryPath), endSuffix, SearchOption.AllDirectories))
                {
                    if (path.ToLower().Contains(key.ToLower()))
                    {
                        abfiles.Add(path, true);
                    }

                }
            }
        }
        return abfiles;
    }

    private static void OnDoAnotherABName(string key, ref Dictionary<string, string> newABNameDic, ref Dictionary<string, string> depsInfo)
    {
        MakeAllSpine(key, ref newABNameDic, ref depsInfo);
        MakeSeparatePNG(key, ref newABNameDic, ref depsInfo);
    }
    private static void MakeAllSpine(string key, ref Dictionary<string, string> newABNameDic, ref Dictionary<string, string> depsInfo)
    {
        string spinePath = Path.Combine(Application.dataPath, "WorkAssets/UISpine/" + key);
        if(FileManager.FileExist(spinePath) == false)
        {
            return;
        }
        DirectoryInfo dir = new DirectoryInfo(spinePath);
        OnMakeAllSpine(dir, newABNameDic, depsInfo);
    }
    private static void OnMakeAllSpine(DirectoryInfo dir, Dictionary<string, string> newABNameDic, Dictionary<string, string> depsInfo)
    {
        if (BuildABCommand.IsHasSubDir(dir))
        {
            DirectoryInfo[] subDirs = dir.GetDirectories();
            foreach (var subDir in subDirs)
            {
                OnMakeAllSpine(subDir, newABNameDic, depsInfo);
            }

            if (BuildABCommand.IsHasFiles(dir))
            {
                MakeSpineAB(dir, newABNameDic, depsInfo);
            }
        }
        else
        {
            MakeSpineAB(dir, newABNameDic, depsInfo);
        }
    }

    private static void MakeSpineAB(DirectoryInfo info, Dictionary<string, string> newBbNameDic, Dictionary<string, string> depsInfo)
    {
        string spineABName = info.Name + "_uispine.ab";
        spineABName = spineABName.ToLower();
        FileInfo[] subInfos = info.GetFiles();
        foreach (var subInfo in subInfos)
        {
            string subName = subInfo.FullName;
            if (ExportTools.IsReal(subName))
            {
                string path = PathUtil.Replace(subName);
                string key = PathUtil.GetAssetPath(path);
                key = key.ToLower();
                newBbNameDic[key] = spineABName;
                depsInfo[key] = spineABName;
            }
        }
    }
    private static void MakeSeparatePNG(string key, ref Dictionary<string, string> newBbNameDic, ref Dictionary<string, string> depsInfo)
    {
        string FontsPath = Path.Combine(Application.dataPath, "WorkAssets/textures_separate/" + key);
        string[] _allFiles = FileManager.GetAllFilesInFolder(FontsPath);
        foreach (var filePath in _allFiles)
        {
            if (filePath.EndsWith(".mat"))
            {
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                string assetPath = PathUtil.GetAssetPath(filePath);
                string abName = fileName + ".ab";
                abName = abName.ToLower();

                string[] deps = AssetDatabase.GetDependencies(assetPath);
                foreach (var dep in deps)
                {
                    if (dep.EndsWith(".mat") || dep.EndsWith(".png"))
                    {
                        string depAssetPath = PathUtil.GetAssetPath(dep);
                        depAssetPath = depAssetPath.ToLower();

                        //if (newBbNameDic.ContainsKey(depAssetPath))
                        //{
                        //    continue;
                        //}
                        newBbNameDic[depAssetPath] = abName;
                        depsInfo[depAssetPath] = abName;
                    }
                }
            }
        }
    }

    private static void MakeDepInfo(string key, Dictionary<string, string> newABNameDic)
    {
        JSONObject JsonObject = new JSONObject();
        foreach (var item in newABNameDic)
        {
            string assetPath = item.Key;
            string abName = item.Value;
            string resPath = PathUtil.RemoveWorkAssetsPath(assetPath);
            JsonObject.AddField(resPath, abName);
        }
        FileManager.CreateDirectory(Application.streamingAssetsPath);
        string depInfoPath = string.Format("{0}/{1}_deps.txt", PathManager.StreamingAssetsABPath, key.ToLower());
        FileManager.DeleteFile(depInfoPath);
        byte[] dataBytes = Encoding.UTF8.GetBytes(JsonObject.ToString());
        dataBytes = AESManager.Encrypt(YQPackageManagerEX.KEY, dataBytes);
        FileManager.WriteAllBytes(depInfoPath, dataBytes);
    }

    private static AssetBundleBuild[] MakeAssetBundleBuild(Dictionary<string, string> newABNameDic, Dictionary<string, bool> abvs)
    {
        var builds = new Dictionary<string, List<string>>();
        foreach (var v in newABNameDic)
        {
            string key = Path.GetFileName(v.Key);
            string abname = v.Value.ToLower();
            List<string> assetpaths;
            if (!builds.TryGetValue(abname, out assetpaths))
            {
                assetpaths = new List<string>();
                builds[abname] = assetpaths;
            }
            builds[abname].Add(v.Key);

            if (!abvs.ContainsKey(abname))
                abvs.Add(abname, true);
        }

        Debug.Log("总共AB数量 " + abvs.Count);

        var index = 0;
        var assetBundleBuilds = new AssetBundleBuild[builds.Count];
        foreach (var item in builds)
        {
            AssetBundleBuild assetBundleBuild = new AssetBundleBuild();
            assetBundleBuild.assetBundleName = item.Key;
            item.Value.Sort();
            var assets = item.Value.ToArray();
            assetBundleBuild.assetNames = assets;
            assetBundleBuilds[index++] = assetBundleBuild;
        }
        //assetBundleBuilds[assetBundleBuilds.Length - 2] = shaderAssetBundleBuild;
        //assetBundleBuilds[assetBundleBuilds.Length - 1] = tmpAssetBundleBuild;
        return assetBundleBuilds;
    }
    static void RemoveRedundantResources()
    {
        string shaderABPath = PathManager.StreamingAssetsABPath + "/shader.ab";
        FileManager.DeleteFile(shaderABPath);
        string machinesABPath = PathManager.StreamingAssetsABPath + "/max";
        FileManager.DeleteFile(machinesABPath);
        string tmpfontsABPath = PathManager.StreamingAssetsABPath + "/tmpfonts.ab";
        FileManager.DeleteFile(tmpfontsABPath);
    }
}
