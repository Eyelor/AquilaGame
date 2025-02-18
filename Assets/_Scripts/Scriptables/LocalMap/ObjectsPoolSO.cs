using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ObjectsPool", menuName = "ScriptableObjects/ObjectsPool")]
public class ObjectsPoolSO : ScriptableObject
{
    [SerializeField] private PrefabRegistrySO allStaticObjects;
    [SerializeField] private PrefabRegistrySO sandObjects;
    [SerializeField] private PrefabRegistrySO grassObjects;
    [SerializeField] private PrefabRegistrySO muddObjects;
    [SerializeField] private PrefabRegistrySO interactableObjects;

    public (string[] allStaticObjects, string[] sandObjects, string[] grassObjects, string[] muddObjects, string[] interactableObjects) GetObjectsPool()
    {
        return (
            allStaticObjects.GetArrayOfAllPrefabsNames(), 
            sandObjects.GetArrayOfAllPrefabsNames(), 
            grassObjects.GetArrayOfAllPrefabsNames(), 
            muddObjects.GetArrayOfAllPrefabsNames(), 
            interactableObjects.GetArrayOfAllPrefabsNames()
        );
    }
}
