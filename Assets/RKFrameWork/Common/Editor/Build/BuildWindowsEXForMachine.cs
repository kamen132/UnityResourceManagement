using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public partial class BuildWindows 
{
    static string shell;
    static string spe = "^";
    static void SaveMachines(BuildTarget buildTarget, string env)
    {
        string SavePath = "";
        if (string.IsNullOrEmpty(env))
        {
            SavePath = Application.dataPath + "/../OutBuild";
        }
        else
        {
            SavePath = Application.dataPath + "/../../../../SaveData/" + env;
        }
        FileManager.CreateDirectory(SavePath);
        string targetPath = "";
        if (buildTarget == BuildTarget.Android)
        {
            targetPath = SavePath + "/LastBuildMachines_Android";
        }
        else
        {
            targetPath = SavePath + "/LastBuildMachines_IOS";
        }
        FileManager.DeleteDirectory(targetPath);
        //FileManager.CopyDirectory(PathManager.StreamingAssetsMachinesABPath, targetPath);
    }
    static void CopyLastMachines(BuildTarget buildTarget, string env)
    {
        Debug.Log("xxx CopyLastMachines===");
        FileManager.CreateDirectory(Application.streamingAssetsPath);
        string SavePath = "";
        if (string.IsNullOrEmpty(env))
        {
            SavePath = Application.dataPath + "/../OutBuild";
        }
        else
        {
            SavePath = Application.dataPath + "/../../../../SaveData/" + env;
        }
        //FileManager.CreateDirectory(SavePath);
        string targetPath = "";
        if (buildTarget == BuildTarget.Android)
        {
            targetPath = SavePath + "/LastBuildMachines_Android";
        }
        else
        {
            targetPath = SavePath + "/LastBuildMachines_IOS";
        }
        //FileManager.DeleteDirectory(PathManager.StreamingAssetsMachinesABPath);
        //FileCatchData data = FileManager.CopyDirectory(targetPath, PathManager.StreamingAssetsMachinesABPath);
        //if (data.state == false)
        //{
        //    Debug.Log("CopyLastMachines fail  " + data.message);
        //}
        //AssetDatabase.Refresh();
    }
    static void MakeALLMachine(BuildTarget buildTarget)
    {
        string outputPath = Application.dataPath + "/WorkAssets/Machines/";
        DirectoryInfo __dir = new DirectoryInfo(outputPath);
        DirectoryInfo[] dirs = __dir.GetDirectories();
        foreach (DirectoryInfo dirInfo in dirs)
        {
            string dirName = PathUtil.GetFolderName(dirInfo.FullName);
            string abName = dirName + ".ab";
            //CombineSingleABProcess.BuildAssetBundle(buildTarget, abName, dirInfo.FullName, PathManager.StreamingAssetsMachinesABPath);
        }
        RemoveRedundantResources();
    }

    static void RemoveRedundantResources()
    {
        //string shaderABPath = PathManager.StreamingAssetsMachinesABPath + "/shader.ab";
        //FileManager.DeleteFile(shaderABPath);
        //string machinesABPath = PathManager.StreamingAssetsMachinesABPath + "/machines";
        //FileManager.DeleteFile(machinesABPath);
        //string tmpfontsABPath = PathManager.StreamingAssetsMachinesABPath + "/tmpfonts.ab";
        //FileManager.DeleteFile(tmpfontsABPath);
        //string fontABPath = PathManager.StreamingAssetsMachinesABPath + "/font.ab";
        //FileManager.DeleteFile(fontABPath);

    }
    static void ExeAllMachinesWithAB(string env)
    {
        string SavePath = Application.dataPath + "/../../../../SaveData/" + env;
        FileManager.CreateDirectory(SavePath);
        string versionPath = SavePath + "/version.json";
        string versionData = (string)FileManager.ReadAllText(versionPath).data;
        JSONObject versionJson = new JSONObject(versionData);
        int machinesIndex = (int)versionJson["machinesIndex"].i;
        ExeAllMachinesFilesMd5(env, machinesIndex + 1);
    }
    
    static void ExeAllMachines(BuildTarget buildTarget, string env)
    {
        string SavePath = Application.dataPath + "/../../../../SaveData/" + env;
        FileManager.CreateDirectory(SavePath);

        string versionPath = SavePath + "/version.json";
        string versionData = (string)FileManager.ReadAllText(versionPath).data;
        JSONObject versionJson = new JSONObject(versionData);
        int appVersion = (int)versionJson["appVersion"].i;

        string allMachinesMD5InfosPath = SavePath + "/MachinesMD5Infos/" + appVersion + "_0.json";
        string fonder = PathUtil.GetFilesParentFolder(allMachinesMD5InfosPath);
        FileManager.CreateDirectory(fonder);

        versionJson.SetField("machinesIndex", 0);
        string versionKey = "machinesInfos";
        string machinesMd5s = "machinesMd5s";
        string allMachinesPath = SavePath + "/Machines/" + appVersion + "_0";
        fonder = PathUtil.GetFolderName(allMachinesPath);
        FileManager.CreateDirectory(fonder);
        //FileManager.CopyDirectory(PathManager.StreamingAssetsMachinesABPath, allMachinesPath);
        ExportTools.RemoveManifest(allMachinesPath);
        ExportTools.RemoveMeta(allMachinesPath);
       
        string[] allMachinesAB = FileManager.GetAllFilesInFolder(allMachinesPath);
        JSONObject infos = new JSONObject();
        JSONObject machinesVersions = new JSONObject();
        JSONObject machinesMd5sJson = new JSONObject();
        foreach (string item in allMachinesAB)
        {
            string fileName = Path.GetFileNameWithoutExtension(item);
            string md5 = FileUtils.GetMD5HashFromFile(item);
            infos.AddField(fileName, md5);
            machinesVersions.AddField(fileName, 0);
            machinesMd5sJson.AddField(fileName, md5);
        }
        FileManager.WriteAllText(allMachinesMD5InfosPath, infos.ToString());
        versionJson.SetField(versionKey, machinesVersions);
        versionJson.SetField(machinesMd5s, machinesMd5sJson);

        versionJson.RemoveField("patchInfos");
        versionJson.RemoveField("patchIndex");
        FileManager.WriteAllText(versionPath, versionJson.ToString());
        ExeAllMachinesFilesMd5(env, 0);
    }

    static void ExeAllMachinesFilesMd5(string env, int machinesIndex)
    {
        string SavePath = Application.dataPath + "/../../../../SaveData/" + env;
        string versionPath = SavePath + "/version.json";
        string versionData = (string)FileManager.ReadAllText(versionPath).data;
        JSONObject versionJson = new JSONObject(versionData);
        int appVersion = (int)versionJson["appVersion"].i;
        string allMachinesMD5FilesInfoPath = SavePath + "/MachinesMD5FilesInfo/" + appVersion + "_" + machinesIndex + ".json";
        string outputPath = Application.dataPath + "/WorkAssets/Machines/";
        DirectoryInfo __dir = new DirectoryInfo(outputPath);
        DirectoryInfo[] dirs = __dir.GetDirectories();
        JSONObject machinesFilesMd5sJson = new JSONObject();
        foreach (DirectoryInfo dirInfo in dirs)
        {
            string machineName = PathUtil.GetFolderName(dirInfo.FullName);
            string machinePath = dirInfo.FullName;
            string[] files = FileManager.GetAllFilesInFolder(machinePath);
            JSONObject filesMd5sJson = new JSONObject();
            foreach (string filePath in files)
            {
                if (ExportTools.IsReal(filePath) == false)
                {
                    continue;
                }
                string md5 = FileUtils.GetMD5HashFromFile(filePath);
                string assetPath = PathUtil.GetAssetPath(filePath);
                assetPath = assetPath.Replace("/", "_");
                string key = assetPath + spe + md5;
                filesMd5sJson.AddField(key, 0);
            }
            machinesFilesMd5sJson.AddField(machineName, filesMd5sJson);
        }
        FileManager.WriteAllText(allMachinesMD5FilesInfoPath, machinesFilesMd5sJson.ToString());
    }
    static void MakeInAppMachines(BuildTarget buildTarget, string env)
    {
        List<string> InAppMachines = BuildConfig.InAppMachines;
        string outputPath = Application.dataPath + "/WorkAssets/Machines/";
        //FileManager.DeleteDirectory(PathManager.StreamingAssetsMachinesABPath);
        foreach (string name in InAppMachines)
        {
            string fullPath = outputPath + name;
            string abName = name + ".ab";
            //CombineSingleABProcess.BuildAssetBundle(buildTarget, abName, fullPath, PathManager.StreamingAssetsMachinesABPath);
        }
        RemoveRedundantResources();
    }

    static void MakeDiffMachines1(BuildTarget buildTarget, string env)
    {
        shell = Application.dataPath + "/../../command/build/BsDiff.sh";
        string SavePath = Application.dataPath + "/../../../../SaveData/" + env;
        FileManager.CreateDirectory(SavePath);

        string versionPath = SavePath + "/version.json";
        string versionData = (string)FileManager.ReadAllText(versionPath).data;
        Debug.Log("xxx versionData" + versionData);
        JSONObject versionJson = new JSONObject(versionData);
        int appVersion = (int)versionJson["appVersion"].i;

        string BaselMachinesPath = SavePath + "/Machines/" + appVersion + "_0";
        string BaseDataPath = SavePath + "/MachinesMD5Infos/" + appVersion + "_0.json";

        int machinesIndex = (int)versionJson["machinesIndex"].i;
        machinesIndex++;

        string diffPath = SavePath + "/Machines/" + appVersion + "_" + machinesIndex.ToString();
        string patchPath = SavePath + "/MachinesPatch/" + appVersion + "_" + machinesIndex.ToString();
        string versionKey = "machinesInfos";
        string machinesMd5s = "machinesMd5s";
        JSONObject machinesVersions = versionJson[versionKey];
        //FileManager.CopyDirectory(PathManager.StreamingAssetsMachinesABPath, diffPath);
        ExportTools.RemoveManifest(diffPath);
        ExportTools.RemoveMeta(diffPath);
        string[] allMachinesAB = FileManager.GetAllFilesInFolder(diffPath);

        string BaseDataStr = (string)FileManager.ReadAllText(BaseDataPath).data;
        JSONObject BaseDataJson = new JSONObject(BaseDataStr);
       
        //Debug.Log("xxx BaseAllMachinesInfoPath==" + BaseDataPath);
        //Debug.Log("xxx allMachinesInfoStr==" + BaseAllMachinesInfoStr);

        JSONObject machinesMd5sJson = versionJson[machinesMd5s];
        foreach (string item in allMachinesAB)
        {
            string fileName = Path.GetFileNameWithoutExtension(item);
            if (fileName.Equals("machines"))
            {
                continue;
            }
            string md5 = FileUtils.GetMD5HashFromFile(item);
            Debug.Log("new_md5:  " + fileName + "--" + md5);
            //如果基础的没有，或者基础的md5跟现在的不一样
            if (BaseDataJson.HasField(fileName) == false || BaseDataJson[fileName].str.Equals(md5) == false)
            {
                Debug.Log("xxx start patch:  " + fileName + "--" + md5 + "-" + BaseDataJson.HasField(fileName));
                if (BaseDataJson.HasField(fileName))
                {
                    Debug.Log("xxx start patch2:  " + BaseDataJson[fileName].str.Equals(md5) + "--" + BaseDataJson[fileName].str);
                }
                machinesVersions.SetField(fileName, machinesIndex);
                machinesMd5sJson.SetField(fileName, md5);
                versionJson.SetField("machinesIndex", machinesIndex);
                FileManager.CreateDirectory(patchPath);
                string fromPath = BaselMachinesPath + "/" + Path.GetFileName(item); ;
                string patchFilePath = patchPath + "/patch_" + machinesIndex + "_" + fileName;
                string args = fromPath + " " + item + " " + patchFilePath;
                //Debug.Log("args==" + args);
                CommandLineTools.MacCommand(shell, args);
            }
        }
        versionJson.SetField(machinesMd5s, machinesMd5sJson);
        versionJson.RemoveField("patchInfos");
        versionJson.RemoveField("patchIndex");

        FileManager.WriteAllText(versionPath, versionJson.ToString());
    }

    static void MakeDiffMachines(BuildTarget buildTarget, string env)
    {
        shell = Application.dataPath + "/../../command/build/BsDiff.sh";
        string SavePath = Application.dataPath + "/../../../../SaveData/" + env;
        FileManager.CreateDirectory(SavePath);

        string versionPath = SavePath + "/version.json";
        string versionData = (string)FileManager.ReadAllText(versionPath).data;
        JSONObject versionJson = new JSONObject(versionData);
        int appVersion = (int)versionJson["appVersion"].i;
        int machinesIndex = (int)versionJson["machinesIndex"].i;

        string lastDataPath = SavePath + "/MachinesMD5FilesInfo/" + appVersion + "_" + machinesIndex + ".json";
        string lastDataStr = (string)FileManager.ReadAllText(lastDataPath).data;
        JSONObject lastFilesMd5Json = new JSONObject(lastDataStr);

        string newDataPath = SavePath + "/MachinesMD5FilesInfo/" + appVersion + "_" + (machinesIndex + 1 ) + ".json";
        string newDataStr = (string)FileManager.ReadAllText(newDataPath).data;
        JSONObject newFilesMd5Json = new JSONObject(newDataStr);

        //----------------
        string diffPath = SavePath + "/Machines/" + appVersion + "_" + machinesIndex.ToString();
       
        string BaselMachinesPath = SavePath + "/Machines/" + appVersion + "_0/";
        string versionKey = "machinesInfos";
        string machinesMd5s = "machinesMd5s";
        JSONObject machinesVersions = versionJson[versionKey];
        JSONObject machinesMd5sJson = versionJson[machinesMd5s];
        machinesIndex++;
        string patchPath = SavePath + "/MachinesPatch/" + appVersion + "_" + machinesIndex.ToString();
        List<string> newKeys = newFilesMd5Json.keys;
        foreach (string fullMachineName in newKeys)
        {
            bool isNeedChange = true;
            if(lastFilesMd5Json.HasField(fullMachineName))
            {
                isNeedChange = IsMachinesChange(lastFilesMd5Json[fullMachineName], newFilesMd5Json[fullMachineName]);
            }
            if(isNeedChange)
            {
                string machineName = GetMachineName(fullMachineName);
                //string abPath = PathManager.StreamingAssetsMachinesABPath + "/" + machineName + ".ab";
                //string md5 = FileUtils.GetMD5HashFromFile(abPath);
                //Debug.Log("machines change:" + machineName + "-newMD%:" + md5);
                //machinesVersions.SetField(machineName, machinesIndex);
                //machinesMd5sJson.SetField(machineName, md5);
                
                //string fromABPath = BaselMachinesPath + machineName + ".ab";
                //if (FileManager.FileExist(fromABPath) == false)//历史没有过这个ab
                //{
                //    machinesVersions.SetField(machineName, 0);
                //    FileManager.CopyFile(abPath, fromABPath);
                //}
                //else
                //{
                //    versionJson.SetField("machinesIndex", machinesIndex);
                //    FileManager.CreateDirectory(patchPath);
                //    string patchFilePath = patchPath + "/patch_" + machinesIndex + "_" + machineName;
                //    string args = fromABPath + " " + abPath + " " + patchFilePath;
                //    Debug.Log("args==" + args);
                //    CommandLineTools.MacCommand(shell, args);
                //}
            }
        }
        versionJson.SetField(versionKey, machinesVersions);
        versionJson.SetField(machinesMd5s, machinesMd5sJson);
        versionJson.RemoveField("patchInfos");
        versionJson.RemoveField("patchIndex");

        FileManager.WriteAllText(versionPath, versionJson.ToString());
    }

    static string GetMachineName(string fullName)
    {
        int index = fullName.LastIndexOf(spe);
        return fullName.Substring(index + 1).ToLower();
    }
    static bool IsMachinesChange(JSONObject lastData, JSONObject newData)
    {
        List<string> newKeys = newData.keys;
        foreach(string fileName in newKeys)
        {
            if(lastData.HasField(fileName) == false)
            {
                return true;
            }
        }
        return false;
    }
}
