using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;

public class PlayerDamageTests
{
    private GameObject player;
    private PlayerDamage playerDamageScript;
    private GameObject enemy, enemy2;
    private MobStatisticComponent enemyStats, enemyStats2;
    private int mobStartHealth = 130;

    [SetUp]
    public void Setup()
    {
        TestEnvironment.IsPlayModeTest = true;
        Debug.Log("Pocz�tek testu PlayerDamageTests: " + TestEnvironment.IsPlayModeTest);

        // Tworzyenie symulowanego obiektu gracza
        player = new GameObject("Player");
        var fakePlayerController = player.AddComponent<FakePlayerController>();
        fakePlayerController.statistics = new PlayerStatistics
        {
            level = 3,
            experience = 300,
            experienceToNextLevel = 500,
            isFighting = true,
            isPlayerAlive = true,
            health = 125,
            maxHealth = 125,
            healthRegen = 3,
            attack = 12,
            luck = 2
        };

        // Inicjalizacja skryptu gracza z obra�eniami
        playerDamageScript = player.AddComponent<PlayerDamage>();
        playerDamageScript.statistics = fakePlayerController.statistics;

        // Tworzenie symulowanych obiekt�w przeciwnika
        enemy = new GameObject("Enemy");
        enemyStats = enemy.AddComponent<MobStatisticComponent>();
        enemyStats.mobStatistics.health = mobStartHealth;
        enemy.tag = "enemy";
        enemy2 = new GameObject("Enemy2");
        enemyStats2 = enemy2.AddComponent<MobStatisticComponent>();
        enemyStats2.mobStatistics.health = mobStartHealth;
        enemy2.tag = "enemy";

        // Dodanie komponentu Collider do przeciwnik�w
        enemy.AddComponent<BoxCollider>();
        enemy2.AddComponent<BoxCollider>();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(player);
        Object.DestroyImmediate(enemy);
        Object.DestroyImmediate(enemy2);
        TestEnvironment.IsPlayModeTest = false;
        Debug.Log("Koniec testu PlayerDamageTests: " + TestEnvironment.IsPlayModeTest);
    }

    [UnityTest]
    public IEnumerator PlayerDamage_TestIsGivenCalculateDamage()
    {
        bool damageDealt = false;

        // Symulujemy obra�enia a� nie b�dzie "miss"
        for (int i = 0; i < 10; i++)
        {           
            playerDamageScript.OnTriggerEnter(enemy.GetComponent<Collider>());
            foreach (var damagedEnemy in playerDamageScript.damagedEnemies)
            {
                var currentEnemyHealth = damagedEnemy.GetComponent<MobStatisticComponent>().mobStatistics.health;
                if (currentEnemyHealth < mobStartHealth) // Obra�enia zadane
                {
                    damageDealt = true;
                    break;
                }
            }
            if (damageDealt)
            {
                break;
            }
            playerDamageScript.damagedEnemies.Clear();
            yield return new WaitForSeconds(1); // symulacja trwania animacji
        }

        Assert.IsTrue(damageDealt, "Przeciwnik powinien dosta� obra�enia w 10 atakach przy luck = " 
            + playerDamageScript.statistics.luck);
    }
    
    [UnityTest]
    public IEnumerator PlayerDamage_TestKillEnemy()
    {
        // Symulacja ca�o�ci zadawania obra�e� przez gracza
        for (int i = 0; i < 100; i++)
        {
            playerDamageScript.OnTriggerEnter(enemy.GetComponent<Collider>());
            if (enemyStats.mobStatistics.health <= 0) break;
            playerDamageScript.damagedEnemies.Clear();
            yield return new WaitForSeconds(1);
        }

        Assert.LessOrEqual(enemyStats.mobStatistics.health, 0, 
            "Przecinik nie zgin�� w 100 pr�bach ataku. Walka nie jest do ko�ca poprawna.");
    }

    [UnityTest]
    public IEnumerator PlayerDamage_TestEnemiesAreHitOnlyOnceInAttack()
    {
        // Symulowanie ataku na dw�ch przeciwnik�w
        playerDamageScript.OnTriggerEnter(enemy.GetComponent<Collider>());
        playerDamageScript.OnTriggerEnter(enemy2.GetComponent<Collider>());
        yield return new WaitForSeconds(1);

        int enemy1healthAfterAttack = enemyStats.mobStatistics.health;
        int enemy2healthAfterAttack = enemyStats2.mobStatistics.health;

        // Weryfikacja, �e przeciwnik 1 jest na li�cie
        Assert.IsTrue(playerDamageScript.damagedEnemies.Contains(enemy), 
            "Przeciwnik 1 powinien by� na li�cie trafionych wrog�w podczas jednego ataku.");

        // Weryfikacja, �e przeciwnik 2 jest na li�cie
        Assert.IsTrue(playerDamageScript.damagedEnemies.Contains(enemy2),
            "Przeciwnik 2 powinien by� na li�cie trafionych wrog�w podczas jednego ataku.");

        // Symulacja ponownego ataku na tych samych przeciwnik�w
        playerDamageScript.OnTriggerEnter(enemy.GetComponent<Collider>());
        playerDamageScript.OnTriggerEnter(enemy2.GetComponent<Collider>());
        yield return new WaitForSeconds(1);

        Assert.AreEqual(enemy1healthAfterAttack, enemyStats.mobStatistics.health,
            "Przeciwnik 1 musi mie� tyle samo zdrowia co po pierwszym ataku; nie m�g� zosta� zaatakowany drugi raz w czasie jego trwania.");

        Assert.AreEqual(enemy2healthAfterAttack, enemyStats2.mobStatistics.health,
           "Przeciwnik 2 musi mie� tyle samo zdrowia co po pierwszym ataku; nie m�g� zosta� zaatakowany drugi raz w czasie jego trwania.");

        // Weryfikacja, �e lista zaatakowanych przeciwnik�w w trakcie trwania
        // Jednego ataku jest r�wna 2
        Assert.AreEqual(2, playerDamageScript.damagedEnemies.Count, 
            "Jeden przeciwnik powinien by� trafiony tylko raz podczas trwania jednego ataku.");

        playerDamageScript.damagedEnemies.Clear();
    }
}
