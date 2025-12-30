using DG.Tweening;
using TMPro;
using UnityEngine;

public class CardData : MonoBehaviour
{
    public Canvas Canvas;
    public SpriteRenderer cardIcon;
    public SpriteRenderer cardImage;
    public TextMeshProUGUI textMeshProUGUI;
    public CardScr Card;
    public float TheRewardifQuestion;
    public Sprite[] frontCardIcon;
    public SpriteRenderer[] itemCornerImages;
    public TextMeshProUGUI cardTitle;
    public Sprite[] itemSprites;
    public Sprite[] cropSprites;
    public void FadeCard(bool destroy)
    {

        GetComponent<BoxCollider2D>().enabled = false;
        transform.SetParent(null, true);

        transform.DOKill();
        Sequence seq = DOTween.Sequence();
        float animTime_ = 0.75f;
        transform.DOScale(Vector3.zero, animTime_).SetEase(Ease.Linear);
        transform.DORotate(new Vector3(0, 0, 1080), animTime_, RotateMode.WorldAxisAdd);
        transform.DOMoveY(1, animTime_).SetEase(Ease.OutBack);
        cardIcon.DOFade(0, animTime_);
        cardImage.DOFade(0, animTime_);
        itemCornerImages[0].DOFade(0, animTime_);
        itemCornerImages[1].DOFade(0, animTime_);
        textMeshProUGUI.DOFade(0, animTime_).OnComplete(
        () =>
        {
            DeckPlacing.Instance.DestroyThisCard(gameObject);
            if (!(destroy || Card.IsThatOneUse))
                DeckPlacing.Instance.AddCardToDiscard(Card);
        }
        );

    }
    public void SortLayers(int a)
    {
        Canvas.sortingOrder = 2 * a;
        cardImage.sortingOrder = 2 * a;
        cardIcon.sortingOrder = 2 * a + 1;
        itemCornerImages[0].sortingOrder = 2 * a + 1;
        itemCornerImages[1].sortingOrder = 2 * a + 1;
    }
    public void InitializeImages()
    {
        cardTitle.text = LanguageManager.instance.TurnToString(Card.CardName, null);
        cardIcon.sprite = Card.Icon;
        if (Card is CropScr CropCard)
        {
            GetComponent<SpriteRenderer>().sprite = frontCardIcon[0];
            for (int i = 0; i < itemCornerImages.Length; i++)
            {
                itemCornerImages[i].sprite = cropSprites[CropCard.id - 2];
            }
        }
        else if (Card is ItemScr ItemCard)
        {
            GetComponent<SpriteRenderer>().sprite = frontCardIcon[1];
            for (int i = 0; i < itemCornerImages.Length; i++)
            {
                itemCornerImages[i].sprite = itemSprites[ItemCard.itemId];
            }
        }

    }

}
