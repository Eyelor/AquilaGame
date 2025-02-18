using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseGame : MonoBehaviour
{
    private bool isPaused = false;
    private float musicVolume;
    [SerializeField] GameObject backgroundPanel;
    [SerializeField] GameObject exitPanel;
    [SerializeField] GameObject exitPanelLineNo;
    [SerializeField] GameObject exitPanelLineYes;
    [SerializeField] GameObject exitMenuPanel;
    [SerializeField] GameObject exitMenuPanelLineNo;
    [SerializeField] GameObject exitMenuPanelLineYes;
    [SerializeField] GameObject mapPanel;
    [SerializeField] GameObject equipmentPanel;

    // Dla panelu zapisu
    [SerializeField] GameObject savePanel;
    [SerializeField] GameObject panelYes;
    [SerializeField] GameObject panelNo;
    [SerializeField] GameObject lineObjectYes;
    [SerializeField] GameObject lineObjectNo;
    [SerializeField] GameObject goBack;
    [SerializeField] GameObject goBackLine;
    [SerializeField] GameObject SaveInfoBefore;
    [SerializeField] GameObject SaveInfoAfter;

    // Dla stanu walki

    [SerializeField] private GameObject[] deactivatedObjects;
    [SerializeField] private Color colorActive;
    [SerializeField] private Color colorInactive;
    [SerializeField] private bool isOcean = false;

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                Pause();
            }
        }
    }

    // Metoda pauzuj¹ca grê
    void Pause()
    {   
        backgroundPanel.SetActive(true);
        if (!isOcean)
        {
            CheckIfPlayerFighting();
        } 
        Time.timeScale = 0f;
        if (mapPanel.activeSelf)
        {
            mapPanel.SetActive(false);
            if (isOcean)
            {
                GlobalGameManager.Instance.ToggleFullMap();
            }
            else
            {
                LocalGameManager.Instance.ToggleFullMap();
            }    
        }
        if (equipmentPanel != null)
        {
            if (equipmentPanel.activeSelf)
            {
                equipmentPanel.SetActive(false);
                LocalGameManager.Instance.ToggleEquipment();
            }
        }
        musicVolume = AudioSystem.Instance.audioSources.musicAudioSource.volume;
        AudioSystem.Instance.audioSources.musicAudioSource.volume = 0f;
        AudioSystem.Instance.audioSources.fightAudioSource.volume = 0f;
        GameInputSystem.Instance.SetForceCursorLockedAndInvisible(false);
        isPaused = true;
        Debug.Log("Gra zatrzymana");
    }

    // Metoda wznawiaj¹ca grê
    public void ResumeGame()
    {
        EnableBoxCollider2D();
        Time.timeScale = 1f;
        backgroundPanel.SetActive(false);
        if (exitPanel.activeSelf)
        {
            exitPanel.SetActive(false);
            exitPanelLineYes.SetActive(false);
            exitPanelLineNo.SetActive(false);
        }
        if (exitMenuPanel.activeSelf)
        {
            exitMenuPanel.SetActive(false);
            exitMenuPanelLineYes.SetActive(false);
            exitMenuPanelLineNo.SetActive(false);
        }
        if (savePanel.activeSelf)
        {
            panelYes.SetActive(true);
            panelNo.SetActive(true);
            lineObjectYes.SetActive(false);
            lineObjectNo.SetActive(false);
            goBack.SetActive(false);
            goBackLine.SetActive(false);
            SaveInfoBefore.SetActive(true);
            SaveInfoAfter.SetActive(false);
            savePanel.SetActive(false);
        }
        AudioSystem.Instance.audioSources.musicAudioSource.volume = musicVolume;
        AudioSystem.Instance.audioSources.fightAudioSource.volume = musicVolume;
        isPaused = false;
        GameInputSystem.Instance.SetForceCursorLockedAndInvisible(true);

        Debug.Log("Gra wznowiona");
    }

    public void CheckIfPlayerFighting()
    {
        foreach (GameObject obj in deactivatedObjects)
        {
            if (PlayerController.Instance.statistics.isFighting)
            {
                var textComponent = obj.GetComponent<TextMeshProUGUI>();
                if (textComponent != null)
                {
                    textComponent.color = colorInactive;
                }

                var colliderComponent = obj.GetComponent<BoxCollider2D>();
                if (colliderComponent != null)
                {
                    colliderComponent.enabled = false;
                }
            }
            else
            {
                var textComponent = obj.GetComponent<TextMeshProUGUI>();
                if (textComponent != null)
                {
                    textComponent.color = colorActive;
                }

                var colliderComponent = obj.GetComponent<BoxCollider2D>();
                if (colliderComponent != null)
                {
                    colliderComponent.enabled = true;
                }
            }
        }
    }

    public void EnableBoxCollider2D()
    {
        BoxCollider2D[] scriptsToDisable = FindObjectsOfType<BoxCollider2D>();
        foreach (BoxCollider2D script in scriptsToDisable)
        {
            script.enabled = true;
        }
    }
}
