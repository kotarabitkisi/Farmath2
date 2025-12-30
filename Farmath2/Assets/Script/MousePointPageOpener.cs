using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization;

public class MousePointPageOpener : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Canvas canvas;
    public GameObject Page;
    public TextMeshProUGUI[] Texts;
    public LocalizedString[] str;
    public bool isEventPageOpener;
    public bool isHarvestPage;
    bool showing;
    private void Update()
    {
        if (showing)
        {
            Page.transform.localPosition = TakeCanvasPosition();
        }
    }
    Vector2 TakeCanvasPosition()
    {
        Vector2 pos;
#if UNITY_ANDROID
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                Input.GetTouch(0).position,
                canvas.worldCamera,
                out pos);
#elif UNITY_STANDALONE_WIN
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            Input.mousePosition,
            canvas.worldCamera,
            out pos);
#endif
        return pos;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        showing = true;
        Vector2 pos= TakeCanvasPosition();
        Vector2 pivot_;
        print(pos);
        pivot_.x = pos.x / Screen.width >= 0 ? 1 : 0;
        pivot_.y = pos.y / Screen.height >= 0 ? 1 : 0;
        Page.GetComponent<RectTransform>().pivot = pivot_;
        Page.SetActive(true);
        if (isEventPageOpener)
        {
            int id = GameManager.instance.Month;
            Texts[0].text = LanguageManager.instance.TurnToString(str[id], null);
        }
        else if (isHarvestPage)
        {
            for (int i = 0; i < Texts.Length - 1; i++)
            {
                Texts[i].text = LanguageManager.instance.TurnToString(str[i], null) + GameManager.instance.HarvestedCropCount[i + 2];
            }
            int TotalHarvestCount = 0;
            for (int i = 0; i < GameManager.instance.HarvestedCropCount.Length; i++)
            {
                TotalHarvestCount += GameManager.instance.HarvestedCropCount[i];
            }
            Texts[Texts.Length - 1].text = LanguageManager.instance.TurnToString(str[Texts.Length - 1], null) + TotalHarvestCount;
        }
        else
        {
            for (int i = 0; i < Texts.Length; i++)
            {
                Texts[i].text = LanguageManager.instance.TurnToString(str[i], null);
            }

        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        showing = false;
        Page.SetActive(false);
    }
}
