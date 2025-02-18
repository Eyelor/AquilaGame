using UnityEngine;
using UnityEngine.SceneManagement;

public class Signpost : MonoBehaviour, IInteractable
{
    [SerializeField] private string _promptPlayer;
    [SerializeField] private GameObject _objectToRemoveFirst;
    [SerializeField] private GameObject _objectToAtivate;
    [SerializeField] private GameObject _objectToDeactivate;

    private string _interactionPrompt;
    public string InteractionPrompt
    {
        get => _interactionPrompt;
        set => _interactionPrompt = value;
    }

    public bool Interact(Interactor interactor)
    {
        if (interactor.name == "PlayerPirat")
        {
            if (AudioSystem.Instance.playMusicCoroutine != null)
            {
                AudioSystem.Instance.StopCoroutine(AudioSystem.Instance.playMusicCoroutine);
                AudioSystem.Instance.playMusicCoroutine = null;
                AudioSystem.Instance.audioSources.musicAudioSource.Stop();
                AudioSystem.Instance.PlaySound(AudioSystem.Instance.audioSources.musicAudioSource, AudioSystem.Instance.loadingPanelMusic, true);
            }
            Debug.Log(_promptPlayer);

            // Dezaktywowuj canvas
            if (_objectToDeactivate != null)
            {
                _objectToDeactivate.SetActive(false);
            }
            else
            {
                Debug.LogWarning("Nie przypisano ¿adnego GameObject do dezaktywacji.");
            }

            // Aktywuj Loading Panel
            if (_objectToAtivate != null)
            {
                _objectToAtivate.SetActive(true);
            }
            else
            {
                Debug.LogWarning("Nie przypisano ¿adnego GameObject do aktywacji.");
            }

            // SprawdŸ, czy obiekt istnieje, jeœli tak, usuñ go
            if (_objectToRemoveFirst != null)
            {
                Destroy(_objectToRemoveFirst);
            }
            SaveManager.Instance.UpdateSavesData("Mapa Globalna");
            SceneManager.LoadScene("GlobalMap");

            return true;
        }
        else
        {
            return false;
        }
    }

    public string GetPrompt(string name)
    {
        if (name == "PlayerPirat")
        {
            return _promptPlayer;
        }
        else
        {
            return "";
        }
    }
}
