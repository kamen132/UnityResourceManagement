using System.Collections.Generic;
using UnityEngine;

namespace Majic.CM
{
    public class TimerManager : MonoBehaviour
    {
        private static volatile TimerManager instance;
        private List<Timer> cacheTimerList = new List<Timer>();
        private Dictionary<int, Timer> updateTimerDic = new Dictionary<int, Timer>();
        private Dictionary<int, Timer> lateUpdateTimerDic = new Dictionary<int, Timer>();
        private Dictionary<int, Timer> fixedUpdateTimerDic = new Dictionary<int, Timer>();

        private Dictionary<int, Timer> newUpdateTimerDic = new Dictionary<int, Timer>();
        private Dictionary<int, Timer> newLateUpdateTimerDic = new Dictionary<int, Timer>();
        private Dictionary<int, Timer> newFixedUpdateTimerDic = new Dictionary<int, Timer>();

        private List<Timer> delayRecycleList = new List<Timer>();
        private int timerIndex = 0;
        public static TimerManager Instance
        {
            get { return instance; }
        }

        private void Awake()
        {
            instance = this;
        }

        private void Update()
        {
            if (newUpdateTimerDic.Count > 0)
            {
                foreach (var item in newUpdateTimerDic)
                {
                    updateTimerDic[item.Key] = item.Value;
                }
                newUpdateTimerDic.Clear();
            }
            if (updateTimerDic.Count > 0)
            {
                RecycleTimerOverTimer(updateTimerDic);
                foreach (var item in updateTimerDic)
                {
                    Timer timer = item.Value;
                    timer.OnUpdate(false);
                }
            }
        }

        private void LateUpdate()
        {
            if (newLateUpdateTimerDic.Count > 0)
            {
                foreach (var item in newLateUpdateTimerDic)
                {
                    lateUpdateTimerDic[item.Key] = item.Value;
                }
                newLateUpdateTimerDic.Clear();
            }

            if (lateUpdateTimerDic.Count > 0)
            {
                RecycleTimerOverTimer(lateUpdateTimerDic);
                foreach (var item in lateUpdateTimerDic)
                {
                    Timer timer = item.Value;
                    timer.OnUpdate(false);
                }
            }
        }

        private void FixedUpdate()
        {
            if (newFixedUpdateTimerDic.Count > 0)
            {
                foreach (var item in newFixedUpdateTimerDic)
                {
                    fixedUpdateTimerDic[item.Key] = item.Value;
                }
                newFixedUpdateTimerDic.Clear();
            }

            if (fixedUpdateTimerDic.Count > 0)
            {
                RecycleTimerOverTimer(fixedUpdateTimerDic);
                foreach (var item in fixedUpdateTimerDic)
                {
                    Timer timer = item.Value;
                    timer.OnUpdate(true);
                }
            }
        }

        public int GetUpdateTimer(float _delay, OnCallBackFloat _OnCallBack, bool _one_times, bool _is_frame = false, bool _scaleTime = true)
        {
            Timer timer = GetTimer();
            timerIndex++;
            newUpdateTimerDic[timerIndex] = timer;
            timer.InitData(_delay, _OnCallBack, _one_times, _is_frame, _scaleTime, timerIndex);
            return timerIndex;
        }
        public int GetLateUpdateTimer(float _delay, OnCallBackFloat _OnCallBack, bool _one_times, bool _is_frame = false, bool _scaleTime = true)
        {
            Timer timer = GetTimer();
            timerIndex++;
            newLateUpdateTimerDic[timerIndex] = timer;
            timer.InitData(_delay, _OnCallBack, _one_times, _is_frame, _scaleTime, timerIndex);
            return timerIndex;
        }
        public int GetFixedUpdateTimer(float _delay, OnCallBackFloat _OnCallBack, bool _one_times, bool _is_frame = false, bool _scaleTime = true)
        {
            Timer timer = GetTimer();
            timerIndex++;
            newFixedUpdateTimerDic[timerIndex] = timer;
            timer.InitData(_delay, _OnCallBack, _one_times, _is_frame, _scaleTime, timerIndex);
            return timerIndex;
        }

        public void OnStopTimer(int _timerIndex)
        {
            Timer timer = GetSaveTimer(_timerIndex);

            if (timer != null)
            {
                timer.OnStop();
            }
        }

        public void OnPauseTimer(int _timerIndex)
        {
            Timer timer = GetSaveTimer(_timerIndex);
            if (timer != null)
            {
                timer.OnPause();
            }
        }

        public void OnResumeTimer(int _timerIndex)
        {
            Timer timer = GetSaveTimer(_timerIndex);
            if (timer != null)
            {
                timer.OnResume();
            }
        }

        Timer GetSaveTimer(int _timerIndex)
        {
            Timer timer = null;
            if (updateTimerDic.ContainsKey(_timerIndex))
            {
                timer = updateTimerDic[_timerIndex];
            }
            if (lateUpdateTimerDic.ContainsKey(_timerIndex))
            {
                timer = lateUpdateTimerDic[_timerIndex];
            }
            if (fixedUpdateTimerDic.ContainsKey(_timerIndex))
            {
                timer = fixedUpdateTimerDic[_timerIndex];
            }

            if (newUpdateTimerDic.ContainsKey(_timerIndex))
            {
                timer = newUpdateTimerDic[_timerIndex];
            }
            if (newLateUpdateTimerDic.ContainsKey(_timerIndex))
            {
                timer = newLateUpdateTimerDic[_timerIndex];
            }
            if (newFixedUpdateTimerDic.ContainsKey(_timerIndex))
            {
                timer = newFixedUpdateTimerDic[_timerIndex];
            }
            return timer;
        }
        public void OnStartTimer(int _timerIndex)
        {
            Timer timer = GetSaveTimer(_timerIndex);
            if (timer != null)
            {
                timer.OnStart();
            }
        }

        public bool IsTimerRunning(int _timerIndex)
        {
            Timer timer = GetSaveTimer(_timerIndex);
            if (timer == null)
            {
                return false;
            }
            return timer.IsRunning();
        }
        void RecycleTimerOverTimer(Dictionary<int, Timer> dic)
        {
            if (dic.Count > 0)
            {
                foreach (var item in dic)
                {
                    Timer timer = item.Value;
                    if (timer.IsOver())
                    {
                        delayRecycleList.Add(timer);
                    }
                }
            }
            if (delayRecycleList.Count > 0)
            {
                foreach (Timer timer in delayRecycleList)
                {
                    cacheTimerList.Add(timer);
                    dic.Remove(timer.timerIndex);
                }
                delayRecycleList.Clear();
            }
        }

        Timer GetTimer()
        {
            Timer timer = null;
            int count = cacheTimerList.Count;
            if (count > 0)
            {
                for (int i = count - 1; i >= 0; i--)
                {
                    timer = cacheTimerList[i];
                    if (timer.CanBeUse())
                    {
                        cacheTimerList.RemoveAt(i);
                        timer.SetUse();
                        return timer;
                    }
                }
            }
            timer = new Timer();
            return timer;
        }

        public void SetTimerID(int timerIndex, int id)
        {
            Timer timer = GetSaveTimer(timerIndex);
            timer.ID = id;
        }
        public void PauseAllTimer(int id = -1)
        {
            foreach (var item in newUpdateTimerDic)
            {
                Timer timer = item.Value;
                if (timer != null && (id == -1 || (id != -1 && timer.ID == id)))
                {
                    timer.OnPause();
                }
            }
            foreach (var item in updateTimerDic)
            {
                Timer timer = item.Value;
                if (timer != null && (id == -1 || (id != -1 && timer.ID == id)))
                {
                    timer.OnPause();
                }
            }

            foreach (var item in newLateUpdateTimerDic)
            {
                Timer timer = item.Value;
                if (timer != null && (id == -1 || (id != -1 && timer.ID == id)))
                {
                    timer.OnPause();
                }
            }
            foreach (var item in lateUpdateTimerDic)
            {
                Timer timer = item.Value;
                if (timer != null && (id == -1 || (id != -1 && timer.ID == id)))
                {
                    timer.OnPause();
                }
            }

            foreach (var item in newFixedUpdateTimerDic)
            {
                Timer timer = item.Value;
                if (timer != null && (id == -1 || (id != -1 && timer.ID == id)))
                {
                    timer.OnPause();
                }
            }
            foreach (var item in fixedUpdateTimerDic)
            {
                Timer timer = item.Value;
                if (timer != null && (id == -1 || (id != -1 && timer.ID == id)))
                {
                    timer.OnPause();
                }
            }
        }

        public void ResumeAllTimer(int id = -1)
        {
            foreach (var item in newUpdateTimerDic)
            {
                Timer timer = item.Value;
                if (timer != null && (id == -1 || (id != -1 && timer.ID == id)))
                {
                    timer.OnResume();
                }
            }
            foreach (var item in updateTimerDic)
            {
                Timer timer = item.Value;
                if (timer != null && (id == -1 || (id != -1 && timer.ID == id)))
                {
                    timer.OnResume();
                }
            }

            foreach (var item in newLateUpdateTimerDic)
            {
                Timer timer = item.Value;
                if (timer != null && (id == -1 || (id != -1 && timer.ID == id)))
                {
                    timer.OnResume();
                }
            }
            foreach (var item in lateUpdateTimerDic)
            {
                Timer timer = item.Value;
                if (timer != null && (id == -1 || (id != -1 && timer.ID == id)))
                {
                    timer.OnResume();
                }
            }

            foreach (var item in newFixedUpdateTimerDic)
            {
                Timer timer = item.Value;
                if (timer != null && (id == -1 || (id != -1 && timer.ID == id)))
                {
                    timer.OnResume();
                }
            }
            foreach (var item in fixedUpdateTimerDic)
            {
                Timer timer = item.Value;
                if (timer != null && (id == -1 || (id != -1 && timer.ID == id)))
                {
                    timer.OnResume();
                }
            }
        }

        public void StopAllTimmer(int id = -1)
        {
            foreach (var item in newUpdateTimerDic)
            {
                Timer timer = item.Value;
                if (timer != null && (id == -1 || (id != -1 && timer.ID == id)))
                {
                    timer.OnStop();
                }
            }
            foreach (var item in updateTimerDic)
            {
                Timer timer = item.Value;
                if (timer != null && (id == -1 || (id != -1 && timer.ID == id)))
                {
                    timer.OnStop();
                }
            }

            foreach (var item in newLateUpdateTimerDic)
            {
                Timer timer = item.Value;
                if (timer != null && (id == -1 || (id != -1 && timer.ID == id)))
                {
                    timer.OnStop();
                }
            }
            foreach (var item in lateUpdateTimerDic)
            {
                Timer timer = item.Value;
                if (timer != null && (id == -1 || (id != -1 && timer.ID == id)))
                {
                    timer.OnStop();
                }
            }

            foreach (var item in newFixedUpdateTimerDic)
            {
                Timer timer = item.Value;
                if (timer != null && (id == -1 || (id != -1 && timer.ID == id)))
                {
                    timer.OnStop();
                }
            }
            foreach (var item in fixedUpdateTimerDic)
            {
                Timer timer = item.Value;
                if (timer != null && (id == -1 || (id != -1 && timer.ID == id)))
                {
                    timer.OnStop();
                }
            }
        }

        //public void Dispose()
        //{
        //    updateTimerDic.Clear();
        //    lateUpdateTimerDic.Clear();
        //    fixedUpdateTimerDic.Clear();
        //    newUpdateTimerDic.Clear();
        //    newLateUpdateTimerDic.Clear();
        //    newFixedUpdateTimerDic.Clear();
        //}
    }
}

