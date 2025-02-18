using UnityEngine;

public class IslandDataComponent : MonoBehaviour
{
    public IslandData islandData;
    
    void Awake()
    {
        // Initialize island data
        if (islandData == null)
        {
            islandData = new IslandData
            {
                id = 0,
                type = "default",
                size = "default",
                affiliation = "default",
                location = new Location
                {
                    posX = transform.position.x,
                    posY = transform.position.y,
                    posZ = transform.position.z,
                    rotX = transform.rotation.eulerAngles.x,
                    rotY = transform.rotation.eulerAngles.y,
                    rotZ = transform.rotation.eulerAngles.z
                },
                prefabName = "default"
            };
        }
    }
}
