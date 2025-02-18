using UnityEngine;
using UnityEngine.UI;

public class FullMapUIManager : Singleton<FullMapUIManager>
{
    [SerializeField] private GameObject fullMapCanvas;
    [SerializeField] private GameObject fullMapRawImage;
    [SerializeField] private Camera fullMapCamera;
    [SerializeField] private RenderTexture fullMapRenderTexture;

    private void Start()
    {
        if (GlobalGameManager.Instance != null)
        {
            GlobalGameManager.Instance.OnFullMapActive += GlobalGameManager_OnFullMapActive;
            GlobalGameManager.Instance.OnFullMapInactive += GlobalGameManager_OnFullMapInactive;
        }
        else if (LocalGameManager.Instance != null)
        {
            LocalGameManager.Instance.OnFullMapActive += LocalGameManager_OnFullMapActive;
            LocalGameManager.Instance.OnFullMapInactive += LocalGameManager_OnFullMapInactive;
        }
        else if (PortGameManager.Instance != null)
        {
            PortGameManager.Instance.OnFullMapActive += PortGameManager_OnFullMapActive;
            PortGameManager.Instance.OnFullMapInactive += PortGameManager_OnFullMapInactive;
        }

        SetUpFullMapTexture();

        Hide();
    }

    private void PortGameManager_OnFullMapInactive(object sender, System.EventArgs e)
    {
        Hide();
    }

    private void PortGameManager_OnFullMapActive(object sender, System.EventArgs e)
    {
        Show();
    }

    private void LocalGameManager_OnFullMapInactive(object sender, System.EventArgs e)
    {
        Hide();
    }

    private void LocalGameManager_OnFullMapActive(object sender, System.EventArgs e)
    {
        Show();
    }

    private void GlobalGameManager_OnFullMapInactive(object sender, System.EventArgs e)
    {
        Hide();
    }

    private void GlobalGameManager_OnFullMapActive(object sender, System.EventArgs e)
    {
        Show();
    }

    private void Show()
    {
        fullMapCanvas.SetActive(true);
    }

    private void Hide()
    {
        fullMapCanvas.SetActive(false);
    }

    private void SetUpFullMapTexture()
    {
        fullMapCamera.targetTexture = fullMapRenderTexture;
        fullMapCamera.Render();

        RenderTexture.active = fullMapRenderTexture;
        Texture2D texture = new Texture2D(fullMapRenderTexture.width, fullMapRenderTexture.height, TextureFormat.ARGB32, false);
        texture.ReadPixels(new Rect(0, 0, fullMapRenderTexture.width, fullMapRenderTexture.height), 0, 0);
        texture.Apply();

        fullMapRawImage.GetComponent<RawImage>().texture = texture;
        
        fullMapCamera.targetTexture = null;
        RenderTexture.active = null;
    }

    protected override void OnDestroy()
    {
        if (GlobalGameManager.Instance != null)
        {
            GlobalGameManager.Instance.OnFullMapActive -= GlobalGameManager_OnFullMapActive;
            Debug.Log($"[FullMapUIManager] OnFullMapActive has {GlobalGameManager.Instance.GetOnFullMapActiveSubscribersCount()} subscribers after OnDestroy.");

            GlobalGameManager.Instance.OnFullMapInactive -= GlobalGameManager_OnFullMapInactive;
            Debug.Log($"[FullMapUIManager] OnFullMapInactive has {GlobalGameManager.Instance.GetOnFullMapInactiveSubscribersCount()} subscribers after OnDestroy.");
        }
        else if (LocalGameManager.Instance != null)
        {
            LocalGameManager.Instance.OnFullMapActive -= LocalGameManager_OnFullMapActive;
            Debug.Log($"[FullMapUIManager] OnFullMapActive has {LocalGameManager.Instance.GetOnFullMapActiveSubscribersCount()} subscribers after OnDestroy.");

            LocalGameManager.Instance.OnFullMapInactive -= LocalGameManager_OnFullMapInactive;
            Debug.Log($"[FullMapUIManager] OnFullMapInactive has {LocalGameManager.Instance.GetOnFullMapInactiveSubscribersCount()} subscribers after OnDestroy.");
        }
        else if (PortGameManager.Instance != null)
        {
            PortGameManager.Instance.OnFullMapActive -= PortGameManager_OnFullMapActive;
            Debug.Log($"[FullMapUIManager] OnFullMapActive has {PortGameManager.Instance.GetOnFullMapActiveSubscribersCount()} subscribers after OnDestroy.");

            PortGameManager.Instance.OnFullMapInactive -= PortGameManager_OnFullMapInactive;
            Debug.Log($"[FullMapUIManager] OnFullMapInactive has {PortGameManager.Instance.GetOnFullMapInactiveSubscribersCount()} subscribers after OnDestroy.");
        }
        base.OnDestroy();
    }
}
