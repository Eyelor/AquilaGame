using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HoverSave : MonoBehaviour
{
    public GameObject boardObject; // Obiekt planszy t≥a 
    private BoxCollider2D boxCollider;
    [SerializeField] private bool isFromMenuPause = false;
    [SerializeField] private GameObject _objectToRemoveFirst;

    private void Awake()
    {
        // Ustaw obiekt domyúlnie jako niewidoczny
        if (boardObject != null)
        {
            boardObject.SetActive(false);
        }
        boxCollider = GetComponent<BoxCollider2D>();
        if (!boxCollider.enabled)
        {
            boxCollider.enabled = true;
        }
    }

    // Metoda wywo≥ywana, gdy kursor najedzie na obiekt
    private void OnMouseEnter()
    {
        if (boardObject != null)
        {
            boardObject.SetActive(true);
        }
    }

    // Metoda wywo≥ywana, gdy kursor opuúci obiekt
    private void OnMouseExit()
    {
        if (boardObject != null)
        {
            boardObject.SetActive(false);
        }
    }

    private void OnMouseDown()
    {
        SavesData savesData = SaveManager.Instance.LoadSavesData();

        if (savesData != null)
        {
            if (savesData.savesDataList.Count > 0)
            {
                if (gameObject.TryGetComponent(out SaveValuesSetter saveValuesSetter) 
                    && saveValuesSetter.GetSaveName().Length >= 7 
                    && int.TryParse(saveValuesSetter.GetSaveName().Substring(6), out int saveId) 
                    && savesData.IsSaveDataExists(saveId))
                {
                    savesData.currentSaveId = saveId;
                    Debug.Log($"(befor update currentSaveId in HoverSave) currentSaveId={SaveManager.Instance.GetSaveManagerCurrentSaveId()}");
                    SaveManager.Instance.UpdateSaveManagerCurrentSaveId(saveId);
                    Debug.Log($"(after update currentSaveId in HoverSave) currentSaveId={SaveManager.Instance.GetSaveManagerCurrentSaveId()}");
                    savesData.UpdateSaveDataDateTime(saveId);
                    SaveManager.Instance.SaveSavesData(savesData);

                    if (!AudioSystem.Instance.audioSources.effectsAudioSource.isPlaying)
                    {
                        AudioSystem.Instance.PlaySound(AudioSystem.Instance.audioSources.effectsAudioSource, AudioSystem.Instance.buttonForwardSound, false);
                    }
                    AudioSystem.Instance.StopMusicCoroutine();
                    AudioSystem.Instance.StopEffectsCoroutine();
                    AudioSystem.Instance.StopAllCoroutines();

                    string sceneToLoad = savesData.GetSaveData(saveId).saveLocation;

                    if (sceneToLoad == "Mapa Globalna")
                    {
                        sceneToLoad = "GlobalMap";
                    }
                    else if (sceneToLoad == "Port")
                    {
                        sceneToLoad = "Port";
                    }
                    else if (sceneToLoad.StartsWith("Wyspa ") 
                        && int.TryParse(sceneToLoad.Substring(6), out int islandId))
                    {
                        IslandDataList islandDataList = SaveManager.Instance.LoadIslandDataList();

                        if (islandDataList != null)
                        {
                            if (islandDataList.islandsDataList.Count > 0)
                            {
                                IslandData islandData = islandDataList.GetIslandData(islandId);

                                if (islandData.size == "large" && islandData.type == "grassy")
                                {
                                    sceneToLoad = "BigIslandGrass";
                                }
                                else if (islandData.size == "large" && islandData.type == "muddy")
                                {
                                    sceneToLoad = "BigIslandMudd";
                                }
                                else if (islandData.size == "large" && islandData.type == "sandy")
                                {
                                    sceneToLoad = "BigIslandSand";
                                }
                                else if (islandData.size == "medium" && islandData.type == "grassy")
                                {
                                    sceneToLoad = "MiddleIslandGrass";
                                }
                                else if (islandData.size == "medium" && islandData.type == "muddy")
                                {
                                    sceneToLoad = "MiddleIslandMudd";
                                }
                                else if (islandData.size == "medium" && islandData.type == "sandy")
                                {
                                    sceneToLoad = "MiddleIslandSand";
                                }
                                else if (islandData.size == "small" && islandData.type == "grassy")
                                {
                                    sceneToLoad = "SmallIslandGrass";
                                }
                                else if (islandData.size == "small" && islandData.type == "muddy")
                                {
                                    sceneToLoad = "SmallIslandMudd";
                                }
                                else if (islandData.size == "small" && islandData.type == "sandy")
                                {
                                    sceneToLoad = "SmallIslandSand";
                                }
                                else
                                {
                                    Debug.LogError("Island data is not correct.");
                                }
                            }
                            else
                            {
                                Debug.LogError("Island data list is empty.");
                            }
                        }
                        else
                        {
                            Debug.LogError("Island data list is null.");
                        }
                    }
                    else
                    {
                        Debug.LogError("Scene name is not correct.");
                    }

                    if (isFromMenuPause)
                    {
                        EnableBoxCollider2D();
                        AudioSystem.Instance.audioSources.musicAudioSource.Stop();
                        AudioSystem.Instance.StopSound(AudioSystem.Instance.audioSources.musicAudioSource);
                        AudioSystem.Instance.StopSound(AudioSystem.Instance.audioSources.fightAudioSource);

                        if (_objectToRemoveFirst != null)
                        {
                            LocalIslandGenerationManager.Instance.exitToMainMenu = true;
                            Destroy(_objectToRemoveFirst);
                        }

                        AudioSystem.Instance.audioSources.musicAudioSource.volume = AudioSystem.Instance.musicVolume;
                        AudioSystem.Instance.audioSources.fightAudioSource.volume = AudioSystem.Instance.musicVolume;

                        Time.timeScale = 1f;
                    }

                    SceneTransitionManager.nextSceneName = sceneToLoad;
                    SceneManager.LoadScene("LoadingPanel");
                }
            }
            else
            {
                // Jeúli nie ma øadnego zapisu, to wyúwietlamy odpowiedni komunikat
                Debug.Log("No saves found.");
            }
        }
        else
        {
            // Jeúli nie ma øadnego zapisu, to wyúwietlamy odpowiedni komunikat
            Debug.Log("No saves found.");
        }
    }

    public void EnableBoxCollider2D()
    {
        // Znajdü wszystkie obiekty zawierajπce komponent "BoxCollider2D"
        BoxCollider2D[] scriptsToDisable = FindObjectsOfType<BoxCollider2D>();

        // Przejdü przez wszystkie znalezione skrypty i w≥πcz je
        foreach (BoxCollider2D script in scriptsToDisable)
        {
            // Debug.Log(script);
            script.enabled = true;
        }
    }
}
