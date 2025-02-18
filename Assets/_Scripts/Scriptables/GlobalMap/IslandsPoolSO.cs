using UnityEngine;

[CreateAssetMenu(fileName = "IslandsPool", menuName = "ScriptableObjects/IslandsPool")]
public class IslandsPoolSO : ScriptableObject
{
    [SerializeField] private string[] types = { 
        "sandy",
        "muddy",
        "grassy"
    };
    [SerializeField] private string[] sizes = {
        "small",
        "medium",
        "large"
    };
    [SerializeField] private string[] affiliations = {
        "redFraction",
        "blueFraction",
        "greenFraction"
    };

    public (string[] types, string[] sizes, string[] affiliations) GetIslandsPool()
    {
        return (types, sizes, affiliations);
    }
}
