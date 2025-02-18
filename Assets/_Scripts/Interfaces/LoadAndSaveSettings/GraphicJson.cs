using TMPro;
using UnityEngine;

public class GraphicJson : MonoBehaviour
{
    // Komponenty TextMeshPro przypisane z Inspektora
    public TextMeshProUGUI verticalSync;    // Tekst dla informacji o synchronizacji pionowej
    public TextMeshProUGUI textureQuality;    // Tekst dla informacji o jako�ci tekstur
    public TextMeshProUGUI effectsQuality; // Tekst dla informacji o jako�ci efekt�w
    public TextMeshProUGUI reflectionsQuality;    // Tekst dla informacji o jako�ci odbi�
    public TextMeshProUGUI shadowsQuality;    // Tekst dla informacji o jako�ci cieni
    public TextMeshProUGUI meshQuality; // Tekst dla informacji o jako�ci siatki
    public TextMeshProUGUI shadowBuffer;    // Tekst dla informacji o buforze cieni
    public TextMeshProUGUI flare;    // Tekst dla informacji o flarze
    public TextMeshProUGUI contactShadows; // Tekst dla informacji o cieniach kontaktowych
    public TextMeshProUGUI spatialReflections; // Tekst dla informacji o odbiciach przestrzennych

    // Klasa zarz�dzaj�ca ustawieniami
    private SettingsDataSerialization settingsManager;

    private void Start()
    {
        // Znalezienie SettingsDataSerialization i zaktualizowanie UI na podstawie obecnych ustawie�
        settingsManager = FindObjectOfType<SettingsDataSerialization>();
        UpdateGraphicUI();
    }

    private void OnDisable()
    {
        SaveGraphicSettings();
        FrameRateSystem.Instance.UpdateFrameRate();
    }

    // Aktualizuje interfejs UI warto�ciami z ustawie�
    void UpdateGraphicUI()
    {
        // Wy�wietlenie informacji czy w��czy� synchronizacje pionow� 
        verticalSync.text = settingsManager.settingsData.graphics.verticalSync ? "W��CZ" : "WY��CZ";

        // Wy�wietlanie infomacji o jako�ci tekstur
        textureQuality.text = settingsManager.settingsData.graphics.textureQuality;

        // Wy�wietlanie infomacji o jako�ci efekt�w
        effectsQuality.text = settingsManager.settingsData.graphics.effectsQuality;

        // Wy�wietlanie infomacji o jako�ci odbi�
        reflectionsQuality.text = settingsManager.settingsData.graphics.reflectionsQuality;

        // Wy�wietlanie infomacji o jako�ci cieni
        shadowsQuality.text = settingsManager.settingsData.graphics.shadowsQuality;

        // Wy�wietlanie infomacji o jako�ci siatki
        meshQuality.text = settingsManager.settingsData.graphics.meshQuality;

        // Wy�wietlenie informacji czy w��czy� bufor cieni
        shadowBuffer.text = settingsManager.settingsData.graphics.shadowBuffer ? "W��CZ" : "WY��CZ";

        // Wy�wietlenie informacji czy w��czy� flar� 
        flare.text = settingsManager.settingsData.graphics.flare ? "W��CZ" : "WY��CZ";

        // Wy�wietlenie informacji czy w��czy� cienie kontaktowe 
        contactShadows.text = settingsManager.settingsData.graphics.contactShadows ? "W��CZ" : "WY��CZ";

        // Wy�wietlenie informacji czy w��czy� odbicia przestrzenne 
        spatialReflections.text = settingsManager.settingsData.graphics.spatialReflections ? "W��CZ" : "WY��CZ";
    }

    // Zapisuje zmiany w ustawieniach na podstawie warto�ci z UI
    public void SaveGraphicSettings()
    {
        // Aktualizacja informacji czy w��czy� synchronizacje pionow�
        settingsManager.settingsData.graphics.verticalSync = verticalSync.text == "W��CZ";

        // Aktualizacja infomacji o jako�ci tekstur
        settingsManager.settingsData.graphics.textureQuality = textureQuality.text;

        // Aktualizacja infomacji o jako�ci efekt�w
        settingsManager.settingsData.graphics.effectsQuality = effectsQuality.text;

        // Aktualizacja infomacji o jako�ci odbi�
        settingsManager.settingsData.graphics.reflectionsQuality = reflectionsQuality.text;

        // Aktualizacja infomacji o jako�ci cieni
        settingsManager.settingsData.graphics.shadowsQuality = shadowsQuality.text;

        // Aktualizacja infomacji o jako�ci siatki
        settingsManager.settingsData.graphics.meshQuality = meshQuality.text;

        // Aktualizacja informacji czy w��czy� bufor cieni
        settingsManager.settingsData.graphics.shadowBuffer = shadowBuffer.text == "W��CZ";

        // Aktualizacja informacji czy w��czy� flar� 
        settingsManager.settingsData.graphics.flare = flare.text == "W��CZ";

        // Aktualizacja informacji czy w��czy� cienie kontaktowe
        settingsManager.settingsData.graphics.contactShadows = contactShadows.text == "W��CZ";

        // Aktualizacja informacji czy w��czy� odbicia przestrzenne
        settingsManager.settingsData.graphics.spatialReflections = spatialReflections.text == "W��CZ";

        // Zapis ustawie�
        settingsManager.SaveSettings();
    }
}
