using TMPro;
using UnityEngine;

public class FarmInfo : MonoBehaviour
{

    Color Color = new Color(0, 1, 0, 5);
    public int Id;
    public int curDay, reqDay;
    public int[] connectedFarmIds = new int[4];//0:L 1:R 2:U 3:D
     
    public Sprite[] farmSprites;
    public Sprite[] farmPeriodSprite1;
    public Sprite[] farmPeriodSprite2;
    public Sprite[] GrowedfarmSprites;
    public SpriteRenderer farmImage;
    public SpriteRenderer SeededfarmImage; 

    public GameObject HoeImage;
    public GameManager GameManager;
    public TextMeshProUGUI HoeCost;

    public bool HolyHoed;
    public bool Watered;
    private void Update()
    {
        if (Id >= 2)
        {
            farmImage.enabled = false;
            SeededfarmImage.enabled = true;
            if (curDay <= reqDay / 3)
            {
                SeededfarmImage.sprite = farmSprites[Id];
            }
            else if (curDay <= reqDay * 2 / 3)
            {
                SeededfarmImage.sprite = farmPeriodSprite1[Id];
            }
            else if (curDay < reqDay)
            {
                SeededfarmImage.sprite = farmPeriodSprite2[Id];
            }
            else if (curDay >= reqDay) { SeededfarmImage.sprite = GrowedfarmSprites[Id]; }
        }
        else { farmImage.enabled = true; SeededfarmImage.enabled = false; farmImage.sprite = farmSprites[Id]; }
    
        



        if (GameManager.cropCardUsing)
        {
            farmImage.color = Color.Lerp(Color.white, Color, (1 - Mathf.Sin(Time.time)) / 2);
        }
    }


}


