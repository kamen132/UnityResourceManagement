using System.Collections.Generic;
using UnityEngine;

namespace Majic.CM
{
    public class AssetUnLoadManager : MonoBehaviour
    {
        private int timerID = -1;
        private bool isFrameUnLoadAB = true;
        public static int unLoadABNum = 5;
        private Dictionary<string, AssetBundle> abDic = new Dictionary<string, AssetBundle>();
        private Dictionary<string, int> removingABKeys = new Dictionary<string, int>();
        public void OnInit()
        {
            if(isFrameUnLoadAB)
            {
                timerID = TimerManager.Instance.GetLateUpdateTimer(1, OnCallBackFloat, false, true, true);
                TimerManager.Instance.OnStartTimer(timerID);
            }
            
        }
        private void OnCallBackFloat(float t)
        {
            int num = 0;
            var itor = abDic.GetEnumerator();
            while (itor.MoveNext())
            {
                var value = itor.Current;
                removingABKeys[value.Key] = 1;
                num++;
                if(num >= unLoadABNum)
                {
                    break;
                }
            }
            if(num > 0)
            {
                var itor2 = removingABKeys.GetEnumerator();
                while (itor2.MoveNext())
                {
                    var value = itor2.Current;
                    abDic[value.Key].Unload(true);
                    abDic.Remove(value.Key);
                }
                removingABKeys.Clear();
            }
        }
        public void AddAB(string key, AssetBundle ab)
        {
            if(isFrameUnLoadAB)
            {
                abDic[key] = ab;
            }
            else
            {
                ab.Unload(true);
            }
        }
        public void UnLoadAllAB()
        {
            var itor = abDic.GetEnumerator();
            while (itor.MoveNext())
            {
                var value = itor.Current;
                value.Value.Unload(true);
            }
            abDic.Clear();
        }
        public AssetBundle TryGetAB(string key)
        {
            if(abDic.ContainsKey(key))
            {
                AssetBundle ab = abDic[key];
                abDic.Remove(key);
                return ab;
            }
            return null;
        }
        public bool IsHasDyingAB(string key)
        {
            return abDic.ContainsKey(key);
        }
        public void Dispose()
        {
            TimerManager.Instance.OnStopTimer(timerID);
            timerID = -1;
            UnLoadAllAB();
        }
        private static volatile AssetUnLoadManager _AssetUnLoadManager;

        public static AssetUnLoadManager Instance
        {
            get
            {
                return _AssetUnLoadManager;
            }
        }
        private void Awake()
        {
            _AssetUnLoadManager = this;
        }
    }
}

