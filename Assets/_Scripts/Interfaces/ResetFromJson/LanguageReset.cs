using TMPro;
using UnityEngine;

public class LanguageReset : MonoBehaviour
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
    }


    private void OnMouseDown()
    {
        if (!AudioSystem.Instance.audioSources.effectsAudioSource.isPlaying)
        {
            AudioSystem.Instance.PlaySound(AudioSystem.Instance.audioSources.effectsAudioSource, AudioSystem.Instance.buttonForwardSound, false);
        }
        // Debug.Log("LanguageReset");
        // Wy�wietlanie j�zyka napis�w
        textsLanguage.text = settingsManager.settingsData.defaults.languageDefault.textsLanguage;

        // Wy�wietlanie j�zyka nagra�
        audioLanguage.text = settingsManager.settingsData.defaults.languageDefault.audioLanguage;

        // Wy�wietlenie informacji czy wy�wietla� napisy ("TAK" lub "NIE")
        displaySubtitles.text = settingsManager.settingsData.defaults.languageDefault.displaySubtitles ? "TAK" : "NIE";
    }
}
