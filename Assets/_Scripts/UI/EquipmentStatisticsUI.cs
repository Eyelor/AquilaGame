using TMPro;
using UnityEngine;

public class EquipmentStatisticsUI : MonoBehaviour
{
    private PlayerStatistics statistics;
    private PlayerAchievements achievements;
    private GameObject player;

    [SerializeField] private TextMeshProUGUI levelValue;
    [SerializeField] private TextMeshProUGUI experienceValue;
    [SerializeField] private TextMeshProUGUI nextLevelExperienceValue;
    [SerializeField] private TextMeshProUGUI healthValue;
    [SerializeField] private TextMeshProUGUI maxHealthValue;
    [SerializeField] private TextMeshProUGUI attackValue;
    [SerializeField] private TextMeshProUGUI luckValue;

    [SerializeField] private TextMeshProUGUI islandsVisited;
    [SerializeField] private TextMeshProUGUI enemiesDefeated;
    [SerializeField] private TextMeshProUGUI missionsComplited;
    [SerializeField] private TextMeshProUGUI goldEarned;
    [SerializeField] private TextMeshProUGUI goldEarnedOther;

    void OnEnable()
    {
        player = GameObject.FindWithTag("Player");
        statistics = player.GetComponent<PlayerController>().statistics;
        achievements = player.GetComponent<PlayerController>().achievements;
    }

    void Update()
    {
        if (statistics != null)
        {
            levelValue.text = statistics.level.ToString();
            experienceValue.text = statistics.experience.ToString();
            nextLevelExperienceValue.text = "/ " + statistics.experienceToNextLevel.ToString();
            if (statistics.health > 0)
            {
                healthValue.text = statistics.health.ToString();
            }
            else
            {
                healthValue.text = "0";
            }
            maxHealthValue.text = "/ " + statistics.maxHealth.ToString();
            attackValue.text = statistics.attack.ToString();
            luckValue.text = statistics.luck.ToString();
        }
        else
        {
            Debug.LogError("Statistics are null!");
        }

        if (achievements != null)
        {
            islandsVisited.text = achievements.islandsVisited.ToString();
            enemiesDefeated.text = achievements.enemiesDefeated.ToString();
            missionsComplited.text = achievements.missionsCompleted.ToString();
            goldEarned.text = achievements.goldEarned.ToString();
            goldEarnedOther.text = achievements.goldEarned.ToString();
        }
        else
        {
            Debug.LogError("Achievements are null!");
        }
    }
}
