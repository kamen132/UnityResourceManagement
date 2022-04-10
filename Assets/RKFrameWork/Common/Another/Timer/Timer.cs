using UnityEngine;
namespace Majic.CM
{
    public class Timer
    {
        //时长，秒或者帧
        private float delay;

        private OnCallBackFloat onCallBack;
        // 是否是一次性计时
        private bool one_times;
        //是否是帧定时器，否则为秒定时器
        private bool is_frame;
        // 使用deltaTime计时，还是采用unscaledDeltaTime计时
        private bool scaledTime;
        //是否已经启用
        private bool isStarted;
        //倒计时
        private float leftDelay;
        //是否已经结束
        private bool isOver = false;
        //启动定时器时的帧数
        private int start_frame_count = Time.frameCount;

        public int timerIndex = 0;

        public int ID { get; set; } = -1;
        public void InitData(float _delay, OnCallBackFloat _OnCallBack, bool _one_times, bool _is_frame, bool _scaledTime, int _timerIndex)
        {
            delay = _delay;
            onCallBack = _OnCallBack;
            one_times = _one_times;
            is_frame = _is_frame;
            scaledTime = _scaledTime;
            timerIndex = _timerIndex;
        }

        public void OnUpdate(bool is_fixedUpdate)
        {
            if (isOver || isStarted == false)
            {
                return;
            }
            bool isTimeUp = false;
            float callBackTime = 1;
            if (is_frame)
            {
                isTimeUp = Time.frameCount >= start_frame_count + delay;
                callBackTime = scaledTime ? Time.deltaTime : Time.unscaledDeltaTime;
            }
            else
            {
                float time = 0;
                if (is_fixedUpdate)
                {
                    time = Time.fixedDeltaTime;
                }
                else
                {
                    time = scaledTime ? Time.deltaTime : Time.unscaledDeltaTime;
                }
                callBackTime = time;
                leftDelay -= time;
                isTimeUp = leftDelay <= 0;
            }
            if (isTimeUp)
            {
                OnTimeUp(callBackTime);
            }
        }
        void OnTimeUp(float time)
        {
            if (onCallBack == null)
            {
                isOver = true;
                return;
            }
            if (one_times)
            {
                isOver = true;
            }
            else
            {
                if (is_frame == false)//非帧
                {
                    leftDelay += delay;
                }
                start_frame_count = Time.frameCount;
            }
            try
            {
                onCallBack(time);
            }
            catch (System.Exception e)
            {
                Debug.LogError("error: timer callBack" + e.ToString());
            }
        }
        public void OnStart()
        {
            isOver = false;
            if (isStarted == false)
            {
                leftDelay = delay;
                isStarted = true;
                start_frame_count = Time.frameCount;
            }
        }
        public void OnPause()
        {
            isStarted = false;
        }
        public void OnResume()
        {
            isStarted = true;
        }
        public void OnStop()
        {
            ID = -1;
            onCallBack = null;
            isOver = true;
            isStarted = false;
        }
        //复位：如果计时器是启动的，并不会停止，只是刷新倒计时
        public void OnReset()
        {
            leftDelay = delay;
            start_frame_count = Time.frameCount;
        }
        public bool IsOver()
        {
            return isOver;
        }

        public bool IsRunning()
        {
            return isStarted == true && isOver == false;
        }
        public void SetUse()
        {
            isOver = false;
        }
        public bool CanBeUse()
        {
            return isOver == true && isStarted == false;
        }
    }
}

