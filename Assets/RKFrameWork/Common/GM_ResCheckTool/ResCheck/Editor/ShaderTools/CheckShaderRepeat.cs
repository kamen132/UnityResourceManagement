using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace GMResChecker
{
    public class CheckShaderRepeat : ResCheckBaseSubWindowEditor
    {
        private static Dictionary<string, UnityEngine.Object> result = new Dictionary<string, UnityEngine.Object>();
        private static Dictionary<string, string> names = new Dictionary<string, string>();
        public override void OnGUIDraw()
        {
            EditorGUILayout.LabelField("--------------------------------------------------------------------------------");
            if (GUILayout.Button("检查shader 重复", GUILayout.Width(400), GUILayout.Height(20)))
            {
                CheckOutShader();
            }
            ShowFindResult();
        }
        public static void CheckOutShader()
        {
            result.Clear();
            names.Clear();
            String BasePath = Application.dataPath;
            string[] shaderNames = Directory.GetFiles(BasePath, "*.shader", SearchOption.AllDirectories);
            for (int i = 0; i < shaderNames.Length; i++)
            {
                string name = shaderNames[i];
                string fileName = Path.GetFileNameWithoutExtension(name);
                string itemPath = ResCheckEditorUtil.GetAssetPath(name);
                Shader shader = ResCheckJenkinsEntrance.LoadAssetAtPath(itemPath) as Shader;
                if (names.ContainsKey(shader.name) == false)
                {
                    names.Add(shader.name, itemPath);
                }
                else
                {
                    Shader oldShader = ResCheckJenkinsEntrance.LoadAssetAtPath(names[shader.name]) as Shader;

                    result[oldShader.name] = oldShader;
                    result[shader.name + "_new"] = shader;
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
            foreach (var item in result)
            {
                EditorGUILayout.BeginHorizontal();
                UnityEngine.Object value = item.Value;
                EditorGUILayout.LabelField(ResCheckJenkinsEntrance.GetAssetPath(value), GUILayout.Width(400));
                EditorGUILayout.ObjectField(value, value.GetType(), true, GUILayout.Width(600));
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
            GUILayout.Space(5);
            EditorGUILayout.LabelField("--------------------------------------------------------------------------------");
            EditorGUILayout.EndScrollView();
        }
    }
}
