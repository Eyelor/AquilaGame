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

        // Ustaw obiekty domyœlnie jako niewidoczne
        if (lineObject != null)
        {
            lineObject.SetActive(false);
        }
        if (additionalTextObject != null)
        {
            additionalTextObject.SetActive(false);
        }
    }

    // Metoda wywo³ywana, gdy kursor najedzie na obiekt
    private void OnMouseEnter()
    {

        if (textMeshPro != null)
        {
            textMeshPro.color = hoverColor;
        }

        // Pokazuje liniê i dodatkowy tekst
        if (lineObject != null)
        {
            lineObject.SetActive(true);
        }
        if (additionalTextObject != null)
        {
            additionalTextObject.SetActive(true);
        }
    }

    // Metoda wywo³ywana, gdy kursor opuœci obiekt
    private void OnMouseExit()
    {
        if (textMeshPro != null)
        {
            textMeshPro.color = originalColor;
        }

        // Ukrywa liniê i dodatkowy tekst
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
