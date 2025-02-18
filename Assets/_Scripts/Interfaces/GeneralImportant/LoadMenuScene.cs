using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadMenuScene : MonoBehaviour
{
    public string sceneToLoad; // Nazwa sceny do za³adowania
    public bool isForward = true;

    private void OnMouseDown()
    {
        // Odtwarzanie dŸwiêku przy u¿yciu AudioManager
        if (isForward)
        {    
            if (!AudioSystem.Instance.audioSources.effectsAudioSource.isPlaying)
            {
                AudioSystem.Instance.PlaySound(AudioSystem.Instance.audioSources.effectsAudioSource, AudioSystem.Instance.buttonForwardSound, false);
            }
        }
        else
        {
            if (!AudioSystem.Instance.audioSources.effectsAudioSource.isPlaying)
            {
                AudioSystem.Instance.PlaySound(AudioSystem.Instance.audioSources.effectsAudioSource, AudioSystem.Instance.buttonBackSound, false);
            }
        }
        // Za³aduj scenê
        SceneManager.LoadScene(sceneToLoad);
    }
}
