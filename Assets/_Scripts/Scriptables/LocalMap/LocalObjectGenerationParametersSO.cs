using UnityEngine;

[CreateAssetMenu(fileName = "LocalObjectGenerationParameters", menuName = "ScriptableObjects/LocalObjectGenerationParameters")]
public class LocalObjectGenerationParametersSO : ScriptableObject
{
    [SerializeField] private GameObject objectPrefab; // Prefab obiektu do wygenerowania
    [SerializeField] private int minNumberOfObjects = 5; // Minimalna liczba obiektów do wygenerowania
    [SerializeField] private int maxNumberOfObjects = 10; // Maksymalna liczba obiektów do wygenerowania
    [SerializeField] private float objectSpacing = 5.0f; // Minimalna odleg³oœæ miêdzy obiektami
    [SerializeField] private float maxSlope = 5.0f; // Maksymalne nachylenie terenu, na którym mo¿na generowaæ obiekty
    [SerializeField] private bool isInteractable = false; // Czy obiekt jest statyczny czy interaktywny

    public (GameObject objectPrefab, int minNumberOfObjects, int maxNumberOfObjects, float objectSpacing, float maxSlope, bool isInteractable) GetLocalObjectGenerationParameters()
    {
        return (objectPrefab, minNumberOfObjects, maxNumberOfObjects, objectSpacing, maxSlope, isInteractable);
    }
}
