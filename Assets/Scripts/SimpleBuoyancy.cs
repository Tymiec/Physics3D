using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class SimpleBuoyancy : MonoBehaviour
{
    public WaterSurface targetSurface = null;

    WaterSearchParameters searchParameters = new WaterSearchParameters();
    WaterSearchResult searchResult = new WaterSearchResult();

    // Update is called once per frame
    void Update()
    {
        if (targetSurface != null)
        {
            searchParameters.startPositionWS = searchResult.candidateLocationWS;
            searchParameters.targetPositionWS = gameObject.transform.position;
            searchParameters.error = 0.02f;
            searchParameters.maxIterations = 16;
            
            if (targetSurface.ProjectPointOnWaterSurface(searchParameters, out searchResult))
            {
                // Debug.Log(searchResult.projectedPositionWS);
                gameObject.transform.position = searchResult.projectedPositionWS;
            }
            // else Debug.LogError("Can't Find Projected Position"); //FIXME Always ignore the first few errors
        }
    }
}