using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class FitToWaterSurface : MonoBehaviour
{
    // Rigidbody component of the floating object
    public Rigidbody rb;
    // Depth at which object starts to experience buoyancy
    public float depthBefSub;
    // Amount of buoyancy force applied
    public float displacementAmt;
    // Number of points applying buoyant force
    public int floaters;

    // Drag coefficient in water
    public float waterDrag;
    // Angular drag coefficient in water
    public float waterAngularDrag;
    // Reference to the water surface management component
    public WaterSurface water;

    // Holds parameters for searching the water surface
    WaterSearchParameters Search;
    // Stores result of water surface search
    WaterSearchResult SearchResult;

    private void FixedUpdate()
    {
        // Apply distributed gravitional force
        rb.AddForceAtPosition(Physics.gravity / floaters, transform.position, ForceMode.Acceleration);

        // Set up search parameters for projecting on water surface
        Search.startPosition = transform.position;

        // Project point onto water surface and get result
        water.FindWaterSurfaceHeight(Search, out SearchResult);

        // If object is below the water surface
        if (transform.position.y < SearchResult.height)
        {
            // Calculate displacement multiplier based on submersion depth
            float displacementMulti = Mathf.Clamp01((SearchResult.height - transform.position.y) / depthBefSub) * displacementAmt;
            // Apply buoyant force upwards
            rb.AddForceAtPosition(new Vector3(0f, Mathf.Abs(Physics.gravity.y) * displacementMulti, 0f), transform.position, ForceMode.Acceleration);
            // Apply water drag force against velocity
            rb.AddForce(displacementMulti * -rb.velocity * waterDrag * Time.fixedDeltaTime, ForceMode.VelocityChange);
            // Apply water angular drag torque against angular velocity
            rb.AddTorque(displacementMulti * -rb.angularVelocity * waterAngularDrag * Time.fixedDeltaTime, ForceMode.VelocityChange);
        }
    }
}
