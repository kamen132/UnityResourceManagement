using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace GMResChecker
{
    public class FindSceneCrossReference : ResCheckBaseSubWindowEditor
    {
        private static Dictionary<string, Dictionary<string, int>> objResultDic = new Dictionary<string, Dictionary<string, int>>();
        private static Dictionary<Object, Dictionary<Object, int>> objectResultDic = new Dictionary<Object, Dictionary<Object, int>>();
        public override void OnGUIDraw()
        {
            EditorGUILayout.LabelField("--------------------------------------------------------------------------------");
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("查找交叉引用", GUILayout.Width(200), GUILayout.Height(20)))
            {
                objectResultDic.Clear();
                objResultDic.Clear();
                FindObjectData();
            }
            GUILayout.Space(10);
            
            EditorGUILayout.EndHorizontal();
            ShowFindResult();
        }
        //[@MenuItem("Tools/GetFindMachinesCrossReference")]
        public static string GetFindCrossReference()
        {
            objectResultDic.Clear();
            objResultDic.Clear();
            FindObjectData();
            EditorUtility.ClearProgressBar();
            StringBuilder builder = new StringBuilder();
            builder.Append("=========GetFindSceneCrossReference:" + (objResultDic.Count) + "=========\n");
            foreach (var item in objResultDic)
            {
                string key = item.Key;
                foreach (var subItem in item.Value)
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
            string BaseAssetPath = "Product/Editor/Resources/Scene";
            string directoryPath = Path.Combine(Application.dataPath, BaseAssetPath);
            string[] dirs = Directory.GetDirectories(directoryPath);
            foreach(var dir in dirs)
            {
                string assetDir = ResCheckEditorUtil.GetAssetPath(dir);
                if(assetDir.Contains("_Editor"))
                {
                    continue;
                }
                string fonderName = PathUtil.GetFolderName(dir);
                
                string[]  files = Directory.GetFiles(dir, "*", SearchOption.AllDirectories);
                foreach(var file in files)
                {
                    if(ResCheckEditorUtil.IsNODeps(file))
                    {
                        continue;
                    }
                    string assetPath = ResCheckEditorUtil.GetAssetPath(file);
                    string[] deps = ResCheckJenkinsEntrance.GetDependencies(assetPath);
                    foreach (string dep in deps)
                    {
                        string newDep = ResCheckEditorUtil.FormatPath(dep);
                        if (
                            newDep.Contains("Resources/Scene/Common") == false
                            && newDep.EndsWith(".cs") == false 
                            &&newDep.Contains("Packages") == false 
                            && newDep.Equals(assetPath) == false
                            &&IsInAnother(newDep, fonderName)
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
        private const string ScenePath = "Assets/Product/Editor/Resources/Scene/";
        private static bool IsInAnother(string newDep, string fonderName)
        {
            //Assets/Product/Editor/Resources/Scene/Scene_inverse_01/F/scene_inverse_01_f_train_01.FBX
            if(newDep.Contains("Dungeon/InteractAnimator") ||
                newDep.Contains("DungeonCommonAssetsInteractAnimator")||
                newDep.Contains("Resources/CommonAssets")
                )
            {
                return false;
            }
            if(newDep.Contains(ScenePath))
            {
                string path = newDep.Replace(ScenePath, "");
                int index = path.IndexOf('/');
                string depFonder = path.Substring(0, index);
                return depFonder.Contains(fonderName) == false;
            }
            return newDep.Contains("tmpmat") == false
                && newDep.Contains("Resources/Shader") == false
                 && newDep.Contains("Editor/Resources/Effect") == false
                && newDep.Contains("H3DTech/Script") == false
                 && newDep.Contains("ThirdParty/AstarPathfindingProject") == false;

            //foreach(var _dir in dirs)
            //{
            //    string dirName = PathUtil.GetFolderName(_dir);
            //    //Debug.Log("dirName==" + dirName + "--path--" + path + "--currentDir==" + currentDir);
            //    if(dir != currentDir && path.Contains(dirName))
            //    {
            //        Debug.Log("path===" + path);
            //        return true;
            //    }
            //}
            //return false;
        }

        private static Vector3 scrollPos = Vector3.zero;
        private void ShowFindResult()
        {
            if(objectResultDic.Count>0)
            {
                if (GUILayout.Button("导出excle", GUILayout.Width(200), GUILayout.Height(20)))
                {
                    string __selectedPath = EditorUtility.OpenFolderPanel("请选择要保存的文件夹", Application.dataPath + "/../", "");
                    List<string> title = new List<string>();
                    title.Add("ObjectName");
                    title.Add("引用其他场景的资源");
                    List<List<string>> data = new List<List<string>>();
                    foreach (var item in objResultDic)
                    {
                        foreach (var subItem2 in item.Value)
                        {
                            List<string> itemData = new List<string>();
                            itemData.Add(item.Key);
                            itemData.Add(subItem2.Key);
                            data.Add(itemData);
                        }
                     
                    }
                    ResCheckJenkinsEntrance.ExportExcle(__selectedPath, "SceneCrossData", "sheet", title, data);

                }
            }
            

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
