using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement; // Importowanie przestrzeni nazw do zmiany scen
using UnityEngine.UI; // Importowanie przestrzeni nazw do obs�ugi UI

public class PremenuController : MonoBehaviour
{
    public TextMeshProUGUI textMeshPro; // Odwo�anie do komponentu TextMeshPro
    public float fadeOutDuration = 1.5f; // Czas trwania zanikania do 0% przezroczysto�ci
    public float fadeInDuration = 1.0f; // Czas trwania zanikania do 100% przezroczysto�ci
    public RectTransform panel; // Panel pierwszej sceny
    public RectTransform nextScenePanel; // Panel drugiej sceny, kt�ry b�dzie wchodzi� od do�u
    public float slideDuration = 1.0f; // Czas trwania animacji przesuni�cia
    public string sceneToLoad = "MainMenu"; // Nazwa sceny do za�adowania

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
            textMeshPro = GetComponent<TextMeshProUGUI>(); // Pobiera komponent TextMeshPro je�li nie jest przypisany
            Debug.LogError("TextMeshPro component not assigned!");
            return;
        }

        if (panel == null || nextScenePanel == null)
        {
            Debug.LogError("Panel or nextScenePanel not assigned!");
            return;
        }

        // Ustaw panel drugiej sceny pocz�tkowo poza ekranem na dole
        Vector2 offscreenPosition = new Vector2(nextScenePanel.anchoredPosition.x, -Screen.height);
        nextScenePanel.anchoredPosition = offscreenPosition;

        // Dodajemy nas�uchiwacz klikni��
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
        while (doAnimation) // Powtarzaj w niesko�czono��
        {
            // Fade in (powr�t do pe�nej widoczno�ci)
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

        // Upewnij si�, �e na ko�cu ma dok�adnie ��dan� warto�� alpha
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
        Vector2 endPos1 = startPos1 + new Vector2(0, Screen.height); // Przesu� w g�r� o wysoko�� ekranu

        Vector2 startPos2 = nextScenePanel.anchoredPosition;
        Vector2 endPos2 = new Vector2(startPos2.x, 0); // Ko�cowa pozycja na �rodku ekranu

        float elapsed = 0.0f;

        while (elapsed < slideDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsed / slideDuration); // U�ywamy SmoothStep dla p�ynnego przej�cia

            // Animacja dla obu paneli
            panel.anchoredPosition = Vector2.Lerp(startPos1, endPos1, t);
            nextScenePanel.anchoredPosition = Vector2.Lerp(startPos2, endPos2, t);
            yield return null;
        }

        // Upewnij si�, �e oba panele ko�cz� na odpowiednich pozycjach
        panel.anchoredPosition = endPos1;
        nextScenePanel.anchoredPosition = endPos2;

        // Za�aduj now� scen� po zako�czeniu animacji
        SceneManager.LoadScene(sceneToLoad);
    }
}
