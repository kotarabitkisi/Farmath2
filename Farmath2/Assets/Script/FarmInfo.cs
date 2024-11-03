using UnityEngine;

public class FarmInfo : MonoBehaviour
{
    public int Id;
    public int curDay, reqDay;
    public int[] connectedFarmIds = new int[4];//0:L 1:R 2:U 3:D
    public Sprite[] farmSprites;
    public Sprite[] GrowedfarmSprites;
    public SpriteRenderer farmImages;
    public GameObject HoeImage;
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
    }


}


