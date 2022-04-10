using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using static GMResChecker.ResCheckEditorUtil;

namespace GMResChecker
{
    public class FindUnuseShaderKeywords : ResCheckBaseSubWindowEditor
    {
        private static MethodInfo GetShaderVariantEntries = null;
        private static ShaderVariantCollection toolSVC = null;

        private static Dictionary<Shader, ShaderKeywordsData> shaderDataDic = new Dictionary<Shader, ShaderKeywordsData>();
        private static Dictionary<Object, List<string>> matResult = new Dictionary<Object, List<string>>();
        public override void OnGUIDraw()
        {
            EditorGUILayout.LabelField("--------------------------------------------------------------------------------");
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("FindUnuseShaderKeywords", GUILayout.Width(400), GUILayout.Height(20)))
            {
                FindData();
            }

            EditorGUILayout.EndHorizontal();
            ShowFindResult();
        }

        public static void AutoClearSceneData()
        {
            FindData();
            ClearSceneUnuseShaderKeywords();
        }
        private static void FindData()
        {
            GetToolSVC();
            List<string> allMats = GetAllMats();
            float index = 1;
            foreach (var matPath in allMats)
            {
                var obj = AssetDatabase.LoadMainAssetAtPath(matPath);
                if (obj is Material)
                {
                    Material mat = obj as Material;
                    EditorUtility.DisplayProgressBar("处理mat", string.Format("处理:{0}", Path.GetFileName(matPath)), index / allMats.Count);
                    string[] shaderKeywords = mat.shaderKeywords;
                    Shader shader = mat.shader;

                    if (shaderDataDic.ContainsKey(shader) == false)
                    {
                        shaderDataDic[shader] = GetShaderKeywords(shader, GetShaderVariantEntries, toolSVC);
                    }
                    Dictionary<string, int> dic = shaderDataDic[shader].keywordsDic;
                    foreach (string keywords in shaderKeywords)
                    {
                        bool isSpecial = keywords.Equals("_EMISSION") && shader.name.Equals("H3D/H3DStandard");
                        if (dic.ContainsKey(keywords) == false || isSpecial)
                        {
                            if (matResult.ContainsKey(mat) == false)
                            {
                                matResult[mat] = new List<string>();
                            }
                            matResult[mat].Add(keywords);
                        }
                    }
                }

                index++;
            }

            EditorUtility.ClearProgressBar();
        }

        static List<string> GetAllMats()
        {
            Dictionary<string, bool> assets = ResCheckJenkinsEntrance.GetHaveDepsFile();
            List<string> allMats = new List<string>();
            foreach (var item in assets)
            {
                string path = PathUtil.GetAssetPath(item.Key);
                if (path.EndsWith(".prefab"))
                {
                    string[] dependenciesPath = ResCheckJenkinsEntrance.GetDependencies(path, true);
                    var mats = dependenciesPath.ToList().FindAll((dp) => dp.EndsWith(".mat"));
                    allMats.AddRange(mats);
                }
                else
                {
                    allMats.Add(path);
                }
            }

            allMats = allMats.Distinct().ToList();
            return allMats;
        }
        static void GetToolSVC()
        {
            if (toolSVC != null)
            {
                return;
            }
            if (GetShaderVariantEntries == null)
            {
                GetShaderVariantEntries = typeof(ShaderUtil).GetMethod("GetShaderVariantEntriesFiltered", BindingFlags.NonPublic | BindingFlags.Static);
            }
            toolSVC = new ShaderVariantCollection();
            string shaderPath = Path.Combine(Application.dataPath, "Product/Editor/Resources/Shader");
            string[] _allFiles = Directory.GetFiles(shaderPath, "*", SearchOption.AllDirectories);
            foreach (string shader in _allFiles)
            {
                if (shader.EndsWith(".shader"))
                {
                    var path = PathUtil.GetAssetPath(shader);
                    ShaderVariantCollection.ShaderVariant shaderVariant = new ShaderVariantCollection.ShaderVariant();
                    shaderVariant.shader = ResCheckJenkinsEntrance.LoadAssetAtPath(path) as Shader;
                    toolSVC.Add(shaderVariant);
                }
            }
        }

        static void ClearAllUnuseShaderKeywords()
        {
            foreach (var item in matResult)
            {
                RemoveWrongRes(item.Key, false);
            }
            AssetDatabase.SaveAssets();
            matResult.Clear();
        }
        static void ClearSceneUnuseShaderKeywords()
        {
            List<Object> remData = new List<Object>();
            foreach (var item in matResult)
            {
                string path = ResCheckJenkinsEntrance.GetAssetPath(item.Key);
                if (path.Contains("Resources/Scene"))
                {
                    RemoveWrongRes(item.Key, false);
                    remData.Add(item.Key);
                }

            }
            AssetDatabase.SaveAssets();
            foreach (var item in remData)
            {
                matResult.Remove(item);
            }
        }
        private static Vector3 scrollPos = Vector3.zero;
        private static Object selectObj = null;
        private static void ShowFindResult()
        {
            GUILayout.Space(5);
            if (matResult.Count > 0)
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("ClearAllUnuseShaderKeywords", GUILayout.Width(200), GUILayout.Height(20)))
                {
                    ClearAllUnuseShaderKeywords();
                }
                if (GUILayout.Button("ClearSceneUnuseShaderKeywords", GUILayout.Width(200), GUILayout.Height(20)))
                {
                    ClearSceneUnuseShaderKeywords();
                }
                EditorGUILayout.EndHorizontal();
            }


            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, true, GUILayout.Width(850), GUILayout.Height(600));
            EditorGUILayout.LabelField("--------------------------------------------------------------------------------");
            GUILayout.Space(5);
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("totle:" + matResult.Count);
            int maxShowCount = 20;
            foreach (var item in matResult)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.ObjectField(item.Key, item.Key.GetType(), true, GUILayout.Width(400));
                EditorGUILayout.BeginVertical();
                List<string> subObjs = item.Value;
                foreach (var subObj in subObjs)
                {
                    EditorGUILayout.LabelField(subObj, GUILayout.Width(200));
                }
                EditorGUILayout.EndVertical();
                if (GUILayout.Button("清除", GUILayout.Width(100), GUILayout.Height(20)))
                {
                    selectObj = item.Key;
                }
                EditorGUILayout.EndHorizontal();
                maxShowCount--;
                if (maxShowCount < 0)
                {
                    break;
                }
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
            RemoveWrongRes(selectObj, true);
        }
        private static void RemoveWrongRes(Object matObj, bool isSingleClear)
        {
            if (matObj != null)
            {
                List<string> data = matResult[matObj];
                Material mat = matObj as Material;
                if (mat == null)
                {
                    return;
                }
                string[] shaderKeywords = mat.shaderKeywords;
                List<string> shaderKeywordsList = shaderKeywords.ToList();
                foreach (var key in data)
                {
                    shaderKeywordsList.Remove(key);
                }
                shaderKeywords = shaderKeywordsList.ToArray();
                mat.shaderKeywords = shaderKeywords;
                EditorUtility.SetDirty(mat);
                if (isSingleClear)
                {
                    matResult.Remove(matObj);
                    AssetDatabase.SaveAssets();
                }

            }
        }
    }
}
