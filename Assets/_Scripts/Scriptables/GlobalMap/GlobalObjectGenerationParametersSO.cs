using UnityEngine;

[CreateAssetMenu(fileName = "GlobalObjectGenerationParameters", menuName = "ScriptableObjects/GlobalObjectGenerationParameters")]
public class GlobalObjectGenerationParametersSO : ScriptableObject
{
    [SerializeField] internal GameObject objectPrefab; // Prefab obiektu do wygenerowania
    [SerializeField] private int minNumberOfObjects = 36; // Minimalna liczba obiektów do wygenerowania
    [SerializeField] private int maxNumberOfObjects = 108; // Maksymalna liczba obiektów do wygenerowania
    [SerializeField] private float objectSpacing = 100.0f; // Minimalna odleg³oœæ miêdzy obiektami
    [SerializeField] private float height = 20.0f; // Wysokoœæ, na której obiekty maj¹ byæ generowane

    public (GameObject objectPrefab, int minNumberOfObjects, int maxNumberOfObjects, float objectSpacing, float height) GetGlobalObjectGenerationParameters()
    {
        return (objectPrefab, minNumberOfObjects, maxNumberOfObjects, objectSpacing, height);
    }
}
