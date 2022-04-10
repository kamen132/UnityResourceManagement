
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public partial class CommonUtils
{
    public static long GetTimeStamp()
    {
        return GetTimeStamp(true);
    }
    //不受单帧影响，获得精准性能的时间
    public static int GetTickCount()
    {
        return Environment.TickCount;
    }

    public static void QuitGame()
    {
        System.Diagnostics.Process.GetCurrentProcess().Kill();
    }

    public static AssetBundleManifest LoadManifest(AssetBundle ab)
    {
        return ab.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
    }

    public static Sprite CreateSprite(Texture2D tex, float pivotX = 0.5f, float pivotY = 0.5f)
    {
        return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(pivotX, pivotY));
    }

    public static RaycastHit MousePhysicsHit(Camera camrea)
    {
        Vector3 mousePosition = Input.mousePosition;
        var ray = camrea.ScreenPointToRay(mousePosition);
        RaycastHit hit = new RaycastHit();
        hit.distance = -1;
        if (Physics.Raycast(ray, out hit))
        {
            return hit;
        }
        return hit;
    }
    public static Vector2 ScreenPosToUGUIPosition(RectTransform trans, Camera uiCamera)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(trans, Input.mousePosition, uiCamera, out localPoint);
        return localPoint;
    }

    public static Vector2 ScreenPosToUGUIPosition(Canvas parentCanvas, Vector2 screenPoint, Camera cam = null)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentCanvas.transform as RectTransform, screenPoint, parentCanvas.worldCamera, out localPoint);
        return localPoint;
    }

    //ScreenPointToLocalPointInRectangle
    public static Vector2 WorldToUGUIPosition(Canvas parentCanvas, Camera worldcamera, Vector3 worldPosition)
    {
        Vector3 screenPos = worldcamera.WorldToScreenPoint(worldPosition);
        Vector2 movePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentCanvas.transform as RectTransform, screenPos, parentCanvas.worldCamera, out movePos);
        return movePos;
    }

    public static RenderTexture CreateRenderTexture(int width, int height, int depth, bool useMipMap)
    {
        RenderTexture data = new RenderTexture(width, height, depth);
        data.useMipMap = useMipMap;
        return data;
    }

    public static RenderTexture CreateDyanmicAtlasRenderTexture(int width, int height, int depth, RenderTextureFormat format)
    {
        RenderTexture data = new RenderTexture(width, height, depth, format);
        data.autoGenerateMips = false;
        data.useMipMap = false;
        data.filterMode = FilterMode.Bilinear;
        data.wrapMode = TextureWrapMode.Clamp;
        data.Create();
        return data;
    }
    public static void SetQualitySettingsLevel(int level)
    {
        string[] names = QualitySettings.names;
        if (names != null && names.Length > level)
        {
            QualitySettings.SetQualityLevel(level);
        }
    }

    public static void SetPixelDragThreshold(int value)
    {
        UnityEngine.EventSystems.EventSystem.current.pixelDragThreshold = Mathf.CeilToInt(Screen.dpi / value);
    }

    public static void UpdatePixelDragThreshold()
    {
        UnityEngine.EventSystems.EventSystem.current.pixelDragThreshold = Mathf.CeilToInt(Screen.dpi / 40);
    }
    public static long GetTimeStamp(bool isSeconds)
    {
        TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
        long ret;
        if (isSeconds)
            ret = Convert.ToInt64(ts.TotalSeconds);
        else
            ret = Convert.ToInt64(ts.TotalMilliseconds);
        return ret;
    }

    public static Uri GetSystemUri(string luaUrl)
    {
        Uri _uri = new Uri(luaUrl);
        return _uri;
    }
    public static Texture2D ScaleTexture(Texture2D source, int targetWidth, int targetHeight)
    {
        Texture2D result = new Texture2D(targetWidth, targetHeight, source.format, false);
        for (int i = 0; i < result.height; ++i)
        {
            for (int j = 0; j < result.width; ++j)
            {
                Color newColor = source.GetPixelBilinear((float)j / (float)result.width, (float)i / (float)result.height);
                result.SetPixel(j, i, newColor);
            }
        }
        result.Apply();
        return result;
    }

    public static void UnityEvent_RemoveAllListeners(UnityEngine.Events.UnityEventBase evt)
    {
        evt.RemoveAllListeners();
    }

    //替换字符串，lua的gsub有一些特殊字符无法进行替换，调用c#的string.replace来替换
    public static string ReplaceStr(string str, string targetStr, string replaceStr)
    {
        if (str != null)
        {
            return str.Replace(targetStr, replaceStr);
        }
        else
            return null;
    }

    public static void ReplaceShaders(Object asset)
    {
        if (asset is GameObject)
        {
            var allRenderers = ((GameObject)asset).GetComponentsInChildren<Renderer>(true);
            for (var i = 0; i < allRenderers.Length; i++)
            {
                var mats = allRenderers[i].sharedMaterials;
                for (var j = 0; j < mats.Length; j++)
                {
                    ReplaceShaders(mats[j]);
                }
            }

            var allParticleSystemRenderers = ((GameObject)asset).GetComponentsInChildren<ParticleSystemRenderer>(true);
            for (var i = 0; i < allParticleSystemRenderers.Length; i++)
            {
                var mat = allParticleSystemRenderers[i].sharedMaterial;
                ReplaceShaders(mat);

                var trailmat = allParticleSystemRenderers[i].trailMaterial;
                ReplaceShaders(trailmat);
            }

            var allTrailRenderers = ((GameObject)asset).GetComponentsInChildren<TrailRenderer>(true);
            for (var i = 0; i < allTrailRenderers.Length; i++)
            {
                var mats = allTrailRenderers[i].sharedMaterials;
                for (var j = 0; j < mats.Length; j++)
                {
                    ReplaceShaders(mats[j]);
                }
            }

            var images = ((GameObject)asset).GetComponentsInChildren<Image>(true);
            for (var i = 0; i < images.Length; i++)
            {
                var mat = images[i].material;
                ReplaceShaders(mat);
            }

            var rawimages = ((GameObject)asset).GetComponentsInChildren<RawImage>(true);
            for (var i = 0; i < rawimages.Length; i++)
            {
                var mat = rawimages[i].material;
                ReplaceShaders(mat);
            }

            var texts = ((GameObject)asset).GetComponentsInChildren<Text>(true);
            for (var i = 0; i < texts.Length; i++)
            {
                var mat = texts[i].material;
                ReplaceShaders(mat);
            }
            var textsUGUI = ((GameObject)asset).GetComponentsInChildren<TMPro.TextMeshProUGUI>(true);
            for (var i = 0; i < textsUGUI.Length; i++)
            {
                var mat = textsUGUI[i].materialForRendering;
                ReplaceShaders(mat);
            }
        }
        else if (asset is Material)
        {
            ((Material)asset).shader = Shader.Find(((Material)asset).shader.name);
        }
        else if (asset is Shader)
        {
            asset = Shader.Find(asset.name);
        }
    }

    public static void CsSharpGc()
    {
        System.GC.Collect();
        System.GC.WaitForPendingFinalizers();
    }

    public static void CheckFileMD5(string filePath, string md5, OnCallBackBool callBack)
    {
        AsyncTask asyncTask = new AsyncTask();
        asyncTask._OnRunInThread = (AsyncTask _asyncTask, List<object> args) =>
        {
            string tempMd5 = FileUtils.GetMD5HashFromFile(filePath);
            if (md5.Equals(tempMd5))
            {
                _asyncTask.SetResult(1);
            }
            else
            {
                _asyncTask.SetResult(2);
            }
            
        };
        asyncTask._OnCallBackInMainThread = (AsyncTask _asyncTask, AsyncTaskResult asyncTaskResult, List<object> args) =>
        {
            int result = _asyncTask.Result;
            callBack(result == 1);
        };
        asyncTask.execute(new List<object>());

        
    }
}