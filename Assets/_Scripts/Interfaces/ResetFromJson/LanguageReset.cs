using TMPro;
using UnityEngine;

public class LanguageReset : MonoBehaviour
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
    }


    private void OnMouseDown()
    {
        if (!AudioSystem.Instance.audioSources.effectsAudioSource.isPlaying)
        {
            AudioSystem.Instance.PlaySound(AudioSystem.Instance.audioSources.effectsAudioSource, AudioSystem.Instance.buttonForwardSound, false);
        }
        // Debug.Log("LanguageReset");
        // Wyœwietlanie jêzyka napisów
        textsLanguage.text = settingsManager.settingsData.defaults.languageDefault.textsLanguage;

        // Wyœwietlanie jêzyka nagrañ
        audioLanguage.text = settingsManager.settingsData.defaults.languageDefault.audioLanguage;

        // Wyœwietlenie informacji czy wyœwietlaæ napisy ("TAK" lub "NIE")
        displaySubtitles.text = settingsManager.settingsData.defaults.languageDefault.displaySubtitles ? "TAK" : "NIE";
    }
}
