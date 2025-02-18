using System.Globalization;
using TMPro;
using UnityEngine;

public class ControlsReset : MonoBehaviour
{
    // Komponenty TextMeshPro przypisane z Inspektora
    public TextMeshProUGUI mouseSensitivity;    // Tekst dla wy�wietlania czu�o�ci myszy
    public TextMeshProUGUI autoReload;    // Tekst dla wy�wietlania informacji o automatycznym prze�adowaniu 
    public TextMeshProUGUI runMode;    // Tekst dla wy�wietlania informacji o trybie biegu

    // Klasa zarz�dzaj�ca ustawieniami
    private SettingsDataSerialization settingsManager;

    void Start()
    {
        // Znalezienie SettingsDataSerialization i zaktualizowanie UI na podstawie obecnych ustawie�
        settingsManager = FindObjectOfType<SettingsDataSerialization>();
    }

    private void OnMouseDown()
    {
        if (!AudioSystem.Instance.audioSources.effectsAudioSource.isPlaying)
        {
            AudioSystem.Instance.PlaySound(AudioSystem.Instance.audioSources.effectsAudioSource, AudioSystem.Instance.buttonForwardSound, false);
        }
        // Debug.Log("ControlsReset");
        // Wy�wietlanie informacji o trybie biegu
        runMode.text = settingsManager.settingsData.defaults.controlsDefault.runMode;

        // Wy�wietlanie czu�o�ci myszy z kropk� jako separator dziesi�tny
        mouseSensitivity.text = settingsManager.settingsData.defaults.controlsDefault.mouseSensitivity.ToString("F1", CultureInfo.InvariantCulture);

        // Wy�wietlanie informacji o automatycznym prze�adowaniu 
        //autoReload.text = settingsManager.settingsData.defaults.controlsDefault.autoReload ? "TAK" : "NIE";

    }
}
