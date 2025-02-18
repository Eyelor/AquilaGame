using System.Collections;
using UnityEngine;

public class ObjectBehaviour : MonoBehaviour
{
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (rb.isKinematic == false)
        {
            StartCoroutine(ChangeObjectAfterStable());
        }
    }

    private IEnumerator ChangeObjectAfterStable()
    {
        Vector3 newEulerAngles = transform.eulerAngles;
        newEulerAngles.y = Random.Range(0f, 360f);
        transform.eulerAngles = newEulerAngles;

        // Ustaw minimalny próg dla zmian pozycji i rotacji
        const float positionThreshold = 0.001f;
        const float rotationThreshold = 0.001f;

        // Zapisz pocz¹tkow¹ pozycjê i rotacjê
        Vector3 previousPosition = transform.position;
        Quaternion previousRotation = transform.rotation;

        // Czekaj, a¿ pozycja i rotacja obiektu przestan¹ siê zmieniaæ
        while (true)
        {
            yield return new WaitForSeconds(0.1f); // Poczekaj 0.1 sekundy na kolejne sprawdzenie stabilnoœci
            // SprawdŸ ró¿nicê miêdzy poprzedni¹ a aktualn¹ pozycj¹ oraz rotacj¹
            float positionChange = Vector3.Distance(transform.position, previousPosition);
            float rotationChange = Quaternion.Angle(transform.rotation, previousRotation);

            // Jeœli zmiany s¹ poni¿ej progów, wyjdŸ z pêtli
            if (positionChange < positionThreshold && rotationChange < rotationThreshold)
                break;

            // Aktualizuj poprzednie wartoœci na bie¿¹co
            previousPosition = transform.position;
            previousRotation = transform.rotation;
        }

        // Ustaw obiekt jako kinematyczny po stabilizacji
        if (!rb.isKinematic)
        {
            rb.isKinematic = true;
        }

        // Upewnienie siê, ¿e warstwa obiektu to "Interactable"
        if (gameObject.layer != LayerMask.NameToLayer("Interactable"))
        {
            var objectDataComponent = GetComponent<ObjectDataComponent>();
            objectDataComponent.objectData.objectLocation = new Location
            {
                posX = transform.position.x,
                posY = transform.position.y,
                posZ = transform.position.z,

                rotX = transform.rotation.eulerAngles.x,
                rotY = transform.rotation.eulerAngles.y,
                rotZ = transform.rotation.eulerAngles.z
            };
        }
    }

}
