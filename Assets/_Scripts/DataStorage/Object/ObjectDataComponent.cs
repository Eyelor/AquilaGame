using UnityEngine;

public class ObjectDataComponent : MonoBehaviour
{
    public ObjectData objectData;

    void Awake()
    {
        // Inicjalizacja danych obiektów
        if (objectData == null)
        {
            objectData = new ObjectData
            {
                id = 0,
                name = "default",
                islandId = 0,
                objectLocation = new Location
                {
                    posX = transform.position.x,
                    posY = transform.position.y,
                    posZ = transform.position.z,

                    rotX = transform.rotation.eulerAngles.x,
                    rotY = transform.rotation.eulerAngles.y,
                    rotZ = transform.rotation.eulerAngles.z
                }
            };
        }
    }
}
