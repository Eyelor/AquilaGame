using UnityEngine;

[CreateAssetMenu(fileName = "PrefabRegistry", menuName = "ScriptableObjects/PrefabRegistry")]
public class PrefabRegistrySO : ScriptableObject
{
    [System.Serializable]
    private class PrefabEntry
    {
        public GameObject prefab;
    }

    [SerializeField] private PrefabEntry[] prefabs;

    public GameObject GetPrefabByName(string prefabName)
    {
        foreach (var entry in prefabs)
        {
            if (entry.prefab.name == prefabName)
            {
                return entry.prefab;
            }
        }
        Debug.LogWarning("Prefab with name " + prefabName + " not found in registry.");
        return null;
    }

    public string[] GetArrayOfAllPrefabsNames()
    {
        string[] prefabsNamesArray = new string[prefabs.Length];
        for (int i = 0; i < prefabs.Length; i++)
        {
            prefabsNamesArray[i] = prefabs[i].prefab.name;
        }
        return prefabsNamesArray;
    }
}