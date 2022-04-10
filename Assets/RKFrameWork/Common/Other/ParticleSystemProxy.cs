//
// 粒子中介类，用于在lua侧控制粒子系统，包括的功能：
// 1、播放，停止、暂停、恢复、重播粒子特效；
// 2、查询粒子特效状态，是否正在播放、暂停、停止
//  3、设置粒子循环、预热、Awake后立即播放
///

using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class ParticleSystemProxy : MonoBehaviour {

    public float destroyOnTime;

    //private bool loop;
    //private bool prewarm;
    //private bool playOnAwake;
    private bool isPlaying;
    private bool isPaused;
    private bool isStopped;

    private List<ParticleSystem> _parentPsList = new List<ParticleSystem>();
    private List<ParticleSystem> _childrenPsList = new List<ParticleSystem>();

    void Awake()
    {
        GetPSComponent(gameObject);
        for(int i = 0; i < transform.childCount; i++)
        {
            GetPSComponent(transform.GetChild(0).gameObject);
        }
        if(_parentPsList.Count <= 0)
        {
            enabled = false;
            throw new System.Exception("没有找到任何粒子系统！");
        }
        GetChildrenPSComponents();
    }

    private void GetPSComponent(GameObject target)
    {
        ParticleSystem ps = target.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            _parentPsList.Add(ps);
        }
    }

    private void GetChildrenPSComponents()
    {
        for(int i = 0; i < _parentPsList.Count; i++)
        {
            ParticleSystem[] psArr = _parentPsList[i].GetComponentsInChildren<ParticleSystem>();
            _childrenPsList.AddRange(psArr);
        }
    }

    /// <summary>
    /// 是否循环
    /// </summary>
    public bool Loop
    {
        get
        {
            return _parentPsList[0].main.loop;
        }

        set
        {
                for(int i = 0; i < _childrenPsList.Count; i++)
                {
                    ParticleSystem.MainModule main = _childrenPsList[i].main;
                    main.loop = value;
                }
            //loop = value;
        }
    }

    /// <summary>
    /// 是否预热粒子系统
    /// </summary>
    public bool Prewarm
    {
        get
        {
            return _parentPsList[0].main. prewarm;
        }

        set
        {
                for (int i = 0; i < _childrenPsList.Count; i++)
                {
                    ParticleSystem.MainModule main = _childrenPsList[i].main;
                    main.prewarm = value;
                }
            //prewarm = value;
        }
    }

    /// <summary>
    /// Awake立即播放粒子
    /// </summary>
    public bool PlayOnAwake
    {
        get
        {
            return _parentPsList[0].main.playOnAwake;
        }

        set
        {
                for (int i = 0; i < _childrenPsList.Count; i++)
                {
                    ParticleSystem.MainModule main = _childrenPsList[i].main;
                    main.playOnAwake = value;
                }
            //playOnAwake = value;
        }
    }

    /// <summary>
    /// 是否在播放中
    /// </summary>
    public bool IsPlaying
    {
        get { return isPlaying; }
    }

    /// <summary>
    /// 是否暂停中
    /// </summary>
    public bool IsPaused
    {
        get { return isPaused; }
    }

    public bool IsStopped
    {
        get { return isStopped; }
    }

    /// <summary>
    /// 播放粒子
    /// </summary>
    public void Play()
    {
        for(int i = 0;  i < _parentPsList.Count; i++)
        {
            _parentPsList[i].Play(true);
        }
        if(_parentPsList.Count > 0)
        {
            isPlaying = true;
            isPaused = false;
            isStopped = false;
        }
    }

    /// <summary>
    /// 停止播放
    /// </summary>
    public void Stop()
    {
        for (int i = 0; i < _parentPsList.Count; i++)
        {
            _parentPsList[i].Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
        if (_parentPsList.Count > 0)
        {
            isPlaying = false;
            isPaused = false;
            isStopped = true;
        }
    }

    /// <summary>
    /// 暂停
    /// </summary>
    public void Pause()
    {
        for (int i = 0; i < _parentPsList.Count; i++)
        {
            _parentPsList[i].Pause(true);
        }
        if (_parentPsList.Count > 0)
        {
            isPlaying = false;
            isPaused = true;
            isStopped = false;
        }
    }

    /// <summary>
    /// 恢复播放
    /// </summary>
    public void Resume()
    {
        for (int i = 0; i < _parentPsList.Count; i++)
        {
            _parentPsList[i].Simulate(0, true, false);
            _parentPsList[i].Play();
        }
        if (_parentPsList.Count > 0)
        {
            isPlaying = true;
            isPaused = false;
            isStopped = false;
        }
    }

    /// <summary>
    /// 重新播放
    /// </summary>
    public void Restart()
    {
        for (int i = 0; i < _parentPsList.Count; i++)
        {
            _parentPsList[i].Simulate(0, true, true);
            _parentPsList[i].Play();
        }
        if (_parentPsList.Count > 0)
        {
            isPlaying = true;
            isPaused = false;
            isStopped = false;
        }
    }

//#if UNITY_EDITOR
//    //测试代码
//    void OnGUI()
//    {
//        if(GUILayout.Button("Play"))
//        {
//            Play();
//        }
//        if (GUILayout.Button("Stop"))
//        {
//            Stop();
//        }
//        if (GUILayout.Button("Pause"))
//        {
//            Pause();
//        }
//        if (GUILayout.Button("Resume"))
//        {
//            Resume();
//        }
//        if (GUILayout.Button("Restart"))
//        {
//            Restart();
//        }
//        if (GUILayout.Button("is playing"))
//        {
//            print("is playing:" + IsPlaying);
//        }
//        if (GUILayout.Button("is paused"))
//        {
//            print("is paused:" + IsPaused);
//        }
//        if (GUILayout.Button("is stopped"))
//        {
//            print("is stopped:" + IsStopped);
//        }
//        Loop = GUILayout.Toggle(Loop, "Loop");
//    }
//#endif
}
