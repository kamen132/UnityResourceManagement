using System.IO;
using UnityEditor;
using UnityEngine;


public partial class StatesWindows : EditorWindow
{

    private static string BaseDefinition = "ENABLE_IL2CPP;THREAD_SAFE;BESTHTTP_DISABLE_SERVERSENT_EVENTS;BESTHTTP_DISABLE_SIGNALR;"
        + "BESTHTTP_DISABLE_UNITY_FORM;BESTHTTP_DISABLE_ALTERNATE_SSL;BESTHTTP_DISABLE_SOCKETIO";//开发所用宏

    private const string ResFromAB = "RES_FROM_AB";
    private const string UseHotfixDLL = "USE_HOTFIX_DLL";
    private const string DownLoadAB = "DOWNLOAD_AB";

    public static StatesWindows window = null;
    [MenuItem("Build/Open States Window", false)]
    public static void OpenExportApkWindow()
    {
        StatesWindows window = (StatesWindows)GetWindow(typeof(StatesWindows));  //定义一个窗口对象  
        float width = 500;
        float height = 500;
        float posX = 500f;
        float posY = 300f;
        window.position = new Rect(posX, posY, width, height);
        StatesWindows.window = window;
        window.OnInit();
    }
    private bool DOWNLOAD_AB = false;
    private bool RES_FROM_AB = false;
    private bool USE_HOTFIX_DLL = false;
    
    public void OnInit()
    {
        DOWNLOAD_AB = MacroDefinition.DOWNLOAD_AB();
        RES_FROM_AB = MacroDefinition.RES_FROM_AB();
        USE_HOTFIX_DLL = MacroDefinition.USE_HOTFIX_DLL();
    }

    void OnGUI()
    {
        GUILayout.Space(10);
        if (GUILayout.Button("use unSafe"))
        {
            PlayerSettings.allowUnsafeCode = true;
            string def = BaseDefinition + ";" + "USE_UnSAFE";
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, def);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, def);
        }
        if (GUILayout.Button("unuse unSafe"))
        {
            PlayerSettings.allowUnsafeCode = false;
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, BaseDefinition);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, BaseDefinition);
        }
        GUILayout.Space(10);
        Application.runInBackground = GUILayout.Toggle(Application.runInBackground, "   RunInBackground");
        EditorGUILayout.LabelField("====================================== ");
        GUILayout.Space(10);
        if (GUILayout.Button("Clear All Symbols"))
        {
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, "");
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, "");
        }
        GUILayout.Space(10);
        EditorGUILayout.LabelField("====================================== ");
        GUILayout.Space(10);
        DOWNLOAD_AB = GUILayout.Toggle(DOWNLOAD_AB, "   DOWNLOAD_AB");
        RES_FROM_AB = GUILayout.Toggle(RES_FROM_AB, "   RES_FROM_AB");
        USE_HOTFIX_DLL = GUILayout.Toggle(USE_HOTFIX_DLL, "   ILRuntime_FROM_AB");
        
        GUILayout.Space(10);
        if (GUILayout.Button("执行命令"))
        {
            ExeIOSComand();
        }
        //if (GUILayout.Button("bspathch"))
        //{
        //    TestBsPatch();
        //}
    }

    void TestBsPatch()
    {
        string fromPath = Application.persistentDataPath + "/1111/machinedancinglion.ab";
        string patchPath = Application.persistentDataPath + "/1111/patch";
        
        string targetPath = Application.persistentDataPath + "/1111/new.ab";
        //int result =  NativeCenter.PatchFile(fromPath, patchPath, targetPath);
        //Debug.Log("11===" + patchPath);
        //Debug.Log("result===" + FileManager.FileExist(fromPath) + "---" + result);
    }
    void ExeIOSComand()
    {
        string def = BaseDefinition;
        if(DOWNLOAD_AB)
        {
            def = def + ";" + DownLoadAB;
        }
        if (RES_FROM_AB)
        {
            def = def + ";" + ResFromAB;
        }
        if (USE_HOTFIX_DLL)
        {
            def = def + ";" + UseHotfixDLL;
        }
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, def);
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, def);
    }


}
