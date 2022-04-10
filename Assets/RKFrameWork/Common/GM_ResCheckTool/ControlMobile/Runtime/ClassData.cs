using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using UnityEngine;


namespace Majic.CM
{
    public class ThreadTask
    {
        public NameValueCollection Parame;
        public Stream Output;
    }
    [Serializable]
    public class SceneDataList
    {
        public List<SceneData> List;
    }
    [Serializable]
    public class SceneData
    {
        public string Name;
        public List<GameObjectData> objectList;
    }
    public class GameObjectChageData
    {
        public List<GameObjectData> data = new List<GameObjectData>();
    }
    [Serializable]
    public class GameObjectData
    {
        public int ID;
        public int ParentID;
        public string Name;
        public bool ActiveSelf;//能改变

        public Vector3 localPosition;//能改变
        public Quaternion quaternion;//能改变
        public Vector3 LocalScale;//能改变
    }

    [Serializable]
    public class CamerasData
    {
        public CameraData[] cameraDataArray;
    }
    [Serializable]
    public class CameraData
    {
        public string name;
        public float fieldOfView = 0;
        public float nearClipPlane = 0;
        public float farClipPlane = 0;
        

        public bool renderPostProcessing = false;
        //public AntialiasingMode antialiasingMode = AntialiasingMode.None;
        public bool stopNaN = false;
        public bool dithering = false;
        public bool renderShadows = false;

        public bool requiresDepthTexture = false;
        public bool requiresOpaqueTexture = false;
        public bool requiresNormalsTexture = false;
        public int renderIndex = 0;
        public bool useOcclusionCulling = false;

        public bool allowHDR = false;
        public bool allowMSAA = false;
    }

    [Serializable]
    public class RenderFeatureData
    {
        public ForwardRenderersData[] renderList;
    }

    [Serializable]
    public class ForwardRenderersData
    {
        public string name;
        public List<BaseRenderersFeature> rendererFeatures = new List<BaseRenderersFeature>();
        public FurRenderFeatureData furRenderFeatureData;
        public FurComputeRenderFeatureData furComputeRenderFeatureData;
        public SSAORenderFeatureData ssaoRenderFeatureData;
    }

    [Serializable]
    public class BaseRenderersFeature
    {
        public string name = "";
        public bool active = false;
    }
    [Serializable]
    public class FurRenderFeatureData: BaseRenderersFeature
    {
        public int PassLayerNum = 0;
    }
    [Serializable]
    public class FurComputeRenderFeatureData : BaseRenderersFeature
    {
        public int PassLayerNum = 0;
    }
    [Serializable]
    public class SSAORenderFeatureData : BaseRenderersFeature
    {
        public int Samples = 0;
    }
    //-----------------------------------------------
    [Serializable]
    public class VolumesData
    {
        public VolumeData[] volumesData;
    }

    [Serializable]
    public class VolumeData
    {
        public string name;
        public bool active = false;
        //public TonemappingData tonemappingData;
        public LayeredBloomData layeredBloomData;
        public VignetteData vignetteData;
        public ColorAdjustmentsData colorAdjustmentsData;
        public ChromaticAberrationData chromaticAberrationData;

        public DepthOfFieldData depthOfFieldData;
        public VolumetricFogData volumetricFogData;

        public LiftGammaGainData liftGammaGainData;
    }
    //[Serializable]
    //public class TonemappingData: BaseRenderersFeature
    //{
    //    public TonemappingMode mode;
    //}
    [Serializable]
    public class LayeredBloomData: BaseRenderersFeature
    {
        public float threshold = 0.5f;
        public Vector2 thresholdRange;

        public float intensity = 0.0f;
        public Vector2 intensityRange;
        public float layer0 =0.02f;
        public Vector2 layer0Range;
        public float layer1 = 0.04f;
        public Vector2 layer1Range;
        public float layer2 = 0.06f;
        public Vector2 layer2Range;
        public float layer3 = 0.08f;
        public Vector2 layer3Range;
    }
    [Serializable]
    public class VignetteData: BaseRenderersFeature
    {
        public Color color;
        public Vector2 center;
        public float intensity = 0;
        public float smoothness = 0.2f;
        public bool rounded = false;
    }
    [Serializable]
    public class ColorAdjustmentsData: BaseRenderersFeature
    {
        public float postExposure = 0;
        public float contrast = 0;
        public Color colorFilter;
        public float hueShift = 0;
        public float saturation = 0;  
    }

    [Serializable]
    public class ChromaticAberrationData: BaseRenderersFeature
    {
        public float intensity = 0;
    }
    [Serializable]
    public class DepthOfFieldData : BaseRenderersFeature
    {
        //public DepthOfFieldMode mode = DepthOfFieldMode.Off;
        public float gaussianStart = 0;
        public float gaussianEnd = 0;
        public float gaussianMaxRadius = 0;
        public bool highQualitySampling = false;
        public float focusDistance = 0;
        public float aperture = 0;
        public float focalLength = 0;
        public int bladeCount = 0;
        public float bladeCurvature = 0;
        public float bladeRotation = 0;
    }
    [Serializable]
    public class VolumetricFogData : BaseRenderersFeature
    {
        public float InnerIntensity = 0;
    }

    [Serializable]
    public class LiftGammaGainData : BaseRenderersFeature
    {

    }

}
