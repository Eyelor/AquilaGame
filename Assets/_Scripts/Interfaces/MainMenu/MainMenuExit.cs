using TMPro;
using UnityEngine;

public class MainMenuExit : MonoBehaviour
{
    private TextMeshProUGUI textMeshPro; // Referencja do komponentu TextMeshPro
    public GameObject exitPanel; // Obiekt panelu UI do wy�wietlenia przy wyj�ciu
    public GameObject exitBackgroundPanel; // Panel przezroczystosci t�a przy wyj�ciu

    private void Awake()
    {
        textMeshPro = GetComponent<TextMeshProUGUI>();

        // Upewnienie si�, �e panel jest domy�lnie niewidoczny
        if (exitPanel != null)
        {
            exitPanel.SetActive(false); 
        }
        // Upewnienie si�, �e panel t�a przezroczysto�ci jest domy�lnie niewidoczny
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

    // Usuwanie aktywno�ci pod plansz� wyj�cia
    private void DisableBoxCollider2D()
    {
        // Znajd� wszystkie obiekty zawieraj�ce komponent "BoxCollider2D"
        BoxCollider2D[] scriptsToDisable = FindObjectsOfType<BoxCollider2D>();
        
        // Przejd� przez wszystkie znalezione skrypty i wy��cz je
        foreach (BoxCollider2D script in scriptsToDisable)
        {
            // Debug.Log(script);
            script.enabled = false;
        }
    }
}
