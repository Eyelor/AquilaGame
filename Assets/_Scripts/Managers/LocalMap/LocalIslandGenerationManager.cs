using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LocalIslandGenerationManager : Singleton<LocalIslandGenerationManager>
{
    [Header("Object Generation Parameters")]
    [SerializeField] private List<LocalObjectGenerationParametersSO> objectsToGenerate; // Lista obiekt�w do wygenerowania
    [SerializeField] private Transform objectsParent; // Rodzic obiekt�w do wygenerowania

    [Header("Mob Generation Parameters")]
    [SerializeField] private List<LocalMobGenerationParametersSO> mobsToGenerate; // Lista mob�w do wygenerowania
    [SerializeField] private Transform mobsParent; // Rodzic mob�w do wygenerowania

    [Header("Terrain Settings")]
    [SerializeField] private Terrain terrain; // Terrain, na kt�rym obiekty maj� by� generowane
    [SerializeField] private float heightMargin = 2.5f; // Margines wysoko�ci wok� �redniej/mediany do ustalenia minHeight i maxHeight
    [SerializeField] private int outBorder = 50; // Margines granic zewn�trznych terenu wewn�trzego
    [SerializeField] private int excessLand = 250; // Nadmiarowy teren

    [Header("Path Generation Settings")]
    [SerializeField] private float addHeight = 5; // Warto�� dodawana dla �cie�ki do minimalnej i maksymalnej wysoko�ci terenu
    [SerializeField] private int howManyToRemove = 5; // Ilo�� element�w do usuni�cia w przypadku omijania przeszk�d
    [SerializeField] private TerrainLayer pathLayer; // Warstwa tekstury �cie�ki
    [SerializeField] private TerrainLayer mainLayer; // Warstwa tekstury g��wnej mapy
    [SerializeField] private int pathWidth = 3; // Szeroko�� �cie�ki
    [SerializeField] private float pathHeightOutOfBorder = 12f; // dodatkowa d�ugo�� �cie�ki przed i po terenie grywalnym 
    [SerializeField] private Transform playerTransform; // Do ustawienia startowej pozycji gracza na pocz�tku �cie�ki
    [SerializeField] private Transform signpostTransform; // Do ustawienia startowej pozycji kierunkowskazu powrotu

    [Header("Branch Generation Parameters")]
    [SerializeField] private BranchGenerationParametersListSO branchGenerationParametersList; // Lista parametr�w odn�g do wygenerowania
    private List<BranchGenerationParameters> branchesToGenerate; // Lista odn�g do wygenerowania

    private float minHeight = 0.0f; // Minimalna wysoko��, na kt�rej mo�na generowa� obiekty
    private float maxHeight = 0.0f; // Maksymalna wysoko��, na kt�rej mo�na generowa� obiekty

    private List<Vector3> pathPoints = new List<Vector3>(); // Lista punkt�w �cie�ki
    private List<Vector3> branchPoints = new List<Vector3>(); // Lista punkt�w odnogi �cie�ki
    private float[,,] originalAlphaMaps; // Oryginalne warstwy alfa terenu
    private bool applicationQuitting = false; // Flaga sprawdzaj�ca czy aplikacja jest wy��czana czy zmieniana jest scena
    public bool exitToMainMenu = false;

    // Wyb�r obiekt�w statycznych do wygenerowania dla ka�dego typu wyspy
    [SerializeField] private PrefabRegistrySO localMapPrefabRegistry; // Rejestr prefab�w

    [SerializeField] private ObjectsPoolSO objectsPoolSO; // Pula obiekt�w do wygenerowania
    private (string[] allStaticObjects, string[] sandObjects, string[] grassObjects, string[] muddObjects, string[] interactableObjects) objectsPool;

    // Dane do zapisu do plik�w JSON
    private int objectId = 1; // Identyfikator wszystkich wygenerowanych obiekt�w na scenie lokalnej

    private PathDataComponent pathDataComponent;
    public CurrentIslandDataComponent currentIslandDataComponent;
    private PlayerDataComponent playerDataComponent;

    private bool isPathGenerated = false;
    private bool isObjectsGenerated = false;

    private SaveManager saveManager;
    private StatisticsManager statisticsManager;
    private PlayerStatistics playerStatistics;

    protected override void Awake()
    {
        base.Awake();

        saveManager = SaveManager.Instance;
        statisticsManager = StatisticsManager.Instance;
        playerStatistics = statisticsManager.LoadStatistics();

        branchesToGenerate = branchGenerationParametersList.GetBranchGenerationParametersList();

        objectsPool = objectsPoolSO.GetObjectsPool();

        SaveOriginalTerrainAlfaLayers();
 
        // Oblicz zakres wysoko�ci
        CalculateHeightRange();

        LoadCurrentIsland();
        LoadPath();
        LoadStaticObjects();

        LoadPlayer();

        // INTERACTABLE OBJECTS i MOBS nie s� zapisywane w jsonie tylko s� zawsze generowane na nowo na mapach
        // zawieraj� interaktywne elementy i loop kt�ry mo�na z nich zebra�
        LoadInteractableObjects();
        LoadMobs();
    }

    protected override void OnApplicationQuit()
    {
        applicationQuitting = true; // Ustawienie flagi, �e aplikacja jest zamykana
        if (isPathGenerated)
        {
            saveManager.SavePathData(currentIslandDataComponent.currentIslandData.islandId);
            Debug.Log("Save path to JSON file");
        }
        if (isObjectsGenerated)
        {
            saveManager.SaveObjectDataList(currentIslandDataComponent.currentIslandData.islandId);
            Debug.Log("Save objects date to JSON file");
        }

        PlayerDataComponent[] playerDataComponents = FindObjectsOfType<PlayerDataComponent>();
        if (playerDataComponents.Length == 1)
        {
            GameObject player = playerDataComponents[0].gameObject;
            playerDataComponent.playerData.islandId = currentIslandDataComponent.currentIslandData.islandId;
            playerDataComponent.playerData.location.posX = player.transform.position.x;
            playerDataComponent.playerData.location.posY = player.transform.position.y;
            playerDataComponent.playerData.location.posZ = player.transform.position.z;
            playerDataComponent.playerData.location.rotX = player.transform.rotation.eulerAngles.x;
            playerDataComponent.playerData.location.rotY = player.transform.rotation.eulerAngles.y;
            playerDataComponent.playerData.location.rotZ = player.transform.rotation.eulerAngles.z;
            playerDataComponent.playerData.isPlayerInside = true;
        }
        else
        {
            Debug.LogError("PlayerDataComponent empty or more than one instance.");
        }
        saveManager.SavePlayerData();

        saveManager.SavePlayerStatistics();
        saveManager.SavePlayerAchievements();

        // Przywracanie oryginalnych warstw alfa terenu, je�li obiekt jest dezaktywowany
        if (originalAlphaMaps != null)
        {
            TerrainData terrainData = terrain.terrainData;
            terrainData.SetAlphamaps(0, 0, originalAlphaMaps);
        }
        
        base.OnApplicationQuit();
    }

    private void OnDisable()
    {
        if (!applicationQuitting) // Sprawdzenie, czy aplikacja nie jest zamykana
        {
            if (currentIslandDataComponent != null)
            {
                if (isPathGenerated)
                {
                    saveManager.SavePathData(currentIslandDataComponent.currentIslandData.islandId);
                    Debug.Log("Save path to JSON file");
                }
                if (isObjectsGenerated)
                {
                    saveManager.SaveObjectDataList(currentIslandDataComponent.currentIslandData.islandId);
                    Debug.Log("Save objects date to JSON file");
                }

                if (!exitToMainMenu)
                {
                    playerDataComponent.playerData.isPlayerInside = false;
                    saveManager.SavePlayerData();

                    currentIslandDataComponent.currentIslandData.isPlayerInside = false;
                    saveManager.SaveCurrentIslandData();
                    saveManager.SavePlayerStatistics();
                    saveManager.SavePlayerAchievements();

                } else {
                    PlayerDataComponent[] playerDataComponents = FindObjectsOfType<PlayerDataComponent>();
                    if (playerDataComponents.Length == 1)
                    {
                        GameObject player = playerDataComponents[0].gameObject;
                        playerDataComponent.playerData.islandId = currentIslandDataComponent.currentIslandData.islandId;
                        playerDataComponent.playerData.location.posX = player.transform.position.x;
                        playerDataComponent.playerData.location.posY = player.transform.position.y;
                        playerDataComponent.playerData.location.posZ = player.transform.position.z;
                        playerDataComponent.playerData.location.rotX = player.transform.rotation.eulerAngles.x;
                        playerDataComponent.playerData.location.rotY = player.transform.rotation.eulerAngles.y;
                        playerDataComponent.playerData.location.rotZ = player.transform.rotation.eulerAngles.z;
                        playerDataComponent.playerData.isPlayerInside = true;
                    }
                    else
                    {
                        Debug.LogError("PlayerDataComponent empty or more than one instance.");
                    }
                    saveManager.SavePlayerData();
                    saveManager.SavePlayerStatistics();
                    saveManager.SavePlayerAchievements();
                    exitToMainMenu = false;
                }        
            }
            else
            {
                Debug.LogWarning("currentIslandDataComponent is null in OnDestroy.");
            }

            // Pprzywracanie oryginalnych warstw alfa terenu, je�li obiekt jest dezaktywowany
            if (originalAlphaMaps != null)
            {
                TerrainData terrainData = terrain.terrainData;
                terrainData.SetAlphamaps(0, 0, originalAlphaMaps);
            }
        }
    }

    #region Loading Objects
    private void LoadCurrentIsland()
    {
        //////////////// CURRENT ISLAND ////////////////
        CurrentIslandData currentIslandData = saveManager.LoadCurrentIslandData();

        // Sprawd�, czy currentIslandDataComponent jest poprawnie przypisany
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
            else
            {
                currentIslandDataComponent.currentIslandData = new CurrentIslandData
                {
                    islandId = 0,
                    islandType = "default",
                    islandSize = "default",
                    islandAffiliation = "default",
                    isPlayerInside = true
                };
            }
        }
    }

    private void LoadPath()
    {
        //////////////// PATH ////////////////
        // Wczytaj dane �cie�ki i odn�g
        PathData pathData = saveManager.LoadPathData(currentIslandDataComponent.currentIslandData.islandId);

        // Sprawd�, czy pathDataComponent jest poprawnie przypisany
        if (pathDataComponent == null)
        {
            pathDataComponent = gameObject.AddComponent<PathDataComponent>();
        }
        if (pathData == null) // Je�li nie wczytano danych �cie�ki, generujemy nowe
        {
            isPathGenerated = true;
            // Generowanie �cie�ki i odn�g
            GenerateMainPath();
        }
        else // Je�li dane �cie�ki s� dost�pne, wczytaj je
        {
            pathDataComponent.pathData.islandId = currentIslandDataComponent.currentIslandData.islandId;
            // Wczytaj g��wne punkty �cie�ki
            List<Vector3> pathPointsJson = new List<Vector3>();
            foreach (var pointData in pathData.points)
            {
                Vector3 point = new Vector3(pointData.posX, pointData.posY, pointData.posZ);
                pathPointsJson.Add(point);
                pathDataComponent.AddPoint(point);
            }
            // Wizualizacja �cie�ki g��wnej
            VisualizePath(pathPointsJson);

            // Wczytaj odn�a �cie�ki
            foreach (var branchData in pathData.branches)
            {
                var branchPoints = new List<Vector3>();
                foreach (var point in branchData.points)
                {
                    branchPoints.Add(new Vector3(point.posX, point.posY, point.posZ));
                }
                // Dodaj odn�g do komponentu, ale upewnij si�, �e odpowiednio konwertujesz
                pathDataComponent.AddBranch(new BranchData { points = branchPoints.Select(v => new PointData { posX = v.x, posY = v.y, posZ = v.z }).ToList() });

                // Wizualizacja odn�a
                VisualizePath(branchPoints);
            }

            // Ustawienie pozycji pocz�tkowej gracza i kierunkowskazu powrotnego na pocz�tku �cie�ki
            if (pathPointsJson.Count > 0)
            {
                Vector3 startPosition = pathPointsJson[((int)pathHeightOutOfBorder) / 4 + 1];
                playerTransform.position = new Vector3(startPosition.x, startPosition.y, startPosition.z + 1);
                signpostTransform.position = new Vector3(startPosition.x + 2, startPosition.y + 1, startPosition.z);

                // Debugowanie - wypisywanie nowej pozycji startowej gracza
                Debug.Log("Nowa pozycja startowa gracza: " + playerTransform.position);
            }

            Debug.Log($"Path and branch data for local map {currentIslandDataComponent.currentIslandData.islandId} loaded.");
        }
    }

    private void LoadStaticObjects()
    {
        //////////////// STATIC OBJECTS ////////////////
        ObjectDataList objectDataList = saveManager.LoadObjectDataList(currentIslandDataComponent.currentIslandData.islandId);

        if (objectDataList == null) // Je�li nie wczytano danych obiekt�w, generujemy nowe
        {
            isObjectsGenerated = true;
            if (currentIslandDataComponent.currentIslandData.islandId == 0)
            {
                foreach (var objParams in objectsToGenerate)
                {
                    /*if (System.Array.Exists(objectsPool.allStaticObjects, element => element == objParams.GetLocalObjectGenerationParameters().objectPrefab.name))
                    {
                        GenerateObjects(objParams);
                    }*/
                    if (System.Array.Exists(objectsPool.sandObjects, element => element == objParams.GetLocalObjectGenerationParameters().objectPrefab.name))
                    {
                        GenerateObjects(objParams);
                    }
                }
            }
            else
            {
                if (currentIslandDataComponent.currentIslandData.islandType == "sandy")
                {
                    foreach (var objParams in objectsToGenerate)
                    {
                        if (System.Array.Exists(objectsPool.sandObjects, element => element == objParams.GetLocalObjectGenerationParameters().objectPrefab.name))
                        {
                            GenerateObjects(objParams);
                        }
                    }
                }
                else if (currentIslandDataComponent.currentIslandData.islandType == "muddy")
                {
                    foreach (var objParams in objectsToGenerate)
                    {
                        if (System.Array.Exists(objectsPool.muddObjects, element => element == objParams.GetLocalObjectGenerationParameters().objectPrefab.name))
                        {
                            GenerateObjects(objParams);
                        }
                    }
                }
                else if (currentIslandDataComponent.currentIslandData.islandType == "grassy")
                {
                    foreach (var objParams in objectsToGenerate)
                    {
                        if (System.Array.Exists(objectsPool.grassObjects, element => element == objParams.GetLocalObjectGenerationParameters().objectPrefab.name))
                        {
                            GenerateObjects(objParams);
                        }
                    }
                }
            }
        }
        else // Obiekty wczytane z pliku Json
        {
            foreach (var data in objectDataList.objectsDataList)
            {
                // Convert rotation from Vector3 to Quaternion
                Quaternion rotation = Quaternion.Euler(data.objectLocation.rotX, data.objectLocation.rotY, data.objectLocation.rotZ);

                // Load the prefab from Resources
                GameObject prefab = localMapPrefabRegistry.GetPrefabByName(data.name);
                if (prefab != null)
                {
                    // Instantiate the prefab
                    GameObject newObject = Instantiate(prefab, new Vector3(data.objectLocation.posX, data.objectLocation.posY, data.objectLocation.posZ), rotation, objectsParent);

                    // Add ObjectDataComponent and set its data
                    var dataComponent = newObject.AddComponent<ObjectDataComponent>();
                    dataComponent.objectData = data;

                    // Ensure the object has a Rigidbody and set it as kinematic
                    Rigidbody rb = newObject.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        rb.isKinematic = true;
                    }
                    else
                    {
                        Debug.LogWarning($"Prefab {data.name} does not have a Rigidbody component.");
                    }
                }
                else
                {
                    Debug.LogWarning($"Prefab with name {data.name} not found in Resources folder.");
                }
            }
            Debug.Log($"Objects data on local map {currentIslandDataComponent.currentIslandData.islandId} was loaded.");
        }
    }

    private void LoadInteractableObjects()
    {
        //////////////// INTERACTABLE OBJECTS ////////////////
        foreach (var objParams in objectsToGenerate)
        {
            if (System.Array.Exists(objectsPool.interactableObjects, element => element == objParams.GetLocalObjectGenerationParameters().objectPrefab.name))
            {
                GenerateObjects(objParams);
            }
        }
    }

    private void LoadMobs()
    {
        //////////////// MOBS ////////////////
        foreach (var mobParams in mobsToGenerate)
        {
            if (currentIslandDataComponent.currentIslandData.islandAffiliation == "default")
            {
                GenerateMobs(mobParams);
            }
            else if (mobParams.GetLocalMobGenerationParameters().affiliation.ToString() == currentIslandDataComponent.currentIslandData.islandAffiliation)
            {
                GenerateMobs(mobParams);
            }
        }
    }

    private void LoadPlayer()
    {
        //////////////// PLAYER ////////////////
        PlayerData playerData = saveManager.LoadPlayerData();
        if (playerData == null) // Je�eli nie istnieje �aden plik JSON
        {
            Debug.Log("Getting data from player on local map.");

            PlayerController[] playerControllers = FindObjectsOfType<PlayerController>();
            if (playerControllers.Length == 1)
            {
                GameObject player = playerControllers[0].gameObject;
                playerDataComponent = player.AddComponent<PlayerDataComponent>();
                playerDataComponent.playerData = new PlayerData
                {
                    islandId = currentIslandDataComponent.currentIslandData.islandId,
                    location = new Location
                    {
                        posX = player.transform.position.x,
                        posY = player.transform.position.y,
                        posZ = player.transform.position.z,
                        rotX = player.transform.rotation.eulerAngles.x,
                        rotY = player.transform.rotation.eulerAngles.y,
                        rotZ = player.transform.rotation.eulerAngles.z
                    },
                    isPlayerInside = true
                };
                player.GetComponent<PlayerController>().onNewIsland = isObjectsGenerated;
            }
            else
            {
                Debug.LogError("PlayerController empty or more than one instance.");
            }

            Debug.Log("Saving player data on local map.");
            saveManager.SavePlayerData();
        }
        else // Je�eli istnieje plik JSON z lokalizacj� gracza Na wyspie lokalnej 
        {
            PlayerController[] playerControllers = FindObjectsOfType<PlayerController>();
            if (playerControllers.Length == 1)
            {
                // Je�eli wyj�cie z lokalnej mapy nast�pi�o poprzez wyj�cie z gry - wczytaj pozycje z JSON
                if (playerData.isPlayerInside)
                {
                    GameObject player = playerControllers[0].gameObject;
                    playerDataComponent = player.AddComponent<PlayerDataComponent>();
                    playerDataComponent.playerData = new PlayerData
                    {
                        islandId = playerData.islandId,
                        location = new Location
                        {
                            posX = playerData.location.posX,
                            posY = playerData.location.posY,
                            posZ = playerData.location.posZ,
                            rotX = playerData.location.rotX,
                            rotY = playerData.location.rotY,
                            rotZ = playerData.location.rotZ
                        },
                        isPlayerInside = playerData.isPlayerInside
                    };
                    playerTransform.position = new Vector3(playerData.location.posX, playerData.location.posY, playerData.location.posZ);
                    playerTransform.rotation = Quaternion.Euler(playerData.location.rotX, playerData.location.rotY, playerData.location.rotZ);
                    player.GetComponent<PlayerController>().onNewIsland = isObjectsGenerated;
                }
                else // Je�eli wyj�cie z lokalnej mapy nast�pi�o poprzez przej�cie na scene globaln� - zamie� JSON na pocz�tkow� pozycje gracza na pocz�tku �ciezki
                {
                    GameObject player = playerControllers[0].gameObject;
                    playerDataComponent = player.AddComponent<PlayerDataComponent>();
                    playerDataComponent.playerData = new PlayerData
                    {
                        islandId = currentIslandDataComponent.currentIslandData.islandId,
                        location = new Location
                        {
                            posX = player.transform.position.x,
                            posY = player.transform.position.y,
                            posZ = player.transform.position.z,
                            rotX = player.transform.rotation.eulerAngles.x,
                            rotY = player.transform.rotation.eulerAngles.y,
                            rotZ = player.transform.rotation.eulerAngles.z
                        },
                        isPlayerInside = true
                    };
                    player.GetComponent<PlayerController>().onNewIsland = isObjectsGenerated;
                    saveManager.SavePlayerData();
                }
            }
            else
            {
                Debug.LogError("PlayerController empty or more than one instance.");
            }

            Debug.Log("Player data on local map was loaded.");
        }
    }
    #endregion

    #region Generating Objects
    private void GenerateObjects(LocalObjectGenerationParametersSO objParams)
    {
        var localObjectGenerationParameters = objParams.GetLocalObjectGenerationParameters();

        // Sprawdzenie czy wszystkie wymagane zmienne s� ustawione
        if (localObjectGenerationParameters.objectPrefab == null || terrain == null)
        {
            Debug.LogError("Object Prefab lub Terrain nie zosta�y ustawione w inspektorze.");
            return;
        }

        // Pobranie rozmiaru terenu
        TerrainData terrainData = terrain.terrainData;
        Vector3 terrainSize = terrainData.size;

        // Ustalanie granic zewn�trznych terenu, w kt�rych obiekty mog� by� generowane
        float minX = excessLand + outBorder;
        float maxX = terrainSize.x - excessLand - outBorder;
        float minZ = excessLand + outBorder;
        float maxZ = terrainSize.z - excessLand - outBorder;
        // Debug.Log(minX + ", " + maxX + ", " + minZ + ", " + maxZ);

        // Losowanie liczby obiekt�w do wygenerowania
        int numberOfObjects = Random.Range(localObjectGenerationParameters.minNumberOfObjects, localObjectGenerationParameters.maxNumberOfObjects + 1);
        Debug.Log("Ilo�� wygenerowanych obiekt�w " + localObjectGenerationParameters.objectPrefab.name + ": " + numberOfObjects);

        for (int i = 0; i < numberOfObjects; i++)
        {
            Vector3 position = Vector3.zero;
            bool positionFound = false;
            int attempts = 0;

            while (!positionFound && attempts < 100)
            {
                attempts++;
                // Losowe pozycjonowanie obiektu
                float x = Random.Range(minX, maxX);
                float z = Random.Range(minZ, maxZ);
                float y;
                if (localObjectGenerationParameters.isInteractable)
                {
                    y = terrain.SampleHeight(new Vector3(x, 0, z)) + localObjectGenerationParameters.objectPrefab.transform.position.y;
                    // Debug.Log("INTERACTABLE " + i + " " + terrain.SampleHeight(new Vector3(x, 0, z)) + " " + objParams.objectPrefab.transform.position.y);
                }
                else
                {
                    y = terrain.SampleHeight(new Vector3(x, 0, z)) + localObjectGenerationParameters.objectPrefab.transform.position.y + (localObjectGenerationParameters.objectPrefab.GetComponent<Renderer>().bounds.size.y / 2f);
                    // Debug.Log("NON-INTERACTABLE " + objectId + " " + terrain.SampleHeight(new Vector3(x, 0, z)) + " " + localObjectGenerationParameters.objectPrefab.GetComponent<Renderer>().bounds.size.y / 2f);
                }
                position = new Vector3(x, y, z);

                // Sprawdzenie czy punkt odpowiada danemu zakresowi wysoko�ci.
                if (terrain.SampleHeight(new Vector3(x, 0, z)) < minHeight || terrain.SampleHeight(new Vector3(x, 0, z)) > maxHeight)
                {
                    // Debug.Log("Wysoko�� terenu poza dopuszczalnym zakresem na pozycji: " + position);
                    continue;
                }

                // Sprawdzenie nachylenia terenu w danym punkcie
                float slope = CalculateSlope(x, z);
                if (slope > localObjectGenerationParameters.maxSlope)
                {
                    // Debug.Log("Nachylenie terenu zbyt du�e: " + slope + " na pozycji: " + position);
                    continue;
                }

                // Sprawdzenie, czy punkt nie znajduje si� na �cie�ce lub w jej pobli�u
                if (IsPositionOnPath(x, z))
                {
                    // Debug.Log("Obiekt na �cie�ce!");
                    continue;
                }

                // Sprawdzanie kolizji w tym miejscu
                BoxCollider boxCollider = localObjectGenerationParameters.objectPrefab.GetComponent<BoxCollider>();
                if (boxCollider == null)
                {
                    Debug.LogError("Nie znaleziono komponenetu Box Collider w Object prefab");
                    return;
                }

                // Okre�lenie wolnej przestrzeni jaka ma by� wok� obiektu (cz�stotliwo�� wyst�powania)
                Vector3 extents = boxCollider.bounds.extents + new Vector3(localObjectGenerationParameters.objectSpacing, localObjectGenerationParameters.objectSpacing, localObjectGenerationParameters.objectSpacing) / 2;
                Collider[] colliders = Physics.OverlapBox(position, extents, localObjectGenerationParameters.objectPrefab.transform.rotation);

                // Ilo�� kolizji ma by� r�wna 1 poniewa�
                // Musi kolidowa� z terenem, ewentualnie z wy�ej po�o�onym obiektem
                if (colliders.Length == 1)
                {
                    positionFound = true;
                }
                else
                {
                    // Debug.Log("Nadmiarowa kolizja znaleziona, pr�ba: " + attempts + " pozycja: " + position);
                }
            }

            if (positionFound)
            {
                // Tworzenie obiektu
                GameObject newObject = Instantiate(localObjectGenerationParameters.objectPrefab, position, localObjectGenerationParameters.objectPrefab.transform.rotation, objectsParent);
                // Debug.Log("Obiekt wygenerowany na pozycji: " + position + " z rotacj�: " + objParams.objectPrefab.transform.rotation.eulerAngles);
                
                if (!localObjectGenerationParameters.isInteractable)
                {
                    var objectDataComponent = newObject.AddComponent<ObjectDataComponent>();
                    objectDataComponent.objectData = new ObjectData
                    {
                        id = objectId,
                        name = localObjectGenerationParameters.objectPrefab.name,
                        islandId = currentIslandDataComponent.currentIslandData.islandId,
                    };

                    objectId += 1;
                }
            }
            else
            {
                Debug.LogWarning("Nie znaleziono odpowiedniego miejsca dla obiektu po 100 pr�bach.");
            }
        }
    }

    private void GenerateMobs(LocalMobGenerationParametersSO mobParams)
    {
        var localMobGenerationParameters = mobParams.GetLocalMobGenerationParameters();

        // Sprawdzenie czy wszystkie wymagane zmienne s� ustawione
        if (localMobGenerationParameters.mobPrefab == null || terrain == null)
        {
            Debug.LogError("Mob Prefab lub Terrain nie zosta�y ustawione w inspektorze.");
            return;
        }

        // Pobranie rozmiaru terenu
        TerrainData terrainData = terrain.terrainData;
        Vector3 terrainSize = terrainData.size;

        // Ustalanie granic zewn�trznych terenu, w kt�rych moby mog� by� generowane
        float minX = excessLand + outBorder;
        float maxX = terrainSize.x - excessLand - outBorder;
        float minZ = excessLand + outBorder;
        float maxZ = terrainSize.z - excessLand - outBorder;
        // Debug.Log(minX + ", " + maxX + ", " + minZ + ", " + maxZ);

        // Pobranie wszystkich obiekt�w szybkiej podr�y
        GameObject[] fastTravelPoints = GameObject.FindGameObjectsWithTag("fastTravel");

        // Losowanie liczby obiekt�w do wygenerowania
        int numberOfMobs = Random.Range(localMobGenerationParameters.minNumberOfMobs, localMobGenerationParameters.maxNumberOfMobs + 1);
        Debug.Log("Ilo�� wygenerowanych mob�w: " + numberOfMobs);

        for (int i = 0; i < numberOfMobs; i++)
        {
            Vector3 position = Vector3.zero;
            bool positionFound = false;
            int attempts = 0;

            while (!positionFound && attempts < 100)
            {
                attempts++;
                // Losowe pozycjonowanie obiektu
                float x = Random.Range(minX, maxX);
                float z = Random.Range(minZ, maxZ);
                float y = terrain.SampleHeight(new Vector3(x, 0, z)) + localMobGenerationParameters.mobPrefab.transform.position.y;
                position = new Vector3(x, y, z);

                // Sprawdzenie czy punkt odpowiada danemu zakresowi wysoko�ci.
                if (terrain.SampleHeight(new Vector3(x, 0, z)) < minHeight || terrain.SampleHeight(new Vector3(x, 0, z)) > maxHeight)
                {
                    // Debug.Log("Wysoko�� terenu poza dopuszczalnym zakresem na pozycji: " + position);
                    continue;
                }

                // Sprawdzenie nachylenia terenu w danym punkcie
                float slope = CalculateSlope(x, z);
                if (slope > localMobGenerationParameters.maxSlope)
                {
                    // Debug.Log("Nachylenie terenu zbyt du�e: " + slope + " na pozycji: " + position);
                    continue;
                }

                // Sprawdzenie dystansu od gracza
                float distanceToPlayer = Vector3.Distance(position, playerTransform.position);
                if (distanceToPlayer < 50f)
                {
                    // Debug.Log("Pozycja zbyt blisko gracza, dystans: " + distanceToPlayer + " na pozycji: " + position);
                    continue;
                }

                // Sprawdzenie dystansu od obiekt�w szybkiej podr�y
                bool tooCloseToFastTravel = false;
                foreach (var fastTravelPoint in fastTravelPoints)
                {
                    if (Vector3.Distance(position, fastTravelPoint.transform.position) < 25f)
                    {
                        tooCloseToFastTravel = true;
                        break;
                    }
                }
                if (tooCloseToFastTravel)
                {
                    continue;
                }

                // Sprawdzanie kolizji w tym miejscu
                CharacterController characterController = localMobGenerationParameters.mobPrefab.GetComponent<CharacterController>();
                if (characterController == null)
                {
                    Debug.LogError("Nie znaleziono komponenetu CharacterController w Mob prefab");
                    return;
                }
                // Okre�lenie wolnej przestrzeni jaka ma by� wok� obiektu (cz�stotliwo�� wyst�powania)
                Vector3 extents = characterController.bounds.extents + new Vector3(localMobGenerationParameters.mobSpacing, localMobGenerationParameters.mobSpacing, localMobGenerationParameters.mobSpacing) / 2;
                Collider[] colliders = Physics.OverlapBox(position, extents, localMobGenerationParameters.mobPrefab.transform.rotation);
                if (colliders.Length == 1)
                {
                    positionFound = true;
                }
                else
                {
                    // Debug.LogWarning("Nadmiarowa kolizja znaleziona, pr�ba: " + attempts + " pozycja: " + position);
                }
            }

            if (positionFound)
            {
                // Generowanie losowej rotacji wok� osi Y
                // Quaternion randomYRotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
                Quaternion rotation = Quaternion.Euler(0, 0, 0);

                // Tworzenie moba z losow� rotacj� w osi Y
                GameObject newObject = Instantiate(localMobGenerationParameters.mobPrefab, position, rotation, mobsParent);
                var mobStatisticComponent = newObject.AddComponent<MobStatisticComponent>();
                int mobHealth = Random.Range(70 + playerStatistics.level * 5, 71 + playerStatistics.level * 25);
                mobStatisticComponent.mobStatistics = new MobStatistics
                {
                    health = mobHealth,
                    maxHealth = mobHealth,
                    healthRegen = Random.Range(1 + playerStatistics.level / 4, playerStatistics.level / 4 + 4),
                    attack = Random.Range(5 + playerStatistics.level / 2, playerStatistics.level  + 13),
                    luck = Random.Range(1 + playerStatistics.level / 3, playerStatistics.level / 3 + 4)
                };
                // Debug.Log("Obiekt wygenerowany na pozycji: " + position + " z rotacj�: " + randomYRotation.eulerAngles);
            }
            else
            {
                Debug.LogWarning("Nie znaleziono odpowiedniego miejsca dla obiektu po 100 pr�bach.");
            }
        }
    }
    #endregion

    #region Tools
    private void SaveOriginalTerrainAlfaLayers()
    {
        // Zapisz oryginalne warstwy alfa terenu
        TerrainData terrainData = terrain.terrainData;
        originalAlphaMaps = terrainData.GetAlphamaps(0, 0, terrainData.alphamapWidth, terrainData.alphamapHeight);
    }

    private void CalculateHeightRange()
    {
        TerrainData terrainData = terrain.terrainData;

        // Zbieranie danych wysoko�ci terenu
        List<float> heights = new List<float>();
        for (int x = excessLand; x < terrainData.heightmapResolution - excessLand; x++)
        {
            for (int z = excessLand; z < terrainData.heightmapResolution - excessLand; z++)
            {
                float height = terrainData.GetHeight(x, z);
                heights.Add(height);
            }
        }

        // Obliczenie mediany wysoko�ci
        heights.Sort();
        float medianHeight = heights[heights.Count / 2];

        // Wyznaczenie minHeight i maxHeight na podstawie mediany i marginesu
        minHeight = medianHeight - heightMargin;
        maxHeight = medianHeight + heightMargin;

        Debug.Log("Mediana wysoko�ci terenu: " + medianHeight);
        Debug.Log("minHeight: " + (medianHeight - heightMargin) + ", maxHeight: " + (medianHeight + heightMargin));
    }

    // Funkcja obliczaj�ca nachylenie terenu w punkcie
    private float CalculateSlope(float x, float z)
    {
        // Funkcja ta oblicza nachylenie terenu w danym punkcie.
        // Pobiera pr�bki wysoko�ci terenu wok� punktu (w kierunkach X i Z) i
        // oblicza r�nice wysoko�ci, aby okre�li� nachylenie.
        float delta = 0.1f;
        float heightCenter = terrain.SampleHeight(new Vector3(x, 0, z));
        float heightLeft = terrain.SampleHeight(new Vector3(x - delta, 0, z));
        float heightRight = terrain.SampleHeight(new Vector3(x + delta, 0, z));
        float heightForward = terrain.SampleHeight(new Vector3(x, 0, z + delta));
        float heightBack = terrain.SampleHeight(new Vector3(x, 0, z - delta));

        float dx = (heightRight - heightLeft) / (2 * delta);
        float dz = (heightForward - heightBack) / (2 * delta);

        float slope = Mathf.Atan(Mathf.Sqrt(dx * dx + dz * dz)) * Mathf.Rad2Deg;
        return slope;
    }

    // Funkcja wykorzystuj�ca klas� PathGeneration do generowania punkt�w �cie�ki 
    private void GenerateMainPath()
    {
        // Instancja PathGeneration
        PathGeneration pathGenerator = new PathGeneration(terrain, minHeight, maxHeight, addHeight, howManyToRemove, excessLand, pathHeightOutOfBorder);

        // Generowanie �cie�ki
        pathPoints = pathGenerator.GeneratePath();
        VisualizePath(pathPoints);

        pathDataComponent.pathData.islandId = currentIslandDataComponent.currentIslandData.islandId;

        // Dodaj punkty g��wnej �cie�ki do komponentu PathData
        foreach (var point in pathPoints)
        {
            pathDataComponent.AddPoint(point);
        }

        // Generowanie odn�g �cie�ki
        foreach (var branchParams in branchesToGenerate)
        {
            branchPoints = pathGenerator.GenerateBranchPath(pathPoints, branchParams.LowerStartPoint, branchParams.HigherStartPoint, branchParams.Direction);
            VisualizePath(branchPoints);

            // Utw�rz now� odnog� jako BranchData
            BranchData branch = new BranchData
            {
                points = new List<PointData>()
            };

            foreach (var point in branchPoints)
            {
                branch.points.Add(new PointData { posX = point.x, posY = point.y, posZ = point.z });
            }

            // Dodaj odnog� do g��wnej �cie�ki
            pathDataComponent.AddBranch(branch);
        }
 
        // Ustawienie pozycji pocz�tkowej gracza na czwartym punkcie �cie�ki gdy� pierwsze 3 s� przed borderem (z przesuni�ciem +1 do osi Z)
        if (pathPoints.Count > 0)
        {
            Vector3 startPosition = pathPoints[((int)pathHeightOutOfBorder)/4 + 1];
            playerTransform.position = new Vector3(startPosition.x, startPosition.y, startPosition.z + 1);
            signpostTransform.position = new Vector3(startPosition.x + 2, startPosition.y + 1, startPosition.z);

            // Debugowanie - wypisywanie nowej pozycji startowej gracza
            Debug.Log("Nowa pozycja startowa gracza: " + playerTransform.position);
        }
        else
        {
            Debug.LogWarning("Nie wygenerowano �adnych punkt�w �cie�ki.");
        }
    }

    private void VisualizePath(List<Vector3> pathPoints)
    {
        // Sprawdzenie, czy jest wystarczaj�ca ilo�� punkt�w �cie�ki do wizualizacji
        if (pathPoints.Count < 2)
        {
            Debug.LogWarning("Brak punkt�w �cie�ki do wizualizacji.");
            return;
        }

        // Zapisz oryginalne warstwy alfa terenu
        TerrainData terrainData = terrain.terrainData;

        // Inicjalizacja indeksu warstwy tekstury �cie�ki
        int pathLayerIndex = -1;
        int mainLayerIndex = -1;

        // Szukanie indeksu warstwy tekstury, kt�ra odpowiada �cie�ce
        for (int i = 0; i < terrainData.terrainLayers.Length; i++)
        {
            if (terrainData.terrainLayers[i] == pathLayer)
            {
                pathLayerIndex = i;
            }
            else if (terrainData.terrainLayers[i] == mainLayer)
            {
                mainLayerIndex = i;
            }
        }

        // Je�li warstwa �cie�ki nie zosta�a znaleziona, wy�wietl b��d i zako�cz funkcj�
        if (pathLayerIndex == -1 || mainLayerIndex == -1)
        {
            Debug.LogError("Nie znaleziono warstwy �cie�ki lub warstwy g��wnej na terenie.");
            return;
        }

        // Pobranie mapy alfa terenu, kt�ra okre�la mieszank� tekstur na powierzchni
        float[,,] alphaMaps = terrainData.GetAlphamaps(0, 0, terrainData.alphamapWidth, terrainData.alphamapHeight);

        // Iteracja przez punkty �cie�ki (u�ywaj�c Catmull-Rom spline)
        for (int i = 0; i < pathPoints.Count - 1; i++)
        {
            Vector3 p0 = pathPoints[Mathf.Max(i - 1, 0)];
            Vector3 p1 = pathPoints[i];
            Vector3 p2 = pathPoints[i + 1];
            Vector3 p3 = pathPoints[Mathf.Min(i + 2, pathPoints.Count - 1)];

            // Obliczenie odleg�o�ci mi�dzy punktami dla dynamicznego kroku
            float segmentLength = Vector3.Distance(p1, p2);
            int steps = Mathf.RoundToInt(segmentLength * 10); // Przyk�adowa warto�� kroku

            // Interpolacja z dynamicznym krokiem
            for (int j = 0; j < steps; j++)
            {
                float t = (float)j / steps;
                Vector3 point = GetCatmullRomPosition(t, p0, p1, p2, p3);

                // Przekszta�cenie wsp�rz�dnych punktu �cie�ki na wsp�rz�dne mapy alfa
                int alphaMapX = Mathf.RoundToInt((point.x / terrainData.size.x) * terrainData.alphamapWidth);
                int alphaMapZ = Mathf.RoundToInt((point.z / terrainData.size.z) * terrainData.alphamapHeight);

                // Malowanie punktu na mapie alfa
                BlurPaintAlphaMap(alphaMaps, alphaMapX, alphaMapZ, pathLayerIndex, mainLayerIndex);
            }
        }

        // Zastosowanie zmodyfikowanej mapy alfa do terenu
        terrainData.SetAlphamaps(0, 0, alphaMaps);
    }

    // Funkcja pomocnicza do interpolacji punkt�w �cie�ki u�ywaj�c Catmull-Rom spline
    private Vector3 GetCatmullRomPosition(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        return 0.5f * (
            (-p0 + 3f * p1 - 3f * p2 + p3) * (t * t * t)
            + (2f * p0 - 5f * p1 + 4f * p2 - p3) * (t * t)
            + (-p0 + p2) * t
            + 2f * p1
        );
    }

    // Funkcja do malowania punktu na mapie alfa z rozmyciem
    private void BlurPaintAlphaMap(float[,,] alphaMaps, int alphaMapX, int alphaMapZ, int pathLayerIndex, int mainLayerIndex)
    {
        TerrainData terrainData = terrain.terrainData;

        // Obliczanie promienia rozmycia na podstawie pathWidth, normalizowany do rozdzielczo�ci terenu
        float blurRadius = (((float)pathWidth) / terrainData.size.x) * terrainData.alphamapWidth;

        // Upewniamy si�, �e promie� rozmycia nie jest mniejszy ni� minimalna warto��
        blurRadius = Mathf.Max(blurRadius, 1.0f);

        // Rozmywanie �cie�ki z p�ynniejszym przej�ciem
        for (int x = Mathf.Max(0, alphaMapX - Mathf.RoundToInt(blurRadius)); x <= Mathf.Min(terrainData.alphamapWidth - 1, alphaMapX + Mathf.RoundToInt(blurRadius)); x++)
        {
            for (int z = Mathf.Max(0, alphaMapZ - Mathf.RoundToInt(blurRadius)); z <= Mathf.Min(terrainData.alphamapHeight - 1, alphaMapZ + Mathf.RoundToInt(blurRadius)); z++)
            {
                // Obliczanie odleg�o�ci od �rodka �cie�ki
                float distance = Vector2.Distance(new Vector2(x, z), new Vector2(alphaMapX, alphaMapZ));

                // Zastosowanie p�ynnej funkcji przej�cia - interpolacja kwadratowa
                float falloff = Mathf.SmoothStep(1.0f, 0.0f, distance / blurRadius);

                // Aktualizacja mapy alfa dla �cie�ki z p�ynniejszym przej�ciem
                if (distance <= blurRadius)
                {
                    for (int layer = 0; layer < terrainData.alphamapLayers; layer++)
                    {
                        if (layer == pathLayerIndex)
                        {
                            // Delikatne zwi�kszanie wp�ywu warstwy �cie�ki
                            alphaMaps[z, x, pathLayerIndex] = Mathf.Max(alphaMaps[z, x, pathLayerIndex], falloff);
                        }
                        else if (layer == mainLayerIndex)
                        {
                            // Delikatne zmniejszenie wp�ywu g��wnej warstwy, aby nie by�o nagiego odci�cia
                            alphaMaps[z, x, mainLayerIndex] = Mathf.Lerp(alphaMaps[z, x, mainLayerIndex], 0.3f, falloff);
                        }
                        else
                        {
                            // Zerowanie innych warstw w tych punktach, aby p�ynnie uwidoczni� �cie�k�
                            alphaMaps[z, x, layer] = Mathf.Lerp(alphaMaps[z, x, layer], 0.0f, falloff);
                        }
                    }
                }
            }
        }
    }

    private bool IsPositionOnPath(float x, float z)
    {
        TerrainData terrainData = terrain.terrainData;

        // Pobranie mapy alfa terenu, kt�ra okre�la mieszank� tekstur na powierzchni
        float[,,] alphaMaps = terrainData.GetAlphamaps(0, 0, terrainData.alphamapWidth, terrainData.alphamapHeight);
        int whatSizeParameter = 1;

        // Inicjalizacja indeksu warstwy tekstury �cie�ki
        int pathLayerIndex = -1;

        // Szukanie indeksu warstwy tekstury, kt�ra odpowiada �cie�ce
        for (int i = 0; i < terrainData.terrainLayers.Length; i++)
        {
            if (terrainData.terrainLayers[i] == pathLayer)
            {
                pathLayerIndex = i;
                break;
            }
        }
        if (terrainData.size.x <= 1900)
        {
            whatSizeParameter = 3;
        }
        else
        {
            whatSizeParameter = 4;
        }

        // Przekszta�cenie wsp�rz�dnych punktu na wsp�rz�dne mapy alfa
        int alphaMapX = Mathf.RoundToInt((x / terrainData.size.x) * terrainData.alphamapWidth);
        int alphaMapZ = Mathf.RoundToInt((z / terrainData.size.z) * terrainData.alphamapHeight);

        // Sprawdzenie, czy wsp�rz�dne znajduj� si� w granicach mapy alfa
        if (alphaMapX >= 0 && alphaMapX < terrainData.alphamapWidth && alphaMapZ >= 0 && alphaMapZ < terrainData.alphamapHeight)
        {
            // Sprawdzenie, czy warstwa �cie�ki w danym punkcie ma warto�� alfa r�wn� 1
            if (alphaMaps[alphaMapZ, alphaMapX, pathLayerIndex] == 1)
            {
                return true; // Punkt znajduje si� na �cie�ce, wi�c nie mo�na tutaj generowa� obiekt�w
            }

            // Sprawdzenie odleg�o�ci od �cie�ki
            for (int xOffset = -pathWidth / whatSizeParameter; xOffset <= pathWidth / whatSizeParameter; xOffset++)
            {
                for (int zOffset = -pathWidth / whatSizeParameter; zOffset <= pathWidth / whatSizeParameter; zOffset++)
                {
                    int xCoord = alphaMapX + xOffset;
                    int zCoord = alphaMapZ + zOffset;
                    if (xCoord >= 0 && xCoord < terrainData.alphamapWidth && zCoord >= 0 && zCoord < terrainData.alphamapHeight)
                    {
                        if (alphaMaps[zCoord, xCoord, pathLayerIndex] == 1)
                        {
                            // Punkt jest w okolicy �cie�ki, nie mo�na tutaj generowa� obiekt�w
                            return true;
                        }
                    }
                }
            }
        }

        return false; // Punkt nie znajduje si� na �cie�ce ani w jej okolicy, mo�na generowa� obiekty
    }
    #endregion
}
