using TMPro;
using UnityEngine;

public class DisplayJson : MonoBehaviour
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

    private void Start()
    {
        // Znalezienie SettingsDataSerialization i zaktualizowanie UI na podstawie obecnych ustawie�
        settingsManager = FindObjectOfType<SettingsDataSerialization>();
        UpdateDisplayUI();
    }

    private void OnDisable()
    {
        SaveDisplaySettings();
        FrameRateSystem.Instance.UpdateFrameRate();
        PostProcessingSystem.Instance.UpdatePostProcessing();
    }

    // Aktualizuje interfejs UI warto�ciami z ustawie�
    void UpdateDisplayUI()
    {
        // Wy�wietlenie informacji o rozdzielczo�ci
        resolution.text = settingsManager.settingsData.display.resolution;

        // Wy�wietlanie informacji o cz�stotliwo�ci od�wie�ania
        if (settingsManager.settingsData.display.refreshRate == -1)
        {
            refreshRate.text = "Nieograniczone";
        }
        else
        {
            refreshRate.text = (settingsManager.settingsData.display.refreshRate).ToString("F0") + " Hz";
        }

        // Wy�wietlanie informacji o jasno�ci
        brightness.text = (settingsManager.settingsData.display.brightness * 100).ToString("F0") + "%";

        // Wy�wietlanie informacji o kontra�cie
        contrast.text = (settingsManager.settingsData.display.contrast * 100).ToString("F0") + "%";

        // Wy�wietlanie informacji o gamma
        gamma.text = (settingsManager.settingsData.display.gamma * 100).ToString("F0") + "%";

        // Wy�wietlenie informacji czy wy�wietla� samouczek
        displayGuides.text = settingsManager.settingsData.display.displayGuides ? "TAK" : "NIE";

        // Wy�wietlenie informacji czy wy�wietla� podpowiedzi 
        displayHints.text = settingsManager.settingsData.display.displayHints ? "TAK" : "NIE";
    }

    // Zapisuje zmiany w ustawieniach na podstawie warto�ci z UI
    public void SaveDisplaySettings()
    {

        // Aktualizacja informacji o rozdzielczo�ci
        settingsManager.settingsData.display.resolution = resolution.text;

        // Aktualizacja informacji o cz�stotliwo�ci od�wie�ania
        settingsManager.settingsData.display.refreshRate = ParseRefreshRate(refreshRate.text);

        // Aktualizacja informacji o jasno�ci (konwersja z procent�w na float)
        settingsManager.settingsData.display.brightness = ParseVolume(brightness.text);

        // Aktualizacja informacji o kontra�cie (konwersja z procent�w na float)
        settingsManager.settingsData.display.contrast = ParseVolume(contrast.text);

        // Aktualizacja informacji o gamma (konwersja z procent�w na float)
        settingsManager.settingsData.display.gamma = ParseVolume(gamma.text);

        // Aktualizacja informacji czy wy�wietla� samouczek
        settingsManager.settingsData.display.displayGuides = displayGuides.text == "TAK";

        // Aktualizacja informacji czy wy�wietla� podpowiedzi 
        settingsManager.settingsData.display.displayHints = displayHints.text == "TAK";

        // Zapis ustawie�
        settingsManager.SaveSettings();
    }

    // Pomocnicza metoda do konwersji warto�ci procentowej na float
    private float ParseVolume(string volumeText)
    {
        // Usuwa znak "%" i konwertuje tekst na float, dziel�c przez 100
        if (float.TryParse(volumeText.Replace("%", ""), out float volume))
        {
            return volume / 100f;
        }

        // Domy�lna warto�� w razie b��du konwersji
        return 0.5f;
    }

    // Pomocnicza metoda do konwersji warto�ci w Hz na int
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

        // Domy�lna warto�� w razie b��du konwersji
        return 60;
    }
}
