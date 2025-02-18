using UnityEngine;

public class FrameRateSystem : PersistentSingleton<FrameRateSystem>
{
    private void Start()
    {
        UpdateFrameRate();
    }

    public void UpdateFrameRate()
    {
        var setteingsData = SaveManager.Instance.LoadSettingsData();

        if (setteingsData != null)
        {
            // Apply from settings
            if (setteingsData.graphics.verticalSync)
            {
                // VSync
                QualitySettings.vSyncCount = 1;
                Application.targetFrameRate = -1;
            }
            else if (setteingsData.display.refreshRate == -1)
            {
                // Unlimited
                QualitySettings.vSyncCount = 0;
                Application.targetFrameRate = -1;
            }
            else
            {
                // Custom
                QualitySettings.vSyncCount = 0;
                Application.targetFrameRate = setteingsData.display.refreshRate;
            }
        }
        else
        {
            // VSync
            QualitySettings.vSyncCount = 1;
            Application.targetFrameRate = -1;
        }

        Debug.Log($"[FrameRateSystem] VSync is {QualitySettings.vSyncCount}. Frame rate updated to {Application.targetFrameRate} Hz.");
    }
}
