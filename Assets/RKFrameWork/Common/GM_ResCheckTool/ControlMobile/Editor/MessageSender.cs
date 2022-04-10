using System;
using System.IO;
using System.Net;
using System.Text;
using UnityEngine;
namespace Majic.CM
{
    public class MessageSender
    {
        static string DefaultURL = "http://{0}:8820/?cmd={1}";
        private static string baseUrl;
        public static void Init(string path)
        {
            baseUrl = path;
        }
        public static SceneDataList GetHierarchyList()
        {
            string result = BaseSend("GetHierarchy");
            Debug.Log("GetHierarchyList==" + result);
            SceneDataList obj = JsonUtility.FromJson<SceneDataList>(result);
            return obj;
        }
        public static RenderFeatureData GetRenderFeaturesData()
        {
            string result = BaseSend("GetRenderFeaturesData");
            Debug.Log("result===" + result);
            RenderFeatureData obj = JsonUtility.FromJson<RenderFeatureData>(result);
            return obj;
        }
        public static CamerasData GetCameraData()
        {
            string result = BaseSend("GetCamerasData");
            Debug.Log("result===" + result);
            CamerasData obj = JsonUtility.FromJson<CamerasData>(result);
            return obj;
        }
        public static VolumesData GetVolumesData()
        {
            string result = BaseSend("GetVolumesData");
            Debug.Log("result===" + result);
            VolumesData obj = JsonUtility.FromJson<VolumesData>(result);
            return obj;
        }
        public static bool OnRenderFeaturesChange(int index, string name, string value)
        {
            string result = BaseSend(string.Format("ChangeCommonRenderFeaturesData&index={0}&name={1}&value={2}", index, name, value));
            return result != null && result.Equals("ok");
        }
        public static bool OnSpecialRenderFeaturesChange(int index, string value)
        {
            string result = BaseSend(string.Format("ChangeSpecialRenderFeaturesData&index={0}&value={1}", index, value));
            return result != null && result.Equals("ok");
        }

        public static bool ChangeCamerasData(int index, string value)
        {
            string result = BaseSend(string.Format("ChangeCamerasData&index={0}&value={1}", index, value));
            return result != null && result.Equals("ok");
        }

        public static string GetModelInfo()
        {
            string result = BaseSend("info");
            return result;
        }

        public static bool OnStart()
        {
            string result = BaseSend("start");
            return result.Equals("ok");
        }

        public static bool ChangeHierarchy(string value)
        {
            string result = BaseSend(string.Format("ChangeHierarchy&value={0}", value));
            return result != null && result.Equals("ok");
        }
        public static bool SetPost(int id, string post, bool enable)
        {
            string result = BaseSend(string.Format("post&id={0}&name={1}&action={2}", id, post, enable ? "enable" : "disable"));
            return result.Equals("ok");
        }

        public static string BaseSend(string parme)
        {
            string url = string.Format(DefaultURL, baseUrl, parme);
            Debug.Log("url==" + url);
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "GET";
            string result = null;
            try
            {
                using (WebResponse wr = req.GetResponse())
                {
                    using (Stream resStream = wr.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(resStream, Encoding.UTF8))
                        {
                            result = reader.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }
            return result;
        }
    }
}

