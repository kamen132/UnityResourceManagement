using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace GMResChecker
{
    public class FindBackUpRes : ResCheckBaseSubWindowEditor
    {
        static Dictionary<string, string> findResults = new Dictionary<string, string>();
        public override void OnGUIDraw()
        {
            EditorGUILayout.LabelField("--------------------------------------------------------------------------------");
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("查找 backup 资源", GUILayout.Width(400), GUILayout.Height(20)))
            {

                OnFindBackUpRes();
                EditorUtility.ClearProgressBar();
            }

            EditorGUILayout.EndHorizontal();
            ShowFindResult();
        }
        void OnFindBackUpRes()
        {
            string BackUpPath = Application.dataPath + "/../BackUp/";
            string[] names = Directory.GetFiles(BackUpPath, "*.*", SearchOption.AllDirectories);
            foreach (var item in names)
            {
                if (item.EndsWith(".meta") == false)
                {
                    string key = Path.GetFileNameWithoutExtension(item);
                    findResults[key] = item;
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
            string removeKey = null;
            foreach (var item in findResults)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(item.Key);
                //EditorGUILayout.ObjectField(item。ke, typeof(UnityEngine.Object), true, GUILayout.Width(400));
                if (GUILayout.Button("还原", GUILayout.Width(200), GUILayout.Height(20)))
                {
                    removeKey = item.Key;
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }

        static void MoveToBackUp(string removeKey)
        {

            //string path =findResults[removeKey];
            //string backUpPath = ResCheckEditorUtil.GetBackUpPath(path);
            //string toPath = backUpPath + path;
            //ResCheckEditorUtil.MoveFile(fromPath, toPath);

            //string fromPathMeta = fromPath + ".meta";
            //string toPathMeta = toPath + ".meta";
            //ResCheckEditorUtil.MoveFile(fromPathMeta, toPathMeta);
            //findResults.Remove(removeKey);
        }
    }
}
