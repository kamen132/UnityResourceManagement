
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
namespace GMResChecker
{
    public class FindShaderFallBack : ResCheckBaseSubWindowEditor
    {
        private int num = 1;
        private Dictionary<int, Object> values = new Dictionary<int, Object>();
        private const string FallBack = "FallBack";
        private const string K1 = "{";
        private const string K2 = "}";

        private const string Notes = "//";
        public override void OnGUIDraw()
        {
            if (GUILayout.Button("Search", GUILayout.Width(150), GUILayout.Height(20)))
            {
                GetResult();
            }
            ShowFindResult();
        }
        private static Dictionary<string, string> result = new Dictionary<string, string>();
        //private static Dictionary<string, Dictionary<string, List<string>>> resultObject = new Dictionary<string, Dictionary<string, List<string>>>();
        void GetResult()
        {
            result.Clear();
            Dictionary<string, int> searchValues = new Dictionary<string, int>();

            foreach (var item in values)
            {
                if (item.Value != null)
                {
                    searchValues[ResCheckJenkinsEntrance.GetAssetPath(item.Value)] = 1;
                }
            }
            Dictionary<string, bool> depsFiles = ResCheckJenkinsEntrance.GetHaveShaderPath();
            foreach (var item in depsFiles)
            {
                string fullPath = item.Key;
                if (fullPath.Contains("UnityBulidIn") || fullPath.Contains("SpineShaders"))
                {
                    continue;
                }

                string[] allLines = FileManager.ReadAllLines(fullPath);
                foreach(string lineData in allLines)
                {
                    //Debug.Log("lineData==" + lineData);
                    if(lineData.Contains(FallBack) && lineData.Contains(Notes) == false)
                    {
                        string fallBackData = lineData.Replace(FallBack, "");
                        fallBackData = fallBackData.Replace(K1, "");
                        fallBackData = fallBackData.Replace(K2, "");
                        fallBackData = fallBackData.Trim();
                        if(result.ContainsKey(fallBackData) == false)
                        {
                            string path = PathUtil.GetPathBySubstring(fullPath, "Product/Editor/Resources/Shader");
                            result[fallBackData] = path;

                            Debug.Log("fallback=" + fallBackData + "==" + path);
                        }
                       
                    }
                }
            }

           
        }
      
        private static Vector3 scrollPos = Vector3.zero;
        void ShowFindResult()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, true, GUILayout.Width(1000), GUILayout.Height(600));
            EditorGUILayout.LabelField("--------------------------------------------------------------------------------");

            if (GUILayout.Button("导出excle", GUILayout.Width(200), GUILayout.Height(20)))
            {
                string __selectedPath = EditorUtility.OpenFolderPanel("请选择要保存的文件夹", Application.dataPath + "/../", "");
                List<string> title = new List<string>();
                title.Add("ObjectName");
                title.Add("PrefabName");
                title.Add("AnitDepObjectName");
                List<List<string>> data = new List<List<string>>();
                //    foreach (var item in resultObject)
                //    {
                //        foreach (var subItem in item.Value)
                //        {
                //            foreach (var subItem2 in subItem.Value)
                //            {
                //                List<string> itemData = new List<string>();
                //                itemData.Add(item.Key);
                //                itemData.Add(subItem.Key);
                //                itemData.Add(subItem2);
                //                data.Add(itemData);
                //            }
                //        }
                //    }
                //    ResCheckJenkinsEntrance.ExportExcle(__selectedPath, "ShaderAnitDependenciesData", "sheet", title, data);
            }

            GUILayout.Space(5);
            EditorGUILayout.BeginVertical();

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }
    }
}

