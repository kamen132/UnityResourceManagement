using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GMResChecker
{
    public class FIndMissingMesh : ResCheckBaseSubWindowEditor
    {
        private static Dictionary<GameObject, List<GameObject>> missingMeshDic = new Dictionary<GameObject, List<GameObject>>();
        private static bool isInit = false;
        public override void OnGUIDraw()
        {
            EditorGUILayout.LabelField("--------------------------------------------------------------------------------");
            EditorGUILayout.BeginHorizontal();
            float width = 180;
            if (GUILayout.Button("FIndMissingMesh", GUILayout.Width(width), GUILayout.Height(20)))
            {
                FindPrefabData();
            }
            EditorGUILayout.EndHorizontal();
          
            ShowFindResult();
        }

        void FindPrefabData()
        {
            if (isInit)
            {
                return;
            }
            Dictionary<string, bool> allData = ResCheckJenkinsEntrance.GetPrefabDepsFile();
            foreach(var item in allData)
            {
                string assetPath = PathUtil.GetAssetPath(item.Key);
                GameObject go = ResCheckJenkinsEntrance.LoadAssetAtPath(assetPath) as GameObject;
                if (go)
                {

                    MeshFilter[] filters = go.GetComponentsInChildren<MeshFilter>(true);
                    foreach (MeshFilter filter in filters)
                    {
                        if (filter.sharedMesh == null)
                        {
                            //Debug.Log("miss mesh: " + go.name + "   __   " + filter.gameObject.name);
                            if (missingMeshDic.ContainsKey(go) == false)
                            {
                                missingMeshDic[go] = new List<GameObject>();
                            }
                            missingMeshDic[go].Add(filter.gameObject);
                            continue;
                        }
                    }
                }
            }
        }

        private static Vector3 scrollPos = Vector3.zero;
     
        private static void ShowFindResult()
        {
            EditorGUILayout.LabelField("---------------------------检查场景 Missing Mesh 数据-----------------------------------------------------");
            GUILayout.Space(5);
            EditorGUILayout.BeginVertical();
            if(missingMeshDic.Count > 0)
            {
                if (GUILayout.Button("导出excle", GUILayout.Width(200), GUILayout.Height(20)))
                {
                    string __selectedPath = EditorUtility.OpenFolderPanel("请选择要保存的文件夹", Application.dataPath + "/../", "");
                    List<string> title = new List<string>();
                    title.Add("PrefabName");
                    title.Add("SubObjectName");
                    List<List<string>> data = new List<List<string>>();
                    foreach (var item in missingMeshDic)
                    {
                        foreach (GameObject obj in item.Value)
                        {
                            List<string> itemData = new List<string>();
                            itemData.Add(item.Key.name);
                            itemData.Add(obj.name);
                            data.Add(itemData);
                        }
                    }
                    ResCheckJenkinsEntrance.ExportExcle(__selectedPath, "ALLMissingMeshData", "sheet", title, data);
                }
            }
            
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, true, GUILayout.Width(750), GUILayout.Height(600));

            GUILayout.Space(5);

            foreach (var item in missingMeshDic)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.ObjectField(item.Key, item.Key.GetType(), true, GUILayout.Width(400));
                EditorGUILayout.BeginVertical();
                foreach (var item2 in item.Value)
                {
                    EditorGUILayout.ObjectField(item2, item2.GetType(), true, GUILayout.Width(300));
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }
    }
}

