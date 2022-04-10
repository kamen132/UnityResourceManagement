using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace GMResChecker
{
    public class ResCheckToolWindows : EditorWindow
    {
        private string[] titles;
        private static ResCheckToolWindows _ToolsWindows;
        private List<ResCheckBaseWindowEditor> windows = new List<ResCheckBaseWindowEditor>();

        [@MenuItem("PMTools/ClearAllRedundant")]
        public static void ClearAllRedundant()
        {
            FindUnuseShaderKeywords.AutoClearSceneData();
            FindRedundantResources.AutoClearSceneData();
        }
        //[@MenuItem("CheckWindow/JenkinsCheckByGit")]
        public static void JenkinsCheckByGit()
        {
            Debug.Log("------------------JenkinsCheckByGit--------------------");
            string[] args = System.Environment.GetCommandLineArgs();
            int count = args.Length;
            StringBuilder builder = new StringBuilder();
            string result = FindBuildinResources.GetBuildInResources();
            builder.Append(result);
           
            Debug.Log("xxx GetETC2FormatResources:" + result);
            result = FindUseOutWorkAssetsResources.GetUseOutWorkAssetsResources();
            builder.Append(result);
            
            Debug.Log("xxx GetFindMachinesCrossReference:" + result);
            result = FindRedundantResources.GetRedundantResources();
            builder.Append(result);
            Debug.Log("xxx GetRedundantResources:" + result);
            result = FindUseUISpineRes.GetUseUISpineRes();
            builder.Append(result);
            Debug.Log("xxx GetUseUISpineRes:" + result);
            result = ErrorRawImage.GetErrorRawImageResources();
            builder.Append(result);


            string SavePath = Application.dataPath + "/../../../../Share";
            FileManager.CreateDirectory(SavePath);
            FileManager.WriteAllText(SavePath + "/CheckRes.txt", builder.ToString());
            Debug.Log("------------------JenkinsCheckByGit-----over---------------");
        }

        [MenuItem("CheckWindow/ResCheckTool")]
        public static void OpenShaderWindow()
        {
            if (_ToolsWindows == null)
            {
                _ToolsWindows = GetWindow<ResCheckToolWindows>();
            }
            _ToolsWindows.minSize = new Vector2(1000, 500);
            _ToolsWindows.Show();
            _ToolsWindows.Init();
            _ToolsWindows.titleContent.text = "ResCheckTool";
        }

        private void Init()
        {
            List<string> titlesList = new List<string>();
            titlesList.Add("About Res");
            ResToolsWindows _ResToolsWindows = CreateInstance<ResToolsWindows>();
            _ResToolsWindows.Init();
            windows.Add(_ResToolsWindows);

            titlesList.Add("About WrongUseRes");
            WrongUseResToolsWindows _WrongUseResToolsWindows = CreateInstance<WrongUseResToolsWindows>();
            _WrongUseResToolsWindows.Init();
            windows.Add(_WrongUseResToolsWindows);
            

            titlesList.Add("About Shader");
            ShaderToolsWindows _ShaderToolsWindows = CreateInstance<ShaderToolsWindows>();
            _ShaderToolsWindows.Init();
            windows.Add(_ShaderToolsWindows);

            titlesList.Add("Roles And Scene");
            RoleAndSceneToolsWindows _RoleToolsWindows = CreateInstance<RoleAndSceneToolsWindows>();
            _RoleToolsWindows.Init();
            windows.Add(_RoleToolsWindows);
            titlesList.Add("About UI");
            UIToolsWindows _UIToolsWindows = CreateInstance<UIToolsWindows>();
            _UIToolsWindows.Init();
            windows.Add(_UIToolsWindows);

            titlesList.Add("Others");
            OtherToolsWindows _OtherToolsWindows = CreateInstance<OtherToolsWindows>();
            _OtherToolsWindows.Init();
            windows.Add(_OtherToolsWindows);

            titlesList.Add("Range");
            RangeToolsWindows _RangeToolsWindows = CreateInstance<RangeToolsWindows>();
            _RangeToolsWindows.Init();
            windows.Add(_RangeToolsWindows);
            titles = titlesList.ToArray();
        }

        private int selectIndex = 0;
        private Vector3 scrollPos = Vector3.zero;

        private void OnGUI()
        {
            GUILayout.Space(10);
            EditorGUILayout.BeginHorizontal();
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, true, GUILayout.Width(200), GUILayout.Height(600));
            int count = titles.Length;

            for(int index = 0; index < count;index ++)
            {
                if(selectIndex == index)
                {
                    GUILayout.Space(5);
                    if (GUILayout.Button(titles[index], GUILayout.Height(40)))
                    {
                        selectIndex = index;
                    }
                    GUILayout.Space(5);
                }
                else
                {
                    if (GUILayout.Button(titles[index]))
                    {
                        selectIndex = index;
                    }
                }
               
            }
            EditorGUILayout.EndScrollView();

            GUILayout.Space(15);
            ResCheckBaseWindowEditor item = windows[selectIndex];
            EditorGUILayout.BeginVertical();
            GUILayout.Label(titles[selectIndex], GUI.skin.button, GUILayout.Height(40));
            GUILayout.Space(5);
            item.OnGUI();
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }
    }
}