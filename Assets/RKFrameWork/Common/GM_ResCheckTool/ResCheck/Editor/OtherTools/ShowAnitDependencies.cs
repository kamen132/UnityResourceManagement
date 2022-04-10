
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace GMResChecker
{
    public class ShowAnitDependencies : ResCheckBaseSubWindowEditor
    {
        private int num = 1;
        private Dictionary<int, Object> values = new Dictionary<int, Object>();
        public override void OnGUIDraw()
        {
            EditorGUILayout.LabelField("请选择或者拖拽您想要查找的反依赖");
            for(int i = 0; i < num;i++)
            {
                Object obj = null;
                if(values.ContainsKey(i))
                {
                    obj = values[i];
                }
                values[i] = EditorGUILayout.ObjectField(obj, typeof(Object), true, GUILayout.Width(200));
            }
            if (GUILayout.Button("Add", GUILayout.Width(150), GUILayout.Height(20)))
            {
                num = num + 1;
            }
            GUILayout.Space(10);
            if (GUILayout.Button("Search", GUILayout.Width(150), GUILayout.Height(20)))
            {
                GetResult();
            }
            ShowFindResult();
        }
        private static Dictionary<string, List<string>> result = new Dictionary<string, List<string>>();
        private static Dictionary<Object, List<Object>> resultObject = new Dictionary<Object, List<Object>>();
        void GetResult()
        {
            result.Clear();
            Dictionary<string, int> searchValues = new Dictionary<string, int>();

            foreach (var item in values)
            {
                if(item.Value != null)
                {
                    Debug.Log("1111==" + ResCheckJenkinsEntrance.GetAssetPath(item.Value));
                    searchValues[ResCheckJenkinsEntrance.GetAssetPath(item.Value)] = 1;
                }
            }
            Dictionary<string, bool> depsFiles = ResCheckJenkinsEntrance.GetHaveDepsFile();
            int index = 0;
            foreach (var item in depsFiles)
            {
                string path = PathUtil.GetAssetPath(item.Key);
                Debug.Log("path==" + path);
                string[] deps = ResCheckJenkinsEntrance.GetDependencies(path, true);
                foreach(string dep in deps)
                {
                    Debug.Log("deps==" + dep);
                    if(searchValues.ContainsKey(dep))
                    {
                        Add(dep, path);
                    }
                }
                index++;
                if(index >10)
                {
                    break;
                }
                
            }
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
        void Add(string dep, string prefabName)
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

            if (GUILayout.Button("导出excle", GUILayout.Width(200), GUILayout.Height(20)))
            {
                string __selectedPath = EditorUtility.OpenFolderPanel("请选择要保存的文件夹", Application.dataPath + "/../", "");
                List<string> title = new List<string>();
                title.Add("ObjectName");
                title.Add("AnitDepObjectName");
                List<List<string>> data = new List<List<string>>();
                foreach (var item in resultObject)
                {
                    List<string> itemData = new List<string>();
                    itemData.Add(item.Key.name);
                    foreach(var subItem in item.Value)
                    {
                        itemData.Add(subItem.name);
                    }
                    data.Add(itemData);
                }
                ResCheckJenkinsEntrance.ExportExcle(__selectedPath, "ShaderAnitDependenciesData", "sheet", title, data);

            }

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

