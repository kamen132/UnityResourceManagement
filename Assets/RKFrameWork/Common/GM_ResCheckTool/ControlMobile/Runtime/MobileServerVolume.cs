#if USE_URP
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
namespace Majic.CM
{
    public partial class MobileServer
    {
        private Volume[] m_volumes;
        VolumesData GetVolumesData()
        {
            m_volumes = FindObjectsOfType<Volume>();

            VolumesData volumesData = new VolumesData();
            volumesData.volumesData = new VolumeData[m_volumes.Length];
            for(int i = 0;i < m_volumes.Length;i++ )
            {
                volumesData.volumesData[i] = OnGetVolumeData(m_volumes[i]);
            }
            return volumesData;
        }

        VolumeData OnGetVolumeData(Volume volume)
        {
            if (volume == null)
            {
                return null;
            }
            VolumeData volumeData = new VolumeData();
            GameObject obj = volume.gameObject;
            volumeData.name = obj.name;
            volumeData.active = obj.activeSelf;
            VolumeProfile profileRef = volume.HasInstantiatedProfile() ? volume.profile : volume.sharedProfile;
            if(profileRef == null)
            {
                return null;
            }
            List<VolumeComponent> components = profileRef.components;
            foreach (VolumeComponent component in components)
            {
                GetTonemappingData(component, volumeData);
                GetLayeredBloomData(component, volumeData);
                GetVignetteBloomData(component, volumeData);
                GetColorAdjustmentsData(component, volumeData);
                GetChromaticAberrationData(component, volumeData);

                GetDepthOfFieldData(component, volumeData);
                GetVolumetricFogData(component, volumeData);
                GetLiftGammaGainData(component, volumeData);
            }
            return volumeData;
        }
        void GetTonemappingData(VolumeComponent component, VolumeData volumeData)
        {
            Tonemapping data = component as Tonemapping;
            if(data == null)
            {
                return;
            }
            TonemappingData tonemappingData = new TonemappingData();
            tonemappingData.active = data.active;
            tonemappingData.name = data.name;
            tonemappingData.mode = data.mode.value;
            volumeData.tonemappingData = tonemappingData;
        }
        void GetLayeredBloomData(VolumeComponent component, VolumeData volumeData)
        {
            LayeredBloom data = component as LayeredBloom;
            if (data == null)
            {
                return;
            }
            LayeredBloomData infoData = new LayeredBloomData();
            infoData.name = data.name;
            infoData.active = data.active;
            infoData.threshold = data.Threshold.value;
            infoData.thresholdRange = new Vector2(data.Threshold.min, data.Threshold.max);
            infoData.intensity = data.Intensity.value;
            infoData.intensityRange = new Vector2(data.Intensity.min, data.Intensity.max);
            infoData.layer0 = data.Layer0.value;
            infoData.layer0Range = new Vector2(data.Layer0.min, data.Layer0.max);
            infoData.layer1 = data.Layer1.value;
            infoData.layer1Range = new Vector2(data.Layer1.min, data.Layer1.max);
            infoData.layer2 = data.Layer2.value;
            infoData.layer2Range = new Vector2(data.Layer2.min, data.Layer2.max);
            infoData.layer3 = data.Layer3.value;
            infoData.layer3Range = new Vector2(data.Layer3.min, data.Layer3.max);
            volumeData.layeredBloomData = infoData;
        }
        void GetVignetteBloomData(VolumeComponent component, VolumeData volumeData)
        {
            Vignette data = component as Vignette;
            if (data == null)
            {
                return;
            }
            VignetteData infoData = new VignetteData();
            infoData.name = data.name;
            infoData.active = data.active;
            infoData.color = data.color.value;
            infoData.center = data.center.value;
            infoData.intensity = data.intensity.value;
            infoData.smoothness = data.smoothness.value;
            infoData.rounded = data.rounded.value;
            volumeData.vignetteData = infoData;
        }
        void GetColorAdjustmentsData(VolumeComponent component, VolumeData volumeData)
        {
            ColorAdjustments data = component as ColorAdjustments;
            if (data == null)
            {
                return;
            }
            ColorAdjustmentsData infoData = new ColorAdjustmentsData();
            infoData.name = data.name;
            infoData.active = data.active;
            infoData.postExposure = data.postExposure.value;
            infoData.contrast = data.contrast.value;
            infoData.colorFilter = data.colorFilter.value;
            infoData.hueShift = data.hueShift.value;
            infoData.saturation = data.saturation.value;
            volumeData.colorAdjustmentsData = infoData;
        }
        void GetChromaticAberrationData(VolumeComponent component, VolumeData volumeData)
        {
            ChromaticAberration data = component as ChromaticAberration;
            if (data == null)
            {
                return;
            }
            ChromaticAberrationData infoData = new ChromaticAberrationData();
            infoData.name = data.name;
            infoData.active = data.active;
            infoData.intensity = data.intensity.value;
            volumeData.chromaticAberrationData = infoData;
        }
        void GetDepthOfFieldData(VolumeComponent component, VolumeData volumeData)
        {
            DepthOfField data = component as DepthOfField;
            if (data == null)
            {
                return;
            }
            DepthOfFieldData infoData = new DepthOfFieldData();
            infoData.active = data.active;
            infoData.name = data.name;
            infoData.mode = data.mode.value;
            infoData.gaussianStart = data.gaussianStart.value;
            infoData.gaussianEnd = data.gaussianEnd.value;
            infoData.gaussianMaxRadius = data.gaussianMaxRadius.value;
            infoData.highQualitySampling = data.highQualitySampling.value;
            infoData.focusDistance = data.focusDistance.value;
            infoData.aperture = data.aperture.value;

            infoData.focalLength = data.focalLength.value;
            infoData.bladeCount = data.bladeCount.value;
            infoData.bladeCurvature = data.bladeCurvature.value;
            infoData.bladeRotation = data.bladeRotation.value;
            
            volumeData.depthOfFieldData = infoData;
        }
        void GetVolumetricFogData(VolumeComponent component, VolumeData volumeData)
        {
            VolumetricFog data = component as VolumetricFog;
            if (data == null)
            {
                return;
            }
            VolumetricFogData infoData = new VolumetricFogData();
            infoData.name = data.name;
            infoData.active = data.active;
            infoData.InnerIntensity = data.InnerIntensity.value;
            volumeData.volumetricFogData = infoData;
        }
        void GetLiftGammaGainData(VolumeComponent component, VolumeData volumeData)
        {
            LiftGammaGain data = component as LiftGammaGain;
            if (data == null)
            {
                return;
            }
            LiftGammaGainData infoData = new LiftGammaGainData();
            infoData.active = data.active;
            infoData.name = data.name;
            volumeData.liftGammaGainData = infoData;
        }

        //-----------------------------
        void ChangeVolumesData(ThreadTask task)
        {
        
                //Volume volume = obj.GetComponent<Volume>();
                //if (volume == null)
                //{
                //    continue;
                //}

                //VolumeProfile profileRef = volume.HasInstantiatedProfile() ? volume.profile : volume.sharedProfile;
                //List<VolumeComponent> components = profileRef.components;
                //foreach (VolumeComponent component in components)
                //{
                //    UpdateTonemappingData(component, volumeData);
                //    UpdateLayeredBloomData(component, volumeData);
                //    UpdateVignetteBloomData(component, volumeData);
                //    UpdateColorAdjustmentsData(component, volumeData);
                //    UpdateChromaticAberrationData(component, volumeData);

                //    UpdateDepthOfFieldData(component, volumeData);
                //    UpdateVolumetricFogData(component, volumeData);
                //    UpdateLiftGammaGainData(component, volumeData);
                //}
            
        }
        void UpdateTonemappingData(VolumeComponent component, VolumeData volumeData)
        {
            Tonemapping data = component as Tonemapping;
            if (data == null)
            {
                return;
            }
            data.active = volumeData.tonemappingData.active;
            data.mode.value = volumeData.tonemappingData.mode;
        }
        void UpdateLayeredBloomData(VolumeComponent component, VolumeData volumeData)
        {
            LayeredBloom data = component as LayeredBloom;
            if (data == null)
            {
                return;
            }
            data.active = volumeData.layeredBloomData.active;
            data.Threshold.value = volumeData.layeredBloomData.threshold;
        }
        void UpdateVignetteBloomData(VolumeComponent component, VolumeData volumeData)
        {
            Vignette data = component as Vignette;
            if (data == null)
            {
                return;
            }
            data.active = volumeData.vignetteData.active;
            data.color.value = volumeData.vignetteData.color;
            data.center.value = volumeData.vignetteData.center;
            data.intensity.value = volumeData.vignetteData.intensity;
            data.smoothness.value = volumeData.vignetteData.smoothness;
            data.rounded.value = volumeData.vignetteData.rounded;
        }
        void UpdateColorAdjustmentsData(VolumeComponent component, VolumeData volumeData)
        {
            ColorAdjustments data = component as ColorAdjustments;
            if (data == null)
            {
                return;
            }
            data.active = volumeData.colorAdjustmentsData.active;
            data.postExposure.value = volumeData.colorAdjustmentsData.postExposure;
            data.contrast.value = volumeData.colorAdjustmentsData.contrast;
            data.colorFilter.value = volumeData.colorAdjustmentsData.colorFilter;
            data.hueShift.value = volumeData.colorAdjustmentsData.hueShift;
            data.saturation.value = volumeData.colorAdjustmentsData.saturation;
        }
        void UpdateChromaticAberrationData(VolumeComponent component, VolumeData volumeData)
        {
            ChromaticAberration data = component as ChromaticAberration;
            if (data == null)
            {
                return;
            }
            data.active = volumeData.chromaticAberrationData.active;
            data.intensity.value = volumeData.chromaticAberrationData.intensity;
        }
        void UpdateDepthOfFieldData(VolumeComponent component, VolumeData volumeData)
        {
            DepthOfField data = component as DepthOfField;
            if (data == null)
            {
                return;
            }
            data.active = volumeData.depthOfFieldData.active;

            data.name = volumeData.depthOfFieldData.name;
            data.mode.value = volumeData.depthOfFieldData.mode;

            data.gaussianStart.value = volumeData.depthOfFieldData.gaussianStart;
            data.gaussianEnd.value = volumeData.depthOfFieldData.gaussianEnd;
            data.gaussianMaxRadius.value = volumeData.depthOfFieldData.gaussianMaxRadius;
            data.highQualitySampling.value = volumeData.depthOfFieldData.highQualitySampling;
            data.focusDistance.value = volumeData.depthOfFieldData.focusDistance;
            data.aperture.value = volumeData.depthOfFieldData.aperture;

            data.focalLength.value = volumeData.depthOfFieldData.focalLength;
            data.bladeCount.value = volumeData.depthOfFieldData.bladeCount;
            data.bladeCurvature.value = volumeData.depthOfFieldData.bladeCurvature;
            data.bladeRotation.value = volumeData.depthOfFieldData.bladeRotation;
        }
        void UpdateVolumetricFogData(VolumeComponent component, VolumeData volumeData)
        {
            VolumetricFog data = component as VolumetricFog;
            if (data == null)
            {
                return;
            }
            data.InnerIntensity.value = volumeData.volumetricFogData.InnerIntensity;
            data.active = volumeData.volumetricFogData.active;
        }
        void UpdateLiftGammaGainData(VolumeComponent component, VolumeData volumeData)
        {
            LiftGammaGain data = component as LiftGammaGain;
            if (data == null)
            {
                return;
            }
            data.active = volumeData.liftGammaGainData.active;
        }
    }
}
#endif
