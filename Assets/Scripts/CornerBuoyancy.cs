using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class CornerBuoyancy : MonoBehaviour
{
    public WaterSurface targetSurface = null;

    WaterSearchParameters searchParameters = new WaterSearchParameters();
    WaterSearchResult searchResult = new WaterSearchResult();

    // Update is called once per frame
    void Update()
    {
        if (targetSurface != null)
        {
            Bounds bounds = GetComponent<Renderer>().bounds;
            Vector3[] corners = new Vector3[8];

            corners[0] = bounds.min;
            corners[1] = new Vector3(bounds.min.x, bounds.min.y, bounds.max.z);
            corners[2] = new Vector3(bounds.min.x, bounds.max.y, bounds.min.z);
            corners[3] = new Vector3(bounds.min.x, bounds.max.y, bounds.max.z);
            corners[4] = new Vector3(bounds.max.x, bounds.min.y, bounds.min.z);
            corners[5] = new Vector3(bounds.max.x, bounds.min.y, bounds.max.z);
            corners[6] = new Vector3(bounds.max.x, bounds.max.y, bounds.min.z);
            corners[7] = bounds.max;

            Vector3 newPosition = Vector3.zero;
            int successfulProjections = 0;

            foreach (var corner in corners)
            {
                searchParameters.startPositionWS = searchResult.candidateLocationWS;
                searchParameters.targetPositionWS = corner;
                searchParameters.error = 0.02f;
                searchParameters.maxIterations = 16;

                if (targetSurface.ProjectPointOnWaterSurface(searchParameters, out searchResult))
                {
                    newPosition += new Vector3(searchResult.projectedPositionWS.x, searchResult.projectedPositionWS.y, searchResult.projectedPositionWS.z);
                    successfulProjections++;
                }
            }

            if (successfulProjections > 0)
            {
                newPosition /= successfulProjections;
                gameObject.transform.position = newPosition;
            }
            else
            {
                Debug.LogError("Can't Find Projected Position for any corner");
            }
        }
    }
}
