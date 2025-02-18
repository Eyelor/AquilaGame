using UnityEngine;

[CreateAssetMenu(fileName = "LocalObjectGenerationParameters", menuName = "ScriptableObjects/LocalObjectGenerationParameters")]
public class LocalObjectGenerationParametersSO : ScriptableObject
{
    [SerializeField] private GameObject objectPrefab; // Prefab obiektu do wygenerowania
    [SerializeField] private int minNumberOfObjects = 5; // Minimalna liczba obiekt�w do wygenerowania
    [SerializeField] private int maxNumberOfObjects = 10; // Maksymalna liczba obiekt�w do wygenerowania
    [SerializeField] private float objectSpacing = 5.0f; // Minimalna odleg�o�� mi�dzy obiektami
    [SerializeField] private float maxSlope = 5.0f; // Maksymalne nachylenie terenu, na kt�rym mo�na generowa� obiekty
    [SerializeField] private bool isInteractable = false; // Czy obiekt jest statyczny czy interaktywny

    public (GameObject objectPrefab, int minNumberOfObjects, int maxNumberOfObjects, float objectSpacing, float maxSlope, bool isInteractable) GetLocalObjectGenerationParameters()
    {
        return (objectPrefab, minNumberOfObjects, maxNumberOfObjects, objectSpacing, maxSlope, isInteractable);
    }
}
