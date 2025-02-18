using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadMenuScene : MonoBehaviour
{
    public string sceneToLoad; // Nazwa sceny do za�adowania
    public bool isForward = true;

    private void OnMouseDown()
    {
        // Odtwarzanie d�wi�ku przy u�yciu AudioManager
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
        // Za�aduj scen�
        SceneManager.LoadScene(sceneToLoad);
    }
}
