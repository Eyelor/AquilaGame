using System.Collections.Generic;
using UnityEngine;

public class GlobalIslandsGenerationManager : Singleton<GlobalIslandsGenerationManager>
{
    [SerializeField] internal List<GlobalObjectGenerationParametersSO> globalObjectsToGenerate; // Lista obiektów do wygenerowania
    [SerializeField] private float maxNegativeX = -1000.0f; // Maksymalna wartoœæ wspó³rzêdnej X
    [SerializeField] private float maxPositiveX = 1000.0f; // Minimalna wartoœæ wspó³rzêdnej X
    [SerializeField] private float maxNegativeZ = -1000.0f; // Maksymalna wartoœæ wspó³rzêdnej Z
    [SerializeField] private float maxPositiveZ = 1000.0f; // Minimalna wartoœæ wspó³rzêdnej Z
    [SerializeField] private float outBorder = 50.0f; // Margines granic zewnêtrznych terenu
                  
    [SerializeField] internal Transform parent; // Obiekt nadrzêdny dla wygenerowanych obiektów
                     
    [SerializeField] private PrefabRegistrySO globalMapPrefabRegistry;
    [SerializeField] internal IslandsPoolSO islandsPoolSO;

    // Ustalanie granic zewnêtrznych obszaru, w których obiekty mog¹ byæ generowane
    internal float minX;
    internal float maxX;
    internal float minZ;
    internal float maxZ;

    internal (string[] types, string[] sizes, string[] affiliations) islandsPool;
    private int IslandIdCounter = 1; // Identyfikator wszystkich wygenerowanych wysp

    private CurrentIslandDataComponent currentIslandDataComponent;

    private SaveManager saveManager;

    protected override void Awake()
    {
        base.Awake();
        
        minX = maxNegativeX + outBorder;
        maxX = maxPositiveX - outBorder;
        minZ = maxNegativeZ + outBorder;
        maxZ = maxPositiveZ - outBorder;


        if (islandsPoolSO != null)
        {
            islandsPool = islandsPoolSO.GetIslandsPool();
        }

        // Sprawdzenie, czy kod dzia³a w trybie testów
        if (!TestEnvironment.IsPlayModeTest)
        {
            saveManager = SaveManager.Instance;

            LoadIslands();
            LoadPlayerShip();
            LoadCurrentIsland();
        }
    }

    #region Loading Objects
    private void LoadIslands()
    {
        IslandDataList islandDataList = saveManager.LoadIslandDataList();

        if (islandDataList == null)
        {
            Debug.Log("Generating objects on global map.");
            foreach (var objParams in globalObjectsToGenerate)
            {
                GenerateObjects(objParams);
            }

            Debug.Log("Saving objects data on global map.");
            saveManager.SaveIslandDataList();
        }
        else
        {
            foreach (var data in islandDataList.islandsDataList)
            {
                // Convert rotation from Vector3 to Quaternion
                Quaternion rotation = Quaternion.Euler(data.location.rotX, data.location.rotY, data.location.rotZ);

                // Load the prefab from Resources
                GameObject prefab = globalMapPrefabRegistry.GetPrefabByName(data.prefabName);
                if (prefab != null)
                {
                    // Instantiate the prefab
                    GameObject newObject = Instantiate(prefab, new Vector3(data.location.posX, data.location.posY, data.location.posZ), rotation, parent);

                    // Add IslandDataComponent and set its data
                    var dataComponent = newObject.AddComponent<IslandDataComponent>();
                    dataComponent.islandData = data;

                    var islandInteractable = newObject.AddComponent<Island>();
                    islandInteractable.SetPromptShip("ZEJŒÆ NA WYSPÊ");
                    newObject.layer = LayerMask.NameToLayer("Interactable");
                }
            }
            Debug.Log("Objects data on global map was loaded.");
        }
    }

    private void LoadPlayerShip()
    {
        PlayerShipData playerShipData = saveManager.LoadPlayerShipData();

        if (playerShipData == null)
        {
            Debug.Log("Getting data from player ship on global map.");

            ShipController[] shipControllers = FindObjectsOfType<ShipController>();

            if (shipControllers.Length == 1)
            {
                GameObject playerShip = shipControllers[0].gameObject;
                var playerShipDataComponent = playerShip.AddComponent<PlayerShipDataComponent>();
                playerShipDataComponent.playerShipData = new PlayerShipData
                {
                    location = new Location
                    {
                        posX = playerShip.transform.position.x,
                        posY = playerShip.transform.position.y,
                        posZ = playerShip.transform.position.z,
                        rotX = playerShip.transform.rotation.eulerAngles.x,
                        rotY = playerShip.transform.rotation.eulerAngles.y,
                        rotZ = playerShip.transform.rotation.eulerAngles.z
                    },
                    isPlayerInside = true
                };
            }
            else
            {
                Debug.LogError("ShipController empty or more than one instance.");
            }

            Debug.Log("Saving player ship data on global map.");
            saveManager.SavePlayerShipData();
        }
        else
        {
            ShipController[] shipControllers = FindObjectsOfType<ShipController>();

            if (shipControllers.Length == 1)
            {
                GameObject playerShip = shipControllers[0].gameObject;
                var playerShipDataComponent = playerShip.AddComponent<PlayerShipDataComponent>();
                playerShipDataComponent.playerShipData = new PlayerShipData
                {
                    location = new Location
                    {
                        posX = playerShipData.location.posX,
                        posY = playerShipData.location.posY,
                        posZ = playerShipData.location.posZ,
                        rotX = playerShipData.location.rotX,
                        rotY = playerShipData.location.rotY,
                        rotZ = playerShipData.location.rotZ
                    },
                    isPlayerInside = playerShipData.isPlayerInside
                };

                if (!playerShipData.isPlayerInside)
                {
                    playerShipDataComponent.playerShipData.isPlayerInside = true;
                }

                saveManager.SavePlayerShipData();

                playerShip.transform.position = new Vector3(playerShipData.location.posX, playerShipData.location.posY, playerShipData.location.posZ);
                playerShip.transform.rotation = Quaternion.Euler(playerShipData.location.rotX, playerShipData.location.rotY, playerShipData.location.rotZ);
            }
            else
            {
                Debug.LogError("ShipController empty or more than one instance.");
            }

            Debug.Log("Player ship data on global map was loaded.");
        }
    }

    private void LoadCurrentIsland()
    {
        CurrentIslandData currentIslandData = saveManager.LoadCurrentIslandData();
        if (currentIslandDataComponent == null)
        {
            currentIslandDataComponent = gameObject.AddComponent<CurrentIslandDataComponent>();
            if (currentIslandData != null)
            {
                currentIslandDataComponent.currentIslandData = new CurrentIslandData
                {
                    islandId = currentIslandData.islandId,
                    islandType = currentIslandData.islandType,
                    islandSize = currentIslandData.islandSize,
                    islandAffiliation = currentIslandData.islandAffiliation,
                    isPlayerInside = currentIslandData.isPlayerInside,
                };
            }

        }
    }
    #endregion

    internal void GenerateObjects(GlobalObjectGenerationParametersSO objParams)
    {
        var globalObjectGenerationParameters = objParams.GetGlobalObjectGenerationParameters();

        // Sprawdzenie czy wszystkie wymagane zmienne s¹ ustawione
        if (globalObjectGenerationParameters.objectPrefab == null)
        {
            Debug.LogError("Object Prefab is not in inspector.");
            return;
        }
        string typeIsland = "", sizeIsland = "";
        if (globalObjectGenerationParameters.objectPrefab.name == "MiniIslandPiaSDet") { typeIsland = islandsPool.types[0]; sizeIsland = islandsPool.sizes[0]; }
        else if (globalObjectGenerationParameters.objectPrefab.name == "MiniIslandPiaMDet") { typeIsland = islandsPool.types[0]; sizeIsland = islandsPool.sizes[1]; }
        else if (globalObjectGenerationParameters.objectPrefab.name == "MiniIslandPiaLDet") { typeIsland = islandsPool.types[0]; sizeIsland = islandsPool.sizes[2]; }
        else if (globalObjectGenerationParameters.objectPrefab.name == "MiniIslandBloSDet") { typeIsland = islandsPool.types[1]; sizeIsland = islandsPool.sizes[0]; }
        else if (globalObjectGenerationParameters.objectPrefab.name == "MiniIslandBloMDet") { typeIsland = islandsPool.types[1]; sizeIsland = islandsPool.sizes[1]; }
        else if (globalObjectGenerationParameters.objectPrefab.name == "MiniIslandBloLDet") { typeIsland = islandsPool.types[1]; sizeIsland = islandsPool.sizes[2]; }
        else if (globalObjectGenerationParameters.objectPrefab.name == "MiniIslandTraSDet") { typeIsland = islandsPool.types[2]; sizeIsland = islandsPool.sizes[0]; }
        else if (globalObjectGenerationParameters.objectPrefab.name == "MiniIslandTraMDet") { typeIsland = islandsPool.types[2]; sizeIsland = islandsPool.sizes[1]; }
        else if (globalObjectGenerationParameters.objectPrefab.name == "MiniIslandTraLDet") { typeIsland = islandsPool.types[2]; sizeIsland = islandsPool.sizes[2]; }

        // Losowanie liczby obiektów do wygenerowania
        int numberOfObjects = Random.Range(globalObjectGenerationParameters.minNumberOfObjects, globalObjectGenerationParameters.maxNumberOfObjects + 1);
        Debug.Log("Iloœæ wygenerowanych obiektów: " + numberOfObjects);

        for (int i = 0; i < numberOfObjects; i++)
        {
            Vector3 position = Vector3.zero;
            bool positionFound = false;
            int attempts = 0;

            if (TestEnvironment.IsPlayModeTest)
            {
                typeIsland = islandsPool.types[Random.Range(0, 3)];
                sizeIsland = islandsPool.sizes[Random.Range(0, 3)];
            }

            while (!positionFound && attempts < 100)
            {
                attempts++;
                // Losowe pozycjonowanie obiektu
                float x = Random.Range(minX, maxX);
                float z = Random.Range(minZ, maxZ);
                float y = globalObjectGenerationParameters.height;
                position = new Vector3(x, y, z);

                // Sprawdzanie czy prefab obiektu posiada Box Collider
                CapsuleCollider capsuleCollider = globalObjectGenerationParameters.objectPrefab.GetComponent<CapsuleCollider>();
                if (capsuleCollider == null)
                {
                    Debug.LogError("Box Collider component not found on the object prefab.");
                    return;
                }

                // Okreœlenie wolnej przstrzeni jaka ma byæ wokó³ obiektu (czêstotliwoœæ wystêpowania)
                Vector3 extents = capsuleCollider.bounds.extents + new Vector3(globalObjectGenerationParameters.objectSpacing, globalObjectGenerationParameters.objectSpacing, globalObjectGenerationParameters.objectSpacing) / 2;
                Collider[] colliders = Physics.OverlapBox(position, extents, globalObjectGenerationParameters.objectPrefab.transform.rotation);
                
                // Iloœæ kolizji ma byæ równa 0 poniewa¿, obiekt ma byæ generowany w miejscu gdzie nie ma innych obiektów
                if (colliders.Length == 0)
                {
                    positionFound = true;
                }
            }

            if (positionFound)
            {
                Quaternion rotation = Quaternion.Euler(globalObjectGenerationParameters.objectPrefab.transform.rotation.x, Random.Range(0.0f, 360.0f), globalObjectGenerationParameters.objectPrefab.transform.rotation.z);

                // Tworzenie obiektu
                GameObject newIsland = Instantiate(globalObjectGenerationParameters.objectPrefab, position, rotation, parent);

                var islandDataComponent = newIsland.AddComponent<IslandDataComponent>();
                islandDataComponent.islandData = new IslandData
                {
                    id = IslandIdCounter,
                    type = typeIsland,
                    size = sizeIsland,
                    affiliation = islandsPool.affiliations[Random.Range(0, islandsPool.affiliations.Length)],
                    location = new Location
                    {
                        posX = newIsland.transform.position.x,
                        posY = newIsland.transform.position.y,
                        posZ = newIsland.transform.position.z,
                        rotX = newIsland.transform.rotation.eulerAngles.x,
                        rotY = newIsland.transform.rotation.eulerAngles.y,
                        rotZ = newIsland.transform.rotation.eulerAngles.z
                    },
                    prefabName = globalObjectGenerationParameters.objectPrefab.name
                };
                IslandIdCounter++;
                var islandInteractable = newIsland.AddComponent<Island>();
                islandInteractable.SetPromptShip("ZEJŒÆ NA WYSPÊ");
                newIsland.layer = LayerMask.NameToLayer("Interactable");
            }
            else
            {
                Debug.LogWarning("Nie znaleziono odpowiedniego miejsca dla obiektu po 100 próbach.");
            }
        }
    }

    protected override void OnApplicationQuit()
    {
        Debug.Log("Saving player ship data on global map on Quit.");

        PlayerShipDataComponent[] playerShipDataComponents = FindObjectsOfType<PlayerShipDataComponent>();
        if (playerShipDataComponents.Length == 1)
        {
            PlayerShipDataComponent playerShipDataComponent = playerShipDataComponents[0];
            GameObject playerShip = playerShipDataComponent.gameObject;

            playerShipDataComponent.playerShipData.location.posX = playerShip.transform.position.x;
            playerShipDataComponent.playerShipData.location.posY = playerShip.transform.position.y;
            playerShipDataComponent.playerShipData.location.posZ = playerShip.transform.position.z;
            playerShipDataComponent.playerShipData.location.rotX = playerShip.transform.rotation.eulerAngles.x;
            playerShipDataComponent.playerShipData.location.rotY = playerShip.transform.rotation.eulerAngles.y;
            playerShipDataComponent.playerShipData.location.rotZ = playerShip.transform.rotation.eulerAngles.z;
        }
        else
        {
            Debug.LogError("PlayerShipDataComponent not found or more than one instance.");
        }
        saveManager.SavePlayerShipData();

        base.OnApplicationQuit();
    }
}
