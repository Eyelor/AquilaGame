using TMPro;
using UnityEngine;

public class DisplayJson : MonoBehaviour
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

    private void Start()
    {
        // Znalezienie SettingsDataSerialization i zaktualizowanie UI na podstawie obecnych ustawieñ
        settingsManager = FindObjectOfType<SettingsDataSerialization>();
        UpdateDisplayUI();
    }

    private void OnDisable()
    {
        SaveDisplaySettings();
        FrameRateSystem.Instance.UpdateFrameRate();
        PostProcessingSystem.Instance.UpdatePostProcessing();
    }

    // Aktualizuje interfejs UI wartoœciami z ustawieñ
    void UpdateDisplayUI()
    {
        // Wyœwietlenie informacji o rozdzielczoœci
        resolution.text = settingsManager.settingsData.display.resolution;

        // Wyœwietlanie informacji o czêstotliwoœci odœwie¿ania
        if (settingsManager.settingsData.display.refreshRate == -1)
        {
            refreshRate.text = "Nieograniczone";
        }
        else
        {
            refreshRate.text = (settingsManager.settingsData.display.refreshRate).ToString("F0") + " Hz";
        }

        // Wyœwietlanie informacji o jasnoœci
        brightness.text = (settingsManager.settingsData.display.brightness * 100).ToString("F0") + "%";

        // Wyœwietlanie informacji o kontraœcie
        contrast.text = (settingsManager.settingsData.display.contrast * 100).ToString("F0") + "%";

        // Wyœwietlanie informacji o gamma
        gamma.text = (settingsManager.settingsData.display.gamma * 100).ToString("F0") + "%";

        // Wyœwietlenie informacji czy wyœwietlaæ samouczek
        displayGuides.text = settingsManager.settingsData.display.displayGuides ? "TAK" : "NIE";

        // Wyœwietlenie informacji czy wyœwietlaæ podpowiedzi 
        displayHints.text = settingsManager.settingsData.display.displayHints ? "TAK" : "NIE";
    }

    // Zapisuje zmiany w ustawieniach na podstawie wartoœci z UI
    public void SaveDisplaySettings()
    {

        // Aktualizacja informacji o rozdzielczoœci
        settingsManager.settingsData.display.resolution = resolution.text;

        // Aktualizacja informacji o czêstotliwoœci odœwie¿ania
        settingsManager.settingsData.display.refreshRate = ParseRefreshRate(refreshRate.text);

        // Aktualizacja informacji o jasnoœci (konwersja z procentów na float)
        settingsManager.settingsData.display.brightness = ParseVolume(brightness.text);

        // Aktualizacja informacji o kontraœcie (konwersja z procentów na float)
        settingsManager.settingsData.display.contrast = ParseVolume(contrast.text);

        // Aktualizacja informacji o gamma (konwersja z procentów na float)
        settingsManager.settingsData.display.gamma = ParseVolume(gamma.text);

        // Aktualizacja informacji czy wyœwietlaæ samouczek
        settingsManager.settingsData.display.displayGuides = displayGuides.text == "TAK";

        // Aktualizacja informacji czy wyœwietlaæ podpowiedzi 
        settingsManager.settingsData.display.displayHints = displayHints.text == "TAK";

        // Zapis ustawieñ
        settingsManager.SaveSettings();
    }

    // Pomocnicza metoda do konwersji wartoœci procentowej na float
    private float ParseVolume(string volumeText)
    {
        // Usuwa znak "%" i konwertuje tekst na float, dziel¹c przez 100
        if (float.TryParse(volumeText.Replace("%", ""), out float volume))
        {
            return volume / 100f;
        }

        // Domyœlna wartoœæ w razie b³êdu konwersji
        return 0.5f;
    }

    // Pomocnicza metoda do konwersji wartoœci w Hz na int
    private int ParseRefreshRate(string refreshRateText)
    {
        // Usuwa znak " Hz" i konwertuje tekst na int
        if (int.TryParse(refreshRateText.Replace(" Hz", ""), out int refreshRateOut))
        {
            return refreshRateOut;
        }
        else if (refreshRateText == "Nieograniczone")
        {
            return -1;
        }

        // Domyœlna wartoœæ w razie b³êdu konwersji
        return 60;
    }
}
