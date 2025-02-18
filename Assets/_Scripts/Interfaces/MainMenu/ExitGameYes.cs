using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitGameYes : MonoBehaviour
{
    private void OnMouseDown()
    {
        if (!AudioSystem.Instance.audioSources.effectsAudioSource.isPlaying)
        {
            AudioSystem.Instance.PlaySound(AudioSystem.Instance.audioSources.effectsAudioSource, AudioSystem.Instance.buttonForwardSound, false);
        }
        Debug.Log("Wyjœcie z aplikacji");
        Application.Quit();

        // If running in the Unity Editor, stop playing the scene
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
