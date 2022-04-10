
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
namespace GMResChecker
{
    public class FindUseUISpineRes : ResCheckBaseSubWindowEditor
    {
        private int num = 1;
        private Dictionary<int, Object> values = new Dictionary<int, Object>();
        public override void OnGUIDraw()
        {
           
            GUILayout.Space(10);
            if (GUILayout.Button("Search", GUILayout.Width(150), GUILayout.Height(20)))
            {
                GetResult();
                OnFinalGetResult();
            }
            ShowFindResult();
        }

        public static string GetUseUISpineRes()
        {
            GetResult();
            foreach (var item in result)
            {
                string dep = item.Key;
                List<string> values = new List<string>();
                Object obj = ResCheckJenkinsEntrance.LoadAssetAtPath(dep);
                resultObject[obj] = new List<Object>();
                foreach (var subItem in item.Value)
                {
                    Object subObj = ResCheckJenkinsEntrance.LoadAssetAtPath(subItem);
                    resultObject[obj].Add(subObj);
                }
            }
            StringBuilder builder = new StringBuilder();
            builder.Append("=========GetUseUISpineRes:" + result.Count + "=========\n");
            foreach (var item in result)
            {
                string deps = item.Key;
                
                foreach (var subItem in item.Value)
                {
                    builder.Append("deps ");
                    builder.Append(deps);
                    builder.Append("  ");
                    builder.Append(subItem);
                    builder.Append("\n");
                }
            }
            builder.Append("\n\n");
            return builder.ToString();
        }

        private static Dictionary<string, List<string>> result = new Dictionary<string, List<string>>();
        private static Dictionary<Object, List<Object>> resultObject = new Dictionary<Object, List<Object>>();
        static void GetResult()
        {
            result.Clear();
            Dictionary<string, int> searchValues = new Dictionary<string, int>();
            string UISpineDir = Application.dataPath + "/WorkAssets/UISpine";
            string[] allFiles = FileManager.GetAllFilesInFolder(UISpineDir);
            foreach(var item in allFiles)
            {
                if(item.EndsWith(".meta") == false)
                {
                    string path = PathUtil.GetAssetPath(item);
                    searchValues[path] = 1;
                }
            }
            Dictionary<string, bool> depsFiles = ResCheckJenkinsEntrance.GetHaveDepsFile();
            foreach (var item in depsFiles)
            {
                string path = PathUtil.GetAssetPath(item.Key);
                string[] deps = ResCheckJenkinsEntrance.GetDependencies(path, true);
                foreach(string dep in deps)
                {
                    if(searchValues.ContainsKey(dep))
                    {
                        Add(dep, path);
                    }
                }
            }
        }

        void OnFinalGetResult()
        {
            resultObject.Clear();
            foreach (var item in result)
            {
                string dep = item.Key;
                List<string> values = new List<string>();
                Object obj = ResCheckJenkinsEntrance.LoadAssetAtPath(dep);
                resultObject[obj] = new List<Object>();
                foreach (var subItem in item.Value)
                {
                    Object subObj = ResCheckJenkinsEntrance.LoadAssetAtPath(subItem);
                    resultObject[obj].Add(subObj);
                }
            }
        }
        static void Add(string dep, string prefabName)
        {
            if (result.ContainsKey(dep) == false)
            {
                result[dep] = new List<string>();

            }
            result[dep].Add(prefabName);
        }
        private static Vector3 scrollPos = Vector3.zero;
        static void ShowFindResult()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, true, GUILayout.Width(1000), GUILayout.Height(600));
            EditorGUILayout.LabelField("--------------------------------------------------------------------------------");

            GUILayout.Space(5);
            EditorGUILayout.BeginVertical();
            foreach (var item in resultObject)
            {
                List<Object> subItems = item.Value;

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.ObjectField(item.Key, item.Key.GetType(), true, GUILayout.Width(250));
                EditorGUILayout.BeginVertical();
                foreach(var subItem in subItems)
                {
                    EditorGUILayout.ObjectField(subItem, subItem.GetType(), true, GUILayout.Width(250));
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }
    }
}

