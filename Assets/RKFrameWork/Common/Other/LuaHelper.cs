using System;
using System.Reflection;
using UnityEngine;

public static class LuaHelper
{
    #region add pos x or y
    public static void AddLocalPositionX(this GameObject obj, float value)
    {
        obj.SetLocalPositionX(obj.GetLocalPositionX() + value);
    }

    public static void AddLocalPositionX(this Transform trans, float value)
    {
        trans.SetLocalPositionX(trans.GetLocalPositionX() + value);
    }

    public static void AddLocalPositionY(this GameObject obj, float value)
    {
        obj.SetLocalPositionY(obj.GetLocalPositionY() + value);
    }

    public static void AddLocalPositionY(this Transform trans, float value)
    {
        trans.SetLocalPositionY(trans.GetLocalPositionY() + value);
    }

    public static void SetLocalPositionX(this GameObject obj, float value)
    {
        obj.transform.SetLocalPositionX(value);
    }

    public static void SetLocalPositionX(this Transform trans, float value)
    {
        var pos = trans.localPosition;
        pos.x = value;
        trans.localPosition = pos;
    }

    public static void SetLocalPositionY(this GameObject obj, float value)
    {
        obj.transform.SetLocalPositionY(value);
    }

    public static void SetLocalPositionY(this Transform trans, float value)
    {
        var pos = trans.localPosition;
        pos.y = value;
        trans.localPosition = pos;
    }

    public static float GetLocalPositionX(this GameObject obj)
    {
        return obj.transform.GetLocalPositionX();
    }

    public static float GetLocalPositionX(this Transform trans)
    {
        return trans.localPosition.x;
    }

    public static float GetLocalPositionY(this GameObject obj)
    {
        return obj.transform.GetLocalPositionY();
    }

    public static float GetLocalPositionY(this Transform trans)
    {
        return trans.localPosition.y;
    }
    #endregion
    public static void SetPosition(Transform trans, float x, float y, float z)
    {
        SetPosition(trans, x, y, z, false);
    }
    public static void SetLocalScale(Transform trans, float x, float y, float z)
    {
        trans.localScale = new Vector3(x, y, z);
    }
    public static void SetAnchoredPosition(RectTransform trans, float x, float y)
    {
        trans.anchoredPosition = new Vector2(x, y);
    }
    public static void AddAnchoredPosition(RectTransform rectTransform, float dx, float dy)
    {
        var anchoredPosition = rectTransform.anchoredPosition;
        anchoredPosition.x += dx;
        anchoredPosition.y += dy;
        rectTransform.anchoredPosition = anchoredPosition;
    }

    //如果 x 或者 y 值为-1，则，设置原有值
    public static void SetPivot(RectTransform trans, float x, float y)
    {
        float sizeX = x;
        float sizeY = y;
        if (x == -1)
        {
            x = trans.pivot.x;
        }
        if (y == -1)
        {
            sizeY = trans.pivot.y;
        }
        trans.pivot = new Vector2(sizeX, sizeY);
    }
    //如果 x 或者 y 值为-1，则，设置原有值
    public static void SetSizeDelta(RectTransform trans, float x, float y)
    {
        float sizeX = x;
        float sizeY = y;
        if (x == -1)
        {
            x = trans.sizeDelta.x;
        }
        if (y == -1)
        {
            sizeY = trans.sizeDelta.y;
        }
        trans.sizeDelta = new Vector2(sizeX, sizeY);
    }

    public static void SetAnchoredPosition(GameObject obj, float x, float y)
    {
        RectTransform rectTransform = obj.transform as RectTransform;
        rectTransform.anchoredPosition = new Vector2(x, y);
    }

    //如果 x 或者 y 值为-1，则，设置原有值
    public static void SetSizeDelta(GameObject obj, float x, float y)
    {
        RectTransform rectTransform = obj.transform as RectTransform;
        float sizeX = x;
        float sizeY = y;
        if (x == -1)
        {
            x = rectTransform.sizeDelta.x;
        }
        if (y == -1)
        {
            sizeY = rectTransform.sizeDelta.y;
        }
        rectTransform.sizeDelta = new Vector2(sizeX, sizeY);
    }

    public static void SetPosition(Transform trans, Transform targetTrans, bool isLocal)
    {
        if (trans != null)
        {
            if (isLocal)
            {
                trans.localPosition = targetTrans.localPosition;
            }
            else
            {
                trans.position = targetTrans.position;
            }
        }
    }
    public static void SetPosition(Transform trans, float x, float y, float z, bool isLocal)
    {
        if (trans != null)
        {
            if (isLocal)
            {
                trans.localPosition = new Vector3(x, y, z);
            }
            else
            {
                trans.position = new Vector3(x, y, z);
            }
        }
    }

    //--localEulerAngles
    public static void SetLocalEulerAngles(Transform trans, float x, float y, float z)
    {
        if (trans != null)
        {
            trans.localEulerAngles = new Vector3(x, y, z);
        }
    }

    public static void SetEulerAngles(Transform trans, float x, float y, float z)
    {
        if (trans != null)
        {
            trans.eulerAngles = new Vector3(x, y, z);
        }
    }

    public static void SetRotation(Transform trans, float x, float y, float z, float w)
    {
        SetRotation(trans, x, y, z, w, false);
    }

    public static void SetRotation(Transform trans, float x, float y, float z, float w, bool isLocal)
    {
        if (trans != null)
        {
            if (isLocal)
            {
                trans.localRotation = new Quaternion(x, y, z, w);
            }
            else
            {
                trans.rotation = new Quaternion(x, y, z, w);
            }
        }
    }
    public static void SetPositionAndRotation(Transform trans, float posX, float posY, float posZ, float rotX, float rotY, float rotZ, float rotW)
    {
        if (trans != null)
        {
            trans.SetPositionAndRotation(new Vector3(posX, posY, posZ), new Quaternion(rotX, rotX, rotZ, rotW));
        }
    }
    //--end localEulerAngles
    public static GameObject GetChild(GameObject go, string childName)
    {
        Transform child = go.transform.Find(childName);
        if (child == null)
        {
            return null;
        }
        return child.gameObject;
    }

    public static Component GetOrAddComponent(GameObject target, string className)
    {
        Component com = target.GetComponent(className);
        if (com == null)
        {
            com = target.AddComponent(GetType(className));
        }
        return com;
    }
    static public T GetOrCreateComponent<T>(this GameObject obj) where T : Component
    {
        var result = obj.GetComponent<T>();
        if (result == null)
        {
            result = obj.AddComponent<T>();
        }
        return result;
    }

    public static Component FindComponent(Transform target, string path, Type className)
    {
        Transform child = null;
        if (string.IsNullOrEmpty(path))
        {
            child = target;
        }else
        {
            child = target.Find(path);
        }
        if (child == null)
        {
            return null;
        }
        Component com = child.GetComponent(className);
        return com;
    }

    public static Component GetOrAddComponent(GameObject target, string path, Type className)
    {
        Transform child = target.transform.Find(path);
        if (child == null)
        {
            return null;
        }
        Component com = child.GetComponent(className);
        if (com == null)
        {
            com = child.gameObject.AddComponent(className);
        }
        return com;
    }
    public static Component GetOrAddComponent(GameObject target, Type className)
    {
        Component com = target.GetComponent(className);
        if (com == null)
        {
            com = target.AddComponent(className);
        }
        return com;
    }

    public static Transform[] GetAllChild(GameObject obj)
    {
        Transform[] child = null;
        int count = obj.transform.childCount;
        child = new Transform[count];
        for (int i = 0; i < count; i++)
        {
            child[i] = obj.transform.GetChild(i);
        }
        return child;
    }

    public static void SetLayerSelf(GameObject go, int layer)
    {
        go.layer = layer;
    }

    public static void SetLayer(GameObject go, int layer)
    {
        if (go.layer == layer)
        {
            return;
        }
        go.layer = layer;

        Transform t = go.transform;

        for (int i = 0, imax = t.childCount; i < imax; ++i)
        {
            Transform child = t.GetChild(i);
            SetLayer(child.gameObject, layer);
        }
    }
    public static void SetLayerByName(GameObject go, string layerMaskName)
    {
        go.layer = LayerMask.NameToLayer(layerMaskName);

        Transform t = go.transform;

        for (int i = 0, imax = t.childCount; i < imax; ++i)
        {
            Transform child = t.GetChild(i);
            SetLayer(child.gameObject, LayerMask.NameToLayer(layerMaskName));
        }
    }
    //-------------------------
    public static Type GetType(string typeName)
    {
        // 先在当前Assembly找,再在UnityEngine里找.
        // 如果都找不到,再遍历所有Assembly
        var type = Assembly.GetExecutingAssembly().GetType(typeName) ?? typeof(ParticleSystem).Assembly.GetType(typeName);
        if (type != null) return type;
        type = GetUnityType(typeName);
        if (type != null) return type;
        foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
        {
            type = a.GetType(typeName);
            if (type != null)
                return type;
        }
        return null;
    }
    private static Type GetUnityType(string typeName)
    {
        string namespaceStr = "UnityEngine";
        if (!typeName.Contains(namespaceStr))
            typeName = namespaceStr + "." + typeName;
        var assembly = Assembly.Load(namespaceStr);
        if (assembly == null)
            return null;
        return assembly.GetType(typeName);

    }
    public static bool IsNull(this UnityEngine.Object o)
    {
        return o == null;
    }
}
