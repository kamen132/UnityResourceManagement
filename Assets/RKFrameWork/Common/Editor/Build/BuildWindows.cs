using System.IO;
using UnityEditor;
using UnityEngine;


public partial class BuildWindows : EditorWindow
{
    //BESTHTTP_DISABLE_COOKIES
    //BESTHTTP_DISABLE_SERVERSENT_EVENTS
    //BESTHTTP_DISABLE_WEBSOCKET:
    //BESTHTTP_DISABLE_SIGNALR
    //BESTHTTP_DISABLE_SOCKETIO
    //BESTHTTP_DISABLE_ALTERNATE_SSL
    //BESTHTTP_DISABLE_UNITY_FORM
    static string[] BUILD_SCENES = new string[] { "Assets/Scenes/PreloadScene.unity" };
    //private static string BaseDefinition = "ENABLE_IL2CPP;THREAD_SAFE;BESTHTTP_DISABLE_SERVERSENT_EVENTS;BESTHTTP_DISABLE_SIGNALR;"
    //    + "BESTHTTP_DISABLE_UNITY_FORM;BESTHTTP_DISABLE_ALTERNATE_SSL;BESTHTTP_DISABLE_SOCKETIO;BESTHTTP_DISABLE_CACHING";//开发所用宏

    private static string BaseDefinition = "USE_HOT;USE_PDB;HotFix;BUNDLE_MODE;RELEASE_PACKAGE;LOAD_PACKAGE_HOTFIX;IS_SHOW_LOGIN_VIEW;DEVELOP_TEST";
    //symbols = DeleteString(symbols, "OVERALL");
    //symbols = DeleteString(symbols, "DISABLE_ILRUNTIME_DEBUG");
    //symbols = AddString(symbols, "HotFix");
    //symbols = AddString(symbols, "ENABLE_PDB");

    private const string ResFromAB = "RES_FROM_AB";

    private const string DownLoadAB = "DOWNLOAD_AB";
    public static string android_with_AB = "导出 android AB的 APK";
    public static string android_with_Res = "导出 android Res的 APK";

    public static string ios_with_AB = "导出 IOS AB的 ipa";
    public static string ios_with_Res = "导出 IOS Res的 ipa";

    public static string env = "";
    //[MenuItem("Build/TestMD5", false)]
    public static void TestMD5()
    {
        string path = Application.persistentDataPath + "/0/machinedollarrush.ab";
        string path2 = Application.persistentDataPath + "/1/machinedollarrush.ab";
        string path3 = Application.persistentDataPath + "/2/machinedollarrush.ab";
        string path4 = Application.persistentDataPath + "/3/machinedollarrush.ab";

        string md51 = FileUtils.GetMD5HashFromFile(path);
        string md52 = FileUtils.GetMD5HashFromFile(path2);
        string md53 = FileUtils.GetMD5HashFromFile(path3);
        string md54 = FileUtils.GetMD5HashFromFile(path4);
        Debug.Log("1===" + md51);
        Debug.Log("2===" + md52);
        Debug.Log("3===" + md53);
        Debug.Log("4===" + md54);
    }

    public static BuildWindows window = null;
    [MenuItem("Build/Open Export Window", false)]
    public static void OpenExportApkWindow()
    {
        BuildWindows window = (BuildWindows)GetWindow(typeof(BuildWindows));  //定义一个窗口对象  
        float width = 500;
        float height = 500;
        float posX = 500f;
        float posY = 300f;
        window.position = new Rect(posX, posY, width, height);
        BuildWindows.window = window;
    }
    
    private bool isNeedCheckDownLoad = false;
    private bool isNeedEncryptAB = true;
    private bool isNeedSDK = true;

    private bool isBuildAndroidAb = true;

    private bool isBuildAndoridAPK = true;

    private bool isBuildIOSAb = true;

    private bool isExportXcode = true;
    private string[] iosEnv = new string[] { "企业证书", "测试", "正式" , "appstore"};
    private int iosEnvIndex = 0;
    void OnGUI()
    {
        GUILayout.Space(10);
        EditorGUILayout.LabelField("###################### Common #########################  ");
        isNeedCheckDownLoad = GUILayout.Toggle(isNeedCheckDownLoad, "   CheckDownLoad");
        isNeedEncryptAB = GUILayout.Toggle(isNeedEncryptAB, "   EncryptAB");
        GUILayout.Space(10);
        EditorGUILayout.LabelField("###################### Android #########################  ");
        GUILayout.Space(10);

        isNeedSDK = GUILayout.Toggle(isNeedSDK, "   NeedSDK");
        isBuildAndroidAb = GUILayout.Toggle(isBuildAndroidAb, "   build  AB");

        isBuildAndoridAPK = GUILayout.Toggle(isBuildAndoridAPK, "   build  APK");
        GUILayout.Space(10);
        if (GUILayout.Button("执行命令"))
        {
            ExeAndroidComand();
        }
        
        GUILayout.Space(10);
        EditorGUILayout.LabelField("####################### IOS ############################  ");
        GUILayout.Space(10);
        
        isBuildIOSAb = GUILayout.Toggle(isBuildIOSAb, "   build  AB");

        isExportXcode = GUILayout.Toggle(isExportXcode, "   ExportXcode");
        GUILayout.Space(10);
        EditorGUILayout.BeginHorizontal();
        iosEnvIndex = EditorGUILayout.Popup(iosEnvIndex, iosEnv, GUILayout.Width(150));
        GUILayout.Label("  企业版or测试版or正式版");
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(10);
        if (GUILayout.Button("执行命令"))
        {
            ExeIOSComand(); 
        }
        GUILayout.Space(10);
    }

    void ExeAndroidComand()
    {
#if UNITY_ANDROID
        //centurygame.Internal.InstallSdkInAndroid.isNeedAndroidSDK = isNeedSDK;
#endif
        if (isBuildAndroidAb)
        {
            //BuildABCommand.OnStartBuildAB(BuildTarget.Android);
            // ProjectBuild.BuildBundle();
            
            //if (isNeedEncryptAB)
            //{
            //    BuildABCommand.EncryptAB(PathManager.StreamingAssetsABPath);
            //}
        }

        if (isBuildAndoridAPK)
        {
            BuildPlayer(BuildTarget.Android, true, isNeedCheckDownLoad);
        }
    }

    void ExeIOSComand()
    {
        if (isBuildIOSAb)
        {
            BuildABCommand.OnStartBuildAB(BuildTarget.iOS);
            if (isNeedEncryptAB)
            {
                BuildABCommand.EncryptAB(PathManager.StreamingAssetsABPath);
            }
        }

        string env = null;
        if(iosEnvIndex == 0)
        {
            env = "intranet";
        }
        else if (iosEnvIndex == 1)
        {
            env = "dev";
        }
        else if (iosEnvIndex == 2)
        {
            env = "adhoc";
        }
        else if (iosEnvIndex == 3)
        {
            env = "appstore";
        }
        if (isExportXcode)
        {
            BuildPlayer(BuildTarget.iOS, true, isNeedCheckDownLoad, env);
        }
    }
    
    static void BackToDefaultSymbols(BuildTarget buildTarget)
    {
        if (buildTarget == BuildTarget.Android)
        {
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, BaseDefinition);
        }
        else if (buildTarget == BuildTarget.iOS)
        {
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, BaseDefinition);
        }
    }
    
    static void ExportData()
    {
        ExportTools.RemovePackLuaFile();
        ExportTools.RemovePackUnpackLua();
        ExportTools.RemovePBInStreamingAssets();

        ExportTools.PackLuaFile();
        ExportTools.PackUnpackLua();
        ExportTools.PackPBFile();
        AssetDatabase.Refresh();
    }
    static bool BuildPlayer(BuildTarget buildTarget, bool isDebug, bool isNeedCheckDownLoad, string env = null)
    {
        BuildWindows.env = env;
        PlayerSettingsConfig.SetConfig(buildTarget, isDebug);
        if (string.IsNullOrEmpty(env) == false && env.Equals("intranet_uwa"))
        {
            BUILD_SCENES = new string[] { "Assets/UWA/LuaMainSceneForIOSUWA.unity" };
        }
        if (string.IsNullOrEmpty(env) == false && 
            (env.Equals("uwa_android")
            || (env.Equals("uwa_android_got_online"))
            || (env.Equals("uwa_android_test"))))
        {
            BUILD_SCENES = new string[] { "Assets/UWAForAndorid/LuaMainSceneForAndroidUWA.unity" };
        }
        ExportTools.RemoveManifest(Application.streamingAssetsPath);
        BuildOptions _BuildOptions = BuildOptions.None;
        string exportPath = BuildConfig.ApkOutputPath;
        string def = BaseDefinition + ";" + ResFromAB;
        if (isNeedCheckDownLoad)
        {
            def = def + ";" + DownLoadAB;
        }

        if (buildTarget == BuildTarget.Android)
        {
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, def);
            exportPath = exportPath + "/" + PlayerSettingsConfig.productName + ".apk";
            if (string.IsNullOrEmpty(env) == false )
            {
                if(env.Equals("uwa_android_test") || env.Equals("android_test"))
                {
                    _BuildOptions = BuildOptions.Development | BuildOptions.ConnectWithProfiler | BuildOptions.AllowDebugging;
                    PlayerSettings.Android.forceSDCardPermission = true;
                }else if(env.Equals("uwa_android") || env.Equals("uwa_android_got_online"))
                {
                    _BuildOptions = BuildOptions.Development;
                    PlayerSettings.Android.forceSDCardPermission = true;
                }
                PlayerSettingsConfig.SetAndroidEnv(env);
            }
            else
            {
                _BuildOptions = BuildOptions.Development | BuildOptions.ConnectWithProfiler | BuildOptions.AllowDebugging;
            }

        }
        else if (buildTarget == BuildTarget.iOS)
        {
            exportPath = exportPath + "/" + "iosBuild";
            if(string.IsNullOrEmpty(env) == false)
            {
                PlayerSettingsConfig.SetIOSEnv(env, ref def);
            }else
            {
                _BuildOptions = BuildOptions.Development | BuildOptions.ConnectWithProfiler | BuildOptions.AllowDebugging;
            }
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, def);
        }
//#if UNITY_ANDROID
//        if (centurygame.Internal.InstallSdkInAndroid.isNeedAndroidSDK == false)
//        {
//            RemoveAndroidDLL();
//        }
//#endif
       
        AssetDatabase.Refresh();
        Debug.Log("on start build player " + PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.Android));
        bool isSuccess = ExportTools.BuildPlayer(exportPath, BUILD_SCENES, buildTarget, _BuildOptions);
        Debug.Log("isSuccess " + isSuccess);
        if (MacroDefinition.UNITY_EDITOR())
        {
            BackToDefaultSymbols(buildTarget);
            PlayerSettingsConfig.BackConfig();
            AssetDatabase.Refresh();
            ExportTools.ShowSuccessDialog(isSuccess, buildTarget.ToString(), BuildConfig.ApkOutputPath);
            //BuildTarget buildTarget, string pathToBuiltProject, string env
#if UNITY_IOS
            centurygame.Internal.InstallSdkInXcode.OnPostprocessBuild(buildTarget, exportPath);
#endif
#if UNITY_ANDROID

#endif
        }
        return isSuccess;
    }
}
