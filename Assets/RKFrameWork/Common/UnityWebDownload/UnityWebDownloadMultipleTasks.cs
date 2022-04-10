
using System.Collections.Generic;
using UnityEngine;

namespace Majic.Http
{

    public class UnityWebDownloadMultipleTasks
    {
        private int max_download_num = 5;
        private List<UnityWebDownload> m_downloadingList = new List<UnityWebDownload>();
        private List<UnityWebDownload> m_waitingDownLoadList = new List<UnityWebDownload>();
        
        public bool StopDownLoadTask(UnityWebDownloadData downloadData)
        {
            UnityWebDownload item = RemoveOneDownLoadData(m_downloadingList, downloadData);
            if(item != null)
            {
                item.AbortDownload();
                return true;
            }

            item = RemoveOneDownLoadData(m_waitingDownLoadList, downloadData);
            if (item != null)
            {
                item.AbortDownload();
                return true;
            }
            return false;
        }

        //往队列添加下载信息
        public void AddDownloadInfo(UnityWebDownloadData downloadData)
        {
            if (downloadData == null) return;
            UnityWebDownload _download = new UnityWebDownload(downloadData);
            _download.onOneDownLoadOver = OnOneDownLoadOver;
            m_waitingDownLoadList.Add(_download);
            OnDownloadNext();
        }

        void OnOneDownLoadOver(UnityWebDownloadData data)
        {
            data.Dispose();
            RemoveOneDownLoadData(m_downloadingList, data);
            OnDownloadNext();
        }
        UnityWebDownload RemoveOneDownLoadData(List<UnityWebDownload> list, UnityWebDownloadData data)
        {
            if (list != null)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    UnityWebDownload item = list[i];
                    if (item != null && item.m_downloadData == data)
                    {
                        list.RemoveAt(i);
                        return item;
                    }
                }
            }
            return null;
        }

        void OnDownloadNext()
        {
            if (m_downloadingList == null || m_waitingDownLoadList == null)
            {
                return;
            }
            if(m_downloadingList.Count > max_download_num)
            {
                return;
            }
            int count = max_download_num - m_downloadingList.Count;
            count = count > m_waitingDownLoadList.Count ? m_waitingDownLoadList.Count : count;
            for(int i = 0;i < count;i++ )
            {
                UnityWebDownload item = m_waitingDownLoadList[0];
                if(item != null)
                {
                    item.StartDownLoad();
                    m_downloadingList.Add(item);
                }
                m_waitingDownLoadList.RemoveAt(0);
            }
        }
        public void Clear()
        {
            for(int i = 0; i < m_downloadingList.Count; i++)
            {
                m_downloadingList[i].m_downloadData.Dispose();
            }
            for(int i = 0; i < m_waitingDownLoadList.Count; i++)
            {
                m_waitingDownLoadList[i].m_downloadData.Dispose();
            }
            m_downloadingList.Clear();
            m_waitingDownLoadList.Clear();
        }
    }

}
