using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;

public class SimulationFightTests
{
    private GameObject player;
    private PlayerDamage playerDamageScript;
    private GameObject enemy;
    private MobDamage enemyDamageScript;

    [SetUp]
    public void Setup()
    {
        TestEnvironment.IsPlayModeTest = true;
        Debug.Log("Pocz¹tek testu SimulationFightTests: " + TestEnvironment.IsPlayModeTest);

        // Tworzyenie symulowanego obiektu gracza
        player = new GameObject("Player");
        var fakePlayerController = player.AddComponent<FakePlayerController>();
        fakePlayerController.statistics = new PlayerStatistics
        {
            level = 1,
            experience = 0,
            experienceToNextLevel = 100,
            isFighting = true,
            isPlayerAlive = true,
            health = 150,
            maxHealth = 150,
            healthRegen = 2,
            attack = 12,
            luck = 2
        };
        playerDamageScript = player.AddComponent<PlayerDamage>();
        playerDamageScript.statistics = fakePlayerController.statistics;
        player.tag = "Player";
        player.AddComponent<BoxCollider>();
        

        // Tworzenie symulowanego obiektu przeciwnika
        enemy = new GameObject("Enemy");
        var enemyStats = enemy.AddComponent<MobStatisticComponent>();
        enemyStats.mobStatistics.health = 150;
        enemyStats.mobStatistics.maxHealth = 150;
        enemyStats.mobStatistics.attack = 12;
        enemyStats.mobStatistics.luck = 2;    
        enemyStats.mobStatistics.healthRegen = 2;
        enemyDamageScript = enemy.AddComponent<MobDamage>();
        enemyDamageScript.statistics = enemyStats.mobStatistics;
        enemy.tag = "enemy";
        enemy.AddComponent<BoxCollider>();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(player);
        Object.DestroyImmediate(enemy);
        TestEnvironment.IsPlayModeTest = false;
        Debug.Log("Koniec testu SimulationFightTests: " + TestEnvironment.IsPlayModeTest);
    }

    [UnityTest]
    public IEnumerator SimulationFight_TestfightPlayerWithMobSameStatistics()
    {
        enemyDamageScript.statistics.health = 150;
        enemyDamageScript.statistics.attack = 12;
        enemyDamageScript.statistics.luck = 2;
        playerDamageScript.statistics.health = 150;
        playerDamageScript.statistics.attack = 12;
        playerDamageScript.statistics.luck = 2;
        bool isMobAlive = true;
        // Symulacja walki
        for (int i = 0; i < 100; i++)
        {
            playerDamageScript.OnTriggerEnter(enemy.GetComponent<Collider>());
            if(enemyDamageScript.statistics.health <= 0)
            {
                isMobAlive = false;
                break;
            }
            enemyDamageScript.OnTriggerEnter(player.GetComponent<Collider>());
            if (playerDamageScript.statistics.health <= 0)
            {
                playerDamageScript.statistics.isPlayerAlive = false;
                break;
            }
            playerDamageScript.damagedEnemies.Clear();
            yield return null;
        }
        Debug.Log("PLAYER ALIVE: " + playerDamageScript.statistics.isPlayerAlive + ", HEALTH: " + playerDamageScript.statistics.health);
        Debug.Log("MOB ALIVE: " + isMobAlive + ", HEALTH: " + enemyDamageScript.statistics.health);
        Assert.AreNotEqual(isMobAlive, playerDamageScript.statistics.isPlayerAlive,
            "Gracz i przeciwnik ¿yj¹ oboje lub oboje s¹ martwi. Walka zakoñczona b³êdnie");
    }

    [UnityTest]
    public IEnumerator SimulationFight_TestfightPlayerWithMob_PlayerStronger()
    {
        enemyDamageScript.statistics.health = 150;
        enemyDamageScript.statistics.attack = 12;
        enemyDamageScript.statistics.luck = 2;
        playerDamageScript.statistics.health = 180;
        playerDamageScript.statistics.attack = 15;
        playerDamageScript.statistics.luck = 3;
        bool isMobAlive = true;
        // Symulacja walki
        for (int i = 0; i < 100; i++)
        {
            playerDamageScript.OnTriggerEnter(enemy.GetComponent<Collider>());
            if (enemyDamageScript.statistics.health <= 0)
            {
                isMobAlive = false;
                break;
            }
            enemyDamageScript.OnTriggerEnter(player.GetComponent<Collider>());
            if (playerDamageScript.statistics.health <= 0)
            {
                playerDamageScript.statistics.isPlayerAlive = false;
                break;
            }
            playerDamageScript.damagedEnemies.Clear();
            yield return null;
        }
        Debug.Log("PLAYER ALIVE: " + playerDamageScript.statistics.isPlayerAlive + ", HEALTH: " + playerDamageScript.statistics.health);
        Debug.Log("MOB ALIVE: " + isMobAlive + ", HEALTH: " + enemyDamageScript.statistics.health);
        Assert.AreNotEqual(isMobAlive, playerDamageScript.statistics.isPlayerAlive,
            "Gracz i przeciwnik ¿yj¹ oboje lub oboje s¹ martwi. Walka zakoñczona b³êdnie");
    }

    [UnityTest]
    public IEnumerator SimulationFight_TestfightPlayerWithMob_MobStronger()
    {
        enemyDamageScript.statistics.health = 180;
        enemyDamageScript.statistics.attack = 15;
        enemyDamageScript.statistics.luck = 3;
        playerDamageScript.statistics.health = 150;
        playerDamageScript.statistics.attack = 12;
        playerDamageScript.statistics.luck = 2;
        bool isMobAlive = true;
        // Symulacja walki
        for (int i = 0; i < 100; i++)
        {
            playerDamageScript.OnTriggerEnter(enemy.GetComponent<Collider>());
            if (enemyDamageScript.statistics.health <= 0)
            {
                isMobAlive = false;
                break;
            }
            enemyDamageScript.OnTriggerEnter(player.GetComponent<Collider>());
            if (playerDamageScript.statistics.health <= 0)
            {
                playerDamageScript.statistics.isPlayerAlive = false;
                break;
            }
            playerDamageScript.damagedEnemies.Clear();
            yield return null;
        }
        Debug.Log("PLAYER ALIVE: " + playerDamageScript.statistics.isPlayerAlive + ", HEALTH: " + playerDamageScript.statistics.health);
        Debug.Log("MOB ALIVE: " + isMobAlive + ", HEALTH: " + enemyDamageScript.statistics.health);
        Assert.AreNotEqual(isMobAlive, playerDamageScript.statistics.isPlayerAlive,
            "Gracz i przeciwnik ¿yj¹ oboje lub oboje s¹ martwi. Walka zakoñczona b³êdnie");
    }
}
