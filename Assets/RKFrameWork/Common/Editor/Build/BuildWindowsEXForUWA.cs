using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public partial class BuildWindows 
{
    static string SharePath = "/Users/mac/mac/jenkinsbuild/Share";
    public static void OnDoForUWA(string forWhat, BuildTarget buildTarget, string env, bool B_isDebug)
    {
        if (forWhat.Equals("uwa_ios_all"))
        {
            OnBuildPlayer(buildTarget, env, B_isDebug);
        }else if(forWhat.Equals("uwa_android_all"))
        {
            OnBuildPlayer(buildTarget, env, true);
        }
    }
    static void CopyApk(BuildTarget buildTarget, string env)
    {
       
        string SavePath = Application.dataPath + "/../../../../SaveData/" + env;
        string versionPath = SavePath + "/version.json";
        string versionData = (string)FileManager.ReadAllText(versionPath).data;
        JSONObject versionJson = new JSONObject(versionData);
        int appVersion = (int)versionJson["appVersion"].i;
        int abVersion = (int)versionJson["abVersion"].i;
        if (buildTarget == BuildTarget.Android)
        {
            string apkPath = Application.dataPath + "/../OutBuild/live777.apk";
            FileManager.CopyFile(apkPath, SavePath + "/apk/maxbet_" + appVersion + ".apk");
        }
        else
        {
            string apkPath = Application.dataPath + "/../OutBuild/iosBuild/Apps/wondercashcasino.ipa";
            if (env.Equals("adhoc") || env.Equals("appstore"))
            {
                apkPath = Application.dataPath + "/../OutBuild/iosBuild/wondercashcasino.ipa";
            }
            else if (env.Equals("intranet_uwa"))
            {
                apkPath = Application.dataPath + "/../OutBuild/iosBuild/Apps/inhouse.ipa";
            }
            FileManager.CopyFile(apkPath, SavePath + "/ipa/maxbet_" + appVersion + ".ipa");
        }
        if (env.Equals("appstore"))
        {
            string apkPath = Application.dataPath + "/../OutBuild/iosBuild/wondercashcasino.ipa";
            string targetPath = SharePath + "/appstoreIpa/appstore_" + appVersion + ".ipa";
            FileManager.CopyFile(apkPath, targetPath);
            string url = SharePath + "/appstore.txt";
            string content = "";
            if (FileManager.FileExist(url))
            {
                content = (string)FileManager.ReadAllText(url).data;
            }
            content = content + string.Format("http://10.0.52.78/appstoreIpa/appstore_{0}.ipa \n\n", appVersion);
            FileManager.WriteAllText(url, content);
        }
        else if (env.Equals("adhoc"))
        {
            string apkPath = Application.dataPath + "/../OutBuild/iosBuild/wondercashcasino.ipa";
            string targetPath = SharePath + "/adhocIpa/adhoc_" + appVersion + ".ipa";
            FileManager.CopyFile(apkPath, targetPath);
            string url = SharePath + "/adhoc.txt";
            string content = "";
            if (FileManager.FileExist(url))
            {
                content = (string)FileManager.ReadAllText(url).data;
            }
            content = content + string.Format("http://10.0.52.78/adhocIpa/adhoc_{0}.ipa \n\n", appVersion);
            FileManager.WriteAllText(url, content);
        }
        else if (env.Equals("intranet_uwa"))
        {
            string apkPath = Application.dataPath + "/../OutBuild/iosBuild/Apps/inhouse.ipa";
            string targetPath = SharePath + "/uwaIntranetIpa/uwaIntranet_" + appVersion + ".ipa";
            FileManager.CopyFile(apkPath, targetPath);
            string url = SharePath + "/uwaIntranet.txt";
            string content = "";
            if (FileManager.FileExist(url))
            {
                content = (string)FileManager.ReadAllText(url).data;
            }
            content = content + string.Format("http://10.0.52.78/uwaIntranetIpa/uwaIntranet__{0}.ipa \n\n", appVersion);
            FileManager.WriteAllText(url, content);
        }else if(env.Equals("uwa_android"))
        {
            string apkPath = Application.dataPath + "/../OutBuild/live777.apk";
            string targetPath = SharePath + "/uwaApk/uwaApk_" + appVersion + ".apk";
            FileManager.CopyFile(apkPath, targetPath);

            string url = SharePath + "/uwaApk.txt";
            string content = "";
            if (FileManager.FileExist(url))
            {
                content = (string)FileManager.ReadAllText(url).data;
            }
            content = content + string.Format("http://10.0.52.78/uwaApk/uwaApk_{0}.apk \n\n", appVersion);
            FileManager.WriteAllText(url, content);
        }
        else if (env.Equals("uwa_android_test"))
        {
            string apkPath = Application.dataPath + "/../OutBuild/live777.apk";
            string targetPath = SharePath + "/uwaApk/uwaTestApk_" + appVersion + ".apk";
            FileManager.CopyFile(apkPath, targetPath);

            string url = SharePath + "/uwaApk.txt";
            string content = "";
            if (FileManager.FileExist(url))
            {
                content = (string)FileManager.ReadAllText(url).data;
            }
            content = content + string.Format("http://10.0.52.78/uwaApk/uwaTestApk_{0}.apk \n\n", appVersion);
            FileManager.WriteAllText(url, content);
        }
        else if (env.Equals("uwa_android_got_online"))
        {
            string apkPath = Application.dataPath + "/../OutBuild/live777.apk";
            string targetPath = SharePath + "/uwaApk/uwaGotOnLineApk_" + appVersion + ".apk";
            FileManager.CopyFile(apkPath, targetPath);

            string url = SharePath + "/uwaApk.txt";
            string content = "";
            if (FileManager.FileExist(url))
            {
                content = (string)FileManager.ReadAllText(url).data;
            }
            content = content + string.Format("http://10.0.52.78/uwaApk//uwaGotOnLineApk_{0}.apk \n\n", appVersion);
            FileManager.WriteAllText(url, content);
        }
        else if (env.Equals("qa_online_intranet"))
        {
            string apkPath = Application.dataPath + "/../OutBuild/iosBuild/Apps/inhouse2.ipa";
            string targetPath = SharePath + "/qaOnlineIntranetIpa/qaOnlineIntranet_" + appVersion + ".ipa";
            FileManager.CopyFile(apkPath, targetPath);
            string url = SharePath + "/qaOnlineIntranet.txt";
            string content = "";
            if (FileManager.FileExist(url))
            {
                content = (string)FileManager.ReadAllText(url).data;
            }
            content = content + string.Format("http://10.0.52.78/qaOnlineIntranetIpa/qaOnlineIntranet_{0}.ipa \n\n", appVersion);
            FileManager.WriteAllText(url, content);
        }
        else if (env.Equals("intranet"))
        {
            string apkPath = Application.dataPath + "/../OutBuild/iosBuild/Apps/inhouse.ipa";
            string targetPath = SharePath + "/intranetIpa/inhouse_" + appVersion + ".ipa";
            FileManager.CopyFile(apkPath, targetPath);
            string url = SharePath + "/intranet.txt";
            string content = "";
            if (FileManager.FileExist(url))
            {
                content = (string)FileManager.ReadAllText(url).data;
            }
            content = content + string.Format("http://10.0.52.78/intranetIpa/inhouse_{0}.ipa \n\n", appVersion);
            FileManager.WriteAllText(url, content);
        }
        else if (env.Equals("googleplay"))
        {
              //---------------------------
            string apkPath = Application.dataPath + "/../OutBuild/live777.apk";
            string obbPath = Application.dataPath + "/../OutBuild/live777.main.obb";
            string targetPath = SharePath + "/googleplayApk/googleplay_" + appVersion + ".apk";
            string targetObbPath = SharePath + "/googleplayApk/googleplayObb_" + appVersion + ".main.obb";

            FileManager.CopyFile(apkPath, targetPath);
            FileManager.CopyFile(obbPath, targetObbPath);
            string url = SharePath + "/googleplay.txt";
            string content = "";
            if (FileManager.FileExist(url))
            {
                content = (string)FileManager.ReadAllText(url).data;
            }
            content = content + string.Format("http://10.0.52.78/googleplayApk/googleplay_{0}.apk \n\n", appVersion);
            content = content + string.Format("http://10.0.52.78/googleplayApk/googleplayObb_{0}.main.obb \n\n", appVersion);
            FileManager.WriteAllText(url, content);

        }
        else if (env.Equals("android_adhoc"))
        {
            string apkPath = Application.dataPath + "/../OutBuild/live777.apk";
            string targetPath = SharePath + "/androidAdhocApk/androidAdhoc_" + appVersion + ".apk";
            FileManager.CopyFile(apkPath, targetPath);

            string url = SharePath + "/androidAdhocApk.txt";
            string content = "";
            if (FileManager.FileExist(url))
            {
                content = (string)FileManager.ReadAllText(url).data;
            }
            content = content + string.Format("http://10.0.52.78/androidAdhocApk/androidAdhoc_{0}.apk \n\n", appVersion);
            FileManager.WriteAllText(url, content);
        }
    }

    static void CopyBranchApk(string branchName)
    {
        string apkPath = Application.dataPath + "/../OutBuild/live777.apk";
        string targetPath = SharePath + "/BranchesApk/BranchesApk_" + branchName + ".apk";
        FileManager.CopyFile(apkPath, targetPath);

        string url = SharePath + "/BranchesApk.txt";
        string content = "";
        if (FileManager.FileExist(url))
        {
            content = (string)FileManager.ReadAllText(url).data;
        }
        content = content + string.Format("http://10.0.52.78/BranchesApk/BranchesApk_{0}.apk \n\n", branchName);
        FileManager.WriteAllText(url, content);
    }
    static void CopyAndroidDLL()
    {
        string uwaPath = Application.dataPath + "/../../command/build/Android";
        string targetPath = Application.dataPath + "/DianDianSDK/Plugins";
        FileManager.CopyDirectory(uwaPath, targetPath);
    }
    static void RemoveAndroidDLL()
    {
        string targetPath = Application.dataPath + "/DianDianSDK/Plugins/Android";
        FileManager.DeleteDirectory( targetPath);
    }
}
