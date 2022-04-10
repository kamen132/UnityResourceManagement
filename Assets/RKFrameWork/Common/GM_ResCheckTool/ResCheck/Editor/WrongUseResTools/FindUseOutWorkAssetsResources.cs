using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace GMResChecker
{
    public class FindUseOutWorkAssetsResources : ResCheckBaseSubWindowEditor
    {
        private static Dictionary<string, Dictionary<string, int>> objResultDic = new Dictionary<string, Dictionary<string, int>>();

        private static Dictionary<Object, Dictionary<Object, int>> objectResultDic = new Dictionary<Object, Dictionary<Object, int>>();
        public override void OnGUIDraw()
        {
            EditorGUILayout.LabelField("--------------------------------------------------------------------------------");
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("查找引用了workAssets以外的资源以及machine与非machine相互引用", GUILayout.Width(400), GUILayout.Height(20)))
            {
                objectResultDic.Clear();
                objResultDic.Clear();
                FindObjectData();
                FindMatData();
            }
            GUILayout.Space(10);
            
            EditorGUILayout.EndHorizontal();
            ShowFindResult();
        }
        //[@MenuItem("Tools/GetUseOutWorkAssetsResources")]
        public static string GetUseOutWorkAssetsResources()
        {
            objectResultDic.Clear();
            objResultDic.Clear();
            FindObjectData();
            FindMatData();
            EditorUtility.ClearProgressBar();
            StringBuilder builder = new StringBuilder();
            builder.Append("=========GetUseOutWorkAssets:" + (objResultDic.Count ) + "=========\n");
            foreach (var item in objResultDic)
            {
                string key = item.Key;
                foreach(var subItem in item.Value)
                {
                    builder.Append(key);
                    builder.Append("  ");
                    builder.Append(subItem.Key);
                    builder.Append("\n");
                }
            }
           
            builder.Append("\n\n");
            return builder.ToString();
        }
        private static void FindObjectData()
        {
            string BaseAssetPath = "WorkAssets";
            string Machines = "Machines";
            string endSuffix = "*.prefab";
            string directoryPath = Path.Combine(Application.dataPath, BaseAssetPath);
            string[] prefabNames = Directory.GetFiles(directoryPath, endSuffix, SearchOption.AllDirectories);
            foreach(var item in prefabNames)
            {
                string assetPath = ResCheckEditorUtil.GetAssetPath(item);
                string[] deps = ResCheckJenkinsEntrance.GetDependencies(assetPath);
                foreach(string dep in deps)
                {
                    string newDep = ResCheckEditorUtil.FormatPath(dep);
                    //Machines
                    bool ignore = newDep.EndsWith(".cs") ||
                        newDep.Contains("Common/Resources")||
                        newDep.Contains("Packages") ;
                    if(ignore)
                    {
                        continue;
                    }
                    bool unUseWorkAssetsRes = newDep.Contains(BaseAssetPath) == false;
                      
                    bool abUseMachinesRes = assetPath.Contains(Machines) == false && newDep.Contains(Machines);
                    bool machinesUseAbRes = NormalNoMachineRes(newDep) == false && assetPath.Contains(Machines) && newDep.Contains(Machines) == false;
                    if (unUseWorkAssetsRes || abUseMachinesRes || machinesUseAbRes)
                    {
                        if(objResultDic.ContainsKey(assetPath) == false)
                        {
                            objResultDic[assetPath] = new Dictionary<string, int>();
                        }
                        objResultDic[assetPath][newDep] = 1;
                    }
                }
            }

            foreach (var item in objResultDic)
            {
                Object obj = ResCheckJenkinsEntrance.LoadAssetAtPath(item.Key);
                objectResultDic[obj] = new Dictionary<Object, int>();
                foreach(var dep in item.Value)
                {
                    Object depObj = ResCheckJenkinsEntrance.LoadAssetAtPath(dep.Key);
                    objectResultDic[obj][depObj] = 1;
                }
            }
        }
        static bool NormalNoMachineRes(string path)
        {
            return path.Contains("WorkAssets/Fonts")
                || path.Contains("WorkAssets/BMFont")
                 || path.Contains("WorkAssets/shader")
                 || path.Contains("WorkAssets/TMPFonts")
                 || path.Contains("WorkAssets/Materials");
        }
        private static void FindMatData()
        {
            string BaseAssetPath = "WorkAssets";
            string endSuffix = "*.mat";
            string directoryPath = Path.Combine(Application.dataPath, BaseAssetPath);
            string[] prefabNames = Directory.GetFiles(directoryPath, endSuffix, SearchOption.AllDirectories);
            foreach (var item in prefabNames)
            {
                string assetPath = ResCheckEditorUtil.GetAssetPath(item);
                string[] deps = ResCheckJenkinsEntrance.GetDependencies(assetPath);
                foreach (string dep in deps)
                {
                    string newDep = ResCheckEditorUtil.FormatPath(dep);
                    if (newDep.Contains(BaseAssetPath) == false &&
                        newDep.EndsWith(".cs") == false &&
                        newDep.Contains("Common/Resources") == false &&
                        newDep.Contains("Packages") == false
                        )
                    {
                        if (objResultDic.ContainsKey(assetPath) == false)
                        {
                            objResultDic[assetPath] = new Dictionary<string, int>();
                        }
                        objResultDic[assetPath][newDep] = 1;
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

            foreach (var item in objectResultDic)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.ObjectField(item.Key, typeof(Object), true, GUILayout.Width(400));
                EditorGUILayout.BeginVertical();
                foreach(var dep in item.Value)
                {
                    EditorGUILayout.ObjectField(dep.Key, typeof(Object), true, GUILayout.Width(400));
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.LabelField("--------------------------------------------------------------------------------");
            EditorGUILayout.EndScrollView();
        }
    }
}
