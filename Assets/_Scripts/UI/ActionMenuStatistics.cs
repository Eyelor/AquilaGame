using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActionMenuStatistics : MonoBehaviour
{
    private PlayerStatistics statistics;
    private GameObject player;
    private Animator animator;

    [SerializeField] private TextMeshProUGUI levelValue;
    [SerializeField] private TextMeshProUGUI experienceValue;
    [SerializeField] private TextMeshProUGUI healthValue;

    [SerializeField] private Image experienceBar; 
    [SerializeField] private Image healthBar;

    [SerializeField] private GameObject FightIcon;
    [SerializeField] private GameObject SprintIcon;
    [SerializeField] private GameObject RegenIcon;
    
    void OnEnable()
    {
        player = GameObject.FindWithTag("Player");
        statistics = player.GetComponent<PlayerController>().statistics;
        animator = player.GetComponent<Animator>();
    }

    void Update()
    {
        if (statistics != null)
        {
            levelValue.text = statistics.level.ToString();

            // Obliczenie procentowego doœwiadczenia
            float experiencePercent = (float)statistics.experience / statistics.experienceToNextLevel;
            experienceValue.text = Mathf.RoundToInt(experiencePercent * 100).ToString() + "%";
            experienceBar.fillAmount = experiencePercent;

            // Obliczenie procentowego zdrowia
            float healthPercent;
            if (statistics.health > 0)
            {
                healthPercent = (float)statistics.health / statistics.maxHealth;
                healthValue.text = Mathf.RoundToInt(healthPercent * 100).ToString() + "%";
                healthBar.fillAmount = healthPercent;
            }
            else
            {
                healthValue.text = "0%";
                healthBar.fillAmount = 0;
            }

            if (statistics.isFighting)
            {
                if (!FightIcon.activeSelf)
                {
                    FightIcon.SetActive(true);
                }
                if (RegenIcon.activeSelf)
                {
                    RegenIcon.SetActive(false);
                }
            } 
            else
            {
                if (FightIcon.activeSelf)
                {
                    FightIcon.SetActive(false);
                }

                if (statistics.health < statistics.maxHealth && statistics.isPlayerAlive)
                {
                    if (!RegenIcon.activeSelf)
                    {
                        RegenIcon.SetActive(true);
                    }                 
                }
                else
                {
                    if (RegenIcon.activeSelf)
                    {
                        RegenIcon.SetActive(false);
                    }
                }
            }

            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Run") || 
                animator.GetCurrentAnimatorStateInfo(0).IsName("Jumping Run"))
            {
                if (!SprintIcon.activeSelf)
                {
                    SprintIcon.SetActive(true);
                }
            }
            else
            {
                if (SprintIcon.activeSelf)
                {
                    SprintIcon.SetActive(false);
                }
            }
        }

        else
        {
            Debug.LogError("Statistics are null!");
        }
    }
}
