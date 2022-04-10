using System.IO;
using System.Text;

namespace GMResChecker
{
    /// <summary>
    /// 包外文件管理器
    /// </summary>
    /// 
    public class FileCatchData
    {
        public bool state = true; //操作是否成功
        public string message = ""; //失败之后的信息
        public object data = null; //需要返回的数据
    }
    public class FileManager
    {
        public static bool FileExist(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return false;
            }
            return File.Exists(path);
        }
        public static bool DirectoryExist(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return false;
            }
            return Directory.Exists(path);
        }

        public static FileCatchData FileMove(string fromPath, string targetPath)
        {
            FileCatchData fileCatchData = new FileCatchData();
            if (string.IsNullOrEmpty(fromPath) || string.IsNullOrEmpty(targetPath))
            {
                fileCatchData.state = false;
                return fileCatchData;
            }
            try
            {
                CreateDirectory(PathUtil.GetFilesParentFolder(targetPath));
                File.Move(fromPath, targetPath);
            }
            catch (System.Exception e)
            {
                fileCatchData.state = false;
                fileCatchData.message = e.Message;
            }
            return fileCatchData;
        }

        /// 读取文件
        public static FileCatchData ReadAllText(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }

            path = PathUtil.Replace(path);
            string _info = string.Empty;
            FileCatchData fileCatchData = new FileCatchData();
            try
            {
                if (File.Exists(path))
                {
                    _info = File.ReadAllText(path);
                    fileCatchData.state = true;
                }
                else
                {
                    fileCatchData.state = false;
                }
            }
            catch (System.Exception e)
            {
                _info = string.Empty;
                fileCatchData.state = false;
                fileCatchData.message = e.Message;
            }
            fileCatchData.data = _info;
            return fileCatchData;
        }

        public static string[] ReadAllLines(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }
            if (File.Exists(path))
            {
                return File.ReadAllLines(path);
            }
            return null;
        }

        public static FileCatchData ReadAllBytes(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }
            byte[] bytes = null;
            FileCatchData fileCatchData = new FileCatchData();
            try
            {
                if (File.Exists(path))
                {
                    bytes = File.ReadAllBytes(path);
                    fileCatchData.state = true;
                }
                else
                {
                    fileCatchData.state = false;
                }
            }
            catch (System.Exception e)
            {
                bytes = null;
                fileCatchData.state = false;
                fileCatchData.message = e.Message;
            }
            fileCatchData.data = bytes;
            return fileCatchData;
        }

        public static FileCatchData WriteAllBytes(string filePath, string data)
        {
            if (string.IsNullOrEmpty(filePath) || string.IsNullOrEmpty(data))
            {
                return null;
            }
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);
            return WriteAllBytes(filePath, dataBytes);
        }

        public static FileCatchData WriteAllBytes(string filePath, byte[] bytes)
        {
            FileCatchData fileCatchData = new FileCatchData();
            if (bytes == null || string.IsNullOrEmpty(filePath))
            {
                fileCatchData.state = false;
            }

            FileStream file = null;
            try
            {
                file = File.Create(filePath);
                file.Write(bytes, 0, bytes.Length);
                file.Close();
            }
            catch (System.Exception e)
            {
                fileCatchData.state = false;
                fileCatchData.message = e.Message;
            }
            return fileCatchData;
        }

        public static FileCatchData WriteAllBytes(string filePath, byte[] bytes, int from, int length)
        {
            FileCatchData fileCatchData = new FileCatchData();

            if (bytes == null || string.IsNullOrEmpty(filePath))
            {
                fileCatchData.state = false;
            }

            FileStream file = null;
            try
            {
                file = File.Create(filePath);
                file.Write(bytes, from, length);
                file.Close();
            }
            catch (System.Exception e)
            {
                fileCatchData.state = false;
                fileCatchData.message = e.Message;
            }
            return fileCatchData;
        }
        /// <summary>
        /// 保存文件
        /// </summary>
        public static FileCatchData WriteAllText(string path, string info, Encoding encoding)
        {
            if (string.IsNullOrEmpty(path) || string.IsNullOrEmpty(info))
            {
                return null;
            }
            FileCatchData fileCatchData = new FileCatchData();
            try
            {
                File.WriteAllText(path, info, encoding);
                fileCatchData.state = true;
            }
            catch (System.Exception e)
            {
                fileCatchData.state = false;
                fileCatchData.message = e.Message;
            }
            return fileCatchData;
        }

        public static FileCatchData WriteAllText(string path, string info)
        {
            if (string.IsNullOrEmpty(path) || string.IsNullOrEmpty(info))
            {
                return null;
            }
            string fonder = PathUtil.GetFilesParentFolder(path);
            CreateDirectory(fonder);
            FileCatchData fileCatchData = new FileCatchData();
            try
            {
                File.WriteAllText(path, info);
                fileCatchData.state = true;
            }
            catch (System.Exception e)
            {
                fileCatchData.state = false;
                fileCatchData.message = e.Message;
            }
            return fileCatchData;
        }
        /// <summary>
        /// 创建文件夹根据有后缀的目录
        /// </summary>
        /// <param name="path">有后缀的目录</param>
        public static FileCatchData CreateDirectory(string path)
        {
            FileCatchData fileCatchData = new FileCatchData();
            if (string.IsNullOrEmpty(path))
            {
                fileCatchData.state = false;
                return fileCatchData;
            }
            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                fileCatchData.state = true;
            }
            catch (System.Exception e)
            {
                fileCatchData.state = false;
                fileCatchData.message = e.Message;
            }
            return fileCatchData;
        }

        //Copy文件目录到指定目录
        public static FileCatchData CopyDirectory(string from, string to)
        {
            if (string.IsNullOrEmpty(from) || string.IsNullOrEmpty(to))
            {
                return null;
            }
            FileCatchData fileCatchData = new FileCatchData();
            try
            {
                //检查是否存在目的目录  
                if (!Directory.Exists(to))
                {
                    Directory.CreateDirectory(to);
                }
                if (Directory.Exists(from) == true)
                {
                    //先来复制文件  
                    DirectoryInfo directoryInfo = new DirectoryInfo(from);
                    FileInfo[] files = directoryInfo.GetFiles();
                    //复制所有文件  
                    foreach (FileInfo file in files)
                    {
                        string _toPath = Path.Combine(to, file.Name);
                        //U2.Debugger.Log("拷贝文件 --->" + file.DirectoryName + "-->" + _toPath);
                        DeleteFile(_toPath);
                        file.CopyTo(_toPath);
                    }
                    //最后复制目录  
                    DirectoryInfo[] directoryInfoArray = directoryInfo.GetDirectories();
                    foreach (DirectoryInfo dir in directoryInfoArray)
                    {
                        CopyDirectory(Path.Combine(from, dir.Name), Path.Combine(to, dir.Name));
                    }
                }
                fileCatchData.state = true;
            }
            catch (System.Exception e)
            {
                fileCatchData.state = false;
                fileCatchData.message = e.Message;
            }
            return fileCatchData;
        }
        public static FileCatchData CopyFile(string from, string to)
        {
            if (string.IsNullOrEmpty(from) || string.IsNullOrEmpty(to))
            {
                return null;
            }
            FileCatchData fileCatchData = new FileCatchData();
            try
            {
                if (File.Exists(from))
                {
                    CreateDirectory(Path.GetDirectoryName(to));
                    DeleteFile(to);
                    FileInfo _fileInfo = new FileInfo(from);
                    _fileInfo.CopyTo(to, true);
                    fileCatchData.state = true;
                }
                else
                {
                    fileCatchData.state = false;
                }

            }
            catch (System.Exception e)
            {
                fileCatchData.state = false;
                fileCatchData.message = e.Message;
            }
            return fileCatchData;
        }

        public static void AppendAllText(string path, string contents)
        {
            if(FileExist(path) == false)
            {
                File.WriteAllText(path, "");
            }
            File.AppendAllText(path, contents);
        }
        /// <summary>
        /// 删除文件夹以及文件夹内的文件
        /// </summary>
        /// <param name="path">路径</param>
        public static FileCatchData DeleteDirectory(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }
            FileCatchData fileCatchData = new FileCatchData();
            try
            {
                //如果存在目录文件，就将其目录文件删除 
                if (Directory.Exists(path))
                {
                    Directory.Delete(path, true);
                    //foreach (string filenamestr in Directory.GetFileSystemEntries(path))
                    //{
                    //    if (File.Exists(filenamestr))
                    //    {
                    //        FileInfo file = new FileInfo(filenamestr);
                    //        if (file.Attributes.ToString().IndexOf("ReadOnly") != -1)
                    //        {
                    //            file.Attributes = FileAttributes.Normal;//去掉文件属性 
                    //        }
                    //        file.Delete();//直接删除其中的文件 
                    //    }
                    //    else
                    //    {
                    //        DeleteDirectory(filenamestr);//递归删除 
                    //    }

                    //}
                    ////删除顶级文件夹
                    //DirectoryInfo DirInfo = new DirectoryInfo(path);

                    //if (DirInfo.Exists)
                    //{
                    //    //Debuger.Log(path);
                    //    DirInfo.Attributes = FileAttributes.Normal & FileAttributes.Directory;    //去掉文件夹属性  
                    //    DirInfo.Delete(true);
                    //}
                }
                fileCatchData.state = true;
            }
            catch (System.Exception e)
            {
                fileCatchData.state = false;
                fileCatchData.message = e.Message;
                // 异常信息 
                // throw new System.Exception("FileManager.DelectDirectory(string path) ------>\n" + e.Message);
            }
            return fileCatchData;
        }
        public static void FileRename(string filePath, string newName)
        {
            string newPath = Path.Combine(Path.GetFullPath(filePath), newName);
            CopyFile(filePath, newName);
            DeleteFile(filePath);
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        public static FileCatchData DeleteFile(string path)
        {
            FileCatchData fileCatchData = new FileCatchData();
            if (string.IsNullOrEmpty(path))
            {
                fileCatchData.state = false;
                return fileCatchData;
            }
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                fileCatchData.state = true;
            }
            catch (System.Exception e)
            {
                fileCatchData.state = false;
                fileCatchData.message = e.Message;
                // throw new System.Exception("FileManager.DeleteFile(string path) ------>\n" + e.Message);
            }
            return fileCatchData;
        }

        /// 文件大小
        public static long GetLength(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return 0;
            }
            System.IO.FileInfo _fileInfo = new System.IO.FileInfo(path);
            return _fileInfo.Length;
        }

        /// string转byte
        public static byte[] GetBytesByString(string info)
        {
            if (string.IsNullOrEmpty(info))
            {
                return null;
            }
            return System.Text.Encoding.UTF8.GetBytes(info);
        }

        /// byte转string
        public static string GetStringByBytes(byte[] bytes)
        {
            if (bytes == null)
            {
                return null;
            }
            return System.Text.Encoding.UTF8.GetString(bytes);
        }

        public static string[] GetSpecifyFilesInFolder(string path, string pattern)
        {
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }

            return Directory.GetFiles(path, pattern, SearchOption.AllDirectories);
        }


        public static string[] GetAllDirsInFolder(string path)
        {
            if (path == null)
            {
                return null;
            }
            return Directory.GetDirectories(path, "*", SearchOption.AllDirectories);
        }

        public static string[] GetDirsInFolder(string path)
        {
            if (path == null)
            {
                return null;
            }
            return Directory.GetDirectories(path, "*", SearchOption.TopDirectoryOnly);
        }
        public static string[] GetAllFilesInFolder(string path)
        {
            if (path == null)
            {
                return null;
            }
            return Directory.GetFiles(path, "*", SearchOption.AllDirectories);
        }
        public static string[] GetFilesInFolder(string path)
        {
            if (path == null)
            {
                return null;
            }
            return Directory.GetFiles(path, "*", SearchOption.TopDirectoryOnly);
        }
        public static void CheckFileAndCreateDirWhenNeeded(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return;
            }

            FileInfo file_info = new FileInfo(filePath);
            DirectoryInfo dir_info = file_info.Directory;
            if (!dir_info.Exists)
            {
                Directory.CreateDirectory(dir_info.FullName);
            }
        }

        public static bool ClearDirectory(string folderPath)
        {
            try
            {
                if (string.IsNullOrEmpty(folderPath))
                {
                    return true;
                }

                if (Directory.Exists(folderPath))
                {
                    DeleteDirectory(folderPath);
                }
                Directory.CreateDirectory(folderPath);
                return true;
            }
            catch (System.Exception ex)
            {
                UnityEngine.Debug.LogError(string.Format("SafeClearDir failed! path = {0} with err = {1}", folderPath, ex.Message));
                return false;
            }
        }
        public static string[] GetDirectoryChildren(string path)
        {
            return Directory.GetFileSystemEntries(path);
        }

        public static string GetFileLastWriteTime(string filePath)
        {
            FileInfo file_info = new FileInfo(filePath);
            return file_info.LastWriteTime.ToLongDateString() + "  " + file_info.LastWriteTime.ToLongTimeString();
        }
        public static string GetFolderLastWriteTime(string filePath)
        {
            DirectoryInfo file_info = new DirectoryInfo(filePath);
            return file_info.LastWriteTime.ToLongDateString() + "  " + file_info.LastWriteTime.ToLongTimeString();
        }
    }

}
