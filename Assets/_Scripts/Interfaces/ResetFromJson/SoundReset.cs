using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SoundReset : MonoBehaviour
{
    // Komponenty TextMeshPro przypisane z Inspektora
    public TextMeshProUGUI audioDevice;    // Tekst dla wy�wietlania urz�dzenia audio
    public TextMeshProUGUI musicVolume;    // Tekst dla wy�wietlania g�o�no�ci muzyki (%)
    public TextMeshProUGUI recordsVolume;    // Tekst dla wy�wietlania g�o�no�ci nagra� (%)
    public TextMeshProUGUI effectsVolume;    // Tekst dla wy�wietlania g�o�no�ci efekt�w (%)

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
        // Debug.Log("SoundReset");
        // Wy�wietlanie urz�dzenia audio
        audioDevice.text = settingsManager.settingsData.defaults.soundDefault.audioDevice;

        // Wy�wietlanie g�o�no�ci muzyki (%)
        musicVolume.text = (settingsManager.settingsData.defaults.soundDefault.musicVolume * 100).ToString("F0") + "%";

        // Wy�wietlanie g�o�no�ci nagra� (%)
        recordsVolume.text = (settingsManager.settingsData.defaults.soundDefault.recordsVolume * 100).ToString("F0") + "%";

        // Wy�wietlanie g�o�no�ci efekt�w (%)
        effectsVolume.text = (settingsManager.settingsData.defaults.soundDefault.effectsVolume * 100).ToString("F0") + "%";
    }
}
