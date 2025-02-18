using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class DisplayReset : MonoBehaviour
{
    // Komponenty TextMeshPro przypisane z Inspektora
    public TextMeshProUGUI resolution;    // Tekst dla informacji o rozdzielczoœci
    public TextMeshProUGUI refreshRate;    // Tekst dla informacji o czêstotliwoœci odœwie¿ania
    public TextMeshProUGUI brightness; // Tekst dla informacji o jasnoœci
    public TextMeshProUGUI contrast;    // Tekst dla informacji o kontraœcie
    public TextMeshProUGUI gamma;    // Tekst dla informacji o gamma
    public TextMeshProUGUI displayGuides; // Tekst dla informacji czy wyœwietlaæ samouczek
    public TextMeshProUGUI displayHints;    // Tekst dla informacji czy wyœwietlaæ podpowiedzi

    // Klasa zarz¹dzaj¹ca ustawieniami
    private SettingsDataSerialization settingsManager;
    public ResolutionsController resolutionsController; // Referencja do ResolutionsController

    void Start()
    {
        // Znalezienie SettingsDataSerialization i zaktualizowanie UI na podstawie obecnych ustawieñ
        settingsManager = FindObjectOfType<SettingsDataSerialization>();
        if (resolutionsController == null)
        {
            // Próba znalezienia ResolutionsController w scenie
            resolutionsController = FindObjectOfType<ResolutionsController>();
        }
    }

    private void OnMouseDown()
    {
        if (!AudioSystem.Instance.audioSources.effectsAudioSource.isPlaying)
        {
            AudioSystem.Instance.PlaySound(AudioSystem.Instance.audioSources.effectsAudioSource, AudioSystem.Instance.buttonForwardSound, false);
        }
        // Debug.Log("DisplayReset");
        // Wyœwietlenie informacji o rozdzielczoœci
        if (resolutionsController != null)
        {
            resolutionsController.SetSelectedResolution(settingsManager.settingsData.defaults.displayDefault.resolution);
        }

        // Wyœwietlanie informacji o czêstotliwoœci odœwie¿ania
        refreshRate.text = (settingsManager.settingsData.defaults.displayDefault.refreshRate).ToString("F0") + " Hz";

        // Wyœwietlanie informacji o jasnoœci
        brightness.text = (settingsManager.settingsData.defaults.displayDefault.brightness * 100).ToString("F0") + "%";

        // Wyœwietlanie informacji o kontraœcie
        contrast.text = (settingsManager.settingsData.defaults.displayDefault.contrast * 100).ToString("F0") + "%";

        // Wyœwietlanie informacji o gamma
        gamma.text = (settingsManager.settingsData.defaults.displayDefault.gamma * 100).ToString("F0") + "%";

        // Wyœwietlenie informacji czy wyœwietlaæ samouczek
        displayGuides.text = settingsManager.settingsData.defaults.displayDefault.displayGuides ? "TAK" : "NIE";

        // Wyœwietlenie informacji czy wyœwietlaæ podpowiedzi 
        displayHints.text = settingsManager.settingsData.defaults.displayDefault.displayHints ? "TAK" : "NIE";

    }
}
