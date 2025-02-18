using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitToMainMenu : MonoBehaviour
{
    [SerializeField] private GameObject _objectToRemoveFirst;

    private void OnMouseDown()
    {
        EnableBoxCollider2D();

        if (!AudioSystem.Instance.audioSources.effectsAudioSource.isPlaying)
        {
            AudioSystem.Instance.PlaySound(AudioSystem.Instance.audioSources.effectsAudioSource, AudioSystem.Instance.buttonForwardSound, false);
        }
        AudioSystem.Instance.StopMusicCoroutine();
        AudioSystem.Instance.StopEffectsCoroutine();
        AudioSystem.Instance.StopAllCoroutines();

        if (_objectToRemoveFirst != null)
        {
            LocalIslandGenerationManager.Instance.exitToMainMenu = true;
            Destroy(_objectToRemoveFirst);
        }
        AudioSystem.Instance.PlaySound(AudioSystem.Instance.audioSources.musicAudioSource, AudioSystem.Instance.menuMusic, true);
        AudioSystem.Instance.audioSources.musicAudioSource.volume = AudioSystem.Instance.musicVolume;
        AudioSystem.Instance.audioSources.fightAudioSource.volume = AudioSystem.Instance.musicVolume;

        Time.timeScale = 1f;
        // Za≥adowanie scenÍ
        SceneManager.LoadScene("MainMenu");
    }

    public void EnableBoxCollider2D()
    {
        // Znajdü wszystkie obiekty zawierajπce komponent "BoxCollider2D"
        BoxCollider2D[] scriptsToDisable = FindObjectsOfType<BoxCollider2D>();

        // Przejdü przez wszystkie znalezione skrypty i w≥πcz je
        foreach (BoxCollider2D script in scriptsToDisable)
        {
            // Debug.Log(script);
            script.enabled = true;
        }
    }
}
