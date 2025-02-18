using System.Collections.Generic;

[System.Serializable]
public class PointData
{
    public float posX;
    public float posY;
    public float posZ;
}

[System.Serializable]
public class BranchData
{
    public List<PointData> points;
}


[System.Serializable]
public class PathData
{
    public int islandId;
    public List<PointData> points;
    public List<BranchData> branches;
}
