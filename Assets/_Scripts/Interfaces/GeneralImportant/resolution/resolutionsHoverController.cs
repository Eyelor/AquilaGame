using TMPro;
using UnityEngine;

public class ResolutionsHoverController : MonoBehaviour
{
    public Color panelColor = Color.black; // Kolor sta³y
    public Color panelHoverColor = Color.yellow; // Kolor po najechaniu

    private TextMeshProUGUI textComponent;
    public ResolutionsController resolutionsController; // Referencja do ResolutionsController

    private void Start()
    {
        textComponent = GetComponent<TextMeshProUGUI>();

        if (resolutionsController == null)
        {
            // Próba znalezienia ResolutionsController w scenie
            resolutionsController = FindObjectOfType<ResolutionsController>();
        }
    }

    // Metody zmieniaj¹ce kolor tekstu przy najechaniu i zjechaniu kursora
    private void OnMouseEnter()
    {
        textComponent.color = panelHoverColor;
    }

    private void OnMouseExit()
    {
        textComponent.color = panelColor;
    }

    // Metoda wywo³ywana po klikniêciu na opcjê
    private void OnMouseDown()
    {
        if (resolutionsController != null)
        {
            resolutionsController.SetSelectedResolution(textComponent.text);
        }

        textComponent.color = panelColor;
    }
}
