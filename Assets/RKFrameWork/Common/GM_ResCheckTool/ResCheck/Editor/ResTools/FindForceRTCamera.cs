using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace GMResChecker
{
    public class FindForceRTCamera : ResCheckBaseSubWindowEditor
    {
        static Dictionary<GameObject, Dictionary<GameObject, int>> findResults = new Dictionary<GameObject, Dictionary<GameObject, int>>();
        public override void OnGUIDraw()
        {
            EditorGUILayout.LabelField("--------------------------------------------------------------------------------");
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();
            if (GUILayout.Button("查找 forceIntoRenderTexture 资源", GUILayout.Width(300), GUILayout.Height(20)))
            {
                findResults.Clear();
                OnFindRes();
                EditorUtility.ClearProgressBar();
            }
            GUILayout.Space(5);
            if (findResults.Count > 0)
            {
                if (GUILayout.Button("清除数据", GUILayout.Width(200), GUILayout.Height(20)))
                {
                    ClearData();
                }
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            ShowFindResult();
        }

        void ClearData()
        {
            foreach (var item in findResults)
            {
                GameObject prefabGameobject = item.Key;
                //GameObject prefabGameobject = PrefabUtility.InstantiatePrefab(go) as GameObject;
                Camera[] cameras = prefabGameobject.GetComponentsInChildren<Camera>(true);
                foreach (Camera camera in cameras)
                {
                    camera.forceIntoRenderTexture = false;
                }
                //PrefabUtility.ReplacePrefab(prefabGameobject, go, ReplacePrefabOptions.Default);
            }

            findResults.Clear();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        void OnFindRes()
        {
            Dictionary<string, bool> result = new Dictionary<string, bool>();
            int index = 0;
            int totleIndex = result.Count;
            foreach (var item in result)
            {
                index++;
                string path = item.Key;
                if (path.EndsWith(".prefab"))
                {
                    path = ResCheckEditorUtil.GetAssetPath(path);

                    GameObject go = ResCheckJenkinsEntrance.LoadAssetAtPath(path) as GameObject;
                    EditorUtility.DisplayProgressBar("加载资源中", Path.GetFileNameWithoutExtension(path), (float)index / totleIndex);
                    if (go)
                    {
                        Camera[] cameras = go.GetComponentsInChildren<Camera>(true);
                        foreach (Camera camera in cameras)
                        {
                            if (camera.forceIntoRenderTexture)
                            {
                                if (findResults.ContainsKey(go) == false)
                                {
                                    findResults[go] = new Dictionary<GameObject, int>();
                                }
                                findResults[go][camera.gameObject] = 1;
                            }
                        }
                    }
                }
            }
        }
        private static Vector3 scrollPos = Vector3.zero;
        private void ShowFindResult()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, true, GUILayout.Width(850), GUILayout.Height(600));
            EditorGUILayout.LabelField("--------------------------------------------------------------------------------");
            GUILayout.Space(5);
            EditorGUILayout.BeginVertical();

            foreach (var item in findResults)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.ObjectField(item.Key, item.Key.GetType(), true, GUILayout.Width(400));
                EditorGUILayout.BeginVertical();
                foreach (var subItem in item.Value)
                {
                    EditorGUILayout.ObjectField(subItem.Key, subItem.Key.GetType(), true, GUILayout.Width(400));
                }
                EditorGUILayout.EndVertical();

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }
    }
}
