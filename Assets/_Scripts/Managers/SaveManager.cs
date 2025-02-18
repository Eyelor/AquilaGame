using System.IO;
using UnityEngine;

public class SaveManager : NativeSingleton<SaveManager>
{
    private JsonDataSerializer<IslandDataList> islandDataListSerializer;
    private JsonDataSerializer<PlayerShipData> playerShipDataSerializer;
    private JsonDataSerializer<CurrentIslandData> currentIslandDataSerializer;
    private JsonDataSerializer<PathData> pathDataSerializer;
    private JsonDataSerializer<ObjectDataList> objectDataListSerializer;
    private JsonDataSerializer<PlayerData> playerDataSerializer;
    private JsonDataSerializer<SavesData> savesDataSerializer;
    private JsonDataSerializer<PlayerStatistics> playerStatisticsSerializer;
    private JsonDataSerializer<PlayerAchievements> playerAchievementsSerializer;

    private int currentSaveId;

    public SaveManager()
    {
        SavesData savesData = LoadSavesData();

        Debug.Log($"(begining of constructor) currentSaveId={currentSaveId}");

        if (savesData == null)
        {
            currentSaveId = 1;
        }
        else
        {
            currentSaveId = savesData.currentSaveId;
        }

        Debug.Log($"(end of constructor) currentSaveId={currentSaveId}");
    }

    #region Island Data List
    public IslandDataList LoadIslandDataList()
    {
        islandDataListSerializer = new JsonDataSerializer<IslandDataList>("Zapis " + currentSaveId, "islands.json");

        return islandDataListSerializer.LoadData();
    }

    public void SaveIslandDataList()
    {
        islandDataListSerializer = new JsonDataSerializer<IslandDataList>("Zapis " + currentSaveId, "islands.json");

        IslandDataList islandDataList = new IslandDataList();
        IslandDataComponent[] islandDataComponents = Object.FindObjectsOfType<IslandDataComponent>();
        System.Array.Sort(islandDataComponents, (x, y) => x.islandData.id.CompareTo(y.islandData.id));

        foreach (var islandDataComponent in islandDataComponents)
        {
            islandDataList.islandsDataList.Add(islandDataComponent.islandData);
        }

        islandDataListSerializer.SaveData(islandDataList);
        UpdateSavesData();
    }
    #endregion

    #region Player Ship Data
    public PlayerShipData LoadPlayerShipData()
    {
        playerShipDataSerializer = new JsonDataSerializer<PlayerShipData>("Zapis " + currentSaveId, "playerShip.json");

        return playerShipDataSerializer.LoadData();
    }

    public void SavePlayerShipData()
    {
        playerShipDataSerializer = new JsonDataSerializer<PlayerShipData>("Zapis " + currentSaveId, "playerShip.json");

        PlayerShipDataComponent[] playerShipDataComponents = Object.FindObjectsOfType<PlayerShipDataComponent>();

        if (playerShipDataComponents.Length != 1)
        {
            Debug.LogError("PlayerShipDataComponent empty or more than one instance.");
            return;
        }

        playerShipDataSerializer.SaveData(playerShipDataComponents[0].playerShipData);
        UpdateSavesData();
    }
    #endregion

    #region Current Island Data
    public CurrentIslandData LoadCurrentIslandData()
    {
        currentIslandDataSerializer = new JsonDataSerializer<CurrentIslandData>("Zapis " + currentSaveId, "currentIsland.json");

        return currentIslandDataSerializer.LoadData();
    }

    public void SaveCurrentIslandData()
    {
        currentIslandDataSerializer = new JsonDataSerializer<CurrentIslandData>("Zapis " + currentSaveId, "currentIsland.json");

        CurrentIslandDataComponent[] currentIslandDataComponents = Object.FindObjectsOfType<CurrentIslandDataComponent>();

        if (currentIslandDataComponents.Length != 1)
        {
            Debug.LogError("CurrentIslandDataComponent empty or more than one instance.");
            return;
        }

        currentIslandDataSerializer.SaveData(currentIslandDataComponents[0].currentIslandData);
        UpdateSavesData();
    }
    #endregion

    #region Path Data
    public PathData LoadPathData(int islandId)
    {
        int currentSaveId = LoadSavesData().currentSaveId;

        pathDataSerializer = new JsonDataSerializer<PathData>("Zapis " + currentSaveId + "/Island" + islandId, "path.json");

        return pathDataSerializer.LoadData();
    }

    public void SavePathData(int islandId)
    {
        int currentSaveId = LoadSavesData().currentSaveId;

        pathDataSerializer = new JsonDataSerializer<PathData>("Zapis " + currentSaveId + "/Island" + islandId, "path.json");

        PathDataComponent[] pathDataComponents = Object.FindObjectsOfType<PathDataComponent>();

        if (pathDataComponents.Length != 1)
        {
            Debug.LogError("PathDataComponent empty or more than one instance.");
            return;
        }

        pathDataSerializer.SaveData(pathDataComponents[0].pathData);
        UpdateSavesData();
    }
    #endregion

    #region Object Data List
    public ObjectDataList LoadObjectDataList(int islandId)
    {
        int currentSaveId = LoadSavesData().currentSaveId;

        objectDataListSerializer = new JsonDataSerializer<ObjectDataList>("Zapis " + currentSaveId + "/Island" + islandId, "objects.json");

        return objectDataListSerializer.LoadData();
    }

    public void SaveObjectDataList(int islandId)
    {
        int currentSaveId = LoadSavesData().currentSaveId;

        objectDataListSerializer = new JsonDataSerializer<ObjectDataList>("Zapis " + currentSaveId + "/Island" + islandId, "objects.json");

        ObjectDataList objectDataList = new ObjectDataList();
        ObjectDataComponent[] objectDataComponents = Object.FindObjectsOfType<ObjectDataComponent>();
        System.Array.Sort(objectDataComponents, (x, y) => x.objectData.id.CompareTo(y.objectData.id));

        foreach (var objectDataComponent in objectDataComponents)
        {
            objectDataList.objectsDataList.Add(objectDataComponent.objectData);
        }

        objectDataListSerializer.SaveData(objectDataList);
        UpdateSavesData();
    }
    #endregion

    #region Player Data
    public PlayerData LoadPlayerData()
    {
        playerDataSerializer = new JsonDataSerializer<PlayerData>("Zapis " + currentSaveId, "player.json");

        return playerDataSerializer.LoadData();
    }

    public void SavePlayerData()
    {
        playerDataSerializer = new JsonDataSerializer<PlayerData>("Zapis " + currentSaveId, "player.json");

        PlayerDataComponent[] playerDataComponents = Object.FindObjectsOfType<PlayerDataComponent>();

        if (playerDataComponents.Length != 1)
        {
            Debug.LogError("PlayerDataComponent empty or more than one instance.");
            return;
        }

        playerDataSerializer.SaveData(playerDataComponents[0].playerData);
        UpdateSavesData();
    }
    #endregion

    #region Player Statistics
    public PlayerStatistics LoadPlayerStatistics()
    {
        playerStatisticsSerializer = new JsonDataSerializer<PlayerStatistics>("Zapis " + currentSaveId, "playerStatistics.json");

        return playerStatisticsSerializer.LoadData();
    }

    public void SavePlayerStatistics()
    {
        playerStatisticsSerializer = new JsonDataSerializer<PlayerStatistics>("Zapis " + currentSaveId, "playerStatistics.json");

        PlayerController[] playerControllers = Object.FindObjectsOfType<PlayerController>();

        if (playerControllers.Length != 1)
        {
            Debug.LogError("PlayerStatisticsComponent empty or more than one instance.");
            return;
        }

        playerStatisticsSerializer.SaveData(playerControllers[0].statistics);
        UpdateSavesData();
    }

    public string GetPlayerStatisticsFilePath()
    {
        return Path.Combine(Application.persistentDataPath, "Zapis " + currentSaveId, "playerStatistics.json");
    }
    #endregion

    #region Saves Data
    public SavesData LoadSavesData()
    {
        savesDataSerializer = new JsonDataSerializer<SavesData>("", "saves.json");

        return savesDataSerializer.LoadData();
    }

    public void SaveSavesData(SavesData savesData, string saveLocationString, bool isSavesNull)
    {
        savesDataSerializer = new JsonDataSerializer<SavesData>("", "saves.json");

        if (isSavesNull)
        {
            savesData = new SavesData();
        }

        savesData.currentSaveId = savesData.GetNextSaveId();
        Debug.Log($"(SaveSavesData befor change) currentSaveId={currentSaveId}");
        currentSaveId = savesData.currentSaveId;
        Debug.Log($"(SaveSavesData after change) currentSaveId={currentSaveId}");
        SaveData saveData = new SaveData()
        {
            saveId = savesData.currentSaveId,
            saveName = "Zapis " + savesData.currentSaveId,
            saveLocation = saveLocationString,
            saveDateTime = System.DateTime.Now.ToString()
        };
        savesData.savesDataList.Add(saveData);

        savesDataSerializer.SaveData(savesData);
    }

    public void SaveSavesData(SavesData savesData)
    {
        savesDataSerializer = new JsonDataSerializer<SavesData>("", "saves.json");

        savesDataSerializer.SaveData(savesData);
    }

    public void UpdateSavesData()
    {
        SavesData savesData = LoadSavesData();

        savesData.UpdateSaveDataDateTime(currentSaveId);

        savesDataSerializer = new JsonDataSerializer<SavesData>("", "saves.json");
        savesDataSerializer.SaveData(savesData);
    }

    public void UpdateSavesData(string location)
    {
        SavesData savesData = LoadSavesData();

        savesData.UpdateSaveDataLocation(currentSaveId, location);
        savesData.UpdateSaveDataDateTime(currentSaveId);

        savesDataSerializer = new JsonDataSerializer<SavesData>("", "saves.json");
        savesDataSerializer.SaveData(savesData);
    }

    public void UpdateSaveManagerCurrentSaveId(int saveId)
    {
        currentSaveId = saveId;
    }

    public int GetSaveManagerCurrentSaveId()
    {
        return currentSaveId;
    }
	#endregion

	#region Settings Data
    public SettingsData LoadSettingsData()
	{
		JsonDataSerializer<SettingsData> settingsDataSerializer = new JsonDataSerializer<SettingsData>("", "settings.json");

		return settingsDataSerializer.LoadData();
	}

    public void SaveSettingsData(SettingsData settingsData)
    {
        JsonDataSerializer<SettingsData> settingsDataSerializer = new JsonDataSerializer<SettingsData>("", "settings.json");

        settingsDataSerializer.SaveData(settingsData);
    }
    #endregion

    #region Player Achievements
    public PlayerAchievements LoadPlayerAchievements()
    {
        playerAchievementsSerializer = new JsonDataSerializer<PlayerAchievements>("Zapis " + currentSaveId, "playerAchievements.json");

        return playerAchievementsSerializer.LoadData();
    }

    public void SavePlayerAchievements()
    {
        playerAchievementsSerializer = new JsonDataSerializer<PlayerAchievements>("Zapis " + currentSaveId, "playerAchievements.json");

        PlayerController[] playerControllers = Object.FindObjectsOfType<PlayerController>();

        if (playerControllers.Length != 1)
        {
            Debug.LogError("PlayerDataComponent empty or more than one instance.");
            return;
        }

        playerAchievementsSerializer.SaveData(playerControllers[0].achievements);
        UpdateSavesData();
    }
    #endregion
}
