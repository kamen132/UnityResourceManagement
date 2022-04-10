using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace GMResChecker
{
    public class CheckShaderPos : ResCheckBaseSubWindowEditor
    {
        private static List<UnityEngine.Object> findResult = new List<UnityEngine.Object>();
        public override void OnGUIDraw()
        {
            EditorGUILayout.LabelField("--------------------------------------------------------------------------------");
            EditorGUILayout.LabelField("shader Path: Assets/WorkAssets/shader");
            if (GUILayout.Button("检查shader不在正确的路径", GUILayout.Width(400), GUILayout.Height(20)))
            {
                CheckOutShader();
            }
            ShowFindResult();
        }
        public static void CheckOutShader()
        {
            findResult.Clear();
            String BasePath = Application.dataPath;
            string[] shaderNames = Directory.GetFiles(BasePath, "*.shader", SearchOption.AllDirectories);
            for (int i = 0; i < shaderNames.Length; i++)
            {
                string itemPath = ResCheckEditorUtil.GetAssetPath(shaderNames[i]);
                if (itemPath.Contains("Assets/WorkAssets/shader") == false &&
                    itemPath.Contains("Assets/Common/Resources/shader") == false &&
                    itemPath.Contains("Assets/WorkAssets/TMPFonts") == false &&
                     itemPath.Contains("Assets/Common/Thirdparty") == false
                    )
                {
                    //Debug.Log("itemPath===" + itemPath);
                    Shader go = ResCheckJenkinsEntrance.LoadAssetAtPath(itemPath) as Shader;
                    if (go)
                    {
                        findResult.Add(go);
                    }
                }
            }
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
                UnityEngine.Object item = findResult[i];
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