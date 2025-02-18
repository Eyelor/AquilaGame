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
    [SerializeField] private int minNumberOfMobs = 5; // Minimalna liczba mobów do wygenerowania
    [SerializeField] private int maxNumberOfMobs = 10; // Maksymalna liczba mobów do wygenerowania
    [SerializeField] private float mobSpacing = 5.0f; // Minimalna odleg³oœæ miêdzy postaciami 
    [SerializeField] private float maxSlope = 5.0f; // Maksymalne nachylenie terenu, na którym mo¿na generowaæ postacie
    [SerializeField] private Affiliation affiliation; // Przynale¿noœæ postaci do frakcji

    public (GameObject mobPrefab, int minNumberOfMobs, int maxNumberOfMobs, float mobSpacing, float maxSlope, Affiliation affiliation) GetLocalMobGenerationParameters()
    {
        return (mobPrefab, minNumberOfMobs, maxNumberOfMobs, mobSpacing, maxSlope, affiliation);
    }
}
