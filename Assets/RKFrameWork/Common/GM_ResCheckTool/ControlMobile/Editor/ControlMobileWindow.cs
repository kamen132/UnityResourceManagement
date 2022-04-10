using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Majic.CM
{
    public partial class ControlMobileWindow : EditorWindow
    {
        private int m_selectIndex = 0;
        private List<CMBaseWindowEditor> m_windows = new List<CMBaseWindowEditor>();
        private static ControlMobileWindow window;


        private static int width = 1280, height = 720;
        private string connectIP = "127.0.0.1";
        private bool isConnecting = false;
        private bool isInitConnectingView = false;

        private bool m_cpuCal = false;
        private bool m_gpuCal = false;
        private int m_cpuCalValue = 0;
        private int m_gpuCalValue = 0;

        private string m_postProcessingStr = "后处理开关";


        //[MenuItem("CheckWindow/GetMesh")]
        static void GetMesh()
        {
            string path = "Assets/Product/Editor/Resources/Scene/scene_kaenstreet_normal/F/kaenstreet_f_building_yuanjing01_no2uv.FBX";
            var obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            MeshFilter _MeshFilter = obj.GetComponentInChildren<MeshFilter>();
            Mesh mesh = _MeshFilter.sharedMesh;
            VertexAttributeDescriptor[] attributes = mesh.GetVertexAttributes();
            int value = 0;
            foreach(VertexAttributeDescriptor item in attributes)
            {
                value += 4 * item.dimension;
                //item.format
            }
            var uv1 = mesh.uv;
            var datuv2a2 = mesh.uv2;
            int[] triangles = mesh.triangles;
            Vector4[] tangents = mesh.tangents;
            Color[] colors = mesh.colors;
            Debug.Log("111" + obj);
        }
        [MenuItem("CheckWindow/OpenControlMobile")]
        static void OpenOnlineMobile()
        {
            window = (ControlMobileWindow)GetWindow(typeof(ControlMobileWindow));
            window.titleContent = new GUIContent("ControlMobileWindow");
            window.position = new Rect((Screen.currentResolution.width - width) / 2, (Screen.currentResolution.height - height) / 2, width, height);
            window.OnInit();
            window.Show();

        }
        public void OnInit()
        {

            m_windows.Clear();
 
            HierarchyView hierarchyView = CreateInstance<HierarchyView>();
            hierarchyView.OnInit();
            m_windows.Add(hierarchyView);

            RenderFeaturesView renderFeaturesView = CreateInstance<RenderFeaturesView>();
            renderFeaturesView.OnInit();
            m_windows.Add(renderFeaturesView);


            CameraView cameraView = CreateInstance<CameraView>();
            cameraView.OnInit();
            m_windows.Add(cameraView);
            //VolumeView volumeView = CreateInstance<VolumeView>();
            //volumeView.OnInit();
            //m_windows.Add(volumeView);



        }
        void OnGUI()
        {
            GUILayout.Space(7);
            connectIP = EditorGUILayout.TextField("IP:", connectIP);
            GUILayout.Space(7);
            if (isConnecting == false)
            {
                ShowConnectView();
            }
            else
            {
                ShowConnectingView();
            }
        }
        void ShowConnectView()
        {
            if (GUILayout.Button("Connect IP address", EditorUIClass.GetStyle(EditorUIClass.ADD_BUTTON_STYLE), GUILayout.Height(43), GUILayout.ExpandWidth(true)))
            {
                MessageSender.Init(connectIP);
                isConnecting = MessageSender.OnStart();
            }
        }
       
        void ShowConnectingView()
        {
            if(isInitConnectingView == false)
            {
                isInitConnectingView = true;
                //InitConnectingView();
            }
            if (GUILayout.Button("Reconnect"))
            {
                isInitConnectingView = false;
                isConnecting = false;
                EditorUIClass.Clear();
                foreach (var window in m_windows)
                {
                    window.Clear();
                }
            }
             //selectIndex = GUILayout.Toolbar(selectIndex, titles, GUILayout.Height(40));
            GUILayout.Space(7);
            //CMBaseWindowEditor item = windows[selectIndex];
            //item.OnGUI();
            UpdateConnectingView();
        }
     
        void ShowToolsView(float width)
        {
            GUILayout.Box("工具栏", EditorUIClass.GetStyle(EditorUIClass.THIRD_LEVEL_TITLE_STYLE),GUILayout.ExpandWidth(true));
            //GUILayout.Label("111111111111111111111111111");
            GUILayout.Space(7);
            m_cpuCal = GUILayout.Toggle(m_cpuCal, "CPU负载");
            EditorGUI.BeginDisabledGroup(m_cpuCal == false);
            GUILayout.Space(7);
            GUILayout.BeginHorizontal();
            GUILayout.Label("负载值", GUILayout.Width(50));
            m_cpuCalValue = (int)EditorGUILayout.Slider(m_cpuCalValue, 0, 100, GUILayout.Width(width - 60));
            GUILayout.EndHorizontal();
            if (GUILayout.Button("Apply", GUILayout.Width(width)))
            {

            }

            EditorGUI.EndDisabledGroup();

            EditorUIClass.DrawLine();

            m_gpuCal = GUILayout.Toggle(m_gpuCal, "GPU负载");
            EditorGUI.BeginDisabledGroup(m_gpuCal == false);
            GUILayout.Space(7);
            GUILayout.BeginHorizontal();
            GUILayout.Label("负载值", GUILayout.Width(50));
            m_gpuCalValue = (int)EditorGUILayout.Slider(m_gpuCalValue, 0, 100, GUILayout.Width(width - 60));
            GUILayout.EndHorizontal();
            if (GUILayout.Button("Apply", GUILayout.Width(width)))
            {

            }
            EditorGUI.EndDisabledGroup();
        }
        void ShowSwitchView(float width)
        {
            float height = 30;
            GUILayout.Box("开关", EditorUIClass.GetStyle(EditorUIClass.THIRD_LEVEL_TITLE_STYLE), GUILayout.ExpandWidth(true));
            GUILayout.Space(7);
            for(int i = 0; i < m_windows.Count; i ++)
            {
                CMBaseWindowEditor data = m_windows[i];
                GUIStyle style = EditorUIClass.GetStyle(EditorUIClass.BUTTON_OFF_STYLE);
                if (m_selectIndex == i)
                {
                    style = EditorUIClass.GetStyle(EditorUIClass.BUTTON_SELECT_STYLE);
                }

                if (GUILayout.Button(data.winName, style, GUILayout.Width(width), GUILayout.Height(height)))
                {
                    m_selectIndex = i;
                }
                GUILayout.Space(7);
            }
        }
        private void UpdateConnectingView()
        {
            GUILayout.BeginHorizontal();
            UpdateLeftView();
            GUILayout.Space(10);
            UpdateMiddleView();
            GUILayout.EndHorizontal();
        }
        private void UpdateLeftView()
        {
            float width = 200;
            GUILayout.BeginVertical(EditorStyles.textArea, GUILayout.Width(width));
            //GUILayout.BeginArea(new Rect(0, 0, 310, 500), EditorStyles.textField);
            //GUILayout.EndArea();
            ShowToolsView(width);
            GUILayout.Space(10);
            ShowSwitchView(width);
            GUILayout.EndVertical();
        }
        private void UpdateMiddleView()
        {
            float width = 500;
            GUILayout.BeginVertical(EditorStyles.textArea, GUILayout.Width(width), GUILayout.Height(500));
            CMBaseWindowEditor data = m_windows[m_selectIndex];
            GUILayout.Box(data.winName, EditorUIClass.GetStyle(EditorUIClass.THIRD_LEVEL_TITLE_STYLE), GUILayout.ExpandWidth(true));
            EditorUIClass.DrawLine();
            data.OnGUI();
            GUILayout.EndVertical();
        }

        private void Update()
        {
            if (isConnecting && m_windows.Count > 0)
            {
                CMBaseWindowEditor item = m_windows[m_selectIndex];
                item.OnUpdate();
            }
              
        }
    }
}

