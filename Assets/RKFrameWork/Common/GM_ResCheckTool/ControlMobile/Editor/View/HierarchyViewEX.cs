using UnityEngine;

namespace Majic.CM
{
    public partial class HierarchyView
    {
        //void CreateVolume(GameObject gameObject, GameObjectData data)
        //{
        //    VolumeData volumeData = data.volumeData;
        //    if (volumeData == null || volumeData.active == false)
        //    {
        //        return;
        //    }
        //    VolumeEditorMonoBehaviour volumeEditor = gameObject.AddComponent<VolumeEditorMonoBehaviour>();
        //    string dataStr = JsonUtility.ToJson(volumeData);
        //    volumeEditor.volumeData = JsonUtility.FromJson<VolumeData>(dataStr);
        //}
        bool CheckVolumData(GameObject obj, GameObjectData gameObjectData)
        {
            VolumeEditorMonoBehaviour script = obj.GetComponent<VolumeEditorMonoBehaviour>();
            if(script == null)
            {
                return false;
            }
            return false;
            //bool isChange = false;
            //VolumeData nowVolumeData = script.volumeData;
            //VolumeData baseVolumeData = gameObjectData.volumeData;
            //if(nowVolumeData.tonemappingData != null)
            //{
            //    if(nowVolumeData.tonemappingData.active != baseVolumeData.tonemappingData.active)
            //    {
            //        baseVolumeData.tonemappingData.active = nowVolumeData.tonemappingData.active;
            //        isChange = true;
            //    }
            //    if (nowVolumeData.tonemappingData.mode != baseVolumeData.tonemappingData.mode)
            //    {
            //        baseVolumeData.tonemappingData.mode = nowVolumeData.tonemappingData.mode;
            //        isChange = true;
            //    }
            //}
            //if (nowVolumeData.layeredBloomData != null)
            //{
            //    if (nowVolumeData.layeredBloomData.active != baseVolumeData.layeredBloomData.active)
            //    {
            //        baseVolumeData.layeredBloomData.active = nowVolumeData.layeredBloomData.active;
            //        isChange = true;
            //    }
            //    if (nowVolumeData.layeredBloomData.Threshold != baseVolumeData.layeredBloomData.Threshold)
            //    {
            //        baseVolumeData.layeredBloomData.Threshold = nowVolumeData.layeredBloomData.Threshold;
            //        isChange = true;
            //    }
            //    if (nowVolumeData.layeredBloomData.Intensity != baseVolumeData.layeredBloomData.Intensity)
            //    {
            //        baseVolumeData.layeredBloomData.Intensity = nowVolumeData.layeredBloomData.Intensity;
            //        isChange = true;
            //    }
            //    if (nowVolumeData.layeredBloomData.Layer0 != baseVolumeData.layeredBloomData.Layer0)
            //    {
            //        baseVolumeData.layeredBloomData.Layer0 = nowVolumeData.layeredBloomData.Layer0;
            //        isChange = true;
            //    }
            //    if (nowVolumeData.layeredBloomData.Layer1 != baseVolumeData.layeredBloomData.Layer1)
            //    {
            //        baseVolumeData.layeredBloomData.Layer1 = nowVolumeData.layeredBloomData.Layer1;
            //        isChange = true;
            //    }
            //    if (nowVolumeData.layeredBloomData.Layer2 != baseVolumeData.layeredBloomData.Layer2)
            //    {
            //        baseVolumeData.layeredBloomData.Layer2 = nowVolumeData.layeredBloomData.Layer2;
            //        isChange = true;
            //    }
            //    if (nowVolumeData.layeredBloomData.Layer3 != baseVolumeData.layeredBloomData.Layer3)
            //    {
            //        baseVolumeData.layeredBloomData.Layer3 = nowVolumeData.layeredBloomData.Layer3;
            //        isChange = true;
            //    }
            //}

            //if (nowVolumeData.vignetteData != null)
            //{
            //    if (nowVolumeData.vignetteData.active != baseVolumeData.vignetteData.active)
            //    {
            //        baseVolumeData.vignetteData.active = nowVolumeData.vignetteData.active;
            //        isChange = true;
            //    }
            //    if (nowVolumeData.vignetteData.color != baseVolumeData.vignetteData.color)
            //    {
            //        baseVolumeData.vignetteData.color = nowVolumeData.vignetteData.color;
            //        isChange = true;
            //    }
            //    if (nowVolumeData.vignetteData.center != baseVolumeData.vignetteData.center)
            //    {
            //        baseVolumeData.vignetteData.center = nowVolumeData.vignetteData.center;
            //        isChange = true;
            //    }
            //    if (nowVolumeData.vignetteData.intensity != baseVolumeData.vignetteData.intensity)
            //    {
            //        baseVolumeData.vignetteData.intensity = nowVolumeData.vignetteData.intensity;
            //        isChange = true;
            //    }
            //    if (nowVolumeData.vignetteData.smoothness != baseVolumeData.vignetteData.smoothness)
            //    {
            //        baseVolumeData.vignetteData.smoothness = nowVolumeData.vignetteData.smoothness;
            //        isChange = true;
            //    }
            //    if (nowVolumeData.vignetteData.rounded != baseVolumeData.vignetteData.rounded)
            //    {
            //        baseVolumeData.vignetteData.rounded = nowVolumeData.vignetteData.rounded;
            //        isChange = true;
            //    }
            //}
            //if (nowVolumeData.colorAdjustmentsData != null)
            //{
            //    if (nowVolumeData.colorAdjustmentsData.active != baseVolumeData.colorAdjustmentsData.active)
            //    {
            //        baseVolumeData.colorAdjustmentsData.active = nowVolumeData.colorAdjustmentsData.active;
            //        isChange = true;
            //    }
            //    if (nowVolumeData.colorAdjustmentsData.postExposure != baseVolumeData.colorAdjustmentsData.postExposure)
            //    {
            //        baseVolumeData.colorAdjustmentsData.postExposure = nowVolumeData.colorAdjustmentsData.postExposure;
            //        isChange = true;
            //    }
            //    if (nowVolumeData.colorAdjustmentsData.contrast != baseVolumeData.colorAdjustmentsData.contrast)
            //    {
            //        baseVolumeData.colorAdjustmentsData.contrast = nowVolumeData.colorAdjustmentsData.contrast;
            //        isChange = true;
            //    }
            //    if (nowVolumeData.colorAdjustmentsData.colorFilter != baseVolumeData.colorAdjustmentsData.colorFilter)
            //    {
            //        baseVolumeData.colorAdjustmentsData.colorFilter = nowVolumeData.colorAdjustmentsData.colorFilter;
            //        isChange = true;
            //    }
            //    if (nowVolumeData.colorAdjustmentsData.hueShift != baseVolumeData.colorAdjustmentsData.hueShift)
            //    {
            //        baseVolumeData.colorAdjustmentsData.hueShift = nowVolumeData.colorAdjustmentsData.hueShift;
            //        isChange = true;
            //    }
            //    if (nowVolumeData.colorAdjustmentsData.saturation != baseVolumeData.colorAdjustmentsData.saturation)
            //    {
            //        baseVolumeData.colorAdjustmentsData.saturation = nowVolumeData.colorAdjustmentsData.saturation;
            //        isChange = true;
            //    }
            //}
            //if (nowVolumeData.chromaticAberrationData != null)
            //{
            //    if (nowVolumeData.chromaticAberrationData.active != baseVolumeData.chromaticAberrationData.active)
            //    {
            //        baseVolumeData.chromaticAberrationData.active = nowVolumeData.chromaticAberrationData.active;
            //        isChange = true;
            //    }
            //    if (nowVolumeData.chromaticAberrationData.intensity != baseVolumeData.chromaticAberrationData.intensity)
            //    {
            //        baseVolumeData.chromaticAberrationData.intensity = nowVolumeData.chromaticAberrationData.intensity;
            //        isChange = true;
            //    }
            //}
            ////--------------
            //if (nowVolumeData.depthOfFieldData != null)
            //{
            //    if (nowVolumeData.depthOfFieldData.active != baseVolumeData.depthOfFieldData.active)
            //    {
            //        baseVolumeData.depthOfFieldData.active = nowVolumeData.depthOfFieldData.active;
            //        isChange = true;
            //    }
            //    if (nowVolumeData.depthOfFieldData.mode != baseVolumeData.depthOfFieldData.mode)
            //    {
            //        baseVolumeData.depthOfFieldData.mode = nowVolumeData.depthOfFieldData.mode;
            //        isChange = true;
            //    }
            //    if (nowVolumeData.depthOfFieldData.gaussianStart != baseVolumeData.depthOfFieldData.gaussianStart)
            //    {
            //        baseVolumeData.depthOfFieldData.gaussianStart = nowVolumeData.depthOfFieldData.gaussianStart;
            //        isChange = true;
            //    }
            //    if (nowVolumeData.depthOfFieldData.gaussianEnd != baseVolumeData.depthOfFieldData.gaussianEnd)
            //    {
            //        baseVolumeData.depthOfFieldData.gaussianEnd = nowVolumeData.depthOfFieldData.gaussianEnd;
            //        isChange = true;
            //    }
            //    if (nowVolumeData.depthOfFieldData.gaussianMaxRadius != baseVolumeData.depthOfFieldData.gaussianMaxRadius)
            //    {
            //        baseVolumeData.depthOfFieldData.gaussianMaxRadius = nowVolumeData.depthOfFieldData.gaussianMaxRadius;
            //        isChange = true;
            //    }
            //    if (nowVolumeData.depthOfFieldData.highQualitySampling != baseVolumeData.depthOfFieldData.highQualitySampling)
            //    {
            //        baseVolumeData.depthOfFieldData.highQualitySampling = nowVolumeData.depthOfFieldData.highQualitySampling;
            //        isChange = true;
            //    }
            //    if (nowVolumeData.depthOfFieldData.focusDistance != baseVolumeData.depthOfFieldData.focusDistance)
            //    {
            //        baseVolumeData.depthOfFieldData.focusDistance = nowVolumeData.depthOfFieldData.focusDistance;
            //        isChange = true;
            //    }
            //    if (nowVolumeData.depthOfFieldData.aperture != baseVolumeData.depthOfFieldData.aperture)
            //    {
            //        baseVolumeData.depthOfFieldData.aperture = nowVolumeData.depthOfFieldData.aperture;
            //        isChange = true;
            //    }
            //    if (nowVolumeData.depthOfFieldData.focalLength != baseVolumeData.depthOfFieldData.focalLength)
            //    {
            //        baseVolumeData.depthOfFieldData.focalLength = nowVolumeData.depthOfFieldData.focalLength;
            //        isChange = true;
            //    }
            //    if (nowVolumeData.depthOfFieldData.bladeCount != baseVolumeData.depthOfFieldData.bladeCount)
            //    {
            //        baseVolumeData.depthOfFieldData.bladeCount = nowVolumeData.depthOfFieldData.bladeCount;
            //        isChange = true;
            //    }
            //    if (nowVolumeData.depthOfFieldData.bladeCurvature != baseVolumeData.depthOfFieldData.bladeCurvature)
            //    {
            //        baseVolumeData.depthOfFieldData.bladeCurvature = nowVolumeData.depthOfFieldData.bladeCurvature;
            //        isChange = true;
            //    }
            //    if (nowVolumeData.depthOfFieldData.bladeRotation != baseVolumeData.depthOfFieldData.bladeRotation)
            //    {
            //        baseVolumeData.depthOfFieldData.bladeRotation = nowVolumeData.depthOfFieldData.bladeRotation;
            //        isChange = true;
            //    }
            //}

            //if (nowVolumeData.volumetricFogData != null)
            //{
            //    if (nowVolumeData.volumetricFogData.active != baseVolumeData.volumetricFogData.active)
            //    {
            //        baseVolumeData.volumetricFogData.active = nowVolumeData.volumetricFogData.active;
            //        isChange = true;
            //    }
            //    if (nowVolumeData.volumetricFogData.InnerIntensity != baseVolumeData.volumetricFogData.InnerIntensity)
            //    {
            //        baseVolumeData.volumetricFogData.InnerIntensity = nowVolumeData.volumetricFogData.InnerIntensity;
            //        isChange = true;
            //    }
            //}
            //if (nowVolumeData.liftGammaGainData != null)
            //{
            //    if (nowVolumeData.liftGammaGainData.active != baseVolumeData.liftGammaGainData.active)
            //    {
            //        baseVolumeData.liftGammaGainData.active = nowVolumeData.liftGammaGainData.active;
            //        isChange = true;
            //    }
            //}
            //return isChange;
        }
    }
}

