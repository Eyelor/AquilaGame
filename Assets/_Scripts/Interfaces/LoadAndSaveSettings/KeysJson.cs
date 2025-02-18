using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class KeysJson : MonoBehaviour
{
    // Komponenty TextMeshPro przypisane z Inspektora
    // Teksty dla informacji o klawiszach dla poszczególnych opcji
    public TextMeshProUGUI openMap;
    public TextMeshProUGUI placeTag;
    public TextMeshProUGUI swipeLeft;
    public TextMeshProUGUI swipeUp;
    public TextMeshProUGUI swipeRight;
    public TextMeshProUGUI swipeDown;
    public TextMeshProUGUI aim;
    public TextMeshProUGUI attack;
    public TextMeshProUGUI reload;
    public TextMeshProUGUI weapon1;
    public TextMeshProUGUI weapon2;
    public TextMeshProUGUI walk;
    public TextMeshProUGUI run;
    public TextMeshProUGUI interact;
    public TextMeshProUGUI stepForward;
    public TextMeshProUGUI stepBack;
    public TextMeshProUGUI stepLeft;
    public TextMeshProUGUI stepRight;
    public TextMeshProUGUI jump;
    public TextMeshProUGUI openInventory;
    public TextMeshProUGUI changeCardForward;
    public TextMeshProUGUI changeCardBack;
    public TextMeshProUGUI characterEquipment;
    public TextMeshProUGUI shipHold;
    public TextMeshProUGUI characterStatistics;
    public TextMeshProUGUI sortItems;

    // Klasa zarz¹dzaj¹ca ustawieniami
    private SettingsDataSerialization settingsManager;

    private void Start()
    {
        // Znalezienie SettingsDataSerialization i zaktualizowanie UI na podstawie obecnych ustawieñ
        settingsManager = FindObjectOfType<SettingsDataSerialization>();
        UpdateKeysUI();
    }

    private void OnDisable()
    {
        // Zapisanie zmian w ustawieniach do pliku JSON
        SaveKeysSettings();
        GameInputSystem.Instance.SaveBindingsToJson();
	}

    // Aktualizuje interfejs UI wartoœciami z ustawieñ
    void UpdateKeysUI()
    {
        // Wyœwietlanie o klawiszach dla poszczególnych opcji
        openMap.text = GameInputSystem.Instance.GetBindingText(GameInputSystem.Binding.openMap);
        //placeTag.text = settingsManager.settingsData.keys.placeTag;
        //swipeLeft.text = settingsManager.settingsData.keys.swipeLeft;
        //swipeUp.text = settingsManager.settingsData.keys.swipeUp;
        //swipeRight.text = settingsManager.settingsData.keys.swipeRight;
        //swipeDown.text = settingsManager.settingsData.keys.swipeDown;
        //aim.text = settingsManager.settingsData.keys.aim;
        attack.text = GameInputSystem.Instance.GetBindingText(GameInputSystem.Binding.attack);
        //reload.text = settingsManager.settingsData.keys.reload;
        weapon1.text = GameInputSystem.Instance.GetBindingText(GameInputSystem.Binding.weapon1);
        //weapon2.text = settingsManager.settingsData.keys.weapon2;
        //walk.text = settingsManager.settingsData.keys.walk;
        run.text = GameInputSystem.Instance.GetBindingText(GameInputSystem.Binding.run);
        interact.text = GameInputSystem.Instance.GetBindingText(GameInputSystem.Binding.interact);
        stepForward.text = GameInputSystem.Instance.GetBindingText(GameInputSystem.Binding.stepForward);
        stepBack.text = GameInputSystem.Instance.GetBindingText(GameInputSystem.Binding.stepBack);
        stepLeft.text = GameInputSystem.Instance.GetBindingText(GameInputSystem.Binding.stepLeft);
        stepRight.text = GameInputSystem.Instance.GetBindingText(GameInputSystem.Binding.stepRight);
        jump.text = GameInputSystem.Instance.GetBindingText(GameInputSystem.Binding.jump);
        openInventory.text = GameInputSystem.Instance.GetBindingText(GameInputSystem.Binding.openInventory);
        //changeCardForward.text = settingsManager.settingsData.keys.changeCardForward;
        //changeCardBack.text = settingsManager.settingsData.keys.changeCardBack;
        //characterEquipment.text = settingsManager.settingsData.keys.characterEquipment;
        //shipHold.text = settingsManager.settingsData.keys.shipHold;
        //characterStatistics.text = settingsManager.settingsData.keys.characterStatistics;
        //sortItems.text = settingsManager.settingsData.keys.sortItems;
    }

    // Zapisuje zmiany w ustawieniach na podstawie wartoœci z UI
    public void SaveKeysSettings()
    {
        // Aktualizacja informacji o klawiszach dla poszczególnych opcji
        settingsManager.settingsData.keys.openMap = openMap.text;
        //settingsManager.settingsData.keys.placeTag = placeTag.text;
        //settingsManager.settingsData.keys.swipeLeft = swipeLeft.text;
        //settingsManager.settingsData.keys.swipeUp = swipeUp.text;
        //settingsManager.settingsData.keys.swipeRight = swipeRight.text;
        //settingsManager.settingsData.keys.swipeDown = swipeDown.text;
        //settingsManager.settingsData.keys.aim = aim.text;
        settingsManager.settingsData.keys.attack = attack.text;
        //settingsManager.settingsData.keys.reload = reload.text;
        settingsManager.settingsData.keys.weapon1 = weapon1.text;
        //settingsManager.settingsData.keys.weapon2 = weapon2.text;
        //settingsManager.settingsData.keys.walk = walk.text;
        settingsManager.settingsData.keys.run = run.text;
        settingsManager.settingsData.keys.interact = interact.text;
        settingsManager.settingsData.keys.stepForward = stepForward.text;
        settingsManager.settingsData.keys.stepBack = stepBack.text;
        settingsManager.settingsData.keys.stepLeft = stepLeft.text;
        settingsManager.settingsData.keys.stepRight = stepRight.text;
        settingsManager.settingsData.keys.jump = jump.text;
        settingsManager.settingsData.keys.openInventory = openInventory.text;
        //settingsManager.settingsData.keys.changeCardForward = changeCardForward.text;
        //settingsManager.settingsData.keys.changeCardBack = changeCardBack.text;
        //settingsManager.settingsData.keys.characterEquipment = characterEquipment.text;
        //settingsManager.settingsData.keys.shipHold = shipHold.text;
        //settingsManager.settingsData.keys.characterStatistics = characterStatistics.text;
        //settingsManager.settingsData.keys.sortItems = sortItems.text;

        // Zapis ustawieñ
        settingsManager.SaveSettings();
    }
}
