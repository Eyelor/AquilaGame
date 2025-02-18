using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : Singleton<CameraController>
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Vector3 distance;
    [SerializeField] private float lookUp;
    [SerializeField] private float lerpAmount;

    [SerializeField] private bool rotateAroundPlayer = true;
    [SerializeField] private float rotationsSpeed = 1.0f;
    [SerializeField] private float verticalSpeed = 1.0f;
    [SerializeField] private float verticalLerpAmount = 0.1f;

    private float currentPitch = 0f;
    private float targetPitch = 0f;

    void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        GameInputSystem.Instance.SetForceCursorLockedAndInvisible(true);
    }


    void Update()
    {
        if (rotateAroundPlayer)
        {
            Quaternion camTurnAngle = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * rotationsSpeed, Vector3.up);
            distance = camTurnAngle * distance;
        }

        // Aktualizacja docelowego k¹ta nachylenia (pitch) kamery
        targetPitch -= Input.GetAxis("Mouse Y") * verticalSpeed;
        targetPitch = Mathf.Clamp(targetPitch, -40f, 40f); // Ograniczenie k¹ta nachylenia w zakresie od -30 do 30 stopni

        // Interpolacja k¹ta nachylenia (pitch) kamery
        currentPitch = Mathf.Lerp(currentPitch, targetPitch, verticalLerpAmount);

        // Obliczenie nowej pozycji kamery
        Vector3 targetPosition = playerTransform.position + distance;
        targetPosition.y += Mathf.Sin(currentPitch * Mathf.Deg2Rad) * distance.magnitude; // Dodanie offsetu w pionie

        transform.position = Vector3.Lerp(transform.position, targetPosition, lerpAmount);

        // Obrót kamery w stronê postaci
        transform.LookAt(playerTransform.position + Vector3.up * lookUp); // Patrz na postaæ z lekkim przesuniêciem w górê
    }
}
