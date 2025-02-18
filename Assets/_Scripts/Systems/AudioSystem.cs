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
    public AudioClip loadingPanelMusic; // Muzyka do Paska �adowania
    public AudioClip[] localIslandMusics; // Tablica muzyki dla lokalnej wyspy
    public AudioClip fightMusic; // Muzyka do walki
    [Range(0f, 1f)] public float musicVolume = 0.5f; // Zmienna do ustawienia g�o�no�ci muzyki
    [HideInInspector] public Coroutine playMusicCoroutine;

    [Header("Records")]
    [Range(0f, 1f)] public float recordsVolume = 0.5f; // Zmienna do ustawienia g�o�no�ci nagra� d�wi�kowych

    [Header("Effects")]
    public AudioClip buttonForwardSound; // D�wi�k przycisku forward
    public AudioClip buttonBackSound; // D�wi�k przycisku Back
    public AudioClip waterSound; // Efekt d�wi�kowy wody na oceanie
    public AudioClip[] localIslandEffects; // Tablica efekt�w d�wi�kowych dla lokalnej mapy
    public AudioClip chestOpen; // Efekt d�wi�kowy do otwarcia skrzyni
    public AudioClip chestClose; // Efekt d�wi�kowy do zamkni�cia skrzyni
    public AudioClip takeWeappon; // Efekt wyci�gania szabli
    public AudioClip hideWeappon; // Efekt chowania szabli
    public AudioClip[] soundsDamage;
    public AudioClip playerDeathSound;
    public AudioClip winningMobSound; // Efekt d�wi�kowy po pokonaniu moba
    public AudioClip walk; // Efekt d�wi�kowy do chodzenia gracza
    public AudioClip run; // Efekt d�wi�kowy do biegni�cia gracza
    [Range(0f, 1f)] public float effectsVolume = 0.5f; // Zmienna do ustawienia g�o�no�ci efekt�w d�wi�kowych

    [Header("Audio Mixers")]
    public AudioMixerGroup stereoGroup; // Grupa AudioMixer dla stereo
    public AudioMixerGroup headphonesGroup; // Grupa AudioMixer dla s�uchawek

    [HideInInspector] public AudioSources audioSources;

    private string jsonFilePath;
    private bool useHeadphones = false; // Zmienna kontroluj�ca, czy u�ywamy s�uchawek
    [HideInInspector] public Coroutine playEffectsCoroutine; // Zmienna do �ledzenia stanu Coroutine

    private int lastEffectIndex = -1; // Wska�nik na ostatni efekt
    private int lastMusicIndex = -1; // Wska�nik na ostatni utw�r

    protected override void Awake()
    {
        base.Awake();

        audioSources = new AudioSources();
        // Inicjalizacja wszystkich AudioSource
        audioSources.InitializeAllSources(gameObject);

        // �cie�ka do pliku JSON
        jsonFilePath = Path.Combine(Application.persistentDataPath, "settings.json");

        // Wczytaj ustawienia d�wi�ku
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

                // Przypisz warto�ci z JSON-a
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

                // Debug.Log("Ustawienia d�wi�ku zosta�y za�adowane.");

                // Sprawd�, czy Audio source nie zosta�y zniszczone
                if (audioSources.musicAudioSource != null) audioSources.musicAudioSource.volume = musicVolume;
                if (audioSources.fightAudioSource != null) audioSources.fightAudioSource.volume = musicVolume;
                if (audioSources.effectsAudioSource != null) audioSources.effectsAudioSource.volume = effectsVolume;
                if (audioSources.localEffectsAudioSource != null) audioSources.localEffectsAudioSource.volume = effectsVolume;
                if (audioSources.playerEffectsAudioSource != null) audioSources.playerEffectsAudioSource.volume = effectsVolume;
                if (audioSources.recordsAudioSource != null) audioSources.recordsAudioSource.volume = recordsVolume;

                // Zaktualizuj wyj�cie audio na podstawie flagi useHeadphones
                UpdateAudioOutput();
            }
            else
            {
                Debug.LogWarning("Brak danych d�wi�kowych w JSON-ie.");
            }
        }
        else
        {
            Debug.LogWarning("Nie znaleziono pliku JSON.");
        }
    }

    public void UpdateAudioOutput()
    {
        // W zale�no�ci od warto�ci useHeadphones przypisz odpowiedni� grup� AudioMixer
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

    // Do uruchamiania efekt�w d�wi�kowych, nagra� i muzyki
    public void PlaySound(AudioSource audioSource, AudioClip clip, bool isLoop)
    {
        audioSource.loop = isLoop; // Zap�tlanie efekt�w d�wi�kowych

        if (audioSource != null && clip != null)
        {
            if (!isLoop)
            {
                audioSource.PlayOneShot(clip); // Odtw�rz efekt d�wi�kowy jeden raz
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
                    audioSource.Play(); // Zacznij odtwarzanie efektu d�wi�kowego w p�tli
                }
            }
        }
    }

    // Do zatrzymania efekt�w d�wi�kowych, nagra� i muzyki
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
                // Losuj indeks, kt�ry r�ni si� od ostatnio wybranego
                do
                {
                    randomIndex = Random.Range(0, localIslandEffects.Length);
                } while (randomIndex == lastEffectIndex);

                lastEffectIndex = randomIndex; // Zapisz aktualny indeks jako ostatni
                AudioClip randomClip = localIslandEffects[randomIndex];

                // Odtw�rz losowy efekt
                PlaySound(audioSources.localEffectsAudioSource, randomClip, false);

                // Czekaj, a� efekt si� sko�czy (czas trwania klipu)
                yield return new WaitWhile(() => audioSources.localEffectsAudioSource.isPlaying);
            }
            else
            {
                Debug.LogWarning("There is no Sound Effects for local island");
                yield break; // Przerwij Coroutine, je�li brak efekt�w
            }
        }
    }

    private IEnumerator PlayMusicsInLoop()
    {
        while (true)
        {
            if (localIslandMusics.Length > 0)
            {
                // Losowa ilo�� czasu przerwy mi�dzy utworami (od 10 do 150 sekund)
                float randomDelay = Random.Range(10f, 50f);
                yield return new WaitForSeconds(randomDelay);

                int randomIndex;
                // Losuj indeks, kt�ry r�ni si� od ostatnio wybranego
                do
                {
                    randomIndex = Random.Range(0, localIslandMusics.Length);
                } while (randomIndex == lastMusicIndex);

                lastMusicIndex = randomIndex; // Zapisz aktualny indeks jako ostatni
                AudioClip randomClip = localIslandMusics[randomIndex];

                // Odtw�rz losowy utw�r
                PlaySound(audioSources.musicAudioSource, randomClip, false);

                // Czekaj, a� muzyka si� sko�czy (czas trwania klipu)
                yield return new WaitWhile(() => audioSources.musicAudioSource.isPlaying);
            }
            else
            {
                Debug.LogWarning("There is no Music for local island");
                yield break; // Przerwij Coroutine, je�li brak muzyki
            }
        }
    }

    // Do p�ynnego zwi�kszania g�o�no�ci przy odpaleniu
    public IEnumerator FadeIn(AudioSource audioSource, AudioClip clip, float fadeTime)
    {
        float targetVolume = musicVolume; // Zak�adam, �e musicVolume to docelowa warto�� g�o�no�ci
        audioSource.clip = clip;
        audioSource.volume = 0; // Rozpocznij od 0 g�o�no�ci
        audioSource.Play();

        while (audioSource.volume < targetVolume)
        {
            audioSource.volume += (targetVolume / fadeTime) * Time.deltaTime; // Stopniowe zwi�kszanie g�o�no�ci
            yield return null; // Czekaj na kolejn� klatk�
        }

        audioSource.volume = targetVolume; // Ustaw docelow� g�o�no�� na koniec
    }

    // Do p�ynnego zmniejszania g�o�no�ci po wy��czeniu
    public IEnumerator FadeOut(AudioSource audioSource, float fadeTime)
    {
        float startVolume = musicVolume;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= (startVolume / fadeTime) * Time.deltaTime; // Stopniowe zmniejszanie g�o�no�ci
            yield return null; // Czekaj na kolejn� klatk�
        }
        audioSource.volume = 0; // Upewnij si�, �e g�o�no�� jest zerowa na ko�cu
        audioSource.Stop(); // Zatrzymaj d�wi�k po fade-out
        audioSource.clip = null; // Zwolnij zas�b audio
        audioSource.volume = startVolume;
    }


    protected override void OnDestroy()
    {
        base.OnDestroy();
    }
}

