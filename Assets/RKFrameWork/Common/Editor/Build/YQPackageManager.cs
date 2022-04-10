using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PackageHeader //包头信息
{
    public string sign;
    public string version;
    public Dictionary<string, string> fileInfos = new Dictionary<string, string>();
    public Dictionary<string, string> uncertainFiles = new Dictionary<string, string>();
}

public class PackageFileEntry
{
    public string sign;
    public string version;
    public Dictionary<string, List<string>> mInfoDataDic = new Dictionary<string, List<string>>();
    public AssetBundle assetBundle;
}

public class YQPackageManager
{
    private PackageHeader _PackageHeader;

    void CheckHeader()
    {
        if (_PackageHeader == null)
        {
            _PackageHeader = new PackageHeader();
            _PackageHeader.sign = PathManager.PACKAGE_FILE_SIGN;
            //string versionFilePath = PathManager.UnPackFonderPath + "/config.txt";
            //Debug.Log("versionFilePath===" + versionFilePath);
            //FileCatchData _FileCatchData = FileManager.ReadAllText(versionFilePath);
            //if (_FileCatchData.state)
            //{
            //    string jsonData = (string)_FileCatchData.data;
            //    JSONObject obj = JSONObject.Create(jsonData);
            //    if (obj != null)
            //    {
            //        JSONObject ClientVersion = obj.GetField("ResVersion");
            //        if (ClientVersion != null && ClientVersion.IsString)
            //        {
            //            _PackageHeader.version = ClientVersion.str;
            //            BuildConfig.RES_VERSION = _PackageHeader.version;
            //            Debug.Log("BuildConfig.RES_VERSION====" + BuildConfig.RES_VERSION);
            //        }
            //    }
            //}
        }
    }

    public void AddFile(string filename, string filePath)
    {
        CheckHeader();
        _PackageHeader.fileInfos[filename] = filePath;
    }

    public void AddUncertainFiles(string filename, string filePath)
    {
        CheckHeader();
        _PackageHeader.uncertainFiles[filename] = filePath;
    }

    public void Flush()
    {
        if (_PackageHeader == null)
        {
            return;
        }

        //string fileName = BuildConfig.PACKAGE_NAME + BuildConfig.RES_VERSION;
        //string filePath = Path.Combine(BuildConfig.ResourceOutPutPath, BuildConfig.BuildTarget + "/" + fileName);
        //try
        //{
        //    FileManager.DeleteFile(filePath);
        //    System.IO.FileStream fileStream = new System.IO.FileStream(filePath, System.IO.FileMode.Append);
        //    FileUtils.WriteString(fileStream, _PackageHeader.sign);
        //    FileUtils.WriteString(fileStream, _PackageHeader.version);

        //    foreach (KeyValuePair<string, string> pair in _PackageHeader.fileInfos)
        //    {
        //        string name = pair.Key;
        //        string path = pair.Value;
        //        FileCatchData data = FileManager.ReadAllBytes(path);
        //        if (data.state == true)
        //        {
        //            byte[] fileBytes = (byte[])data.data;
        //            string md5 = FileUtils.GetMD5HashFromFile(path);
        //            FileUtils.WriteString(fileStream, md5);
        //            FileUtils.WriteInt(fileStream, fileBytes.Length);
        //            fileStream.Write(fileBytes, 0, fileBytes.Length);
        //        }
        //        else
        //        {
        //            Debug.Log("error------------------");
        //        }
        //    }
        //    Debug.Log("xxx uncertainFiles count===" + _PackageHeader.uncertainFiles.Count);
        //    //只要有的都放进去
        //    foreach (KeyValuePair<string, string> pair in _PackageHeader.uncertainFiles)
        //    {
        //        string name = pair.Key;
        //        string path = pair.Value;
        //        bool isFileExist = FileManager.FileExist(path);
        //        if(isFileExist)
        //        {
        //            FileUtils.WriteInt(fileStream, 1);
        //            FileCatchData data = FileManager.ReadAllBytes(path);
        //            if (data.state == true)
        //            {
        //                byte[] fileBytes = (byte[])data.data;
        //                string md5 = FileUtils.GetMD5HashFromFile(path);
        //                FileUtils.WriteString(fileStream, md5);
        //                FileUtils.WriteInt(fileStream, fileBytes.Length);
        //                fileStream.Write(fileBytes, 0, fileBytes.Length);
        //            }
        //        }
        //        else
        //        {
        //            FileUtils.WriteInt(fileStream, 0);
        //        }
        //    }
        //    fileStream.Close();
        //}
        //catch
        //{

        //}
        //_PackageHeader = null;
    }
    public void Clear()
    {
        _PackageHeader = null;
    }
    private static volatile YQPackageManager ms_Instance;
    public static YQPackageManager Instance
    {
        get
        {
            if (ms_Instance == null)
            {
                ms_Instance = new YQPackageManager();
            }
            return ms_Instance;
        }
    }
}
