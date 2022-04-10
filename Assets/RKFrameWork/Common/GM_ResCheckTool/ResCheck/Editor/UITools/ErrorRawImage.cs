using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace GMResChecker
{
    public class ErrorRawImage : ResCheckBaseSubWindowEditor
    {
        private static Dictionary<GameObject, List<RawImage>> findResult = new Dictionary<GameObject, List<RawImage>>();

        public override void OnGUIDraw()
        {
            EditorGUILayout.LabelField("--------------------------------------------------------------------------------");

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("查找RawImage中引用Texture的prefab", GUILayout.Width(400), GUILayout.Height(20)))
            {

                findResult.Clear();
                FindPrefab();
                EditorUtility.ClearProgressBar();
            }
            EditorGUILayout.EndHorizontal();
            ShowFindResult();
        }
        //[@MenuItem("Tools/GetErrorRawImageResources")]
        public static string GetErrorRawImageResources()
        {
            findResult.Clear();
            FindPrefab();
            StringBuilder builder = new StringBuilder();
            builder.Append("=========GetErrorRawImageResources Result:" + (findResult.Count) + "=========\n");
            foreach (var item in findResult)
            {
                builder.Append("obj:");
                builder.Append(item.Key.name);
                builder.Append("   subObj:");
                foreach(var subItem in item.Value)
                {
                    builder.Append(subItem.name);
                    builder.Append("   image:");
                    builder.Append(subItem.texture.name);
                    builder.Append("  :   ");
                }
                builder.Append("\n");
            }
            
            builder.Append("\n\n");
            EditorUtility.ClearProgressBar();
            return builder.ToString();
        }
        static void FindPrefab()
        {
            Dictionary<string, bool> directoryList = ResCheckJenkinsEntrance.GetPrefabDepsFile();
            int nameIndex = 0;
            foreach (var item in directoryList)
            {
                string assetPath = ResCheckEditorUtil.GetAssetPath(item.Key);
                string title = string.Format("加载prefab {0} 中", assetPath);
                EditorUtility.DisplayProgressBar(title, Path.GetFileName(assetPath), nameIndex / directoryList.Count);
                GameObject go = ResCheckJenkinsEntrance.LoadAssetAtPath(assetPath) as GameObject;
                OnDealObjs(go);
                nameIndex++;
            }
        }
        static void OnDealObjs(GameObject go)
        {
            RawImage[] datas = go.GetComponentsInChildren<RawImage>(true);
            foreach (var item in datas)
            {
                
                if (item.texture != null && item.texture.dimension == TextureDimension.Tex2D)
                {
                    if (findResult.ContainsKey(go) == false)
                    {
                        findResult[go] = new List<RawImage>();
                    }
                    findResult[go].Add(item);
                }
            }
            //datas = go.GetComponentsInChildren<UIDynamicRawImage>();
            //foreach (var item in datas)
            //{
            //    if (item.texture != null)
            //    {
            //        if (findResult.ContainsKey(go) == false)
            //        {
            //            findResult[go] = new List<RawImage>();
            //        }
            //        findResult[go].Add(item);
            //    }
            //}
        }
        private static Vector3 scrollPos = Vector3.zero;
        public void ShowFindResult()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, true, GUILayout.Width(1000), GUILayout.Height(600));
            GUILayout.Space(5);
            EditorGUILayout.LabelField("--------------------------------------------------------------------------------");
            EditorGUILayout.BeginVertical();
            foreach (var item in findResult)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.ObjectField(item.Key, item.Key.GetType(), true, GUILayout.Width(300));
                List<RawImage> meshsData = item.Value;
                EditorGUILayout.BeginVertical();
                foreach (var meshData in meshsData)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.ObjectField(meshData, meshData.GetType(), true, GUILayout.Width(300));
                    EditorGUILayout.ObjectField(meshData.texture, meshData.texture.GetType(), true, GUILayout.Width(300));
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
