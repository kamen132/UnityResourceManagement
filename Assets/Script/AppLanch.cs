using System;
using System.Collections;
using System.Collections.Generic;
using Majic.CM;
using UnityEngine;

public class AppLanch : MonoBehaviour
{
    private void Awake()
    {
        gameObject.AddComponent<ResourcesManager>();
        gameObject.AddComponent<AssetLoadController>();
        gameObject.AddComponent<AssetBundleManager>();
        gameObject.AddComponent<SoundManager>();
        gameObject.AddComponent<CoroutineHelper>();
        gameObject.AddComponent<TimerManager>();
        //gameObject.AddComponent<NativeCenter>();
        ResourcesManager.Instance.Startup();
    }
}
