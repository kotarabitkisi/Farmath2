using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FarmerInfo : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    public TextMeshProUGUI farmerName;
    public TextMeshProUGUI farmerDescription;
    public Image farmerImage;
    public bool touched, choosed;
    public Vector3 firstPosition;
    public RectTransform transformOfImage;
    public Canvas canvas;
    private void Update()
    {
        if (touched)
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
            transformOfImage.localPosition = pos;
        }
        if (choosed)
        {
            farmerImage.raycastTarget = false;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        firstPosition = transformOfImage.position;
        farmerImage.raycastTarget = false;
        touched = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        touched = false;

        if (eventData.pointerEnter != null && eventData.pointerEnter.CompareTag("HeroPlace"))
        {
            GameObject hittedHeroPlace = eventData.pointerEnter.gameObject;
            HireHero(this.gameObject, hittedHeroPlace);

        }
        else { this.transform.position = firstPosition; GetComponent<Image>().raycastTarget = true; }

    }
    public void HireHero(GameObject Hero, GameObject Par)
    {
        Hero.GetComponent<RectTransform>().position = Par.GetComponent<RectTransform>().position; choosed = true; Par.GetComponent<Image>().raycastTarget = false;
    }
}
