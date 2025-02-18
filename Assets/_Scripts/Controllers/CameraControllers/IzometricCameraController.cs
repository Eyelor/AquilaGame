using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsometricCameraController : Singleton<IsometricCameraController>
{
    [SerializeField] private Transform player;    // Obiekt gracza, za którym kamera ma pod¹¿aæ
    [SerializeField] private Vector3 defaultOffset;      // Domyœlne przesuniêcie kamery wzglêdem gracza
    [SerializeField] private Vector3 collisionOffset;    // Przesuniêcie kamery, gdy wystêpuje kolizja
    [SerializeField] private float offsetSmoothSpeed = 0.2f; // Prêdkoœæ wyg³adzania zmiany offsetu
    [SerializeField] private float collisionBuffer = 0.5f; // Odleg³oœæ od przeszkody

    private Vector3 currentOffset; // Aktualne przesuniêcie kamery
    private Vector3 targetOffset;  // Docelowe przesuniêcie kamery
    private bool isColliding;               // Flaga do œledzenia kolizji
    private bool isCollidingCamera;         // Flaga do œledzenia kolizji wokó³ kamery
    private bool isMovingToCollisionOffset; // Flaga, aby sprawdziæ, czy kamera przechodzi do collisionOffset

    void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        GameInputSystem.Instance.SetForceCursorLockedAndInvisible(true);

        // Ustawienie domyœlnego offsetu
        if (defaultOffset == Vector3.zero)
        {
            defaultOffset = new Vector3(8, 6, -8);
        }

        // Pocz¹tkowy offset to domyœlny offset
        currentOffset = defaultOffset;
        targetOffset = defaultOffset;
        isColliding = false;
        isCollidingCamera = false;
        isMovingToCollisionOffset = false;


        // Domyœlny offset kolizji
        if (collisionOffset == Vector3.zero)
        {
            collisionOffset = new Vector3(8, 12, -8); // offset przy kolizji
        }
    }

    void LateUpdate()
    {
        // Oblicz pozycjê docelow¹ kamery
        Vector3 desiredPosition = player.position + targetOffset;

        if (PlayerController.isTeleporting)
        {
            transform.position = player.position + currentOffset;
            transform.LookAt(player);
        }
        // Sprawdzenie kolizji tylko z TerrainCollider
        RaycastHit hit;
        isColliding = Physics.Raycast(player.position, desiredPosition - player.position, out hit, currentOffset.magnitude + collisionBuffer);

        // Dodanie sprawdzania kolizji wokó³ kamery za pomoc¹ szeœcianu 1x1x1
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
                // Debug.Log("P³ynne przejœcie bo kolizja.");
                // Rozpocznij p³ynne przejœcie do collisionOffset
                targetOffset = collisionOffset;
                isMovingToCollisionOffset = true;
            }

            // P³ynna zmiana offsetu do collisionOffset
            currentOffset = Vector3.Lerp(currentOffset, targetOffset, offsetSmoothSpeed * Time.deltaTime);
        }
        else
        {
            // Sprawdzenie, czy osi¹gniêto collisionOffset przed przejœciem do defaultOffset
            if (isMovingToCollisionOffset)
            {
                if (Vector3.Distance(currentOffset, collisionOffset) < 0.5f)
                {
                    // Debug.Log("Osi¹gniêto collisionOffset");
                    // Ju¿ w collisionOffset - sprawdzenie, czy mo¿na wróciæ do defaultOffset
                    targetOffset = defaultOffset;
                    isMovingToCollisionOffset = false;
                }
                else
                {
                    // Debug.Log("Kontynuowanie do collisionOffset");
                    // Jeœli kamera nie jest jeszcze w collisionOffset, kontynuuj ruch do collisionOffset
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

        // Ustawienie rotacji kamery, aby by³a skierowana w kierunku gracza
        transform.LookAt(player);
    }
}
