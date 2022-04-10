using UnityEditor;
using UnityEngine;
//using UnityEngine.Rendering.Universal;

namespace Majic.CM
{
    public class CameraView : CMBaseWindowEditor
    {
        private CamerasData camerasData;
        private string[] titles = null;
        private int selectIndex = 0;
        public float space = 7;
        public bool isChange = false;
        public override void OnGUI()
        {
            if (GUILayout.Button("Refresh", GUILayout.ExpandWidth(true)))
            {
                camerasData = MessageSender.GetCameraData();
                CameraData[] cameraDataArray = camerasData.cameraDataArray;
                titles = new string[cameraDataArray.Length];
                for (int i = 0; i < cameraDataArray.Length; i++)
                {
                    CameraData item = cameraDataArray[i];
                    titles[i] = item.name;
                }
            }
            if (titles != null)
            {
                selectIndex = GUILayout.Toolbar(selectIndex, titles, GUILayout.ExpandWidth(true), GUILayout.Height(40));
            }
            GUILayout.Space(7);
            UpdateSelectView();
        }

        void UpdateSelectView()
        {
            if(camerasData == null || camerasData.cameraDataArray == null)
            {
                return;
            }
            CameraData cameraData = camerasData.cameraDataArray[selectIndex];

            isChange = false;
            UpdateFieldOfView(cameraData);
            UpdateNearClipPlane(cameraData);
            UpdateFarClipPlane(cameraData);
            UpdateRenderPostProcessing(cameraData);
            //UpdateAntiAliasing(cameraData);
            UpdateStopNaN(cameraData);
            UpdateDithering(cameraData);
            UpdateRenderShadows(cameraData);
            UpdateOpaqueTexture(cameraData);
            UpdateDepthTexture(cameraData);
            UpdateNormalsTexture(cameraData);
            UpdeteRenderIndex(cameraData);
            UpdateOcclusionCulling(cameraData);
            UpdateHDR(cameraData);
            UpdateMSAA(cameraData);
            if (isChange)
            {
                MessageSender.ChangeCamerasData(selectIndex, JsonUtility.ToJson(cameraData));
            }
        }
        void UpdateNearClipPlane(CameraData cameraData)
        {
            GUILayout.BeginVertical(EditorUIClass.GetStyle(EditorUIClass.AREA1_STYLE), GUILayout.ExpandWidth(true));
            GUILayout.BeginHorizontal();
            GUILayout.Label("nearClipPlane:", GUILayout.ExpandWidth(true));
            string editorStr = GUILayout.TextArea(cameraData.nearClipPlane.ToString(), GUILayout.Width(40));
            float value = float.Parse(editorStr);
            if (value != cameraData.nearClipPlane)
            {
                cameraData.nearClipPlane = value;
                isChange = true;
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.Space(space);
        }
        void UpdateFarClipPlane(CameraData cameraData)
        {
            GUILayout.BeginVertical(EditorUIClass.GetStyle(EditorUIClass.AREA1_STYLE), GUILayout.ExpandWidth(true));
            GUILayout.BeginHorizontal();
            GUILayout.Label("farClipPlane:", GUILayout.Width(100));
            string editorStr = GUILayout.TextArea(cameraData.farClipPlane.ToString(), GUILayout.Width(40));
            float value = float.Parse(editorStr);
            if (value != cameraData.farClipPlane)
            {
                cameraData.farClipPlane = value;
                isChange = true;
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.Space(space);
        }
        void UpdateRenderPostProcessing(CameraData cameraData)
        {
            GUILayout.BeginVertical(EditorUIClass.GetStyle(EditorUIClass.AREA1_STYLE), GUILayout.ExpandWidth(true));
            bool active = GUILayout.Toggle(cameraData.renderPostProcessing, "renderPostProcessing", GUILayout.ExpandWidth(true));
            if (cameraData.renderPostProcessing != active)
            {
                cameraData.renderPostProcessing = active;
                isChange = true;
            }
            GUILayout.EndVertical();
            GUILayout.Space(space);
        }
        //void UpdateAntiAliasing(CameraData cameraData)
        //{
        //    GUILayout.BeginVertical(EditorUIClass.GetStyle(EditorUIClass.AREA1_STYLE), GUILayout.ExpandWidth(true));
        //    AntialiasingMode enumData = (AntialiasingMode)EditorGUILayout.EnumPopup("Anti-aliasing", cameraData.antialiasingMode, GUILayout.ExpandWidth(true));
        //    if (enumData != cameraData.antialiasingMode)
        //    {
        //        cameraData.antialiasingMode = enumData;
        //        isChange = true;
        //    }
        //    GUILayout.EndVertical();
        //    GUILayout.Space(space);
        //}
        void UpdateStopNaN(CameraData cameraData)
        {
            GUILayout.BeginVertical(EditorUIClass.GetStyle(EditorUIClass.AREA1_STYLE), GUILayout.ExpandWidth(true));
            bool active = GUILayout.Toggle(cameraData.stopNaN, "stopNaN", GUILayout.ExpandWidth(true));
            if (cameraData.stopNaN != active)
            {
                cameraData.stopNaN = active;
                isChange = true;
            }
            GUILayout.EndVertical();
            GUILayout.Space(space);
        }
        void UpdateDithering(CameraData cameraData)
        {
            GUILayout.BeginVertical(EditorUIClass.GetStyle(EditorUIClass.AREA1_STYLE), GUILayout.ExpandWidth(true));
            bool active = GUILayout.Toggle(cameraData.dithering, "dithering", GUILayout.ExpandWidth(true));
            if (cameraData.dithering != active)
            {
                cameraData.dithering = active;
                isChange = true;
            }
            GUILayout.EndVertical();
            GUILayout.Space(space);
        }
        void UpdateRenderShadows(CameraData cameraData)
        {
            GUILayout.BeginVertical(EditorUIClass.GetStyle(EditorUIClass.AREA1_STYLE), GUILayout.ExpandWidth(true));
            bool active = GUILayout.Toggle(cameraData.renderShadows, "renderShadows", GUILayout.ExpandWidth(true));
            if (cameraData.renderShadows != active)
            {
                cameraData.renderShadows = active;
                isChange = true;
            }
            GUILayout.EndVertical();
            GUILayout.Space(space);
        }
        void UpdateOpaqueTexture(CameraData cameraData)
        {
            GUILayout.BeginVertical(EditorUIClass.GetStyle(EditorUIClass.AREA1_STYLE), GUILayout.ExpandWidth(true));
            bool active = GUILayout.Toggle(cameraData.requiresOpaqueTexture, "requiresOpaqueTexture", GUILayout.ExpandWidth(true));
            if (cameraData.requiresOpaqueTexture != active)
            {
                cameraData.requiresOpaqueTexture = active;
                isChange = true;
            }
            GUILayout.EndVertical();
            GUILayout.Space(space);
        }
        void UpdateDepthTexture(CameraData cameraData)
        {
            GUILayout.BeginVertical(EditorUIClass.GetStyle(EditorUIClass.AREA1_STYLE), GUILayout.ExpandWidth(true));
            bool active = GUILayout.Toggle(cameraData.requiresDepthTexture, "requiresDepthTexture", GUILayout.ExpandWidth(true));
            if (cameraData.requiresDepthTexture != active)
            {
                cameraData.requiresDepthTexture = active;
                isChange = true;
            }
            GUILayout.EndVertical();
            GUILayout.Space(space);
        }
        void UpdateNormalsTexture(CameraData cameraData)
        {
            GUILayout.BeginVertical(EditorUIClass.GetStyle(EditorUIClass.AREA1_STYLE), GUILayout.ExpandWidth(true));
            bool active = GUILayout.Toggle(cameraData.requiresNormalsTexture, "requiresNormalsTexture", GUILayout.ExpandWidth(true));
            if (cameraData.requiresNormalsTexture != active)
            {
                cameraData.requiresNormalsTexture = active;
                isChange = true;
            }
            GUILayout.EndVertical();
            GUILayout.Space(space);
        }
        void UpdeteRenderIndex(CameraData cameraData)
        {
            GUILayout.BeginVertical(EditorUIClass.GetStyle(EditorUIClass.AREA1_STYLE), GUILayout.ExpandWidth(true));
            GUILayout.BeginHorizontal();
            GUILayout.Label("renderIndex", GUILayout.Width(100));
            int valueInt = (int)EditorGUILayout.Slider(cameraData.renderIndex, 0, 10, GUILayout.ExpandWidth(true));
            if (valueInt != cameraData.renderIndex)
            {
                cameraData.renderIndex = valueInt;
                isChange = true;
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.Space(space);
        }
        void UpdateOcclusionCulling(CameraData cameraData)
        {
            GUILayout.BeginVertical(EditorUIClass.GetStyle(EditorUIClass.AREA1_STYLE), GUILayout.ExpandWidth(true));
            bool active = GUILayout.Toggle(cameraData.useOcclusionCulling, "useOcclusionCulling", GUILayout.ExpandWidth(true));
            if (cameraData.useOcclusionCulling != active)
            {
                cameraData.useOcclusionCulling = active;
                isChange = true;
            }
            GUILayout.EndVertical();
            GUILayout.Space(space);
        }
        void UpdateHDR(CameraData cameraData)
        {
            GUILayout.BeginVertical(EditorUIClass.GetStyle(EditorUIClass.AREA1_STYLE), GUILayout.ExpandWidth(true));
            bool active = GUILayout.Toggle(cameraData.allowHDR, "allowHDR", GUILayout.ExpandWidth(true));
            if (cameraData.allowHDR != active)
            {
                cameraData.allowHDR = active;
                isChange = true;
            }
            GUILayout.EndVertical();
            GUILayout.Space(space);
        }
        void UpdateMSAA(CameraData cameraData)
        {
            GUILayout.BeginVertical(EditorUIClass.GetStyle(EditorUIClass.AREA1_STYLE), GUILayout.ExpandWidth(true));
            bool active = GUILayout.Toggle(cameraData.allowMSAA, "allowMSAA", GUILayout.ExpandWidth(true));
            if (cameraData.allowMSAA != active)
            {
                cameraData.allowMSAA = active;
                isChange = true;
            }
            GUILayout.EndVertical();
        }
        void UpdateFieldOfView(CameraData cameraData)
        {
            GUILayout.BeginVertical(EditorUIClass.GetStyle(EditorUIClass.AREA1_STYLE), GUILayout.ExpandWidth(true));
            GUILayout.BeginHorizontal();
            GUILayout.Label("fieldOfView", GUILayout.Width(100));
            int value = (int)EditorGUILayout.Slider(cameraData.fieldOfView, 0, 100, GUILayout.ExpandWidth(true));
            if (value != cameraData.fieldOfView)
            {
                cameraData.fieldOfView = value;
                isChange = true;
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.Space(space);
        }


        public override void OnInit()
        {
            winName = "相机开关";
        }

        public override void OnUpdate()
        {
            
        }

        public override void Clear()
        {
            selectIndex = 0;
            camerasData = null;
            titles = null;
        }
    }
}

