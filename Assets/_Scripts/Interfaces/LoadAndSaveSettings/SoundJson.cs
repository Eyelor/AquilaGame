using TMPro;
using UnityEngine;

public class SoundJson : MonoBehaviour
{
    // Komponenty TextMeshPro przypisane z Inspektora
    public TextMeshProUGUI audioDevice;    // Tekst dla wy�wietlania urz�dzenia audio
    public TextMeshProUGUI musicVolume;    // Tekst dla wy�wietlania g�o�no�ci muzyki (%)
    public TextMeshProUGUI recordsVolume;    // Tekst dla wy�wietlania g�o�no�ci nagra� (%)
    public TextMeshProUGUI effectsVolume;    // Tekst dla wy�wietlania g�o�no�ci efekt�w (%)

    // Klasa zarz�dzaj�ca ustawieniami
    private SettingsDataSerialization settingsManager;
    private AudioSystem audioSystem;

    private void Start()
    {
        // Znalezienie SettingsDataSerialization i zaktualizowanie UI na podstawie obecnych ustawie�
        settingsManager = FindObjectOfType<SettingsDataSerialization>();
        audioSystem = FindObjectOfType<AudioSystem>();
        UpdateSoundUI();
    }

    private void OnDisable()
    {
        SaveSoundSettings();
    }

    // Aktualizuje interfejs UI warto�ciami z ustawie�
    void UpdateSoundUI()
    {
        // Wy�wietlanie urz�dzenia audio
        audioDevice.text = settingsManager.settingsData.sound.audioDevice;

        // Wy�wietlanie g�o�no�ci muzyki (%)
        musicVolume.text = (settingsManager.settingsData.sound.musicVolume * 100).ToString("F0") + "%";

        // Wy�wietlanie g�o�no�ci nagra� (%)
        recordsVolume.text = (settingsManager.settingsData.sound.recordsVolume * 100).ToString("F0") + "%";

        // Wy�wietlanie g�o�no�ci efekt�w (%)
        effectsVolume.text = (settingsManager.settingsData.sound.effectsVolume * 100).ToString("F0") + "%";
    }

    // Zapisuje zmiany w ustawieniach na podstawie warto�ci z UI
    public void SaveSoundSettings()
    {
        // Aktualizacja informacji o urz�dzeniu audio na podstawie warto�ci wy�wietlanej
        settingsManager.settingsData.sound.audioDevice = audioDevice.text;

        // Aktualizacja informacji o g�o�no�ci muzyki (konwersja z procent�w na float)
        settingsManager.settingsData.sound.musicVolume = ParseVolume(musicVolume.text);
        
        // Aktualizacja informacji o g�o�no�ci nagra� (konwersja z procent�w na float)
        settingsManager.settingsData.sound.recordsVolume = ParseVolume(recordsVolume.text);

        // Aktualizacja informacji o g�o�no�ci efekt�w (konwersja z procent�w na float)
        settingsManager.settingsData.sound.effectsVolume = ParseVolume(effectsVolume.text);

        // Zapis ustawie�
        settingsManager.SaveSettings();

        // Zaktualizowanie Audio Managera
        audioSystem.LoadSoundSettings();
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
}
