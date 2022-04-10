using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace Majic.CM
{
    public partial class MobileServer : MonoBehaviour
    {
        public float fps = 0, minFps, maxFps = 0;

        private HttpListener httpListener = null;
        private Thread serverThread = null;
        private bool isRuning = false;
        private Queue<ThreadTask> reqList = new Queue<ThreadTask>();
        Dictionary<int, GameObject> objectsDic = new Dictionary<int, GameObject>();
        Dictionary<int, GameObjectData> objectDatas = new Dictionary<int, GameObjectData>();
        public float showTime = 1f;
        
        private float deltaTime = 0f;
        private RenderFeatureData renderFeaturesData;
        private CamerasData camerasData;

        private VolumesData m_volumesData;

        private float count = 0;
        public bool CPUOverhead = false;
        public int CPUTime = 5;

        void Update()
        {
            if (reqList.Count > 0)
            {
                ThreadTask task = reqList.Dequeue();
                DoCmd(task);
            }
        }

        private void LateUpdate()
        {
            //count++;
            //deltaTime += Time.unscaledDeltaTime;
            //if (deltaTime >= showTime)
            //{
            //    fps = count / deltaTime;
            //    count = 0;
            //    deltaTime = 0f;
            //}
            //if (minFps > fps)
            //{
            //    minFps = fps;
            //}
            //if (maxFps < fps)
            //{
            //    maxFps = fps;
            //}
            CPUCal();
            GPURender();
        }
        private void CPUCal()
        {
            if (CPUOverhead)
            {
                for (int i = 1; i < CPUTime * 100; i++)
                {
                    for (int j = i; j < CPUTime * 100; j++)
                    {
                        count += Mathf.Pow(count * 2.1f * i, count * 2.1f * j);
                    }
                }
            }
        }
        void OnEnable()
        {
            DontDestroyOnLoad(gameObject);
            ServerStart();
            InitGPUCal();
        }

        SceneDataList GetSceneDataList()
        {
            objectsDic.Clear();
            SceneDataList sceneDataList = new SceneDataList();
            sceneDataList.List = new List<SceneData>();

            int sceneCount = SceneManager.sceneCount;
            for (int i = 0; i < sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                sceneDataList.List.Add(GetSceneData(scene));
            }
            sceneDataList.List.Add(GetSceneData(gameObject.scene));
            objectsDic.Remove(gameObject.GetInstanceID());
            return sceneDataList;
        }
        SceneData GetSceneData(Scene scene)
        {
            SceneData sceneData = new SceneData();
            sceneData.Name = scene.name;
            sceneData.objectList = new List<GameObjectData>();
            GameObject[] os = scene.GetRootGameObjects();
            for (int j = 0; j < os.Length; j++)
            {
                GetObjectDicData(sceneData.objectList, os[j].transform, null);
            }
            return sceneData;
        }

        private const string START = "start";
        private const string END = "end";
        private const string CMD = "cmd";
        private const string GET_HIERARCHY = "GetHierarchy";
        private const string CHANGE_HIERARCHY = "ChangeHierarchy";
        private const string GET_RENDERFEATURESDATA = "GetRenderFeaturesData";

        private const string CHANGE_COMMON_RENDERFEATURESDATA = "ChangeCommonRenderFeaturesData";
        private const string CHANGE_SPECIAL_RENDERFEATURESDATA = "ChangeSpecialRenderFeaturesData";

        private const string GET_CAMERASDATA = "GetCamerasData";
        private const string CHANGE_CAMERASDATA = "ChangeCamerasData";

        private const string GET_VOLUMESDATA = "GetVolumesData";
        private const string CHANGE_VOLUMESDATA = "ChangeVolumesData";

        void DoCmd(ThreadTask task)
        {
            string result = string.Empty;
            switch (task.Parame[CMD])
            {
                case START:
                    {
                        result = "ok";
                        break;
                    }
                case GET_HIERARCHY:
                    {
                        SceneDataList sList = GetSceneDataList();
                        result = JsonUtility.ToJson(sList);
                        Debug.Log(GET_HIERARCHY + result);
                        break;
                    }
                case CHANGE_HIERARCHY:
                    {
                        ChangeGameObject(task);
                        result = "ok";
                        break;
                    }
             
                case GET_CAMERASDATA:
                    {
                        camerasData = GetCamerasData();
                        result = JsonUtility.ToJson(camerasData);
                        Debug.Log(GET_CAMERASDATA + result);
                        break;
                    }
                case CHANGE_CAMERASDATA:
                    {
                        ChangeCamerasData(task);
                        break;
                    }
#if USE_URP
                    case GET_RENDERFEATURESDATA:
                        {
                            renderFeaturesData = GetRenderFeaturesData();
                            result = JsonUtility.ToJson(renderFeaturesData);
                            Debug.Log(GET_RENDERFEATURESDATA + result);
                            break;
                        }
                    case CHANGE_COMMON_RENDERFEATURESDATA:
                        {
                            ChangeCommonRenderFeaturesData(task);
                            break;
                        }
                    case CHANGE_SPECIAL_RENDERFEATURESDATA:
                        {
                            ChangeSpecialRenderFeaturesData(task);
                            break;
                        }
                    case GET_VOLUMESDATA:
                        {
                            m_volumesData = GetVolumesData();
                            result = JsonUtility.ToJson(m_volumesData);
                            Debug.Log(GET_VOLUMESDATA + result);
                            break;
                        }
                    case CHANGE_VOLUMESDATA:
                        {
                            ChangeVolumesData(task);
                            break;
                        }
#endif

                case END:
                    {
                        result = "ok";
                        break;
                    }
            }
            byte[] buff = Encoding.UTF8.GetBytes(result);
            task.Output.Write(buff, 0, buff.Length);
            task.Output.Close();
            task.Output.Dispose();
        }
      
        void ServerStart()
        {
            if (isRuning)
            {
                return;
            }
            httpListener = new HttpListener();
            httpListener.Prefixes.Add("http://+:8820/");
            httpListener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
            httpListener.Start();

            isRuning = true;
            serverThread = new Thread(ServerThreadRun);
            serverThread.Start();
        }
        void ServerThreadRun()
        {
            while (isRuning)
            {
                var context = httpListener.GetContext();
                HttpListenerRequest request = context.Request;
                HttpListenerResponse response = context.Response;
                response.ContentEncoding = Encoding.UTF8;
                //Stream output = response.OutputStream;
                ThreadTask task = new ThreadTask();
                task.Parame = request.QueryString;
                task.Output = response.OutputStream;
                reqList.Enqueue(task);
            }
        }

        private void OnDisable()
        {
            ServerStop();
        }
        void ServerStop()
        {
            isRuning = false;
            if (serverThread != null)
            {
                serverThread.Abort();
            }
            if (httpListener != null)
            {
                httpListener.Stop();
                httpListener = null;
            }
        }


        public bool GPUOverhead = false;
        public int GPUTime = 30;

        private Material mat;
        private Shader shaderGPUOverhead;

        private RenderTexture srcTex;
        private RenderTexture destTex;
        private void InitGPUCal()
        {
            if (shaderGPUOverhead == null)
            {
                shaderGPUOverhead = Shader.Find("ControlMobile/GPUCal");
            }
            mat = new Material(shaderGPUOverhead);
            srcTex = new RenderTexture(1920, 1080, 0, RenderTextureFormat.ARGB32);
            destTex = new RenderTexture(1920, 1080, 0, RenderTextureFormat.ARGB32);
        }
        private void GPURender()
        {
            if (GPUOverhead)
            {
                mat.SetInt("loopCount", GPUTime);
                Graphics.Blit(srcTex, destTex, mat);
            }
        }

    }
}
