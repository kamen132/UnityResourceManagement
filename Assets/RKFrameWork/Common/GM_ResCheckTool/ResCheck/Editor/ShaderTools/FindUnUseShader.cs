using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace GMResChecker
{
    public class FindUnUseShader : ResCheckBaseSubWindowEditor
    {
        private static Dictionary<string, int> useShaderDic = new Dictionary<string, int>();
        private static List<Shader> findResult = new List<Shader>();
        public override void OnGUIDraw()
        {
            EditorGUILayout.LabelField("--------------------------------------------------------------------------------");
            EditorGUILayout.LabelField("shader Path: Assets/WorkAssets/shader");
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("检查无用的 shader", GUILayout.Width(400), GUILayout.Height(20)))
            {
                useShaderDic.Clear();
                findResult.Clear();
                FindUnUseShaderOfPrefab();
                FinUnUseShaderOfMat();
                EditorUtility.ClearProgressBar();
                OnFindUnUseShader();
            }
            if (GUILayout.Button("将无用的shader移到 back_up 目录", GUILayout.Width(400), GUILayout.Height(20)))
            {
                MoveToBackUpFolder();
                //AssetDatabase.Refresh();
            }
            EditorGUILayout.EndHorizontal();
            ShowFindResult();
        }

        static void MoveToBackUpFolder()
        {
            string BackUpPath = Application.dataPath + "/WorkAssets/shaderBackUp/";
            List<string> removeList = new List<string>();
            foreach (var item in findResult)
            {
                string path = ResCheckJenkinsEntrance.GetAssetPath(item);

                string fromPath = Path.GetFullPath(path);
                string toPath = BackUpPath + ResCheckEditorUtil.GetWorkAssetsPath(path);
                ResCheckEditorUtil.MoveFile(fromPath, toPath);

                string fromPathMeta = fromPath + ".meta";
                string toPathMeta = toPath + ".meta";
                ResCheckEditorUtil.MoveFile(fromPathMeta, toPathMeta);

            }
        }

        static void OnFindUnUseShader()
        {
            String BasePath = Application.dataPath + "/WorkAssets/shader";
            string[] shaderNames = Directory.GetFiles(BasePath, "*.shader", SearchOption.AllDirectories);
            for (int i = 0; i < shaderNames.Length; i++)
            {
                string path = shaderNames[i];
                //if(path.Contains("from_built_in"))
                //{
                //    continue;
                //}
                path = ResCheckEditorUtil.GetAssetPath(path);
                Shader shader = ResCheckJenkinsEntrance.LoadAssetAtPath(path) as Shader;
                if (useShaderDic.ContainsKey(shader.name) == false)
                {
                    findResult.Add(shader);
                }
            }
        }

        static void FindUnUseShaderOfPrefab()
        {
            string endSuffix = "*.prefab";
            List<string> directoryList = ResCheckJenkinsEntrance.NeedSinglePackDirectorys[endSuffix];
            foreach (var directoryPath in directoryList)
            {
                Debug.Log("directoryPath===" + Path.GetFileName(directoryPath));
                string[] pathNames = Directory.GetFiles(directoryPath, endSuffix, SearchOption.AllDirectories);
                for (int nameIndex = 0; nameIndex < pathNames.Length; nameIndex++)
                {
                    string assetPath = pathNames[nameIndex];
                    assetPath = ResCheckEditorUtil.GetAssetPath(assetPath);
                    string title = String.Format("加载prefab {0} 中", Path.GetFileName(directoryPath));
                    EditorUtility.DisplayProgressBar(title, Path.GetFileName(assetPath), (float)nameIndex / pathNames.Length);
                    List<String> result = ResCheckEditorUtil.GetDep(assetPath, ".shader");
                    foreach (var item in result)
                    {
                        Shader shader = ResCheckJenkinsEntrance.LoadAssetAtPath(item) as Shader;
                        useShaderDic[shader.name] = 1;
                    }
                    //GameObject go = ResCheckJenkinsEntrance.LoadAssetAtPath<GameObject>(assetPath);
                    //if (go)
                    //{
                    //    Renderer[] renders = go.GetComponentsInChildren<Renderer>(true);
                    //    foreach (var render in renders)
                    //    {
                    //        foreach (var mat in render.sharedMaterials)
                    //        {
                    //            if(mat!= null && mat.shader != null)
                    //            {
                    //                useShaderDic[mat.shader.name] = 1;
                    //            }
                    //        }
                    //    }
                    //}
                }
            }
        }



        static void FinUnUseShaderOfMat()
        {
            string endSuffix = "*.mat";
            List<string> directoryList = ResCheckJenkinsEntrance.NeedSinglePackDirectorys[endSuffix];
            foreach (var directoryPath in directoryList)
            {
                string[] pathNames = Directory.GetFiles(directoryPath, endSuffix, SearchOption.AllDirectories);

                for (int nameIndex = 0; nameIndex < pathNames.Length; nameIndex++)
                {
                    string assetPath = pathNames[nameIndex];
                    assetPath = ResCheckEditorUtil.GetAssetPath(assetPath);
                    EditorUtility.DisplayProgressBar("查找mat中", Path.GetFileName(assetPath), (float)nameIndex / pathNames.Length);
                    Material mat = ResCheckJenkinsEntrance.LoadAssetAtPath(assetPath) as Material;
                    if (!mat) continue;
                    AddToData(mat.shader);
                }
            }
        }
        static void AddToData(Shader shader)
        {
            string path = ResCheckJenkinsEntrance.GetAssetPath(shader);
            if (path.Contains("always_in_shader"))
            {
                return;
            }
            useShaderDic[shader.name] = 1;
        }
        private static Vector3 scrollPos = Vector3.zero;
        public static void ShowFindResult()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, true, GUILayout.Width(1000), GUILayout.Height(600));
            GUILayout.Space(5);
            EditorGUILayout.LabelField("--------------------------------------------------------------------------------");
            EditorGUILayout.BeginVertical();
            for (int i = 0; i < findResult.Count; i++)
            {
                Shader item = findResult[i];
                EditorGUILayout.BeginHorizontal();
                //EditorGUILayout.LabelField(item.name, GUILayout.Width(400));
                EditorGUILayout.ObjectField(item, item.GetType(), true, GUILayout.Width(600));
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
            GUILayout.Space(5);
            EditorGUILayout.LabelField("--------------------------------------------------------------------------------");
            EditorGUILayout.EndScrollView();
        }
    }
}
