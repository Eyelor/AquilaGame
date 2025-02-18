using TMPro;
using UnityEngine;

public class CheckIsland: MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI islandTypeSize;
    [SerializeField] private GameObject[] affilationText;
    [SerializeField] private RectTransform anchorIconRectTransform;

    public void checkMiniIsland(string miniIslandName, string affilation)
    {
        anchorIconRectTransform.localPosition = new Vector3(469.2f, anchorIconRectTransform.localPosition.y);
        if (miniIslandName.StartsWith("MiniIslandPiaSDet")) { islandTypeSize.text = "MA£A PUSTYNNA WYSPA"; }
        else if (miniIslandName.StartsWith("MiniIslandPiaMDet")) { islandTypeSize.text = "ŒREDNIA PUSTYNNA WYSPA"; }
        else if (miniIslandName.StartsWith("MiniIslandPiaLDet")) { islandTypeSize.text = "DU¯A PUSTYNNA WYSPA"; }
        else if (miniIslandName.StartsWith("MiniIslandBloSDet")) { islandTypeSize.text = "MA£A B£OTNISTA WYSPA"; }
        else if (miniIslandName.StartsWith("MiniIslandBloMDet")) { islandTypeSize.text = "ŒREDNIA B£OTNISTA WYSPA"; }
        else if (miniIslandName.StartsWith("MiniIslandBloLDet")) { islandTypeSize.text = "DU¯A B£OTNISTA WYSPA"; }
        else if (miniIslandName.StartsWith("MiniIslandTraSDet")) { islandTypeSize.text = "MA£A TRAWIASTA WYSPA"; }
        else if (miniIslandName.StartsWith("MiniIslandTraMDet")) { islandTypeSize.text = "ŒREDNIA TRAWIASTA WYSPA"; }
        else if (miniIslandName.StartsWith("MiniIslandTraLDet")) { islandTypeSize.text = "DU¯A TRAWIASTA WYSPA"; }
        else if (miniIslandName.StartsWith("MiniIslandPort")) { islandTypeSize.text = "PORT"; anchorIconRectTransform.localPosition = new Vector3(435f, anchorIconRectTransform.localPosition.y); }
        
        if (affilation == "greenFraction") 
        {
            if (!affilationText[0].activeSelf) affilationText[0].SetActive(true);
            if (affilationText[1].activeSelf) affilationText[1].SetActive(false);
            if (affilationText[2].activeSelf) affilationText[2].SetActive(false);
            if (affilationText[3].activeSelf) affilationText[3].SetActive(false);
        }
        else if (affilation == "blueFraction") 
        {
            if (affilationText[0].activeSelf) affilationText[0].SetActive(false);
            if (!affilationText[1].activeSelf) affilationText[1].SetActive(true);
            if (affilationText[2].activeSelf) affilationText[2].SetActive(false);
            if (affilationText[3].activeSelf) affilationText[3].SetActive(false);
        }
        else if (affilation == "redFraction") 
        {
            if (affilationText[0].activeSelf) affilationText[0].SetActive(false);
            if (affilationText[1].activeSelf) affilationText[1].SetActive(false);
            if (!affilationText[2].activeSelf) affilationText[2].SetActive(true);
            if (affilationText[3].activeSelf) affilationText[3].SetActive(false);
        } 
        else
        {
            if (affilationText[0].activeSelf) affilationText[0].SetActive(false);
            if (affilationText[1].activeSelf) affilationText[1].SetActive(false);
            if (affilationText[2].activeSelf) affilationText[2].SetActive(false);
            if (!affilationText[3].activeSelf) affilationText[3].SetActive(true);
        }
    }
}
