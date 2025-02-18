using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Continue : MonoBehaviour
{
    [SerializeField] PauseGame menuPauseCanvas;
    [SerializeField] GameObject lineObject; // Obiekt linii
    [SerializeField] GameObject additionalTextObject; // Obiekt dodatkowego tekstu

    private void OnMouseDown()
    {
        if (!AudioSystem.Instance.audioSources.effectsAudioSource.isPlaying)
        {
            AudioSystem.Instance.PlaySound(AudioSystem.Instance.audioSources.effectsAudioSource, AudioSystem.Instance.buttonForwardSound, false);
        }
        menuPauseCanvas.ResumeGame();

        // Ustaw obiekty domyœlnie jako niewidoczne
        if (lineObject != null)
        {
            lineObject.SetActive(false);
        }
        if (additionalTextObject != null)
        {
            additionalTextObject.SetActive(false);
        }
    }
}
