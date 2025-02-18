using TMPro;
using UnityEngine;

public class ResolutionsController : MonoBehaviour
{
    public Color panelColor = Color.black; // Kolor sta�y
    public Color panelHoverColor = Color.yellow; // Kolor po najechaniu

    public GameObject frequencyClose; // G��wny element wy�wietlaj�cy aktualn� rozdzielczo��
    public GameObject frequencyTop; // G�rna cz�� menu rozwijanego
    public GameObject frequencyOpen; // Menu rozwijane

    public TextMeshProUGUI[] dropdownOptions; // Teksty opcji w menu rozwijanym

    private static string[] resolutions = { "1920 x 1080", "1600 x 900", "1366 x 768", "1360 x 768" };
    private int selectedResolutionIndex;

    private TextMeshProUGUI mainResolutionText; // Tekst g��wnej rozdzielczo�ci

    [SerializeField] private bool isControllerDisabled = false;

    private void Start()
    {
        if (isControllerDisabled) return;

        if (frequencyClose != null) frequencyClose.SetActive(true);
        if (frequencyOpen != null) frequencyOpen.SetActive(false);
        if (frequencyTop != null) frequencyTop.SetActive(false);

        mainResolutionText = GetComponent<TextMeshProUGUI>();
        findIndex();

        // Ustawienie pocz�tkowej rozdzielczo�ci
        if (mainResolutionText != null)
        {
            mainResolutionText.text = resolutions[selectedResolutionIndex];
        }

        UpdateDropdownOptions();
    }

    void findIndex()
    {
        string nextValue = mainResolutionText.text.Trim();

        // Znajd� indeks odpowiadaj�cy pocz�tkowej warto�ci
        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i] == nextValue)
            {
                selectedResolutionIndex = i;
                break;
            }
        }
    }

    // Metoda aktualizuj�ca opcje w menu rozwijanym
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

        // Ukrycie nadmiarowych opcji, je�li jest ich mniej ni� miejsc w menu
        for (int i = dropdownOptionIndex; i < dropdownOptions.Length; i++)
        {
            dropdownOptions[i].text = "";
        }
    }

    // Metody zmieniaj�ce kolor tekstu przy najechaniu i zjechaniu kursora
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

    // Metoda wywo�ywana po klikni�ciu na g��wny element (otwieranie/zamykanie menu)
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

    // Metoda ustawiaj�ca wybran� rozdzielczo��
    public void SetSelectedResolution(string resolution)
    {
        // Znalezienie indeksu wybranej rozdzielczo�ci
        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i] == resolution)
            {
                selectedResolutionIndex = i;
                break;
            }
        }

        // Aktualizacja tekstu g��wnej rozdzielczo�ci
        if (mainResolutionText != null)
        {
            mainResolutionText.text = resolution;
        }

        // Zamkni�cie menu rozwijanego
        if (frequencyClose != null) frequencyClose.SetActive(true);
        if (frequencyOpen != null) frequencyOpen.SetActive(false);
        if (frequencyTop != null) frequencyTop.SetActive(false);

        // Aktualizacja opcji w menu rozwijanym
        UpdateDropdownOptions();
    }
}
