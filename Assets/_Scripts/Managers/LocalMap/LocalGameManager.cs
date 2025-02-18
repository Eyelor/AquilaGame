using System;
using UnityEngine;

public class LocalGameManager : Singleton<LocalGameManager>
{
    public event EventHandler OnFullMapActive;
    public event EventHandler OnFullMapInactive;
    public event EventHandler OnEquipmentActive;
    public event EventHandler OnEquipmentInactive;

    private bool _isFullMapActive = false;
    private bool _isEquipmentActive = false;

    private void Start()
    {
        GameInputSystem.Instance.OnFullMapAction += GameInputSystem_OnFullMapAction;
        GameInputSystem.Instance.OnEquipmentAction += GameInputSystem_OnEquipmentAction;
    }

    private void GameInputSystem_OnFullMapAction(object sender, System.EventArgs e)
    {
        ToggleFullMap();
    }

    private void GameInputSystem_OnEquipmentAction(object sender, System.EventArgs e)
    {
        ToggleEquipment();
    }

    public void ToggleFullMap()
    {
        if (Time.timeScale != 0f)
        {
            _isFullMapActive = !_isFullMapActive;
            if (_isFullMapActive)
            {
                if (_isEquipmentActive)
                {
                    ToggleEquipment();
                }
                OnFullMapActive?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                OnFullMapInactive?.Invoke(this, EventArgs.Empty);
            }
        } 
    }

    public void ToggleEquipment()
    {
        if (Time.timeScale != 0f)
        {
            _isEquipmentActive = !_isEquipmentActive;
            if (_isEquipmentActive)
            {
                if (_isFullMapActive)
                {
                    ToggleFullMap();
                }
                OnEquipmentActive?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                OnEquipmentInactive?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    protected override void OnDestroy()
    {
        if (GameInputSystem.Instance != null)
        {
            GameInputSystem.Instance.OnFullMapAction -= GameInputSystem_OnFullMapAction;
            Debug.Log($"[LocalGameManager] OnFullMapAction has {GameInputSystem.Instance.GetOnFullMapActionSubscribersCount()} subscribers after OnDestroy.");
            GameInputSystem.Instance.OnEquipmentAction -= GameInputSystem_OnEquipmentAction;
            Debug.Log($"[LocalGameManager] OnEquipmentAction has {GameInputSystem.Instance.GetOnEquipmentActionSubscribersCount()} subscribers after OnDestroy.");
        }

        base.OnDestroy();
    }

    public int GetOnFullMapActiveSubscribersCount()
    {
        return OnFullMapActive != null ? OnFullMapActive.GetInvocationList().Length : 0;
    }

    public int GetOnFullMapInactiveSubscribersCount()
    {
        return OnFullMapInactive != null ? OnFullMapInactive.GetInvocationList().Length : 0;
    }

    public int GetOnEquipmentActiveSubscribersCount()
    {
        return OnEquipmentActive != null ? OnEquipmentActive.GetInvocationList().Length : 0;
    }

    public int GetOnEquipmentInactiveSubscribersCount()
    {
        return OnEquipmentInactive != null ? OnEquipmentInactive.GetInvocationList().Length : 0;
    }
}
