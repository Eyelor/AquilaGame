using System.IO;
using UnityEngine;

public class SettingsDataSerialization : MonoBehaviour
{
    public SettingsData settingsData;
    private string jsonFilePath;

    private void Awake()
    {
        jsonFilePath = Path.Combine(Application.persistentDataPath, "", "settings.json");
        LoadSettings();
    }

    public void LoadSettings()
    {
        if (File.Exists(jsonFilePath))
        {
            string json = File.ReadAllText(jsonFilePath);
            settingsData = JsonUtility.FromJson<SettingsData>(json);
        }
        else
        {
            Debug.LogWarning("Settings file not found, creating default settings.");
            OptionsDefaultSettingsSO defaultSettings = ScriptableObject.CreateInstance<OptionsDefaultSettingsSO>();
            DefaultSettings defaults = defaultSettings.GetDefaultSettings();
            // Mo¿esz tu zainicjowaæ domyœlne wartoœci
            settingsData = new SettingsData
            {
                controls = defaults.controlsDefault,
                keys = defaults.keysDefault,
                graphics = defaults.graphicsDefault,
                sound = defaults.soundDefault,
                display = defaults.displayDefault,
                language = defaults.languageDefault,
                defaults = defaults
            };  
        }
    }

    public void SaveSettings()
    {
        string json = JsonUtility.ToJson(settingsData, true);
        File.WriteAllText(jsonFilePath, json);
    }
}
