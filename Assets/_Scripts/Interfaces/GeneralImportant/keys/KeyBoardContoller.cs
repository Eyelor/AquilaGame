using UnityEngine;

public class KeyBoardContoller : MonoBehaviour
{
    public GameObject boardObject; // Obiekt panelu t�a elementu
    public BoxCollider2D boxCollider; // Referencja do komponentu Box Collidera

    private void Awake()
    {
        // Ustaw obiekt domy�lnie jako niewidoczny
        if (boardObject != null)
        {
            boardObject.SetActive(false);
        }
    }

    private void OnMouseOver()
    {
        if (Cursor.visible == false)
        {
            boardObject.SetActive(false);
        } else
        {
            boardObject.SetActive(true);
            if (boxCollider != null)
            {
                boxCollider.enabled = true;
            }
        }
    }

    // Metoda wywo�ywana, gdy kursor najedzie na obiekt
    private void OnMouseEnter()
    {
        // Pokazuje panel je�eli kursor jest aktywny
        if (boardObject != null && Cursor.visible == true)
        {
            boardObject.SetActive(true);
        }
    }

    // Metoda wywo�ywana, gdy kursor opu�ci obiekt
    private void OnMouseExit()
    {
        // Ukrywa panel=
        if (boardObject != null && Cursor.visible == true)
        {
            boardObject.SetActive(false);
        }
    }
}
