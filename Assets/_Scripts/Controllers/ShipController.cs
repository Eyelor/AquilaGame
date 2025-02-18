using UnityEngine;

public class ShipController : Singleton<ShipController>
{
    // Variables to adjust speed, acceleration, deceleration, and rotation
    [SerializeField] internal float maxForwardSpeed = 12f;
    [SerializeField] private float forwardAcceleration = 4f;
    [SerializeField] private float forwardDeceleration = 6f;

    [SerializeField] internal float maxRotationSpeed = 100f;
    [SerializeField] private float rotationAcceleration = 50f;
    [SerializeField] private float rotationDeceleration = 30f;

    // Private variables to track current speed and rotation speed
    internal float _currentMoveSpeed = 0f;
    internal float _currentRotationSpeed = 0f;
    internal float rotationInput;
    internal bool moveForward;

    // Reference to the Rigidbody component
    private Rigidbody _rb;

    // Start is called before the first frame update
    private void Start()
    {
        // Get the Rigidbody component attached to the boat
        _rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    private void Update()
    {
        // Sprawdzenie, czy kod dzia³a w trybie testów
        if (!TestEnvironment.IsPlayModeTest)
        {
            Vector2 movementVectorNormalized = GameInputSystem.Instance.GetMovementVectorNormalized();

            // Get input from the horizontal axis (A, D for horizontal)
            rotationInput = movementVectorNormalized.x;

            // Determine movement input
            moveForward = movementVectorNormalized.y > 0;
        }
       
        // Accelerate or decelerate movement speed
        if (moveForward)
        {
            _currentMoveSpeed = Mathf.MoveTowards(_currentMoveSpeed, maxForwardSpeed, forwardAcceleration * Time.deltaTime);
        }
        else
        {
            _currentMoveSpeed = Mathf.MoveTowards(_currentMoveSpeed, 0, forwardDeceleration * Time.deltaTime);
        }

        // Accelerate or decelerate rotation speed
        if (rotationInput != 0)
        {
            _currentRotationSpeed = Mathf.MoveTowards(_currentRotationSpeed, rotationInput * maxRotationSpeed, rotationAcceleration * Time.deltaTime);
        }
        else
        {
            _currentRotationSpeed = Mathf.MoveTowards(_currentRotationSpeed, 0, rotationDeceleration * Time.deltaTime);
        }

        // Calculate movement and rotation
        Vector3 movement = transform.forward * _currentMoveSpeed * Time.deltaTime;
        float rotation = _currentRotationSpeed * Time.deltaTime;

        // Apply movement to the Rigidbody
        _rb.MovePosition(_rb.position + movement);

        // Apply rotation to the Rigidbody
        _rb.MoveRotation(_rb.rotation * Quaternion.Euler(0f, rotation, 0f));
    }
}
