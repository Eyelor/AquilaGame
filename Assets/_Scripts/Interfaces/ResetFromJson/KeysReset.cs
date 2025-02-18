using System.Globalization;
using TMPro;
using UnityEngine;

public class KeysReset : MonoBehaviour
{
    // Komponenty TextMeshPro przypisane z Inspektora
    // Teksty dla informacji o klawiszach dla poszczególnych opcji
    public GameObject openMap;
    public GameObject placeTag;
    public GameObject swipeLeft;
    public GameObject swipeUp;
    public GameObject swipeRight;
    public GameObject swipeDown;
    public GameObject aim;
    public GameObject attack;
    public GameObject reload;
    public GameObject weapon1;
    public GameObject weapon2;
    public GameObject walk;
    public GameObject run;
    public GameObject interact;
    public GameObject stepForward;
    public GameObject stepBack;
    public GameObject stepLeft;
    public GameObject stepRight;
    public GameObject jump;
    public GameObject openInventory;
    public GameObject changeCardForward;
    public GameObject changeCardBack;
    public GameObject characterEquipment;
    public GameObject shipHold;
    public GameObject characterStatistics;
    public GameObject sortItems;

    // Klasa zarz¹dzaj¹ca ustawieniami
    private SettingsDataSerialization settingsManager;

    void Start()
    {
        // Znalezienie SettingsDataSerialization i zaktualizowanie UI na podstawie obecnych ustawieñ
        settingsManager = FindObjectOfType<SettingsDataSerialization>();
    }

    private void OnMouseDown()
    {
        if (!AudioSystem.Instance.audioSources.effectsAudioSource.isPlaying)
        {
            AudioSystem.Instance.PlaySound(AudioSystem.Instance.audioSources.effectsAudioSource, AudioSystem.Instance.buttonForwardSound, false);
        }

        GameInputSystem.Instance.ResetBindings();

        // Debug.Log("KeysReset");
        // Wyœwietlanie o klawiszach dla poszczególnych opcji
        openMap.GetComponent<TextMeshProUGUI>().text = settingsManager.settingsData.defaults.keysDefault.openMap; CheckMouseInput(openMap);
        //placeTag.GetComponent<TextMeshProUGUI>().text = settingsManager.settingsData.defaults.keysDefault.placeTag; CheckMouseInput(placeTag);
        //swipeLeft.GetComponent<TextMeshProUGUI>().text = settingsManager.settingsData.defaults.keysDefault.swipeLeft; CheckMouseInput(swipeLeft);
        //swipeUp.GetComponent<TextMeshProUGUI>().text = settingsManager.settingsData.defaults.keysDefault.swipeUp; CheckMouseInput(swipeUp);
        //swipeRight.GetComponent<TextMeshProUGUI>().text = settingsManager.settingsData.defaults.keysDefault.swipeRight; CheckMouseInput(swipeRight);
        //swipeDown.GetComponent<TextMeshProUGUI>().text = settingsManager.settingsData.defaults.keysDefault.swipeDown; CheckMouseInput(swipeDown);
        //aim.GetComponent<TextMeshProUGUI>().text = settingsManager.settingsData.defaults.keysDefault.aim; CheckMouseInput(aim);
        attack.GetComponent<TextMeshProUGUI>().text = settingsManager.settingsData.defaults.keysDefault.attack; CheckMouseInput(attack);
        //reload.GetComponent<TextMeshProUGUI>().text = settingsManager.settingsData.defaults.keysDefault.reload; CheckMouseInput(reload);
        weapon1.GetComponent<TextMeshProUGUI>().text = settingsManager.settingsData.defaults.keysDefault.weapon1; CheckMouseInput(weapon1);
        //weapon2.GetComponent<TextMeshProUGUI>().text = settingsManager.settingsData.defaults.keysDefault.weapon2; CheckMouseInput(weapon2);
        //walk.GetComponent<TextMeshProUGUI>().text = settingsManager.settingsData.defaults.keysDefault.walk; CheckMouseInput(walk);
        run.GetComponent<TextMeshProUGUI>().text = settingsManager.settingsData.defaults.keysDefault.run; CheckMouseInput(run);
        interact.GetComponent<TextMeshProUGUI>().text = settingsManager.settingsData.defaults.keysDefault.interact; CheckMouseInput(interact);
        stepForward.GetComponent<TextMeshProUGUI>().text = settingsManager.settingsData.defaults.keysDefault.stepForward; CheckMouseInput(stepForward);
        stepBack.GetComponent<TextMeshProUGUI>().text = settingsManager.settingsData.defaults.keysDefault.stepBack; CheckMouseInput(stepBack);
        stepLeft.GetComponent<TextMeshProUGUI>().text = settingsManager.settingsData.defaults.keysDefault.stepLeft; CheckMouseInput(stepLeft);
        stepRight.GetComponent<TextMeshProUGUI>().text = settingsManager.settingsData.defaults.keysDefault.stepRight; CheckMouseInput(stepRight);
        jump.GetComponent<TextMeshProUGUI>().text = settingsManager.settingsData.defaults.keysDefault.jump; CheckMouseInput(jump);
        openInventory.GetComponent<TextMeshProUGUI>().text = settingsManager.settingsData.defaults.keysDefault.openInventory; CheckMouseInput(openInventory);
        //changeCardForward.GetComponent<TextMeshProUGUI>().text = settingsManager.settingsData.defaults.keysDefault.changeCardForward; CheckMouseInput(changeCardForward);
        //changeCardBack.GetComponent<TextMeshProUGUI>().text = settingsManager.settingsData.defaults.keysDefault.changeCardBack; CheckMouseInput(changeCardBack);
        //characterEquipment.GetComponent<TextMeshProUGUI>().text = settingsManager.settingsData.defaults.keysDefault.characterEquipment; CheckMouseInput(characterEquipment);
        //shipHold.GetComponent<TextMeshProUGUI>().text = settingsManager.settingsData.defaults.keysDefault.shipHold; CheckMouseInput(shipHold);
        //characterStatistics.GetComponent<TextMeshProUGUI>().text = settingsManager.settingsData.defaults.keysDefault.characterStatistics; CheckMouseInput(characterStatistics);
        //sortItems.GetComponent<TextMeshProUGUI>().text = settingsManager.settingsData.defaults.keysDefault.sortItems; CheckMouseInput(sortItems);

    }

    public void CheckMouseInput(GameObject keyObject)
    {
        // Pobranie komponentu TextMeshProUGUI z obiektu GameObject
        TextMeshProUGUI textComponent = keyObject.GetComponent<TextMeshProUGUI>();

        if (textComponent == null)
        {
            Debug.LogError("Brak komponentu TextMeshProUGUI w obiekcie: " + keyObject.name);
            return;
        }

        // Znalezienie dzieci na podstawie ich nazw
        GameObject lpm = keyObject.transform.Find("LPM")?.gameObject; // LPM
        GameObject ppm = keyObject.transform.Find("PPM")?.gameObject; // PPM
        GameObject spm = keyObject.transform.Find("SPM")?.gameObject; // SPM

        if (lpm == null || ppm == null || spm == null)
        {
            Debug.LogError("Nie znaleziono jednego z obiektów LPM, PPM lub SPM dla " + keyObject.name);
            return;
        }

        // Sprawdzenie tekstu i odpowiednie dzia³ania
        if (textComponent.text == "LMB")
        {
            // Dezktywacja samego TextMeshProUGUI jeœli jest aktywny
            if (textComponent.enabled == true) textComponent.enabled = false;

            // Aktywacja LPM, dezaktywacja pozosta³ych
            lpm.SetActive(true);
            ppm.SetActive(false);
            spm.SetActive(false);
        }
        else if (textComponent.text == "RMB")
        {
            // Dezktywacja samego TextMeshProUGUI jeœli jest aktywny
            if (textComponent.enabled == true) textComponent.enabled = false;

            // Aktywacja PPM, dezaktywacja pozosta³ych
            ppm.SetActive(true);
            lpm.SetActive(false);
            spm.SetActive(false);
        }
        else if (textComponent.text == "MMB")
        {
            // Dezktywacja samego TextMeshProUGUI jeœli jest aktywny
            if (textComponent.enabled == true) textComponent.enabled = false;

            // Aktywacja SPM, dezaktywacja pozosta³ych
            spm.SetActive(true);
            lpm.SetActive(false);
            ppm.SetActive(false);
        }
        else
        {
            // Dezaktywacja wszystkich przycisków myszki
            if (lpm.activeSelf) lpm.SetActive(false);
            if (ppm.activeSelf) ppm.SetActive(false);
            if (spm.activeSelf) spm.SetActive(false);

            // Aktywacja samego TextMeshProUGUI jeœli nie jest aktywny
            if (textComponent.enabled == false) textComponent.enabled = true;
        }
    }

}
