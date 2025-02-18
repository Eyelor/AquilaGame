using TMPro;
using UnityEngine;
using System.Globalization;

public class ControlsJson : MonoBehaviour
{
    // Komponenty TextMeshPro przypisane z Inspektora
    public TextMeshProUGUI mouseSensitivity;    // Tekst dla wyœwietlania czu³oœci myszy
    public TextMeshProUGUI autoReload;    // Tekst dla wyœwietlania informacji o automatycznym prze³adowaniu 
    public TextMeshProUGUI runMode;    // Tekst dla wyœwietlania informacji o trybie biegu

    // Klasa zarz¹dzaj¹ca ustawieniami
    private SettingsDataSerialization settingsManager;

    private void Start()
    {
        // Znalezienie SettingsDataSerialization i zaktualizowanie UI na podstawie obecnych ustawieñ
        settingsManager = FindObjectOfType<SettingsDataSerialization>();
        UpdateControlsUI();
    }

    private void OnDisable()
    {
        SaveControlsSettings();
        GameInputSystem.Instance.UpdateMouseSensitivity();
    }

    // Aktualizuje interfejs UI wartoœciami z ustawieñ
    void UpdateControlsUI()
    {
        // Wyœwietlanie informacji o trybie biegu
        runMode.text = settingsManager.settingsData.controls.runMode;

        // Wyœwietlanie czu³oœci myszy z kropk¹ jako separator dziesiêtny
        mouseSensitivity.text = settingsManager.settingsData.controls.mouseSensitivity.ToString("F1", CultureInfo.InvariantCulture);

        // Wyœwietlanie informacji o automatycznym prze³adowaniu 
        //autoReload.text = settingsManager.settingsData.controls.autoReload ? "TAK" : "NIE";
    }

    // Zapisuje zmiany w ustawieniach na podstawie wartoœci z UI
    public void SaveControlsSettings()
    {
        // Aktualizacja informacji o trybie biegu
        settingsManager.settingsData.controls.runMode = runMode.text;

        // Konwersja tekstu na float dla czu³oœci myszy z u¿yciem kropki jako separatora dziesiêtnego
        if (float.TryParse(mouseSensitivity.text, NumberStyles.Float, CultureInfo.InvariantCulture, out float sensitivityValue))
        {
            settingsManager.settingsData.controls.mouseSensitivity = sensitivityValue;
        }
        else
        {
            Debug.LogWarning("Nie uda³o siê przekonwertowaæ czu³oœci myszy na float.");
        }

        // Aktualizacja informacji o automatycznym prze³adowaniu
        //settingsManager.settingsData.controls.autoReload = autoReload.text == "TAK";

        // Zapis ustawieñ
        settingsManager.SaveSettings();
    }
}
