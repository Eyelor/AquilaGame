using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsometricCameraController : Singleton<IsometricCameraController>
{
    [SerializeField] private Transform player;    // Obiekt gracza, za kt�rym kamera ma pod��a�
    [SerializeField] private Vector3 defaultOffset;      // Domy�lne przesuni�cie kamery wzgl�dem gracza
    [SerializeField] private Vector3 collisionOffset;    // Przesuni�cie kamery, gdy wyst�puje kolizja
    [SerializeField] private float offsetSmoothSpeed = 0.2f; // Pr�dko�� wyg�adzania zmiany offsetu
    [SerializeField] private float collisionBuffer = 0.5f; // Odleg�o�� od przeszkody

    private Vector3 currentOffset; // Aktualne przesuni�cie kamery
    private Vector3 targetOffset;  // Docelowe przesuni�cie kamery
    private bool isColliding;               // Flaga do �ledzenia kolizji
    private bool isCollidingCamera;         // Flaga do �ledzenia kolizji wok� kamery
    private bool isMovingToCollisionOffset; // Flaga, aby sprawdzi�, czy kamera przechodzi do collisionOffset

    void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        GameInputSystem.Instance.SetForceCursorLockedAndInvisible(true);

        // Ustawienie domy�lnego offsetu
        if (defaultOffset == Vector3.zero)
        {
            defaultOffset = new Vector3(8, 6, -8);
        }

        // Pocz�tkowy offset to domy�lny offset
        currentOffset = defaultOffset;
        targetOffset = defaultOffset;
        isColliding = false;
        isCollidingCamera = false;
        isMovingToCollisionOffset = false;


        // Domy�lny offset kolizji
        if (collisionOffset == Vector3.zero)
        {
            collisionOffset = new Vector3(8, 12, -8); // offset przy kolizji
        }
    }

    void LateUpdate()
    {
        // Oblicz pozycj� docelow� kamery
        Vector3 desiredPosition = player.position + targetOffset;

        if (PlayerController.isTeleporting)
        {
            transform.position = player.position + currentOffset;
            transform.LookAt(player);
        }
        // Sprawdzenie kolizji tylko z TerrainCollider
        RaycastHit hit;
        isColliding = Physics.Raycast(player.position, desiredPosition - player.position, out hit, currentOffset.magnitude + collisionBuffer);

        // Dodanie sprawdzania kolizji wok� kamery za pomoc� sze�cianu 1x1x1
        Collider[] colliders = Physics.OverlapBox(transform.position, Vector3.one * 0.5f, Quaternion.identity);
        isCollidingCamera = false;
        foreach (Collider collider in colliders)
        {
            if (collider is TerrainCollider)
            {
                isCollidingCamera = true;
                break;
            }
        }

        if ((isColliding && hit.collider is TerrainCollider) || isCollidingCamera)
        {
            // Kolizja z TerrainCollider
            if (!isMovingToCollisionOffset)
            {
                // Debug.Log("P�ynne przej�cie bo kolizja.");
                // Rozpocznij p�ynne przej�cie do collisionOffset
                targetOffset = collisionOffset;
                isMovingToCollisionOffset = true;
            }

            // P�ynna zmiana offsetu do collisionOffset
            currentOffset = Vector3.Lerp(currentOffset, targetOffset, offsetSmoothSpeed * Time.deltaTime);
        }
        else
        {
            // Sprawdzenie, czy osi�gni�to collisionOffset przed przej�ciem do defaultOffset
            if (isMovingToCollisionOffset)
            {
                if (Vector3.Distance(currentOffset, collisionOffset) < 0.5f)
                {
                    // Debug.Log("Osi�gni�to collisionOffset");
                    // Ju� w collisionOffset - sprawdzenie, czy mo�na wr�ci� do defaultOffset
                    targetOffset = defaultOffset;
                    isMovingToCollisionOffset = false;
                }
                else
                {
                    // Debug.Log("Kontynuowanie do collisionOffset");
                    // Je�li kamera nie jest jeszcze w collisionOffset, kontynuuj ruch do collisionOffset
                    currentOffset = Vector3.Lerp(currentOffset, targetOffset, offsetSmoothSpeed * Time.deltaTime);
                }
            }
            else
            {
                // Brak kolizji - natychmiastowe ustawienie offsetu na defaultOffset
                targetOffset = defaultOffset;
                currentOffset = Vector3.Lerp(currentOffset, targetOffset, offsetSmoothSpeed * Time.deltaTime);
            }
        }

        // Aktualizacja pozycji kamery bez Lerp, gdy brak kolizji
        Vector3 smoothedPosition = player.position + currentOffset;
        transform.position = smoothedPosition;

        // Ustawienie rotacji kamery, aby by�a skierowana w kierunku gracza
        transform.LookAt(player);
    }
}
