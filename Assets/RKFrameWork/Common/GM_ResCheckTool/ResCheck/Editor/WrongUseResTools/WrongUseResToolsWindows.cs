using System.Collections.Generic;
using UnityEngine;

namespace GMResChecker
{
    public class WrongUseResToolsWindows : ResCheckBaseWindowEditor
    {
        private string[] titles;

        private List<ResCheckBaseSubWindowEditor> windows = new List<ResCheckBaseSubWindowEditor>();


        public void Init()
        {
            List<string> titlesList = new List<string>();
            titlesList.Add("FindUseOutWorkAssetsRes");
            windows.Add(CreateInstance<FindUseOutWorkAssetsResources>());

            titlesList.Add("FIndMissingMesh");
            windows.Add(CreateInstance<FIndMissingMesh>());
            
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