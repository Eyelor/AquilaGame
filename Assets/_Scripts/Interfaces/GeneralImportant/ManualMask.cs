using UnityEngine;

public class ManualMask : MonoBehaviour
{
    public RectTransform viewport; // Obiekt Viewport, który definiuje widoczny obszar
    public RectTransform content;  // Obiekt Content, który zawiera elementy do przewijania

    void Update()
    {
        MaskContent();
    }

    void MaskContent()
    {
        // Pêtla po wszystkich elementach wewn¹trz kontenera Content
        foreach (RectTransform child in content)
        {
            // Sprawdzamy, czy co najmniej 40% elementu jest w Viewport
            if (IsAtLeast40PercentVisible(child))
            {
                // Co najmniej 40% elementu jest w Viewport, ustawiamy widocznoœæ
                SetElementActive(child, true);
            }
            else
            {
                // Wiêcej ni¿ 60% elementu jest poza Viewport, ukrywamy go
                SetElementActive(child, false);
            }
        }
    }

    bool IsAtLeast40PercentVisible(RectTransform element)
    {
        // Pobieramy naro¿niki Viewport i elementu
        Vector3[] viewportCorners = new Vector3[4];
        viewport.GetWorldCorners(viewportCorners);

        Vector3[] elementCorners = new Vector3[4];
        element.GetWorldCorners(elementCorners);

        // Wysokoœæ elementu
        float elementHeight = elementCorners[1].y - elementCorners[0].y;
        // Obliczamy wysokoœæ widoczn¹ w Viewport
        float visibleHeight = Mathf.Min(viewportCorners[1].y, elementCorners[1].y) - Mathf.Max(viewportCorners[0].y, elementCorners[0].y);

        // Obliczamy procent widocznej wysokoœci
        float visiblePercentage = visibleHeight / elementHeight;

        // Sprawdzamy, czy widoczna czêœæ stanowi co najmniej 40% wysokoœci elementu
        return visiblePercentage >= 0.4f;
    }

    void SetElementActive(RectTransform element, bool active)
    {
        // Ustawiamy aktywnoœæ obiektu na podstawie flagi "active"
        element.gameObject.SetActive(active);
        if (element.GetChild(0).gameObject.name == "opcja_hover" && active == false)
        {
            element.GetChild(0).gameObject.SetActive(false);
        }
    }
}
