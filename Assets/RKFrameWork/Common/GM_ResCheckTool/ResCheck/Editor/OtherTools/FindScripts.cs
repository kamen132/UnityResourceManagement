using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace GMResChecker
{
    public class FindScripts : ResCheckBaseSubWindowEditor
    {
        private static Dictionary<GameObject, UnityEngine.Object[]> meshColliders = new Dictionary<GameObject, UnityEngine.Object[]>();
        public override void OnGUIDraw()
        {
            EditorGUILayout.LabelField("--------------------------------------------------------------------------------");
            //EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("查找 挂有 MeshCollider 的Prefab", GUILayout.Width(400), GUILayout.Height(20)))
            {
                OnFindScripts(typeof(MeshCollider));
                EditorUtility.ClearProgressBar();
            }
            //EditorGUILayout.EndHorizontal();
            ShowFindResult();
        }
        static void OnFindScripts(Type type)
        {
            string endSuffix = "*.prefab";
            List<string> directoryList = ResCheckJenkinsEntrance.NeedSinglePackDirectorys[endSuffix];
            //List<string> directoryList = new List<string>();
            //directoryList.Add("Assets/WorkAssets/scene_new/");
            //directoryList.Add("Assets/WorkAssets/effect/");
            foreach (var directoryPath in directoryList)
            {
                string[] pathNames = Directory.GetFiles(directoryPath, endSuffix, SearchOption.AllDirectories);
                for (int nameIndex = 0; nameIndex < pathNames.Length; nameIndex++)
                {
                    string assetPath = pathNames[nameIndex];
                    assetPath = ResCheckEditorUtil.FormatPath(assetPath);
                    assetPath = ResCheckEditorUtil.GetAssetPath(assetPath);
                    string title = String.Format("加载prefab {0} 中", Path.GetFileName(directoryPath));
                    EditorUtility.DisplayProgressBar(title, Path.GetFileName(assetPath), (float)nameIndex / pathNames.Length);
                    GameObject go = ResCheckJenkinsEntrance.LoadAssetAtPath(assetPath) as GameObject;
                    if (go)
                    {
                        var data = go.GetComponentsInChildren(type, true);
                        if (data.Length > 0)
                        {
                            meshColliders[go] = data;
                        }
                    }
                }
            }
        }

        private static Vector3 scrollPos = Vector3.zero;
        private static void ShowFindResult()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, true, GUILayout.Width(850), GUILayout.Height(600));
            EditorGUILayout.LabelField("--------------------------------------------------------------------------------");
            GUILayout.Space(5);
            EditorGUILayout.BeginVertical();
            foreach (var item in meshColliders)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.ObjectField(item.Key, item.Key.GetType(), true, GUILayout.Width(400));
                UnityEngine.Object[] value = item.Value;
                EditorGUILayout.BeginVertical();
                foreach (var data in value)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.ObjectField(data, data.GetType(), true, GUILayout.Width(400));
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
