using UnityEngine.Networking;


namespace Majic.Http
{
    public class UnityWebDownloadLoaderBuffer: DownloadHandlerScript
    {
        private UnityWebDownloadLoader unityWebDownloadLoader;


        public UnityWebDownloadLoaderBuffer(UnityWebDownloadLoader loader) :base(new byte[1024 * 200])
        {
            unityWebDownloadLoader = loader;
        }

        [System.Obsolete]
        protected override void ReceiveContentLength(int contentLength)
        {
            unityWebDownloadLoader.ReceiveContentLength?.Invoke(contentLength);
        }
        
        protected override bool ReceiveData(byte[] data, int dataLength)
        {
            bool ret = base.ReceiveData(data, dataLength);
            if(unityWebDownloadLoader != null && unityWebDownloadLoader.OnStreamingData != null)
            {
                unityWebDownloadLoader.OnStreamingData(data, dataLength);
            }
            return ret;
        }
    }
  
}
