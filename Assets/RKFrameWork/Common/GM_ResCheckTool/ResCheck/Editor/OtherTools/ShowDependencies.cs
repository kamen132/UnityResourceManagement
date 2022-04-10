
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace GMResChecker
{
    public class ShowDependencies : ResCheckBaseSubWindowEditor
    {
        private static Object findObj;
        private static List<Object> findResult = new List<Object>();
        public override void OnGUIDraw()
        {
            EditorGUILayout.LabelField("请选择或者拖拽您想要查找的");
            findObj = EditorGUILayout.ObjectField(findObj, typeof(Object), true, GUILayout.Width(200));
            if (GUILayout.Button("Search", GUILayout.Width(150), GUILayout.Height(20)))
            {
                if (findObj == null)
                {
                    return;
                }
                var deps = ResCheckJenkinsEntrance.GetDependencies(ResCheckJenkinsEntrance.GetAssetPath(findObj), true);
                findResult.Clear();
                foreach (string item in deps)
                {
                    findResult.Add(ResCheckJenkinsEntrance.LoadAssetAtPath(item));
                }
            }
            ShowFindResult();
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

