using UnityEngine;

[CreateAssetMenu(fileName = "DefaultOptionsSettings", menuName = "ScriptableObjects/DefaultOptionsSettings")]
public class OptionsDefaultSettingsSO : ScriptableObject
{
    [SerializeField] private ControlsSettings controlsDefault = new ControlsSettings
    {
        mouseSensitivity = 1.0f,
        //autoReload = true,
        runMode = "PRZYTRZYMAJ"
    };
    [SerializeField] private KeysSettings keysDefault = new KeysSettings
    {
        openMap = "M",
        //placeTag = "Mouse1",
        //swipeLeft = "A",
        //swipeUp = "W",
        //swipeRight = "D",
        //swipeDown = "S",
        //aim = "Mouse1",
        attack = "Space",
        //reload = "R",
        weapon1 = "Q",
        //weapon2 = "Alpha2",
        //walk = "CapsLock",
        run = "Left Shift",
        interact = "E",
        stepForward = "W",
        stepBack = "S",
        stepLeft = "A",
        stepRight = "D",
        jump = "MMB",
        openInventory = "F",
        //changeCardForward = "PageUp",
        //changeCardBack = "PageDown",
        //characterEquipment = "F1",
        //shipHold = "F2",
        //characterStatistics = "F3",
        //sortItems = "R"
        INPUT_SYSTEM_BINDING_OVERRIDES = ""
    };
    [SerializeField] private GraphicSettings graphicsDefault = new GraphicSettings
    {
        verticalSync = false,
        textureQuality = "WYSOKA",
        effectsQuality = "WYSOKA",
        reflectionsQuality = "WYSOKA",
        shadowsQuality = "WYSOKA",
        meshQuality = "WYSOKA",
        shadowBuffer = false,
        flare = true,
        contactShadows = true,
        spatialReflections = true
    };
    [SerializeField] private SoundSettings soundDefault = new SoundSettings
    {
        audioDevice = "STEREO",
        musicVolume = 0.30000001192092898f,
        recordsVolume = 0.4000000059604645f,
        effectsVolume = 0.5f
    };
    [SerializeField] private DisplaySettings displayDefault = new DisplaySettings
    {
        resolution = "1920 x 1080",
        refreshRate = 60,
        brightness = 0.5f,
        contrast = 0.5f,
        gamma = 0.5f,
        displayGuides = false,
        displayHints = true
    };
    [SerializeField] private LanguageSettings languageDefault = new LanguageSettings
    {
        textsLanguage = "POLSKI",
        audioLanguage = "POLSKI",
        displaySubtitles = true
    };

    public DefaultSettings GetDefaultSettings()
    {
        return new DefaultSettings
        {
            controlsDefault = controlsDefault,
            keysDefault = keysDefault,
            graphicsDefault = graphicsDefault,
            soundDefault = soundDefault,
            displayDefault = displayDefault,
            languageDefault = languageDefault
        };
    }
}
