using UnityEngine;
using UnityEngine.SceneManagement;

public class Port : Singleton<Port>, IInteractable
{
    [SerializeField] private string _promptShip;

    private string _interactionPrompt;
    public string InteractionPrompt
    {
        get => _interactionPrompt;
        set => _interactionPrompt = value;
    }

    public bool Interact(Interactor interactor)
    {
        if (interactor.name == "PlayerShip")
        {
            Debug.Log(_promptShip);

            //SaveManager.Instance.UpdateSavesData("Port");
            //SceneTransitionManager.nextSceneName = "Port";
            //SceneManager.LoadScene("LoadingPanel");

            return true;
        }
        else
        {
            return false;
        }
    }

    public string GetPrompt(string name)
    {
        if (name == "PlayerShip")
        {
            return _promptShip;
        }
        else
        {
            return "";
        }
    }
}
