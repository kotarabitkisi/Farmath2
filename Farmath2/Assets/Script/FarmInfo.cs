using DG.Tweening;
using TMPro;
using UnityEngine;

public class FarmInfo : MonoBehaviour
{
    public SpriteCrossfade crossfadeScript;
    public SpriteCrossfade crossfadeScriptnotseeded;
    [Header("MainStats")]
    public BoxCollider2D Col;
    Color Color = new Color(0, 0.25f, 0, 5);
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
    public SpriteRenderer[] farmImage;
    public SpriteRenderer[] SeededfarmImage;
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
            holyEffect.SetActive(HolyHoed);
            negativeEffect.SetActive(Negatived);
            Renderer.color = Watered ? WateredColor : Color.white;
        }
        float _EffectTime = 0.5f;
        if (Id >= 2)
        {
            if (farmImage[0].enabled)
            {
                for (int i = 0; i < farmImage.Length; i++)
                {
                    farmImage[i].enabled = false;
                    SeededfarmImage[i].enabled = true;
                    _EffectTime = 0;
                }
            }
            if (Id == 8)
            {
                _EffectTime = 0.5f;
            }
            if (curDay <= reqDay / 3)
            {
                crossfadeScript.FadeTo(farmSprites[Id], _EffectTime);
            }
            else if (curDay <= reqDay * 2 / 3)
            {
                crossfadeScript.FadeTo(farmPeriodSprite1[Id], _EffectTime);
            }
            else if (curDay < reqDay)
            {
                crossfadeScript.FadeTo(farmPeriodSprite2[Id], _EffectTime);
            }
            else if (curDay >= reqDay) { crossfadeScript.FadeTo(GrowedfarmSprites[Id], _EffectTime); }
        }
        else
        {
            if (SeededfarmImage[0].enabled)
            {
                for (int i = 0; i < farmImage.Length; i++)
                {
                    farmImage[i].enabled = true;
                    SeededfarmImage[i].enabled = false;
                    _EffectTime = 0;
                }
            }
            crossfadeScriptnotseeded.FadeTo(farmSprites[Id], _EffectTime);
        }

        if (GameManager.debuffs[1] && (Id >= 8 || Id == 1))
        {
            invasionEffect.SetActive(true);
        }
        else { invasionEffect.SetActive(false); }


    }
    private void Update()
    {
        if (GameManager.deckPlacing.cropCardUsing != null && Id == 1)
        {
            for (int i = 0; i < farmImage.Length; i++)
            {
                farmImage[i].color = Color.Lerp(Color.white, Color, (1 - Mathf.Sin(Time.time * 2.5f)) / 2);
            }
        }
    }
    public void ChangeScale(float first, float second)
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(gameObject.transform.DOScale(Vector3.one * first, 0.25f));
        seq.Append(gameObject.transform.DOScale(Vector3.one * second, 0.25f));
    }

}


