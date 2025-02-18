using TMPro;
using UnityEngine;

public class MainMenuExit : MonoBehaviour
{
    private TextMeshProUGUI textMeshPro; // Referencja do komponentu TextMeshPro
    public GameObject exitPanel; // Obiekt panelu UI do wyœwietlenia przy wyjœciu
    public GameObject exitBackgroundPanel; // Panel przezroczystosci t³a przy wyjœciu

    private void Awake()
    {
        textMeshPro = GetComponent<TextMeshProUGUI>();

        // Upewnienie siê, ¿e panel jest domyœlnie niewidoczny
        if (exitPanel != null)
        {
            exitPanel.SetActive(false); 
        }
        // Upewnienie siê, ¿e panel t³a przezroczystoœci jest domyœlnie niewidoczny
        if (exitBackgroundPanel != null)
        {
            exitBackgroundPanel.SetActive(false);
        }
    }
    private void OnMouseDown()
    {
        if (!AudioSystem.Instance.audioSources.effectsAudioSource.isPlaying)
        {
            AudioSystem.Instance.PlaySound(AudioSystem.Instance.audioSources.effectsAudioSource, AudioSystem.Instance.buttonForwardSound, false);
        }
        DisableBoxCollider2D();
        if (exitPanel != null)
        {
            exitPanel.SetActive(true);
        }
        if (exitBackgroundPanel != null)
        {
            exitBackgroundPanel.SetActive(true);
        }

    }

    // Usuwanie aktywnoœci pod plansz¹ wyjœcia
    private void DisableBoxCollider2D()
    {
        // ZnajdŸ wszystkie obiekty zawieraj¹ce komponent "BoxCollider2D"
        BoxCollider2D[] scriptsToDisable = FindObjectsOfType<BoxCollider2D>();
        
        // PrzejdŸ przez wszystkie znalezione skrypty i wy³¹cz je
        foreach (BoxCollider2D script in scriptsToDisable)
        {
            // Debug.Log(script);
            script.enabled = false;
        }
    }
}
