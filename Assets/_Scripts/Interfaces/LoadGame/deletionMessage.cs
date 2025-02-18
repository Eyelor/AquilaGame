using TMPro;
using UnityEngine;

public class DeletionMessage : MonoBehaviour
{
    public Color hoverColor = Color.red; // Kolor po najechaniu kursorem
    private Color originalColor; // Oryginalny kolor tekstu
    private TextMeshProUGUI textMeshPro; // Referencja do komponentu TextMeshPro

    public GameObject lineObject; // Obiekt linii
    public GameObject deletingTextMessage; // Obiekt dodatkowego tekstu
    public TextMeshProUGUI nameOfSave; // Referencja do nazwy zapisu gry

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
        if (deletingTextMessage != null)
        {
            deletingTextMessage.SetActive(false);
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
        if (deletingTextMessage != null)
        {
            // Pobierz komponent TextMeshProUGUI z obiektu deletingTextMessage
            TextMeshProUGUI deletingTextComponent = deletingTextMessage.GetComponent<TextMeshProUGUI>();

            if (deletingTextComponent != null && nameOfSave != null)
            {
                // Dodaj tekst z nameOfSave do deletingTextMessage
                deletingTextComponent.text = "usuñ zapis " + nameOfSave.text.ToLower();
            }

            deletingTextMessage.SetActive(true);
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
        if (deletingTextMessage != null)
        {
            deletingTextMessage.SetActive(false);
        }
    }
}
