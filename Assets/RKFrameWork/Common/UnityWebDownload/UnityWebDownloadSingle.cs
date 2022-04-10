
using System.Collections.Generic;


namespace Majic.Http
{
    public class UnityWebDownloadSingle
    {
        private List<UnityWebDownload> downLoadList = new List<UnityWebDownload>();
        //private Download m_currentDownload = null;
        public bool StopDownLoadTask(UnityWebDownloadData downloadData)
        {
            UnityWebDownload _Download = RemoveOneDownLoadData(downloadData);
            if(_Download != null)
            {
                _Download.AbortDownload();
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
            _download.StartDownLoad();
            downLoadList.Add(_download);
        }

        void OnOneDownLoadOver(UnityWebDownloadData data)
        {
            RemoveOneDownLoadData(data);
        }

        UnityWebDownload RemoveOneDownLoadData(UnityWebDownloadData data)
        {
            data.Dispose();
            if (downLoadList != null)
            {
                for (int i = 0; i < downLoadList.Count; i++)
                {
                    UnityWebDownload item = downLoadList[i];
                    if (item != null && item.m_downloadData == data)
                    {
                        downLoadList.RemoveAt(i);
                        return item;
                    }
                }
            }
            return null;
        }
        public void Clear()
        {
            for(int i = 0;i < downLoadList.Count;i++)
            {
                downLoadList[i].m_downloadData.Dispose();
            }
            downLoadList.Clear();
        }
    }

}
