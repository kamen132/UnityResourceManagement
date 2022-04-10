using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class YQPackageManagerEX
{
    public static string ZipPassword = "dsfsdl*()*(&)lsfm5631456";
    public static string KEY = "gfddsfgdf54dfg7812e7wn3654f*&*&(FJIIWOD";
    public static string KEY2 = "gfddgsfsdfsa8979-0wn3654f*&*&(FJIIWOD";
    public static string ABKEY = "015482dd6789abcdef";
    private static Dictionary<string, byte[]> luaData = new Dictionary<string, byte[]>();
    public static int bundleOffSize = 17;
    public byte[] GetLuaDataByName(string name)
    {
        name = name.ToLower();
        name = name.Replace("/", ".");
        if (luaData != null && luaData.ContainsKey(name))
        {
            return luaData[name];
        }
        Debug.Log("xxx error cannt find name =  " + name);
        return null;
    }

    public string GetABName(string path)
    {
        return path;
    }
    //public bool UnPackByteDataWithVersion(string path, string sign, string dataDirectoryPath, int version)
    //{
    //    FileCatchData _FileCatchData = FileManager.ReadAllBytes(path);
    //    if (_FileCatchData.state == false)
    //    {
    //        return false;
    //    }
    //    byte[] packageData = (byte[])_FileCatchData.data;
    //    if (packageData == null)
    //    {
    //        return false;
    //    }
    //    int totleLength = packageData.Length;
    //    FileManager.DeleteDirectory(dataDirectoryPath);
    //    FileManager.CreateDirectory(dataDirectoryPath);
    //    int length = 0;

    //    FileUtils.GetBufferString(packageData, ref length);
    //    FileUtils.GetBufferString(packageData, ref length);

    //    string assetBundleFilePath = Path.Combine(dataDirectoryPath, sign);
    //    bool isSuccess = LoadByteData(packageData, ref length, assetBundleFilePath, dataDirectoryPath);
    //    if (isSuccess == false) return false;

    //    string dependenciesFilePath = Path.Combine(dataDirectoryPath, sign + ".info");
    //    isSuccess = LoadByteData(packageData, ref length, dependenciesFilePath, dataDirectoryPath);
    //    if (isSuccess == false) return false;

    //    string luaDataFilePath = Path.Combine(dataDirectoryPath, "info.info");
    //    isSuccess = LoadByteData(packageData, ref length, luaDataFilePath, dataDirectoryPath);
    //    if (isSuccess == false) return false;

    //    Debug.Log("xxx length===" + length);
    //    Debug.Log("xxx totleLength===" + totleLength);

    //    if (totleLength > length)
    //    {
    //        string unPackluaDataFilePath = Path.Combine(PathManager.GetRootDataPath, PathManager.download_unpackLuaName);
    //        isSuccess = LoadByteData(packageData, ref length, unPackluaDataFilePath, dataDirectoryPath);
    //        if (isSuccess == false) return false;
    //        if (totleLength > length)
    //        {
    //            string unPackAssetBundleFilePath = Path.Combine(PathManager.GetRootDataPath, "unpack_data");
    //            isSuccess = LoadByteData(packageData, ref length, unPackAssetBundleFilePath, dataDirectoryPath);
    //            if (isSuccess == false) return false;

    //            string unPackDependenciesFilePath = Path.Combine(PathManager.GetRootDataPath, "unpack_data_info.info");
    //            isSuccess = LoadByteData(packageData, ref length, unPackDependenciesFilePath, dataDirectoryPath);
    //            if (isSuccess == false) return false;
    //        }
    //    }
    //    return true;
    //}

    //public bool UnPackByteData(string path, string sign, string dataDirectoryPath, string AssetBundleKey, string InfoKey)
    //{
    //    FileCatchData _FileCatchData = FileManager.ReadAllBytes(path);
    //    if (_FileCatchData.state == false)
    //    {
    //        return false;
    //    }
    //    byte[] packageData = (byte[])_FileCatchData.data;
    //    if (packageData == null)
    //    {
    //        return false;
    //    }
    //    int totleLength = packageData.Length;
    //    FileManager.DeleteDirectory(dataDirectoryPath);
    //    FileManager.CreateDirectory(dataDirectoryPath);
    //    int length = 0;

    //    FileUtils.GetBufferString(packageData, ref length);
    //    FileUtils.GetBufferString(packageData, ref length);

    //    string assetBundleFilePath = Path.Combine(dataDirectoryPath, sign);
    //    bool isSuccess = LoadByteData(packageData, ref length, assetBundleFilePath, dataDirectoryPath);
    //    if (isSuccess == false) return false;

    //    string dependenciesFilePath = Path.Combine(dataDirectoryPath, sign + ".info");
    //    isSuccess = LoadByteData(packageData, ref length, dependenciesFilePath, dataDirectoryPath);
    //    if (isSuccess == false) return false;

    //    string luaDataFilePath = Path.Combine(dataDirectoryPath, "info.info");
    //    isSuccess = LoadByteData(packageData, ref length, luaDataFilePath, dataDirectoryPath);
    //    if (isSuccess == false) return false;

    //    Debug.Log("xxx length===" + length);
    //    Debug.Log("xxx totleLength===" + totleLength);

    //    if (totleLength > length)
    //    {
    //        string SoundDirPath = Path.Combine(dataDirectoryPath, PathManager.save_sound_path_name);
    //        isSuccess = LoadSoundByteData(packageData, ref length, SoundDirPath);
    //        if (isSuccess == false) return false;
    //    }

    //    if (totleLength > length)
    //    {
    //        string unPackluaDataFilePath = Path.Combine(PathManager.GetRootDataPath, PathManager.download_unpackLuaName);
    //        isSuccess = LoadByteData(packageData, ref length, unPackluaDataFilePath, dataDirectoryPath);
    //        if (isSuccess == false) return false;
    //        if (totleLength > length)
    //        {
    //            string unPackAssetBundleFilePath = Path.Combine(PathManager.GetRootDataPath, "unpack_data");
    //            isSuccess = LoadUncertainByteData(packageData, ref length, unPackAssetBundleFilePath, dataDirectoryPath);
    //            if (isSuccess == false) return false;

    //            string unPackDependenciesFilePath = Path.Combine(PathManager.GetRootDataPath, "unpack_data_info.info");
    //            isSuccess = LoadUncertainByteData(packageData, ref length, unPackDependenciesFilePath, dataDirectoryPath);
    //            if (isSuccess == false) return false;
    //        }
    //    }
    //    return true;
    //}

    bool LoadSoundByteData(byte[] packageData, ref int length, string outPath)
    {
        FileManager.DeleteDirectory(outPath);
        FileManager.CreateDirectory(outPath);
        int isHave = FileUtils.GetBufferInt(packageData, ref length);
        if (isHave == 1)
        {
            string md5 = FileUtils.GetBufferString(packageData, ref length);
            int dataTotleLength = FileUtils.GetBufferInt(packageData, ref length);
            int count = FileUtils.GetBufferInt(packageData, ref length);
            for (int i = 0; i < count;i++)
            {
                string fileName = FileUtils.GetBufferString(packageData, ref length);
                int dataLength = FileUtils.GetBufferInt(packageData, ref length);
                FileCatchData _FileCatchData = FileManager.WriteAllBytes(Path.Combine(outPath, fileName), packageData, length, dataLength);
                length += dataLength;
            }
        }
        return true;
    }
    bool LoadUncertainByteData(byte[] packageData, ref int length, string outPath, string dataDirectoryPath)
    {
        int isHave = FileUtils.GetBufferInt(packageData, ref length);
        if(isHave == 1)
        {
            return LoadByteData(packageData, ref length, outPath, dataDirectoryPath);
        }
        return true;
    }
    bool LoadByteData(byte[] packageData,  ref int length, string outPath, string dataDirectoryPath)
    {
        FileUtils.GetBufferString(packageData, ref length);
        int dataLength = FileUtils.GetBufferInt(packageData, ref length);
        FileCatchData _FileCatchData = FileManager.WriteAllBytes(outPath, packageData, length, dataLength);
        if (_FileCatchData == null || _FileCatchData.state == false)
        {
            FileManager.DeleteDirectory(dataDirectoryPath);
            return false;
        }
        length += dataLength;
        return true;
    }

    bool LoadStringData(byte[] packageData, ref int length, string outPath, string dataDirectoryPath)
    {
        //string infoMd5 =
        FileUtils.GetBufferString(packageData, ref length);
        string info = FileUtils.GetBufferString(packageData, ref length);
        FileCatchData _FileCatchData = FileManager.WriteAllText(outPath, info);
        if (_FileCatchData == null || _FileCatchData.state == false)
        {
            FileManager.DeleteDirectory(dataDirectoryPath);
            return false;
        }
        return true;
    }
    //public void LoadAllLuaData()
    //{
    //    LoadLuaDataFromResource(PathUtil.RemoveFileExtension(PathManager.needPackLuaName));
    //}

    public bool LoadLuaDataFromResource(string path)
    {
        path = PathUtil.RemoveResourcePath(path);
        TextAsset fileres = Resources.Load<TextAsset>(path);
        if(fileres == null)
        {
            return false;
        }
        byte[] raws = fileres.bytes;
        return LoadLua(raws);
    }

    public bool LoadLuaDataFromSA(string path)
    {
        FileCatchData result = FileManager.ReadStreamingAssets(path);
        if (result.state == false)
        {
            return false;
        }
        byte[] raws = (byte[])result.data;
        return LoadLua(raws);
    }
    public bool LoadLuaDataFromPD(string path)
    {
        FileCatchData result = FileManager.ReadAllBytes(path);
        if (result.state == false)
        {
            return false;
        }
        byte[] raws = (byte[])result.data;
        return LoadLua(raws);
    }
    public bool LoadLuaData(string path)
    {
        FileCatchData _FileCatchData = FileManager.ReadAllBytes(path);
        if (_FileCatchData.state == false)
        {
            return false;
        }
        byte[] packageData = (byte[])_FileCatchData.data;
        if (packageData == null)
        {
            return false;
        }
        return LoadLua(packageData);
    }

    bool LoadLua(byte[] packageData)
    {
        int length = 0;
        int count = FileUtils.GetBufferInt(packageData, ref length);
        for (int i = 0; i < count; i++)
        {
            string key = FileUtils.GetBufferString(packageData, ref length);
            int itemLength = FileUtils.GetBufferInt(packageData, ref length);
            //byte[] itemBytes = new byte[itemLength];
            //Buffer.BlockCopy(packageData, length, itemBytes, 0, itemLength); // Buffer.BlockCopy 性能高于 Array.Copy
            //itemBytes = AESManager.Decrypt(KEY, itemBytes);
            luaData[key] = AESManager.Decrypt(KEY, packageData, length, itemLength);
            length += itemLength;
        }
        return true;
    }

    public FileCatchData GetPBData(string path, bool isSA)
    {
        FileCatchData data = null;
        if(isSA)
        {
            data = FileManager.ReadStreamingAssets(path);
        }else
        {
            data = FileManager.ReadAllBytes(path);
        }
        data.data = AESManager.Decrypt(KEY, (byte[])data.data);
        return data;
    }

    public void Clear()
    {
        luaData.Clear();
    }
    private static volatile YQPackageManagerEX ms_Instance;
    public static YQPackageManagerEX Instance
    {
        get
        {
            if (ms_Instance == null)
            {
                ms_Instance = new YQPackageManagerEX();
            }
            return ms_Instance;
        }
    }
}
