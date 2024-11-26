using UnityEngine;

public class FarmInfo : MonoBehaviour
{

    Color Color=new Color(0,1,0,5);
    public int Id;
    public int curDay, reqDay;
    public int[] connectedFarmIds = new int[4];//0:L 1:R 2:U 3:D
    public Sprite[] farmSprites;
    public Sprite[] GrowedfarmSprites;
    public SpriteRenderer farmImages;
    public GameObject HoeImage;
    public GameManager GameManager;

    public bool HolyHoed;
    public bool Watered;
    private void Awake()
    {
        farmImages = GetComponent<SpriteRenderer>();
    }
    private void Update()
    {
        if (Id >= 2 && reqDay <= curDay)
        {
            farmImages.sprite = GrowedfarmSprites[Id];
        }
        else { farmImages.sprite = farmSprites[Id]; }
        if (GameManager.cropCardUsing)
        {
            farmImages.color = Color.Lerp(Color.white, Color, (1 - Mathf.Sin(Time.time)) / 2);
        }
    }


}


