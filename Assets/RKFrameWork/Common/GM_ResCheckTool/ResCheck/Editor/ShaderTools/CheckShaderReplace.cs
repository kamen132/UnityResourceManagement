using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
namespace GMResChecker
{
    public class FindData
    {
        public UnityEngine.Object baseObj;
        public UnityEngine.Object subObj;
        public Material mat;
    }
    public class CheckShaderReplace : ResCheckBaseSubWindowEditor
    {
        private class CacheData
        {
            public string shaderPath;
            public Dictionary<string, Material> mats = new Dictionary<string, Material>();
        }
        private static Dictionary<string, Material> findResult = new Dictionary<string, Material>();
        private static Dictionary<string, CacheData> caches = new Dictionary<string, CacheData>();
        private static string fromShaderName;
        private static Shader replaceShader;
        private static Shader beReplaceShader;
        public override void OnGUIDraw()
        {
            EditorGUILayout.LabelField("--------------------------------------------------------------------------------");
            EditorGUILayout.LabelField("将一个shader所有被引用的mat的shader都替换为另外一个shader");
            GUILayout.Space(15);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(25);

            EditorGUILayout.BeginVertical(GUILayout.Width(200));
            if (GUILayout.Button("SetSelect", GUILayout.Width(150), GUILayout.Height(20)))
            {
                string selectPath = GetSelectPath();
                replaceShader = ResCheckJenkinsEntrance.LoadAssetAtPath(selectPath) as Shader;
            }
            GUILayout.Space(5);
            replaceShader = (Shader)EditorGUILayout.ObjectField(replaceShader, typeof(Shader), true, GUILayout.Width(200));
            GUILayout.Space(5);
            EditorGUILayout.EndVertical();
            GUILayout.Space(5);
            GUILayout.Button("替换", GUILayout.Width(100), GUILayout.Height(60));
            GUILayout.Space(10);

            EditorGUILayout.BeginVertical(GUILayout.Width(150));
            if (GUILayout.Button("SetSelect", GUILayout.Width(150), GUILayout.Height(20)))
            {
                string selectPath = GetSelectPath();
                beReplaceShader = ResCheckJenkinsEntrance.LoadAssetAtPath(selectPath) as Shader;
            }
            GUILayout.Space(5);
            beReplaceShader = (Shader)EditorGUILayout.ObjectField(beReplaceShader, typeof(Shader), true, GUILayout.Width(200));
            GUILayout.Space(5);
            if (GUILayout.Button("查找用到此shader的mat", GUILayout.Width(200), GUILayout.Height(20)))
            {
                FindWhoUseTheShader(beReplaceShader);
                EditorUtility.ClearProgressBar();
            }
            EditorGUILayout.EndVertical();

            if (findResult.Count > 0)
            {
                GUILayout.Space(25);
                if (GUILayout.Button("开始替换", GUILayout.Width(200), GUILayout.Height(60)))
                {
                    BeginReplace();
                }
            }
            EditorGUILayout.EndHorizontal();
            ShowFindResult();
        }

        static void BeginReplace()
        {
            if (replaceShader == null || beReplaceShader == null)
            {
                return;
            }
            foreach (var item in findResult)
            {
                item.Value.shader = replaceShader;
            }
            caches[replaceShader.name].mats.AddRange(caches[beReplaceShader.name].mats);
            caches[beReplaceShader.name].mats.Clear();
            AssetDatabase.SaveAssets();
        }

        static void CheckCache()
        {
            if (caches.Count > 0)
            {
                return;
            }
            string endSuffix = "*.prefab";
            List<string> directoryList = ResCheckJenkinsEntrance.NeedSinglePackDirectorys[endSuffix];
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
                    Renderer[] renders = go.GetComponentsInChildren<Renderer>(true);
                    foreach (var render in renders)
                    {
                        foreach (var mat in render.sharedMaterials)
                        {
                            if (mat == null || mat.shader == null)
                            {
                                continue;
                            }
                            if (caches.ContainsKey(mat.shader.name) == false)
                            {
                                CacheData data = new CacheData();
                                caches[mat.shader.name] = data;
                            }
                            if (caches[mat.shader.name].mats.ContainsKey(mat.name) == false)
                            {
                                caches[mat.shader.name].mats[mat.name] = mat;
                            }
                        }
                    }
                }
            }
        }
        static void FindWhoUseTheShader(Shader shader)
        {
            if (shader == null)
            {
                return;
            }

            CheckCache();

            if (caches.ContainsKey(shader.name))
            {
                findResult = caches[shader.name].mats;
            }
            else
            {
                findResult.Clear();
            }

        }
        static string GetSelectPath()
        {
            var ids = Selection.assetGUIDs;
            if (ids.Length > 0)
            {
                return AssetDatabase.GUIDToAssetPath(ids[0]);
            }
            return null;
        }

        private static Vector3 scrollPos = Vector3.zero;
        public static void ShowFindResult()
        {
            if (findResult.Count == 0)
            {
                return;
            }
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(55);
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, true, GUILayout.Width(500), GUILayout.Height(600));
            GUILayout.Space(5);
            EditorGUILayout.LabelField("--------------------------------------------------------------------------------");
            EditorGUILayout.BeginVertical();
            foreach (var item in findResult)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.ObjectField(item.Value, item.Value.GetType(), true, GUILayout.Width(300));

                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
            GUILayout.Space(5);
            EditorGUILayout.LabelField("--------------------------------------------------------------------------------");
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndHorizontal();
        }
    }
}
