using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullMapIslandIconUI : MonoBehaviour
{
    [SerializeField] private float worldSize;
    [SerializeField] private float mapSize;
    [SerializeField] private List<Sprite> islandIcons;
    [SerializeField] private Material islandIconMaterial;
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

        IslandDataComponent[] islandDataComponents = Object.FindObjectsOfType<IslandDataComponent>();

        foreach (IslandDataComponent islandDataComponent in islandDataComponents)
        {
            Transform islandTransform = islandDataComponent.transform;

            // Create a new game object under this game object with sprite renderer and set the sprite to the island icon, then get its rect transform
            GameObject islandIconGameObject = new GameObject("IslandIcon", typeof(RectTransform));
            RectTransform islandIconTransform = islandIconGameObject.GetComponent<RectTransform>();
            islandIconTransform.SetParent(GetComponent<RectTransform>());
            SpriteRenderer islandIconSpriteRenderer = islandIconGameObject.AddComponent<SpriteRenderer>();

            // Set the sprite by string name based on the island data component data
            string islandSize = islandDataComponent.islandData.size;
            string islandType = islandDataComponent.islandData.type;
            string islandIconName = "";

            if (islandSize == "small" && islandType == "sandy")
            {
                islandIconName = "miDesSSprite";
            }
            else if (islandSize == "small" && islandType == "muddy")
            {
                islandIconName = "miMudSSprite";
            }
            else if (islandSize == "small" && islandType == "grassy")
            {
                islandIconName = "miGraSSprite";
            }
            else if (islandSize == "medium" && islandType == "sandy")
            {
                islandIconName = "miDesMSprite";
            }
            else if (islandSize == "medium" && islandType == "muddy")
            {
                islandIconName = "miMudMSprite";
            }
            else if (islandSize == "medium" && islandType == "grassy")
            {
                islandIconName = "miGraMSprite";
            }
            else if (islandSize == "large" && islandType == "sandy")
            {
                islandIconName = "miDesLSprite";
            }
            else if (islandSize == "large" && islandType == "muddy")
            {
                islandIconName = "miMudLSprite";
            }
            else if (islandSize == "large" && islandType == "grassy")
            {
                islandIconName = "miGraLSprite";
            }

            foreach (Sprite islandIcon in islandIcons)
            {
                if (islandIcon.name == islandIconName)
                {
                    islandIconGameObject.layer = 5;
                    islandIconSpriteRenderer.sprite = islandIcon;
                    islandIconSpriteRenderer.material = islandIconMaterial;
                    islandIconSpriteRenderer.sortingOrder = 21;
                    islandIconTransform.localScale = new Vector3(20f, 20f, 1f);
                    islandIconTransform.localPosition = new Vector3(islandTransform.position.x / scale, islandTransform.position.z / scale);
                    islandIconTransform.localRotation = Quaternion.Euler(0, 0, -islandTransform.eulerAngles.y);
                    break;
                }
            }
        }

        Port[] ports = Object.FindObjectsOfType<Port>();

        foreach (Port port in ports)
        {
            Transform islandTransform = port.transform;

            GameObject portIslandIconGameObject = new GameObject("PortIslandIcon", typeof(RectTransform));
            RectTransform portIslandIconTransform = portIslandIconGameObject.GetComponent<RectTransform>();
            portIslandIconTransform.SetParent(GetComponent<RectTransform>());
            SpriteRenderer portIslandIconSpriteRenderer = portIslandIconGameObject.AddComponent<SpriteRenderer>();

            string portIslandIconName = "";

            if (port.name == "MiniIslandPort")
            {
                portIslandIconName = "portSprite";
            }

            foreach (Sprite islandIcon in islandIcons)
            {
                if (islandIcon.name == portIslandIconName)
                {
                    portIslandIconGameObject.layer = 5;
                    portIslandIconSpriteRenderer.sprite = islandIcon;
                    portIslandIconSpriteRenderer.material = islandIconMaterial;
                    portIslandIconSpriteRenderer.sortingOrder = 21;
                    portIslandIconTransform.localScale = new Vector3(20f, 20f, 1f);
                    portIslandIconTransform.localPosition = new Vector3(islandTransform.position.x / scale, islandTransform.position.z / scale);
                    portIslandIconTransform.localRotation = Quaternion.Euler(0, 0, -islandTransform.eulerAngles.y);
                    break;
                }
            }
        }    
    }
}
