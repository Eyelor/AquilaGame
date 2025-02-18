[System.Serializable]
public class SettingsData
{
    public ControlsSettings controls;
    public KeysSettings keys;
    public GraphicSettings graphics;
    public SoundSettings sound;
    public DisplaySettings display;
    public LanguageSettings language;

    // Domy�lne ustawienia, kt�re s�u�� do resetowania
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