using UnityEngine;

public class FullMapPlayerPointerUI : MonoBehaviour
{

    [SerializeField] private RectTransform playerPointerTransform;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float worldSize;
    [SerializeField] private float mapSize;
    [SerializeField] private RectTransform borderRectTransform;

    private float scale;

#if UNITY_EDITOR
#else
    private float yOffset;
    private float xSize;
    private float baseScale;
    private float baseXSize;
    private float borderScale;
#endif

    private void Awake()
    {
#if UNITY_EDITOR
#else
        yOffset = -80.0f;
        baseScale = 99f;
        baseXSize = 1000f;
#endif
    }

    private void Start()
    {
#if UNITY_EDITOR
#else
        xSize = Screen.height - 80.0f;
        transform.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(xSize, yOffset);

        borderScale = baseScale * (xSize / baseXSize);
        borderRectTransform.localScale = new Vector3(borderScale, borderScale, 1);

        mapSize = xSize;
#endif

        scale = worldSize / mapSize;
    }

    private void Update()
    {
        playerPointerTransform.localPosition = new Vector3(playerTransform.position.x / scale, playerTransform.position.z / scale);
        playerPointerTransform.localRotation = Quaternion.Euler(0, 0, -playerTransform.eulerAngles.y - 180);
    }
}
