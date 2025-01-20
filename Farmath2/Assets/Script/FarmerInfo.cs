using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FarmerInfo : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    public FarmerScr farmer;
    public TextMeshProUGUI farmerName;
    public TextMeshProUGUI farmerDescription;
    public Image farmerImage;
    public bool touched, choosed;
    public Vector3 firstPosition;
    public RectTransform transformOfImage;
    GameManager Gmanager;
    private void Start()
    {
        Gmanager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }
    private void Update()
    {
        if (touched)
        {
            GetComponent<RectTransform>().position = Input.mousePosition;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        print(eventData.pointerEnter);
        firstPosition = transformOfImage.position;
        GetComponent<Image>().raycastTarget = false;
        touched = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        touched = false;

        if (eventData.pointerEnter != null && eventData.pointerEnter.CompareTag("HeroPlace"))
        {
            GameObject hittedHeroPlace = eventData.pointerEnter.gameObject;
            this.transform.position = hittedHeroPlace.transform.position; choosed = true; hittedHeroPlace.GetComponent<Image>().raycastTarget = false;
        }
        else { this.transform.position = firstPosition; GetComponent<Image>().raycastTarget = true; }

    }
}
