using TMPro;
using UnityEngine;

public class StatisticsUI : MonoBehaviour
{
    private PlayerStatistics statistics;
    private GameObject player;

    [SerializeField] private TextMeshProUGUI levelValue;
    [SerializeField] private TextMeshProUGUI experienceValue;
    [SerializeField] private TextMeshProUGUI healthValue;

    void OnEnable()
    {
        player = GameObject.FindWithTag("Player");
        statistics = player.GetComponent<PlayerController>().statistics;
    }

    void Update()
    {
        if (statistics != null)
        {
            levelValue.text = statistics.level.ToString();
            experienceValue.text = statistics.experience.ToString();
            if (statistics.health > 0)
            {
                healthValue.text = statistics.health.ToString();
            }
            else
            {
                healthValue.text = "0";
            }
        }
        else
        {
            Debug.LogError("Statistics are null!");
        }
    }
}
