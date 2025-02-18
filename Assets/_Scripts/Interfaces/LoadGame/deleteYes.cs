using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class deleteYes : MonoBehaviour
{
    public GameObject parentObject; // Obiekt nadrzêdny do przeszukania
    public GameObject saveToDelete; // Obiekt, którego nazwa zostanie u¿yta do wyszukania obiektu do usuniêcia
    public GameObject exitPanel; // Obiekt panelu UI do wyœwietlenia przy wyjœciu
    public GameObject exitBackgroundPanel; // Panel przezroczystosci t³a przy wyjœciu
    public GameObject lineObject; // Obiekt linii
    public RectTransform contentRectTransform; // RectTransform obiektu Content

    private void OnMouseDown()
    {       
        if (!AudioSystem.Instance.audioSources.effectsAudioSource.isPlaying)
        {
            AudioSystem.Instance.PlaySound(AudioSystem.Instance.audioSources.effectsAudioSource, AudioSystem.Instance.buttonForwardSound, false);
        }
        if (saveToDelete != null && parentObject != null)
        {
            // Nazwa obiektu, którego szukamy do usuniêcia
            string nameToDelete = saveToDelete.name;

            // Szukaj podrzêdnego obiektu o nazwie 'nameToDelete' w hierarchii 'parentObject'
            Transform targetChild = FindChildByName(parentObject.transform, nameToDelete);

            if (targetChild != null)
            {
                // Pobierz indeks obiektu do usuniêcia w hierarchii rodzica
                int indexToDelete = targetChild.GetSiblingIndex();

                if (targetChild.gameObject.TryGetComponent(out SaveValuesSetter saveValuesSetter) 
                    && saveValuesSetter.GetSaveName().Length >= 7 
                    && int.TryParse(saveValuesSetter.GetSaveName().Substring(6), out int saveId))
                {
                    SavesData savesData = SaveManager.Instance.LoadSavesData();

                    if (savesData != null)
                    {
                        savesData.DeleteSaveData(saveId);
                        SaveManager.Instance.SaveSavesData(savesData);
                    }

                    // Combine the folder path with persistentDataPath
                    string folderPath = Path.Combine(Application.persistentDataPath, "Zapis " + saveId);

                    // Check if the directory exists to avoid errors
                    if (Directory.Exists(folderPath))
                    {
                        // Delete the folder and all of its contents
                        Directory.Delete(folderPath, true);
                        Debug.Log($"Folder '{"Zapis " + saveId}' and its contents have been deleted.");
                    }
                    else
                    {
                        Debug.LogWarning($"Folder '{"Zapis " + saveId}' does not exist at '{folderPath}'.");
                    }
                }
                else
                {
                    Debug.LogError("Save name is not in correct format.");
                }

                // Usuñ znaleziony obiekt
                Destroy(targetChild.gameObject);

                // Debug.Log(indexToDelete);
                // Przesuñ wszystkie kolejne obiekty w hierarchii o 120 jednostek w osi Y, ale tylko te, których nazwa zaczyna siê od "SAVE"
                for (int i = indexToDelete; i < parentObject.transform.childCount; i++)
                {
                    Transform childTransform = parentObject.transform.GetChild(i);

                    // SprawdŸ, czy nazwa obiektu zaczyna siê od "SAVE"
                    if (childTransform.name.StartsWith("SAVE"))
                    {
                        // U¿ycie lokalnej pozycji do przesuniêcia
                        childTransform.localPosition += new Vector3(0, 120, 0); // Przesuniêcie o 120 jednostek w osi Y

                        // Debug.Log("OBIEKT O INDEKSIE " + i + " JEST " + childTransform.gameObject.activeSelf);
                        // Sprawdzenie obiektu o indeksie +5 i +6 do indeksu usuwanego
                        if (i > indexToDelete && i <= indexToDelete + 6)
                        {
                            // SprawdŸ, czy obiekt jest aktywny, a jeœli nie, to go aktywuj
                            if (!childTransform.gameObject.activeSelf)
                            {
                                childTransform.gameObject.SetActive(true);
                                // Debug.Log("Obiekt o indeksie 4 zosta³ aktywowany.");
                            }
                        }
                    }
                }
            }
            else
            {
                Debug.LogWarning("Nie znaleziono obiektu o nazwie " + nameToDelete + " w podrzêdnych komponentach.");
            }

            if (exitPanel != null)
            {
                exitPanel.SetActive(false);
            }
            if (exitBackgroundPanel != null)
            {
                exitBackgroundPanel.SetActive(false);
            }
            if (lineObject != null)
            {
                lineObject.SetActive(false);
            }
            EnableBoxCollider2D();
        }
        else
        {
            Debug.LogWarning("Nie ustawiono obiektu saveToDelete lub parentObject w inspektorze.");
        }
    }

    // Metoda pomocnicza do wyszukiwania dziecka o danej nazwie w hierarchii
    private Transform FindChildByName(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name)
            {
                return child;
            }
        }
        return null;
    }

    // Przywrócenie aktywnoœci pod plansz¹ wyjœcia
    private void EnableBoxCollider2D()
    {
        // ZnajdŸ wszystkie obiekty zawieraj¹ce komponent "BoxCollider2D"
        BoxCollider2D[] scriptsToDisable = FindObjectsOfType<BoxCollider2D>();

        // PrzejdŸ przez wszystkie znalezione skrypty i w³¹cz je
        foreach (BoxCollider2D script in scriptsToDisable)
        {
            script.enabled = true;
        }
    }
}
