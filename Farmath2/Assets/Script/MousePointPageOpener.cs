using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class MousePointPageOpener : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject Page;
    public TextMeshProUGUI[] Texts;
    public string[] str;
    public GameManager GameManager;
    public void OnPointerEnter(PointerEventData eventData)
    {
        Page.SetActive(true);
        for (int i = 0; i < Texts.Length-1; i++)
        {
            Texts[i].text = str[i] + GameManager.HarvestedCropCount[i + 2];
        }
        int TotalHarvestCount = 0;
        for (int i = 0;i < GameManager.HarvestedCropCount.Length; i++)
        {
            TotalHarvestCount += GameManager.HarvestedCropCount[i];
        }
        Texts[Texts.Length - 1].text = str[Texts.Length - 1] + TotalHarvestCount;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Page.SetActive(false);
    }
}
