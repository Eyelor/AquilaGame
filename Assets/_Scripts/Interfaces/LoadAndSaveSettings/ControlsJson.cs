using TMPro;
using UnityEngine;
using System.Globalization;

public class ControlsJson : MonoBehaviour
{
    // Komponenty TextMeshPro przypisane z Inspektora
    public TextMeshProUGUI mouseSensitivity;    // Tekst dla wy�wietlania czu�o�ci myszy
    public TextMeshProUGUI autoReload;    // Tekst dla wy�wietlania informacji o automatycznym prze�adowaniu 
    public TextMeshProUGUI runMode;    // Tekst dla wy�wietlania informacji o trybie biegu

    // Klasa zarz�dzaj�ca ustawieniami
    private SettingsDataSerialization settingsManager;

    private void Start()
    {
        // Znalezienie SettingsDataSerialization i zaktualizowanie UI na podstawie obecnych ustawie�
        settingsManager = FindObjectOfType<SettingsDataSerialization>();
        UpdateControlsUI();
    }

    private void OnDisable()
    {
        SaveControlsSettings();
        GameInputSystem.Instance.UpdateMouseSensitivity();
    }

    // Aktualizuje interfejs UI warto�ciami z ustawie�
    void UpdateControlsUI()
    {
        // Wy�wietlanie informacji o trybie biegu
        runMode.text = settingsManager.settingsData.controls.runMode;

        // Wy�wietlanie czu�o�ci myszy z kropk� jako separator dziesi�tny
        mouseSensitivity.text = settingsManager.settingsData.controls.mouseSensitivity.ToString("F1", CultureInfo.InvariantCulture);

        // Wy�wietlanie informacji o automatycznym prze�adowaniu 
        //autoReload.text = settingsManager.settingsData.controls.autoReload ? "TAK" : "NIE";
    }

    // Zapisuje zmiany w ustawieniach na podstawie warto�ci z UI
    public void SaveControlsSettings()
    {
        // Aktualizacja informacji o trybie biegu
        settingsManager.settingsData.controls.runMode = runMode.text;

        // Konwersja tekstu na float dla czu�o�ci myszy z u�yciem kropki jako separatora dziesi�tnego
        if (float.TryParse(mouseSensitivity.text, NumberStyles.Float, CultureInfo.InvariantCulture, out float sensitivityValue))
        {
            settingsManager.settingsData.controls.mouseSensitivity = sensitivityValue;
        }
        else
        {
            Debug.LogWarning("Nie uda�o si� przekonwertowa� czu�o�ci myszy na float.");
        }

        // Aktualizacja informacji o automatycznym prze�adowaniu
        //settingsManager.settingsData.controls.autoReload = autoReload.text == "TAK";

        // Zapis ustawie�
        settingsManager.SaveSettings();
    }
}
