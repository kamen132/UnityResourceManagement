
using UnityEditor;
using UnityEngine;
namespace GMResChecker
{
    public abstract class ResCheckBaseWindowEditor : EditorWindow
    {
        public string m_winName = "windName";
        public abstract void OnGUI();
    }
}
