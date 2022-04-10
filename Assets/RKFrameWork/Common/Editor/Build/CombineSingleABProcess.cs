using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class CombineSingleABProcess
{
    static List<string> allFile = new List<string>();
    static List<string> allShaderFiles = new List<string>();
    static List<string> allTmpFiles = new List<string>();
    static AssetBundleBuild shaderAssetBundleBuild;
    static AssetBundleBuild tmpAssetBundleBuild;
    public static void BuildAssetBundle(BuildTarget target, string abName, string fromPath, string outPath)
    {
        EditorTools.ClearAssetBundlesName();
        allFile.Clear();
        PackFiles(fromPath);
        MakeShaderAB();
        MakeTMPAB();
        if (!Directory.Exists(outPath))
        {
            Directory.CreateDirectory(outPath);
        }
        AssetBundleBuild[] data = MakeAssetBundleBuild(abName);
        BuildABCommand.MakeAssetBundles(data, target, outPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    static void PackFiles(string source)
    {
        source = PathUtil.Replace(source);
        string[] _allFiles = FileManager.GetAllFilesInFolder(source);
        foreach (var filePath in _allFiles)
        {
            if (ExportTools.IsReal(filePath) == false)
            {
                continue;
            }
            string key = PathUtil.GetAssetPath(filePath);
            key = key.ToLower();
            if(allFile.Contains(key) == false)
            {
                allFile.Add(key);
            }
        }
    }
    private static void MakeShaderAB()
    {
        if(allShaderFiles.Count > 0)
        {
            return;
        }
        shaderAssetBundleBuild = new AssetBundleBuild();
        string shaderPath = Path.Combine(Application.dataPath, "WorkAssets/shader/");
        string[] _allFiles = FileManager.GetAllFilesInFolder(shaderPath);
       
        foreach (var filePath in _allFiles)
        {
            if (ExportTools.IsReal(filePath) == false)
            {
                continue;
            }
            string path = PathUtil.Replace(filePath);
            string key = PathUtil.GetAssetPath(path);
            key = key.ToLower();
            allShaderFiles.Add(key);
        }
        string abName = "shader.ab";
        shaderAssetBundleBuild.assetBundleName = abName;
        var assets = allShaderFiles.ToArray();
        shaderAssetBundleBuild.assetNames = assets;
    }

    private static void MakeTMPAB()
    {
        if (allTmpFiles.Count > 0)
        {
            return;
        }
        tmpAssetBundleBuild = new AssetBundleBuild();
        string tmpPath = Path.Combine(Application.dataPath, "WorkAssets/TMPFonts/");
        string[] _allFiles = FileManager.GetAllFilesInFolder(tmpPath);

        foreach (var filePath in _allFiles)
        {
            if (ExportTools.IsReal(filePath) == false)
            {
                continue;
            }
            string path = PathUtil.Replace(filePath);
            string key = PathUtil.GetAssetPath(path);
            key = key.ToLower();
            allTmpFiles.Add(key);
        }
        string abName = "tmpfonts.ab";
        tmpAssetBundleBuild.assetBundleName = abName;
        var assets = allTmpFiles.ToArray();
        tmpAssetBundleBuild.assetNames = assets;
    }
    static AssetBundleBuild[] MakeAssetBundleBuild(string abName)
    {
        var assetBundleBuilds = new AssetBundleBuild[3];
        allFile.Sort();
        AssetBundleBuild assetBundleBuild = new AssetBundleBuild();
        assetBundleBuild.assetBundleName = abName;
        var assets = allFile.ToArray();
        assetBundleBuild.assetNames = assets;
        assetBundleBuilds[0] = assetBundleBuild;
        assetBundleBuilds[1] = shaderAssetBundleBuild;
        assetBundleBuilds[2] = tmpAssetBundleBuild;
        return assetBundleBuilds;
    }
}
