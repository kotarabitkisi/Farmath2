using TMPro;
using UnityEngine;

public class FarmInfo : MonoBehaviour
{
    [Header("MainStats")]
    public BoxCollider2D Col;
    Color Color = new Color(0, 1, 0, 5);
    public int Id;
    public int curDay, reqDay;
    public int[] connectedFarmIds = new int[4];//0:L 1:R 2:U 3:D
    public bool HolyHoed;
    public bool Watered;
    public bool Negatived;
    public bool firstBossEffected;
    [Header("Sprites")]
    public Sprite[] farmSprites;
    public Sprite[] farmPeriodSprite1;
    public Sprite[] farmPeriodSprite2;
    public Sprite[] GrowedfarmSprites;
    public Sprite ThatDisgustingBossImage;
    public SpriteRenderer farmImage;
    public SpriteRenderer SeededfarmImage;
    [Header("HoeObjects")]
    public GameObject HoeImage;
    public GameManager GameManager;
    public TextMeshProUGUI HoeCost;
    [Header("Effects")]
    public SpriteRenderer Renderer;
    public Material HolyMat, NormalMat;
    public GameObject NegativeEffect, HolyEffect;
    public Color WateredColor;
    private void Update()
    {

        if (firstBossEffected) { SeededfarmImage.sprite = ThatDisgustingBossImage; farmImage.sprite = ThatDisgustingBossImage; Col.enabled = false; farmImage.enabled = true; SeededfarmImage.enabled = false; }
        else
        {
            Col.enabled = true;
            if (Id != 0)
            {
                if (HolyHoed)
                {
                    Renderer.material = HolyMat;
                    HolyEffect.SetActive(true);
                }
                else { Renderer.material = NormalMat; HolyEffect.SetActive(false); }
                if (Negatived) { NegativeEffect.SetActive(true); }
                else { NegativeEffect.SetActive(false); }
                if (Watered)
                {
                    Renderer.color = WateredColor;
                }
                else { Renderer.color = Color.white; }
            }
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
        }




        if (GameManager.deckPlacing.cropCardUsing && Id == 1&&!firstBossEffected)
        {
            farmImage.color = Color.Lerp(Color.white, Color, (1 - Mathf.Sin(Time.time * 2.5f)) / 2);
        }
    }


}


