
using UnityEditor;
namespace Majic.CM
{
    public abstract class CMBaseWindowEditor : EditorWindow
    {
        public string winName = "windName";
        public abstract void OnGUI();
        public abstract void OnInit();
        public abstract void OnUpdate();

        public abstract void Clear();

    }
}
