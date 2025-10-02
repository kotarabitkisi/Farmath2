using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
        seq.Join(transform.DOScale(Vector3.one * 3f, 0.5f).SetEase(Ease.Linear));
        seq.Join(transform.DOMove(Vector3.zero, 0.5f).SetEase(Ease.Linear));
        seq.Join(cardIcon.DOFade(0, 0.5f));
        seq.Join(cardImage.DOFade(0, 0.5f));
        seq.Join(itemCornerImages[0].DOFade(0, 0.5f));
        seq.Join(itemCornerImages[1].DOFade(0, 0.5f));
        seq.Join(textMeshProUGUI.DOFade(0, 0.5f));
        seq.OnComplete(() =>
        {
            DeckPlacing.Instance.DestroyThisCard(gameObject);
            if (!(destroy || Card.IsThatOneUse))
                DeckPlacing.Instance.AddCardToDiscard(Card);
        });
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
        cardTitle.text = Card.CardName;
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
