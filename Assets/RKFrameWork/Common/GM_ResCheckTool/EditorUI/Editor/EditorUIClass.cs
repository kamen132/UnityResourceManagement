
using UnityEditor;
using UnityEngine;

namespace Majic.CM
{
    public class EditorUIClass
    {
        public const string ADD_BUTTON_STYLE = "AddButtonStyle";
        public const string BUTTON_OFF_STYLE = "ButtonOffStyle";
        public const string BUTTON_SELECT_STYLE = "ButtonSelectedStyle";
        public const string THIRD_LEVEL_TITLE_STYLE = "ThirdLevelTitleStyle";
        
        public const string LINE_STYLE = "LineStyle";

        public const string AREA1_STYLE = "Area1";
        
        private static GUISkin SKIN_DATA;
        public static string SKIN_PATH = "Assets/Common/GM_ResCheckTool/EditorUI/Skin/EditorSkin.guiskin";


        public static GUIStyle GetStyle(string styleName)
        {
            if(SKIN_DATA == null)
            {
                SKIN_DATA = AssetDatabase.LoadAssetAtPath<GUISkin>(SKIN_PATH);
            }
            return SKIN_DATA.GetStyle(styleName);
        }
        public static void DrawLine()
        {
            GUILayout.Space(7);
            GUILayout.Label("", GetStyle(LINE_STYLE), GUILayout.ExpandWidth(true), GUILayout.Height(1));
            GUILayout.Space(7);
        }
        public static void Clear()
        {
            SKIN_DATA = null;
        }
    }
}
