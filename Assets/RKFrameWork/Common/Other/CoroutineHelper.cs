using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineHelper : MonoBehaviour
{
    private static volatile CoroutineHelper ms_Instance;
    public static CoroutineHelper Instance
    {
        get
        {
            return ms_Instance;
        }
    }
    private void Awake()
    {
        ms_Instance = this;
    }
    private int coroutineIndex = 0; //coroutine id index
    private Dictionary<int, OnCallBack> coroutineCallBackDic = new Dictionary<int, OnCallBack>();
    private Dictionary<int, Coroutine> coroutineDic = new Dictionary<int, Coroutine>();
    public int StartWaitForSecond(OnCallBack callBack, float second)
    {
        coroutineIndex++;
        coroutineCallBackDic[coroutineIndex] = callBack;
        Coroutine coroutine = StartCoroutine(WaitForSecond(second, coroutineIndex));
        coroutineDic[coroutineIndex] = coroutine;
        return coroutineIndex;
    }

    public int StartWaitForNextFrame(OnCallBack callBack)
    {
        coroutineIndex++;
        coroutineCallBackDic[coroutineIndex] = callBack;
        Coroutine coroutine = StartCoroutine(WaitForNextFrame(coroutineIndex));
        coroutineDic[coroutineIndex] = coroutine;
        return coroutineIndex;
    }

    public int StartWaitForFrame(OnCallBack callBack, int count)
    {
        coroutineIndex++;
        coroutineCallBackDic[coroutineIndex] = callBack;
        Coroutine coroutine = StartCoroutine(WaitForFrame(coroutineIndex, count));
        coroutineDic[coroutineIndex] = coroutine;
        return coroutineIndex;
    }

    public int WaitForEndOfFrame(OnCallBack callBack)
    {
        coroutineIndex++;
        coroutineCallBackDic[coroutineIndex] = callBack;
        Coroutine coroutine = StartCoroutine(WaitForEndOfFrames(coroutineIndex));
        coroutineDic[coroutineIndex] = coroutine;
        return coroutineIndex;
    }

    IEnumerator WaitForEndOfFrames(int key)
    {
        yield return new WaitForEndOfFrame(); ;
        if (coroutineCallBackDic.ContainsKey(key))
        {
            coroutineCallBackDic[key]();
            coroutineCallBackDic.Remove(key);
            coroutineDic.Remove(key);
        }
    }
    IEnumerator WaitForFrame(int key, int count)
    {
        while (count > 0)
        {
            count--;
            yield return null;
        }
        if (coroutineCallBackDic.ContainsKey(key))
        {
            coroutineCallBackDic[key]();
            coroutineCallBackDic.Remove(key);
            coroutineDic.Remove(key);
        }
    }

    IEnumerator WaitForNextFrame(int key)
    {
        yield return null;
        if (coroutineCallBackDic.ContainsKey(key))
        {
            coroutineCallBackDic[key]();
            coroutineCallBackDic.Remove(key);
            coroutineDic.Remove(key);
        }
    }

    IEnumerator WaitForSecond(float second, int key)
    {
        yield return new WaitForSeconds(second);
        if(coroutineCallBackDic.ContainsKey(key))
        {
            coroutineCallBackDic[key]();
            coroutineCallBackDic.Remove(key);
            coroutineDic.Remove(key);
        }
    }

    public void StopCoroutine(int key)
    {
        if(coroutineDic.ContainsKey(key))
        {
            Coroutine coroutine = coroutineDic[key];
            StopCoroutine(coroutine);
            coroutineDic.Remove(key);
            coroutineCallBackDic.Remove(key);
        }
        
    }
    public void Clear()
    {
        coroutineCallBackDic.Clear();
        coroutineDic.Clear();
        OnStopAllCoroutines();
    }

    public void OnStopAllCoroutines()
    {
        StopAllCoroutines();
    }
}
