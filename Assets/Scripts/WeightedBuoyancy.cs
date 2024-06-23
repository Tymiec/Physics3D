using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class GravityBuoyancy : MonoBehaviour
{
    public WaterSurface targetSurface = null;
    public float objectMass = 1.0f;
    public float waterDensity = 1000.0f;
    public float gravity = 9.81f;
    public float dampingCoefficient = 0.1f;
    public float dragCoefficient = 0.05f;
    public float maxForce = 1000f;

    private Rigidbody rb;
    private WaterSearchParameters searchParameters;
    private WaterSearchResult searchResult;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        rb.useGravity = false;
        rb.drag = dragCoefficient;
        rb.angularDrag = dragCoefficient;
    }

    void FixedUpdate()
    {
        if (targetSurface != null)
        {
            Vector3 objectCenter = transform.position;
            searchParameters.startPositionWS = searchResult.candidateLocationWS;
            searchParameters.targetPositionWS = objectCenter;
            searchParameters.error = 0.02f;
            searchParameters.maxIterations = 16;

            if (targetSurface.ProjectPointOnWaterSurface(searchParameters, out searchResult))
            {
                float submergedDepth = Mathf.Max(0, searchResult.projectedPositionWS.y - objectCenter.y);
                if (submergedDepth > 0)
                {
                    Bounds bounds = GetComponent<Renderer>().bounds;
                    float volume = bounds.size.x * bounds.size.y * bounds.size.z;
                    float buoyantForceMagnitude = waterDensity * gravity * submergedDepth * volume;
                    Vector3 buoyantForce = new Vector3(0, buoyantForceMagnitude, 0);

                    Vector3 gravityForce = new Vector3(0, -objectMass * gravity, 0);
                    Vector3 dampingForce = -dampingCoefficient * rb.velocity;

                    Vector3 totalForce = gravityForce + buoyantForce + dampingForce;

                    totalForce = Vector3.ClampMagnitude(totalForce, maxForce);

                    rb.AddForce(totalForce);
                }
            }
            else
            {
                Debug.LogError("Can't Find Projected Position");
            }
        }
    }
}
