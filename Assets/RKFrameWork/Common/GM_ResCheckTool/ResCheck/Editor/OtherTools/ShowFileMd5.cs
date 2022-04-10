using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace GMResChecker
{
    public class ShowFileMd5 : ResCheckBaseSubWindowEditor
    {
       
        private string BasePath = null;
       
        public override void OnGUIDraw()
        {
            EditorGUILayout.LabelField("-----------------------请选择要查询MD5的文件---------------------------------------------------------");
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("选择文件", GUILayout.Width(200), GUILayout.Height(20)))
            {

                var path = Application.persistentDataPath;
                var selectedPath = EditorUtility.OpenFilePanel("请选择要查询MD5的文件", path, "");
                if (string.IsNullOrEmpty(selectedPath))
                {
                    return;
                }
                Debug.Log("selectedPath===" + selectedPath);
                //string md5 = FileUtils.GetMD5HashFromFile(selectedPath);
                //Debug.Log("md5:  " + md5);
            }
            EditorGUILayout.EndHorizontal();

        }
        
    }
}
