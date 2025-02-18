using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SoundReset : MonoBehaviour
{
    // Komponenty TextMeshPro przypisane z Inspektora
    public TextMeshProUGUI audioDevice;    // Tekst dla wyœwietlania urz¹dzenia audio
    public TextMeshProUGUI musicVolume;    // Tekst dla wyœwietlania g³oœnoœci muzyki (%)
    public TextMeshProUGUI recordsVolume;    // Tekst dla wyœwietlania g³oœnoœci nagrañ (%)
    public TextMeshProUGUI effectsVolume;    // Tekst dla wyœwietlania g³oœnoœci efektów (%)

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
        // Debug.Log("SoundReset");
        // Wyœwietlanie urz¹dzenia audio
        audioDevice.text = settingsManager.settingsData.defaults.soundDefault.audioDevice;

        // Wyœwietlanie g³oœnoœci muzyki (%)
        musicVolume.text = (settingsManager.settingsData.defaults.soundDefault.musicVolume * 100).ToString("F0") + "%";

        // Wyœwietlanie g³oœnoœci nagrañ (%)
        recordsVolume.text = (settingsManager.settingsData.defaults.soundDefault.recordsVolume * 100).ToString("F0") + "%";

        // Wyœwietlanie g³oœnoœci efektów (%)
        effectsVolume.text = (settingsManager.settingsData.defaults.soundDefault.effectsVolume * 100).ToString("F0") + "%";
    }
}
