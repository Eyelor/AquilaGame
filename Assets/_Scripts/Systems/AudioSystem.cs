using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class SettingsSoundData
{
    public SoundSettings sound;
}

public class AudioSystem : PersistentSingleton<AudioSystem>
{
    [Header("Music")]
    public AudioClip menuMusic; // Muzyka do Menu
    public AudioClip loadingPanelMusic; // Muzyka do Paska £adowania
    public AudioClip[] localIslandMusics; // Tablica muzyki dla lokalnej wyspy
    public AudioClip fightMusic; // Muzyka do walki
    [Range(0f, 1f)] public float musicVolume = 0.5f; // Zmienna do ustawienia g³oœnoœci muzyki
    [HideInInspector] public Coroutine playMusicCoroutine;

    [Header("Records")]
    [Range(0f, 1f)] public float recordsVolume = 0.5f; // Zmienna do ustawienia g³oœnoœci nagrañ dŸwiêkowych

    [Header("Effects")]
    public AudioClip buttonForwardSound; // DŸwiêk przycisku forward
    public AudioClip buttonBackSound; // DŸwiêk przycisku Back
    public AudioClip waterSound; // Efekt dŸwiêkowy wody na oceanie
    public AudioClip[] localIslandEffects; // Tablica efektów dŸwiêkowych dla lokalnej mapy
    public AudioClip chestOpen; // Efekt dŸwiêkowy do otwarcia skrzyni
    public AudioClip chestClose; // Efekt dŸwiêkowy do zamkniêcia skrzyni
    public AudioClip takeWeappon; // Efekt wyci¹gania szabli
    public AudioClip hideWeappon; // Efekt chowania szabli
    public AudioClip[] soundsDamage;
    public AudioClip playerDeathSound;
    public AudioClip winningMobSound; // Efekt dŸwiêkowy po pokonaniu moba
    public AudioClip walk; // Efekt dŸwiêkowy do chodzenia gracza
    public AudioClip run; // Efekt dŸwiêkowy do biegniêcia gracza
    [Range(0f, 1f)] public float effectsVolume = 0.5f; // Zmienna do ustawienia g³oœnoœci efektów dŸwiêkowych

    [Header("Audio Mixers")]
    public AudioMixerGroup stereoGroup; // Grupa AudioMixer dla stereo
    public AudioMixerGroup headphonesGroup; // Grupa AudioMixer dla s³uchawek

    [HideInInspector] public AudioSources audioSources;

    private string jsonFilePath;
    private bool useHeadphones = false; // Zmienna kontroluj¹ca, czy u¿ywamy s³uchawek
    [HideInInspector] public Coroutine playEffectsCoroutine; // Zmienna do œledzenia stanu Coroutine

    private int lastEffectIndex = -1; // WskaŸnik na ostatni efekt
    private int lastMusicIndex = -1; // WskaŸnik na ostatni utwór

    protected override void Awake()
    {
        base.Awake();

        audioSources = new AudioSources();
        // Inicjalizacja wszystkich AudioSource
        audioSources.InitializeAllSources(gameObject);

        // Œcie¿ka do pliku JSON
        jsonFilePath = Path.Combine(Application.persistentDataPath, "settings.json");

        // Wczytaj ustawienia dŸwiêku
        LoadSoundSettings();
    }

    public void LoadSoundSettings()
    {
        if (File.Exists(jsonFilePath))
        {
            // Wczytaj plik JSON
            string json = File.ReadAllText(jsonFilePath);
            SettingsData settingsData = JsonUtility.FromJson<SettingsData>(json);

            if (settingsData != null && settingsData.sound != null)
            {
                SoundSettings soundSettings = settingsData.sound;

                // Przypisz wartoœci z JSON-a
                if (soundSettings.audioDevice == "STEREO")
                {
                    useHeadphones = false;
                }
                else
                {
                    useHeadphones = true;
                }
                musicVolume = soundSettings.musicVolume;
                recordsVolume = soundSettings.recordsVolume;
                effectsVolume = soundSettings.effectsVolume;

                // Debug.Log("Ustawienia dŸwiêku zosta³y za³adowane.");

                // SprawdŸ, czy Audio source nie zosta³y zniszczone
                if (audioSources.musicAudioSource != null) audioSources.musicAudioSource.volume = musicVolume;
                if (audioSources.fightAudioSource != null) audioSources.fightAudioSource.volume = musicVolume;
                if (audioSources.effectsAudioSource != null) audioSources.effectsAudioSource.volume = effectsVolume;
                if (audioSources.localEffectsAudioSource != null) audioSources.localEffectsAudioSource.volume = effectsVolume;
                if (audioSources.playerEffectsAudioSource != null) audioSources.playerEffectsAudioSource.volume = effectsVolume;
                if (audioSources.recordsAudioSource != null) audioSources.recordsAudioSource.volume = recordsVolume;

                // Zaktualizuj wyjœcie audio na podstawie flagi useHeadphones
                UpdateAudioOutput();
            }
            else
            {
                Debug.LogWarning("Brak danych dŸwiêkowych w JSON-ie.");
            }
        }
        else
        {
            Debug.LogWarning("Nie znaleziono pliku JSON.");
        }
    }

    public void UpdateAudioOutput()
    {
        // W zale¿noœci od wartoœci useHeadphones przypisz odpowiedni¹ grupê AudioMixer
        if (useHeadphones)
        {
            audioSources.musicAudioSource.outputAudioMixerGroup = headphonesGroup;
            audioSources.fightAudioSource.outputAudioMixerGroup = headphonesGroup;
            audioSources.effectsAudioSource.outputAudioMixerGroup = headphonesGroup;
            audioSources.localEffectsAudioSource.outputAudioMixerGroup = headphonesGroup;
            audioSources.playerEffectsAudioSource.outputAudioMixerGroup = headphonesGroup;
            audioSources.recordsAudioSource.outputAudioMixerGroup = headphonesGroup;
        }
        else
        {
            audioSources.musicAudioSource.outputAudioMixerGroup = stereoGroup;
            audioSources.fightAudioSource.outputAudioMixerGroup = stereoGroup;
            audioSources.effectsAudioSource.outputAudioMixerGroup = stereoGroup;
            audioSources.localEffectsAudioSource.outputAudioMixerGroup = stereoGroup;
            audioSources.playerEffectsAudioSource.outputAudioMixerGroup = stereoGroup;
            audioSources.recordsAudioSource.outputAudioMixerGroup = stereoGroup;
        }
    }

    // Do uruchamiania efektów d¿wiêkowych, nagrañ i muzyki
    public void PlaySound(AudioSource audioSource, AudioClip clip, bool isLoop)
    {
        audioSource.loop = isLoop; // Zapêtlanie efektów dŸwiêkowych

        if (audioSource != null && clip != null)
        {
            if (!isLoop)
            {
                audioSource.PlayOneShot(clip); // Odtwórz efekt dŸwiêkowy jeden raz
            }
            else
            {
                if ((audioSource == audioSources.musicAudioSource && clip != loadingPanelMusic) || audioSource == audioSources.fightAudioSource)
                {
                    StartCoroutine(FadeIn(audioSource, clip, 0.5f)); // Fade-in dla muzyki
                }
                else
                {
                    audioSource.clip = clip;
                    audioSource.Play(); // Zacznij odtwarzanie efektu dŸwiêkowego w pêtli
                }
            }
        }
    }

    // Do zatrzymania efektów d¿wiêkowych, nagrañ i muzyki
    public void StopSound(AudioSource audioSource)
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            if (audioSource == audioSources.localEffectsAudioSource && playEffectsCoroutine != null)
            {
                StopCoroutine(playEffectsCoroutine);
                playEffectsCoroutine = null;
            }
            if (audioSource == audioSources.musicAudioSource)
            {
                if (playMusicCoroutine != null)
                {
                    StopCoroutine(playMusicCoroutine);
                    playMusicCoroutine = null;
                }
                StartCoroutine(FadeOut(audioSource, 1f));
            }
            if (audioSource == audioSources.fightAudioSource)
            {
                StartCoroutine(FadeOut(audioSource, 1f));
            }
            else
            {
                audioSource.Stop();
                audioSource.clip = null;
            }
        }
    }

    public void PlayLocalIslandEffects()
    {
        if (playEffectsCoroutine == null)
        {
            playEffectsCoroutine = StartCoroutine(PlayEffectsInLoop());
        }
    }

    public void PlayLocalIslandMusic()
    {
        if (playMusicCoroutine == null)
        {
            playMusicCoroutine = StartCoroutine(PlayMusicsInLoop());
        }
    }

    public void StopMusicCoroutine()
    {
        audioSources.musicAudioSource.Stop();
        if(playMusicCoroutine != null)
        {
            StopCoroutine(playMusicCoroutine);
            playMusicCoroutine = null;
        }
        
    }

    public void StopEffectsCoroutine()
    {
        audioSources.localEffectsAudioSource.Stop();
        if (playEffectsCoroutine != null)
        {
            StopCoroutine(playEffectsCoroutine);
            playEffectsCoroutine = null;
        }
    }

    private IEnumerator PlayEffectsInLoop()
    {
        while (true)
        {
            if (localIslandEffects.Length > 0)
            {
                int randomIndex;
                // Losuj indeks, który ró¿ni siê od ostatnio wybranego
                do
                {
                    randomIndex = Random.Range(0, localIslandEffects.Length);
                } while (randomIndex == lastEffectIndex);

                lastEffectIndex = randomIndex; // Zapisz aktualny indeks jako ostatni
                AudioClip randomClip = localIslandEffects[randomIndex];

                // Odtwórz losowy efekt
                PlaySound(audioSources.localEffectsAudioSource, randomClip, false);

                // Czekaj, a¿ efekt siê skoñczy (czas trwania klipu)
                yield return new WaitWhile(() => audioSources.localEffectsAudioSource.isPlaying);
            }
            else
            {
                Debug.LogWarning("There is no Sound Effects for local island");
                yield break; // Przerwij Coroutine, jeœli brak efektów
            }
        }
    }

    private IEnumerator PlayMusicsInLoop()
    {
        while (true)
        {
            if (localIslandMusics.Length > 0)
            {
                // Losowa iloœæ czasu przerwy miêdzy utworami (od 10 do 150 sekund)
                float randomDelay = Random.Range(10f, 50f);
                yield return new WaitForSeconds(randomDelay);

                int randomIndex;
                // Losuj indeks, który ró¿ni siê od ostatnio wybranego
                do
                {
                    randomIndex = Random.Range(0, localIslandMusics.Length);
                } while (randomIndex == lastMusicIndex);

                lastMusicIndex = randomIndex; // Zapisz aktualny indeks jako ostatni
                AudioClip randomClip = localIslandMusics[randomIndex];

                // Odtwórz losowy utwór
                PlaySound(audioSources.musicAudioSource, randomClip, false);

                // Czekaj, a¿ muzyka siê skoñczy (czas trwania klipu)
                yield return new WaitWhile(() => audioSources.musicAudioSource.isPlaying);
            }
            else
            {
                Debug.LogWarning("There is no Music for local island");
                yield break; // Przerwij Coroutine, jeœli brak muzyki
            }
        }
    }

    // Do p³ynnego zwiêkszania g³oœnoœci przy odpaleniu
    public IEnumerator FadeIn(AudioSource audioSource, AudioClip clip, float fadeTime)
    {
        float targetVolume = musicVolume; // Zak³adam, ¿e musicVolume to docelowa wartoœæ g³oœnoœci
        audioSource.clip = clip;
        audioSource.volume = 0; // Rozpocznij od 0 g³oœnoœci
        audioSource.Play();

        while (audioSource.volume < targetVolume)
        {
            audioSource.volume += (targetVolume / fadeTime) * Time.deltaTime; // Stopniowe zwiêkszanie g³oœnoœci
            yield return null; // Czekaj na kolejn¹ klatkê
        }

        audioSource.volume = targetVolume; // Ustaw docelow¹ g³oœnoœæ na koniec
    }

    // Do p³ynnego zmniejszania g³oœnoœci po wy³¹czeniu
    public IEnumerator FadeOut(AudioSource audioSource, float fadeTime)
    {
        float startVolume = musicVolume;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= (startVolume / fadeTime) * Time.deltaTime; // Stopniowe zmniejszanie g³oœnoœci
            yield return null; // Czekaj na kolejn¹ klatkê
        }
        audioSource.volume = 0; // Upewnij siê, ¿e g³oœnoœæ jest zerowa na koñcu
        audioSource.Stop(); // Zatrzymaj dŸwiêk po fade-out
        audioSource.clip = null; // Zwolnij zasób audio
        audioSource.volume = startVolume;
    }


    protected override void OnDestroy()
    {
        base.OnDestroy();
    }
}

