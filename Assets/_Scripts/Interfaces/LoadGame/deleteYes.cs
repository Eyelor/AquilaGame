using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class deleteYes : MonoBehaviour
{
    public GameObject parentObject; // Obiekt nadrz�dny do przeszukania
    public GameObject saveToDelete; // Obiekt, kt�rego nazwa zostanie u�yta do wyszukania obiektu do usuni�cia
    public GameObject exitPanel; // Obiekt panelu UI do wy�wietlenia przy wyj�ciu
    public GameObject exitBackgroundPanel; // Panel przezroczystosci t�a przy wyj�ciu
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
            // Nazwa obiektu, kt�rego szukamy do usuni�cia
            string nameToDelete = saveToDelete.name;

            // Szukaj podrz�dnego obiektu o nazwie 'nameToDelete' w hierarchii 'parentObject'
            Transform targetChild = FindChildByName(parentObject.transform, nameToDelete);

            if (targetChild != null)
            {
                // Pobierz indeks obiektu do usuni�cia w hierarchii rodzica
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

                // Usu� znaleziony obiekt
                Destroy(targetChild.gameObject);

                // Debug.Log(indexToDelete);
                // Przesu� wszystkie kolejne obiekty w hierarchii o 120 jednostek w osi Y, ale tylko te, kt�rych nazwa zaczyna si� od "SAVE"
                for (int i = indexToDelete; i < parentObject.transform.childCount; i++)
                {
                    Transform childTransform = parentObject.transform.GetChild(i);

                    // Sprawd�, czy nazwa obiektu zaczyna si� od "SAVE"
                    if (childTransform.name.StartsWith("SAVE"))
                    {
                        // U�ycie lokalnej pozycji do przesuni�cia
                        childTransform.localPosition += new Vector3(0, 120, 0); // Przesuni�cie o 120 jednostek w osi Y

                        // Debug.Log("OBIEKT O INDEKSIE " + i + " JEST " + childTransform.gameObject.activeSelf);
                        // Sprawdzenie obiektu o indeksie +5 i +6 do indeksu usuwanego
                        if (i > indexToDelete && i <= indexToDelete + 6)
                        {
                            // Sprawd�, czy obiekt jest aktywny, a je�li nie, to go aktywuj
                            if (!childTransform.gameObject.activeSelf)
                            {
                                childTransform.gameObject.SetActive(true);
                                // Debug.Log("Obiekt o indeksie 4 zosta� aktywowany.");
                            }
                        }
                    }
                }
            }
            else
            {
                Debug.LogWarning("Nie znaleziono obiektu o nazwie " + nameToDelete + " w podrz�dnych komponentach.");
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

    // Przywr�cenie aktywno�ci pod plansz� wyj�cia
    private void EnableBoxCollider2D()
    {
        // Znajd� wszystkie obiekty zawieraj�ce komponent "BoxCollider2D"
        BoxCollider2D[] scriptsToDisable = FindObjectsOfType<BoxCollider2D>();

        // Przejd� przez wszystkie znalezione skrypty i w��cz je
        foreach (BoxCollider2D script in scriptsToDisable)
        {
            script.enabled = true;
        }
    }
}
