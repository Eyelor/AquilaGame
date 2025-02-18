using System.Collections.Generic;

[System.Serializable]
public class SavesData
{
    public int currentSaveId;
    public List<SaveData> savesDataList;

    public SavesData()
    {
        savesDataList = new List<SaveData>();
    }

    public void DeleteSaveData(int saveId)
    {
        SaveData saveData = savesDataList.Find(x => x.saveId == saveId);
        if (saveData != null)
        {
            if (saveId == currentSaveId)
            {
                currentSaveId = 0;
            }
            savesDataList.Remove(saveData);
        }
    }

    public SaveData GetSaveData(int saveId)
    {
        return savesDataList.Find(x => x.saveId == saveId);
    }

    public void UpdateSaveDataLocation(int saveId, string saveLocation)
    {
        SaveData saveData = savesDataList.Find(x => x.saveId == saveId);
        if (saveData != null)
        {
            saveData.saveLocation = saveLocation;
        }
    }

    public void UpdateSaveDataDateTime(int saveId)
    {
        SaveData saveData = savesDataList.Find(x => x.saveId == saveId);
        if (saveData != null)
        {
            saveData.saveDateTime = System.DateTime.Now.ToString();
        }
    }

    public bool IsSaveDataExists(int saveId)
    {
        return savesDataList.Exists(x => x.saveId == saveId);
    }

    public int GetNextSaveId()
    {
        int nextSaveId = 1;

        if (savesDataList != null)
        {
            foreach (var saveData in savesDataList)
            {
                if (saveData.saveId >= nextSaveId)
                {
                    nextSaveId = saveData.saveId + 1;
                }
            }
        }

        return nextSaveId;
    }
}
