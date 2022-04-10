using System;
using System.IO;
using UnityEngine;

namespace Majic.Http
{
    public delegate void OnOneUnityWebDownLoadOver(UnityWebDownloadData data);
    public class UnityWebDownload
    {
        public UnityWebDownloadData m_downloadData = null;
        private UnityWebDownloadLoader m_currentLoader = null;
        private FileStream fileStream;
        public OnOneUnityWebDownLoadOver onOneDownLoadOver = null;
        private long downLoadedLength = 0;//已经下载了的长度
        private long  fileTitleLength = 0;
        //private int speedTimerIndex = -1;
        public UnityWebDownload(UnityWebDownloadData downloadData)
        {
            if (downloadData == null) return;
            m_downloadData = downloadData;
            m_currentLoader = null;
        }
        
        public void StartDownLoad()
    	{
            if(m_downloadData == null)
            {
                return;
            }
			string _directoryNameForSavePath = Path.GetDirectoryName(m_downloadData.savaFilePath);
            if (Directory.Exists(_directoryNameForSavePath) == false)
            {
                Directory.CreateDirectory(_directoryNameForSavePath);
            }
            downLoadedLength = FileUtils.GetFileLength(m_downloadData.savaFilePath);

            //fileStream = new FileStream(m_downloadData.savaFilePath, FileMode.Append);
            //downLoadedLength = fileStream.Length;
            m_currentLoader = new UnityWebDownloadLoader(m_downloadData.url, OnLoadFinish);

            m_currentLoader.Timeout = m_downloadData.m_timeOut;
         
            if (downLoadedLength > 0)
            {
                Debug.Log("range=" + downLoadedLength + "__" + m_downloadData.savaFilePath);
                m_currentLoader.SetRangeHeader(downLoadedLength);
            }

            //m_currentLoader.OnDownloadProgress += OnDownloadProgress;
            m_currentLoader.OnStreamingData += OnStreamingData;
            m_currentLoader.ReceiveContentLength += ReceiveContentLength;
            
            m_currentLoader.Send();
        }

        private void ReceiveContentLength(int length)
        {
            fileTitleLength = length;
            //Debug.Log("ReceiveContentLength==" + length);
        }

        private void OnStreamingData(byte[] dataFragment, int dataFragmentLength)
        {
            try
            {
                //Debug.Log("OnStreamingData==" + dataFragmentLength);
                using (fileStream = new FileStream(m_downloadData.savaFilePath, FileMode.Append))
                {
                    fileStream.Write(dataFragment, 0, dataFragmentLength);
                    fileStream.Close();
                }

                //fileStream.Write(dataFragment, 0, dataFragmentLength);
                downLoadedLength += dataFragmentLength;
                float progress = 0;
                if (downLoadedLength > 0 )
                {
                    progress = downLoadedLength /fileTitleLength;
                }
                m_downloadData.onProgress?.Invoke(progress);
            }
            catch (Exception e)
            {
                Debug.Log("OnDataDownloaded Exception====" + e);
            }
        }

        private void OnLoadFinish(bool isSuccess, byte[] data, string msg)
        {
            ClearCacheData();
            //Debug.Log("m_downloadData.savaFilePath===" + m_downloadData.savaFilePath);
            if (isSuccess)
            {
                if (string.IsNullOrEmpty(m_downloadData.fileMd5) == false)
                {
                    FileUtils.CheckFileMD5(m_downloadData.savaFilePath, m_downloadData.fileMd5, (bool isSame) =>
                    {
                        if (isSame)
                        {
                            OnRunSuccess();
                        }
                        else
                        {
                            OnRunFailure(DownLoadErrorState.MD5CheckFail, msg);
                        }
                    });
                }
                else
                {
                    OnRunSuccess();
                }
            }
            else
            {
                OnRunFailure(DownLoadErrorState.None, msg);
            }
            onOneDownLoadOver?.Invoke(m_downloadData);
        }
        
        void OnRunFailure(DownLoadErrorState state, string msg)
        {
            m_downloadData.onOneFailure?.Invoke(state, m_downloadData, msg);
            m_downloadData.onOneFailure = null;
            //m_downloadData = null;
        }
        void OnRunSuccess()
        {
            m_downloadData.onOneSuccess?.Invoke(m_downloadData);
            m_downloadData.onOneSuccess = null;
            //m_downloadData = null;
        }

        void ClearCacheData()
        {
            //if(fileStream != null)
            //{
            //    fileStream.Close();
            //    fileStream.Dispose();
            //    fileStream = null;
            //}
           
            GC.Collect();
        }
        public void AbortDownload()
        {
            ClearCacheData();
            Debug.Log("AbortDownload:" + m_downloadData.url);
            //TimerManager.Instance.OnStopTimer(speedTimerIndex);
            //speedTimerIndex = -1;
            if (m_currentLoader != null)
            {
                m_currentLoader.Abort();
                m_currentLoader = null;
            }
        }

    }

}

