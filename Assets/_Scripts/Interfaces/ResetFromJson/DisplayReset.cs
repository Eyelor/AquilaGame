using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class DisplayReset : MonoBehaviour
{
    // Komponenty TextMeshPro przypisane z Inspektora
    public TextMeshProUGUI resolution;    // Tekst dla informacji o rozdzielczo�ci
    public TextMeshProUGUI refreshRate;    // Tekst dla informacji o cz�stotliwo�ci od�wie�ania
    public TextMeshProUGUI brightness; // Tekst dla informacji o jasno�ci
    public TextMeshProUGUI contrast;    // Tekst dla informacji o kontra�cie
    public TextMeshProUGUI gamma;    // Tekst dla informacji o gamma
    public TextMeshProUGUI displayGuides; // Tekst dla informacji czy wy�wietla� samouczek
    public TextMeshProUGUI displayHints;    // Tekst dla informacji czy wy�wietla� podpowiedzi

    // Klasa zarz�dzaj�ca ustawieniami
    private SettingsDataSerialization settingsManager;
    public ResolutionsController resolutionsController; // Referencja do ResolutionsController

    void Start()
    {
        // Znalezienie SettingsDataSerialization i zaktualizowanie UI na podstawie obecnych ustawie�
        settingsManager = FindObjectOfType<SettingsDataSerialization>();
        if (resolutionsController == null)
        {
            // Pr�ba znalezienia ResolutionsController w scenie
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
        // Wy�wietlenie informacji o rozdzielczo�ci
        if (resolutionsController != null)
        {
            resolutionsController.SetSelectedResolution(settingsManager.settingsData.defaults.displayDefault.resolution);
        }

        // Wy�wietlanie informacji o cz�stotliwo�ci od�wie�ania
        refreshRate.text = (settingsManager.settingsData.defaults.displayDefault.refreshRate).ToString("F0") + " Hz";

        // Wy�wietlanie informacji o jasno�ci
        brightness.text = (settingsManager.settingsData.defaults.displayDefault.brightness * 100).ToString("F0") + "%";

        // Wy�wietlanie informacji o kontra�cie
        contrast.text = (settingsManager.settingsData.defaults.displayDefault.contrast * 100).ToString("F0") + "%";

        // Wy�wietlanie informacji o gamma
        gamma.text = (settingsManager.settingsData.defaults.displayDefault.gamma * 100).ToString("F0") + "%";

        // Wy�wietlenie informacji czy wy�wietla� samouczek
        displayGuides.text = settingsManager.settingsData.defaults.displayDefault.displayGuides ? "TAK" : "NIE";

        // Wy�wietlenie informacji czy wy�wietla� podpowiedzi 
        displayHints.text = settingsManager.settingsData.defaults.displayDefault.displayHints ? "TAK" : "NIE";

    }
}
