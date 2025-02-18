using UnityEngine;

public class FullMapFastTravelPointerLocalUI : MonoBehaviour
{
    [SerializeField] private RectTransform fastTravelPointerTransform;
    [SerializeField] private Transform fastTravelTransform;
    [SerializeField] private float worldSize;
    [SerializeField] private float mapSize;
    [SerializeField] private RectTransform borderRectTransform;

    private float scale;
    private float half;

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

        half = worldSize / 2;
    }

    private void Start()
    {
#if UNITY_EDITOR
#else
        xSize = Screen.height - 80.0f;
        if (gameObject.name == "FullMapSignpostPointer")
        {
            transform.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(xSize, yOffset);
        }
        else
        {
            transform.parent.transform.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(xSize, yOffset);
        }

        borderScale = baseScale * (xSize / baseXSize);
        borderRectTransform.localScale = new Vector3(borderScale, borderScale, 1);

        mapSize = xSize;
#endif

        scale = worldSize / mapSize;
        
        fastTravelPointerTransform.localPosition = new Vector3((fastTravelTransform.position.x - half) / scale, (fastTravelTransform.position.z - half) / scale);
    }
}
