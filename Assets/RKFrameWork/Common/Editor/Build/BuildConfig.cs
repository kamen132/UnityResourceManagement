using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BuildConfig
{
    public static bool IsLoadAssetPost = false;


    public const string data_info_name = "dependencies";
    public const string unpack_data_info_name = "unpack_dependencies";

    public const string out_sound_name = "sound_data";
    public const string lua_info_name = "lua_data";
    public const string unpack_lua_info_name = "unpack_lua_data";
    public const string prefab_name = "pb_na";
    public const string dependencies = "dp";
    
    public static bool IsBuildAssetBundle = false; //build apk时候，是否需要重新build assetbundle
    

    public static string ResourceOutPutPath = Application.dataPath + "/../OutResourcePack";//Resource build出的bandle 的目标目录

    public static string ApkOutputPath = Application.dataPath + "/../OutBuild";
   
    public static string ZIPSuffix = ".zip";        //后缀
    
    public static string ResourcesInfoName = "resources.info";
    
    //public static string BuildSourcePath = "";  //真正要build的源目录
    //public static string BuildOutPutPath = "";  //真正要build的源目录

    public static string ASSETBUNDLE_PATH = "";
    public static string ASSETBUNDLE_FILENAME = "";

    public static string RES_VERSION = "";
    public static List<string> InAppMachines = new List<string>()
    {
        //"MachineJalapenoFiesta",
        "MachineDancingLion"
    };

}
