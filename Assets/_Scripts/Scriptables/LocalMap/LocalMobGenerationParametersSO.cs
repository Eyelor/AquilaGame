using UnityEngine;

public enum Affiliation
{
    blueFraction,
    greenFraction,
    redFraction
}

[CreateAssetMenu(fileName = "LocalMobGenerationParameters", menuName = "ScriptableObjects/LocalMobGenerationParameters")]
public class LocalMobGenerationParametersSO : ScriptableObject
{
    [SerializeField] private GameObject mobPrefab; // Prefab postaci do wygenerowania
    [SerializeField] private int minNumberOfMobs = 5; // Minimalna liczba mob�w do wygenerowania
    [SerializeField] private int maxNumberOfMobs = 10; // Maksymalna liczba mob�w do wygenerowania
    [SerializeField] private float mobSpacing = 5.0f; // Minimalna odleg�o�� mi�dzy postaciami 
    [SerializeField] private float maxSlope = 5.0f; // Maksymalne nachylenie terenu, na kt�rym mo�na generowa� postacie
    [SerializeField] private Affiliation affiliation; // Przynale�no�� postaci do frakcji

    public (GameObject mobPrefab, int minNumberOfMobs, int maxNumberOfMobs, float mobSpacing, float maxSlope, Affiliation affiliation) GetLocalMobGenerationParameters()
    {
        return (mobPrefab, minNumberOfMobs, maxNumberOfMobs, mobSpacing, maxSlope, affiliation);
    }
}
