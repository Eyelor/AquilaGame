using UnityEngine;

public class PlayerDataComponent : MonoBehaviour
{
    public PlayerData playerData;

    void Awake()
    {
        if (playerData == null)
        {
            playerData = new PlayerData
            {
                islandId = 0,
                location = new Location
                {
                    posX = transform.position.x,
                    posY = transform.position.y,
                    posZ = transform.position.z,
                    rotX = transform.rotation.eulerAngles.x,
                    rotY = transform.rotation.eulerAngles.y,
                    rotZ = transform.rotation.eulerAngles.z
                },
                isPlayerInside = false
            };
        }
    }
}
