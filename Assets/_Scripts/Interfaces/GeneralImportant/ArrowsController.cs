using TMPro;
using UnityEngine;

public class ArrowsController : MonoBehaviour
{
    public enum ArrowType { Left, Right } // Okre�lenie typu strza�ki
    public enum TableType { PercentageValues, AudioDevices, Languages, YesOrNo, CloseOpen, Quality, Frequencies, MouseSensitivity, RunningMode } // Okre�lenie typu tablicy

    public ArrowType arrowType; // Ustawianie typu strza�ki w inspektorze
    public TableType tableType; // Ustawianie typu tablicy w inspektorze

    public Color arrowColor = Color.black; // Kolor sta�y
    public Color arrowHoverColor = Color.grey; // Kolor po najechaniu
    public Color arrowNonactiveColor = Color.gray; // Kolor nieaktywno�ci

    // Tekst wy�wietlaj�cy warto��
    public TextMeshProUGUI valueText;

    // Tablice do wyboru
    private string[] percentageValues = { "0%", "10%", "20%", "30%", "40%", "50%", "60%", "70%", "80%", "90%", "100%" };
    private string[] audioDevices = { "STEREO", "S�UCHAWKI" };
    private string[] languages = { "POLSKI" };
    private string[] yesOrNo = { "TAK", "NIE"};
    private string[] closeOpen = { "WY��CZ", "W��CZ" };
    private string[] quality = { "NISKA", "�REDNIA", "WYSOKA" };
    private string[] frequencies = { "60 Hz", "75 Hz", "120 Hz", "144 Hz", "165 Hz", "240 Hz", "360 Hz", "Nieograniczone" };
    private string[] mouseSensitivity = { "0.1", "0.2", "0.3", "0.4", "0.5", "0.6", "0.7", "0.8", "0.9", "1.0", "1.1", "1.2", "1.3", "1.4", "1.5", "1.6", "1.7", "1.8", "1.9", "2.0"};
    private string[] runningMode = { "PRZYTRZYMAJ", "PRZE��CZ" };

    private int currentIndex;
    private string[] goodTable;

    void Start()
    {
        SelectTable(); // Wybierz odpowiedni� tablic� na podstawie nameOfTable
        findIndex(); // Znajd� indeks pocz�tkowy
        UpdateUI(); // Zaktualizuj interfejs u�ytkownika
    }

    void SelectTable()
    {
        // Wybierz tablic� na podstawie warto�ci tableType
        switch (tableType)
        {
            case TableType.PercentageValues:
                goodTable = percentageValues;
                break;
            case TableType.AudioDevices:
                goodTable = audioDevices;
                break;
            case TableType.Languages:
                goodTable = languages;
                break;
            case TableType.YesOrNo:
                goodTable = yesOrNo;
                break;
            case TableType.CloseOpen:
                goodTable = closeOpen;
                break;
            case TableType.Quality:
                goodTable = quality;
                break;
            case TableType.Frequencies:
                goodTable = frequencies;
                break;
            case TableType.MouseSensitivity:
                goodTable = mouseSensitivity;
                break;
            case TableType.RunningMode:
                goodTable = runningMode;
                break;
            default:
                Debug.LogWarning("Nieznany typ tablicy: " + tableType);
                goodTable = percentageValues; // Domy�lnie ustaw na percentageValues
                break;
        }
    }

    private void Update()
    {
        findIndex();

        if (arrowType == ArrowType.Left && currentIndex >= 1
            && GetComponent<TextMeshProUGUI>().color == arrowNonactiveColor)
        {
            GetComponent<TextMeshProUGUI>().color = arrowColor;
            GetComponent<BoxCollider2D>().enabled = true;
        } 
        else if (arrowType == ArrowType.Left && currentIndex == 0
            && GetComponent<TextMeshProUGUI>().color == arrowColor)
        {
            GetComponent<TextMeshProUGUI>().color = arrowNonactiveColor;
            GetComponent<BoxCollider2D>().enabled = false;
        }

        if (arrowType == ArrowType.Right && currentIndex <= goodTable.Length - 2
            && GetComponent<TextMeshProUGUI>().color == arrowNonactiveColor)
        {
            GetComponent<TextMeshProUGUI>().color = arrowColor;
            GetComponent<BoxCollider2D>().enabled = true;
        }
        else if (arrowType == ArrowType.Right && currentIndex == goodTable.Length - 1
            && GetComponent<TextMeshProUGUI>().color == arrowColor)
        {
            GetComponent<TextMeshProUGUI>().color = arrowNonactiveColor;
            GetComponent<BoxCollider2D>().enabled = false;
        }
    }

    void findIndex()
    {
        string nextValue = valueText.text.Trim();

        // Znajd� indeks odpowiadaj�cy pocz�tkowej warto�ci
        for (int i = 0; i < goodTable.Length; i++)
        {
            if (goodTable[i] == nextValue)
            {
                currentIndex = i;
                break;
            }
        }
    }
    // Metoda aktualizuj�ca interfejs u�ytkownika
    void UpdateUI()
    {
        // Ustawienie tekstu
        valueText.text = goodTable[currentIndex];

        // Aktualizacja stanu strza�ek w zale�no�ci od typu
        if (arrowType == ArrowType.Left)
        {
            if (currentIndex > 0)
            {
                GetComponent<TextMeshProUGUI>().color = arrowColor;
                GetComponent<BoxCollider2D>().enabled = true;
            }
            else
            {
                GetComponent<TextMeshProUGUI>().color = arrowNonactiveColor;
                GetComponent<BoxCollider2D>().enabled = false;
            }
        }
        else if (arrowType == ArrowType.Right)
        {
            
            if (currentIndex < goodTable.Length - 1)
            {
                GetComponent<TextMeshProUGUI>().color = arrowColor;
                GetComponent<BoxCollider2D>().enabled = true;
            }
            else
            {
                GetComponent<TextMeshProUGUI>().color = arrowNonactiveColor;
                GetComponent<BoxCollider2D>().enabled = false;
            }
        }
        
    }

    // Metoda wywo�ywana, gdy kursor najedzie na obiekt
    private void OnMouseEnter()
    {
        if (arrowType == ArrowType.Left && currentIndex > 0)
        {
            GetComponent<TextMeshProUGUI>().color = arrowHoverColor;
        }
        else if (arrowType == ArrowType.Right && currentIndex < goodTable.Length - 1)
        {
            GetComponent<TextMeshProUGUI>().color = arrowHoverColor;
        }
    }

    // Metoda wywo�ywana, gdy kursor opu�ci obiekt
    private void OnMouseExit()
    {
        if (arrowType == ArrowType.Left && currentIndex > 0)
        {
            GetComponent<TextMeshProUGUI>().color = arrowColor;
        }
        else if (arrowType == ArrowType.Right && currentIndex < goodTable.Length - 1)
        {
            GetComponent<TextMeshProUGUI>().color = arrowColor;
        }
    }
    // Metoda wywo�ywana, gdy obiekt zostanie klikni�ty
    private void OnMouseDown()
    {
        if (arrowType == ArrowType.Left && currentIndex > 0)
        {
            currentIndex--;
            UpdateUI();
            if (currentIndex > 0)
            {
                GetComponent<TextMeshProUGUI>().color = arrowHoverColor;
            }
        }
        else if (arrowType == ArrowType.Right && currentIndex < goodTable.Length - 1)
        {
            currentIndex++;
            UpdateUI();
            if (currentIndex < goodTable.Length - 1)
            {
                GetComponent<TextMeshProUGUI>().color = arrowHoverColor;
            }
        }
    }
}
