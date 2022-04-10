using System.Collections.Generic;
using UnityEngine;

namespace GMResChecker
{
    public class RoleAndSceneToolsWindows : ResCheckBaseWindowEditor
    {
        private string[] titles;

        private List<ResCheckBaseSubWindowEditor> windows = new List<ResCheckBaseSubWindowEditor>();


        public void Init()
        {
            List<string> titlesList = new List<string>();
            titlesList.Add("GetRoleInfo");
            windows.Add(CreateInstance<GetRoleInfo>());
            titlesList.Add("AboutScene");
            windows.Add(CreateInstance<AboutScene>());
            titlesList.Add("FindSceneCrossReference");
            windows.Add(CreateInstance<FindSceneCrossReference>());
            titlesList.Add("FindRoleCrossReference");
            windows.Add(CreateInstance<FindRoleCrossReference>());
            
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