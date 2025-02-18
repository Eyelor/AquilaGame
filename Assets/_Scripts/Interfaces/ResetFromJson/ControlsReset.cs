using System.Globalization;
using TMPro;
using UnityEngine;

public class ControlsReset : MonoBehaviour
{
    // Komponenty TextMeshPro przypisane z Inspektora
    public TextMeshProUGUI mouseSensitivity;    // Tekst dla wyœwietlania czu³oœci myszy
    public TextMeshProUGUI autoReload;    // Tekst dla wyœwietlania informacji o automatycznym prze³adowaniu 
    public TextMeshProUGUI runMode;    // Tekst dla wyœwietlania informacji o trybie biegu

    // Klasa zarz¹dzaj¹ca ustawieniami
    private SettingsDataSerialization settingsManager;

    void Start()
    {
        // Znalezienie SettingsDataSerialization i zaktualizowanie UI na podstawie obecnych ustawieñ
        settingsManager = FindObjectOfType<SettingsDataSerialization>();
    }

    private void OnMouseDown()
    {
        if (!AudioSystem.Instance.audioSources.effectsAudioSource.isPlaying)
        {
            AudioSystem.Instance.PlaySound(AudioSystem.Instance.audioSources.effectsAudioSource, AudioSystem.Instance.buttonForwardSound, false);
        }
        // Debug.Log("ControlsReset");
        // Wyœwietlanie informacji o trybie biegu
        runMode.text = settingsManager.settingsData.defaults.controlsDefault.runMode;

        // Wyœwietlanie czu³oœci myszy z kropk¹ jako separator dziesiêtny
        mouseSensitivity.text = settingsManager.settingsData.defaults.controlsDefault.mouseSensitivity.ToString("F1", CultureInfo.InvariantCulture);

        // Wyœwietlanie informacji o automatycznym prze³adowaniu 
        //autoReload.text = settingsManager.settingsData.defaults.controlsDefault.autoReload ? "TAK" : "NIE";

    }
}
