using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement; // Importowanie przestrzeni nazw do zmiany scen
using UnityEngine.UI; // Importowanie przestrzeni nazw do obs³ugi UI

public class PremenuController : MonoBehaviour
{
    public TextMeshProUGUI textMeshPro; // Odwo³anie do komponentu TextMeshPro
    public float fadeOutDuration = 1.5f; // Czas trwania zanikania do 0% przezroczystoœci
    public float fadeInDuration = 1.0f; // Czas trwania zanikania do 100% przezroczystoœci
    public RectTransform panel; // Panel pierwszej sceny
    public RectTransform nextScenePanel; // Panel drugiej sceny, który bêdzie wchodzi³ od do³u
    public float slideDuration = 1.0f; // Czas trwania animacji przesuniêcia
    public string sceneToLoad = "MainMenu"; // Nazwa sceny do za³adowania

    private bool doAnimation = true;

    private void Awake()
    {
        
    }
    private void Start()
    {
        // Rozpocznij odtwarzanie muzyki menu
        AudioSystem.Instance.PlaySound(AudioSystem.Instance.audioSources.musicAudioSource, AudioSystem.Instance.menuMusic, true);

        if (textMeshPro == null)
        {
            textMeshPro = GetComponent<TextMeshProUGUI>(); // Pobiera komponent TextMeshPro jeœli nie jest przypisany
            Debug.LogError("TextMeshPro component not assigned!");
            return;
        }

        if (panel == null || nextScenePanel == null)
        {
            Debug.LogError("Panel or nextScenePanel not assigned!");
            return;
        }

        // Ustaw panel drugiej sceny pocz¹tkowo poza ekranem na dole
        Vector2 offscreenPosition = new Vector2(nextScenePanel.anchoredPosition.x, -Screen.height);
        nextScenePanel.anchoredPosition = offscreenPosition;

        // Dodajemy nas³uchiwacz klikniêæ
        Button button = textMeshPro.GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(OnTextClicked);
        }
        else
        {
            Debug.LogError("Button component not found on TextMeshPro object!");
        }

        StartCoroutine(AnimateTextAlpha());
    }

    private IEnumerator AnimateTextAlpha()
    {
        while (doAnimation) // Powtarzaj w nieskoñczonoœæ
        {
            // Fade in (powrót do pe³nej widocznoœci)
            yield return StartCoroutine(FadeTextAlpha(0.0f, 1.0f, fadeInDuration));

            // Fade out (zanikanie)
            yield return StartCoroutine(FadeTextAlpha(1.0f, 0.0f, fadeOutDuration));
        }
    }

    private IEnumerator FadeTextAlpha(float startAlpha, float endAlpha, float duration)
    {
        float elapsed = 0.0f;
        Color originalColor = textMeshPro.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            textMeshPro.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        // Upewnij siê, ¿e na koñcu ma dok³adnie ¿¹dan¹ wartoœæ alpha
        textMeshPro.color = new Color(originalColor.r, originalColor.g, originalColor.b, endAlpha);
    }

    private void OnTextClicked()
    {
        doAnimation = false;
        StartCoroutine(SlideUpAndLoadScene());
    }

    private IEnumerator SlideUpAndLoadScene()
    {
        Vector2 startPos1 = panel.anchoredPosition;
        Vector2 endPos1 = startPos1 + new Vector2(0, Screen.height); // Przesuñ w górê o wysokoœæ ekranu

        Vector2 startPos2 = nextScenePanel.anchoredPosition;
        Vector2 endPos2 = new Vector2(startPos2.x, 0); // Koñcowa pozycja na œrodku ekranu

        float elapsed = 0.0f;

        while (elapsed < slideDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsed / slideDuration); // U¿ywamy SmoothStep dla p³ynnego przejœcia

            // Animacja dla obu paneli
            panel.anchoredPosition = Vector2.Lerp(startPos1, endPos1, t);
            nextScenePanel.anchoredPosition = Vector2.Lerp(startPos2, endPos2, t);
            yield return null;
        }

        // Upewnij siê, ¿e oba panele koñcz¹ na odpowiednich pozycjach
        panel.anchoredPosition = endPos1;
        nextScenePanel.anchoredPosition = endPos2;

        // Za³aduj now¹ scenê po zakoñczeniu animacji
        SceneManager.LoadScene(sceneToLoad);
    }
}
