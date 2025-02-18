[System.Serializable]
public class SettingsData
{
    public ControlsSettings controls;
    public KeysSettings keys;
    public GraphicSettings graphics;
    public SoundSettings sound;
    public DisplaySettings display;
    public LanguageSettings language;

    // Domyœlne ustawienia, które s³u¿¹ do resetowania
    public DefaultSettings defaults;
}

[System.Serializable]
public class DefaultSettings
{
    public ControlsSettings controlsDefault;
    public KeysSettings keysDefault;
    public GraphicSettings graphicsDefault;
    public SoundSettings soundDefault;
    public DisplaySettings displayDefault;
    public LanguageSettings languageDefault;
}