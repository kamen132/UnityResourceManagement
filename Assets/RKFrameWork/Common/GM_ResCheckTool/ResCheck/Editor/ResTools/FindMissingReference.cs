using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace GMResChecker
{
    public class FindMissingReference : ResCheckBaseSubWindowEditor
    {
        private static Dictionary<GameObject, List<GameObject>> findResult = new Dictionary<GameObject, List<GameObject>>();
        private static Dictionary<UnityEngine.Object, List<UnityEngine.Object>> prefabs = new Dictionary<UnityEngine.Object, List<UnityEngine.Object>>();
        private static Dictionary<UnityEngine.Object, string> refPaths = new Dictionary<UnityEngine.Object, string>();
        public override void OnGUIDraw()
        {
            EditorGUILayout.LabelField("--------------------------------------------------------------------------------");
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("检查丢失引用", GUILayout.Width(400), GUILayout.Height(20)))
            {
                FindData();
            }
            if (findResult.Count > 0)
            {
                if (GUILayout.Button("删除所有丢失引用", GUILayout.Width(400), GUILayout.Height(20)))
                {
                    RemovAllMissingScript();
                    FindData();
                }
            }
            EditorGUILayout.EndHorizontal();
            ShowFindResult();
        }
        private static void FindData()
        {
            findResult.Clear();
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
                    EditorUtility.DisplayProgressBar(title, Path.GetFileName(assetPath), nameIndex / pathNames.Length);
                    GameObject go = ResCheckJenkinsEntrance.LoadAssetAtPath(assetPath) as GameObject;
                    OnDealObjs(go);
                }
            }
            EditorUtility.ClearProgressBar();
        }
        private static void OnDealObjs(GameObject go)
        {
            DealObjs(go, go);
        }
        private static void DealObjs(GameObject parent, GameObject go)
        {
            if (go == null)
            {
                return;
            }
            int count = go.transform.childCount;
            for (int i = 0; i < count; i++)
            {
                IsMissScript(parent, go.transform.GetChild(i).gameObject);

            }
            IsMissScript(parent, go);
        }
        private static void IsMissScript(GameObject parent, GameObject go)
        {
            var components = go.GetComponents<Component>();
            for (int j = 0; j < components.Length; j++)
            {
                if (components[j] == null)
                {
                    AddFindData(parent, go);
                }
            }
        }

        static void AddFindData(GameObject parent, GameObject child)
        {
            if (findResult.ContainsKey(parent) == false)
            {
                findResult[parent] = new List<GameObject>();
            }
            if (findResult[parent].Contains(child) == false)
            {
                findResult[parent].Add(child);
            }
        }
        private static void RemovAllMissingScript()
        {
            foreach (var item in findResult)
            {
                var go = item.Key;
                RemoveOneMissScript(go);
            }
        }
        private static void RemoveOneMissScript(GameObject go)
        {
            int count = go.transform.childCount;
            for (int i = 0; i < count; i++)
            {
                RemoveMissScript(go.transform.GetChild(i).gameObject);
            }
            RemoveMissScript(go);
        }
        //不靠谱，因为不加载到场景，只能便利一个层级
        private static void RemoveMissScript(GameObject go)
        {
            int count = go.transform.childCount;
            var components = go.GetComponents<Component>();
            var serializedObject = new SerializedObject(go);
            var prop = serializedObject.FindProperty("m_Component");
            int r = 0;
            for (int j = 0; j < components.Length; j++)
            {
                if (components[j] == null)
                {
                    prop.DeleteArrayElementAtIndex(j - r);
                    r++;
                }
            }
            serializedObject.ApplyModifiedProperties();
        }
        private static void DealObj2(GameObject go)
        {
            Regex guidRegex = new Regex("m_Script: {fileID: (.*), guid: (?<GuidValue>.*?), type:");
            Regex fileRegex = new Regex("--- !u!(?<groupNum>.*?) &(?<fileID>.*?)\r\n");

            string filePath = ResCheckJenkinsEntrance.GetAssetPath(go);
            string s = File.ReadAllText(filePath);

            MatchCollection matchList = guidRegex.Matches(s);
            Debug.Log("matchList.Count===" + matchList.Count);
            if (matchList != null)
            {
                for (int i = matchList.Count - 1; i >= 0; i--)
                {
                    string guid = matchList[i].Groups["GuidValue"].Value;
                    //Debug.Log("guid===" + guid);

                    string assetpath = AssetDatabase.GUIDToAssetPath(guid);
                    //Debug.Log("assetpath===" + assetpath);
                    bool isFileExist = ResCheckEditorUtil.FileExist(assetpath);
                    if (isFileExist == false)
                    {

                    }
                    //Debug.Log("isFileExist===" + isFileExist);

                    //isChange = true;
                    //int startIndex = s.LastIndexOf(groupSpilChar, matchList[i].Index);
                    //int endIndex = s.IndexOf(groupSpilChar, matchList[i].Index);

                    //Match fileMatch = fileRegex.Match(s.Substring(startIndex, fileStrLenght));
                    //fileStr = "- " + fileMatch.Groups["groupNum"].Value + ": {fileID: " + fileMatch.Groups["fileID"].Value + "}\r\n  ";

                    //s = s.Replace(s.Substring(startIndex, endIndex - startIndex), "");
                    //s = s.Replace(fileStr, "");

                }
            }
            //if (isChange)
            //{
            //    File.WriteAllText(filePath, s);
            //    Debug.Log(obj.name + " missing scripts destory success!");
            //}
        }
        private static void DealObj1(GameObject go)
        {
            Debug.Log("--iter.go-----" + go.name);
            Component[] cps = go.GetComponentsInChildren<Component>(true);//获取这个物体身上所有的组件
            foreach (var cp in cps)//遍历每一个组件
            {
                if (!cp)
                {
                    if (!prefabs.ContainsKey(go))
                    {
                        prefabs.Add(go, new List<UnityEngine.Object>() { cp });
                    }
                    else
                    {
                        prefabs[go].Add(cp);
                    }
                    continue;
                }
                SerializedObject so = new SerializedObject(cp);
                var iter = so.GetIterator();//拿到迭代器
                while (iter.NextVisible(true))//如果有下一个属性
                {
                    if (iter.propertyType == SerializedPropertyType.ObjectReference)  //如果这个属性类型是引用类型的
                    {
                        Debug.Log("--iter.objectReferenceValue-----" + iter.objectReferenceValue);
                        Debug.Log("--iter.objectReferenceInstanceIDValue-----" + iter.objectReferenceInstanceIDValue);
                        if (iter.objectReferenceValue == null && iter.objectReferenceInstanceIDValue != 0)  //引用对象是null 并且 引用ID不是0 说明丢失了引用
                        {
                            Debug.Log("-------" + iter.propertyPath);
                            if (!refPaths.ContainsKey(cp))
                            {
                                refPaths.Add(cp, iter.propertyPath);
                            }
                            else

                            {
                                refPaths[cp] += " | " + iter.propertyPath;
                            }

                            if (prefabs.ContainsKey(go))
                            {
                                if (!prefabs[go].Contains(cp)) prefabs[go].Add(cp);
                            }
                            else
                            {
                                prefabs.Add(go, new List<UnityEngine.Object>() { cp });
                            }
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
            foreach (var item in findResult)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.ObjectField(item.Key, item.Key.GetType(), true, GUILayout.Width(400));
                EditorGUILayout.BeginVertical();
                List<GameObject> subObjs = item.Value;
                foreach (var subObj in subObjs)
                {
                    EditorGUILayout.ObjectField(subObj, subObj.GetType(), true, GUILayout.Width(200));
                }
                EditorGUILayout.EndVertical();

                if (GUILayout.Button("删除丢失的脚本", GUILayout.Width(200), GUILayout.Height(20)))
                {
                    RemoveOneMissScript(item.Key);
                    FindData();
                    return;
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }
    }
}
