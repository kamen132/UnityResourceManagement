using System.Collections.Generic;
using UnityEngine;

namespace GMResChecker
{
    public class OtherToolsWindows : ResCheckBaseWindowEditor
    {
        private string[] titles;

        private List<ResCheckBaseSubWindowEditor> windows = new List<ResCheckBaseSubWindowEditor>();

        public void Init()
        {
            List<string> titlesList = new List<string>();
            //titlesList.Add("生成 导出 cupemap");
            //windows.Add(CreateInstance<CubeMapTool>());
            titlesList.Add("查找文件依赖");
            windows.Add(CreateInstance<ShowDependencies>());
            titlesList.Add("查找文件的反依赖");
            windows.Add(CreateInstance<ShowAnitDependencies>());
            
            titlesList.Add(" 查找 特定脚本");
            windows.Add(CreateInstance<FindScripts>());
            //titlesList.Add(" 查找 特殊格式文件");
            //windows.Add(CreateInstance<FindSpecialFormat>());
            titlesList.Add("打单独ab包");
            windows.Add(CreateInstance<MakeSingleAB>());
            titlesList.Add("查询MD5");
            windows.Add(CreateInstance<ShowFileMd5>());
            
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