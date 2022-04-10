using System.Collections.Generic;
using UnityEngine;

namespace GMResChecker
{
    public class ShaderToolsWindows : ResCheckBaseWindowEditor
    {
        private string[] titles;

        private List<ResCheckBaseSubWindowEditor> windows = new List<ResCheckBaseSubWindowEditor>();

        public void Init()
        {
            List<string> titlesList = new List<string>();
            titlesList.Add("Find Build In");
            windows.Add(CreateInstance<FindBuildinResources>());
            titlesList.Add("Check Shader for incorrect path");
            windows.Add(CreateInstance<CheckShaderPos>());
            titlesList.Add("FindShaderAnit");
            windows.Add(CreateInstance<FindShaderAnit>());

            titlesList.Add("FindShaderFallBack");
            windows.Add(CreateInstance<FindShaderFallBack>());

            titlesList.Add("FindUnuseShaderKeywords");
            windows.Add(CreateInstance<FindUnuseShaderKeywords>());
            //titlesList.Add("检查Shader 重复");
            //windows.Add(CreateInstance<CheckShaderRepeat>());
            //titlesList.Add("替换 Shader");
            //windows.Add(CreateInstance<CheckShaderReplace>());
            //titlesList.Add("查找无用的 Shader");
            //windows.Add(CreateInstance<FindUnUseShader>());

            titles = titlesList.ToArray();
        }
        private int selectIndex = 0;

        public override void OnGUI()
        {
            selectIndex = GUILayout.Toolbar(selectIndex, titles, GUILayout.Height(30));
            ResCheckBaseSubWindowEditor item = windows[selectIndex];
            item.OnGUIDraw();
        }
    }
}