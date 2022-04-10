using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class BuildABCommand
{
    public const string prefabExt = ".prefab";
    
    public static AssetBundleBuild[] OnStartBuildAB(BuildTarget buildTarget)
    {
        var startTime = CommonUtils.GetTickCount();
        Dictionary<string, Dictionary<string, bool>> allDepsDic = new Dictionary<string, Dictionary<string, bool>>();
        Dictionary<string, bool> allNeedFiles = CollectAllNeedFiles();

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
        RemoveSubPrefab(allDepsDic);
        Dictionary<string, string> result = GenABNames(allDepsDic);
        //foreach(var item in result)
        //{
        //    if(item.Key.ToLower().Contains("spine_ui_tasklight"))
        //    {
        //        Debug.Log("2222222");
        //    }
        //}
        Debug.Log("总资源数为 " + result.Count);
        Dictionary<string, string> newABNameDic = new Dictionary<string, string>();
        Dictionary<string, string> depsInfo = new Dictionary<string, string>();
        OnDoABName(result, newABNameDic, depsInfo);
        OnDoAnotherABName(newABNameDic, depsInfo);
        MakeDepInfo(newABNameDic);
        AssetBundleBuild[] data = MakeAssetBundleBuild(newABNameDic);
        FileManager.DeleteDirectory(PathManager.StreamingAssetsABPath);
        FileManager.CreateDirectory(PathManager.StreamingAssetsABPath);
        MakeAssetBundles(data, buildTarget, PathManager.StreamingAssetsABPath);
       
        Debug.Log("OnStartBuildAB OVER use time " + (CommonUtils.GetTickCount() - startTime));
        return data;
    }
    
    private static Dictionary<string, bool> CollectAllNeedFiles()
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
                    if(IsSlentDownloadKey(path) == false)
                    {
                        abfiles.Add(path, true);
                    }
                    
                }
            }
        }
        return abfiles;
    }

    public static void RemoveSubPrefab(Dictionary<string, Dictionary<string, bool>> resDeps)
    {
        var resDeprs = DepReverse(resDeps, false); //desp--prefab
        Dictionary<string, bool> removes = new Dictionary<string, bool>();
        foreach (var it in resDeps)// prefab-deps
        {
            if (removes.ContainsKey(it.Key) == false)
            {
                var deps = it.Value;
                if (resDeprs.ContainsKey(it.Key))
                {
                    var deprs = resDeprs[it.Key];
                    foreach (var it2 in deprs)
                    {
                        if (deps.ContainsKey(it2.Key) == false && it2.Key != it.Key)
                        {
                            removes.Add(it.Key, true);
                            break;
                        }
                    }

                }
            }
        }
        foreach (var it in removes)
        {
            resDeps.Remove(it.Key);
        }
    }

    private static Dictionary<string, Dictionary<string, bool>> DepReverse(Dictionary<string, Dictionary<string, bool>> resDeps, bool isContainsSelf)
    {
        var resDeprs = new Dictionary<string, Dictionary<string, bool>>();
        foreach (var deps in resDeps)
        {
            foreach (var it2 in deps.Value)
            {
                if(isContainsSelf == false)
                {
                    if (deps.Key == it2.Key)
                    {
                        continue;
                    }
                }
                
                if (resDeprs.ContainsKey(it2.Key) == false)
                {
                    resDeprs[it2.Key] = new Dictionary<string, bool>();
                }
                resDeprs[it2.Key][deps.Key] = true;
                
            }
        }
        return resDeprs;
    }

    public static Dictionary<string, string> GenABNames(Dictionary<string, Dictionary<string, bool>> resDeps)
    {
        var resDeprs = new Dictionary<string, List<string>>();
        foreach (var it in resDeps)
        {
            foreach (var it2 in it.Value)
            {
                if (resDeprs.ContainsKey(it2.Key) == false)
                {
                    resDeprs.Add(it2.Key, new List<string>());
                }
                resDeprs[it2.Key].Add(it.Key);
            }
        }
        var removes = new Dictionary<string, bool>();
        foreach (var it in resDeprs)
        {
            if (it.Value.Count > 1)
                it.Value.Sort();
            else
                removes.Add(it.Key, true);
        }
        var combines = new Dictionary<string, List<string>>();
        foreach (var it in resDeprs)
        {
            if (removes.ContainsKey(it.Key) == false)
            {
                var name = string.Join("|", it.Value.ToArray());
                if (combines.ContainsKey(name) == false)
                {
                    combines.Add(name, new List<string>());
                }
                combines[name].Add(it.Key);
            }
        }
        var newCombines = new Dictionary<string, List<string>>();
        foreach (var it in combines)
        {
            it.Value.Sort();
            newCombines[it.Value[0]] = it.Value;
        }

        var abNames = new Dictionary<string, string>();
        foreach (var it in newCombines)
        {
            foreach (var it2 in it.Value)
            {
                var abname = Path.GetFileName(it.Key);
                Debug.Assert(abname != "", "abname can not be empty string");
                abNames.Add(it2, abname);
            }
        }

        foreach (var it in resDeps) //最后一次过滤，那些没找到子依赖，都设置成父依赖
        {
            var abname = Path.GetFileName(it.Key);
            Debug.Assert(abname != "", "abname can not be empty string");
            foreach (var it2 in it.Value)
            {
                if (abNames.ContainsKey(it2.Key) == false)
                {
                    abNames.Add(it2.Key, abname);
                }
            }
        }
        return abNames;
    }

    public static void OnDoABName(Dictionary<string, string> abNames, Dictionary<string, string> newABNameDic, Dictionary<string, string> depsInfo)
    {
        foreach (var it in abNames)
        {
            string finalabname = null;
            var assetpath = it.Key.ToLower();
            if(NotSpecialFile(assetpath) == false)
            {
                finalabname = null;
            }else
            {
                finalabname = it.Value;
            }
            
            if (finalabname != null)
            {
                finalabname = finalabname.Replace('.', '_');
                finalabname = finalabname.ToLower() + ".ab";
                newABNameDic[assetpath] = finalabname;
                depsInfo[assetpath] = finalabname;
            }
        }
    }

    private static void OnDoAnotherABName(Dictionary<string, string> newABNameDic, Dictionary<string, string> depsInfo)
    {
        MakeTMP(newABNameDic, depsInfo);
        MakeFontAB(newABNameDic, depsInfo);
        MakeShaderAB(newABNameDic, depsInfo);
        MakeAllSpine(newABNameDic, depsInfo);
        //MakeAllSeparateTexture(ref newABNameDic, ref depsInfo);
        MakeBMFont(newABNameDic, depsInfo);
        MakeSeparatePNG(newABNameDic, depsInfo);
        MakeLoginAB(newABNameDic, depsInfo);
    }

    private static void MakeSeparatePNG(Dictionary<string, string> newBbNameDic, Dictionary<string, string> depsInfo)
    {
        string FontsPath = Path.Combine(Application.dataPath, "WorkAssets/textures_separate/");
        string[] _allFiles = FileManager.GetAllFilesInFolder(FontsPath);
        Debug.Log("MakeSeparatePNG==" + _allFiles.Length);
        foreach (var filePath in _allFiles)
        {
            if (IsSlentDownloadKey(filePath))
            {
                continue;
            }
            if (filePath.EndsWith(".mat"))
            {
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                string assetPath = PathUtil.GetAssetPath(filePath);
                string abName = fileName + ".ab";
                abName = abName.ToLower();

                string[]  deps = AssetDatabase.GetDependencies(assetPath);
                foreach (var dep in deps)
                {
                    if(dep.EndsWith(".mat") || dep.EndsWith(".png"))
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

    private static void MakeAllSpine(Dictionary<string, string> newABNameDic, Dictionary<string, string> depsInfo)
    {
        string spinePath = Path.Combine(Application.dataPath, "WorkAssets/UISpine/");
        DirectoryInfo dir = new DirectoryInfo(spinePath);
        OnMakeAllSpine(dir, newABNameDic, depsInfo);
    }

    private static void OnMakeAllSpine(DirectoryInfo dir, Dictionary<string, string> newABNameDic, Dictionary<string, string> depsInfo)
    {
        if (IsHasSubDir(dir))
        {
            DirectoryInfo[] subDirs = dir.GetDirectories();
            foreach (var subDir in subDirs)
            {
                OnMakeAllSpine(subDir, newABNameDic, depsInfo);
            }
            
            if (IsHasFiles(dir))
            {
                MakeSpineAB(dir, newABNameDic, depsInfo);
            }
        }else
        {
            MakeSpineAB(dir, newABNameDic, depsInfo);
        }
    }
    private static void MakeBMFont(Dictionary<string, string> newABNameDic, Dictionary<string, string> depsInfo)
    {
        string bmFontPath = Path.Combine(Application.dataPath, "WorkAssets/BMFont/");
        if(Directory.Exists(bmFontPath) == false)
        {
            return;
        }
        DirectoryInfo dir = new DirectoryInfo(bmFontPath);
        OnMakeBMFont(dir, newABNameDic, depsInfo);
        
    }
    private static void OnMakeBMFont(DirectoryInfo dir, Dictionary<string, string> newABNameDic, Dictionary<string, string> depsInfo)
    {
        if (IsHasSubDir(dir))
        {
            DirectoryInfo[] subDirs = dir.GetDirectories();
            foreach (var subDir in subDirs)
            {
                OnMakeBMFont(subDir, newABNameDic, depsInfo);
            }

            if (IsHasFiles(dir))
            {
                MakeBMFontItem(dir, newABNameDic, depsInfo);
            }
        }
        else
        {
            MakeBMFontItem(dir, newABNameDic, depsInfo);
        }
    }

    private static void MakeBMFontItem(DirectoryInfo info, Dictionary<string, string> newBbNameDic, Dictionary<string, string> depsInfo)
    {
        string spineABName = info.Name + "_bmfont.ab";
        spineABName = spineABName.ToLower();
        FileInfo[] subInfos = info.GetFiles();
        foreach (var subInfo in subInfos)
        {
            string subName = subInfo.FullName;
            if (ExportTools.IsReal(subName))
            {
                string key = PathUtil.GetAssetPath(subName);
                key = key.ToLower();
                newBbNameDic[key] = spineABName;
                depsInfo[key] = spineABName;
            }
        }
    }

    private static void MakeTMP(Dictionary<string, string> newBbNameDic, Dictionary<string, string> depsInfo)
    {
        string FontsPath = Path.Combine(Application.dataPath, "WorkAssets/TMPFonts/");
        DirectoryInfo dir = new DirectoryInfo(FontsPath);
        FileInfo[] allFiles = dir.GetFiles();
        string abName = "tmpfonts.ab";
        foreach (var fileInfo in allFiles)
        {
            if (ExportTools.IsReal(fileInfo.Name) == false)
            {
                continue;
            }
            string key = PathUtil.GetAssetPath(fileInfo.FullName);
            key = key.ToLower();
            newBbNameDic[key] = abName;
            depsInfo[key] = abName;
        }
    }
    
    private static void MakeFontAB(Dictionary<string, string> newBbNameDic, Dictionary<string, string> depsInfo)
    {
        string FontsPath = Path.Combine(Application.dataPath, "WorkAssets/Fonts/");
        DirectoryInfo dir = new DirectoryInfo(FontsPath);
        FileInfo[] allFiles = dir.GetFiles();
        string abName = "font.ab";
        foreach (var fileInfo in allFiles)
        {
            if (ExportTools.IsReal(fileInfo.Name) == false)
            {
                continue;
            }
            string key = PathUtil.GetAssetPath(fileInfo.FullName);
            key = key.ToLower();
            newBbNameDic[key] = abName;
            depsInfo[key] = abName;
        }
    }
    private static void MakeShaderAB(Dictionary<string, string> newBbNameDic, Dictionary<string, string> depsInfo)
    {
        string shaderPath = Path.Combine(Application.dataPath, "WorkAssets/shader/");
        string[] _allFiles = FileManager.GetAllFilesInFolder(shaderPath);
        string abName = "shader.ab";
        foreach (var filePath in _allFiles)
        {
            if (ExportTools.IsReal(filePath) == false)
            {
                continue;
            }
            string path = PathUtil.Replace(filePath);
            string key = PathUtil.GetAssetPath(path);
            key = key.ToLower();
            newBbNameDic[key] = abName;
            depsInfo[key] = abName;
        }
    }
    private static void MakeLoginAB(Dictionary<string, string> newBbNameDic, Dictionary<string, string> depsInfo)
    {
        string shaderPath = Path.Combine(Application.dataPath, "WorkAssets/GameStart/");
        string[] _allFiles = FileManager.GetAllFilesInFolder(shaderPath);
        string abName = "gamestart_prefab.ab";
        foreach (var filePath in _allFiles)
        {
            if (ExportTools.IsReal(filePath) == false)
            {
                continue;
            }
            string path = PathUtil.Replace(filePath);
            string key = PathUtil.GetAssetPath(path);
            key = key.ToLower();
            newBbNameDic[key] = abName;
            depsInfo[key] = abName;
        }
    }
    private static void MakeSpineAB(DirectoryInfo info, Dictionary<string, string> newBbNameDic, Dictionary<string, string> depsInfo)
    {
        if (IsSlentDownloadKey(info.FullName))
        {
            return;
        }
        string spineABName = info.Name + "_uispine.ab";
        spineABName = spineABName.ToLower();
        FileInfo[] subInfos = info.GetFiles();
        foreach (var subInfo in subInfos)
        {
            string subName = subInfo.FullName;
            if(ExportTools.IsReal(subName))
            {
                string path = PathUtil.Replace(subName);
                string key = PathUtil.GetAssetPath(path);
                key = key.ToLower();
                newBbNameDic[key] = spineABName;
                depsInfo[key] = spineABName;
            }
        }
    }
    //废弃，都没有ref 都在textures_separate里面，接受引用以及代码调用
    //private static void MakeAllSeparateTexture(ref Dictionary<string, string> newABNameDic, ref Dictionary<string, string> depsInfo)
    //{
    //    string path = Path.Combine(Application.dataPath, "WorkAssets/textures_separate/");
    //    DirectoryInfo dir = new DirectoryInfo(path);
    //    DirectoryInfo[] dirInfos = dir.GetDirectories();
    //    foreach (DirectoryInfo info in dirInfos)
    //    {
    //        if (IsHasSubDir(info))
    //        {
    //            DirectoryInfo[] subDirs = info.GetDirectories();
    //            foreach (var subDir in subDirs)
    //            {
    //                MakeSeparateTextureAB(subDir, ref newABNameDic, ref depsInfo);
    //            }
    //        }
    //        else
    //        {
    //            MakeSeparateTextureAB(info, ref newABNameDic, ref depsInfo);
    //        }
    //    }
    //}
   

    private static void MakeSeparateTextureAB(DirectoryInfo info, ref Dictionary<string, string> newBbNameDic, ref Dictionary<string, string> depsInfo)
    {
        if (IsSlentDownloadKey(info.FullName))
        {
            return;
        }
        string abName = info.Name + "_st.ab";
        abName = abName.ToLower();
        FileInfo[] subInfos = info.GetFiles();
        foreach (var subInfo in subInfos)
        {
            string subName = subInfo.FullName;
            if (ExportTools.IsReal(subName))
            {
                string path = PathUtil.Replace(subName);
                string key = PathUtil.GetAssetPath(path);
                key = key.ToLower();
                newBbNameDic[key] = abName;
                depsInfo[key] = abName;
            }
        }
    }

    public static bool IsHasSubDir(DirectoryInfo info)
    {
        DirectoryInfo[] result = info.GetDirectories();
        return result.Length > 0;
    }

    public static bool IsHasFiles(DirectoryInfo info)
    {
        FileInfo[] result = info.GetFiles();
        return result.Length > 0;
    }
    private static AssetBundleBuild[] MakeAssetBundleBuild(Dictionary<string, string> newABNameDic)
    {
        var builds = new Dictionary<string, List<string>>();
        Dictionary<string, bool> abvs = new Dictionary<string, bool>();
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
            //if(item.Key.Contains("spine_ui_tasklight"))
            //{
            //    Debug.Log("11111");
            //}
            assetBundleBuild.assetBundleName = item.Key;
            item.Value.Sort();
            var assets = item.Value.ToArray();
            assetBundleBuild.assetNames = assets;
            assetBundleBuilds[index++] = assetBundleBuild;
        }
        return assetBundleBuilds;
    }

    private static void MakeDepInfo(Dictionary<string, string> newABNameDic)
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
        //FileManager.DeleteFile(PathManager.StreamingAssetsDepInfoPath);
        byte[] dataBytes = Encoding.UTF8.GetBytes(JsonObject.ToString());
        dataBytes = AESManager.Encrypt(YQPackageManagerEX.KEY, dataBytes);
        //FileManager.WriteAllBytes(PathManager.StreamingAssetsDepInfoPath, dataBytes);
    }
    public static void MakeAssetBundles(AssetBundleBuild[] builds, BuildTarget buildTarget, string outputPath)
    {
        EditorTools.ClearAssetBundlesName();
        AssetDatabase.Refresh();
        var options = BuildAssetBundleOptions.ChunkBasedCompression 
            | BuildAssetBundleOptions.StrictMode
            //| BuildAssetBundleOptions.DisableWriteTypeTree
            | BuildAssetBundleOptions.DeterministicAssetBundle;
        //BuildPipeline.SetAssetBundleEncryptKey(YQPackageManagerEX.ABKEY);
        AssetBundleManifest result = BuildPipeline.BuildAssetBundles(outputPath, builds, options, buildTarget);
        if (!result)
        {
            throw new Exception(string.Format("MakeAssetBundles Error {0}", result));
        }
    }

    public static bool NotSpecialFile(string filePath)
    {
            return  filePath.Contains("BMFont".ToLower()) == false &&
             filePath.Contains("Fonts".ToLower()) == false &&
             filePath.Contains("textures_separate") == false &&
              filePath.Contains("TMPFonts".ToLower()) == false &&
              filePath.EndsWith(".shader") == false &&
               filePath.EndsWith(".cginc") == false &&
                filePath.EndsWith(".shadervariants") == false &&
                 filePath.EndsWith(".ttf") == false &&
                  filePath.EndsWith(".otf") == false &&
                   filePath.EndsWith(".fontsettings") == false;
    }
    
    public static bool IsSlentDownloadKey(string path)
    {
        //string lower = path.ToLower();
        //return lower.Contains(SilentDownload.dazzlingpin.ToLower());
        return false;//改变策略，全打ab
    }
    public static void EncryptAB(string path)
    {
        string[] allFiles = FileManager.GetAllFilesInFolder(path);
        foreach(string abPath in allFiles)
        {
            if(abPath.EndsWith(".ab"))
            {
                byte[] readBytes = (byte[])FileManager.ReadAllBytes(abPath).data;
                byte[] writeBytes = new byte[readBytes.Length + YQPackageManagerEX.bundleOffSize];
                Array.Copy(readBytes, 0, writeBytes, 0, YQPackageManagerEX.bundleOffSize);
                Array.Copy(readBytes, 0, writeBytes, YQPackageManagerEX.bundleOffSize, readBytes.Length);
                File.WriteAllBytes(abPath, writeBytes);
            }
        }
        AssetDatabase.Refresh();
    }
}
