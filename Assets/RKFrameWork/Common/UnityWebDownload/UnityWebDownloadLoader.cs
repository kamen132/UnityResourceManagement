
using Majic.CM;
using System;
using UnityEngine;
using UnityEngine.Networking;

public delegate void OnCallBackBoolBSString(bool a, byte[] b, string c);
public delegate void OnCallBackBSInt(byte[] dataFragment, int dataFragmentLength);

namespace Majic.Http
{
    public class UnityWebDownloadLoader
    {
        public OnCallBackFloat OnDownloadProgress;
        private OnCallBackBoolBSString OnDownloadFinish;
        public OnCallBackBSInt OnStreamingData;
        public OnCallBackInt ReceiveContentLength;
        private enum DownLoadingState
        {
            Init = 0,
            Downloading,
            Retring,
            Success,
            TimeOut,
            Failed
        }
        private UnityWebRequest unityWebRequest;
       
        private DownLoadingState downloadingState = DownLoadingState.Init;
        private float lastDownloadProgress = 0;
        private int downloadingTimmer = -1;
        private float duration = 0;
        public UnityWebDownloadLoader(string url, OnCallBackBoolBSString onDownloadFinish)
        {
            Uri _uri = new Uri(url);
            unityWebRequest = new UnityWebRequest(_uri, "GET") { downloadHandler = new UnityWebDownloadLoaderBuffer(this) };
            unityWebRequest.SetRequestHeader("Content-Type", "application/octet-stream");
            unityWebRequest.disposeDownloadHandlerOnDispose = true;
            //unityWebRequest.timeout = _Timeout;
            OnDownloadFinish = onDownloadFinish;
        }

        private void OnDownloadSuccess()
        {
            byte[] downloadData = unityWebRequest.downloadHandler.data;
            unityWebRequest.Dispose();
            unityWebRequest = null;
            OnDownloadFinish?.Invoke(true, downloadData, null);
            OnDownloadFinish = null;
            TimerManager.Instance.OnStopTimer(downloadingTimmer);
            downloadingTimmer = -1;
        }
        private void OnDownloadFailed()
        {
            string error = unityWebRequest.error;
            if(downloadingState == DownLoadingState.TimeOut)
            {
                error = "TimeOut";
            }
            unityWebRequest.Dispose();
            unityWebRequest = null;
            OnDownloadFinish?.Invoke(false, null, error);
            OnDownloadFinish = null;
            TimerManager.Instance.OnStopTimer(downloadingTimmer);
            downloadingTimmer = -1;
        }
        private void OnDownloading()
        {
            if(unityWebRequest.isDone)
            {
                
                if (unityWebRequest.result == UnityWebRequest.Result.ConnectionError || unityWebRequest.result == UnityWebRequest.Result.ProtocolError)
                {
                    downloadingState = DownLoadingState.Failed;
                }
                else
                {
                    if (unityWebRequest.responseCode == 200 || unityWebRequest.responseCode == 304)
                    {
                        downloadingState = DownLoadingState.Success;
                    }
                    else
                    {
                        downloadingState = DownLoadingState.Failed;
                    }
                }
            }else
            {
                if (MathUtil.FlootEqual(lastDownloadProgress, unityWebRequest.downloadProgress))
                {
                    duration += Time.deltaTime;
                    if (duration >= Timeout)
                    {
                        downloadingState = DownLoadingState.TimeOut;
                    }
                }
                else
                {
                    lastDownloadProgress = unityWebRequest.downloadProgress;
                    //lastProcess = req.downloadProgress;
                    //duration = 0;
                    OnDownloadProgress?.Invoke(lastDownloadProgress);
                }
            }
        }
        private void DownloadTimmerCallBack(float time)
        {
            switch(downloadingState)
            {
                case DownLoadingState.Downloading:
                    OnDownloading();
                    break;
                case DownLoadingState.Success:
                    OnDownloadSuccess();
                    break;
                case DownLoadingState.Failed:
                case DownLoadingState.TimeOut:
                    OnDownloadFailed();
                    break;
            }
        }
        public UnityWebRequestAsyncOperation Send()
        {
            downloadingTimmer = TimerManager.Instance.GetUpdateTimer(1, DownloadTimmerCallBack, false, true);
            TimerManager.Instance.OnStartTimer(downloadingTimmer);
            downloadingState = DownLoadingState.Downloading;
            return unityWebRequest.SendWebRequest();
        }
        public void SetRangeHeader(long firstBytePos)
        {
            unityWebRequest.SetRequestHeader("Range", string.Format("bytes={0}-", firstBytePos));
        }

        public int Timeout { get; set; } = 10;

        public void Abort()
        {
            unityWebRequest?.Dispose();
            unityWebRequest = null;
            OnDownloadFinish = null;
            OnDownloadProgress = null;
        }
        
    }
  
}
