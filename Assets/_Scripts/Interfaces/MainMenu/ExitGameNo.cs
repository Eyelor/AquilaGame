using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ExitGameNo : MonoBehaviour
{
    private TextMeshProUGUI textMeshPro; 
    public GameObject exitPanel; // Obiekt panelu UI do wyúwietlenia przy wyjúciu
    public GameObject exitBackgroundPanel; // Panel przezroczystosci t≥a przy wyjúciu
    public GameObject lineObject; // Obiekt linii

    private void Awake()
    {
        textMeshPro = GetComponent<TextMeshProUGUI>();
    }
    private void OnMouseDown()
    {
        if (!AudioSystem.Instance.audioSources.effectsAudioSource.isPlaying)
        {
            AudioSystem.Instance.PlaySound(AudioSystem.Instance.audioSources.effectsAudioSource, AudioSystem.Instance.buttonForwardSound, false);
        }
        if (exitPanel != null)
        {
            exitPanel.SetActive(false);
        }
        if (exitBackgroundPanel != null)
        {
            exitBackgroundPanel.SetActive(false);
        }
        if (lineObject != null)
        {
            lineObject.SetActive(false);
        }
        EnableBoxCollider2D();
    }
    // PrzywrÛcenie aktywnoúci pod planszπ wyjúcia
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
