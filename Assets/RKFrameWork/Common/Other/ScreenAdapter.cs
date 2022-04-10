
using UnityEngine;

public class ScreenAdapter : MonoBehaviour
{
    GUIStyle style = null;
    private float topHeight = 60;
    private float bottomHeight = 50;
    Texture2D tex2D;
    bool AdapterlandScapeLeft;
    bool AdapterlandScapeRight;
    bool AdapterPortrait;
    private void Awake()
    {
        style = new GUIStyle();
        tex2D = new Texture2D(5, 5);
        Color32[] tmpColor = new Color32[5 * 5];
        for (int k = 0; k < tmpColor.Length; ++k)
        {
            tmpColor[k] = Color.yellow;
        }

        tex2D.SetPixels32(0, 0, 5, 5, tmpColor);
        style.normal.background = tex2D;
    }
    public void SetData(bool AdapterlandScapeLeft, bool AdapterlandScapeRight, bool AdapterPortrait)
    {
        this.AdapterlandScapeLeft = AdapterlandScapeLeft;
        this.AdapterlandScapeRight = AdapterlandScapeRight;
        this.AdapterPortrait = AdapterPortrait;
    }

    void OnGUI()
    {
        if (AdapterPortrait)
        {
            float width = 324;                 //顶部宽
            float posX = Screen.width * 0.5f - width * 0.5f;
            GUI.Box(new Rect(posX, 0, width, topHeight), "", style);
            GUI.Box(new Rect(posX, Screen.height - bottomHeight, width, bottomHeight), "", style);
        }else if(AdapterlandScapeLeft)
        {
            float height = 324;
            float posY = Screen.height * 0.5f - height * 0.5f;
            GUI.Box(new Rect(0, posY, topHeight, height), "", style);
            GUI.Box(new Rect(Screen.width - bottomHeight, posY, bottomHeight, height), "", style);
        }
        else if (AdapterlandScapeRight)
        {
            float height = 324;
            float posY = Screen.height * 0.5f - height * 0.5f;
            GUI.Box(new Rect(0, posY, bottomHeight, height), "", style);
            GUI.Box(new Rect(Screen.width - topHeight, posY, topHeight, height), "", style);
        }
    }
}