using UnityEngine;
using TMPro;

public class keysRebinding : MonoBehaviour
{
    public KeyCode nextKey;
    public GameObject leftMouse;
    public GameObject rightMouse;
    public GameObject scrollMouse;
    public GameObject boardObject; // Obiekt planszy t�a
    public GameInputSystem.Binding key; // Przypisany klawisz

    private bool isRebinding = false;  // Flaga, czy u�ytkownik aktualnie zmienia klawisz
    private TextMeshProUGUI keyText; // Referencja do komponentu TextMeshPro
    private BoxCollider2D boxCollider; // Referencja do komponentu Box Collidera
    private bool mouseClickIgnored = false; // Flaga, by ignorowa� pierwszy klik mysz�

    private bool ifMouse = true;

    private void Awake()
    {
        keyText = GetComponent<TextMeshProUGUI>();
        boxCollider = GetComponent<BoxCollider2D>();
        if (leftMouse.activeSelf) leftMouse.SetActive(false);
        if (rightMouse.activeSelf) rightMouse.SetActive(false);
        if (scrollMouse.activeSelf) scrollMouse.SetActive(false);
    }

    void Start()
    {
        // Wy�wietla pocz�tkowo przypisany klawisz
        // keyText.text = key.ToString();
        if (keyText.text == "LMB")
        {
            GetComponent<TextMeshProUGUI>().enabled = false;
            leftMouse.SetActive(true);
        }
        else if (keyText.text == "RMB")
        {
            GetComponent<TextMeshProUGUI>().enabled = false;
            rightMouse.SetActive(true);
        }
        else if (keyText.text == "MMB")
        {
            GetComponent<TextMeshProUGUI>().enabled = false;
            scrollMouse.SetActive(true);
        }
    }

    private void OnMouseDown()
    {

        if (!isRebinding)
        {
            // Debug.Log("Rozpocz�cie zmiany klawisza");
            isRebinding = true;  // Rozpoczynamy proces zmiany klawisza
            if (leftMouse.activeSelf) leftMouse.SetActive(false);
            if (rightMouse.activeSelf) rightMouse.SetActive(false);
            if (scrollMouse.activeSelf) scrollMouse.SetActive(false);
            GetComponent<TextMeshProUGUI>().enabled = true;

            keyText.text = "Wybierz klawisz...";  // Aktualizacja tekstu w TextMeshPro

            mouseClickIgnored = true; // Ignorujemy pierwszy klik mysz� (kt�ry wywo�a� zmian�)

            // Ukryj kursor i zablokuj jego ruch
            GameInputSystem.Instance.SetForceCursorLockedAndInvisible(true);
        }

        GameInputSystem.Instance.RebindBinding(key, () => {
            keyText.text = GameInputSystem.Instance.GetBindingText(key);  // Aktualizacja UI w TextMeshPro
        });
    }

    // Update jest wywo�ywany co ka�d� klatk�
    void Update()
    {
        while (ifMouse)
        {
            if (keyText.text == "LMB")
            {
                GetComponent<TextMeshProUGUI>().enabled = false;
                leftMouse.SetActive(true);
            }
            else if (keyText.text == "RMB")
            {
                GetComponent<TextMeshProUGUI>().enabled = false;
                rightMouse.SetActive(true);
            }
            else if (keyText.text == "MMB")
            {
                GetComponent<TextMeshProUGUI>().enabled = false;
                scrollMouse.SetActive(true);
            }
            ifMouse = false;
        }

        if (isRebinding)
        {
            // Sprawdzenie, kt�ry klawisz zosta� naci�ni�ty
            foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(keyCode))
                {
                    // Je�eli to pierwszy klik mysz� (LMB), ignorujemy go
                    if (keyCode == KeyCode.Mouse0 && mouseClickIgnored)
                    {
                        mouseClickIgnored = false; // Ignorujemy tylko pierwszy klik mysz�
                        continue; // Pomijamy pierwszy klik LMB
                    }

                    // Przypisanie nowego klawisza (��cznie z LMB)
                    nextKey = keyCode;
                    //keyText.text = nextKey.ToString();  // Aktualizacja UI w TextMeshPro

                    if (keyCode == KeyCode.Mouse0)
                    {
                        GetComponent<TextMeshProUGUI>().enabled = false;
                        leftMouse.SetActive(true);
                    }
                    else if (keyCode == KeyCode.Mouse1)
                    {
                        GetComponent<TextMeshProUGUI>().enabled = false;
                        rightMouse.SetActive(true);
                    }
                    else if (keyCode == KeyCode.Mouse2)
                    {
                        GetComponent<TextMeshProUGUI>().enabled = false;
                        scrollMouse.SetActive(true);
                    }

                    // Poka� kursor po zako�czeniu przypisania
                    GameInputSystem.Instance.SetForceCursorLockedAndInvisible(false);

                    // Debug.Log("Nowy klawisz: " + key);
                    isRebinding = false;  // Zako�czenie procesu zmiany klawisza
                    break;
                }
            }
        }
    }

    private void OnMouseOver()
    {
        if (Cursor.visible == false)
        {
            boxCollider.enabled = false;
        }
    }
    // Metoda wywo�ywana, gdy kursor najedzie na obiekt
    private void OnMouseEnter()
    {
        // Pokazuje lini� i dodatkowy tekst
        if (boardObject != null && Cursor.visible == true)
        {
            boardObject.SetActive(true);
        }
    }

    private void OnMouseExit()
    {
        // Pokazuje lini� i dodatkowy tekst
        if (boardObject != null)
        {
            boardObject.SetActive(false);
        }
    }
}
