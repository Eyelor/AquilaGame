using System.Collections.Generic;

[System.Serializable]
public class IslandDataList
{
    public List<IslandData> islandsDataList;

    public IslandDataList()
    {
        islandsDataList = new List<IslandData>();
    }

    public IslandData GetIslandData(int islandId)
    {
        return islandsDataList.Find(x => x.id == islandId);
    }
}
