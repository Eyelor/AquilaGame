using TMPro;
using UnityEngine;

public class ResolutionsController : MonoBehaviour
{
    public Color panelColor = Color.black; // Kolor sta³y
    public Color panelHoverColor = Color.yellow; // Kolor po najechaniu

    public GameObject frequencyClose; // G³ówny element wyœwietlaj¹cy aktualn¹ rozdzielczoœæ
    public GameObject frequencyTop; // Górna czêœæ menu rozwijanego
    public GameObject frequencyOpen; // Menu rozwijane

    public TextMeshProUGUI[] dropdownOptions; // Teksty opcji w menu rozwijanym

    private static string[] resolutions = { "1920 x 1080", "1600 x 900", "1366 x 768", "1360 x 768" };
    private int selectedResolutionIndex;

    private TextMeshProUGUI mainResolutionText; // Tekst g³ównej rozdzielczoœci

    [SerializeField] private bool isControllerDisabled = false;

    private void Start()
    {
        if (isControllerDisabled) return;

        if (frequencyClose != null) frequencyClose.SetActive(true);
        if (frequencyOpen != null) frequencyOpen.SetActive(false);
        if (frequencyTop != null) frequencyTop.SetActive(false);

        mainResolutionText = GetComponent<TextMeshProUGUI>();
        findIndex();

        // Ustawienie pocz¹tkowej rozdzielczoœci
        if (mainResolutionText != null)
        {
            mainResolutionText.text = resolutions[selectedResolutionIndex];
        }

        UpdateDropdownOptions();
    }

    void findIndex()
    {
        string nextValue = mainResolutionText.text.Trim();

        // ZnajdŸ indeks odpowiadaj¹cy pocz¹tkowej wartoœci
        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i] == nextValue)
            {
                selectedResolutionIndex = i;
                break;
            }
        }
    }

    // Metoda aktualizuj¹ca opcje w menu rozwijanym
    public void UpdateDropdownOptions()
    {
        int dropdownOptionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            if (i != selectedResolutionIndex)
            {
                if (dropdownOptionIndex < dropdownOptions.Length)
                {
                    dropdownOptions[dropdownOptionIndex].text = resolutions[i];
                    dropdownOptionIndex++;
                }
            }
        }

        // Ukrycie nadmiarowych opcji, jeœli jest ich mniej ni¿ miejsc w menu
        for (int i = dropdownOptionIndex; i < dropdownOptions.Length; i++)
        {
            dropdownOptions[i].text = "";
        }
    }

    // Metody zmieniaj¹ce kolor tekstu przy najechaniu i zjechaniu kursora
    private void OnMouseEnter()
    {
        if (mainResolutionText != null)
        {
            mainResolutionText.color = panelHoverColor;
        }
    }

    private void OnMouseExit()
    {
        if (mainResolutionText != null)
        {
            mainResolutionText.color = panelColor;
        }
    }

    // Metoda wywo³ywana po klikniêciu na g³ówny element (otwieranie/zamykanie menu)
    private void OnMouseDown()
    {
        if (isControllerDisabled) return;

        if (frequencyClose != null && frequencyClose.activeSelf)
        {
            frequencyClose.SetActive(false);
            if (frequencyOpen != null) frequencyOpen.SetActive(true);
            if (frequencyTop != null) frequencyTop.SetActive(true);
        }
        else
        {
            frequencyClose.SetActive(true);
            if (frequencyOpen != null) frequencyOpen.SetActive(false);
            if (frequencyTop != null) frequencyTop.SetActive(false);
        }
    }

    // Metoda ustawiaj¹ca wybran¹ rozdzielczoœæ
    public void SetSelectedResolution(string resolution)
    {
        // Znalezienie indeksu wybranej rozdzielczoœci
        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i] == resolution)
            {
                selectedResolutionIndex = i;
                break;
            }
        }

        // Aktualizacja tekstu g³ównej rozdzielczoœci
        if (mainResolutionText != null)
        {
            mainResolutionText.text = resolution;
        }

        // Zamkniêcie menu rozwijanego
        if (frequencyClose != null) frequencyClose.SetActive(true);
        if (frequencyOpen != null) frequencyOpen.SetActive(false);
        if (frequencyTop != null) frequencyTop.SetActive(false);

        // Aktualizacja opcji w menu rozwijanym
        UpdateDropdownOptions();
    }
}
