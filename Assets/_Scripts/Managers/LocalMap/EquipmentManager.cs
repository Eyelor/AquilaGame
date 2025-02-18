using UnityEngine;
using UnityEngine.UI;

public class EquipmentManager : Singleton<EquipmentManager>
{
    [SerializeField] private GameObject equipmentCanvas;

    void Start()
    {
        if (LocalGameManager.Instance != null)
        {
            LocalGameManager.Instance.OnEquipmentActive += LocalGameManager_OnEquipmentActive;
            LocalGameManager.Instance.OnEquipmentInactive += LocalGameManager_OnEquipmentInactive;
        }
        equipmentCanvas.SetActive(false);
    }
    private void LocalGameManager_OnEquipmentInactive(object sender, System.EventArgs e)
    {
        equipmentCanvas.SetActive(false);
    }

    private void LocalGameManager_OnEquipmentActive(object sender, System.EventArgs e)
    {
        equipmentCanvas.SetActive(true);
    }

    protected override void OnDestroy()
    {
        if (LocalGameManager.Instance != null)
        {
            LocalGameManager.Instance.OnEquipmentActive -= LocalGameManager_OnEquipmentActive;
            Debug.Log($"[FullMapUIManager] OnEquipmentActive has {LocalGameManager.Instance.GetOnEquipmentActiveSubscribersCount()} subscribers after OnDestroy.");

            LocalGameManager.Instance.OnEquipmentInactive -= LocalGameManager_OnEquipmentInactive;
            Debug.Log($"[FullMapUIManager] OnEquipmentInactive has {LocalGameManager.Instance.GetOnEquipmentInactiveSubscribersCount()} subscribers after OnDestroy.");
        }
        base.OnDestroy();
    }
}
