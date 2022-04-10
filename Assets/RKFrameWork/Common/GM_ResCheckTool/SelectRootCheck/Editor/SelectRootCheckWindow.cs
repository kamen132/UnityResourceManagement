using UnityEditor;
using UnityEngine;

public class SelectRootCheckWindow: EditorWindow
{
    private GameObject selectGameObject = null;
    private int _totleCount = 0;
    [MenuItem("CheckWindow/SelectRootCheckWindow", false)]
    public static void OpenSelectRootChildCount()
    {
        SelectRootCheckWindow window = (SelectRootCheckWindow)GetWindow(typeof(SelectRootCheckWindow));  //定义一个窗口对象  
        float width = 500;
        float height = 500;
        float posX = 500f;
        float posY = 300f;
        window.position = new Rect(posX, posY, width, height);
    }
    void OnGUI()
    {
        GUILayout.Space(10);
        EditorGUILayout.LabelField("###################### SelectRootChildCount #########################  ");
        GUILayout.Space(10);
        EditorGUILayout.LabelField("please select Root:");
        GUILayout.Space(10);
        EditorGUILayout.BeginHorizontal();
        

        GameObject[] gameObjects = Selection.gameObjects;
        if (gameObjects.Length > 0)
        {
            GameObject one = gameObjects[0];
            if (selectGameObject != one)
            {
                selectGameObject = one;
                _totleCount = GetChildCount(one.transform);
            }
            EditorGUILayout.LabelField("select Root:" + selectGameObject.name + "  " + _totleCount);
        }
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(10);
    }

    int GetChildCount(Transform root)
    {
        int totleCount = 1;

        for (int i = 0; i < root.childCount; ++i)
        {
            Transform child = root.GetChild(i);
            OnGetChildCount(child, ref totleCount);
        }
        return totleCount;
    }
    void OnGetChildCount(Transform root, ref int totleCount)
    {
        totleCount++;
        if (root.childCount > 0)
        {
            for (int i = 0; i < root.childCount; ++i)
            {
                Transform child = root.GetChild(i);
                totleCount++;
                OnGetChildCount(child, ref totleCount);
            }
        }
    }
}
