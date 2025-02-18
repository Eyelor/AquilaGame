using UnityEngine;

public class AudioSources
{
    public AudioSource musicAudioSource; // AudioSource dla muzyki
    public AudioSource fightAudioSource; // AudioSource dla walki

    public AudioSource effectsAudioSource; // AudioSource dla efekt�w d�wi�kowych
    public AudioSource localEffectsAudioSource; // AudioSource dla efekt�w d�wi�kowych t�a na wyspach lokalnych
    public AudioSource playerEffectsAudioSource; // AudioSource dla efekt�w d�wi�kowych postaci

    public AudioSource recordsAudioSource; // AudioSource dla nagra� d�wi�kowych

    // Metoda do ustawiania w�a�ciwo�ci dla AudioSource
    public void SetAudioSource(ref AudioSource audioSource, GameObject gameObject)
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    // Metoda do ustawienia wszystkich AudioSource
    public void InitializeAllSources(GameObject gameObject)
    {
        SetAudioSource(ref musicAudioSource, gameObject);
        SetAudioSource(ref fightAudioSource, gameObject);
        SetAudioSource(ref effectsAudioSource, gameObject);
        SetAudioSource(ref localEffectsAudioSource, gameObject);
        SetAudioSource(ref playerEffectsAudioSource, gameObject);
        SetAudioSource(ref recordsAudioSource, gameObject);
    }

    // Metoda zatrzymuj�ca odtwarzanie wszystkich AudioSource
    public void StopAllSounds(bool withMusic)
    {
        if (withMusic && musicAudioSource.isPlaying)
        {
            musicAudioSource.Stop();
            musicAudioSource.clip = null;
        }

        if (fightAudioSource.isPlaying)
        {
            fightAudioSource.Stop();
            fightAudioSource.clip = null;
        }

        if (effectsAudioSource.isPlaying)
        {
            effectsAudioSource.Stop();
            effectsAudioSource.clip = null;
        }

        if (localEffectsAudioSource.isPlaying)
        {
            localEffectsAudioSource.Stop();
            localEffectsAudioSource.clip = null;
        }

        if (playerEffectsAudioSource.isPlaying)
        {
            playerEffectsAudioSource.Stop();
            playerEffectsAudioSource.clip = null;
        }

        if (recordsAudioSource.isPlaying)
        {
            recordsAudioSource.Stop();
            recordsAudioSource.clip = null;
        }
    }
}
