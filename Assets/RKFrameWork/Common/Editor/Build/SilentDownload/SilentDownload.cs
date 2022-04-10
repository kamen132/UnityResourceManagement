using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public partial class SilentDownload
{
    public static void OnSilentDownload()
    {
        Debug.Log("------------------OnSilentDownload--------------------");
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
        Debug.Log("------------------OnSilentDownload-----over---------------");
    }
  
}
