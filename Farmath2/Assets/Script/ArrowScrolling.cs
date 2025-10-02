using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ArrowScrolling : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public ScrollRect scrollRect;
    public bool isScrolling;
    public float scrollSpeed = 5f;
    public float scrollBorderUp, scrollBorderDown;
    public Image Image;
    void Update()
    {
        if (isScrolling)
        {
            scrollRect.verticalNormalizedPosition += scrollSpeed;
        }
        if (scrollRect.verticalNormalizedPosition <= scrollBorderUp && scrollRect.verticalNormalizedPosition >= scrollBorderDown)
        {
            Image.raycastTarget = true;
            Image.enabled = true;
        }
        else
        {
            Image.raycastTarget = false;
            Image.enabled = false;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {

        isScrolling = true;

    }

    public void OnPointerExit(PointerEventData eventData)
    {

        isScrolling = false;

    }
}
