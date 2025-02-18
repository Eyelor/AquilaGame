using System.Collections.Generic;
using UnityEngine;

public class PathGeneration
{
    private Terrain terrain;
    private float minHeight;
    private float maxHeight;
    private float addHeight;
    private int howManyToRemove;
    private int excessLand;
    private float pathHeightOutOfBorder;
    public PathGeneration(Terrain terrain, float minHeight, float maxHeight, float addHeight, int howManyToRemove, int excessLand, float pathHeightOutOfBorder)
    {
        this.terrain = terrain;
        this.minHeight = minHeight;
        this.maxHeight = maxHeight;
        this.addHeight = addHeight;
        this.howManyToRemove = howManyToRemove;
        this.excessLand = excessLand;
        this.pathHeightOutOfBorder = pathHeightOutOfBorder;
    }

    // Funkcja generuj¹ca ca³¹ œcie¿kê
    public List<Vector3> GeneratePath()
    {
        List<Vector3> pathPoints = new List<Vector3>();
        TerrainData terrainData = terrain.terrainData;
        int desiredPathLength = (((int) terrainData.size.z - excessLand * 2) / 4) + (((int) pathHeightOutOfBorder) / 2);

        Vector3 startPoint = GenerateStartPoint();
        if (startPoint == Vector3.zero)
        {
            Debug.LogWarning("Nie uda³o siê znaleœæ odpowiedniego punktu startowego po 100 próbach.");
            return pathPoints;
        }

        pathPoints.Add(startPoint);

        int mode = 1;
        for (int i = 1; i < desiredPathLength ; i++)
        {
            // Wywo³anie funkcji generuj¹cej punkty z argumentami:
            // Pierwszy argument to ostatni punkt œcie¿ki
            // Nastêpne dwa argumenty dotycz¹ zakresu na osi x, a ostatni argument osi z
            Vector3 nextPoint = GenerateNextPoint(pathPoints[i - 1], -5f, 5f, 4f);
            if (nextPoint != Vector3.zero)
            {
                pathPoints.Add(nextPoint);
            }
            else
            {
                Debug.LogWarning("Nie znaleziono odpowiedniego nastêpnego punktu dla œcie¿ki. MODE: " + mode);

                // Je¿eli mode mniejszy od 3 to mo¿na próbowaæ generowaæ zastêpcze punkty
                if(mode < 3)
                {
                    // Usuniêcie ostatnich howManyToRemove punktów je¿eli jest ich wystarczaj¹ca iloœæ
                    // I je¿eli jest mo¿liwoœæ ich zast¹pienia alternatywnymi
                    if (pathPoints.Count >= (howManyToRemove + 1))
                    {
                        // Ustalenie ostatniego ósmego od koñca poprawego punktu œcie¿ku
                        Vector3 lastValidPoint = pathPoints[pathPoints.Count - (howManyToRemove + 1)];

                        // Wygenerowanie zastêpczych maks howManyToRemove punktów w trybie mode = 1 (w prawo), lub mode = 2 (w lewo)
                        // Lista musi zawieraæ tyle dobrych punktów ile usuniêto. Inaczej zmieniamy tryb i próbujemy przejœæ drug¹ stron¹
                        List<Vector3> newPoints = GenerateAlternativePoints(lastValidPoint, mode, -5f, 5f, 4f, howManyToRemove);
                        if (newPoints.Count == howManyToRemove + 3)
                        {
                            pathPoints.RemoveRange(pathPoints.Count - howManyToRemove, howManyToRemove);
                            pathPoints.AddRange(newPoints);
                            i = pathPoints.Count - 1; // Dostosowanie indeksu przejœcia pêtli
                            mode = 1;
                            continue;
                        }
                        else
                        {
                            mode += 1;
                            i = pathPoints.Count - 1;
                            continue; 
                        }
                    }
                    else
                    {
                        // Wyczyszczenie istniej¹cej œcie¿ki i stworzeni jej od nowa
                        pathPoints.Clear();
                        startPoint = GenerateStartPoint();
                        if (startPoint == Vector3.zero)
                        {
                            Debug.LogWarning("Nie uda³o siê znaleœæ odpowiedniego punktu startowego po 100 próbach.");
                            return pathPoints;
                        }

                        pathPoints.Add(startPoint);
                        i = 1;
                        continue;
                    }
                }
                else
                {
                    return pathPoints;
                }
            }
        }

        Vector3 endPoint = GenerateEndPoint(pathPoints[desiredPathLength - 1]);
        if (endPoint == Vector3.zero)
        {
            Debug.LogWarning("Nie uda³o siê znaleœæ odpowiedniego punktu koñcowego");
            return pathPoints;
        }

        pathPoints.Add(endPoint);

        return pathPoints;
    }

    // Funkcja generuj¹ca punkt startowy œcie¿ki
    private Vector3 GenerateStartPoint()
    {
        Vector3 startPoint = Vector3.zero;
        TerrainData terrainData = terrain.terrainData;

        int attempts = 0;
        while (attempts < 100)
        {
            // Wybór zakresu w osi x w którym bêdzie generowany punkt startowy œcie¿ki
            float x = Random.Range((terrainData.size.x / 2) - 200, (terrainData.size.x / 2) + 200);
            float z = excessLand - pathHeightOutOfBorder;
            float y = terrain.SampleHeight(new Vector3(x, 0, z)) + terrain.transform.position.y;
            startPoint = new Vector3(x, y, z);

            // Sprawdzenie czy wybrany punkt spe³nia warunki
            if (IsValidPoint(startPoint))
            {
                return startPoint;
            }

            attempts++;
        }

        return Vector3.zero;
    }

    // Funkcja generuj¹ca kolejne punkty œcie¿ki oprócz startowego i koñcowego
    private Vector3 GenerateNextPoint(Vector3 currentPoint, float numberDxMin, float numberDxMax, float numberDz)
    {
        List<Vector3> validPoints = new List<Vector3>();
        TerrainData terrainData = terrain.terrainData;

        for (float dx = numberDxMin; dx <= numberDxMax; dx += 1f)
        {
            // Ograniczenie ¿e nastêpny punkt mo¿e byæ punktem pomiêdzy
            // numberDxMin i numberDxMax na osi x w odleg³oœci od poprzedniego punktu i
            // oddalony o numberDz na osi z
            float x = currentPoint.x + dx;
            float z = currentPoint.z + numberDz;

            // Sprawdzenie czy wpó³rzêdne znaduj¹ siê wewn¹trz terenu 
            if (x < excessLand || x >= (terrainData.size.x - excessLand) || z < excessLand - pathHeightOutOfBorder || z >= (terrainData.size.z - excessLand) + pathHeightOutOfBorder)
            {
                continue;
            }

            // Ustalenie aktualnej pozycji terenu w osi y dla wspo³rzêdnych x i z 
            float y = terrain.SampleHeight(new Vector3(x, 0, z)) + terrain.transform.position.y;
            Vector3 potentialPoint = new Vector3(x, y, z);
                
            // Sprawdzenie czy dany punkt spe³nia warunki
            // Jeœli tak to jest dodawany do listy poprawnych punktów 
            if (IsValidPoint(potentialPoint))
            {
                validPoints.Add(potentialPoint);
            }  
        }

        // Je¿eli istniej¹ jakieœ poprawne punkty to jest z nich
        // losowany jeden punkt bêd¹cy nastêpnym punktem œcie¿ki
        if (validPoints.Count > 0)
        {
            Vector3 goodPoint = validPoints[Random.Range(0, validPoints.Count)];
            // Debug.Log(goodPoint);
            return goodPoint;
        }
        return Vector3.zero;
    }

    // Funkcja generuj¹ca punkt koñcowy œcie¿ki
    private Vector3 GenerateEndPoint(Vector3 currentPoint)
    {
        Vector3 endPoint = Vector3.zero;
        TerrainData terrainData = terrain.terrainData;

        int attempts = 0;
        while (attempts < 100)
        {
            // Wybór zakresu w osi x w którym bêdzie generowany punkt koñcowy œcie¿ki
            float x = currentPoint.x + Random.Range(-5f, 5f);
            float z = terrainData.size.z - excessLand + pathHeightOutOfBorder;
            float y = terrain.SampleHeight(new Vector3(x, 0, z)) + terrain.transform.position.y;
            endPoint = new Vector3(x, y, z);

            // Sprawdzenie czy wybrany punkt spe³nia warunki
            if (IsValidPoint(endPoint))
            {
                return endPoint;
            }

            attempts++;
        }

        return Vector3.zero;
    }

    // Funkcja generuj¹ca alternatywn¹ iloœæ punktów
    private List<Vector3> GenerateAlternativePoints(Vector3 currentPoint, int mode, float numberDxMin, float numberDxMax, float numberDz, int howManyToRemove)
    {
        List<Vector3> alternativePoints = new List<Vector3>();
        TerrainData terrainData = terrain.terrainData;

        // Przejœcie pêtli tyle razy ile usuniêto punktów
        for (int i = 0; i < howManyToRemove + 3; i++)
        {
            // Dostosowanie kierunku œciezki na podstawie zmiennej mode
            float dx = mode == 1 ? numberDxMax : numberDxMin;
            float x = currentPoint.x + dx;
            float z = currentPoint.z + numberDz;

            if (x < excessLand || x >= (terrainData.size.x - excessLand) || z < excessLand - pathHeightOutOfBorder || z >= (terrainData.size.z - excessLand) + pathHeightOutOfBorder)
            {
                continue;
            }

            float y = terrain.SampleHeight(new Vector3(x, 0, z)) + terrain.transform.position.y;
            Vector3 potentialPoint = new Vector3(x, y, z);

            if (IsValidPoint(potentialPoint))
            {
                alternativePoints.Add(potentialPoint);
                currentPoint = potentialPoint; // Przejœcie do nowego punktu w nastêpnej iteracji
            }
            else
            {
                break;
            }
        }

        // Zwrócenie listy alternatywnych punktów
        return alternativePoints;
    }

    // Funkcja generuj¹ca odnogê œcie¿ki g³ównej
    public List<Vector3> GenerateBranchPath(List<Vector3> mainPath, int lowerStartPoint, int higherStartPoint, bool direction)
    {
        // Lista punktów dla odnogi
        List<Vector3> branchPoints = new List<Vector3>();

        // Sprawdzenie, czy g³ówna œcie¿ka jest wystarczaj¹co d³uga, aby wygenerowaæ odnogê
        if (mainPath.Count < higherStartPoint)
        {
            Debug.LogWarning("G³ówna œcie¿ka jest zbyt krótka, aby wygenerowaæ odnogê.");
            return branchPoints;
        }

        // Wybór losowego punktu pocz¹tkowego odnogi w przedziale od 50 do 120 punktu œcie¿ki g³ównej
        int startIndex = Random.Range(lowerStartPoint, higherStartPoint + 1);
        Vector3 startPoint = mainPath[startIndex];
        branchPoints.Add(startPoint);

        // Ustalanie kierunku generowania odnogi (na prawo czy na lewo) w zale¿noœci od wyboru przy wywo³aniu
        bool goingRight = direction;
        int branchLength = mainPath.Count - startIndex - Random.Range(10,30); // Obliczanie d³ugoœci odnogi w punktach œcie¿ki

        // Ustalanie minimalnej i maksymalnej zmiany pozycji x w zale¿noœci od kierunku
        float minX = goingRight ? 2f : -5f;
        float maxX = goingRight ? 5f : -2f;

        int mode = 1;
        int howManyToRemoveBranch = howManyToRemove / 2;
        // Generowanie punktów odnogi
        for (int i = 1; i <= branchLength; i++)
        {
            Vector3 nextPoint; 

            // Ustalanie minimalnej i maksymalnej zmiany pozycji x w zale¿noœci od kierunku
            // Generowanie nowego punktu na podstawie poprzedniego
            if (i <= 15)
            {
                nextPoint = GenerateNextPoint(branchPoints[i - 1], minX, maxX, Random.Range(-4f, 4f));
            } 
            else
            {
                if (goingRight)
                {
                    nextPoint = GenerateNextPoint(branchPoints[i - 1], -2f, 5f, Random.Range(0f, 4f));
                } 
                else
                {
                    nextPoint = GenerateNextPoint(branchPoints[i - 1], -5f, 2f, Random.Range(0f, 4f));
                }
               
            }
            if (nextPoint != Vector3.zero)
            {
                branchPoints.Add(nextPoint);
            }
            else
            {
                Debug.LogWarning("Nie znaleziono odpowiedniego nastêpnego punktu dla odnogi! MODE: " + mode);

                // Je¿eli mode mniejszy od 3 to mo¿na próbowaæ generowaæ zastêpcze punkty
                if (mode < 3)
                {
                    // Usuniêcie ostatnich howManyToRemoveBranch punktów je¿eli jest ich wystarczaj¹ca iloœæ
                    // I je¿eli jest mo¿liwoœæ ich zast¹pienia alternatywnymi
                    if (branchPoints.Count >= (howManyToRemoveBranch + 1))
                    {
                        // Ustalenie ostatniego przed usuwaniem od koñca poprawego punktu œcie¿ku
                        Vector3 lastValidPoint = branchPoints[branchPoints.Count - (howManyToRemoveBranch + 1)];

                        // Wygenerowanie zastêpczych maks howManyToRemoveBranch punktów w trybie mode = 1 (w prawo), lub mode = 2 (w lewo)
                        // Lista musi zawieraæ tyle dobrych punktów ile usuniêto. Inaczej zmieniamy tryb i próbujemy przejœæ drug¹ stron¹
                        List<Vector3> newPoints;
                        if (goingRight) newPoints = GenerateAlternativePoints(lastValidPoint, mode, -2f, 5f, 4f, howManyToRemoveBranch);
                        else newPoints = GenerateAlternativePoints(lastValidPoint, mode, -5f, 2f, 4f, howManyToRemoveBranch);

                        if (newPoints.Count == howManyToRemoveBranch + 3)
                        {
                            branchPoints.RemoveRange(branchPoints.Count - howManyToRemoveBranch, howManyToRemoveBranch);
                            branchPoints.AddRange(newPoints);
                            i = branchPoints.Count - 1; // Dostosowanie indeksu przejœcia pêtli
                            mode = 1;
                            continue;
                        }
                        else
                        {
                            mode += 1;
                            i = branchPoints.Count - 1;
                            continue;
                        }
                    }
                    else
                    {
                        return branchPoints;
                    }
                }
                else
                {
                    return branchPoints;
                }
            }
        }

        return branchPoints;
    }

    // Funkcja sprawdzaj¹ca poprawnoœæ punktów œcie¿ki
    private bool IsValidPoint(Vector3 point)
    {
        TerrainData terrainData = terrain.terrainData;

        // Sprawdzenie czy w odgleg³oœci 7 od wygenerowanego punktu
        // ka¿dy punkt spe³nia warunki poprawnoœci
        for (float dx = -7f; dx <= 7f; dx += 1f)
        {
            for (float dz = 7f; dz <= 7f; dz += 1f)
            {
                float x = point.x + dx;
                float z = point.z + dz;

                if (x < excessLand || x >= (terrainData.size.x - excessLand) || z < excessLand - pathHeightOutOfBorder || z >= (terrainData.size.z - excessLand) + pathHeightOutOfBorder)
                {
                    continue;
                }

                float y = terrain.SampleHeight(new Vector3(x, 0, z)) + terrain.transform.position.y;

                // Sprawdzenie warunku dotycz¹cego po³o¿enia na osi y
                if (y < minHeight - addHeight || y > maxHeight + addHeight)
                {
                    return false;
                }

                // Sprawdzenie warunku dotycz¹cego nachylenia terenu w punkcie
                float slope = CalculateSlope(x, z);
                if (slope > 15.0f)
                {
                    return false;
                }
            }
        }
        return true;
    }

    // Funkcja obliczaj¹ca nachylenie terenu w punkcie
    private float CalculateSlope(float x, float z)
    {
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
}
