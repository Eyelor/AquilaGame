using UnityEngine;

[CreateAssetMenu(fileName = "InitialPlayerStatistics", menuName = "ScriptableObjects/InitialPlayerStatistics")]
public class InitialPlayerStatisticsSO : ScriptableObject
{
    [SerializeField] private int level = 1;
    [SerializeField] private long experience = 0;
    [SerializeField] private long experienceToNextLevel = 100;
    [SerializeField] private bool isFighting = false;
    [SerializeField] private bool isPlayerAlive = true;
    [SerializeField] private int health = 100;
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int healthRegen = 2;
    [SerializeField] private int attack = 10;
    [SerializeField] private int luck = 2;

    public int Level => level;
    public long Experience => experience;
    public long ExperienceToNextLevel => experienceToNextLevel;
    public bool IsFighting => isFighting;
    public bool IsPlayerAlive => isPlayerAlive;
    public int Health => health;
    public int MaxHealth => maxHealth;
    public int HealthRegen => healthRegen;
    public int Attack => attack;
    public int Luck => luck;
}
