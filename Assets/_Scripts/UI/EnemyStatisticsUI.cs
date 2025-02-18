using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyStatisticsUI : MonoBehaviour
{
    private PlayerStatistics playerStatistics;
    private Quaternion initialRotation;

    [SerializeField] private TextMeshProUGUI levelValue;
    [SerializeField] private TextMeshProUGUI healthValue;
    [SerializeField] private Image healthBar;

    [SerializeField] private GameObject FightIcon;
    [SerializeField] private GameObject BossIcon;

    private MobStatistics mobStatistics;
    private int mobLevel;
    private bool isLevelAdjust = true;

    void Start()
    {
        // Zapamiêtaj pocz¹tkow¹ globaln¹ rotacjê
        initialRotation = transform.rotation;

        mobStatistics = GetComponentInParent<MobStatisticComponent>().mobStatistics;
        if (mobStatistics == null)
        {
            Debug.LogError("Nie znaleziono komponentu MobStatisticComponent w rodzicu!");
        }
    }

    void LateUpdate()
    {
        // Ustaw globaln¹ rotacjê na pocz¹tkow¹
        transform.rotation = initialRotation;
    }

    private void Update()
    {
        if (isLevelAdjust)
        {
            playerStatistics = GameObject.FindWithTag("Player").GetComponent<PlayerController>().statistics;
            mobLevel = CalculateLevel();
            levelValue.text = "Pirat (" + mobLevel + ")";
            isLevelAdjust = false;
        }

        float healthPercent;
        if (mobStatistics.health > 0)
        {
            healthPercent = (float)mobStatistics.health / mobStatistics.maxHealth;
            healthValue.text = Mathf.RoundToInt(healthPercent * 100).ToString() + "%";
            healthBar.fillAmount = healthPercent;
        }
        else
        {
            StartCoroutine(DestroyInfoAfterSecond());
            healthValue.text = "0%";
            healthBar.fillAmount = 0;
        }

        if (GetComponentInParent<MobController>().isMobFightingNow)
        {
            if (!FightIcon.activeSelf)
            {
                if (BossIcon.activeSelf)
                {
                    BossIcon.GetComponent<RectTransform>().anchoredPosition = new Vector2(BossIcon.GetComponent<RectTransform>().anchoredPosition.x - 11.5f,
                                                                                  BossIcon.GetComponent<RectTransform>().anchoredPosition.y);
                    FightIcon.GetComponent<RectTransform>().anchoredPosition = new Vector2(FightIcon.GetComponent<RectTransform>().anchoredPosition.x + 11.5f,
                                                                                           FightIcon.GetComponent<RectTransform>().anchoredPosition.y);
                }
                FightIcon.SetActive(true);
            }
        } else
        {
            if (FightIcon.activeSelf)
            {
                FightIcon.SetActive(false);
                if (BossIcon.activeSelf)
                {
                    BossIcon.GetComponent<RectTransform>().anchoredPosition = new Vector2(BossIcon.GetComponent<RectTransform>().anchoredPosition.x + 11.5f,
                                                                                  BossIcon.GetComponent<RectTransform>().anchoredPosition.y);
                    FightIcon.GetComponent<RectTransform>().anchoredPosition = new Vector2(FightIcon.GetComponent<RectTransform>().anchoredPosition.x - 11.5f,
                                                                                           FightIcon.GetComponent<RectTransform>().anchoredPosition.y);
                }             
            }
        }
    }

    private int CalculateLevel()
    {
        int level = 0;
        if ((mobStatistics.attack <= playerStatistics.attack - 5) 
            || ((mobStatistics.attack <= playerStatistics.attack - 3) && mobStatistics.maxHealth <= playerStatistics.maxHealth))
        {
            // 1 - atak mniejszy o 5 lub mniej || atak miêdzy mniejszy o 5 a mniejszy o 3 (w³¹cznie) i mniejsze zdrowie
            level = 1;
        }
        else if (((mobStatistics.attack <= playerStatistics.attack - 3) && mobStatistics.maxHealth > playerStatistics.maxHealth)
            || ((mobStatistics.attack <= playerStatistics.attack - 1) && mobStatistics.maxHealth <= playerStatistics.maxHealth))
        {
            // 2 - atak miêdzy mniejszy o 5 a mniejszy o 3 (w³¹cznie) i wiêksze zdrowie
            // || atak miêdzy mniejszy o 3 a mniejszy o 1 (w³¹cznie) i mniejsze zdrowie
            level = 2;
        }
        else if (((mobStatistics.attack <= playerStatistics.attack - 1) && mobStatistics.maxHealth > playerStatistics.maxHealth)
            || ((mobStatistics.attack <= playerStatistics.attack + 1) && mobStatistics.maxHealth <= playerStatistics.maxHealth))
        {
            // 3 - atak miêdzy mniejszy o 3 a mniejszy o 1 (w³¹cznie) i wiêksze zdrowie
            // || atak miêdzy mniejszy o 1 a wiêkszy o 1 (w³¹cznie) i mniejsze zdrowie
            level = 3;
        }
        else if (((mobStatistics.attack <= playerStatistics.attack + 1) && mobStatistics.maxHealth > playerStatistics.maxHealth)
            || ((mobStatistics.attack <= playerStatistics.attack + 2) && mobStatistics.maxHealth <= playerStatistics.maxHealth))
        {
            // 4 - atak miêdzy mniejszy o 1 a wiêkszy o 1 (w³¹cznie) i wiêksze zdrowie
            // || atak miêdzy wiêkszy o 1 a wiêkszy o 2 (w³¹cznie) i mniejsze zdrowie
            level = 4;
        }
        else if (((mobStatistics.attack <= playerStatistics.attack + 2) && mobStatistics.maxHealth > playerStatistics.maxHealth)
            || ((mobStatistics.attack >= playerStatistics.attack + 3) && mobStatistics.maxHealth <= playerStatistics.maxHealth))
        {
            // 5 - atak miêdzy wiêkszy o 1 a wiêkszy o 2(w³¹cznie) i wiêksze zdrowie
            // || atak wiêkszy o 3(w³¹cznie) i wiêcej i mniejsze zdrowie
            level = 5;
        }
        else if (((mobStatistics.attack >= playerStatistics.attack + 3) && mobStatistics.maxHealth > playerStatistics.maxHealth))
        {
            // 6 (czaszka) - atak wiêkszy o 3 (w³¹cznie) lub wiêcej i wiêksze zdrowie
            level = 6;
            if (!BossIcon.activeSelf)
            {
                BossIcon.SetActive(true);
            }
        }

        return level;
    }

    IEnumerator DestroyInfoAfterSecond()
    {
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }

    public int GetMobLevel()
    {
        return mobLevel;
    }
}
