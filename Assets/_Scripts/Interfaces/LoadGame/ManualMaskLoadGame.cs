using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ManualMaskLoadGame : MonoBehaviour
{
    public RectTransform content;  // Obiekt Content, który zawiera dynamicznie dodawane/usuwane zapisy (podkomponenty)
    [SerializeField] private RectTransform saveTemplate;
    private int currentSaveIndex = 1;    // Indeks aktualnie dezaktywowanego/aktywowanego zapisu
    private float nextThreshold = 90f;  // Pocz¹tkowa wartoœæ progu dla dezaktywacji od góry
    private float previousThreshold = -30f; // Próg do aktywacji
    private int visibleCount = 6;        // Liczba pocz¹tkowo widocznych elementów
    private float lowerThreshold = 40f; // Pocz¹tkowa wartoœæ progu dla dolnych zapisów
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

        // Sprawdzamy, czy liczba dzieci w content zmieni³a siê
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
        // Pobieramy górn¹ pozycjê contentu
        float contentTop = content.anchoredPosition.y;

        // Sprawdzamy, czy osi¹gniêto kolejny próg dezaktywacji od góry
        if (contentTop >= nextThreshold && currentSaveIndex < content.childCount)
        {
            // Dezaktywujemy bie¿¹cy zapis
            SetSaveActive(currentSaveIndex, false);

            // Zwiêkszamy indeks i próg dla nastêpnego zapisu
            currentSaveIndex++;
            nextThreshold += 120f;
            previousThreshold += 120f;  // Próg do aktywacji dla wczeœniejszych
        }
    }

    void CheckAndActivateSaves()
    {
        // Pobieramy górn¹ pozycjê contentu
        float contentTop = content.anchoredPosition.y;

        // Sprawdzamy, czy przesunêliœmy siê z powrotem powy¿ej progu aktywacji
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
        // Pobieramy doln¹ pozycjê contentu
        float contentBottom = content.anchoredPosition.y;

        // Aktywacja dolnych elementów przy scrollowaniu w górê
        if (contentBottom >= lowerThreshold && visibleCount < content.childCount)
        {
            // Debug.Log(contentBottom);
            // Aktywujemy kolejny element z do³u
            SetSaveActive(visibleCount, true);

            // Zwiêkszamy liczbê widocznych elementów
            visibleCount++;
            lowerThreshold += 120f;  // Przesuwamy próg dla kolejnych zapisów
        }
    }

    void CheckAndDeactivateLowerSaves()
    {
        // Pobieramy doln¹ pozycjê contentu
        float contentBottom = content.anchoredPosition.y;

        // Dezaktywacja dolnych elementów przy scrollowaniu w dó³
        if (contentBottom < lowerThreshold - 120f && visibleCount > 5)
        {
            // Zmniejszamy liczbê widocznych elementów
            visibleCount--;

            // Dezaktywujemy dolny element
            SetSaveActive(visibleCount, false);

            // Przesuwamy próg dla dezaktywacji kolejnych zapisów
            lowerThreshold -= 120f;
        }
    }

    void SetSaveActive(int index, bool active)
    {
        // Sprawdzamy, czy indeks jest poprawny i ustawiamy aktywnoœæ podkomponentu
        if (index >= 0 && index < content.childCount)
        {
            Transform save = content.GetChild(index);
            if (save == saveTemplate) return;
            save.gameObject.SetActive(active);
        }
    }

    // Metoda do aktualizacji wysokoœci contentu
    void UpdateContentHeight()
    {
        int saveCount = content.childCount;

        // Jeœli nie ma zapisów, ustawiamy domyœln¹ wartoœæ bottom na 600
        if (saveCount == 0)
        {
            content.offsetMin = new Vector2(content.offsetMin.x, 600f);
        }
        else
        {
            // Dla ka¿dego zapisu odejmujemy 120 jednostek od bottom contentu
            float newBottom = 620f - (120f * saveCount);
            content.offsetMin = new Vector2(content.offsetMin.x, newBottom);
        }
    }

    void UpdateContentDown()
    {
        Vector2 offsetMin = content.offsetMin;
        offsetMin.y += 120;  // Zwiêksz wartoœæ dolnego marginesu
        content.offsetMin = offsetMin;
    }

    void UpdateContentUp()
    {
        Vector2 offsetMin = content.offsetMin;
        offsetMin.y -= 120;  // Zwiêksz wartoœæ dolnego marginesu
        content.offsetMin = offsetMin;
    }

    private void InitialConfiguration()
    {
        // Na pocz¹tku aktywujemy tylko pierwsze 5 elementów
        for (int i = 0; i < content.childCount; i++)
        {
            SetSaveActive(i, i < visibleCount);
        }

        // Ustawiamy odpowiedni¹ wysokoœæ contentu na podstawie liczby zapisów
        UpdateContentHeight();

        // Ustawienie pocz¹tkowej liczby dzieci
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

        // Ustawiamy odpowiednie parametry na podstawie liczby zapisów
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
