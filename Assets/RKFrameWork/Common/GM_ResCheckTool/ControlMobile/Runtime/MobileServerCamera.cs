using UnityEngine;
namespace Majic.CM
{
    public partial class MobileServer
    {
        private Camera[] cameras;
        CamerasData GetCamerasData()
        {
            CamerasData camerasData = new CamerasData();
           
            cameras = FindObjectsOfType<Camera>();
            CameraData[] cameraDataArray = new CameraData[cameras.Length];
            camerasData.cameraDataArray = cameraDataArray;
            int length = cameras.Length;
            for(int i = 0; i < length;i++)
            {
                Camera camera = cameras[i];
               
                CameraData cameraData = new CameraData();
                cameraData.name = camera.name;
                cameraData.fieldOfView = camera.fieldOfView;

                cameraData.farClipPlane = camera.farClipPlane;
                cameraData.nearClipPlane = camera.nearClipPlane;
       
                //UniversalAdditionalCameraData camData = camera.GetComponent<UniversalAdditionalCameraData>();
                //if (camData != null)
                //{
                //    cameraData.renderPostProcessing = camData.renderPostProcessing;
                //    cameraData.antialiasingMode = camData.antialiasing;
                //    cameraData.stopNaN = camData.stopNaN;
                //    cameraData.dithering = camData.dithering;
                //    cameraData.renderShadows = camData.renderShadows;
                //    cameraData.requiresDepthTexture = camData.requiresDepthTexture;
                //    cameraData.requiresOpaqueTexture = camData.requiresColorTexture;
                //    cameraData.requiresNormalsTexture = camData.requiresNormalsTexture;
                //    cameraData.renderIndex = camData.GetRendererIndex();
                //}
                cameraData.useOcclusionCulling = camera.useOcclusionCulling;
                cameraData.allowHDR = camera.allowHDR;
                cameraData.allowMSAA = camera.allowMSAA;
                cameraDataArray[i] = cameraData;
            }
            return camerasData;
        }

        void ChangeCamerasData(ThreadTask task)
        {
            string indexStr = task.Parame["index"];
            int index = int.Parse(indexStr);

            string value = task.Parame["value"];
            CameraData cameraData = JsonUtility.FromJson<CameraData>(value);
            Camera camera = cameras[index];

            camera.fieldOfView = cameraData.fieldOfView;
            camera.farClipPlane = cameraData.farClipPlane;
            camera.nearClipPlane = cameraData.nearClipPlane;
            //UniversalAdditionalCameraData camData = camera.GetComponent<UniversalAdditionalCameraData>();
            //if (camData != null)
            //{
            //    camData.renderPostProcessing = cameraData.renderPostProcessing;
            //    camData.antialiasing = cameraData.antialiasingMode;
            //    camData.stopNaN = cameraData.stopNaN;
            //    camData.dithering = cameraData.dithering;
            //    camData.renderShadows = cameraData.renderShadows;
            //    camData.requiresDepthTexture = cameraData.requiresDepthTexture;
            //    camData.requiresColorTexture = cameraData.requiresOpaqueTexture;
            //    camData.requiresNormalsTexture = cameraData.requiresNormalsTexture;
            //    camData.SetRenderer(cameraData.renderIndex);
            //}
            camera.useOcclusionCulling = cameraData.useOcclusionCulling;
            camera.allowHDR = cameraData.allowHDR;
            camera.allowMSAA = cameraData.allowMSAA;
        }
    }
}
