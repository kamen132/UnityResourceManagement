
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Majic.CM
{
    public class RenderFeaturesView : CMBaseWindowEditor
    {
        private RenderFeatureData renderFeatureData;
        private string[] titles = null;
        private int selectIndex = 0;
        public float space = 7;
        public override void OnGUI()
        {
            if(titles == null)
            {
                if (GUILayout.Button("Refresh", GUILayout.ExpandWidth(true)))
                {
                    renderFeatureData = MessageSender.GetRenderFeaturesData();
                    ForwardRenderersData[] renderList = renderFeatureData.renderList;
                    titles = new string[renderList.Length];
                    for (int i = 0; i < renderList.Length; i++)
                    {
                        ForwardRenderersData item = renderList[i];
                        titles[i] = item.name;
                    }
                }
            }else
            {
                selectIndex = GUILayout.Toolbar(selectIndex, titles, GUILayout.Height(40));
            }
            
            GUILayout.Space(7);
            UpdateSelectView();
        }

        void UpdateSelectView()
        {
            if(renderFeatureData == null || renderFeatureData.renderList == null)
            {
                return;
            }
            ForwardRenderersData data = renderFeatureData.renderList[selectIndex];
            List<BaseRenderersFeature> rendererFeatures = data.rendererFeatures;
            foreach(BaseRenderersFeature feature in rendererFeatures)
            {
                GUILayout.BeginVertical(EditorUIClass.GetStyle("Area1"), GUILayout.ExpandWidth(true));
                bool active = GUILayout.Toggle(feature.active, feature.name);
                if (feature.active != active)
                {
                    feature.active = active;
                    MessageSender.OnRenderFeaturesChange(selectIndex, feature.name, JsonUtility.ToJson(feature));
                }
                GUILayout.EndVertical();
                GUILayout.Space(space);
            }
            UpdateFurRenderFeature(data);
            GUILayout.Space(space);
            UpdateFurComputeRenderFeature(data);
            GUILayout.Space(space);
            UpdateSSAORenderFeatureData(data);
        }

        void UpdateFurRenderFeature(ForwardRenderersData data)
        {
            FurRenderFeatureData furRenderFeatureData = data.furRenderFeatureData;
            if (furRenderFeatureData != null && furRenderFeatureData.name.Length != 0)
            {
                GUILayout.BeginVertical(EditorUIClass.GetStyle("Area1"), GUILayout.ExpandWidth(true));
                bool isChange = false;
                bool active = GUILayout.Toggle(furRenderFeatureData.active, furRenderFeatureData.name, GUILayout.ExpandWidth(true));
                if (furRenderFeatureData.active != active)
                {
                    furRenderFeatureData.active = active;
                    isChange = true;
                }
                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                GUILayout.Label("PassLayerNum:", GUILayout.Width(150));
                int value = (int)EditorGUILayout.Slider(furRenderFeatureData.PassLayerNum, 0, 200, GUILayout.ExpandWidth(true));
                GUILayout.Space(7);
                if (value != furRenderFeatureData.PassLayerNum)
                {
                    furRenderFeatureData.PassLayerNum = value;
                    isChange = true;
                }
                if (isChange)
                {
                    MessageSender.OnSpecialRenderFeaturesChange(selectIndex, JsonUtility.ToJson(data));
                }

                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
            }
           
        }

        void UpdateFurComputeRenderFeature(ForwardRenderersData data)
        {
           
            FurComputeRenderFeatureData furComputeRenderFeatureData = data.furComputeRenderFeatureData;
            if (furComputeRenderFeatureData != null && furComputeRenderFeatureData.name.Length != 0)
            {
                GUILayout.BeginVertical(EditorUIClass.GetStyle("Area1"), GUILayout.ExpandWidth(true));
                bool isChange = false;
                bool active = GUILayout.Toggle(furComputeRenderFeatureData.active, furComputeRenderFeatureData.name, GUILayout.ExpandWidth(true));
                if (furComputeRenderFeatureData.active != active)
                {
                    furComputeRenderFeatureData.active = active;
                    isChange = true;
                }

                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                GUILayout.Label("PassLayerNum", GUILayout.Width(150));
                int value = (int)EditorGUILayout.Slider(furComputeRenderFeatureData.PassLayerNum, 0, 200, GUILayout.ExpandWidth(true));
                GUILayout.Space(7);
                if (value != furComputeRenderFeatureData.PassLayerNum)
                {
                    furComputeRenderFeatureData.PassLayerNum = value;
                    isChange = true;
                }
                if (isChange)
                {
                    MessageSender.OnSpecialRenderFeaturesChange(selectIndex, JsonUtility.ToJson(data));
                }

                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
            }
           
        }
        void UpdateSSAORenderFeatureData(ForwardRenderersData data)
        {
            SSAORenderFeatureData ssaoRenderFeatureData = data.ssaoRenderFeatureData;
            if (ssaoRenderFeatureData != null && ssaoRenderFeatureData.name.Length != 0)
            {
                GUILayout.BeginVertical(EditorUIClass.GetStyle("Area1"), GUILayout.ExpandWidth(true));
                bool isChange = false;
                bool active = GUILayout.Toggle(ssaoRenderFeatureData.active, ssaoRenderFeatureData.name, GUILayout.ExpandWidth(true));
                if (ssaoRenderFeatureData.active != active)
                {
                    ssaoRenderFeatureData.active = active;
                    isChange = true;
                }

                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                GUILayout.Label("Samples:", GUILayout.Width(150));
                int value = (int)EditorGUILayout.Slider(ssaoRenderFeatureData.Samples, 0, 20, GUILayout.ExpandWidth(true));
                GUILayout.Space(7);
                if (value != ssaoRenderFeatureData.Samples)
                {
                    ssaoRenderFeatureData.Samples = value;
                    isChange = true;
                }
                if (isChange)
                {
                    MessageSender.OnSpecialRenderFeaturesChange(selectIndex, JsonUtility.ToJson(data));
                }
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
            }
        }
        public override void OnInit()
        {
            winName = "Feature开关";
        }

        public override void OnUpdate()
        {
            
        }

        public override void Clear()
        {
            selectIndex = 0;
            renderFeatureData = null;
            titles = null;
        }
    }
}

