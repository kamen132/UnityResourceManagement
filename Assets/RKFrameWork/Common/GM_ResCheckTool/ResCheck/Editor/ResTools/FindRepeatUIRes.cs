using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
namespace GMResChecker
{
    public class FindRepeatUIRes : ResCheckBaseSubWindowEditor
    {

        private static Dictionary<string, Dictionary<string, int>> depDic = new Dictionary<string, Dictionary<string, int>>();
        private static Dictionary<string, Dictionary<string, int>> moveDic = new Dictionary<string, Dictionary<string, int>>();
        private static Dictionary<string, Dictionary<string, int>> configDic = new Dictionary<string, Dictionary<string, int>>();
        //[@MenuItem("Tools/UI 资源检查 重复引用资源的uiprefab", false, 1001)]
        public static void OnFindRepeatUIRes()
        {
            configDic.Clear();
            moveDic.Clear();
            depDic.Clear();
            LoadConfig();
            DoUIPrefab();
            CheckInCommon();
            MoveAsset();
            Debug.Log("need move asset index==" + moveDic.Count);
        }
        public override void OnGUIDraw()
        {

        }
        static void LoadConfig()
        {
            string filePath = Application.dataPath + "/Scripts/Editor/prefab_with_atlas.txt";
            string data = (string)File.ReadAllText(filePath);
            Debug.Log("data=" + data);
            string[] parmes = new string[] { "\r\n" };
            string[] rows = data.Split(parmes, StringSplitOptions.RemoveEmptyEntries);
            if (rows.Length > 1)
            {
                for (int i = 1; i < rows.Length; i++)
                {
                    string item = rows[i];
                    string[] items = item.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                    string key = null;
                    for (int j = 0; j < items.Length; j++)
                    {
                        string itemName = items[j].Trim(); ;
                        if (j == 0)
                        {
                            //itemName = " Assets/WorkAssets/ui/battle/".ToLower() + itemName;
                            key = itemName.ToLower();

                            if (configDic.ContainsKey(itemName) == false)
                            {
                                configDic[itemName] = new Dictionary<string, int>();
                            }

                        }
                        else
                        {
                            itemName = itemName + ".prefab";
                            Dictionary<string, int> dic = configDic[key];
                            dic[itemName] = 1;
                        }
                    }
                }

            }
        }
        static void DoUIPrefab()
        {
            string uiPrefabRoot = Application.dataPath + "/WorkAssets/ui_assets/prefab";
            //Debug.Log("uiPrefabRoot===" + uiPrefabRoot);
            uiPrefabRoot = ResCheckEditorUtil.FormatPath(uiPrefabRoot);
            string[] prefabs = Directory.GetFiles(uiPrefabRoot, "*.prefab", SearchOption.AllDirectories);
            //Debug.Log("Length===" + prefabs.Length);
            for (int i = 0; i < prefabs.Length; i++)
            {
                string prefab = prefabs[i];
                string DoPrefabPath = ResCheckEditorUtil.GetAssetPath(prefab);
                string[] dependencies = ResCheckJenkinsEntrance.GetDependencies(DoPrefabPath, true);
                if (depDic.ContainsKey(DoPrefabPath) == false)
                {
                    depDic[DoPrefabPath] = new Dictionary<string, int>();
                }
                Dictionary<string, int> prefabDeps = depDic[DoPrefabPath];
                for (int j = 0; j < dependencies.Length; j++)
                {
                    string depName = dependencies[j];
                    if (IsDepNeed(depName))
                    {
                        prefabDeps[depName] = 1;
                    }
                }
            }
        }

        static void MoveAsset()
        {
            List<string> title = new List<string>();
            title.Add("pic name");
            title.Add("prefab names");
            List<List<string>> data = new List<List<string>>();
            foreach (var item in moveDic)
            {
                List<string> list = new List<string>();
                string picName = item.Key;
                list.Add(picName);
                Dictionary<string, int> prefabDic = item.Value;
                StringBuilder builder = new StringBuilder();
                foreach (var prefabItem in prefabDic)
                {
                    builder.Append(prefabItem.Key);
                    builder.Append(" | ");
                }
                list.Add(builder.ToString());
                data.Add(list);
            }
            //ResCheckJenkinsEntrance.ExportExcle("FindRepeatUIRes", "sheet", title, data);
        }
        static void CheckInCommon()
        {
            foreach (var itemDep in depDic)
            {
                string prefabName = itemDep.Key;
                Dictionary<string, int> prefabDepDic = itemDep.Value;
                foreach (var prefabDepItem in prefabDepDic)
                {
                    string picName = prefabDepItem.Key;
                    if (NoFromCommon(picName))
                    {
                        CheckSamePic(prefabName, picName);
                    }
                }
            }
        }

        static void CheckSamePic(string prefabName, string picName)
        {
            foreach (var otherItemDep in depDic)
            {
                string otherPrefabName = otherItemDep.Key;
                if (otherPrefabName.Equals(prefabName) == false)
                {
                    Dictionary<string, int> otherPrefabDepDic = otherItemDep.Value;
                    if (otherPrefabDepDic.ContainsKey(picName))
                    {
                        //Debug.Log(" same pic name = " + picName + "  from prefab=" + prefabName + "  and " + otherPrefabName);
                        //moveDic[picName] = GetMovePath(picName);
                        AddPicName(picName, prefabName, otherPrefabName);
                    }
                }
            }
        }

        static void AddPicName(string picName, string prefabName1, string prefabName2)
        {
            prefabName1 = Path.GetFileName(prefabName1).Trim();
            prefabName2 = Path.GetFileName(prefabName2).Trim();
            if (InConfig(picName, prefabName1, prefabName2))
            {
                return;
            }
            //string name = FindBaseAtlasPath(picName);
            //Debug.Log("name===" + name);
            if (moveDic.ContainsKey(picName))
            {
                Dictionary<string, int> item = moveDic[picName];
                item[prefabName1] = 1;
                item[prefabName2] = 1;
            }
            else
            {
                Dictionary<string, int> item = new Dictionary<string, int>();
                item[prefabName1] = 1;
                item[prefabName2] = 1;
                moveDic[picName] = item;
            }
        }

        static string GetMovePath(string picName)
        {
            string parentName = Directory.GetParent(picName).Name;
            return "Assets/WorkAssets/ui/common/" + parentName + "/" + Path.GetFileName(picName);
        }

        static string FindBaseAtlasPath(string picName)
        {
            picName = picName.ToLower().Trim();
            int index = picName.IndexOf("workassets");
            picName = picName.Substring(index + 14);
            string[] parmes = new string[] { "/" };
            string[] rows = picName.Split(parmes, StringSplitOptions.RemoveEmptyEntries);
            if (rows.Length > 0)
            {
                return rows[0];
            }
            return "";
        }
        static bool InConfig(string picName, string prefabName1, string prefabName2)
        {
            picName = FindBaseAtlasPath(picName);
            if (configDic.ContainsKey(picName))
            {
                Dictionary<string, int> item = configDic[picName];
                if (item.ContainsKey(prefabName1) && item.ContainsKey(prefabName2))
                {
                    return true;
                }
            }
            return false;
        }

        static bool NoFromCommon(string picName)
        {
            picName = picName.ToLower();
            return picName.Contains("ui/common") == false
                && picName.Contains("ui/icons") == false
                && picName.Contains("ui/image") == false
                && picName.Contains("ui/complex") == false
                && picName.Contains("WorkAssets/effect") == false
                && picName.Contains("dynamictexture") == false
                 && picName.Contains("ui/herocommon") == false
                ;
        }
        static bool IsDepNeed(string depName)
        {
            depName = depName.ToLower();
            return depName.EndsWith(".png")
                && depName.Contains("assets/workassets/ui")
                ;
        }
    }
}

