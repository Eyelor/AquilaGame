using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Unity.VisualScripting;
using System;

public class LoadingPanelEndManager : Singleton<LoadingPanelEndManager>
{
    public GameObject objectToDeactivate; // GameObject, który chcesz dezaktywowaæ
    public GameObject pauseCanvasToActivate; // GameObject, który chcesz dezaktywowaæ
    public GameObject objectToActivate; // GameObject, który chcesz aktywowaæ po dezaktywacji obiektu
    public GameObject playerToActivate; // Player, który chcesz aktywowaæ po dezaktywacji
    public float delayBeforeAction = 4f; // Czas oczekiwania przed dezaktywacj¹ obiektu
    public Image loadingBar; // Pasek ³adowania, typ UI Image, który u¿ywa Sprite (SVG)
    public Image maskingImage; // Maskuj¹cy obiekt Image z kolorem

    private float smoothTime = 0.5f; // Czas wyg³adzania przejœcia miêdzy krokami
    private RectTransform maskingRectTransform; // RectTransform maskuj¹cego obiektu
    private float initialWidth; // Pocz¹tkowa szerokoœæ paska ³adowania
    private float targetWidth; // Szerokoœæ docelowa dla aktualnego kroku
    private float velocity = 0.0f; // Prêdkoœæ dla SmoothDamp
    private AsyncOperation loadingOperation; // Operacja asynchronicznego ³adowania sceny

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

        // Ustaw pocz¹tkow¹ szerokoœæ maskuj¹cego obiektu na pe³n¹ szerokoœæ paska
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

        // Wywo³aj `FinishLoading` jako korutynê
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
        // Czekaj okreœlon¹ iloœæ czasu przed dezaktywacj¹ Panelu £adowania
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
            Debug.LogWarning("Nie przypisano ¿adnego obiektu Playera do aktywacji.");
        }

        // Dezaktywowuj panel
        if (objectToDeactivate != null)
        {
            objectToDeactivate.SetActive(false);
            Debug.Log($"GameObject '{objectToDeactivate.name}' zosta³ dezaktywowany.");
        }
        else
        {
            Debug.LogWarning("Nie przypisano ¿adnego GameObject do dezaktywacji.");
        }

        // Aktywuj Canvas
        if (objectToActivate != null)
        {
            objectToActivate.SetActive(true);
            Debug.Log($"GameObject '{objectToActivate.name}' zosta³ aktywowany.");
        }
        else
        {
            Debug.LogWarning("Nie przypisano ¿adnego GameObject do aktywacji.");
        }
        // Aktywuj Canvas Menu Pausy
        if (pauseCanvasToActivate != null)
        {
            pauseCanvasToActivate.SetActive(true);
            Debug.Log($"GameObject '{pauseCanvasToActivate.name}' zosta³ aktywowany.");
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
        AudioSystem.Instance.audioSources.musicAudioSource.volume = 0; // Upewnij siê, ¿e g³oœnoœæ jest zerowa na koñcu
        AudioSystem.Instance.audioSources.musicAudioSource.Stop(); // Zatrzymaj dŸwiêk po fade-out
        AudioSystem.Instance.audioSources.musicAudioSource.clip = null; // Zwolnij zasób audio
        AudioSystem.Instance.audioSources.musicAudioSource.volume = startVolume;
    }

    IEnumerator FinishLoading()
    {
        float targetWidth = 0; // Docelowa szerokoœæ maski, aby ca³kowicie ods³oniæ pasek ³adowania
        while (maskingRectTransform.sizeDelta.x > 0.1f)
        {
            float currentWidth = Mathf.SmoothDamp(maskingRectTransform.sizeDelta.x, targetWidth, ref velocity, smoothTime);
            maskingRectTransform.sizeDelta = new Vector2(currentWidth, maskingRectTransform.sizeDelta.y);
            yield return null;
        }

        // Ustaw maskê na zerow¹ szerokoœæ
        maskingRectTransform.sizeDelta = new Vector2(0, maskingRectTransform.sizeDelta.y);
        Debug.Log("£adowanie zakoñczone");
    }
}
