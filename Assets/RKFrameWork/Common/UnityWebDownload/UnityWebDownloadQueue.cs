

using System.Collections.Generic;

namespace Majic.Http
{
    public class UnityWebDownloadQueue
    {
        private UnityWebDownload m_currentDownload = null;
        private List<UnityWebDownload> m_downloads = new List<UnityWebDownload>();
        // 是否正在下载
        public bool IsDownload
        {
            get
            {
                if (m_currentDownload != null)
                {
                    return true;
                }
                return false;
            }
        }
        public bool StopDownLoadTask(UnityWebDownloadData downloadData)
        {
            downloadData.Dispose();
            if(m_currentDownload.m_downloadData == downloadData)
            {
                m_currentDownload.AbortDownload();
                m_currentDownload = null;
            }
            for (int i = 0; i < m_downloads.Count; i++)
            {
                UnityWebDownload item = m_downloads[i];
                if (item != null && item.m_downloadData == downloadData)
                {
                    m_downloads.RemoveAt(i);
                    //下载下一个
                    OnDownloadNext();
                    return true;
                }
            }
            return false;
        }

        //往队列添加下载信息
        public void AddDownloadInfo(UnityWebDownloadData downloadData)
        {
            if (downloadData == null) return;
            UnityWebDownload _download = new UnityWebDownload(downloadData);
            _download.onOneDownLoadOver = OnOneDownLoadOver;
            m_downloads.Add(_download);
            OnDownloadNext();
        }

        void OnOneDownLoadOver(UnityWebDownloadData data)
        {
            data.Dispose();
            if (m_downloads.Count>0)
            {
                m_downloads.RemoveAt(0);
            }
            m_currentDownload = null;
            OnDownloadNext();
        }

		//下载下一个
        void OnDownloadNext()
        {
			if (m_downloads.Count > 0)
            {
                if (IsDownload == false)
                {
                    m_currentDownload = m_downloads[0];
                    m_currentDownload.StartDownLoad();
                    //Debug.Log("----start download----" + Path.GetFileName(m_currentDownload.m_downloadData.savaFilePath));
                }
            }
            else
            {
                //Debug.Log("无下载任务" );
            }
        }
        public void Clear()
        {
            for(int i = 0; i < m_downloads.Count; i++)
            {
                m_downloads[i].m_downloadData.Dispose();
            }
            m_downloads.Clear();
        }
    }

}
