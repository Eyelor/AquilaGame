using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Rendering;

public class PostProcessingSystem : PersistentSingleton<PostProcessingSystem>
{
    [SerializeField] private VolumeProfile volumeProfile;

    private void Start()
    {
        UpdatePostProcessing();
    }

    public void UpdatePostProcessing()
    {
        if (volumeProfile != null)
        {
            //UpdateColorAdjustments();
            //UpdateGraphicsQuality();
        }
        else
        {
            Debug.LogError("Volume Profile is not assigned!");
        }
    }

    private void UpdateColorAdjustments()
    {
        ColorAdjustments colorAdjustments = GetColorAdjustments();
        // Ensure the Volume Profile has a ColorAdjustments override
        if (colorAdjustments != null)
        {
            var setteingsData = SaveManager.Instance.LoadSettingsData();

            if (setteingsData != null)
            {
                // Apply from settings
                if (setteingsData.display.brightness >= -0.0f && setteingsData.display.brightness <= 1.0f)
                {
                    // Brightness from settings
                    SetBrightness(setteingsData.display.brightness, colorAdjustments);
                }
                else
                {
                    // Set default value for brightness
                    SetBrightness(0.5f, colorAdjustments);
                }

                if (setteingsData.display.contrast >= -0.0f && setteingsData.display.contrast <= 1.0f)
                {
                    // Brightness from settings
                    SetContrast(setteingsData.display.contrast, colorAdjustments);
                }
                else
                {
                    // Set default value for contrast
                    SetContrast(0.5f, colorAdjustments);
                }
            }
            else
            {
                // Set default values for brightness and contrast
                SetBrightness(0.5f, colorAdjustments);
                SetContrast(0.5f, colorAdjustments);
            }
        }
        else
        {
            Debug.LogWarning("The Volume Profile does not contain a ColorAdjustments override!");
        }
    }

    //private void UpdateGraphicsQuality()
    //{
    //    var settingsData = SaveManager.Instance.LoadSettingsData();

    //    if (settingsData != null)
    //    {
    //        SetTextureQuality(settingsData.graphics.textureQuality);
    //        SetEffectsQuality(settingsData.graphics.effectsQuality);
    //        SetReflectionsQuality(settingsData.graphics.reflectionsQuality);
    //        SetShadowsQuality(settingsData.graphics.shadowsQuality);
    //        SetMeshQuality(settingsData.graphics.meshQuality);

    //        // Toggle features
    //        SetShadowBuffer(settingsData.graphics.shadowBuffer);
    //        SetFlare(settingsData.graphics.flare);
    //        SetContactShadows(settingsData.graphics.contactShadows);
    //        SetSpatialReflections(settingsData.graphics.spatialReflections);
    //    }
    //    else
    //    {
    //        Debug.LogWarning("Graphics settings data not found!");
    //    }
    //}

    //private void SetTextureQuality(string quality)
    //{
    //    switch (quality.ToUpper())
    //    {
    //        case "NISKA":
    //            QualitySettings.masterTextureLimit = 2; // Low texture quality
    //            break;
    //        case "ŒREDNIA":
    //            QualitySettings.masterTextureLimit = 1; // Medium texture quality
    //            break;
    //        case "WYSOKA":
    //            QualitySettings.masterTextureLimit = 0; // High texture quality
    //            break;
    //        default:
    //            Debug.LogWarning("Invalid texture quality setting!");
    //            break;
    //    }

    //    Debug.Log($"Texture quality set to {quality.ToUpper()} (globalTextureMipmapLimit = {QualitySettings.globalTextureMipmapLimit})");
    //}

    //private void SetEffectsQuality(string quality)
    //{
    //    switch (quality.ToUpper())
    //    {
    //        case "NISKA":
    //            QualitySettings.particleRaycastBudget = 128;
    //            break;
    //        case "ŒREDNIA":
    //            QualitySettings.particleRaycastBudget = 512;
    //            break;
    //        case "WYSOKA":
    //            QualitySettings.particleRaycastBudget = 1024;
    //            break;
    //        default:
    //            Debug.LogWarning("Invalid effects quality setting!");
    //            break;
    //    }

    //    Debug.Log($"Effects quality set to {quality.ToUpper()} (particleRaycastBudget = {QualitySettings.particleRaycastBudget})");
    //}

    //private void SetReflectionsQuality(string quality)
    //{
    //    switch (quality.ToUpper())
    //    {
    //        case "NISKA":
    //            HDRenderPipelineGlobalSettings.instance.lightLoopSettings.reflectionProbeCacheResolution = 64;
    //            break;
    //        case "ŒREDNIA":
    //            HDRenderPipelineGlobalSettings.instance.lightLoopSettings.reflectionProbeCacheResolution = 128;
    //            break;
    //        case "WYSOKA":
    //            HDRenderPipelineGlobalSettings.instance.lightLoopSettings.reflectionProbeCacheResolution = 256;
    //            break;
    //        default:
    //            Debug.LogWarning("Invalid reflections quality setting!");
    //            break;
    //    }
    //}

    //private void SetShadowsQuality(string quality)
    //{
    //    switch (quality.ToUpper())
    //    {
    //        case "NISKA":
    //            HDRenderPipeline.currentPipeline.shadowSettings.shadowResolution = 512;
    //            break;
    //        case "ŒREDNIA":
    //            HDRenderPipeline.currentPipeline.shadowSettings.shadowResolution = 1024;
    //            break;
    //        case "WYSOKA":
    //            HDRenderPipeline.currentPipeline.shadowSettings.shadowResolution = 2048;
    //            break;
    //        default:
    //            Debug.LogWarning("Invalid shadows quality setting!");
    //            break;
    //    }
    //}

    //private void SetMeshQuality(string quality)
    //{
    //    // Adjust mesh LOD bias or other settings based on quality
    //    switch (quality.ToUpper())
    //    {
    //        case "NISKA":
    //            QualitySettings.lodBias = 0.5f;
    //            break;
    //        case "ŒREDNIA":
    //            QualitySettings.lodBias = 1.0f;
    //            break;
    //        case "WYSOKA":
    //            QualitySettings.lodBias = 2.0f;
    //            break;
    //        default:
    //            Debug.LogWarning("Invalid mesh quality setting!");
    //            break;
    //    }
    //}

    //private void SetShadowBuffer(bool enabled)
    //{
    //    HDRenderPipeline.currentPipeline.shadowSettings.enableScreenSpaceShadows = enabled;
    //}

    //private void SetFlare(bool enabled)
    //{
    //    LensFlareCommonSRP.Instance.enabled = enabled;
    //}

    //private void SetContactShadows(bool enabled)
    //{
    //    HDRenderPipeline.currentPipeline.contactShadows.enable = enabled;
    //}

    //private void SetSpatialReflections(bool enabled)
    //{
    //    HDRenderPipeline.currentPipeline.screenSpaceReflection.enable = enabled;
    //}

    private ColorAdjustments GetColorAdjustments()
    {
        if (volumeProfile.TryGet(out ColorAdjustments colorAdjustments))
        {
            return colorAdjustments;
        }

        return null;
    }

    private void SetBrightness(float value, ColorAdjustments colorAdjustments)
    {
        if (colorAdjustments != null)
        {
            colorAdjustments.postExposure.value = Mathf.Lerp(-5.0f, 5.0f, value);
            Debug.Log($"[PostProcessingSystem] Brightness set to {colorAdjustments.postExposure.value}.");
        }
    }

    private void SetContrast(float value, ColorAdjustments colorAdjustments)
    {
        if (colorAdjustments != null)
        {
            colorAdjustments.contrast.value = Mathf.Lerp(-100.0f, 100.0f, value);
            Debug.Log($"[PostProcessingSystem] Contrast set to {colorAdjustments.contrast.value}.");
        }
    }
}
