using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GraphicReset : MonoBehaviour
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

    void Start()
    {
        // Znalezienie SettingsDataSerialization i zaktualizowanie UI na podstawie obecnych ustawie�
        settingsManager = FindObjectOfType<SettingsDataSerialization>();
    }

    private void OnMouseDown()
    {
        if (!AudioSystem.Instance.audioSources.effectsAudioSource.isPlaying)
        {
            AudioSystem.Instance.PlaySound(AudioSystem.Instance.audioSources.effectsAudioSource, AudioSystem.Instance.buttonForwardSound, false);
        }
        // Debug.Log("GraphicReset");
        // Wy�wietlenie informacji czy w��czy� synchronizacje pionow� 
        verticalSync.text = settingsManager.settingsData.defaults.graphicsDefault.verticalSync ? "W��CZ" : "WY��CZ";

        // Wy�wietlanie infomacji o jako�ci tekstur
        textureQuality.text = settingsManager.settingsData.defaults.graphicsDefault.textureQuality;

        // Wy�wietlanie infomacji o jako�ci efekt�w
        effectsQuality.text = settingsManager.settingsData.defaults.graphicsDefault.effectsQuality;

        // Wy�wietlanie infomacji o jako�ci odbi�
        reflectionsQuality.text = settingsManager.settingsData.defaults.graphicsDefault.reflectionsQuality;

        // Wy�wietlanie infomacji o jako�ci cieni
        shadowsQuality.text = settingsManager.settingsData.defaults.graphicsDefault.shadowsQuality;

        // Wy�wietlanie infomacji o jako�ci siatki
        meshQuality.text = settingsManager.settingsData.defaults.graphicsDefault.meshQuality;

        // Wy�wietlenie informacji czy w��czy� bufor cieni
        shadowBuffer.text = settingsManager.settingsData.defaults.graphicsDefault.shadowBuffer ? "W��CZ" : "WY��CZ";

        // Wy�wietlenie informacji czy w��czy� flar� 
        flare.text = settingsManager.settingsData.defaults.graphicsDefault.flare ? "W��CZ" : "WY��CZ";

        // Wy�wietlenie informacji czy w��czy� cienie kontaktowe 
        contactShadows.text = settingsManager.settingsData.defaults.graphicsDefault.contactShadows ? "W��CZ" : "WY��CZ";

        // Wy�wietlenie informacji czy w��czy� odbicia przestrzenne 
        spatialReflections.text = settingsManager.settingsData.defaults.graphicsDefault.spatialReflections ? "W��CZ" : "WY��CZ";
    }
}
