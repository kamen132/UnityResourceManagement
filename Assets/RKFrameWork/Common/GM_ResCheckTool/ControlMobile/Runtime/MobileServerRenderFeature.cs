#if USE_URP
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
namespace Majic.CM
{
    public partial class MobileServer
    {
        RenderFeatureData GetRenderFeaturesData()
        {
            ScriptableRendererData[]  rendererDataList = UniversalRenderPipeline.asset.m_RendererDataList;

            RenderFeatureData renderFeatureData = new RenderFeatureData();
            renderFeatureData.renderList = new ForwardRenderersData[rendererDataList.Length];
            for (int i = 0; i < rendererDataList.Length;i++)
            {
                ForwardRenderersData forwardRenderersData = new ForwardRenderersData();
                renderFeatureData.renderList[i] = forwardRenderersData;

                ScriptableRendererData item = rendererDataList[i];
                forwardRenderersData.name = item.name;
                List<ScriptableRendererFeature> rendererFeatures = item.rendererFeatures;
                foreach(ScriptableRendererFeature feature in rendererFeatures)
                {
                    FurRenderFeature furRenderFeature = feature as FurRenderFeature;
                    if(furRenderFeature != null)
                    {
                        FurRenderFeatureData furRenderFeatureData = new FurRenderFeatureData();
                        furRenderFeatureData.name = furRenderFeature.name;
                        furRenderFeatureData.active = feature.isActive;
                        furRenderFeatureData.PassLayerNum = furRenderFeature.settings.PassLayerNum;
                        forwardRenderersData.furRenderFeatureData = furRenderFeatureData;
                        continue;
                    }

                    FurComputeRenderFeature furComputeRenderFeature = feature as FurComputeRenderFeature;
                    if (furComputeRenderFeature != null)
                    {
                        FurComputeRenderFeatureData furComputeRenderFeatureData = new FurComputeRenderFeatureData();
                        furComputeRenderFeatureData.name = furComputeRenderFeature.name;
                        furComputeRenderFeatureData.active = feature.isActive;
                        furComputeRenderFeatureData.PassLayerNum = furComputeRenderFeature.settings.PassLayerNum;
                        forwardRenderersData.furComputeRenderFeatureData = furComputeRenderFeatureData;
                        continue;
                    }
                    SSAORenderFeature ssaoRenderFeature = feature as SSAORenderFeature;
                    if (ssaoRenderFeature != null)
                    {
                        SSAORenderFeatureData ssaoRenderFeatureData = new SSAORenderFeatureData();
                        ssaoRenderFeatureData.Samples = ssaoRenderFeature.Samples;
                        ssaoRenderFeatureData.name = ssaoRenderFeature.name;
                        ssaoRenderFeatureData.active = feature.isActive;
                        forwardRenderersData.ssaoRenderFeatureData = ssaoRenderFeatureData;
                        //forwardRenderersData.rendererFeatures.Add(ssaoRenderFeatureData);
                        continue;
                    }
                    
                    BaseRenderersFeature baseRenderersFeature = new BaseRenderersFeature();
                    baseRenderersFeature.name = feature.name;
                    baseRenderersFeature.active = feature.isActive;
                    forwardRenderersData.rendererFeatures.Add(baseRenderersFeature);

                }
            }
            return renderFeatureData;
        }

        void ChangeCommonRenderFeaturesData(ThreadTask task)
        {
            string indexStr = task.Parame["index"];
            int index = int.Parse(indexStr);
            string name = task.Parame["name"];
            string value = task.Parame["value"];
            BaseRenderersFeature gameObjectData = JsonUtility.FromJson<BaseRenderersFeature>(value);
            ScriptableRendererData[] rendererDataList = UniversalRenderPipeline.asset.m_RendererDataList;
            ScriptableRendererData scriptableRendererData = rendererDataList[index];
            foreach(ScriptableRendererFeature feature in scriptableRendererData.rendererFeatures)
            {
                if(gameObjectData.name.Equals(feature.name))
                {
                    feature.SetActive(gameObjectData.active);
                }
            }
        }
        void ChangeSpecialRenderFeaturesData(ThreadTask task)
        {
            string indexStr = task.Parame["index"];
            int index = int.Parse(indexStr);
            string value = task.Parame["value"];
            ForwardRenderersData forwardRenderersData = JsonUtility.FromJson<ForwardRenderersData>(value);
            ScriptableRendererData[] rendererDataList = UniversalRenderPipeline.asset.m_RendererDataList;
            ScriptableRendererData scriptableRendererData = rendererDataList[index];
            foreach (ScriptableRendererFeature feature in scriptableRendererData.rendererFeatures)
            {
                FurRenderFeature furRenderFeature = feature as FurRenderFeature;
                if (furRenderFeature != null)
                {
                    FurRenderFeatureData furRenderFeatureData = forwardRenderersData.furRenderFeatureData;
                    feature.SetActive(furRenderFeatureData.active);
                    furRenderFeature.settings.PassLayerNum = furRenderFeatureData.PassLayerNum;
                }
                FurComputeRenderFeature furComputeRenderFeature = feature as FurComputeRenderFeature;
                if (furComputeRenderFeature != null)
                {
                    FurComputeRenderFeatureData furComputeRenderFeatureData = forwardRenderersData.furComputeRenderFeatureData;
                    feature.SetActive(furComputeRenderFeatureData.active);
                    furComputeRenderFeature.settings.PassLayerNum = furComputeRenderFeatureData.PassLayerNum;
                }
                SSAORenderFeature ssaoRenderFeature = feature as SSAORenderFeature;
                if (ssaoRenderFeature != null)
                {
                    SSAORenderFeatureData ssaoRenderFeatureData = forwardRenderersData.ssaoRenderFeatureData;
                    feature.SetActive(ssaoRenderFeatureData.active);
                    ssaoRenderFeature.Samples = ssaoRenderFeatureData.Samples;
                }
            }
              
        }
    }
}
#endif
