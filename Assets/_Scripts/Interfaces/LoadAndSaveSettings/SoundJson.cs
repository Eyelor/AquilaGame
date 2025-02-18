using TMPro;
using UnityEngine;

public class SoundJson : MonoBehaviour
{
    // Komponenty TextMeshPro przypisane z Inspektora
    public TextMeshProUGUI audioDevice;    // Tekst dla wyœwietlania urz¹dzenia audio
    public TextMeshProUGUI musicVolume;    // Tekst dla wyœwietlania g³oœnoœci muzyki (%)
    public TextMeshProUGUI recordsVolume;    // Tekst dla wyœwietlania g³oœnoœci nagrañ (%)
    public TextMeshProUGUI effectsVolume;    // Tekst dla wyœwietlania g³oœnoœci efektów (%)

    // Klasa zarz¹dzaj¹ca ustawieniami
    private SettingsDataSerialization settingsManager;
    private AudioSystem audioSystem;

    private void Start()
    {
        // Znalezienie SettingsDataSerialization i zaktualizowanie UI na podstawie obecnych ustawieñ
        settingsManager = FindObjectOfType<SettingsDataSerialization>();
        audioSystem = FindObjectOfType<AudioSystem>();
        UpdateSoundUI();
    }

    private void OnDisable()
    {
        SaveSoundSettings();
    }

    // Aktualizuje interfejs UI wartoœciami z ustawieñ
    void UpdateSoundUI()
    {
        // Wyœwietlanie urz¹dzenia audio
        audioDevice.text = settingsManager.settingsData.sound.audioDevice;

        // Wyœwietlanie g³oœnoœci muzyki (%)
        musicVolume.text = (settingsManager.settingsData.sound.musicVolume * 100).ToString("F0") + "%";

        // Wyœwietlanie g³oœnoœci nagrañ (%)
        recordsVolume.text = (settingsManager.settingsData.sound.recordsVolume * 100).ToString("F0") + "%";

        // Wyœwietlanie g³oœnoœci efektów (%)
        effectsVolume.text = (settingsManager.settingsData.sound.effectsVolume * 100).ToString("F0") + "%";
    }

    // Zapisuje zmiany w ustawieniach na podstawie wartoœci z UI
    public void SaveSoundSettings()
    {
        // Aktualizacja informacji o urz¹dzeniu audio na podstawie wartoœci wyœwietlanej
        settingsManager.settingsData.sound.audioDevice = audioDevice.text;

        // Aktualizacja informacji o g³oœnoœci muzyki (konwersja z procentów na float)
        settingsManager.settingsData.sound.musicVolume = ParseVolume(musicVolume.text);
        
        // Aktualizacja informacji o g³oœnoœci nagrañ (konwersja z procentów na float)
        settingsManager.settingsData.sound.recordsVolume = ParseVolume(recordsVolume.text);

        // Aktualizacja informacji o g³oœnoœci efektów (konwersja z procentów na float)
        settingsManager.settingsData.sound.effectsVolume = ParseVolume(effectsVolume.text);

        // Zapis ustawieñ
        settingsManager.SaveSettings();

        // Zaktualizowanie Audio Managera
        audioSystem.LoadSoundSettings();
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
}
