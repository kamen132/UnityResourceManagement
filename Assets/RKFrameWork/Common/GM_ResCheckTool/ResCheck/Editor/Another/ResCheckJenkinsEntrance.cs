using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
namespace GMResChecker
{
    public class ResCheckJenkinsEntrance
    {
        public static string BaseWorkPath = Application.dataPath + "/WorkAssets/";

       public static Dictionary<string, List<string>> s_ResDirs = new Dictionary<string, List<string>>()
        {
            {
                "*.prefab",new List<string>()
                {
                    BaseWorkPath + "Prefabs",
                }
            },
            {
                "*.png",new List<string>()
                {
                    //BaseWorkPath + "textures_dynamic",//动态图集，每个图片会单独打成一个ab，rgba32压缩格式
                    //BaseWorkPath + "textures_rgba32",//不想被压缩（图集以及textures_separate就不支持了,为什么不能放到动态图集：因为动态图集单个图为一个ab，这个文件夹的png，可以随着prefab一起打到一个ab。所以，大图小图都可能在这个文件夹下

                        //BaseWorkPath + "textures_separate",//大图，不方便放到图集中，比如bg，panelbg。但是他比图集压缩损失更小
                        //BaseWorkPath + "textures_rgba32_dazzlingPin",//暂时先使用，当改变下载方式的时候，要去掉
                        //BaseWorkPath + "textures_download",//暂时先使用，当改变下载方式的时候，要去掉
                }
            },
            {
                "*.mp3",new List<string>()
                {
                    BaseWorkPath + "Audios",
                }
            } ,
            {
                "*.mat",new List<string>()
                {
                    BaseWorkPath + "Materials",
                }
            } ,
        };
        private static Dictionary<string, Object> objs = new Dictionary<string, Object>();
        public static IDictionary<string, List<string>> NeedSinglePackDirectorys
        {
            get
            {
                return s_ResDirs;
            }
        }
        private static Dictionary<string, string[]> deps = new Dictionary<string, string[]>();
        private static Dictionary<string, string[]> depsRecursive = new Dictionary<string, string[]>();

        private static Dictionary<Object, string> objsToPath = new Dictionary<Object, string>();
        public static List<string> GetNeedETCTexturePath()
        {
            List<string> result = new List<string>()
            {
                 BaseWorkPath + "Effect",
                 BaseWorkPath + "UISpine",
                 BaseWorkPath + "Machines",
            };
            return result;
        }

        public static Dictionary<string, bool> GetPrefabDepsFile()
        {
            Dictionary<string, bool> values = new Dictionary<string, bool>();
            string Ext = ".prefab";
            List<string> value = NeedSinglePackDirectorys["*" + Ext];
            foreach (var path in value)
            {
                string[] files = FileManager.GetAllFilesInFolder(path);
                foreach (var file in files)
                {
                    if (file.EndsWith(Ext))
                    {
                        values[file] = true;
                    }
                }
            }
            return values;
        }
        public static Dictionary<string, bool> GetHaveDepsFile()
        {
            Dictionary<string, bool> values = GetPrefabDepsFile();

            string Ext = ".mat";
            List<string> value = s_ResDirs["*" + Ext];
            foreach (var path in value)
            {
                string[] files = FileManager.GetAllFilesInFolder(path);
                foreach (var file in files)
                {
                    if (file.EndsWith(Ext))
                    {
                        values[file] = true;
                    }
                }
            }
            return values;
        }
        public static Dictionary<string, bool> GetHaveShaderPath()
        {
            Dictionary<string, bool> values = new Dictionary<string, bool>();
            string Ext = ".shader";
            List<string> value = s_ResDirs["*" + Ext];
            foreach (var path in value)
            {
                string[] files = FileManager.GetAllFilesInFolder(path);
                foreach (var file in files)
                {
                    if (file.EndsWith(Ext))
                    {
                        values[file] = true;
                    }
                }
            }
            return values;
        }
        //public static void ReadWorkbook(string excelPath, ref IWorkbook workbook)
        //{
        //    using (var fileStream = File.OpenRead(excelPath))
        //    {
        //        if (excelPath.EndsWith(".xlsx"))      // 2007版本
        //            workbook = new XSSFWorkbook(fileStream);
        //        else if (excelPath.EndsWith(".xls"))  // 2003版本
        //            workbook = new HSSFWorkbook(fileStream);
        //    }
        //}
        public static void ExportExcle(string dir, string fileName, string sheetName, List<string> titleList, List<List<string>> data)
        {
            //var workbook = new HSSFWorkbook();
            //var sheet = workbook.CreateSheet(sheetName);

            //var typeRow = sheet.CreateRow(0);
            //for (int i = 0; i < titleList.Count; i++)
            //{
            //    var typeCell = typeRow.CreateCell(i);
            //    typeCell.SetCellValue(titleList[i]);
            //}

            //for (int i = 0; i < data.Count; i++)
            //{
            //    var row = sheet.CreateRow(i + 1);
            //    var itemData = data[i];
            //    for (int j = 0; j < itemData.Count; j++)
            //    {
            //        var cell = row.CreateCell(j);
            //        string itemDataStr = itemData[j];
            //        double doubleData = 0;
            //        bool isSuccess = double.TryParse(itemDataStr, out doubleData);
            //        if (isSuccess)
            //        {
            //            cell.SetCellValue(doubleData);
            //        }
            //        else
            //        {
            //            cell.SetCellValue(itemDataStr);
            //        }
            //    }
            //}

            //string excelOutputPath = dir + "/" + fileName + ".xls";
            //if (File.Exists(excelOutputPath))
            //{
            //    File.Delete(excelOutputPath);
            //}

            //using (var fs = File.Create(excelOutputPath))
            //{
            //    workbook.Write(fs);
            //}
        }

        public static void ExportExcle(string dir, string fileName, List<string> sheetNames, List<List<string>> titleLists, List<List<List<string>>> datas)
        {
            //var workbook = new HSSFWorkbook();

            //for (int m = 0; m < sheetNames.Count; m++)
            //{
            //    string sheetName = sheetNames[m];
            //    List<string> titleList = titleLists[m];
            //    List<List<string>> data = datas[m];
            //    var sheet = workbook.CreateSheet(sheetName);

            //    var typeRow = sheet.CreateRow(0);
            //    for (int i = 0; i < titleList.Count; i++)
            //    {
            //        var typeCell = typeRow.CreateCell(i);
            //        typeCell.SetCellValue(titleList[i]);
            //    }

            //    for (int i = 0; i < data.Count; i++)
            //    {
            //        var row = sheet.CreateRow(i + 1);
            //        var itemData = data[i];
            //        for (int j = 0; j < itemData.Count; j++)
            //        {
            //            var cell = row.CreateCell(j);
            //            string itemDataStr = itemData[j];
            //            double doubleData = 0;
            //            bool isSuccess = double.TryParse(itemDataStr, out doubleData);
            //            if(isSuccess)
            //            {
            //                cell.SetCellValue(doubleData);
            //            }else
            //            {
            //                cell.SetCellValue(itemDataStr);
            //            }
                        
            //        }
            //    }
            //}
            
            //string excelOutputPath = dir + "/" + fileName + ".xls";
            //if (File.Exists(excelOutputPath))
            //{
            //    File.Delete(excelOutputPath);
            //}

            //using (var fs = File.Create(excelOutputPath))
            //{
            //    workbook.Write(fs);
            //}
        }
        public static Object LoadAssetAtPath(string assetPath)
        {
            if (objs.ContainsKey(assetPath))
            {
                return objs[assetPath];
            }

            Object obj = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Object));
            objs[assetPath] = obj;
            return obj;
        }

        public static string GetAssetPath(Object obj)
        {
            if (objsToPath.ContainsKey(obj))
            {
                return objsToPath[obj];
            }
            string path = AssetDatabase.GetAssetPath(obj);
            objsToPath[obj] = path;
            return path;
        }

        public static string[] GetDependencies(string path, bool recursive = true)
        {
            if (recursive)
            {
                return GetDependenciesRecursive(path);
            }
            return GetDependenciesNORecursive(path);
        }
        public static string[] GetDependenciesRecursive(string path)
        {
            if (depsRecursive.ContainsKey(path))
            {
                return depsRecursive[path];
            }
            string[] dependencies = AssetDatabase.GetDependencies(path, true);
            depsRecursive[path] = dependencies;
            return dependencies;
        }
        public static string[] GetDependenciesNORecursive(string path)
        {
            if (deps.ContainsKey(path))
            {
                return deps[path];
            }
            string[] dependencies = AssetDatabase.GetDependencies(path, true);
            deps[path] = dependencies;
            return dependencies;
        }

        public static void Clear()
        {
            objs.Clear();
            deps.Clear();
            depsRecursive.Clear();
            objsToPath.Clear();
        }
    }
}
