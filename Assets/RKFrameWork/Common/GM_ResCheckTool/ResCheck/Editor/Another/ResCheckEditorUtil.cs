using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
namespace GMResChecker
{
    public class ResCheckEditorUtil
    {
        public static bool IsPrefab(string path)
        {
            bool isPrefab = path.EndsWith(".prefab") || path.EndsWith(".fbx") || path.EndsWith(".FBX");

            return isPrefab;
        }
        public static bool IsTImeline(string path)
        {
            bool isTImeline = path.EndsWith("timeline.prefab");
            return isTImeline;
        }
        public static bool FileExist(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return false;
            }
            return File.Exists(path);
        }
        public static string GetFileExtension(string path)
        {
            if (path == null)
            {
                return null;
            }
            return Path.GetExtension(path).ToLower();
        }
        public static void MoveFile(string srcPath, string destPath)
        {
            srcPath = FormatPath(srcPath);
            destPath = FormatPath(destPath);
            DirectoryInfo destDir = Directory.GetParent(destPath);
            if (Directory.Exists(destDir.FullName) == false)
            {
                Directory.CreateDirectory(destDir.FullName);
            }
            File.Copy(srcPath, destPath, true);
            File.Delete(srcPath);
        }

      
        public static string GetAssetPath(string fullPath)
        {
            if (string.IsNullOrEmpty(fullPath))
            {
                return null;
            }
            fullPath = FormatPath(fullPath);
            string subName = "Assets";
            int startIndex = fullPath.IndexOf(subName);
            if (startIndex == -1)
            {
                return null;
            }
            string sourceAssetPath = fullPath.Substring(startIndex);
            sourceAssetPath = FormatPath(sourceAssetPath);
            return sourceAssetPath;
        }

        public static string GetWorkAssetsPath(string fullPath)
        {
            if (string.IsNullOrEmpty(fullPath))
            {
                return null;
            }
            string subName = "WorkAssets";
            int startIndex = fullPath.IndexOf(subName);
            if (startIndex == -1)
            {
                return null;
            }
            string sourceAssetPath = fullPath.Substring(startIndex);
            sourceAssetPath = FormatPath(sourceAssetPath);
            return sourceAssetPath;
        }
        public static float CalTextureSize(Texture2D tTexture)
        {
            int tWidth = tTexture.width;
            int tHeight = tTexture.height;

            float bitsPerPixel = GetBitsPerPixel(tTexture.format);
            int mipMapCount = tTexture.mipmapCount;
            int mipLevel = 1;
            float tSize = 0;
            while (mipLevel <= mipMapCount)
            {
                tSize += tWidth * tHeight * bitsPerPixel * 0.125f;
                tWidth = tWidth / 2;
                tHeight = tHeight / 2;
                mipLevel++;
            }
            return tSize;
        }
        public static float GetBitsPerPixel(TextureFormat format)
        {
            switch (format)
            {
                case TextureFormat.Alpha8: //     Alpha-only texture format.
                    return 8;
                case TextureFormat.ARGB4444: //     A 16 bits/pixel texture format. Texture stores color with an alpha channel.
                    return 16;
                case TextureFormat.RGBA4444: //     A 16 bits/pixel texture format.
                    return 16;
                case TextureFormat.RGB24:   // A color texture format.
                    return 24;
                case TextureFormat.RGBA32:  //Color with an alpha channel texture format.
                    return 32;
                case TextureFormat.ARGB32:  //Color with an alpha channel texture format.
                    return 32;
                case TextureFormat.RGB565:  //     A 16 bit color texture format.
                    return 16;
                case TextureFormat.DXT1:    // Compressed color texture format.
                    return 4;
                case TextureFormat.DXT5:    // Compressed color with alpha channel texture format.
                    return 8;
                case TextureFormat.PVRTC_RGB2://     PowerVR (iOS) 2 bits/pixel compressed color texture format.
                    return 2;
                case TextureFormat.PVRTC_RGBA2://     PowerVR (iOS) 2 bits/pixel compressed with alpha channel texture format
                    return 2;
                case TextureFormat.PVRTC_RGB4://     PowerVR (iOS) 4 bits/pixel compressed color texture format.
                    return 4;
                case TextureFormat.PVRTC_RGBA4://     PowerVR (iOS) 4 bits/pixel compressed with alpha channel texture format
                    return 4;
                case TextureFormat.ETC_RGB4://     ETC (GLES2.0) 4 bits/pixel compressed RGB texture format.
                    return 4;
                case TextureFormat.ETC2_RGBA8://     ATC (ATITC) 8 bits/pixel compressed RGB texture format.
                    return 8;
                case TextureFormat.BGRA32://     Format returned by iPhone camera
                    return 32;
                case TextureFormat.ASTC_4x4://      ASTC (4x4 pixel block in 128 bits) compressed RGB texture format.
                    return 8;
                case TextureFormat.ASTC_5x5://     ASTC (5x5 pixel block in 128 bits) compressed RGB texture format.
                    return 5.12f;
                case TextureFormat.ASTC_6x6://      ASTC (6x6 pixel block in 128 bits) compressed RGB texture format.
                    return 3.56f;
                case TextureFormat.ASTC_8x8://    ASTC (8x8 pixel block in 128 bits) compressed RGB texture format.
                    return 2;
                case TextureFormat.ASTC_10x10://    ASTC (10x10 pixel block in 128 bits) compressed RGB texture format.
                    return 1.28f;
                case TextureFormat.ASTC_12x12://     ASTC (12x12 pixel block in 128 bits) compressed RGB texture format.
                    return 0.89f;
            }
            return 0;
        }

        public static List<string> GetDep(string assetPath, string suffix)
        {
            List<string> result = new List<string>();
            string[] data = ResCheckJenkinsEntrance.GetDependencies(assetPath);
            for (int i = 0; i < data.Length; i++)
            {
                string item = data[i];
                if (item.EndsWith(suffix))
                {
                    result.Add(item);
                }
            }
            return result;
        }

        public static bool IsTexture2D(string fileName)
        {
            string endExt = Path.GetExtension(fileName);
            return endExt.Equals(".png")
                    || endExt.Equals(".PNG")
                    || endExt.Equals(".jpg")
                    || endExt.Equals(".psd")
                    || endExt.Equals(".PSD")
                    || endExt.Equals(".tga")
                    || endExt.Equals(".exr");
        }

        public static int GetTransformChildrenNum(Transform trans)
        {
            if (trans == null)
            {
                return 0;
            }
            int num = 0;
            num += trans.childCount;
            for (int i = 0; i < trans.childCount; i++)
            {
                num += GetTransformChildrenNum(trans.GetChild(i));
            }
            return num;
        }
        public static string HumanReadableFilesize(double size)
        {
            double oldSize = size;
            string[] units = new string[] { "B", "KB", "MB", "GB", "TB", "PB" };
            double mod = 1024.0;
            int i = 0;
            double addNum = 0;
            size = getSize(size, out addNum, out i);

            if (addNum == 1)
            {
                return Math.Floor(size) + units[i];
            }
            else
            {
                double otherNum = (oldSize - addNum * Math.Floor(size));
                double addNum2 = 0;
                int i2 = 0;
                double subSize = getSize(otherNum, out addNum2, out i2);
                double data = Math.Floor(size) + subSize / mod;
                return data.ToString("f2") + units[i];
            }
        }

        public static string HumanReadableNum(double size)
        {
            double oldSize = size;
            string[] units = new string[] { "", "k", "M", "G", "T", "P" };
            double mod = 1024.0;
            int i = 0;
            double addNum = 0;
            size = getSize(size, out addNum, out i);

            if (addNum == 1)
            {
                return Math.Floor(size) + units[i];
            }
            else
            {
                double otherNum = (oldSize - addNum * Math.Floor(size));
                double addNum2 = 0;
                int i2 = 0;
                double subSize = getSize(otherNum, out addNum2, out i2);
                double data = Math.Floor(size) + subSize / mod;
                return data.ToString("f2") + units[i];
            }
        }
        static double getSize(double size, out double addNum, out int i)
        {
            addNum = 1;
            i = 0;
            double mod = 1024.0;
            while (size >= mod)
            {
                size /= mod;
                addNum *= mod;
                i++;
            }
            return size;
        }
        public static string FormatPath(string path)
        {
            return path.Replace('\\', '/');
        }
        public class ShaderKeywordsData
        {
            public int[] passtypes = new int[] { };
            public List<List<string>> keywords = new List<List<string>>();
            public Dictionary<string, int> keywordsDic = new Dictionary<string, int>();
            public string[] remainingKeywords = null;
        }
        public static ShaderKeywordsData GetShaderKeywords(Shader shader, MethodInfo GetShaderVariantEntries, ShaderVariantCollection toolSVC)
        {
            //2019.3接口
            //            internal static void GetShaderVariantEntriesFiltered(
            //                Shader                  shader,                     0
            //                int                     maxEntries,                 1
            //                string[]                filterKeywords,             2
            //                ShaderVariantCollection excludeCollection,          3
            //                out int[]               passTypes,                  4
            //                out string[]            keywordLists,               5
            //                out string[]            remainingKeywords)          6

           
            var _filterKeywords = new string[] { };
            var _passtypes = new int[] { };
            var _keywords = new string[] { };
            var _remainingKeywords = new string[] { };
            //toolSVC = new ShaderVariantCollection();
            object[] args = new object[]
            {
                shader, 256, _filterKeywords, toolSVC, _passtypes, _keywords, _remainingKeywords
            };
            GetShaderVariantEntries.Invoke(null, args);

            ShaderKeywordsData shaderKeywordsData = new ShaderKeywordsData();


            //string[] data2 = args[6] as string[];
            //Debug.Log("ccc" + data2.Length);
            //foreach (string data in data2)
            //{
            //    Debug.Log("22222" + data);
            //}
            shaderKeywordsData.passtypes = args[4] as int[];
            var kws = args[5] as string[];
           
            foreach (var kw in kws)
            {
                string[] _kws = kw.Split(' ');
                shaderKeywordsData.keywords.Add(new List<string>(_kws));
                foreach(string keywords in _kws)
                {
                    shaderKeywordsData.keywordsDic[keywords] = 1;
                }
            }
            string[] remainingKeywords = args[6] as string[];
            foreach (string keywords in remainingKeywords)
            {
                shaderKeywordsData.keywordsDic[keywords] = 1;
            }
            shaderKeywordsData.remainingKeywords = remainingKeywords;
            return shaderKeywordsData;
        }
        public static bool IsNODeps(string path)
        {
            return path.EndsWith(".png") 
                || path.EndsWith(".fbx")
                || path.EndsWith(".meta")
                || path.EndsWith(".tga")
                || path.EndsWith(".unity")
                || path.Contains("LightingData")
                || path.EndsWith(".FBX");
        }
        public static bool IsReal(string filePath)
        {
            return filePath.EndsWith(".meta") == false &&
                filePath.EndsWith(".DS_Store") == false &&
                filePath.EndsWith(".git") == false &&
                filePath.EndsWith(".gitignore") == false &&
                filePath.EndsWith(".vscode") == false &&
                filePath.EndsWith(".cs") == false &&
                 filePath.EndsWith(".svn") == false;
        }

        public static void MakeFileWriteable(string path)
        {
            if (string.IsNullOrEmpty(path))
                return;

            if (File.Exists(path))
            {
                File.SetAttributes(path, File.GetAttributes(path) & ~FileAttributes.Hidden);
                File.SetAttributes(path, File.GetAttributes(path) & ~(FileAttributes.Archive | FileAttributes.ReadOnly));
            }
        }

        public static string TimeStr()
        {
            return DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        }

        public static string DateStr()
        {
            return DateTime.Now.ToString("yyyy-MM-dd");
        }
    }
}