using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MissionsManager : Singleton<MissionsManager>
{
    [Header("Explore Mission")]
    [SerializeField] private TextMeshProUGUI exploreMissionName;

    [Header("Objective 1")]
    [SerializeField] private TextMeshProUGUI exploreMissionObjective1ProgressValue;
    [SerializeField] private TextMeshProUGUI exploreMissionObjective1Slash;
    [SerializeField] private TextMeshProUGUI exploreMissionObjective1ObjectiveValue;
    [SerializeField] private GameObject exploreMissionObjective1InProgress;
    [SerializeField] private GameObject exploreMissionObjective1Completed;

    [Header("Fight Mission")]
    [SerializeField] private TextMeshProUGUI fightMissionName;

    [Header("Objective 1")]
    [SerializeField] private TextMeshProUGUI fightMissionObjective1Name;
    [SerializeField] private TextMeshProUGUI fightMissionObjective1ProgressValue;
    [SerializeField] private TextMeshProUGUI fightMissionObjective1Slash;
    [SerializeField] private TextMeshProUGUI fightMissionObjective1ObjectiveValue;
    [SerializeField] private GameObject fightMissionObjective1InProgress;
    [SerializeField] private GameObject fightMissionObjective1Completed;

    [Header("Objective 2")]
    [SerializeField] private TextMeshProUGUI fightMissionObjective2Name;
    [SerializeField] private TextMeshProUGUI fightMissionObjective2ProgressValue;
    [SerializeField] private TextMeshProUGUI fightMissionObjective2Slash;
    [SerializeField] private TextMeshProUGUI fightMissionObjective2ObjectiveValue;
    [SerializeField] private GameObject fightMissionObjective2InProgress;
    [SerializeField] private GameObject fightMissionObjective2Completed;

    public Color greenColor;

    private bool isFirstFrame = true;
    private bool isInitializeMissions = true;

    private enum Difficulty
    {
        £atwy,
        Œredni,
        Trudny,
    }

    private Difficulty exploreDifficulty;
    private Difficulty fightDifficulty;

    private int numberOfChestsOnMap;
    private int numberOfChestsToOpen;
    public int numberOfChestsOpened = 0;
    private bool isExploreMissionCompletedFirstTimeInUpdate = true;

    private EnemyStatisticsUI[] enemyStatisticsUIs;
    private int numberOfWeakEnemies;
    private int numberOfStrongEnemies;
    private int numberOfWeakEnemiesToKill;
    private int numberOfStrongEnemiesToKill;
    public int numberOfWeakEnemiesKilled = 0;
    public int numberOfStrongEnemiesKilled = 0;
    private bool isFightMissionCompletedFirstTimeInUpdate = true;

    private void Update()
    {
        if (!isFirstFrame && isInitializeMissions)
        {
            InitializeMissions();
            isInitializeMissions = false;
        }

        if (isFirstFrame)
        {
            isFirstFrame = false;
            return;
        }

        UpdateMissionsProgressTexts();
    }

    private void InitializeMissions()
    {
        System.Array values = System.Enum.GetValues(typeof(Difficulty));
        System.Random random = new System.Random();
        exploreDifficulty = (Difficulty)values.GetValue(random.Next(values.Length));
        fightDifficulty = (Difficulty)values.GetValue(random.Next(values.Length));

        numberOfChestsOnMap = FindNumberOfChestsGenerated();
        int oneOfThreeAmountOfChests = (int)(numberOfChestsOnMap / 3);
        int oneOfThreeOfOneOfThreeAmountOfChests = (int)(oneOfThreeAmountOfChests / 3);

        switch(exploreDifficulty)
        {
            case Difficulty.£atwy:
                numberOfChestsToOpen = Random.Range(oneOfThreeAmountOfChests - oneOfThreeOfOneOfThreeAmountOfChests, oneOfThreeAmountOfChests);
                if (numberOfChestsOnMap > 0 && numberOfChestsToOpen == 0) numberOfChestsToOpen = 1;
                break;
            case Difficulty.Œredni:
                numberOfChestsToOpen = Random.Range((oneOfThreeAmountOfChests * 2) - oneOfThreeOfOneOfThreeAmountOfChests, oneOfThreeAmountOfChests * 2);
                if (numberOfChestsOnMap > 0 && numberOfChestsToOpen == 0) numberOfChestsToOpen = 1;
                break;
            case Difficulty.Trudny:
                numberOfChestsToOpen = Random.Range(numberOfChestsOnMap - oneOfThreeOfOneOfThreeAmountOfChests, numberOfChestsOnMap);
                if (numberOfChestsOnMap > 0 && numberOfChestsToOpen == 0) numberOfChestsToOpen = 1;
                break;
            default:
                Debug.LogError("[MissionsManager] Explore difficulty error");
                break;
        }

        enemyStatisticsUIs = FindEnemyStatisticsUI();

        Debug.Log($"Number of enemies on map: {enemyStatisticsUIs.Length}");
        foreach (EnemyStatisticsUI enemyStatisticsUI in enemyStatisticsUIs)
        {
            Debug.Log($"Enemy level: {enemyStatisticsUI.GetMobLevel()}");
        }
        
        switch(fightDifficulty)
        {
            case Difficulty.£atwy:
                numberOfWeakEnemies = GetNumberOfEnemiesWithLevel(1);
                numberOfStrongEnemies = GetNumberOfEnemiesWithLevel(2);
                numberOfWeakEnemiesToKill = Random.Range(1, numberOfWeakEnemies);
                numberOfStrongEnemiesToKill = Random.Range(1, numberOfStrongEnemies);
                if (numberOfWeakEnemies > 0 && numberOfWeakEnemiesToKill == 0) numberOfWeakEnemiesToKill = 1;
                if (numberOfStrongEnemies > 0 && numberOfStrongEnemiesToKill == 0) numberOfStrongEnemiesToKill = 1;
                break;
            case Difficulty.Œredni:
                numberOfWeakEnemies = GetNumberOfEnemiesWithLevel(3);
                numberOfStrongEnemies = GetNumberOfEnemiesWithLevel(4);
                numberOfWeakEnemiesToKill = Random.Range(numberOfWeakEnemies - (CalculateOneOfThreeAmountOfWeakEnemies(numberOfWeakEnemies) * 2), numberOfWeakEnemies);
                numberOfStrongEnemiesToKill = Random.Range(numberOfStrongEnemies - (CalculateOneOfThreeAmountOfStrongEnemies(numberOfStrongEnemies) * 2), numberOfStrongEnemies);
                if (numberOfWeakEnemies > 0 && numberOfWeakEnemiesToKill == 0) numberOfWeakEnemiesToKill = 1;
                if (numberOfStrongEnemies > 0 && numberOfStrongEnemiesToKill == 0) numberOfStrongEnemiesToKill = 1;
                break;
            case Difficulty.Trudny:
                numberOfWeakEnemies = GetNumberOfEnemiesWithLevel(5);
                numberOfStrongEnemies = GetNumberOfEnemiesWithLevel(6);
                numberOfWeakEnemiesToKill = Random.Range(numberOfWeakEnemies - CalculateOneOfThreeAmountOfWeakEnemies(numberOfWeakEnemies), numberOfWeakEnemies);
                numberOfStrongEnemiesToKill = Random.Range(numberOfStrongEnemies - CalculateOneOfThreeAmountOfStrongEnemies(numberOfStrongEnemies), numberOfStrongEnemies);
                if (numberOfWeakEnemies > 0 && numberOfWeakEnemiesToKill == 0) numberOfWeakEnemiesToKill = 1;
                if (numberOfStrongEnemies > 0 && numberOfStrongEnemiesToKill == 0) numberOfStrongEnemiesToKill = 1;
                break;
            default:
                Debug.LogError("[MissionsManager] Fight difficulty error");
                break;
        }

        UpdateMissionsTexts();
    }

    private void UpdateMissionsTexts()
    {
        exploreMissionName.text = $"Odkrywanie ({exploreDifficulty})";

        exploreMissionObjective1ProgressValue.text = numberOfChestsOpened.ToString();
        exploreMissionObjective1ObjectiveValue.text = numberOfChestsToOpen.ToString();

        fightMissionName.text = $"Walka ({fightDifficulty})";

        int levelWeak;
        int levelStrong;
        switch(fightDifficulty)
        {
            case Difficulty.£atwy:
                levelWeak = 1;
                levelStrong = 2;
                break;
            case Difficulty.Œredni:
                levelWeak = 3;
                levelStrong = 4;
                break;
            case Difficulty.Trudny:
                levelWeak = 5;
                levelStrong = 6;
                break;
            default:
                Debug.LogError("[MissionsManager] Fight difficulty error");
                return;
        }

        fightMissionObjective1Name.text = $"Pokonaj piratów poziomu {levelWeak}";
        fightMissionObjective1ProgressValue.text = numberOfWeakEnemiesKilled.ToString();
        fightMissionObjective1ObjectiveValue.text = numberOfWeakEnemiesToKill.ToString();

        fightMissionObjective2Name.text = $"Pokonaj piratów poziomu {levelStrong}";
        fightMissionObjective2ProgressValue.text = numberOfStrongEnemiesKilled.ToString();
        fightMissionObjective2ObjectiveValue.text = numberOfStrongEnemiesToKill.ToString();
    }

    private int FindNumberOfChestsGenerated()
    {
        Chest[] chests = Object.FindObjectsOfType<Chest>();

        return chests.Length;
    }

    private EnemyStatisticsUI[] FindEnemyStatisticsUI()
    {
        EnemyStatisticsUI[] enemyStatisticsUIs = Object.FindObjectsOfType<EnemyStatisticsUI>();

        return enemyStatisticsUIs;
    }

    private int GetNumberOfEnemiesWithLevel(int level)
    {
        int numberOfEnemies = 0;

        foreach (EnemyStatisticsUI enemyStatisticsUI in enemyStatisticsUIs)
        {
            if (enemyStatisticsUI.GetMobLevel() == level)
            {
                numberOfEnemies++;
            }
        }

        return numberOfEnemies;
    }

    private int CalculateOneOfThreeAmountOfWeakEnemies(int numberOfWeakEnemies)
    {
        return (int)(numberOfWeakEnemies / 3);
    }

    private int CalculateOneOfThreeAmountOfStrongEnemies(int numberOfStrongEnemies)
    {
        return (int)(numberOfStrongEnemies / 3);
    }

    private void UpdateMissionsProgressTexts()
    {
        if (numberOfChestsOpened == numberOfChestsToOpen)
        {
            exploreMissionObjective1ProgressValue.color = greenColor;
            exploreMissionObjective1Slash.color = greenColor;
            exploreMissionObjective1ObjectiveValue.color = greenColor;
            exploreMissionObjective1InProgress.SetActive(false);
            exploreMissionObjective1Completed.SetActive(true);
            exploreMissionObjective1ProgressValue.text = numberOfChestsOpened.ToString();

            if (isExploreMissionCompletedFirstTimeInUpdate)
            {
                // Tu dostajemy  nagrodê za ukoñczenie misji odkrywania
                switch(exploreDifficulty)
                {
                    case Difficulty.£atwy:
                        PlayerController.Instance.statistics.experience += 200;
                        PlayerController.Instance.achievements.missionsCompleted += 1;
                        PlayerController.Instance.gameObject.GetComponent<NumberParticleEmitter>().EmitNumberParticle("+200", "blue");
                        break;
                    case Difficulty.Œredni:
                        PlayerController.Instance.statistics.experience += 500;
                        PlayerController.Instance.achievements.missionsCompleted += 1;
                        PlayerController.Instance.gameObject.GetComponent<NumberParticleEmitter>().EmitNumberParticle("+500", "blue");
                        break;
                    case Difficulty.Trudny:
                        PlayerController.Instance.statistics.experience += 1000;
                        PlayerController.Instance.achievements.missionsCompleted += 1;
                        PlayerController.Instance.gameObject.GetComponent<NumberParticleEmitter>().EmitNumberParticle("+1000", "blue");
                        break;
                    default:
                        Debug.LogError("[MissionsManager] Explore difficulty error");
                        break;
                }
                Debug.Log("Explore mission completed");
                isExploreMissionCompletedFirstTimeInUpdate = false;
            }
        }
        else
        {
            exploreMissionObjective1ProgressValue.text = numberOfChestsOpened.ToString();
        }

        if (numberOfWeakEnemiesKilled == numberOfWeakEnemiesToKill)
        {
            fightMissionObjective1ProgressValue.color = greenColor;
            fightMissionObjective1Slash.color = greenColor;
            fightMissionObjective1ObjectiveValue.color = greenColor;
            fightMissionObjective1InProgress.SetActive(false);
            fightMissionObjective1Completed.SetActive(true);
            fightMissionObjective1ProgressValue.text = numberOfWeakEnemiesKilled.ToString();

            if (isFightMissionCompletedFirstTimeInUpdate && numberOfStrongEnemiesKilled == numberOfStrongEnemiesToKill)
            {
                // Tu dostajemy nagrodê za ukoñczenie misji walki
                switch(fightDifficulty)
                {
                    case Difficulty.£atwy:
                        PlayerController.Instance.statistics.experience += 200;
                        PlayerController.Instance.achievements.missionsCompleted += 1;
                        PlayerController.Instance.gameObject.GetComponent<NumberParticleEmitter>().EmitNumberParticle("+200", "blue");
                        break;
                    case Difficulty.Œredni:
                        PlayerController.Instance.statistics.experience += 500;
                        PlayerController.Instance.achievements.missionsCompleted += 1;
                        PlayerController.Instance.gameObject.GetComponent<NumberParticleEmitter>().EmitNumberParticle("+500", "blue");
                        break;
                    case Difficulty.Trudny:
                        PlayerController.Instance.statistics.experience += 1000;
                        PlayerController.Instance.achievements.missionsCompleted += 1;
                        PlayerController.Instance.gameObject.GetComponent<NumberParticleEmitter>().EmitNumberParticle("+1000", "blue");
                        break;
                    default:
                        Debug.LogError("[MissionsManager] Fight difficulty error");
                        break;
                }
                Debug.Log("Fight mission completed");
                isFightMissionCompletedFirstTimeInUpdate = false;
            }
        }
        else
        {
            fightMissionObjective1ProgressValue.text = numberOfWeakEnemiesKilled.ToString();
        }

        if (numberOfStrongEnemiesKilled == numberOfStrongEnemiesToKill)
        {
            fightMissionObjective2ProgressValue.color = greenColor;
            fightMissionObjective2Slash.color = greenColor;
            fightMissionObjective2ObjectiveValue.color = greenColor;
            fightMissionObjective2InProgress.SetActive(false);
            fightMissionObjective2Completed.SetActive(true);
            fightMissionObjective2ProgressValue.text = numberOfStrongEnemiesKilled.ToString();

            if (isFightMissionCompletedFirstTimeInUpdate && numberOfWeakEnemiesKilled == numberOfWeakEnemiesToKill)
            {
                // Tu dostajemy nagrodê za ukoñczenie misji walki
                switch (fightDifficulty)
                {
                    case Difficulty.£atwy:
                        PlayerController.Instance.statistics.experience += 200;
                        PlayerController.Instance.achievements.missionsCompleted += 1;
                        PlayerController.Instance.gameObject.GetComponent<NumberParticleEmitter>().EmitNumberParticle("+200", "blue");
                        break;
                    case Difficulty.Œredni:
                        PlayerController.Instance.statistics.experience += 500;
                        PlayerController.Instance.achievements.missionsCompleted += 1;
                        PlayerController.Instance.gameObject.GetComponent<NumberParticleEmitter>().EmitNumberParticle("+500", "blue");
                        break;
                    case Difficulty.Trudny:
                        PlayerController.Instance.statistics.experience += 1000;
                        PlayerController.Instance.achievements.missionsCompleted += 1;
                        PlayerController.Instance.gameObject.GetComponent<NumberParticleEmitter>().EmitNumberParticle("+1000", "blue");
                        break;
                    default:
                        Debug.LogError("[MissionsManager] Fight difficulty error");
                        break;
                }
                Debug.Log("Fight mission completed");
                isFightMissionCompletedFirstTimeInUpdate = false;
            }
        }
        else
        {
            fightMissionObjective2ProgressValue.text = numberOfStrongEnemiesKilled.ToString();
        }
    }

    public void ChestOpenedFirstTime()
    {
        if (numberOfChestsOpened < numberOfChestsToOpen) numberOfChestsOpened++;
    }

    private void WeakEnemyKilled(int level)
    {
        switch(fightDifficulty)
        {
            case Difficulty.£atwy:
                if (level == 1 && numberOfWeakEnemiesKilled < numberOfWeakEnemiesToKill) numberOfWeakEnemiesKilled++;
                break;
            case Difficulty.Œredni:
                if (level == 3 && numberOfWeakEnemiesKilled < numberOfWeakEnemiesToKill) numberOfWeakEnemiesKilled++;
                break;
            case Difficulty.Trudny:
                if (level == 5 && numberOfWeakEnemiesKilled < numberOfWeakEnemiesToKill) numberOfWeakEnemiesKilled++;
                break;
            default:
                Debug.LogError("[MissionsManager] Fight difficulty error");
                break;
        }
    }

    private void StrongEnemyKilled(int level)
    {
        switch (fightDifficulty)
        {
            case Difficulty.£atwy:
                if (level == 2 && numberOfStrongEnemiesKilled < numberOfStrongEnemiesToKill) numberOfStrongEnemiesKilled++;
                break;
            case Difficulty.Œredni:
                if (level == 4 && numberOfStrongEnemiesKilled < numberOfStrongEnemiesToKill) numberOfStrongEnemiesKilled++;
                break;
            case Difficulty.Trudny:
                if (level == 6 && numberOfStrongEnemiesKilled < numberOfStrongEnemiesToKill) numberOfStrongEnemiesKilled++;
                break;
            default:
                Debug.LogError("[MissionsManager] Fight difficulty error");
                break;
        }
    }

    public void EnemyKilled(int level)
    {
        WeakEnemyKilled(level);
        StrongEnemyKilled(level);
    }
}
