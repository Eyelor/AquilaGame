using UnityEngine;

/// <summary>
/// Statyczna instancja podobna do singletona, ale zamiast niszczy� ka�d� now� instancj�, nadpisuje
/// aktualn� instancj�. Jest to przydayne do resetowania stanu i oszcz�dza to robienie tego manualnie.
/// </summary>
public abstract class StaticInstance<T> : MonoBehaviour where T : StaticInstance<T>
{
    public static T Instance { get; internal set; }
    private static int _instanceCount = 0;

    protected virtual void Awake()
    {
        Instance = this as T;
        _instanceCount++;
        Debug.Log($"[StaticInstance] Instance of {typeof(T).Name} created. Total instances: {_instanceCount}");
    }

    protected virtual void OnApplicationQuit()
    {
        Instance = null;
        _instanceCount--;
        Debug.Log($"[StaticInstance] Instance of {typeof(T).Name} destroyed. Remaining instances: {_instanceCount}");
        Destroy(gameObject);
    }

    protected virtual void OnDestroy()
    {
        if (Instance == this as T)
        {
            Instance = null;
            _instanceCount--;
            Debug.Log($"[StaticInstance] {typeof(T).Name} instance destroyed manually. Remaining instances: {_instanceCount}");
        }
    }
}

/// <summary>
/// Transformuje statyczn� instancj� w podstawowoego singletona. Zniszczy ka�d� now� wersj� pozostawiaj�c
/// oryginaln� instancj� nienaruszon�.
/// </summary>
public abstract class Singleton<T> : StaticInstance<T> where T : Singleton<T>
{
    protected override void Awake()
    {
        if (Instance != null && Instance != this as T)
        {
            Debug.LogWarning($"[Singleton] Attempting to create a second instance of {typeof(T).Name}. Destroying the new one.");
            Destroy(gameObject);
        }
        else
        {
            base.Awake();
        }
    }
}

/// <summary>
/// Trwa�a wersja singletona. Przetrwa wszystkie zmiany sceny. Nale�y j� umie�ci� w root obiekcie lub jako
/// potomek obkiektu nadrz�dnego, w kt�rym znajduj� si� tylko klasy PersistentSingleton. Idealna dla klas
/// systemowych, kt�re potrzebuj� stanowych, statycznych danych. Nadaje si� r�wnie� do �r�de� d�wi�ku, gdzie
/// muzyka jest grana na ekranach �adowania, itp.
/// </summary>
public abstract class PersistentSingleton<T> : Singleton<T> where T : PersistentSingleton<T>
{
    protected static string GLOBAL_ROOT_OBJECT_FOR_PERSISTENT_SINGLETONS_NAME = "Systems";

    protected override void Awake()
    {
        base.Awake();
        if (transform.root.name == GLOBAL_ROOT_OBJECT_FOR_PERSISTENT_SINGLETONS_NAME)
        {
            DontDestroyOnLoad(transform.root.gameObject);
            Debug.Log($"[PersistentSingleton] {typeof(T).Name} instance will persist across scenes.");
        }
        else
        {
            Debug.LogWarning($"[PersistentSingleton] {typeof(T).Name} instance is not a child of the global root object for persistent singletons. It may be for testing puropses.");
        }
    }
}
