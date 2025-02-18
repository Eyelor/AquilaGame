using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;

public class GlobalIslandsGenerationTests
{
    private GameObject managerObject, parentObject;
    private GlobalIslandsGenerationManager manager;
    private List<GlobalObjectGenerationParametersSO> globalObjectsToGenerate;
    private IslandsPoolSO islandsPoolSO;
    private List<GameObject> cubes;
    private Quaternion rotation;
    private float objectSpacing;
    private BoxCollider boxCollider;
    

    [SetUp]
    public void SetUp()
    {
        TestEnvironment.IsPlayModeTest = true;
        Debug.Log("Pocz¹tek testu GlobalIslandsGenerationTests: " + TestEnvironment.IsPlayModeTest);

        // Tworzenie obiektu z przypisanym skryptem singletonu
        managerObject = new GameObject("GlobalIslandsGenerationManager");
        manager = managerObject.AddComponent<GlobalIslandsGenerationManager>();

        // Tworzenie rodzica dla generowanych obiektów
        parentObject = new GameObject("TestIslandsParent");
        manager.parent = parentObject.transform;

        // Tworzenie obiektu IslandsPoolSO i przypisanie danych
        islandsPoolSO = ScriptableObject.CreateInstance<IslandsPoolSO>();
        manager.islandsPool = islandsPoolSO.GetIslandsPool();

        // Tworzenie parametrów generacji i prefabu testowego
        var generationParameters = ScriptableObject.CreateInstance<GlobalObjectGenerationParametersSO>();
        var cubePrefab = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        cubePrefab.name = "Cube";
        cubePrefab.transform.localScale = new Vector3(40f, 40f, 40f);
        // Wymuszenie aktualizacji BoxCollider
        var collider = cubePrefab.GetComponent<CapsuleCollider>();
        if (collider != null)
        {
            GameObject.DestroyImmediate(collider);
            cubePrefab.AddComponent<CapsuleCollider>();
        }
        generationParameters.objectPrefab = cubePrefab;

        // Przypisanie parametrów generacji
        globalObjectsToGenerate = new List<GlobalObjectGenerationParametersSO> { generationParameters };
        manager.globalObjectsToGenerate = globalObjectsToGenerate;

        // Ustawienie singletonu
        GlobalIslandsGenerationManager.Instance = manager;

        // Generowanie obiektów
        manager.GenerateObjects(globalObjectsToGenerate[0]);
        cubes = new List<GameObject>();
        foreach (Transform child in manager.parent)
        {
            if (child.name == "Cube(Clone)")
            {
                child.tag = "testIsland";
                cubes.Add(child.gameObject);
            }
        }
    }

    [TearDown]
    public void TearDown()
    {
        // Czyszczenie sceny po teœcie
        Object.DestroyImmediate(managerObject);
        Object.DestroyImmediate(parentObject);

        foreach (var param in globalObjectsToGenerate)
        {
            Object.DestroyImmediate(param.objectPrefab);
        }
        foreach (var cube in cubes)
        {
            Object.DestroyImmediate(cube);
        }
        TestEnvironment.IsPlayModeTest = false;
        Debug.Log("Koniec testu GlobalIslandsGenerationTests: " + TestEnvironment.IsPlayModeTest);
    }

    [UnityTest]
    public IEnumerator GlobalGenerator_CheckIslandsCount()
    {
        var globalObjectGenerationParameters = globalObjectsToGenerate[0].GetGlobalObjectGenerationParameters();
        yield return new WaitForSeconds(1f); 
        
        // Sprawdzenie czy wygenerowana iloœæ obiektów jest z w³aœciwego zakresu
        int minObjects = globalObjectGenerationParameters.minNumberOfObjects;
        int maxObjects = globalObjectGenerationParameters.maxNumberOfObjects;
        Assert.IsTrue(cubes.Count >= minObjects && cubes.Count <= maxObjects,
            $"Liczba wygenerowanych obiektów ({cubes.Count}) jest poza zakresem [{minObjects}, {maxObjects}].");
    }

    [UnityTest]
    public IEnumerator GlobalGenerator_CheckParentAssignment()
    {
        yield return new WaitForSeconds(1f);

        // Sprawdzenie, czy ka¿dy obiekt ma przypisanego odpowiedniego rodzica
        foreach (var cube in cubes)
        {
            Assert.AreEqual(manager.parent, cube.transform.parent, $"Obiekt {cube.name} nie zosta³ przypisany do rodzica.");
        }
    }

    [UnityTest]
    public IEnumerator GlobalGenerator_CheckIslandsComponents()
    {
        yield return new WaitForSeconds(1f);

        // Sprawdzenie, czy ka¿dy obiekt ma komponenty IslandDataComponent i Island
        foreach (var cube in cubes)
        {
            Assert.IsNotNull(cube.GetComponent<IslandDataComponent>(),
                $"Obiekt {cube.name} nie ma komponentu IslandDataComponent.");
            Assert.IsNotNull(cube.GetComponent<Island>(),
                $"Obiekt {cube.name} nie ma komponentu Island.");
        }
    }

    [UnityTest]
    public IEnumerator GlobalGenerator_CheckIslandsPosition()
    {
        yield return new WaitForSeconds(1f);

        // Sprawdzenie czy obiekty mieszcz¹ siê w okreœlonym zakresie przestrzeni
        foreach (var cube in cubes)
        {
            Vector3 position = cube.transform.position;
            Assert.IsTrue(position.x >= manager.minX && position.x <= manager.maxX,
                $"Obiekt {cube.name} ma nieprawid³ow¹ pozycjê X: {position.x}");
            Assert.IsTrue(position.y == 20, 
                $"Obiekt {cube.name} ma nieprawid³ow¹ pozycjê Y: {position.y}");
            Assert.IsTrue(position.z >= manager.minZ && position.z <= manager.maxZ,
                $"Obiekt {cube.name} ma nieprawid³ow¹ pozycjê Z: {position.z}");
        }
    }

    [UnityTest]
    public IEnumerator GlobalGenerator_CheckIslandsCollision()
    {
        var globalObjectGenerationParameters = globalObjectsToGenerate[0].GetGlobalObjectGenerationParameters();

        for (int repeat = 0; repeat < 100; repeat++)
        {
            Debug.Log($"Powtórzenie testu: {repeat + 1}");

            // Testowanie kolizji na nowo wygenerowanych obiektach
            foreach (var cube in cubes)
            {
                Vector3 position = cube.transform.position;
                Debug.Log($"Test obiektu: {cube.name} na pozycji {position}");

                CapsuleCollider boxCollider = globalObjectGenerationParameters.objectPrefab.GetComponent<CapsuleCollider>();
                Vector3 extents = 2 * boxCollider.bounds.extents;

                Collider[] colliders = Physics.OverlapBox(position, extents, cube.transform.rotation);

                foreach (var collider in colliders)
                {
                    Debug.Log($"Collided with: {collider.gameObject.name} + {collider.gameObject.transform.position}");
                }

                Assert.AreEqual(1, colliders.Length, $"Obiekt {cube.name} zachodzi na inny obiekt (zbyt ma³a przestrzeñ) w przebiegu {repeat + 1}.");
            }

            // Czyszczenie wygenerowanych obiektów po teœcie
            foreach (var cube in cubes)
            {
                Object.DestroyImmediate(cube);
            }

            // Tworzymy nowe obiekty w ka¿dej iteracji
            cubes = new List<GameObject>();

            // Generowanie obiektów
            manager.GenerateObjects(globalObjectsToGenerate[0]);

            // Dodajemy wygenerowane obiekty do listy cubes
            foreach (Transform child in manager.parent)
            {
                if (child.name == "Cube(Clone)")
                {
                    child.tag = "testIsland";
                    cubes.Add(child.gameObject);
                }
            }

            yield return null;
        }
    }

}
