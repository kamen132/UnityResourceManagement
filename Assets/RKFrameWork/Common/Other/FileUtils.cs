//using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class FileUtils
{

    static StringBuilder _reusableStringBuilder = new StringBuilder();
    static MD5 _reusableMd5Computer = new MD5CryptoServiceProvider();

    public static void CopyFromSaToPD(string fromPath, string toPath)
    {
        FileCatchData data = FileManager.ReadStreamingAssets(fromPath);
        if(data.state == false)
        {
            return;
        }
        FileManager.WriteAllBytes(toPath, (byte[])data.data);
    }
    public static string GetAesStringByTextAsset(TextAsset asset)
    {
        if (asset != null)
        {
            byte[] data = asset.bytes;
            if (data != null)
            {
                data = AESManager.Decrypt(YQPackageManagerEX.KEY, data);
                return Encoding.UTF8.GetString(data, 0, data.Length);
            }
        }
        return null;
    }
    public static string GetPdAesString(string key, string path)
    {
        if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(path))
        {
            return null;
        }
        FileCatchData data = FileManager.ReadAllBytes(path);
        if (data == null || data.state == false)
        {
            return null;
        }
        byte[] result = (byte[])data.data;
        result = AESManager.Decrypt(key, result);
        return Encoding.UTF8.GetString(result);
    }

    public static byte[] GetPdAesBytes(string key, string path)
    {
        if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(path))
        {
            return null;
        }
        FileCatchData data = FileManager.ReadAllBytes(path);
        if (data == null || data.state == false)
        {
            return null;
        }
        byte[] result = (byte[])data.data;
        result = AESManager.Decrypt(key, result);
        return result;
    }

    public static string GetSaAesString(string key, string path)
    {
        if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(path))
        {
            return null;
        }
        FileCatchData data = FileManager.ReadStreamingAssets(path);
        if (data == null || data.state == false)
        {
            return null;
        }
        byte[] result = (byte[])data.data;
        result = AESManager.Decrypt(key, result);
        return Encoding.UTF8.GetString(result);
    }
    public static string GetBufferString(byte[] buffer, ref int length)
    {
        if (buffer == null)
        {
            return null;
        }
        int lengthString = GetBufferInt(buffer, ref length);
        byte[] tempStringBuffer = new byte[lengthString];
        for (int i = 0; i < lengthString; i++)
        {
            tempStringBuffer[i] = buffer[length + i];
        }
        length += lengthString;
        return Encoding.UTF8.GetString(tempStringBuffer);
    }

    public static int GetBufferInt(byte[] buffer, ref int length)
    {
        if (buffer == null)
        {
            return 0;
        }
        int data = System.BitConverter.ToInt32(buffer, length);
        length += 4;
        return data;
    }

    public static void WriteInt(System.IO.FileStream fileStream, int data)
    {
        if (fileStream == null)
        {
            return;
        }
        byte[] dataByte = System.BitConverter.GetBytes(data);
        fileStream.Write(dataByte, 0, dataByte.Length);
    }

    public static void WriteString(System.IO.FileStream fileStream, string data)
    {
        if (fileStream == null)
        {
            return;
        }
        int length = data.Length;
        byte[] lengthByte = BitConverter.GetBytes(length);
        fileStream.Write(lengthByte, 0, lengthByte.Length);
        byte[] dataBytes = Encoding.UTF8.GetBytes(data);
        fileStream.Write(dataBytes, 0, dataBytes.Length);
    }
    
    public static long GetFileLength(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            return 0;
        }
        try
        {
            System.IO.FileInfo fileTemp = new System.IO.FileInfo(filePath);
            if (fileTemp != null && fileTemp.Exists)
            {
                return fileTemp.Length;
            }
        }
        catch
        {

        }
        return -1;
    }
    public static void CheckFileMD5(string filePath, string md5, OnCallBackBool callBack)
    {
        AsyncTask asyncTask = new AsyncTask();
        asyncTask._OnRunInThread = (AsyncTask _asyncTask, List<object> args) =>
        {
            string tempMd5 = GetMD5HashFromFile(filePath);
            if (md5.Equals(tempMd5))
            {
                _asyncTask.SetResult(1);
            }
            else
            {
                _asyncTask.SetResult(2);
            }

        };
        asyncTask._OnCallBackInMainThread = (AsyncTask _asyncTask, AsyncTaskResult asyncTaskResult, List<object> args) =>
        {
            int result = _asyncTask.Result;
            callBack(result == 1);
        };
        asyncTask.execute(new List<object>());
    }
    public static bool CheckFileMD5Equal(string zipPath, string md5)
    {
        bool legalFile = false;
        if (!string.IsNullOrEmpty(md5) && !string.IsNullOrEmpty(zipPath))
        {
            string md5_local = GetMD5HashFromFile(zipPath);
            if (string.IsNullOrEmpty(md5_local) == false && md5_local.ToLower().Equals(md5.ToLower()))
            {
                legalFile = true;
            }
            else
            {
                Debug.Log("xxx md5 not right 1=" + md5_local + "  2==" + md5);
            }
        }
        return legalFile;
    }

    public static string GetStringMD5(string myString)
    {
        if (string.IsNullOrEmpty(myString))
        {
            return string.Empty;
        }
        byte[] fromData = System.Text.Encoding.UTF8.GetBytes(myString);
        byte[] targetData = _reusableMd5Computer.ComputeHash(fromData);
        _reusableStringBuilder.Length = 0;
        for (int i = 0; i < targetData.Length; i++)
        {
            _reusableStringBuilder.Append(targetData[i].ToString("x").PadLeft(2, '0'));
        }

        return _reusableStringBuilder.ToString();
    }

    public static string GetMD5HashFromFile(string filePath)
    {
        if (string.IsNullOrEmpty(filePath) || File.Exists(filePath) == false)
        {
            return null;
        }
        try
        {
            FileStream file = new FileStream(filePath, FileMode.Open);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] retVal = md5.ComputeHash(file);
            file.Close();

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++)
            {
                sb.Append(retVal[i].ToString("x2"));
            }
            return sb.ToString();
        }
        catch (Exception ex)
        {
            Debug.LogError("GetMD5HashFromFile() fail,error:" + ex.Message);
            return null;
        }
    }

    public static long GetFileSize(string filePath)
    {
        if (string.IsNullOrEmpty(filePath) || File.Exists(filePath) == false)
        {
            return 0;
        }
        try
        {
            FileInfo info = new FileInfo(filePath);
            return info.Length;
        }
        catch (Exception ex)
        {
            throw new Exception("GetFileSize() fail,error:" + ex.Message);
        }
    }

    /*public static bool DecompressZip(string zipFile, string outFolder, string password = null)
    {
        bool result = false;
        FileStream fs = null;
        ZipInputStream zipStream = null;
        ZipEntry ent = null;
        try
        {
            if (string.IsNullOrEmpty(zipFile) || string.IsNullOrEmpty(outFolder) || zipFile.ToLower().Equals(outFolder.ToLower()))
            {
                return false;
            }
            string fileName = null;

            var stream = File.OpenRead(zipFile);
            if (!Directory.Exists(outFolder))
            {
                Directory.CreateDirectory(outFolder);
            }
            ZipConstants.DefaultCodePage = 0;

            zipStream = new ZipInputStream(stream);
            if(string.IsNullOrEmpty(password) == false)
            {
                zipStream.Password = password;
            }
           
            while ((ent = zipStream.GetNextEntry()) != null)
            {
                if (!string.IsNullOrEmpty(ent.Name))
                {
                    fileName = Path.Combine(outFolder, ent.Name);
                    #region Android
                    fileName = fileName.Replace('\\', '/');

                    if (fileName.EndsWith("/"))
                    {
                        Directory.CreateDirectory(fileName);
                        continue;
                    }
                    #endregion
                    if (!Directory.Exists(Path.GetDirectoryName(fileName)))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(fileName));
                    }
                    if(File.Exists(fileName))
                    {
                        //Debug.Log("xxx DecompressZip file exist:" + fileName);
                        File.Delete(fileName);
                    }
                    fs = File.Create(fileName);
                    int size = 2048;
                    byte[] data = new byte[size];

                    while (true)
                    {
                        size = zipStream.Read(data, 0, data.Length);

                        if (size > 0)
                        {
                            fs.Write(data, 0, size);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            fs.Flush();
            result = true;
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString() + "file:" + zipFile);
        }
        finally
        {
            if (fs != null)
            {
                fs.Close();
                fs.Dispose();
            }
            if (zipStream != null)
            {
                zipStream.Close();
                zipStream.Dispose();
            }
            if (ent != null)
            {
                ent = null;
            }
            GC.Collect();
            GC.Collect(1);
        }
        return result;
    }*/

    // level 0-9,数值越大，压缩率越高
    public static bool CompressZip(string fromDirPath, string targetZIPPath, int level, string password)
    {
        //这里需要完善
        return false;
        // if (string.IsNullOrEmpty(fromDirPath) || string.IsNullOrEmpty(targetZIPPath))
        // {
        //     return false;
        // }
        //
        // FileManager.DeleteFile(targetZIPPath);
        // FileManager.CreateDirectory(Path.GetDirectoryName(targetZIPPath));
        // using (ZipOutputStream s = new ZipOutputStream(File.Create(targetZIPPath)))
        // {
        //     if (string.IsNullOrEmpty(password) == false)
        //     {
        //         s.Password = password;
        //     }
        //     s.SetLevel(level);
        //     Compress(fromDirPath, s, fromDirPath);
        //     s.Finish();
        //     s.Close();
        // }
        // return true;
    }

    // static void Compress(string source, ZipOutputStream s, string basePath)
    // {
    //     string[] filenames = Directory.GetFileSystemEntries(source);
    //     foreach (string file in filenames)
    //     {
    //         if (Directory.Exists(file))
    //         {
    //             Compress(file, s, basePath);
    //         }
    //         else
    //         {
    //             using (FileStream fs = File.OpenRead(file))
    //             {
    //                 byte[] buffer = new byte[4 * 1024];
    //                 string path = PathUtil.Replace(file);
    //                 path = path.Replace(basePath, "");
    //                 path = path.Substring(1);
    //                 ZipEntry entry = new ZipEntry(path);
    //                 entry.DateTime = DateTime.Now;
    //                 s.PutNextEntry(entry);
    //                 int sourceBytes;
    //                 do
    //                 {
    //                     sourceBytes = fs.Read(buffer, 0, buffer.Length);
    //                     s.Write(buffer, 0, sourceBytes);
    //                 } while (sourceBytes > 0);
    //             }
    //         }
    //     }
    // }
    //
    // public static bool PathIsDirectory(string absolutePath)
    // {
    //     FileAttributes attr = File.GetAttributes(absolutePath);
    //     if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
    //         return true;
    //     else
    //         return false;
    // }


    /// <summary>
    /// Given an absolute path, return a path rooted at the Assets folder.
    /// </summary>
    /// <remarks>
    /// Asset relative paths can only be used in the editor. They will break in builds.
    /// </remarks>
    /// <example>
    /// /Folder/UnityProject/Assets/resources/music returns Assets/resources/music
    /// </example>
    public static string AssetsRelativePath(string absolutePath)
    {
        if (absolutePath.StartsWith(Application.dataPath))
        {
            return "Assets" + absolutePath.Substring(Application.dataPath.Length);
        }
        else
        {
            throw new System.ArgumentException("Full path does not contain the current project's Assets folder", "absolutePath");
        }
    }


    /// <summary>
    /// Get all available Resources directory paths within the current project.
    /// </summary>
    public static string[] GetResourcesDirectories()
    {
        List<string> result = new List<string>();
        Stack<string> stack = new Stack<string>();
        // Add the root directory to the stack
        stack.Push(Application.dataPath);
        // While we have directories to process...
        while (stack.Count > 0)
        {
            // Grab a directory off the stack
            string currentDir = stack.Pop();
            try
            {
                foreach (string dir in Directory.GetDirectories(currentDir))
                {
                    if (Path.GetFileName(dir).Equals("Resources"))
                    {
                        // If one of the found directories is a Resources dir, add it to the result
                        result.Add(dir);
                    }
                    // Add directories at the current level into the stack
                    stack.Push(dir);
                }
            }
            catch
            {
                Debug.LogError("Directory " + currentDir + " couldn't be read from.");
            }
        }
        return result.ToArray();
    }

    public static List<string> GetAllResources(string dirPath, string rootPath)
    {
        if (string.IsNullOrEmpty(dirPath))
        {
            Debug.LogError("dirPath is null or empty");
            return null;
        }

        string fullPath = Path.Combine(rootPath, dirPath);

        if (!Directory.Exists(fullPath))
        {
            Debug.LogError("dirPath not exitst.");
        }

        DirectoryInfo dir = new DirectoryInfo(fullPath);

        FileInfo[] fileInfo = dir.GetFiles("*.*", SearchOption.AllDirectories);

        List<string> resList = new List<string>();

        foreach (FileInfo file in fileInfo)
        {
            if (file.Name[0] == '.')
            {
                continue;
            }

            if (file.Extension == ".meta")
            {
                continue;
            }

            //resList.Add(file.FullName.Replace(rootPath, string.Empty));
            resList.Add(file.FullName.Substring(file.FullName.IndexOf("Assets")));
        }

        return resList;
    }
}
