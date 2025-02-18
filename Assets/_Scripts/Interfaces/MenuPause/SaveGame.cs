using TMPro;
using UnityEngine;

public class SaveGame : MonoBehaviour
{
    public GameObject savePanel;
    public GameObject panelYes;
    public GameObject panelNo;
    public GameObject lineObjectYes;
    public GameObject lineObjectNo;
    public GameObject goBack;
    public GameObject goBackLine;
    public GameObject SaveInfoBefore;
    public GameObject SaveInfoAfter;
    public bool mainPanel = true;
    public bool isOcean = false;

    private void OnMouseDown()
    {
        if (mainPanel)
        {
            if (!AudioSystem.Instance.audioSources.effectsAudioSource.isPlaying)
            {
                AudioSystem.Instance.PlaySound(AudioSystem.Instance.audioSources.effectsAudioSource, AudioSystem.Instance.buttonForwardSound, false);
            }
            panelYes.SetActive(false);
            panelNo.SetActive(false);
            lineObjectYes.SetActive(false);
            lineObjectNo.SetActive(false);
            goBack.SetActive(true);
            SaveInfoBefore.SetActive(false);
            SaveInfoAfter.SetActive(true);

            if (!isOcean)
            {
                PlayerDataComponent[] playerDataComponents = FindObjectsOfType<PlayerDataComponent>();
                if (playerDataComponents.Length == 1)
                {
                    GameObject player = playerDataComponents[0].gameObject;
                    player.GetComponent<PlayerDataComponent>().playerData.islandId = LocalIslandGenerationManager.Instance.currentIslandDataComponent.currentIslandData.islandId;
                    player.GetComponent<PlayerDataComponent>().playerData.location.posX = player.transform.position.x;
                    player.GetComponent<PlayerDataComponent>().playerData.location.posY = player.transform.position.y;
                    player.GetComponent<PlayerDataComponent>().playerData.location.posZ = player.transform.position.z;
                    player.GetComponent<PlayerDataComponent>().playerData.location.rotX = player.transform.rotation.eulerAngles.x;
                    player.GetComponent<PlayerDataComponent>().playerData.location.rotY = player.transform.rotation.eulerAngles.y;
                    player.GetComponent<PlayerDataComponent>().playerData.location.rotZ = player.transform.rotation.eulerAngles.z;
                    player.GetComponent<PlayerDataComponent>().playerData.isPlayerInside = true;
                }
                else
                {
                    Debug.LogError("PlayerDataComponent empty or more than one instance.");
                }
                SaveManager.Instance.SavePlayerData();
                SaveManager.Instance.SavePlayerStatistics();
                SaveManager.Instance.SavePlayerAchievements();
            }
            else
            {
                PlayerShipDataComponent[] playerShipDataComponents = FindObjectsOfType<PlayerShipDataComponent>();
                if (playerShipDataComponents.Length == 1)
                {
                    PlayerShipDataComponent playerShipDataComponent = playerShipDataComponents[0];
                    GameObject playerShip = playerShipDataComponents[0].gameObject;

                    playerShipDataComponent.playerShipData.location.posX = playerShip.transform.position.x;
                    playerShipDataComponent.playerShipData.location.posY = playerShip.transform.position.y;
                    playerShipDataComponent.playerShipData.location.posZ = playerShip.transform.position.z;
                    playerShipDataComponent.playerShipData.location.rotX = playerShip.transform.rotation.eulerAngles.x;
                    playerShipDataComponent.playerShipData.location.rotY = playerShip.transform.rotation.eulerAngles.y;
                    playerShipDataComponent.playerShipData.location.rotZ = playerShip.transform.rotation.eulerAngles.z;
                }
                else
                {
                    Debug.LogError("PlayerShipDataComponent not found or more than one instance.");
                }
                SaveManager.Instance.SavePlayerShipData();
            }
            
        }
        else
        {
            if (!AudioSystem.Instance.audioSources.effectsAudioSource.isPlaying)
            {
                AudioSystem.Instance.PlaySound(AudioSystem.Instance.audioSources.effectsAudioSource, AudioSystem.Instance.buttonForwardSound, false);
            }
            panelYes.SetActive(true);
            panelNo.SetActive(true);
            goBack.SetActive(false);
            goBackLine.SetActive(false);
            SaveInfoBefore.SetActive(true);
            SaveInfoAfter.SetActive(false);
            savePanel.SetActive(false);
            EnableBoxCollider2D();
        }

    }

    public void EnableBoxCollider2D()
    {
        // Znajdü wszystkie obiekty zawierajπce komponent "BoxCollider2D"
        BoxCollider2D[] scriptsToDisable = FindObjectsOfType<BoxCollider2D>();

        // Przejdü przez wszystkie znalezione skrypty i w≥πcz je
        foreach (BoxCollider2D script in scriptsToDisable)
        {
            // Debug.Log(script);
            script.enabled = true;
        }
    }
}
