using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GraphicReset : MonoBehaviour
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
        // Debug.Log("GraphicReset");
        // Wyœwietlenie informacji czy w³¹czyæ synchronizacje pionow¹ 
        verticalSync.text = settingsManager.settingsData.defaults.graphicsDefault.verticalSync ? "W£¥CZ" : "WY£¥CZ";

        // Wyœwietlanie infomacji o jakoœci tekstur
        textureQuality.text = settingsManager.settingsData.defaults.graphicsDefault.textureQuality;

        // Wyœwietlanie infomacji o jakoœci efektów
        effectsQuality.text = settingsManager.settingsData.defaults.graphicsDefault.effectsQuality;

        // Wyœwietlanie infomacji o jakoœci odbiæ
        reflectionsQuality.text = settingsManager.settingsData.defaults.graphicsDefault.reflectionsQuality;

        // Wyœwietlanie infomacji o jakoœci cieni
        shadowsQuality.text = settingsManager.settingsData.defaults.graphicsDefault.shadowsQuality;

        // Wyœwietlanie infomacji o jakoœci siatki
        meshQuality.text = settingsManager.settingsData.defaults.graphicsDefault.meshQuality;

        // Wyœwietlenie informacji czy w³¹czyæ bufor cieni
        shadowBuffer.text = settingsManager.settingsData.defaults.graphicsDefault.shadowBuffer ? "W£¥CZ" : "WY£¥CZ";

        // Wyœwietlenie informacji czy w³¹czyæ flarê 
        flare.text = settingsManager.settingsData.defaults.graphicsDefault.flare ? "W£¥CZ" : "WY£¥CZ";

        // Wyœwietlenie informacji czy w³¹czyæ cienie kontaktowe 
        contactShadows.text = settingsManager.settingsData.defaults.graphicsDefault.contactShadows ? "W£¥CZ" : "WY£¥CZ";

        // Wyœwietlenie informacji czy w³¹czyæ odbicia przestrzenne 
        spatialReflections.text = settingsManager.settingsData.defaults.graphicsDefault.spatialReflections ? "W£¥CZ" : "WY£¥CZ";
    }
}
