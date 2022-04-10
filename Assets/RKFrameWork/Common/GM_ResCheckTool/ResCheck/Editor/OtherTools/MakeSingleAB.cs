
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace GMResChecker
{
    public class MakeSingleAB : ResCheckBaseSubWindowEditor
    {
        private static Object findObj;
        private static List<Object> findResult = new List<Object>();
        static List<string> allFile = new List<string>();
        public override void OnGUIDraw()
        {
            EditorGUILayout.LabelField("请选择或者拖拽您想要打ab的prefab");
            findObj = EditorGUILayout.ObjectField(findObj, typeof(Object), true, GUILayout.Width(200));
            if (GUILayout.Button("开始生成", GUILayout.Width(150), GUILayout.Height(20)))
            {
                if (findObj == null)
                {
                    return;
                }
                CollectVariantProcessor.CanStart = true;
                var deps = ResCheckJenkinsEntrance.GetDependencies(ResCheckJenkinsEntrance.GetAssetPath(findObj), true);
                allFile.Clear();
                foreach (var filePath in deps)
                {
                    if (ResCheckEditorUtil.IsReal(filePath) == false)
                    {
                        continue;
                    }
                    string key = PathUtil.GetAssetPath(filePath);
                    key = key.ToLower();
                    if (allFile.Contains(key) == false)
                    //if (allFile.Contains(key) == false && key.EndsWith(".png") == false)
                    {
                        allFile.Add(key);
                    }
                   
                }

                findResult.Clear();
                AssetBundleBuild[] data = MakeAssetBundleBuild(findObj.name);
                string outPath = Application.streamingAssetsPath +"/AB";
                FileManager.DeleteDirectory(outPath);
                FileManager.CreateDirectory(outPath);
                BuildPipeline.BuildAssetBundles(outPath, data, BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.Android);
                AssetDatabase.Refresh();
                CollectVariantProcessor.CanStart = false;
            }
            ShowFindResult();
        }

        static AssetBundleBuild[] MakeAssetBundleBuild(string abName)
        {
            var assetBundleBuilds = new AssetBundleBuild[1];
            allFile.Sort();
            AssetBundleBuild assetBundleBuild = new AssetBundleBuild();
            assetBundleBuild.assetBundleName = abName;
            var assets = allFile.ToArray();
            assetBundleBuild.assetNames = assets;
            assetBundleBuilds[0] = assetBundleBuild;
            return assetBundleBuilds;
        }

        private static Vector3 scrollPos = Vector3.zero;
        static void ShowFindResult()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, true, GUILayout.Width(1000), GUILayout.Height(600));
            EditorGUILayout.LabelField("--------------------------------------------------------------------------------");

            GUILayout.Space(5);
            EditorGUILayout.BeginVertical();
            foreach (var item in findResult)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.ObjectField(item, item.GetType(), true, GUILayout.Width(250));
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }
    }
}

