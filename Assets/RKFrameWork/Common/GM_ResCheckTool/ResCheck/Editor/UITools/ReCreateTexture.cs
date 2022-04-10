using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace GMResChecker
{
    public class ReCreateTexture : ResCheckBaseSubWindowEditor
    {
        private static List<Texture> errorMipMapTextureList = new List<Texture>();
        private static List<Texture> errorTextureList = new List<Texture>();
        private string[] titles = new string[] { "1", "2", "3" };
        private int selectIndex = 0;
        private string selectPath = "";
        public override void OnGUIDraw()
        {
            EditorGUILayout.LabelField("--------------------------------------------------------------------------------");

            selectIndex = EditorGUILayout.Popup(selectIndex, titles, GUILayout.Width(150));
            GUILayout.Space(10);
            EditorGUILayout.TextField("路径：", selectPath);
            if (GUILayout.Button("选择图片路径", GUILayout.Width(400), GUILayout.Height(20)))
            {
                DirectoryInfo __dir = new DirectoryInfo(Application.dataPath);
                string _path = __dir.Parent.Parent.FullName + "/res";
                if (string.IsNullOrEmpty(selectPath) == false)
                {
                    _path = selectPath;
                }
                string __selectedPath = EditorUtility.OpenFolderPanel("请选择文件夹", _path, "");
                if (string.IsNullOrEmpty(__selectedPath))
                {
                    return;
                }
                selectPath = __selectedPath;
            }
            EditorGUILayout.LabelField("--------------------------------------------------------------------------------");
            if (GUILayout.Button("开始生成", GUILayout.Width(400), GUILayout.Height(20)))
            {
                Debug.Log("selectPath===" + selectPath);
                DoCreate();
                AssetDatabase.Refresh();
                Debug.Log("ReCreate success===");
            }
            EditorGUILayout.LabelField("--------------------------------------------------------------------------------");
            //EditorGUILayout.EndHorizontal();

        }
        void DoCreate()
        {
            if(string.IsNullOrEmpty(selectPath))
            {
                Debug.Log("selectPath IsNullOrEmpty");
                return;
            }
            int pix = int.Parse(titles[selectIndex]);
            string[] allFiles = FileManager.GetAllFilesInFolder(selectPath);
           

            foreach (string file in allFiles)
            {
                if(file.EndsWith(".png"))
                {
                    //string assetPath = PathUtil.GetAssetPath(file);
                    byte[] data = (byte[])FileManager.ReadAllBytes(file).data;
                    Texture2D texture = new Texture2D(0, 0, TextureFormat.ARGB32, false);
                    texture.LoadImage(data);
                    Texture2D textureTarget = new Texture2D(texture.width + pix * 2, texture.height + pix * 2, TextureFormat.ARGB32, false);
                    Color32[] tmpColor = new Color32[textureTarget.width *  textureTarget.height];
                    for (int k = 0; k < tmpColor.Length; ++k)
                    {
                        tmpColor[k] = Color.clear;
                    }
                    textureTarget.SetPixels32(0, 0, textureTarget.width, textureTarget.height, tmpColor);
                    Graphics.CopyTexture(texture, 0, 0, 0, 0, texture.width, texture.height, textureTarget, 0, 0, pix, pix);
                    FileManager.WriteAllBytes(file, textureTarget.EncodeToPNG());
                }
            }
        }
       
    }
}
