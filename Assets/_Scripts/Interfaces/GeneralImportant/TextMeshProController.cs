using UnityEngine;
using TMPro; // Importowanie przestrzeni nazw TextMeshPro

public class TextMeshProController : MonoBehaviour
{
    public Color hoverColor = Color.red; // Kolor po najechaniu kursorem
    private Color originalColor; // Oryginalny kolor tekstu
    private TextMeshProUGUI textMeshPro; // Referencja do komponentu TextMeshPro

    public GameObject lineObject; // Obiekt linii
    public GameObject additionalTextObject; // Obiekt dodatkowego tekstu

    private void Awake()
    {
        textMeshPro = GetComponent<TextMeshProUGUI>();

        if (textMeshPro != null)
        {
            originalColor = textMeshPro.color; // Zapisz oryginalny kolor
        }

        // Ustaw obiekty domy�lnie jako niewidoczne
        if (lineObject != null)
        {
            lineObject.SetActive(false);
        }
        if (additionalTextObject != null)
        {
            additionalTextObject.SetActive(false);
        }
    }

    // Metoda wywo�ywana, gdy kursor najedzie na obiekt
    private void OnMouseEnter()
    {

        if (textMeshPro != null)
        {
            textMeshPro.color = hoverColor;
        }

        // Pokazuje lini� i dodatkowy tekst
        if (lineObject != null)
        {
            lineObject.SetActive(true);
        }
        if (additionalTextObject != null)
        {
            additionalTextObject.SetActive(true);
        }
    }

    // Metoda wywo�ywana, gdy kursor opu�ci obiekt
    private void OnMouseExit()
    {
        if (textMeshPro != null)
        {
            textMeshPro.color = originalColor;
        }

        // Ukrywa lini� i dodatkowy tekst
        if (lineObject != null)
        {
            lineObject.SetActive(false);
        }
        if (additionalTextObject != null)
        {
            additionalTextObject.SetActive(false);
        }
    }
}
