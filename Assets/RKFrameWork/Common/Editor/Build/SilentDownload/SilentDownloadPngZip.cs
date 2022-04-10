using System.IO;
using UnityEditor;
using UnityEngine;

public class SilentDownloadPngZip
{
    //[MenuItem("Build/SilentDownloadPngZip", false)]
    public static void BuildDownloadPngZip()
    {
        string path = Application.dataPath + "/WorkAssets/textures_download_zip";
        string[] dirs = Directory.GetDirectories(path);
        string OutBuildPath = Application.dataPath + "/../OutBuild/";
        string SavePath = Application.dataPath + "/../../../../SaveData";

        foreach (string dir in dirs)
        {
            string folderName = PathUtil.GetFolderName(dir);
            string tempSavePath = OutBuildPath + folderName;
            FileManager.DeleteDirectory(tempSavePath);
            string fromDir = PathUtil.Replace(dir);
            FileManager.CopyDirectory(fromDir, tempSavePath);
            ExportTools.RemoveMeta(tempSavePath);
            string zipPath = string.Format("{0}/PngZip/{1}.zip", SavePath, folderName);
            FileUtils.CompressZip(tempSavePath, zipPath, 8, YQPackageManagerEX.ZipPassword);
        }
    }
}
