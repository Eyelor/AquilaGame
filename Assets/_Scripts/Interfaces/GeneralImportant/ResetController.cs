using TMPro;
using UnityEngine;

public class ResetController : MonoBehaviour
{
    public Color hoverColor = Color.red; // Kolor po najechaniu kursorem
    private Color originalColor; // Oryginalny kolor tekstu
    private TextMeshProUGUI textMeshPro; // Referencja do komponentu TextMeshPro

    public GameObject plateObject; // Obiekt tabliczki pod
    public GameObject plateHoverObject; // Obiekt tabliczki nad
    public GameObject additionalTextObject; // Obiekt dodatkowego tekstu

    private void Awake()
    {
        textMeshPro = GetComponent<TextMeshProUGUI>();

        if (textMeshPro != null)
        {
            originalColor = textMeshPro.color; // Zapisz oryginalny kolor
        }

        // Ustaw obiekty domyœlnie jako niewidoczne
        if (plateHoverObject != null)
        {
            plateHoverObject.SetActive(false);
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

        // Pokazuje drug¹ tabliczke zamiast pierwszej i dodatkowy tekst
        if (plateObject != null)
        {
            plateObject.SetActive(false);
        }
        if (plateHoverObject != null)
        {
            plateHoverObject.SetActive(true);
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

        // Ukrywa drug¹ tabliczkê i dodatkowy tekst
        if (plateHoverObject != null)
        {
            plateHoverObject.SetActive(false);
        }
        if (additionalTextObject != null)
        {
            additionalTextObject.SetActive(false);
        }
        if (plateObject != null)
        {
            plateObject.SetActive(true);
        }
    }
}
