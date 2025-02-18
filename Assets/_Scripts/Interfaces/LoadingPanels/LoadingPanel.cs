using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoadingPanel : MonoBehaviour
{
    public Image loadingBar; // Pasek �adowania, typ UI Image, kt�ry u�ywa Sprite (SVG)
    public Image maskingImage; // Maskuj�cy obiekt Image z kolorem

    private float smoothTime = 0.5f; // Czas wyg�adzania przej�cia mi�dzy krokami
    private RectTransform maskingRectTransform; // RectTransform maskuj�cego obiektu
    private float initialWidth; // Pocz�tkowa szeroko�� paska �adowania
    private float velocity = 0.0f; // Pr�dko�� dla SmoothDamp
    private AsyncOperation loadingOperation; // Operacja asynchronicznego �adowania sceny
    private float fakeProgress = 0f; // Sztuczny post�p dla p�ynnego �adowania
    private float[] checkpoints; // Tablica punkt�w kontrolnych dla post�pu
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

        // Ustaw pocz�tkow� szeroko�� maskuj�cego obiektu na pe�n� szeroko�� paska
        maskingRectTransform.sizeDelta = new Vector2(initialWidth, maskingRectTransform.sizeDelta.y);

        // Ustal punkty kontrolne dla sztucznego post�pu �adowania
        SetFakeProgressCheckpoints();

        // Rozpocznij asynchroniczne �adowanie sceny docelowej
        StartCoroutine(LoadTargetSceneAsync());
    }

    void SetFakeProgressCheckpoints()
    {
        // Losowo generowane punkty kontrolne mi�dzy 0 a 0.9, aby symulowa� skoki w �adowaniu
        checkpoints = new float[Random.Range(3, 6)]; // Od 3 do 5 punkt�w kontrolnych
        for (int i = 0; i < checkpoints.Length; i++)
        {
            checkpoints[i] = Random.Range((float)i / checkpoints.Length, (float)(i + 1) / checkpoints.Length) * 0.9f;
        }
        checkpoints[checkpoints.Length - 1] = 0.9f; // Ostatni checkpoint ustawiamy na 0.9
    }

    IEnumerator LoadTargetSceneAsync()
    {
        // Rozpocznij �adowanie sceny docelowej w tle
        loadingOperation = SceneManager.LoadSceneAsync(SceneTransitionManager.nextSceneName);
        loadingOperation.allowSceneActivation = false; // Zapobiegaj automatycznemu prze��czeniu sceny po zako�czeniu �adowania

        while (fakeProgress < 0.9f || loadingOperation.progress < 0.9f || currentWidth > initialWidth * 0.11f) // Dop�ki �adowanie nie osi�gnie 90%
        {
            // Sztuczne zwi�kszanie post�pu
            if (fakeProgress < checkpoints[currentCheckpointIndex])
            {
                fakeProgress += Time.deltaTime * Random.Range(0.05f, 0.3f); // Losowe tempo �adowania do nast�pnego checkpointu
                fakeProgress = Mathf.Min(fakeProgress, checkpoints[currentCheckpointIndex]); // Ogranicz post�p do aktualnego checkpointu
            }
            else
            {
                // Przejd� do nast�pnego checkpointu
                if (currentCheckpointIndex < checkpoints.Length - 1)
                {
                    // Debug.Log("Checkpoint osi�gni�ty: " + checkpoints[currentCheckpointIndex]);
                    yield return StartCoroutine(PauseAtCheckpoint()); // Pauza przed przej�ciem do nast�pnego checkpointu
                    currentCheckpointIndex++;
                }
            }

            // U�yj mniejszej z warto�ci: fakeProgress lub rzeczywistego post�pu �adowania
            float progress = Mathf.Min(fakeProgress, loadingOperation.progress / 0.9f);
            // Debug.Log("Post�p �adowania: " + progress);

            // Oblicz szeroko�� maski na podstawie post�pu �adowania
            currentWidth = Mathf.SmoothDamp(maskingRectTransform.sizeDelta.x, initialWidth * (1 - progress), ref velocity, smoothTime);
            // Debug.Log("Szeroko�� maski: " + currentWidth);

            // Ustaw now� szeroko�� maskuj�cego obiektu
            maskingRectTransform.sizeDelta = new Vector2(currentWidth, maskingRectTransform.sizeDelta.y);

            // Monitoruj status �adowania
            // Debug.Log($"Actual Progress: {loadingOperation.progress * 100}% | Fake Progress: {fakeProgress * 100}% | Progress: {progress * 100}%");

            yield return null; // Czekaj na nast�pn� klatk�
        }

        // Teraz mo�na aktywowa� za�adowan� scen�
        loadingOperation.allowSceneActivation = true;
    }

    IEnumerator PauseAtCheckpoint()
    {
        // Pauza przy ka�dym punkcie kontrolnym
        yield return new WaitForSeconds(Random.Range(0.5f, 1.5f)); // Losowy czas pauzy mi�dzy 0.5 a 1.5 sekundy
    }
}
