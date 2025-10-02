using DG.Tweening;
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
    [Header("Sprites")]
    public Sprite[] farmSprites;
    public Sprite[] farmPeriodSprite1;
    public Sprite[] farmPeriodSprite2;
    public Sprite[] GrowedfarmSprites;
    public SpriteRenderer farmImage;
    public SpriteRenderer SeededfarmImage;
    [Header("HoeObjects")]
    public GameObject HoeImage;
    public GameManager GameManager;
    public TextMeshProUGUI HoeCost;
    [Header("Effects")]
    public SpriteRenderer Renderer;
    public GameObject negativeEffect, holyEffect, invasionEffect;
    public Color WateredColor;
    private void Start()
    {
        InitializeSpriteAndEffect();
    }
    public void InitializeSpriteAndEffect()
    {
        Col.enabled = true;
        if (Id != 0)
        {
            if (HolyHoed)
            {
                holyEffect.SetActive(true);
            }
            else { holyEffect.SetActive(false); }
            if (Negatived) { negativeEffect.SetActive(true); }
            else { negativeEffect.SetActive(false); }
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

        if (GameManager.debuffs[1] && (Id >= 8 || Id == 1))
        {
            invasionEffect.SetActive(true);
        }
        else { invasionEffect.SetActive(false); }

        if (GameManager.deckPlacing.cropCardUsing != null && Id == 1)
        {
            farmImage.color = Color.Lerp(Color.white, Color, (1 - Mathf.Sin(Time.time * 2.5f)) / 2);
        }
    }
    public void ChangeScale(float first, float second)
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(gameObject.transform.DOScale(Vector3.one * first, 0.25f));
        seq.Append(gameObject.transform.DOScale(Vector3.one * second, 0.25f));
    }

}


