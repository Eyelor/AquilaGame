using TMPro;
using UnityEngine;

public class LanguageJson : MonoBehaviour
{
    // Komponenty TextMeshPro przypisane z Inspektora
    public TextMeshProUGUI textsLanguage;    // Tekst dla wy�wietlania j�zyka napis�w
    public TextMeshProUGUI audioLanguage;    // Tekst dla wy�wietlania j�zyka nagra� 
    public TextMeshProUGUI displaySubtitles; // Tekst dla informacji czy wy�wietla� napisy

    // Klasa zarz�dzaj�ca ustawieniami
    private SettingsDataSerialization settingsManager;

    private void Start()
    {
        // Znalezienie SettingsDataSerialization i zaktualizowanie UI na podstawie obecnych ustawie�
        settingsManager = FindObjectOfType<SettingsDataSerialization>();
        UpdateLanguageUI();
    }

    private void OnDisable()
    {
        // Zapisanie zmian w ustawieniach do pliku JSON
        SaveLanguageSettings();
    }

    // Aktualizuje interfejs UI warto�ciami z ustawie�
    void UpdateLanguageUI()
    {
        // Wy�wietlanie j�zyka napis�w
        textsLanguage.text = settingsManager.settingsData.language.textsLanguage;

        // Wy�wietlanie j�zyka nagra�
        audioLanguage.text = settingsManager.settingsData.language.audioLanguage;

        // Wy�wietlenie informacji czy wy�wietla� napisy ("TAK" lub "NIE")
        displaySubtitles.text = settingsManager.settingsData.language.displaySubtitles ? "TAK" : "NIE";
    }

    // Zapisuje zmiany w ustawieniach na podstawie warto�ci z UI
    public void SaveLanguageSettings()
    {
        // Aktualizacja informacji o j�zyku napis�w na podstawie warto�ci wy�wietlanej
        settingsManager.settingsData.language.textsLanguage = textsLanguage.text;

        // Aktualizacja informacji o j�zyku nagra� na podstawie warto�ci wy�wietlanej
        settingsManager.settingsData.language.audioLanguage = audioLanguage.text;

        // Aktualizacja informacji czy wy�wietla� napisy na podstawie tekstu ("TAK" lub "NIE")
        settingsManager.settingsData.language.displaySubtitles = displaySubtitles.text == "TAK";

        // Zapis ustawie�
        settingsManager.SaveSettings();
    }
}
