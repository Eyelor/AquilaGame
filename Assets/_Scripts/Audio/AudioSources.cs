using UnityEngine;

public class AudioSources
{
    public AudioSource musicAudioSource; // AudioSource dla muzyki
    public AudioSource fightAudioSource; // AudioSource dla walki

    public AudioSource effectsAudioSource; // AudioSource dla efektów dŸwiêkowych
    public AudioSource localEffectsAudioSource; // AudioSource dla efektów dŸwiêkowych t³a na wyspach lokalnych
    public AudioSource playerEffectsAudioSource; // AudioSource dla efektów dŸwiêkowych postaci

    public AudioSource recordsAudioSource; // AudioSource dla nagrañ dŸwiêkowych

    // Metoda do ustawiania w³aœciwoœci dla AudioSource
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

    // Metoda zatrzymuj¹ca odtwarzanie wszystkich AudioSource
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
