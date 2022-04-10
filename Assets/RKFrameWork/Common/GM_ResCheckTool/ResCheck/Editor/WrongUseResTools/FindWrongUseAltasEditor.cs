using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
namespace GMResChecker
{
    public class FindWrongUseAltasEditor : ResCheckBaseSubWindowEditor
    {
        private class WrongUseAltasData
        {
            public string prefabName;
            public string imageName;
        }
        private static Dictionary<string, string[]> atlasDepsDic = new Dictionary<string, string[]>();
        private static Dictionary<string, Dictionary<string, int>> otherPrefabDepsDic = new Dictionary<string, Dictionary<string, int>>();
        private static List<WrongUseAltasData> result = new List<WrongUseAltasData>();
        //[@MenuItem("Tools/检查错误引用了图集的非UI的prefab", false, 1002)]
        public static void OnFindWrongUseAltas()
        {
            result.Clear();
            GetAtalsImage();
            CheckPrefabs();
            StartCheck();
            Debug.Log("------------查找完成---------");
            OnExport();
        }

        static void OnExport()
        {
            List<string> title = new List<string>();
            title.Add("prefab name");
            title.Add("image names");
            List<List<string>> data = new List<List<string>>();
            foreach (WrongUseAltasData item in result)
            {
                List<string> list = new List<string>();
                list.Add(item.prefabName);
                list.Add(item.imageName);
                data.Add(list);
            }
            //ResCheckJenkinsEntrance.ExportExcle("FindWrongUseAltas", "sheet", title, data);
        }
        public override void OnGUIDraw()
        {

        }
        static void GetAtalsImage()
        {
            string imagePath = "Assets/WorkAssets/ui/";
            string[] paths = Directory.GetFiles(imagePath, "*.spriteatlas", SearchOption.AllDirectories);
            //Debug.Log("paths---" + paths.Length);
            for (int i = 0; i < paths.Length; i++)
            {
                string path = paths[i];
                path = ResCheckEditorUtil.FormatPath(path);
                //Debug.Log("path====" + path);
                string[] pngs = ResCheckJenkinsEntrance.GetDependencies(path);
                atlasDepsDic[path] = pngs;
            }
        }
        static void CheckPrefabs()
        {
            Dictionary<string, bool> files = new Dictionary<string, bool>();
            string uiPrefabPath = "Assets/WorkAssets/ui_assets";
            string uiPngPath = "Assets/WorkAssets/ui";
            foreach (var item in files)
            {
                string assetPath = ResCheckEditorUtil.GetAssetPath(item.Key);

                if (assetPath.Contains(uiPrefabPath) == false && assetPath.EndsWith(".prefab")) //不去查找ui的prefab
                {

                    Dictionary<string, int> depsDic = new Dictionary<string, int>();
                    string[] deps = ResCheckJenkinsEntrance.GetDependencies(assetPath, false);
                    //Debug.Log("---" + deps.Length);
                    for (int i = 0; i < deps.Length; i++)
                    {
                        string dep = deps[i];
                        if (dep.Contains(uiPngPath) && dep.Contains("Assets/WorkAssets/ui/common") == false)
                        {
                            depsDic[dep] = 1;
                        }
                    }
                    if (depsDic.Count > 0)
                    {
                        //Debug.Log("assetPath===" + assetPath + "==" + deps.Length);
                        otherPrefabDepsDic[assetPath] = depsDic;
                    }
                }
            }
        }

        static void StartCheck()
        {
            foreach (var item in atlasDepsDic)
            {
                string atlasName = item.Key;
                string[] deps = item.Value;
                for (int i = 0; i < deps.Length; i++)
                {
                    IsInAtals(atlasName, deps[i]);
                }
            }
        }

        static void IsInAtals(string atlasName, string name)
        {
            foreach (var item in otherPrefabDepsDic)
            {
                string prefabName = item.Key;
                Dictionary<string, int> dic = item.Value;
                if (dic.ContainsKey(name))
                {
                    WrongUseAltasData data = new WrongUseAltasData();
                    data.prefabName = prefabName;
                    data.imageName = name;
                    result.Add(data);
                }
            }
        }
    }
}