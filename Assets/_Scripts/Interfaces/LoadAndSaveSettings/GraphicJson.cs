using TMPro;
using UnityEngine;

public class GraphicJson : MonoBehaviour
{
    // Komponenty TextMeshPro przypisane z Inspektora
    public TextMeshProUGUI verticalSync;    // Tekst dla informacji o synchronizacji pionowej
    public TextMeshProUGUI textureQuality;    // Tekst dla informacji o jakoœci tekstur
    public TextMeshProUGUI effectsQuality; // Tekst dla informacji o jakoœci efektów
    public TextMeshProUGUI reflectionsQuality;    // Tekst dla informacji o jakoœci odbiæ
    public TextMeshProUGUI shadowsQuality;    // Tekst dla informacji o jakoœci cieni
    public TextMeshProUGUI meshQuality; // Tekst dla informacji o jakoœci siatki
    public TextMeshProUGUI shadowBuffer;    // Tekst dla informacji o buforze cieni
    public TextMeshProUGUI flare;    // Tekst dla informacji o flarze
    public TextMeshProUGUI contactShadows; // Tekst dla informacji o cieniach kontaktowych
    public TextMeshProUGUI spatialReflections; // Tekst dla informacji o odbiciach przestrzennych

    // Klasa zarz¹dzaj¹ca ustawieniami
    private SettingsDataSerialization settingsManager;

    private void Start()
    {
        // Znalezienie SettingsDataSerialization i zaktualizowanie UI na podstawie obecnych ustawieñ
        settingsManager = FindObjectOfType<SettingsDataSerialization>();
        UpdateGraphicUI();
    }

    private void OnDisable()
    {
        SaveGraphicSettings();
        FrameRateSystem.Instance.UpdateFrameRate();
    }

    // Aktualizuje interfejs UI wartoœciami z ustawieñ
    void UpdateGraphicUI()
    {
        // Wyœwietlenie informacji czy w³¹czyæ synchronizacje pionow¹ 
        verticalSync.text = settingsManager.settingsData.graphics.verticalSync ? "W£¥CZ" : "WY£¥CZ";

        // Wyœwietlanie infomacji o jakoœci tekstur
        textureQuality.text = settingsManager.settingsData.graphics.textureQuality;

        // Wyœwietlanie infomacji o jakoœci efektów
        effectsQuality.text = settingsManager.settingsData.graphics.effectsQuality;

        // Wyœwietlanie infomacji o jakoœci odbiæ
        reflectionsQuality.text = settingsManager.settingsData.graphics.reflectionsQuality;

        // Wyœwietlanie infomacji o jakoœci cieni
        shadowsQuality.text = settingsManager.settingsData.graphics.shadowsQuality;

        // Wyœwietlanie infomacji o jakoœci siatki
        meshQuality.text = settingsManager.settingsData.graphics.meshQuality;

        // Wyœwietlenie informacji czy w³¹czyæ bufor cieni
        shadowBuffer.text = settingsManager.settingsData.graphics.shadowBuffer ? "W£¥CZ" : "WY£¥CZ";

        // Wyœwietlenie informacji czy w³¹czyæ flarê 
        flare.text = settingsManager.settingsData.graphics.flare ? "W£¥CZ" : "WY£¥CZ";

        // Wyœwietlenie informacji czy w³¹czyæ cienie kontaktowe 
        contactShadows.text = settingsManager.settingsData.graphics.contactShadows ? "W£¥CZ" : "WY£¥CZ";

        // Wyœwietlenie informacji czy w³¹czyæ odbicia przestrzenne 
        spatialReflections.text = settingsManager.settingsData.graphics.spatialReflections ? "W£¥CZ" : "WY£¥CZ";
    }

    // Zapisuje zmiany w ustawieniach na podstawie wartoœci z UI
    public void SaveGraphicSettings()
    {
        // Aktualizacja informacji czy w³¹czyæ synchronizacje pionow¹
        settingsManager.settingsData.graphics.verticalSync = verticalSync.text == "W£¥CZ";

        // Aktualizacja infomacji o jakoœci tekstur
        settingsManager.settingsData.graphics.textureQuality = textureQuality.text;

        // Aktualizacja infomacji o jakoœci efektów
        settingsManager.settingsData.graphics.effectsQuality = effectsQuality.text;

        // Aktualizacja infomacji o jakoœci odbiæ
        settingsManager.settingsData.graphics.reflectionsQuality = reflectionsQuality.text;

        // Aktualizacja infomacji o jakoœci cieni
        settingsManager.settingsData.graphics.shadowsQuality = shadowsQuality.text;

        // Aktualizacja infomacji o jakoœci siatki
        settingsManager.settingsData.graphics.meshQuality = meshQuality.text;

        // Aktualizacja informacji czy w³¹czyæ bufor cieni
        settingsManager.settingsData.graphics.shadowBuffer = shadowBuffer.text == "W£¥CZ";

        // Aktualizacja informacji czy w³¹czyæ flarê 
        settingsManager.settingsData.graphics.flare = flare.text == "W£¥CZ";

        // Aktualizacja informacji czy w³¹czyæ cienie kontaktowe
        settingsManager.settingsData.graphics.contactShadows = contactShadows.text == "W£¥CZ";

        // Aktualizacja informacji czy w³¹czyæ odbicia przestrzenne
        settingsManager.settingsData.graphics.spatialReflections = spatialReflections.text == "W£¥CZ";

        // Zapis ustawieñ
        settingsManager.SaveSettings();
    }
}
