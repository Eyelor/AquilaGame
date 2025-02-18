using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Unity.VisualScripting;
using System;

public class LoadingPanelEndManager : Singleton<LoadingPanelEndManager>
{
    public GameObject objectToDeactivate; // GameObject, kt�ry chcesz dezaktywowa�
    public GameObject pauseCanvasToActivate; // GameObject, kt�ry chcesz dezaktywowa�
    public GameObject objectToActivate; // GameObject, kt�ry chcesz aktywowa� po dezaktywacji obiektu
    public GameObject playerToActivate; // Player, kt�ry chcesz aktywowa� po dezaktywacji
    public float delayBeforeAction = 4f; // Czas oczekiwania przed dezaktywacj� obiektu
    public Image loadingBar; // Pasek �adowania, typ UI Image, kt�ry u�ywa Sprite (SVG)
    public Image maskingImage; // Maskuj�cy obiekt Image z kolorem

    private float smoothTime = 0.5f; // Czas wyg�adzania przej�cia mi�dzy krokami
    private RectTransform maskingRectTransform; // RectTransform maskuj�cego obiektu
    private float initialWidth; // Pocz�tkowa szeroko�� paska �adowania
    private float targetWidth; // Szeroko�� docelowa dla aktualnego kroku
    private float velocity = 0.0f; // Pr�dko�� dla SmoothDamp
    private AsyncOperation loadingOperation; // Operacja asynchronicznego �adowania sceny

    protected override void Awake()
    {
        base.Awake();

        if (loadingBar == null || maskingImage == null)
        {
            Debug.LogError("Loading bar or masking image is not assigned.");
            return;
        }

        maskingRectTransform = maskingImage.GetComponent<RectTransform>();
        initialWidth = loadingBar.rectTransform.rect.width;

        // Ustaw pocz�tkow� szeroko�� maskuj�cego obiektu na pe�n� szeroko�� paska
        maskingRectTransform.sizeDelta = new Vector2(initialWidth * 0.1f, maskingRectTransform.sizeDelta.y);

        // Aktywuj playera
        if (playerToActivate != null)
        {
            // Pobierz komponent skryptu z obiektu.
            Interactor scriptIteractorActivate = playerToActivate.GetComponent<Interactor>();
            if (scriptIteractorActivate != null)
            {
                // Dezaktywuj skrypt.
                scriptIteractorActivate.enabled = false;
            }
            Animator scriptToActivate = playerToActivate.GetComponent<Animator>();
            if (scriptToActivate != null)
            {
                // Dezaktywuj skrypt.
                scriptToActivate.enabled = false;
            }
            ShipController shipScriptToActivate = playerToActivate.GetComponent<ShipController>();
            if (shipScriptToActivate != null)
            {
                // Dezaktywuj skrypt.
                shipScriptToActivate.enabled = false;
            }
        }

        // Wywo�aj `FinishLoading` jako korutyn�
        StartCoroutine(FinishLoading());

        // Rozpocznij proces dezaktywacji obiektu i aktywacji innego obiektu
        StartCoroutine(DeactivateLoadingPanelAndActivateCanvas());
    }

    void Start()
    {
        if (AudioSystem.Instance.audioSources.musicAudioSource.clip != AudioSystem.Instance.loadingPanelMusic)
        {
            AudioSystem.Instance.StopSound(AudioSystem.Instance.audioSources.musicAudioSource);
            AudioSystem.Instance.PlaySound(AudioSystem.Instance.audioSources.musicAudioSource, AudioSystem.Instance.loadingPanelMusic, true);
        }
    }
    IEnumerator DeactivateLoadingPanelAndActivateCanvas()
    {
        // Czekaj okre�lon� ilo�� czasu przed dezaktywacj� Panelu �adowania
        yield return new WaitForSeconds(delayBeforeAction);
        
        // Aktywuj playera
        if (playerToActivate != null)
        {
            // Pobierz komponent skryptu z obiektu.
            Animator scriptToActivate = playerToActivate.GetComponent<Animator>();

            if (scriptToActivate != null)
            {
                // Aktywuj skrypt.
                scriptToActivate.enabled = true;
            }

            Interactor scriptIteractorActivate = playerToActivate.GetComponent<Interactor>();

            if (scriptIteractorActivate != null)
            {
                // Aktywuj skrypt.
                scriptIteractorActivate.enabled = true;
            }

            ShipController shipScriptToActivate = playerToActivate.GetComponent<ShipController>();

            if (shipScriptToActivate != null)
            {
                // Aktywuj skrypt.
                shipScriptToActivate.enabled = true;
            }
        }
        else
        {
            Debug.LogWarning("Nie przypisano �adnego obiektu Playera do aktywacji.");
        }

        // Dezaktywowuj panel
        if (objectToDeactivate != null)
        {
            objectToDeactivate.SetActive(false);
            Debug.Log($"GameObject '{objectToDeactivate.name}' zosta� dezaktywowany.");
        }
        else
        {
            Debug.LogWarning("Nie przypisano �adnego GameObject do dezaktywacji.");
        }

        // Aktywuj Canvas
        if (objectToActivate != null)
        {
            objectToActivate.SetActive(true);
            Debug.Log($"GameObject '{objectToActivate.name}' zosta� aktywowany.");
        }
        else
        {
            Debug.LogWarning("Nie przypisano �adnego GameObject do aktywacji.");
        }
        // Aktywuj Canvas Menu Pausy
        if (pauseCanvasToActivate != null)
        {
            pauseCanvasToActivate.SetActive(true);
            Debug.Log($"GameObject '{pauseCanvasToActivate.name}' zosta� aktywowany.");
        }
        else
        {
            Debug.LogWarning("Nie przypisano MenuPausy do aktywacji.");
        }

        maskingRectTransform.sizeDelta = new Vector2(initialWidth * 0.1f, maskingRectTransform.sizeDelta.y);

        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "GlobalMap") // Sprawdzenie, czy jest to scena globalna
        {
            AudioSystem.Instance.PlaySound(AudioSystem.Instance.audioSources.localEffectsAudioSource, AudioSystem.Instance.waterSound, true);
        }
        else
        {
            // AudioSystem.Instance.StopSound(AudioSystem.Instance.audioSources.musicAudioSource);
            AudioSystem.Instance.PlayLocalIslandEffects();
            AudioSystem.Instance.PlayLocalIslandMusic();
        }
        float startVolume = AudioSystem.Instance.musicVolume;

        while (AudioSystem.Instance.audioSources.musicAudioSource.volume > 0.05f)
        {
            AudioSystem.Instance.audioSources.musicAudioSource.volume -= (startVolume / 0.5f) * Time.deltaTime;
            yield return null;
        }
        AudioSystem.Instance.audioSources.musicAudioSource.volume = 0; // Upewnij si�, �e g�o�no�� jest zerowa na ko�cu
        AudioSystem.Instance.audioSources.musicAudioSource.Stop(); // Zatrzymaj d�wi�k po fade-out
        AudioSystem.Instance.audioSources.musicAudioSource.clip = null; // Zwolnij zas�b audio
        AudioSystem.Instance.audioSources.musicAudioSource.volume = startVolume;
    }

    IEnumerator FinishLoading()
    {
        float targetWidth = 0; // Docelowa szeroko�� maski, aby ca�kowicie ods�oni� pasek �adowania
        while (maskingRectTransform.sizeDelta.x > 0.1f)
        {
            float currentWidth = Mathf.SmoothDamp(maskingRectTransform.sizeDelta.x, targetWidth, ref velocity, smoothTime);
            maskingRectTransform.sizeDelta = new Vector2(currentWidth, maskingRectTransform.sizeDelta.y);
            yield return null;
        }

        // Ustaw mask� na zerow� szeroko��
        maskingRectTransform.sizeDelta = new Vector2(0, maskingRectTransform.sizeDelta.y);
        Debug.Log("�adowanie zako�czone");
    }
}
