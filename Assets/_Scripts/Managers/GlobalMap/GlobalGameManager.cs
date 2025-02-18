using System;
using UnityEngine;

public class GlobalGameManager : Singleton<GlobalGameManager>
{
    public event EventHandler OnFullMapActive;
    public event EventHandler OnFullMapInactive;

    private bool _isFullMapActive = false;

    private void Start()
    {
        GameInputSystem.Instance.OnFullMapAction += GameInputSystem_OnFullMapAction;
    }

    private void GameInputSystem_OnFullMapAction(object sender, System.EventArgs e)
    {
        ToggleFullMap();
    }

    public void ToggleFullMap()
    {
        _isFullMapActive = !_isFullMapActive;
        if (_isFullMapActive)
        {      
            OnFullMapActive?.Invoke(this, EventArgs.Empty);          
        }
        else
        {
            OnFullMapInactive?.Invoke(this, EventArgs.Empty);
        }
    }

    protected override void OnDestroy()
    {
        if (GameInputSystem.Instance != null)
        {
            GameInputSystem.Instance.OnFullMapAction -= GameInputSystem_OnFullMapAction;
            Debug.Log($"[GlobalGameManager] OnFullMapAction has {GameInputSystem.Instance.GetOnFullMapActionSubscribersCount()} subscribers after OnDestroy.");
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
}
