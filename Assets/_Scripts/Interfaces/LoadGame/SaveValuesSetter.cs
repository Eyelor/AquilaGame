using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SaveValuesSetter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI saveNameText;
    [SerializeField] private TextMeshProUGUI saveLocationText;
    [SerializeField] private TextMeshProUGUI saveDateText;

    public void SetSaveValues(string saveName, string saveLocation, string saveDateTime)
    {
        saveNameText.text = saveName;
        saveLocationText.text = saveLocation;
        saveDateText.text = saveDateTime;
    }

    public string GetSaveName()
    {
        return saveNameText.text;
    }
}
