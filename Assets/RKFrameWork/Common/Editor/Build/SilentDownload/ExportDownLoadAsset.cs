using System.IO;
using UnityEditor;
using UnityEngine;

public class ExportDownLoadAsset
{
    public static void OnExportDownLoadAsset()
    {
        Debug.Log("------------------OnExportDownLoadAsset--------------------");
        string[] args = System.Environment.GetCommandLineArgs();
        int count = args.Length;
        string forWhat = "";
      
        for (int i = 0; i < count; i++)
        {
            if (args[i] == "-forWhat")
            {
                forWhat = args[i + 1];
            }
        }
        if(forWhat.Equals("PngZip"))
        {
            SilentDownloadPngZip.BuildDownloadPngZip();
        }
        else if(forWhat.Equals("SpineZip"))
        {
            SilentDownloadSpineZip.BuildDownloadSpineZip();
        }
    }
}
