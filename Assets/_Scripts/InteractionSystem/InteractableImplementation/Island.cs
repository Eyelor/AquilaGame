using UnityEngine;
using UnityEngine.SceneManagement;

public class Island : MonoBehaviour, IInteractable
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

            if (gameObject.GetComponent<IslandDataComponent>() is not null)
            {
                SaveManager saveManager = SaveManager.Instance;

                PlayerShipDataComponent[] playerShipDataComponents = FindObjectsOfType<PlayerShipDataComponent>();
                if (playerShipDataComponents.Length == 1)
                {
                    var playerShipDataComponent = playerShipDataComponents[0];
                    playerShipDataComponent.playerShipData.location.posX = interactor.transform.position.x;
                    playerShipDataComponent.playerShipData.location.posY = interactor.transform.position.y;
                    playerShipDataComponent.playerShipData.location.posZ = interactor.transform.position.z;
                    playerShipDataComponent.playerShipData.location.rotX = interactor.transform.rotation.eulerAngles.x;
                    playerShipDataComponent.playerShipData.location.rotY = interactor.transform.rotation.eulerAngles.y;
                    playerShipDataComponent.playerShipData.location.rotZ = interactor.transform.rotation.eulerAngles.z;
                    playerShipDataComponent.playerShipData.isPlayerInside = false;
                }
                else
                {
                    Debug.LogError("PlayerShipDataComponent empty or more than one instance.");
                }
                saveManager.SavePlayerShipData();

                IslandDataComponent islandDataComponent = gameObject.GetComponent<IslandDataComponent>();

                Debug.Log("Closest collider island id: " + islandDataComponent.islandData.id);
                Debug.Log("Closest collider island type: " + islandDataComponent.islandData.type);
                Debug.Log("Closest collider island size: " + islandDataComponent.islandData.size);
                Debug.Log("Closest collider island affiliation: " + islandDataComponent.islandData.affiliation);
                Debug.Log("Closest collider island x: " + islandDataComponent.islandData.location.posX);
                Debug.Log("Closest collider island z: " + islandDataComponent.islandData.location.posZ);

                // Zapis informacji o wyspie do pliku JSON currentIsland
                CurrentIslandDataComponent[] currentIslandDataComponents = FindObjectsOfType<CurrentIslandDataComponent>();
                if (currentIslandDataComponents.Length == 1)
                {
                    var currentIslandDataComponent = currentIslandDataComponents[0];
                    currentIslandDataComponent.currentIslandData.islandId = islandDataComponent.islandData.id;
                    currentIslandDataComponent.currentIslandData.islandType = islandDataComponent.islandData.type;
                    currentIslandDataComponent.currentIslandData.islandSize = islandDataComponent.islandData.size;
                    currentIslandDataComponent.currentIslandData.islandAffiliation = islandDataComponent.islandData.affiliation;
                    currentIslandDataComponent.currentIslandData.isPlayerInside = true;
                }
                else
                {
                    Debug.LogError("CurrentIslandDataComponent empty or more than one instance.");
                }
                saveManager.SaveCurrentIslandData();

                SaveManager.Instance.UpdateSavesData("Wyspa " + islandDataComponent.islandData.id);

                if (islandDataComponent.islandData.size == "large" && islandDataComponent.islandData.type == "grassy")
                {
                    SceneTransitionManager.nextSceneName = "BigIslandGrass";
                    SceneManager.LoadScene("LoadingPanel");
                }
                else if (islandDataComponent.islandData.size == "large" && islandDataComponent.islandData.type == "muddy")
                {
                    SceneTransitionManager.nextSceneName = "BigIslandMudd";
                    SceneManager.LoadScene("LoadingPanel");
                }
                else if (islandDataComponent.islandData.size == "large" && islandDataComponent.islandData.type == "sandy")
                {
                    SceneTransitionManager.nextSceneName = "BigIslandSand";
                    SceneManager.LoadScene("LoadingPanel");
                }
                else if (islandDataComponent.islandData.size == "medium" && islandDataComponent.islandData.type == "grassy")
                {
                    SceneTransitionManager.nextSceneName = "MiddleIslandGrass";
                    SceneManager.LoadScene("LoadingPanel");
                }
                else if (islandDataComponent.islandData.size == "medium" && islandDataComponent.islandData.type == "muddy")
                {
                    SceneTransitionManager.nextSceneName = "MiddleIslandMudd";
                    SceneManager.LoadScene("LoadingPanel");
                }
                else if (islandDataComponent.islandData.size == "medium" && islandDataComponent.islandData.type == "sandy")
                {
                    SceneTransitionManager.nextSceneName = "MiddleIslandSand";
                    SceneManager.LoadScene("LoadingPanel");
                }
                else if (islandDataComponent.islandData.size == "small" && islandDataComponent.islandData.type == "grassy")
                {
                    SceneTransitionManager.nextSceneName = "SmallIslandGrass";
                    SceneManager.LoadScene("LoadingPanel");
                }
                else if (islandDataComponent.islandData.size == "small" && islandDataComponent.islandData.type == "muddy")
                {
                    SceneTransitionManager.nextSceneName = "SmallIslandMudd";
                    SceneManager.LoadScene("LoadingPanel");
                }
                else if (islandDataComponent.islandData.size == "small" && islandDataComponent.islandData.type == "sandy")
                {
                    SceneTransitionManager.nextSceneName = "SmallIslandSand";
                    SceneManager.LoadScene("LoadingPanel");
                }
            }
            else
            {
                return false;
            }

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

    public void SetPromptShip(string promptShip)
    {
        _promptShip = promptShip;
    }
}
