
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
namespace GMResChecker
{
    public class FindShaderAnit : ResCheckBaseSubWindowEditor
    {
        private int num = 1;
        private Dictionary<int, Object> values = new Dictionary<int, Object>();
        public override void OnGUIDraw()
        {
            EditorGUILayout.LabelField("请选择shader");
            for (int i = 0; i < num; i++)
            {
                Object obj = null;
                if (values.ContainsKey(i))
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
        private static Dictionary<Object, Dictionary<Object, List<Object>>> resultObject = new Dictionary<Object, Dictionary<Object, List<Object>>>();
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
            Dictionary<string, bool> depsFiles = ResCheckJenkinsEntrance.GetHaveDepsFile();
            foreach (var item in depsFiles)
            {
                string path = PathUtil.GetAssetPath(item.Key);
                string[] deps = ResCheckJenkinsEntrance.GetDependencies(path, true);
                foreach (string dep in deps)
                {
                    if (searchValues.ContainsKey(dep))
                    {
                        Add(dep, path);
                    }
                }
            }
           
        }
        void OnSearch()
        {
            resultObject.Clear();
            foreach (var item in result)
            {
                string dep = item.Key;
                List<string> values = new List<string>();
                Object obj = ResCheckJenkinsEntrance.LoadAssetAtPath(dep);
                Shader shader = obj as Shader;
                resultObject[obj] = new Dictionary<Object, List<Object>>();
                foreach (var subItem in item.Value)
                {
                    GameObject subObj = ResCheckJenkinsEntrance.LoadAssetAtPath(subItem) as GameObject;
                    Renderer[] renders = subObj.GetComponentsInChildren<Renderer>();
                    List<Object> subObjList = new List<Object>();
                    foreach (Renderer renderer in renders)
                    {
                        if(renderer.sharedMaterial == null)
                        {
                            Debug.Log("null renderer==" + subObj + "===" +renderer.gameObject.name);
                        }
                        else
                        {
                            if (renderer.sharedMaterial.shader == shader)
                            {
                                subObjList.Add(renderer.gameObject);
                               
                            }
                        }
                      
                    }
                    if (subObjList.Count > 0)
                    {
                        resultObject[obj][subObj] = subObjList;
                    }
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
        void ShowFindResult()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, true, GUILayout.Width(1000), GUILayout.Height(600));
            EditorGUILayout.LabelField("--------------------------------------------------------------------------------");

            if (GUILayout.Button("导出excle", GUILayout.Width(200), GUILayout.Height(20)))
            {
                OnSearch();
                string __selectedPath = EditorUtility.OpenFolderPanel("请选择要保存的文件夹", Application.dataPath + "/../", "");
                List<string> title = new List<string>();
                title.Add("ObjectName");
                title.Add("PrefabName");
                title.Add("AnitDepObjectName");
                List<List<string>> data = new List<List<string>>();
                foreach (var item in resultObject)
                {
                    foreach (var subItem in item.Value)
                    {
                        foreach (var subItem2 in subItem.Value)
                        {
                            List<string> itemData = new List<string>();
                            itemData.Add(item.Key.name);
                            itemData.Add(subItem.Key.name);
                            itemData.Add(subItem2.name);
                            data.Add(itemData);
                        }
                    }
                }
                ResCheckJenkinsEntrance.ExportExcle(__selectedPath, "ShaderAnitDependenciesData", "sheet", title, data);

            }

            GUILayout.Space(5);
            EditorGUILayout.BeginVertical();

            foreach (var item in resultObject)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.ObjectField(item.Key, item.Key.GetType(), true, GUILayout.Width(250));
                EditorGUILayout.BeginVertical();
                foreach (var subItem in item.Value)
                {
                    EditorGUILayout.ObjectField(subItem.Key, subItem.GetType(), true, GUILayout.Width(250));
                    EditorGUILayout.BeginHorizontal();
                    foreach (var subItem2 in subItem.Value)
                    {
                        EditorGUILayout.ObjectField(subItem2, subItem.GetType(), true, GUILayout.Width(250));
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }
    }
}

