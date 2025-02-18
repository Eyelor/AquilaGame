using UnityEngine;

public class ManualMaskKeys : MonoBehaviour
{
    public RectTransform content;

    private int currentElementIndex = 0;
    private float nextThreshold = 135f;
    private float previousThreshold = 20f;
    private int visibleCount = 5;
    private float lowerThreshold = 0f;

    private float borderValue1 = 115f;

    void Start()
    {
        for (int i = 0; i < content.childCount; i++)
        {
            SetElementActive(i, i < visibleCount);
        }
    }

    void Update()
    {
        CheckAndDeactivateHigherElements();
        CheckAndActivateHigherElements();
        CheckAndActivateLowerElements();
        CheckAndDeactivateLowerElements();
    }

    void CheckAndDeactivateHigherElements()
    {
        float contentTop = content.anchoredPosition.y;

        if (contentTop >= nextThreshold && currentElementIndex < content.childCount)
        {
            SetElementActive(currentElementIndex, false);

            nextThreshold += borderValue1;
            previousThreshold += borderValue1;

            currentElementIndex++;
        }
    }

    void CheckAndActivateHigherElements()
    {
        float contentTop = content.anchoredPosition.y;

        if (contentTop < previousThreshold && currentElementIndex > 0)
        {
            currentElementIndex--;
            SetElementActive(currentElementIndex, true);

            nextThreshold -= borderValue1;
            previousThreshold -= borderValue1;
        }
    }

    void CheckAndActivateLowerElements()
    {
        float contentBottom = content.anchoredPosition.y;

        if (contentBottom >= lowerThreshold && visibleCount < content.childCount)
        {
            SetElementActive(visibleCount, true);

            lowerThreshold += borderValue1;

            visibleCount++;
        }
    }

    void CheckAndDeactivateLowerElements()
    {
        // Pobieramy doln� pozycj� contentu
        float contentBottom = content.anchoredPosition.y;

        // Dezaktywacja dolnych element�w przy scrollowaniu w d�
        if (contentBottom < lowerThreshold - borderValue1 && visibleCount > 5)
        {
            // Zmniejszamy liczb� widocznych element�w
            visibleCount--;

            // Dezaktywujemy dolny element
            SetElementActive(visibleCount, false);

            // Przesuwamy pr�g dla dezaktywacji kolejnych element�w
            lowerThreshold -= borderValue1;
        }
    }

    void SetElementActive(int index, bool active)
    {
        if (index >= 0 && index < content.childCount)
        {
            Transform element = content.GetChild(index);
            element.gameObject.SetActive(active);
            if (element.GetChild(0).gameObject.name == "opcja_hover")
            {
                element.GetChild(0).gameObject.SetActive(false);
            }
        }
    }
}
