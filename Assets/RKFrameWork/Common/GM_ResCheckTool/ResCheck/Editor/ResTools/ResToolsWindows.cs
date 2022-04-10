using System.Collections.Generic;
using UnityEngine;

namespace GMResChecker
{
    public class ResToolsWindows : ResCheckBaseWindowEditor
    {
        private string[] titles;

        private List<ResCheckBaseSubWindowEditor> windows = new List<ResCheckBaseSubWindowEditor>();


        public void Init()
        {
            List<string> titlesList = new List<string>();
            titlesList.Add("Missing References");
            windows.Add(CreateInstance<FindMissingReference>());
            //titlesList.Add(" About Scene");
            //windows.Add(CreateInstance<FindSceneBigTexture>());
            //titlesList.Add("Find UnUse Res");
            //windows.Add(CreateInstance<FindUnUseRes>());
            //titlesList.Add(" backup资源");
            //windows.Add(CreateInstance<FindBackUpRes>());
            //titlesList.Add("查找无用的 Material");
            //windows.Add(CreateInstance<FindUnUseMaterial>());
            //titlesList.Add("About Camera");
            //windows.Add(CreateInstance<FindForceRTCamera>());
            //titlesList.Add("查找所有的components");
            //windows.Add(CreateInstance<ShowAllComponents>());
            titlesList.Add("Find Redundant Res");
            windows.Add(CreateInstance<FindRedundantResources>());
            titlesList.Add("CompressAnimationRes");
            windows.Add(CreateInstance<CompressAnimationRes>());
            
           
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