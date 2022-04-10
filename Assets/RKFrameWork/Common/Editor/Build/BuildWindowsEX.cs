using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public partial class BuildWindows
{
    public static void ExportAppByGit()
    {
        Debug.Log("------------------ExportAppByGit--------------------");
        string[] args = System.Environment.GetCommandLineArgs();
        int count = args.Length;
        string forWhat = "";
        string target = "";
        string isDebug = "1";
        string env = "";
        string branches = "";
        for (int i = 0; i < count; i++)
        {
            if (args[i] == "-forWhat")
            {
                forWhat = args[i + 1];
            }
            else if (args[i] == "-buildTarget")
            {
                target = args[i + 1];
            }
            else if (args[i] == "-isDebug")
            {
                isDebug = args[i + 1];
            }
            else if (args[i] == "-env")
            {
                env = args[i + 1];
            }
            else if (args[i] == "-branches")
            {
                branches = args[i + 1];
            }
        }
        BuildTarget buildTarget = BuildTarget.Android;
        bool B_isDebug = int.Parse(isDebug) == 1;
        if (target.Equals("Android") == false)
        {
            buildTarget = BuildTarget.iOS;
        }

        if (forWhat.Equals("build_ab"))
        {
            OnBuildAB(buildTarget, env, B_isDebug);
        }
        else if (forWhat.Equals("build_player"))
        {
            OnBuildPlayer(buildTarget, env, B_isDebug);
        }

        OnDoForUWA(forWhat, buildTarget, env, B_isDebug);
        if (forWhat.Equals("android_branches"))
        {
            OnBuildBranchPlayer(buildTarget, env, true);
            CopyBranchApk(branches);
        }
        Debug.Log("------------------ExportAppByGit-----over---------------");
    }

    static void OnBuildBranchPlayer(BuildTarget buildTarget, string env, bool B_isDebug)
    {
        BuildABCommand.OnStartBuildAB(buildTarget);
        BuildABCommand.EncryptAB(PathManager.StreamingAssetsABPath);
        AssetDatabase.Refresh();
        ExportData();
        MakeALLMachine(buildTarget);
        //BuildABCommand.EncryptAB(PathManager.StreamingAssetsMachinesABPath);
        MakeVersionEnv(env);
        BuildPlayer(buildTarget, B_isDebug, false, env);
    }

    static void OnBuildAB(BuildTarget buildTarget, string env, bool B_isDebug)
    {
        if(env.Equals("android_develop")
            || env.Equals("intranet")
            || env.Equals("android_dev_qa")
            || env.Equals("qa_online_intranet")
            || env.Equals("android_master")
            )
        {
            MaxPatchCount = TestMaxPatchCount;
        }
        AssetBundleBuild[] _AssetBundleBuild = BuildABCommand.OnStartBuildAB(buildTarget);
        BuildABCommand.EncryptAB(PathManager.StreamingAssetsABPath);
        //bool isSilentABChange = false;
        bool isSilentABChange = SilentDownload.MakeSilentABPatch(buildTarget, env);
        AssetDatabase.Refresh();
        ExportData();
        bool isChange = MakeABDiff(buildTarget, env, _AssetBundleBuild, isSilentABChange);
        MakeLuaPatch(buildTarget, env, isChange);
        float startTime = CommonUtils.GetTickCount();

        CopyLastMachines(buildTarget, env);
        MakeALLMachine(buildTarget);
        SaveMachines(buildTarget, env);

        //BuildABCommand.EncryptAB(PathManager.StreamingAssetsMachinesABPath);
        ExeAllMachinesWithAB(env);
        MakeDiffMachines(buildTarget, env);
        Debug.Log("OnMakeMachine OVER use time " + (CommonUtils.GetTickCount() - startTime));
    }

    static void OnBuildPlayer(BuildTarget buildTarget, string env, bool B_isDebug)
    {
        AssetBundleBuild[] _AssetBundleBuild = BuildABCommand.OnStartBuildAB(buildTarget);
        BuildABCommand.EncryptAB(PathManager.StreamingAssetsABPath);
        AssetDatabase.Refresh();
        ExportData();

        MakeInAppMachines(buildTarget, env);
        //BuildABCommand.EncryptAB(PathManager.StreamingAssetsMachinesABPath);
        MakeBuildInfo(buildTarget, env, _AssetBundleBuild);
        AddAnotherVersionData(env);
        SilentDownload.ExeSilentAB(buildTarget, env);

        bool isSuccess = BuildPlayer(buildTarget, B_isDebug, true, env);
        if (isSuccess)
        {
            CopyLastMachines(buildTarget, env);
            MakeALLMachine(buildTarget);
            SaveMachines(buildTarget, env);
            //BuildABCommand.EncryptAB(PathManager.StreamingAssetsMachinesABPath);
            ExeAllMachines(buildTarget, env);
            if(buildTarget == BuildTarget.Android)
            {
                CopyApk(buildTarget, env);
            }
        }
    }

    static void MakeBuildInfo(BuildTarget buildTarget, string env, AssetBundleBuild[] _AssetBundleBuild)
    {
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
        string resPath = "";
        appVersion++;
        resPath = SavePath + "/AB/" + appVersion + "_0";
        PlayerSettingsConfig.bundleVersion = string.Format("{0}.{1}.0", 1, appVersion.ToString());

        PlayerSettingsConfig.bundleVersionCode = appVersion;

        FileManager.CreateDirectory(resPath);
        FileManager.CopyDirectory(PathManager.StreamingAssetsABPath, resPath + "/max");
        //FileManager.CopyFile(PathManager.StreamingAssetsDepInfoPath, resPath + "/dps.txt");
        //FileManager.CopyFile(PathManager.StreamingAssetsPBPath, resPath + "/lua.pb");
        //FileManager.CopyFile(PathManager.StreamingAssetsUnpackLuaPath, resPath + "/unpack_lua.txt");
        //FileManager.CopyFile(PathManager.StreamingAssetsNeedPackLuaPath, resPath + "/need_pack_lua.txt");


        string copyluaDataPath = SavePath + "/BSLuaFile/" + appVersion + "_0/need_pack_lua.txt";
        //FileManager.CopyFile(PathManager.StreamingAssetsNeedPackLuaPath, copyluaDataPath);

        versionJson.SetField("appVersion", appVersion);
        versionJson.SetField("abVersion", 0);
        versionJson.SetField("luaVersion", 0);
        versionJson.SetField("machinesIndex", 0);
        if (versionJson.HasField("luaPatchIndexs"))
        {
            versionJson.RemoveField("luaPatchIndexs");
        }
        if (versionJson.HasField("abPatchIndexs"))
        {
            versionJson.RemoveField("abPatchIndexs");
        }
        if (versionJson.HasField("abPatchMd5s"))
        {
            versionJson.RemoveField("abPatchMd5s");
        }
        if (versionJson.HasField("luaPatchMd5s"))
        {
            versionJson.RemoveField("luaPatchMd5s");
        }

        JSONObject InAppMachines = new JSONObject();
        foreach (var name in BuildConfig.InAppMachines)
        {
            InAppMachines.SetField(name.ToLower(), 1);
        }
        versionJson.SetField("inAppMachines", InAppMachines);

        FileManager.WriteAllText(versionPath, versionJson.ToString());
        ExportTools.RemoveMeta(SavePath);
        ExportTools.RemoveManifest(SavePath);
        CreateFileInfo(buildTarget, resPath, env, _AssetBundleBuild);
        string targetVersionPath = Path.Combine(Application.streamingAssetsPath, "version.json");
        FileManager.DeleteFile(targetVersionPath);
        FileManager.CopyFile(versionPath, targetVersionPath);
        AssetDatabase.Refresh();
    }

    public static void CopyAPK()
    {
        Debug.Log("------------------CopyAPK--------------------");
        string[] args = System.Environment.GetCommandLineArgs();
        int count = args.Length;
        string target = "";
        string env = "";
        for (int i = 0; i < count; i++)
        {
            if (args[i] == "-buildTarget")
            {
                target = args[i + 1];
            }
            else if (args[i] == "-env")
            {
                env = args[i + 1];
            }
        }
        BuildTarget buildTarget = BuildTarget.Android;
        if (target.Equals("Android") == false)
        {
            buildTarget = BuildTarget.iOS;
        }
        CopyApk(buildTarget, env);
        Debug.Log("------------------CopyAPK-----over---------------");
    }

    public static void AddAnotherVersionData(string env)
    {
        string SavePath = Application.dataPath + "/../../../../SaveData/" + env;
        FileManager.CreateDirectory(SavePath);
        string versionPath = SavePath + "/version.json";
        string versionData = (string)FileManager.ReadAllText(versionPath).data;
        JSONObject versionJson = new JSONObject(versionData);
        if (env.Equals("android_test"))
        {
            versionJson.SetField("downLoadApkUrl", "http://10.0.52.78:8086/s/F22S");
        }
        else if (env.Equals("intranet"))
        {
            versionJson.SetField("downLoadApkUrl", "http://10.0.52.78:8086/s/hHAB");
        }
        else if (env.Equals("dev"))
        {
            versionJson.SetField("downLoadApkUrl", "http://10.0.52.78:8086/s/lyDF");
        }
        else if (env.Equals("android_master"))
        {
            versionJson.SetField("downLoadApkUrl", "http://10.0.52.78:8086/s/Wtjv");
        }
        else if (env.Equals("android_develop"))
        {
            versionJson.SetField("downLoadApkUrl", "http://10.0.52.78:8086/s/Qy0U");
        }
        else if (env.Equals("android_dev_qa"))
        {
            versionJson.SetField("downLoadApkUrl", "http://10.0.52.78:8086/s/Qy0U");
        }
        else if (env.Equals("android_online"))
        {
            versionJson.SetField("downLoadApkUrl", "http://10.0.52.78:8086/s/HSOL");
        }

        versionJson.SetField("env", env);
        FileManager.WriteAllText(versionPath, versionJson.ToString());
        string targetVersionPath = Path.Combine(Application.streamingAssetsPath, "version.json");
        FileManager.CopyFile(versionPath, targetVersionPath);
    }

    static void MakeVersionEnv(string env)
    {
        string targetVersionPath = Path.Combine(Application.streamingAssetsPath, "version.json");
        if (FileManager.FileExist(targetVersionPath) == false)
        {
            string fromPath = Application.dataPath + "/../../command/build/version.json";
            FileManager.CopyFile(fromPath, targetVersionPath);
        }
        string versionData = (string)FileManager.ReadAllText(targetVersionPath).data;
        JSONObject versionJson = new JSONObject(versionData);
        versionJson.SetField("env", env);
        FileManager.WriteAllText(targetVersionPath, versionJson.ToString());
    }

}
