using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public GameObject HoverObj;
    private RectTransform rectTransform;
    private bool isHovered = false;
    private bool isClicked = false;
    public float hoverScale = 1.2f; 
    public float clickScale = 1.5f; 
    public float animationSpeed = 5f; 
    private Vector3 defaultScale;
    private void Awake()
    {
        if (HoverObj == null)
        {
            HoverObj = this.gameObject;
        }
    }

    void Start()
    {
        rectTransform = HoverObj.GetComponent<RectTransform>();
        defaultScale = rectTransform.localScale;
    }

    void Update()
    {
        if (isClicked)
        {
            rectTransform.localScale = Vector3.Lerp(rectTransform.localScale, defaultScale * clickScale, Time.deltaTime * animationSpeed);
        }
        else if (isHovered)
        {
            rectTransform.localScale = Vector3.Lerp(rectTransform.localScale, defaultScale * hoverScale, Time.deltaTime * animationSpeed);
        }
        else
        {
            rectTransform.localScale = Vector3.Lerp(rectTransform.localScale, defaultScale, Time.deltaTime * animationSpeed);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        isClicked = false; 
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        isClicked = true;
    }
}
