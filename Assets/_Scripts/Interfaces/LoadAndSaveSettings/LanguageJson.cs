using TMPro;
using UnityEngine;

public class LanguageJson : MonoBehaviour
{
    // Komponenty TextMeshPro przypisane z Inspektora
    public TextMeshProUGUI textsLanguage;    // Tekst dla wyœwietlania jêzyka napisów
    public TextMeshProUGUI audioLanguage;    // Tekst dla wyœwietlania jêzyka nagrañ 
    public TextMeshProUGUI displaySubtitles; // Tekst dla informacji czy wyœwietlaæ napisy

    // Klasa zarz¹dzaj¹ca ustawieniami
    private SettingsDataSerialization settingsManager;

    private void Start()
    {
        // Znalezienie SettingsDataSerialization i zaktualizowanie UI na podstawie obecnych ustawieñ
        settingsManager = FindObjectOfType<SettingsDataSerialization>();
        UpdateLanguageUI();
    }

    private void OnDisable()
    {
        // Zapisanie zmian w ustawieniach do pliku JSON
        SaveLanguageSettings();
    }

    // Aktualizuje interfejs UI wartoœciami z ustawieñ
    void UpdateLanguageUI()
    {
        // Wyœwietlanie jêzyka napisów
        textsLanguage.text = settingsManager.settingsData.language.textsLanguage;

        // Wyœwietlanie jêzyka nagrañ
        audioLanguage.text = settingsManager.settingsData.language.audioLanguage;

        // Wyœwietlenie informacji czy wyœwietlaæ napisy ("TAK" lub "NIE")
        displaySubtitles.text = settingsManager.settingsData.language.displaySubtitles ? "TAK" : "NIE";
    }

    // Zapisuje zmiany w ustawieniach na podstawie wartoœci z UI
    public void SaveLanguageSettings()
    {
        // Aktualizacja informacji o jêzyku napisów na podstawie wartoœci wyœwietlanej
        settingsManager.settingsData.language.textsLanguage = textsLanguage.text;

        // Aktualizacja informacji o jêzyku nagrañ na podstawie wartoœci wyœwietlanej
        settingsManager.settingsData.language.audioLanguage = audioLanguage.text;

        // Aktualizacja informacji czy wyœwietlaæ napisy na podstawie tekstu ("TAK" lub "NIE")
        settingsManager.settingsData.language.displaySubtitles = displaySubtitles.text == "TAK";

        // Zapis ustawieñ
        settingsManager.SaveSettings();
    }
}
