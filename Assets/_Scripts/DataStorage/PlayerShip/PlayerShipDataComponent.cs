using UnityEngine;

public class PlayerShipDataComponent : MonoBehaviour
{
    public PlayerShipData playerShipData;

    void Awake()
    {
        if (playerShipData == null)
        {
            playerShipData = new PlayerShipData
            {
                location = new Location
                {
                    posX = transform.position.x,
                    posY = transform.position.y,
                    posZ = transform.position.z,
                    rotX = transform.rotation.eulerAngles.x,
                    rotY = transform.rotation.eulerAngles.y,
                    rotZ = transform.rotation.eulerAngles.z
                },
                isPlayerInside = true
            };
        }
    }
}
