using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ManualMaskLoadGame : MonoBehaviour
{
    public RectTransform content;  // Obiekt Content, kt�ry zawiera dynamicznie dodawane/usuwane zapisy (podkomponenty)
    [SerializeField] private RectTransform saveTemplate;
    private int currentSaveIndex = 1;    // Indeks aktualnie dezaktywowanego/aktywowanego zapisu
    private float nextThreshold = 90f;  // Pocz�tkowa warto�� progu dla dezaktywacji od g�ry
    private float previousThreshold = -30f; // Pr�g do aktywacji
    private int visibleCount = 6;        // Liczba pocz�tkowo widocznych element�w
    private float lowerThreshold = 40f; // Pocz�tkowa warto�� progu dla dolnych zapis�w
    private int previousChildCount = 0; // Poprzednia liczba dzieci w content

    private void Awake()
    {
        saveTemplate.gameObject.SetActive(false);
    }

    void Start()
    {
        InitialLoading();
    }

    void Update()
    {

        // Sprawdzamy, czy liczba dzieci w content zmieni�a si�
        if (content.childCount != previousChildCount)
        {
            if (content.childCount < previousChildCount)
            {
                UpdateContentDown();
            } 
            else if (content.childCount > previousChildCount)
            {
                UpdateContentUp();
            }
            previousChildCount = content.childCount;
        }

        CheckAndDeactivateSaves();
        CheckAndActivateSaves();
        CheckAndActivateLowerSaves();
        CheckAndDeactivateLowerSaves();

        UpdateSaveDate();
    }

    void CheckAndDeactivateSaves()
    {
        // Pobieramy g�rn� pozycj� contentu
        float contentTop = content.anchoredPosition.y;

        // Sprawdzamy, czy osi�gni�to kolejny pr�g dezaktywacji od g�ry
        if (contentTop >= nextThreshold && currentSaveIndex < content.childCount)
        {
            // Dezaktywujemy bie��cy zapis
            SetSaveActive(currentSaveIndex, false);

            // Zwi�kszamy indeks i pr�g dla nast�pnego zapisu
            currentSaveIndex++;
            nextThreshold += 120f;
            previousThreshold += 120f;  // Pr�g do aktywacji dla wcze�niejszych
        }
    }

    void CheckAndActivateSaves()
    {
        // Pobieramy g�rn� pozycj� contentu
        float contentTop = content.anchoredPosition.y;

        // Sprawdzamy, czy przesun�li�my si� z powrotem powy�ej progu aktywacji
        if (contentTop < previousThreshold && currentSaveIndex > 0)
        {
            // Zmniejszamy indeks i aktywujemy poprzedni zapis
            currentSaveIndex--;
            SetSaveActive(currentSaveIndex, true);

            // Zmniejszamy progi do kolejnego aktywowania i dezaktywowania
            nextThreshold -= 120f;
            previousThreshold -= 120f;
        }
    }

    void CheckAndActivateLowerSaves()
    {
        // Pobieramy doln� pozycj� contentu
        float contentBottom = content.anchoredPosition.y;

        // Aktywacja dolnych element�w przy scrollowaniu w g�r�
        if (contentBottom >= lowerThreshold && visibleCount < content.childCount)
        {
            // Debug.Log(contentBottom);
            // Aktywujemy kolejny element z do�u
            SetSaveActive(visibleCount, true);

            // Zwi�kszamy liczb� widocznych element�w
            visibleCount++;
            lowerThreshold += 120f;  // Przesuwamy pr�g dla kolejnych zapis�w
        }
    }

    void CheckAndDeactivateLowerSaves()
    {
        // Pobieramy doln� pozycj� contentu
        float contentBottom = content.anchoredPosition.y;

        // Dezaktywacja dolnych element�w przy scrollowaniu w d�
        if (contentBottom < lowerThreshold - 120f && visibleCount > 5)
        {
            // Zmniejszamy liczb� widocznych element�w
            visibleCount--;

            // Dezaktywujemy dolny element
            SetSaveActive(visibleCount, false);

            // Przesuwamy pr�g dla dezaktywacji kolejnych zapis�w
            lowerThreshold -= 120f;
        }
    }

    void SetSaveActive(int index, bool active)
    {
        // Sprawdzamy, czy indeks jest poprawny i ustawiamy aktywno�� podkomponentu
        if (index >= 0 && index < content.childCount)
        {
            Transform save = content.GetChild(index);
            if (save == saveTemplate) return;
            save.gameObject.SetActive(active);
        }
    }

    // Metoda do aktualizacji wysoko�ci contentu
    void UpdateContentHeight()
    {
        int saveCount = content.childCount;

        // Je�li nie ma zapis�w, ustawiamy domy�ln� warto�� bottom na 600
        if (saveCount == 0)
        {
            content.offsetMin = new Vector2(content.offsetMin.x, 600f);
        }
        else
        {
            // Dla ka�dego zapisu odejmujemy 120 jednostek od bottom contentu
            float newBottom = 620f - (120f * saveCount);
            content.offsetMin = new Vector2(content.offsetMin.x, newBottom);
        }
    }

    void UpdateContentDown()
    {
        Vector2 offsetMin = content.offsetMin;
        offsetMin.y += 120;  // Zwi�ksz warto�� dolnego marginesu
        content.offsetMin = offsetMin;
    }

    void UpdateContentUp()
    {
        Vector2 offsetMin = content.offsetMin;
        offsetMin.y -= 120;  // Zwi�ksz warto�� dolnego marginesu
        content.offsetMin = offsetMin;
    }

    private void InitialConfiguration()
    {
        // Na pocz�tku aktywujemy tylko pierwsze 5 element�w
        for (int i = 0; i < content.childCount; i++)
        {
            SetSaveActive(i, i < visibleCount);
        }

        // Ustawiamy odpowiedni� wysoko�� contentu na podstawie liczby zapis�w
        UpdateContentHeight();

        // Ustawienie pocz�tkowej liczby dzieci
        previousChildCount = content.childCount;
    }

    private void InitialLoading()
    {
        SavesData savesData = SaveManager.Instance.LoadSavesData();

        if (savesData != null)
        {
            float addPosition = -64.81259f;
            foreach (SaveData save in savesData.savesDataList)
            {
                RectTransform saveInstance = Instantiate(saveTemplate, content);
                if (SceneManager.GetActiveScene().name != "LoadGame")
                {
                    saveInstance.gameObject.GetComponentInChildren<deleteSave>().gameObject.SetActive(false);
                    Transform tabliczka_usun = saveInstance.transform.Find("tabliczka_usun");
                    tabliczka_usun.gameObject.SetActive(false);
                }
                //saveInstance.gameObject.SetActive(true);
                saveInstance.anchoredPosition = new Vector2(saveInstance.anchoredPosition.x, addPosition);
                saveInstance.GetComponent<SaveValuesSetter>().SetSaveValues(save.saveName, save.saveLocation, save.saveDateTime);
                saveInstance.gameObject.name = "SAVE" + save.saveId;
                addPosition -= 120f;
            }
        }

        // Ustawiamy odpowiednie parametry na podstawie liczby zapis�w
        InitialConfiguration();
    }

    private void UpdateSaveDate()
    {
        SavesData savesData = SaveManager.Instance.LoadSavesData();

        if (savesData != null)
        {
            SaveValuesSetter[] saveValuesSetters = Object.FindObjectsOfType<SaveValuesSetter>();
            foreach (SaveValuesSetter saveValuesSetter in saveValuesSetters)
            {
                foreach (SaveData save in savesData.savesDataList)
                {
                    if (saveValuesSetter.GetSaveName() == save.saveName)
                    {
                        saveValuesSetter.SetSaveValues(save.saveName, save.saveLocation, save.saveDateTime);
                    }
                }
            }
            
        }
    }
}
