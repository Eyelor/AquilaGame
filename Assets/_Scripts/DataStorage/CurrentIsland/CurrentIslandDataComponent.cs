using UnityEngine;

public class CurrentIslandDataComponent : MonoBehaviour
{
    public CurrentIslandData currentIslandData;

    void Awake()
    {
        if (currentIslandData == null)
        {
            currentIslandData = new CurrentIslandData
            {
                islandId = 0,
                islandType = "default",
                islandSize = "default",
                islandAffiliation = "default",
                isPlayerInside = false
            };
        }
    }

}
