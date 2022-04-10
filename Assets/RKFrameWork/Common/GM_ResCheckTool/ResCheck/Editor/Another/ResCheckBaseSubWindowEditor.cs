
using UnityEditor;
using UnityEngine;
namespace GMResChecker
{
    public abstract class ResCheckBaseSubWindowEditor : EditorWindow
    {
        public string m_winName = "windName";
        public abstract void OnGUIDraw();
    }
}
