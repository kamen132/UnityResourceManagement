using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace GMResChecker
{
    public class FindBigTexture : ResCheckBaseSubWindowEditor
    {
        private enum SelectState
        {
            FindBigTexture = 1,
            FindPrefabData,
            FindMeshsData,
            FindEmptyMesh
        }
    
        private static SelectState _SelectState = SelectState.FindBigTexture;

        private static Dictionary<Texture2D, float> _findBigTextureData = new Dictionary<Texture2D, float>();

        private static bool isInit = false;
        public override void OnGUIDraw()
        {
            EditorGUILayout.LabelField("--------------------------------------------------------------------------------");
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("检查大贴图", GUILayout.Width(200), GUILayout.Height(20)))
            {
                _SelectState = SelectState.FindBigTexture;
                FindData();
            }
            
            if (GUILayout.Button("清除缓存", GUILayout.Width(100), GUILayout.Height(20)))
            {
                _SelectState = SelectState.FindBigTexture;
                _findBigTextureData.Clear();
                isInit = false;
                ResCheckJenkinsEntrance.Clear();
            }

            EditorGUILayout.EndHorizontal();
            ShowFindResult();
        }

        private static void FindData()
        {
            if (isInit)
            {
                return;
            }
            isInit = true;

            string directoryPath = Application.dataPath + "/WorkAssets";
            string[]  allFiles = FileManager.GetAllFilesInFolder(directoryPath);
            foreach (string file in allFiles)
            {
                if (ResCheckEditorUtil.IsTexture2D(file))
                {
                    Texture2D texture = ResCheckJenkinsEntrance.LoadAssetAtPath(PathUtil.GetAssetPath(file)) as Texture2D;
                    if (texture == null)
                    {
                        Debug.Log("texture nul: " + file);
                        continue;
                    }
                    float value = ResCheckEditorUtil.CalTextureSize(texture);
                    _findBigTextureData[texture] = value;
                }
            }
            _findBigTextureData = _findBigTextureData.OrderByDescending(o => o.Value).ToDictionary(p => p.Key, o => o.Value);
        }
        
        private static Vector3 scrollPos = Vector3.zero;
        private static void ShowFindBigTextrueResult()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, true, GUILayout.Width(850), GUILayout.Height(600));
            EditorGUILayout.LabelField("---------------------------检查所有大贴图-----------------------------------------------------");
            GUILayout.Space(5);
            EditorGUILayout.BeginVertical();
            foreach (var item in _findBigTextureData)
            {
                Texture2D tex = item.Key;
                float size = item.Value;
                if(size < 512 * 1024)
                {
                    continue;
                }
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.ObjectField(tex, tex.GetType(), true, GUILayout.Width(400));
                EditorGUILayout.LabelField(ResCheckEditorUtil.HumanReadableFilesize(size), GUILayout.Width(80));
                EditorGUILayout.LabelField(tex.format.ToString(), GUILayout.Width(120));
                EditorGUILayout.LabelField(tex.width + " X " + tex.height, GUILayout.Width(120));
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }
       
        private static Vector3 scrollPos2 = Vector3.zero;
       
       
      
        private static void ShowFindResult()
        {
            //dirs.Add(ResCheckJenkinsEntrance.LoadAssetAtPath(defaulutCheckPaths[i], typeof(Object)));
            switch (_SelectState)
            {
                case SelectState.FindBigTexture:
                    ShowFindBigTextrueResult();
                    break;
                case SelectState.FindPrefabData:

                    break;
                case SelectState.FindMeshsData:

                    break;
                case SelectState.FindEmptyMesh:

                    break;
            }
        }
    }
}
