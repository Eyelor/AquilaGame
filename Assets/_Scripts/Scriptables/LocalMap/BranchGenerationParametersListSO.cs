using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BranchGenerationParameters
{
    [SerializeField] private int lowerStartPoint = 50; // Punkt œcie¿ki g³ównej jako dolny zakres losowania pocz¹tku odnogi œcie¿ki
    [SerializeField] private int higherStartPoint = 150; // Punkt œcie¿ki g³ównej jako górny zakres losowania pocz¹tku odnogi œcie¿ki
    [SerializeField] private bool direction = true; // Kierunek odnogi œciezki (true = w prawo, false = w lewo)

    public int LowerStartPoint => lowerStartPoint;
    public int HigherStartPoint => higherStartPoint;
    public bool Direction => direction;
}

[CreateAssetMenu(fileName = "BranchGenerationParametersList", menuName = "ScriptableObjects/BranchGenerationParametersList")]
public class BranchGenerationParametersListSO : ScriptableObject
{
    [SerializeField] private List<BranchGenerationParameters> branchGenerationParametersList = new List<BranchGenerationParameters>();

    public List<BranchGenerationParameters> GetBranchGenerationParametersList()
    {
        return branchGenerationParametersList;
    }
}
