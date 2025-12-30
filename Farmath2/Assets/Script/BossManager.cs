using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class BossManager : MonoBehaviour
{
    public GameObject BossCanvas;
    public Image panel;
    public RectTransform BossImage, BossText;
    public Ease ease, ease1;



    public LevelSettings levelSettings;
    public GameObject Sun;
    public GameObject Camera;
    public DeckPlacing DeckPlacing;
    public GameManager gameManager;
    public EffectColorChanging ECC;
    public CardScr[] AllBossCards;
    public FarmInfo[] AllFarms;
    public int BossEffectCount;
    public Volume volume;
    public ColorAdjustments adj;
    public float postExValue;
    public Color BossSoytariColor;
    public void BossStart()
    {
        StartCoroutine(ECC.ChangeColor(BossSoytariColor, 5));
        BossJoinAnim();
    }
    private void Update()
    {
        if (postExValue <= 1 && adj != null)
        {
            postExValue += Time.deltaTime / 3;
            adj.postExposure.value = Mathf.Lerp(0, 0.8f, postExValue);
            levelSettings.InitializeVolume();
        }
    }
    [ContextMenu("BossAnim")]
    void BossJoinAnim()
    {
        BossText.anchoredPosition = new Vector2(-1920, BossText.anchoredPosition.y);
        BossImage.anchoredPosition = new Vector2(1920, BossImage.anchoredPosition.y);
        BossCanvas.SetActive(true);
        Sequence seq = DOTween.Sequence();

        seq.Append(BossText.DOAnchorPosX(300, 1).SetEase(ease));
        seq.Join(BossImage.DOAnchorPosX(-300, 1).SetEase(ease));
        seq.Join(panel.DOFade(1, 3));

        seq.AppendInterval(2);

        seq.Append(BossText.DOAnchorPosX(1920, 1).SetEase(ease1));
        seq.Join(BossImage.DOAnchorPosX(-1920, 1).SetEase(ease1));

        seq.Join(panel.DOFade(0, 3));
        seq.OnComplete(() => { BossCanvas.SetActive(false); Logger.instance.StartDialougeCondition(1); });

        volume.profile.TryGet(out adj);
        Sun.transform.DORotate(new Vector3(0, 0, -30), 3);
    }
    public void BossCardUsed()
    {
        gameManager.PlaySound(7);
        StartCoroutine(gameManager.ShakeTheObj(Camera, 0.2f, 0.05f, 0, false));
        gameManager.money *= 0.995f;
        gameManager.InitializeMoneyText();
    }
    public void AddBossCard()
    {
        DeckPlacing.AddCardToDiscard(AllBossCards[Random.Range(0, AllBossCards.Length)]);
    }

    public void BossPutNegative()
    {

        for (int i = 0; i < AllFarms.Length; i++)
        {
            AllFarms[i].Negatived = false;
        }
        for (int i = 0; i < 12; i++)
        {
            for (int f = 0; f < 1000; f++)
            {
                FarmInfo chosenFarm = AllFarms[Random.Range(0, 24)];
                if (!chosenFarm.Negatived)
                {
                    chosenFarm.Negatived = true;
                    break;
                }
            }
        }
        for (int i = 0; i < AllFarms.Length; i++)
        {
            AllFarms[i].InitializeSpriteAndEffect();
        }
        gameManager.PlaySound(7);
    }
}
