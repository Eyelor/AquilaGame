using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLastGame : MonoBehaviour
{
    //public string sceneToLoad; // Nazwa sceny do za³adowania

    private void OnMouseDown()
    {
        SavesData savesData = SaveManager.Instance.LoadSavesData();
        
        if (savesData != null)
        {
            if (savesData.savesDataList.Count > 0 && savesData.IsSaveDataExists(savesData.currentSaveId))
            {
                if (!AudioSystem.Instance.audioSources.effectsAudioSource.isPlaying)
                {
                    AudioSystem.Instance.PlaySound(AudioSystem.Instance.audioSources.effectsAudioSource, AudioSystem.Instance.buttonForwardSound, false);
                }

                string sceneToLoad = savesData.GetSaveData(savesData.currentSaveId).saveLocation;

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

                SceneTransitionManager.nextSceneName = sceneToLoad;
                SceneManager.LoadScene("LoadingPanel");
            }
            else
            {
                // Jeœli nie ma ¿adnego zapisu, to wyœwietlamy odpowiedni komunikat
                Debug.Log("No saves found or current save id <= 0.");
            }
        }
        else
        {
            // Jeœli nie ma ¿adnego zapisu, to wyœwietlamy odpowiedni komunikat
            Debug.Log("No saves found.");
        }
    }
}
