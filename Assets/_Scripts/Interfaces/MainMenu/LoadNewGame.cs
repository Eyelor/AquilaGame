using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadNewGame : MonoBehaviour
{
    public string sceneToLoad; // Nazwa sceny do za³adowania

    private void OnMouseDown()
    {
        SavesData savesData = SaveManager.Instance.LoadSavesData();

        if (savesData != null )
        {
            SaveManager.Instance.SaveSavesData(savesData, "Mapa Globalna", false);
        }
        else
        {
            SaveManager.Instance.SaveSavesData(savesData, "Mapa Globalna", true);
        }

        if (!AudioSystem.Instance.audioSources.effectsAudioSource.isPlaying)
        {
            AudioSystem.Instance.PlaySound(AudioSystem.Instance.audioSources.effectsAudioSource, AudioSystem.Instance.buttonForwardSound, false);
        }

        SceneTransitionManager.nextSceneName = sceneToLoad;
        SceneManager.LoadScene("LoadingPanel");
    }
}
