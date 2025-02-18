using System.IO;
using UnityEngine;

public class StatisticsManager : NativeSingleton<StatisticsManager>
{

    private InitialPlayerStatisticsSO initialPlayerStatistics;
    private string filePath;

    public StatisticsManager()
    {
        initialPlayerStatistics = ScriptableObject.CreateInstance<InitialPlayerStatisticsSO>();
    }

    public PlayerStatistics LoadStatistics()
    {
        filePath = SaveManager.Instance.GetPlayerStatisticsFilePath();
        // Sprawdzenie, czy plik istnieje
        if (File.Exists(filePath))
        {
            return LoadStatisticsFromFile();
        }
        else
        {
            return LoadStatisticsFromScriptableObject();
        }
    }

    // Odczytuje statystyki z pliku JSON
    private PlayerStatistics LoadStatisticsFromFile()
    {
        try
        {
            return SaveManager.Instance.LoadPlayerStatistics();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"B³¹d podczas odczytywania statystyk z pliku JSON: {e.Message}");
            return LoadStatisticsFromScriptableObject();
        }
    }

    // Odczytuje statystyki ze ScriptableObject
    private PlayerStatistics LoadStatisticsFromScriptableObject()
    {
        return new PlayerStatistics
        {
            level = initialPlayerStatistics.Level,
            experience = initialPlayerStatistics.Experience,
            experienceToNextLevel = initialPlayerStatistics.ExperienceToNextLevel,
            isFighting = initialPlayerStatistics.IsFighting,
            isPlayerAlive = initialPlayerStatistics.IsPlayerAlive,
            health = initialPlayerStatistics.Health,
            maxHealth = initialPlayerStatistics.MaxHealth,
            healthRegen = initialPlayerStatistics.HealthRegen,
            attack = initialPlayerStatistics.Attack,
            luck = initialPlayerStatistics.Luck,
        };
    }
}
