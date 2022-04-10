using System.IO;
namespace GMResChecker
{
    public static class PathUtil
    {
        public static string Replace(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return null;
            }
            return s.Replace("\\", "/");
        }
        public static string RemoveResourcePath(string fullPath)
        {
            if (string.IsNullOrEmpty(fullPath))
            {
                return null;
            }
            string subName = "Resources";
            int startIndex = fullPath.LastIndexOf(subName);
            if (startIndex == -1)
            {
                return subName;
            }
            string sourceAssetPath = fullPath.Substring(startIndex + subName.Length + 1);
            sourceAssetPath = Replace(sourceAssetPath);
            return sourceAssetPath;
        }

        public static string GetPathBySubstring(string fullPath, string subName)
        {
            if (string.IsNullOrEmpty(fullPath) || string.IsNullOrEmpty(subName))
            {
                return null;
            }
            int startIndex = fullPath.LastIndexOf(subName);
            if (startIndex == -1)
            {
                return subName;
            }
            string sourceAssetPath = fullPath.Substring(startIndex + subName.Length + 1);
            sourceAssetPath = Replace(sourceAssetPath);
            return sourceAssetPath;
        }
        public static string RemoveresourcePath(string fullPath)
        {
            if (string.IsNullOrEmpty(fullPath))
            {
                return null;
            }
            string subName = "resources";
            int startIndex = fullPath.LastIndexOf(subName);
            if (startIndex == -1)
            {
                return subName;
            }
            string sourceAssetPath = fullPath.Substring(startIndex + subName.Length + 1);
            sourceAssetPath = Replace(sourceAssetPath);
            return sourceAssetPath;
        }
        public static string RemoveWorkAssetsPath(string fullPath)
        {
            if (string.IsNullOrEmpty(fullPath))
            {
                return null;
            }
            string subName = "workassets";
            int startIndex = fullPath.LastIndexOf(subName);
            if (startIndex == -1)
            {
                return fullPath;
            }
            string sourceAssetPath = fullPath.Substring(startIndex + subName.Length + 1);
            sourceAssetPath = Replace(sourceAssetPath);
            return sourceAssetPath;
        }
        public static string RemovePath(string fullPath, string subName)
        {
            if (string.IsNullOrEmpty(fullPath))
            {
                return null;
            }
            int startIndex = fullPath.LastIndexOf(subName);
            if (startIndex == -1)
            {
                return fullPath;
            }
            string sourceAssetPath = fullPath.Substring(startIndex + subName.Length + 1);
            sourceAssetPath = Replace(sourceAssetPath);
            return sourceAssetPath;
        }
        public static string GetAppdataPath(string fullPath)
        {
            if (string.IsNullOrEmpty(fullPath))
            {
                return null;
            }
            string subName = "appdata";
            int startIndex = fullPath.LastIndexOf(subName);
            if (startIndex == -1)
            {
                return fullPath;
            }
            string sourceAssetPath = fullPath.Substring(startIndex);
            sourceAssetPath = Replace(sourceAssetPath);
            return sourceAssetPath;
        }

        public static string GetAssetPath(string fullPath)
        {
            if (string.IsNullOrEmpty(fullPath))
            {
                return null;
            }
            string subName = "Assets";
            int startIndex = fullPath.IndexOf(subName);
            if (startIndex == -1)
            {
                return fullPath;
            }
            string sourceAssetPath = fullPath.Substring(startIndex);
            sourceAssetPath = Replace(sourceAssetPath);
            return sourceAssetPath;
        }
        public static string GetFolderName(string folderPath)
        {
            folderPath = Replace(folderPath);
            int __index_ = folderPath.LastIndexOf('/') + 1;
            string __folderName = folderPath.Substring(__index_, folderPath.Length - __index_);
            return __folderName;
        }
        public static string GetFilesParentFolder(string filePath)
        {
            return Path.GetDirectoryName(filePath);
        }
        public static string GetFolderParentFolder(string filePath)
        {
            return Replace(Directory.GetParent(filePath).FullName);
        }
        public static string GetFileName(string filePath)
        {
            return Path.GetFileName(filePath);
        }
        public static string GetFileNameWithoutExtension(string filePath)
        {
            return Path.GetFileNameWithoutExtension(filePath);
        }

        public static string GetFileExtension(string path)
        {
            if (path == null)
            {
                return null;
            }
            return Path.GetExtension(path).ToLower();
        }

        public static string RemoveFileExtension(string path)
        {
            if (path == null)
            {
                return null;
            }
            int __indexDot = path.LastIndexOf('.');
            if (__indexDot != -1)
            {
                return path.Substring(0, __indexDot);
            }
            return path;
        }
    }
}


