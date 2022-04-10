#if USE_URP
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Majic.CM
{
    public class VolumeView : CMBaseWindowEditor
    {
        private VolumesData m_volumesData;
        private string[] titles = null;
        private int selectIndex = 0;
        public bool isChange = false;
        public float space = 7;
        public override void OnGUI()
        {
            if (GUILayout.Button("Refresh", GUILayout.ExpandWidth(true)))
            {
                m_volumesData = MessageSender.GetVolumesData();
                VolumeData[] volumesData = m_volumesData.volumesData;
                titles = new string[volumesData.Length];
                for (int i = 0; i < volumesData.Length; i++)
                {
                    VolumeData item = volumesData[i];
                    titles[i] = item.name;
                }
            }
            if (titles != null)
            {
                selectIndex = GUILayout.Toolbar(selectIndex, titles, GUILayout.Height(40));
            }
            GUILayout.Space(7);
            UpdateView();
        }
        //public class VolumeData
        //{
        //    public string name;
        //    public bool active = false;

        //    public LayeredBloomData layeredBloomData;
        //    public VignetteData vignetteData;
        //    public ColorAdjustmentsData colorAdjustmentsData;
        //    public ChromaticAberrationData chromaticAberrationData;

        //    public DepthOfFieldData depthOfFieldData;
        //    public VolumetricFogData volumetricFogData;

        //    public LiftGammaGainData liftGammaGainData;
        //}
        void UpdateView()
        {
            isChange = false;
            if(m_volumesData == null || m_volumesData.volumesData == null)
            {
                return;
            }
            VolumeData[] volumesData = m_volumesData.volumesData;
            VolumeData volumeData = volumesData[selectIndex];
            UpdateTonemappingData(volumeData);
            UpdateLayeredBloomData(volumeData);
        }

        void UpdateLayeredBloomData(VolumeData volumeData)
        {
            if (volumeData.layeredBloomData == null || volumeData.layeredBloomData.name.Length == 0)
            {
                return;
            }
            GUILayout.BeginVertical(EditorUIClass.GetStyle(EditorUIClass.AREA1_STYLE), GUILayout.ExpandWidth(true));
            LayeredBloomData layeredBloomData = volumeData.layeredBloomData;
            bool active = GUILayout.Toggle(layeredBloomData.active, layeredBloomData.name, GUILayout.ExpandWidth(true));
            if (layeredBloomData.active != active)
            {
                layeredBloomData.active = active;
                isChange = true;
            }

            GUILayout.BeginHorizontal();
            GUILayout.Label("threshold", GUILayout.Width(150));
            float value = EditorGUILayout.Slider(layeredBloomData.threshold, layeredBloomData.thresholdRange.x, layeredBloomData.thresholdRange.y, GUILayout.ExpandWidth(true));
            if (value != layeredBloomData.threshold)
            {
                layeredBloomData.threshold = value;
                isChange = true;
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("intensity", GUILayout.Width(150));
            value = EditorGUILayout.Slider(layeredBloomData.intensity, layeredBloomData.intensityRange.x, layeredBloomData.intensityRange.y, GUILayout.ExpandWidth(true));
            if (value != layeredBloomData.intensity)
            {
                layeredBloomData.intensity = value;
                isChange = true;
            }
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
            GUILayout.Space(space);
        }
        void UpdateTonemappingData(VolumeData volumeData)
        {
            if(volumeData.tonemappingData == null || volumeData.tonemappingData.name.Length == 0)
            {
                return;
            }
            GUILayout.BeginVertical(EditorUIClass.GetStyle(EditorUIClass.AREA1_STYLE), GUILayout.ExpandWidth(true));
            TonemappingData tonemappingData = volumeData.tonemappingData;
            bool active = GUILayout.Toggle(tonemappingData.active, tonemappingData.name, GUILayout.ExpandWidth(true));
            if (tonemappingData.active != active)
            {
                tonemappingData.active = active;
                isChange = true;
            }

            TonemappingMode enumData = (TonemappingMode)EditorGUILayout.EnumPopup("mode", tonemappingData.mode, GUILayout.ExpandWidth(true));
            if (enumData != tonemappingData.mode)
            {
                tonemappingData.mode = enumData;
                isChange = true;
            }
            GUILayout.EndVertical();
            GUILayout.Space(space);
        }


        public override void OnInit()
        {
            winName = "屏幕后期开关";
        }

        public override void OnUpdate()
        {
            
        }

        public override void Clear()
        {
            selectIndex = 0;
            m_volumesData = null;
            titles = null;
        }
    }
}
#endif
