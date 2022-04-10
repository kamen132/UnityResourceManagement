using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace GMResChecker
{
    public class FindSpecialFormat : ResCheckBaseSubWindowEditor
    {
        private enum SelectState
        {
            FindBigTexture = 1,
            FindPrefabData
        }
        private List<String> foremats = new List<string>() {
        "cubemap"

    };
        private string BasePath = null;
        //private static SelectState _SelectState = SelectState.FindBigTexture;
        //private static Dictionary<Texture2D, int> _findBigTextureData = new Dictionary<Texture2D, int>();
        //private static Dictionary<GameObject, int> _findPrefabData = new Dictionary<GameObject, int>();
        //private static Dictionary<GameObject, List<FindPrefabDataAnotherData>> _findPrefabDataAnother = new Dictionary<GameObject, List<FindPrefabDataAnotherData>>();
        //private static GameObject selectObj;
        public override void OnGUIDraw()
        {
            EditorGUILayout.LabelField("--------------------------------------------------------------------------------");
            EditorGUILayout.BeginHorizontal();
            for (int i = 0; i < foremats.Count; i++)
            {
                string foremat = foremats[i];
                if (GUILayout.Button(foremat, GUILayout.Width(200), GUILayout.Height(20)))
                {
                    FindFormateData(foremat);
                }
            }
            //if (GUILayout.Button("检查场景大贴图", GUILayout.Width(400), GUILayout.Height(20)))
            //{
            //    _SelectState = SelectState.FindBigTexture;
            //    _findPrefabData.Clear();
            //    _findBigTextureData.Clear();
            //    FindBigTextureData();
            //}

            //if (GUILayout.Button("检查场景贴图总大小", GUILayout.Width(400), GUILayout.Height(20)))
            //{
            //    _SelectState = SelectState.FindPrefabData;
            //    _findPrefabData.Clear();
            //    _findBigTextureData.Clear();
            //    FindPrefabData();
            //}

            EditorGUILayout.EndHorizontal();

        }
        void FindFormateData(string formate)
        {
            BasePath = Application.dataPath;
            string endSuffix = "*." + formate;

            string[] sceneNames = Directory.GetFiles(BasePath, endSuffix, SearchOption.AllDirectories);
            for (int nameIndex = 0; nameIndex < sceneNames.Length; nameIndex++)
            {
                string assetPath = sceneNames[nameIndex];
                assetPath = ResCheckEditorUtil.FormatPath(assetPath);
                assetPath = ResCheckEditorUtil.GetAssetPath(assetPath);
                EditorUtility.DisplayProgressBar("加载中", Path.GetFileName(assetPath), (nameIndex / sceneNames.Length));
                Cubemap cubemap = ResCheckJenkinsEntrance.LoadAssetAtPath(assetPath) as Cubemap;
            }
        }
    }
}
