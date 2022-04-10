using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class SilentDownloadSpineZip
{
    //[MenuItem("Build/SilentDownloadSpineZip", false)]
    public static void BuildDownloadSpineZip()
    {
        string path = Application.dataPath + "/WorkAssets/spine_download_zip";
        OnBuildDownloadSpineZip(path);
    }
    private static void OnBuildDownloadSpineZip(string dirPath)
    {
        string[] dirs = Directory.GetDirectories(dirPath);
        foreach (string dir in dirs)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(dir);
            if (BuildABCommand.IsHasSubDir(dirInfo))
            {
                OnBuildDownloadSpineZip(dir);
                if (BuildABCommand.IsHasFiles(dirInfo))
                {
                    MakeSpineZip(dir);
                }
            }
            else
            {
                MakeSpineZip(dir);
            }
        }
    }

    private static void MakeSpineZip(string dirPath)
    {
        string OutBuildPath = Application.dataPath + "/../OutBuild/";
        string SavePath = Application.dataPath + "/../../../../SaveData";
        string folderName = PathUtil.GetPathBySubstring(dirPath, "spine_download_zip");
        folderName = folderName.Replace("/", "_");
        Debug.Log("folderName==" + folderName);
        string tempSavePath = OutBuildPath + folderName;
        FileManager.DeleteDirectory(tempSavePath);
        string fromDir = PathUtil.Replace(dirPath);
        FileManager.CopyDirectory(fromDir, tempSavePath);
        ExportTools.RemoveMeta(tempSavePath);
        //加密txt文件；
        string[] files = FileManager.GetAllFilesInFolder(tempSavePath);
        foreach (string file in files)
        {
            if (file.EndsWith(".txt") || file.EndsWith(".json"))
            {
                byte[] dataBytes = (byte[])FileManager.ReadAllBytes(file).data;
                dataBytes = AESManager.Encrypt(YQPackageManagerEX.KEY2, dataBytes);
                FileManager.DeleteFile(file);
                FileManager.WriteAllBytes(file, dataBytes);
            }
        }
        string zipPath = string.Format("{0}/SpineZip/{1}.zip", SavePath, folderName);
        Debug.Log("zipPath==" + zipPath);
        FileUtils.CompressZip(tempSavePath, zipPath, 8, YQPackageManagerEX.ZipPassword);
    }
}
