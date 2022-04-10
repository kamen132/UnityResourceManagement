namespace Majic.Http
{
    public class UnityWebDownloadManager
    {
        private static UnityWebDownloadSingle m_downloadSingle = new UnityWebDownloadSingle();
        private static UnityWebDownloadQueue m_downloadqueue = new UnityWebDownloadQueue();
        private static UnityWebDownloadMultipleTasks m_downloadMultipleTask = new UnityWebDownloadMultipleTasks();

        public static bool AddDownloadQueue(UnityWebDownloadData downloadData)
        {
            if (downloadData == null 
                || string.IsNullOrEmpty(downloadData.savaFilePath)
                || string.IsNullOrEmpty(downloadData.url) 
            ) { return false; }

            if (m_downloadSingle == null || m_downloadqueue == null ||  m_downloadMultipleTask == null)
            {
                return false;
            }
            switch(downloadData.downLoadState)
            {
                case DownLoadState.Single:
                    m_downloadSingle.AddDownloadInfo(downloadData);
                    break;
                case DownLoadState.Queue:
                    m_downloadqueue.AddDownloadInfo(downloadData);
                    break;
                case DownLoadState.MultipleTasks:
                    m_downloadMultipleTask.AddDownloadInfo(downloadData);
                    break;
            }
            return true;
        }

        public static bool StopDownLoadTask(UnityWebDownloadData downloadData)
        {
            if (m_downloadSingle == null || m_downloadqueue == null || m_downloadMultipleTask == null)
            {
                return false;
            }
            switch (downloadData.downLoadState)
            {
                case DownLoadState.Single:
                    m_downloadSingle.StopDownLoadTask(downloadData);
                    break;
                case DownLoadState.Queue:
                    m_downloadqueue.StopDownLoadTask(downloadData);
                    break;
                case DownLoadState.MultipleTasks:
                    m_downloadMultipleTask.StopDownLoadTask(downloadData);
                    break;
            }
            return true;
        }
        public static void Clear()
        {
            m_downloadSingle.Clear();
            m_downloadqueue.Clear();
            
            m_downloadMultipleTask.Clear();
        }
    }

}