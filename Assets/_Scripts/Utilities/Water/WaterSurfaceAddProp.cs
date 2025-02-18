using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class WaterSurfaceAddProp : MonoBehaviour
{
    public WaterSurface water;
    
    public float simulationFoamDrag = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        water.simulationFoamDrag = simulationFoamDrag;
    }

}
