using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoadingPanel : MonoBehaviour
{
    public Image loadingBar; // Pasek ³adowania, typ UI Image, który u¿ywa Sprite (SVG)
    public Image maskingImage; // Maskuj¹cy obiekt Image z kolorem

    private float smoothTime = 0.5f; // Czas wyg³adzania przejœcia miêdzy krokami
    private RectTransform maskingRectTransform; // RectTransform maskuj¹cego obiektu
    private float initialWidth; // Pocz¹tkowa szerokoœæ paska ³adowania
    private float velocity = 0.0f; // Prêdkoœæ dla SmoothDamp
    private AsyncOperation loadingOperation; // Operacja asynchronicznego ³adowania sceny
    private float fakeProgress = 0f; // Sztuczny postêp dla p³ynnego ³adowania
    private float[] checkpoints; // Tablica punktów kontrolnych dla postêpu
    private int currentCheckpointIndex = 0; // Indeks aktualnego punktu kontrolnego
    private float currentWidth;

    private void Start()
    {
        AudioSystem.Instance.audioSources.StopAllSounds(true);
        if (!AudioSystem.Instance.audioSources.musicAudioSource.isPlaying)
        {
            AudioSystem.Instance.PlaySound(AudioSystem.Instance.audioSources.musicAudioSource, AudioSystem.Instance.loadingPanelMusic, true);
        }

        if (loadingBar == null || maskingImage == null)
        {
            Debug.LogError("Loading bar or masking image is not assigned.");
            return;
        }

        maskingRectTransform = maskingImage.GetComponent<RectTransform>();
        initialWidth = loadingBar.rectTransform.rect.width;

        // Ustaw pocz¹tkow¹ szerokoœæ maskuj¹cego obiektu na pe³n¹ szerokoœæ paska
        maskingRectTransform.sizeDelta = new Vector2(initialWidth, maskingRectTransform.sizeDelta.y);

        // Ustal punkty kontrolne dla sztucznego postêpu ³adowania
        SetFakeProgressCheckpoints();

        // Rozpocznij asynchroniczne ³adowanie sceny docelowej
        StartCoroutine(LoadTargetSceneAsync());
    }

    void SetFakeProgressCheckpoints()
    {
        // Losowo generowane punkty kontrolne miêdzy 0 a 0.9, aby symulowaæ skoki w ³adowaniu
        checkpoints = new float[Random.Range(3, 6)]; // Od 3 do 5 punktów kontrolnych
        for (int i = 0; i < checkpoints.Length; i++)
        {
            checkpoints[i] = Random.Range((float)i / checkpoints.Length, (float)(i + 1) / checkpoints.Length) * 0.9f;
        }
        checkpoints[checkpoints.Length - 1] = 0.9f; // Ostatni checkpoint ustawiamy na 0.9
    }

    IEnumerator LoadTargetSceneAsync()
    {
        // Rozpocznij ³adowanie sceny docelowej w tle
        loadingOperation = SceneManager.LoadSceneAsync(SceneTransitionManager.nextSceneName);
        loadingOperation.allowSceneActivation = false; // Zapobiegaj automatycznemu prze³¹czeniu sceny po zakoñczeniu ³adowania

        while (fakeProgress < 0.9f || loadingOperation.progress < 0.9f || currentWidth > initialWidth * 0.11f) // Dopóki ³adowanie nie osi¹gnie 90%
        {
            // Sztuczne zwiêkszanie postêpu
            if (fakeProgress < checkpoints[currentCheckpointIndex])
            {
                fakeProgress += Time.deltaTime * Random.Range(0.05f, 0.3f); // Losowe tempo ³adowania do nastêpnego checkpointu
                fakeProgress = Mathf.Min(fakeProgress, checkpoints[currentCheckpointIndex]); // Ogranicz postêp do aktualnego checkpointu
            }
            else
            {
                // PrzejdŸ do nastêpnego checkpointu
                if (currentCheckpointIndex < checkpoints.Length - 1)
                {
                    // Debug.Log("Checkpoint osi¹gniêty: " + checkpoints[currentCheckpointIndex]);
                    yield return StartCoroutine(PauseAtCheckpoint()); // Pauza przed przejœciem do nastêpnego checkpointu
                    currentCheckpointIndex++;
                }
            }

            // U¿yj mniejszej z wartoœci: fakeProgress lub rzeczywistego postêpu ³adowania
            float progress = Mathf.Min(fakeProgress, loadingOperation.progress / 0.9f);
            // Debug.Log("Postêp ³adowania: " + progress);

            // Oblicz szerokoœæ maski na podstawie postêpu ³adowania
            currentWidth = Mathf.SmoothDamp(maskingRectTransform.sizeDelta.x, initialWidth * (1 - progress), ref velocity, smoothTime);
            // Debug.Log("Szerokoœæ maski: " + currentWidth);

            // Ustaw now¹ szerokoœæ maskuj¹cego obiektu
            maskingRectTransform.sizeDelta = new Vector2(currentWidth, maskingRectTransform.sizeDelta.y);

            // Monitoruj status ³adowania
            // Debug.Log($"Actual Progress: {loadingOperation.progress * 100}% | Fake Progress: {fakeProgress * 100}% | Progress: {progress * 100}%");

            yield return null; // Czekaj na nastêpn¹ klatkê
        }

        // Teraz mo¿na aktywowaæ za³adowan¹ scenê
        loadingOperation.allowSceneActivation = true;
    }

    IEnumerator PauseAtCheckpoint()
    {
        // Pauza przy ka¿dym punkcie kontrolnym
        yield return new WaitForSeconds(Random.Range(0.5f, 1.5f)); // Losowy czas pauzy miêdzy 0.5 a 1.5 sekundy
    }
}
