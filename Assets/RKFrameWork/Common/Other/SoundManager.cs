using Majic.CM;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{

    private static volatile SoundManager ms_Instance;
    public const int BG_AudioSourceIndex = 0;
    private Transform _soundRoot;
    private float _bgmVolume = 1.0f;
    private float _soundVolume = 1.0f;
    private AudioSource _bgm;
    private AudioSource _bgm2;
    private Dictionary<int, AudioSource> audioSourceDic = new Dictionary<int, AudioSource>();
    private Dictionary<int, int> audioTimerDic = new Dictionary<int, int>();
    private List<AudioSource> audioSourceCahce = new List<AudioSource>();
    private int audioSourceIndex = BG_AudioSourceIndex;

    private int bgm1_tweenTimerIndex = -1;

    private int bgm1TimerIndex = -1;
    private int bgm1IDIndex = -1;

    private int bgm2TimerIndex = -1;
    private int bgm2IDIndex = -1;
    private bool isMuteSound = false;
    public static SoundManager Instance
    {
        get
        {
            if(ms_Instance == null)
            {
                GameObject _soundRoot = GameObject.Find("SoundRoot");
                if (null == _soundRoot)
                {
                    _soundRoot = new GameObject("SoundRoot");
                    _soundRoot.AddComponent<AudioListener>();
                }
                ms_Instance =  _soundRoot.AddComponent<SoundManager>();
                ms_Instance.OnInit();
            }
            return ms_Instance;
        }
    }

    public void OnInit()
    {
        _soundRoot = GameObject.Find("SoundRoot").transform;
        _bgm = GetAudioSource();
        //audioSourceDic[BG_AudioSourceIndex] = _bgm;
        if (_bgm2 == null)
        {
            _bgm2 = GetAudioSource();
        }
    }

    #region bgm1
    public int PlayBGM(AudioClip clip, float volume, bool loop , float delay, OnCallBack callBack, int preSoundID)
    {
        if (preSoundID == -1)
        {
            audioSourceIndex++;
            preSoundID = audioSourceIndex;
        }
        bgm1IDIndex = preSoundID;
        if (clip != null)
        {
            Play(_bgm, clip, loop, delay, volume);
        }
        TimerManager.Instance.OnStopTimer(bgm1_tweenTimerIndex);
        TimerManager.Instance.OnStopTimer(bgm1TimerIndex);
        bgm1TimerIndex = -1;
        if (callBack != null)
        {
            bgm1TimerIndex = TimerManager.Instance.GetUpdateTimer(clip.length + delay, (float time) =>
            {
                if (loop == false)
                {
                    _bgm.Stop();
                    _bgm.clip = null;
                    bgm1IDIndex = -1;
                }
                callBack();
            }, loop == false, false, false);
            TimerManager.Instance.OnStartTimer(bgm1TimerIndex);
        }
        return bgm1IDIndex;
    }
    
    public void PauseBGM(int index = -1)
    {
        if(index != -1 && index != bgm1IDIndex)
        {
            return;
        }
        TimerManager.Instance.OnPauseTimer(bgm1_tweenTimerIndex);
        _bgm.Pause();
    }
    public void ResumeBGM(int index = -1)
    {
        if(index != -1 && index != bgm1IDIndex)
        {
            return;
        }
        TimerManager.Instance.OnResumeTimer(bgm1_tweenTimerIndex);
        _bgm.UnPause();
    }
    public void StopBGM(int index = -1)
    {
        if (index != -1 && index != bgm1IDIndex)
        {
            return;
        }
        TimerManager.Instance.OnStopTimer(bgm1_tweenTimerIndex);
        _bgm.Stop();
        _bgm.clip = null;
    }
    public void TweenVolumeBGM(float from, float to, float dur, OnCallBack callBack = null)
    {
        AudioSource audioSource = _bgm;
        audioSource.volume = from;
        if (from == to)
        {
            return;
        }
        float speed = (to - from) / dur;
        float nowVolum = from;
        float fromTime = 0;
        TimerManager.Instance.OnStopTimer(bgm1_tweenTimerIndex);
        bgm1_tweenTimerIndex = TimerManager.Instance.GetUpdateTimer(1, (float time) =>
        {
            fromTime += time;
            nowVolum = from + speed * fromTime;
            audioSource.volume = nowVolum;

            //Debug.Log("fromTime==" + fromTime + "time==" + time + "nowVolum==" + nowVolum + "speed" + speed);
            if (fromTime >= dur)
            {
                TimerManager.Instance.OnStopTimer(bgm1_tweenTimerIndex);
                if (callBack != null)
                {
                    callBack();
                }
            }
        }, false, true, true);
        TimerManager.Instance.OnStartTimer(bgm1_tweenTimerIndex);

    }
    public bool IsBGMPlaying(int index = -1)
    {
        if (index == -1)
        {
            return _bgm.isPlaying;
        }
        return _bgm.isPlaying == true && bgm1IDIndex == index;
    }
    #endregion
    public int PreGetSoundID()
    {
        audioSourceIndex++;
        return audioSourceIndex;
    }

    public int PlaySound(AudioClip audioClip, float volume, bool loop, float delay, OnCallBack callBack, int preSoundID)
    {
        if(preSoundID == -1)
        {
            audioSourceIndex++;
            preSoundID = audioSourceIndex;
        }
        
        AudioSource audioSource = GetAudioSource();
        if (!isMuteSound)
        {
            Play(audioSource, audioClip, loop, delay, volume);
        }
        audioSourceDic[preSoundID] = audioSource;

        int timerIndex = TimerManager.Instance.GetUpdateTimer(audioClip.length + delay, (float time) =>
        {
            if(loop == false)
            {
                StopSound(preSoundID);
            }
            if(callBack != null)
            {
                callBack();
            }
        }, loop == false, false, false);
        TimerManager.Instance.OnStartTimer(timerIndex);
        audioTimerDic[preSoundID] = timerIndex;
        return preSoundID;
    }

    #region bgm2
    public int PlayBGM2(AudioClip clip, float volume, bool loop, float delay, OnCallBack callBack, int preSoundID)
    {
        if (preSoundID == -1)
        {
            audioSourceIndex++;
            preSoundID = audioSourceIndex;
        }
        bgm2IDIndex = preSoundID;
        if (clip != null)
        {
            Play(_bgm2, clip, loop, delay, volume);
        }

        TimerManager.Instance.OnStopTimer(bgm2TimerIndex);
        bgm2TimerIndex = -1;
        if (callBack != null)
        {
            bgm2TimerIndex = TimerManager.Instance.GetUpdateTimer(clip.length + delay, (float time) =>
            {
                if (loop == false)
                {
                    _bgm2.Stop();
                    _bgm2.clip = null;
                    bgm2IDIndex = -1;
                }
                callBack();
            }, loop == false, false, false);
            TimerManager.Instance.OnStartTimer(bgm2TimerIndex);
        }
        return bgm2IDIndex;
    }

    public void PauseBGM2(int index = -1)
    {
        if (index != -1 && index != bgm2IDIndex)
        {
            return;
        }
        _bgm2.Pause();
    }

    public void ResumeBGM2(int index = -1)
    {
        if (index != -1 && index != bgm2IDIndex)
        {
            return;
        }
        _bgm2.UnPause();
    }
    public void StopBGM2(int index = -1)
    {
        if (index != -1 && index != bgm2IDIndex)
        {
            return;
        }
        _bgm2.Stop();
        _bgm2.clip = null;
    }
    public bool IsBGM2Playing(int index = -1)
    {
        if (index == -1)
        {
            return _bgm2.isPlaying;
        }
        return _bgm2.isPlaying == true && bgm2IDIndex == index;
    }
    #endregion
    AudioSource GetAudioSource()
    {
        if(audioSourceCahce.Count == 0)
        {
            GameObject _soundPlayer = new GameObject("SoundPlayer");
            
            _soundPlayer.transform.SetParent(_soundRoot.transform);
            AudioSource _audioSouce = _soundPlayer.AddComponent<AudioSource>();
            return _audioSouce;
        }
        return audioSourceCahce.Pop();
    }

    public void MuteAll(bool mute ,bool includeBGM) 
    {
        var _enumerator = audioSourceDic.GetEnumerator();
        while (_enumerator.MoveNext())
        {
            var _cur = _enumerator.Current;
            _cur.Value.mute = mute;
        }

        isMuteSound = mute;

        if(includeBGM)
        {
            _bgm.mute = mute;
            if(_bgm2 != null)
            {
                _bgm2.mute = mute;
            }
        }
    }

    public void MuteBGM(bool mute)
    {
        _bgm.mute = mute;
        if (_bgm2 != null)
        {
            _bgm2.mute = mute;
        }
    }

    public void StopSound(int soundId)
    {
        if(audioSourceDic.ContainsKey(soundId))
        {
            AudioSource audioSource = audioSourceDic[soundId];
            audioSource.Stop();
            audioSource.clip = null;
            audioSourceDic.Remove(soundId);
            audioSourceCahce.Add(audioSource);
        }

        if(audioTimerDic.ContainsKey(soundId))
        {
            int timerIndex = audioTimerDic[soundId];
            TimerManager.Instance.OnStopTimer(timerIndex);
            audioTimerDic.Remove(soundId);
        }
    }

    public void PauseSound(int soundId)
    {
        if (audioSourceDic.ContainsKey(soundId) && audioSourceDic[soundId] != null)
        {
            audioSourceDic[soundId].Pause();
        }
        if (audioTimerDic.ContainsKey(soundId))
        {
            int timerIndex = audioTimerDic[soundId];
            TimerManager.Instance.OnPauseTimer(timerIndex);
        }
    }

    public void ResumeSound(int soundId)
    {
        if (audioSourceDic.ContainsKey(soundId) && audioSourceDic[soundId] != null)
        {
            audioSourceDic[soundId].UnPause();
        }
        if (audioTimerDic.ContainsKey(soundId))
        {
            int timerIndex = audioTimerDic[soundId];
            TimerManager.Instance.OnResumeTimer(timerIndex);
        }
    }

    public void PauseAllSound()
    {
        foreach(var item in audioSourceDic)
        {
            PauseSound(item.Key);
        }
        _bgm.Pause();
    }

    public void ResumeAllSound()
    {
        foreach (var item in audioSourceDic)
        {
            ResumeSound(item.Key);
        }
        _bgm.UnPause();
    }

    public void StopAllSound()
    {
        foreach (var item in audioSourceDic)
        {
            int soundId = item.Key;
            AudioSource audioSource = audioSourceDic[soundId];
            audioSource.Stop();
            audioSource.clip = null;
            audioSourceCahce.Add(audioSource);
            if (audioTimerDic.ContainsKey(soundId))
            {
                int timerIndex = audioTimerDic[soundId];
                TimerManager.Instance.OnStopTimer(timerIndex);
                audioTimerDic.Remove(soundId);
            }
        }
        audioSourceDic.Clear();
        _bgm.Stop();
        if(_bgm2)
        {
            _bgm2.Stop();
        }
    }
    public bool IsPlaying(int soundId)
    {
        if (audioSourceDic.ContainsKey(soundId) && audioSourceDic[soundId] != null)
        {
            return audioSourceDic[soundId].isPlaying;
        }
        return false;
    }

    public void SetBGMVolume(float volume)
    {
        _bgmVolume = volume;
        _bgm.volume = _bgmVolume;
        if(_bgm2)
        {
            _bgm2.volume = _bgmVolume;
        }
    }

    public float GetBGMVolume()
    {
        return _bgmVolume;
    }

    public float GetCurrentBGMVolume()
    {
        return _bgm.volume;
    }
    
    public void SetAllSoundVolume(float volume)
    {
        _soundVolume = volume;
        var _enumerator = audioSourceDic.GetEnumerator();
        while (_enumerator.MoveNext())
        {
            var _cur = _enumerator.Current;
            _cur.Value.volume = _soundVolume;
        }
    }
    public float GetSoundVolume(int soundId)
    {
        if (audioSourceDic.ContainsKey(soundId) && audioSourceDic[soundId] != null)
        {
            return audioSourceDic[soundId].volume;
        }
        return 0;
    }
    public void SetSoundVolumeById(int soundId, float volume)
    {
        if (audioSourceDic.ContainsKey(soundId) && audioSourceDic[soundId] != null)
        {
            audioSourceDic[soundId].volume = volume;
        }
    }

    private void Play(AudioSource audioSource, AudioClip clip, bool loop , float delay, float volume)
    {
        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.loop = loop;
        audioSource.volume = volume;
        audioSource.PlayDelayed(delay);
    }

    public void Dispose()
    {
        audioSourceDic.Clear();
        audioTimerDic.Clear();
        audioSourceCahce.Clear();
    }
}

