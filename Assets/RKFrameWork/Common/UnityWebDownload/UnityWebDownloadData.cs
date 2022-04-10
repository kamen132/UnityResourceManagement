namespace Majic.Http
{
    public delegate void OnUnityWebOneFailure(DownLoadErrorState state, UnityWebDownloadData data, string msg); 
    public delegate void OnUnityWebOneSuccess(UnityWebDownloadData data);
    public delegate void OnOneDownLoadOver(UnityWebDownloadData data);
    public delegate void OnDownLoadOver();       //下载完成

    public delegate void OnOneSuccess(UnityWebDownloadData data);

    public delegate void OnOneFailure(DownLoadErrorState state, UnityWebDownloadData data, string msg); // 失败回调

    public delegate void OnProgress(float progress);                        // 进度回调

    public delegate void OnCallBackRealStatePtr(System.IntPtr a);
    public enum DownLoadState
    {
        Single = 0,     //直接下
        Queue,          //队列
        MultipleTasks //多下载任务
    }
    public enum DownLoadErrorState
    {
        None = 0,
        WriteError, //写失败
        NoNetWork, //没有网络
        MD5CheckFail, //md5检查失败
        Aborted, //被我们自己关闭的
    }
    public class UnityWebDownloadData
    {
        public int writeBufferSize = 8;
        public string url;
        public string proxyUrl = string.Empty;
        public string savaFilePath;
        public string fileMd5;
       
        public DownLoadState downLoadState = DownLoadState.Single;//默认为简单下载
        public int m_timeOut =10;                                // 超时

        public OnUnityWebOneSuccess onOneSuccess = null;
        public OnUnityWebOneFailure onOneFailure = null;
        public OnProgress onProgress = null;
        public OnCallBackFloat onSpeed = null;
        public OnDownLoadOver onDownLoadOver = null;
        public int retryTimes = 0;
        public int maxRetryTimes = 2;

        public UnityWebDownloadData(string url, string savaFilePath)
        {
            this.url = url;
            this.savaFilePath = savaFilePath;
        }

        public OnCallBackString ForCallback { get; set; }

        //释放lua侧的回调
        public void Dispose()
        {
            onOneFailure = null;
            onOneSuccess = null;
            onProgress = null;
            onDownLoadOver = null;
        }
    }
}












