using System.Collections.Generic;
using UnityEngine;

public class PathDataComponent : MonoBehaviour
{
    public PathData pathData;

    void Awake()
    {
        if (pathData == null)
        {
            pathData = new PathData
            {
                islandId = 0,
                points = new List<PointData>(),
                branches = new List<BranchData>()
            };
        }
    }

    // Metoda do dodawania punktów do œcie¿ki
    public void AddPoint(Vector3 position)
    {
        pathData.points.Add(new PointData { posX = position.x, posY = position.y, posZ = position.z });
    }

    // Metoda do dodawania odnogi do œcie¿ki
    public void AddBranch(BranchData branch)
    {
        pathData.branches.Add(branch);
    }
}
