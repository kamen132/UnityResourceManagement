using UnityEngine;
using UnityEngine.Networking;
using static UnityEngine.Networking.UnityWebRequest;

namespace Majic.Http
{
    public delegate void OnRequestSuccessDelegate(byte[] data);
    public delegate void OnRequestErrorDelegate(Result result, long responseCode, string error);
    public class UnityWebHttp
    {
        private string url;
        private OnRequestSuccessDelegate successCallBack;
        private OnRequestErrorDelegate errorCallBack;
        private UnityWebRequest unityWebRequest;
        public UnityWebHttp CreateHttpRequest(string url, OnRequestSuccessDelegate successCallBack, OnRequestErrorDelegate errorCallBack)
        {
            return new UnityWebHttp(url, successCallBack, errorCallBack);
        }
        
        public UnityWebHttp(string url, OnRequestSuccessDelegate successCallBack, OnRequestErrorDelegate errorCallBack)
        {
            this.url = url;
            this.successCallBack = successCallBack;
            this.errorCallBack = errorCallBack;
            unityWebRequest = new UnityWebRequest(this.url, "POST");
            SetParam();
        }

        void OnRequestCompleted(AsyncOperation asyncOperation)
        {
            if (unityWebRequest.result == Result.Success)
            {
                successCallBack(unityWebRequest.downloadHandler.data);
            }
            else
            {
                errorCallBack(unityWebRequest.result, unityWebRequest.responseCode, unityWebRequest.error);
            }
            unityWebRequest.Dispose();
            unityWebRequest = null;
            successCallBack = null;
            errorCallBack = null;
        }
        public void Send(byte[] byteData)
        {
            unityWebRequest.uploadHandler = new UploadHandlerRaw(byteData);
            unityWebRequest.downloadHandler = new DownloadHandlerBuffer();
            
            UnityWebRequestAsyncOperation async = unityWebRequest.SendWebRequest();
            async.completed += OnRequestCompleted;
        }
        public void SetParam()
        {
            unityWebRequest.timeout = 15;
            unityWebRequest.SetRequestHeader("Content-Type", "application/x-protobuf");
        }
    }

}

