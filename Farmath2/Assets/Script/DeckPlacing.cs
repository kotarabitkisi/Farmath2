using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static GameManager;

public class DeckPlacing : MonoBehaviour
{
    [ExecuteInEditMode]
    public BossManager BossManager;
    public EffectColorChanging ColorScr;
    public TextMeshProUGUI cardCountTxt;
    public GameObject closedCardPrefab;
    public GameObject cardPrefab;
    public CardScr[] allCardScr;
    public bool takeRandomized;

    public List<GameObject> openedCards;
    public bool cardCollected;
    public float Ypos2;
    const float xPos = 1.25f, yPos = 1;
    const int OpenedCardLimit = 3;
    public float rotationValue;
    public int chooseCardToDestroy;
    public GameObject chosenMovingCard;
    public GameManager GManager;
    [Header("Discard")]
    public bool discardactivated;
    public List<CardScr> discardedCards;
    public Color DiscardColor1, DiscardColor2, BossSoytariColor;



    void Update()
    {

        Vector2 mousePosition;
        Collider2D collider;
        if (Application.platform == RuntimePlatform.Android && Input.touchCount >= 1)
        {
            Touch touch = Input.GetTouch(0);
            mousePosition = Camera.main.ScreenToWorldPoint(touch.position);
            collider = Physics2D.OverlapPoint(mousePosition);
        }
        else
        {
            mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            collider = Physics2D.OverlapPoint(mousePosition);
        }




        if (GManager.activeCardState == ActiveCardState.NONE)
        {
            if (chosenMovingCard != null)
            {
                if ((collider != null && collider.gameObject != chosenMovingCard) || (collider == null))
                {
                    chosenMovingCard.transform.DOScale(cardPrefab.transform.lossyScale, 0.25f);
                    chosenMovingCard.transform.DOLocalMoveY(yPos, 0.25f);
                    //chosenCard.transform.DORotate(new Vector3(0, 0, 0), 0.2f).SetEase(Ease.Linear);
                    chosenMovingCard.GetComponent<BoxCollider2D>().offset = new Vector2(0, 0);
                    chosenMovingCard.GetComponent<BoxCollider2D>().size = new Vector2(1, 1f);
                    chosenMovingCard = null;
                    InitializeAllCardsPositions();
                }
                if (Input.GetMouseButtonDown(0))
                {
                    if (chooseCardToDestroy > 0)
                    {
                        for (int i = 0; i < openedCards.Count; i++)
                        {
                            if (chosenMovingCard == openedCards[i])
                            {
                                openedCards.RemoveAt(i);
                                Destroy(chosenMovingCard);
                                chooseCardToDestroy--;
                            }
                        }
                    }
                    else
                    {
                        MakeThisCardUsingCard(true, chosenMovingCard);
                        chosenMovingCard = null;
                    }

                }
            }
            else
            {
                if (collider != null && collider.gameObject.CompareTag("Card"))
                {
                    chosenMovingCard = collider.gameObject;
                    chosenMovingCard.transform.DOLocalMoveY(yPos + Ypos2, 0.25f);
                    chosenMovingCard.transform.DOScale(cardPrefab.transform.lossyScale * 1.5f, 0.25f);
                    chosenMovingCard.GetComponent<SpriteRenderer>().sortingOrder = 50;
                    chosenMovingCard.transform.GetChild(0).GetComponent<Canvas>().sortingOrder = 51;
                    chosenMovingCard.GetComponent<BoxCollider2D>().offset = new Vector2(0, -0.165f);
                    chosenMovingCard.GetComponent<BoxCollider2D>().size = new Vector2(1, 1.35f);
                }
            }
        }
        closedCardPrefab.SetActive(discardedCards.Count > 0);
        cardCountTxt.text = "Card: " + discardedCards.Count.ToString();
    }
    public void DeleteAllOfOpenedCards()
    {
        int Count = openedCards.Count;
        if (Count != 0)
        {
            for (int i = 0; i < Count; i++)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
        }

        openedCards.Clear();
    }
    public IEnumerator DayPassedTakeCard()
    {

        DeleteAllOfOpenedCards();

        if (GManager.debuffs[2])
        {
            for (int i = 0; i < 6; i++)
            {
                TakeCardFromDiscard();
                yield return new WaitForSecondsRealtime(0.1f);
            }
            chooseCardToDestroy = 3;
        }
        else
        {
            for (int i = 0; i < OpenedCardLimit; i++)
            {
                TakeCardFromDiscard();
                yield return new WaitForSecondsRealtime(0.1f);
            }
        }
        if (GManager.farmers[0].GetComponent<FarmerInfo>().choosed)
        {
            TakeCardFromDiscard();
            yield return new WaitForSecondsRealtime(0.1f);
        }
    }
    public void TakeCardFromDiscard()
    {
        if (discardedCards.Count != 0)
        {
            int cardIndex = 0;

            if (takeRandomized)
            {
                cardIndex = Random.Range(0, discardedCards.Count);
            }

            CardScr Card = discardedCards[cardIndex];
            discardedCards.RemoveAt(cardIndex);
            RaycastHit2D hit = Physics2D.Raycast(closedCardPrefab.transform.position, Vector2.zero);
            GameObject newOpenedCard = Instantiate(cardPrefab, hit.point, Quaternion.identity);
            newOpenedCard.transform.SetParent(transform, true);
            GameObject newOpenedCardCanvas = newOpenedCard.transform.GetChild(0).gameObject;
            newOpenedCardCanvas.transform.GetChild(0).GetComponent<Image>().sprite = Card.Icon;
            newOpenedCardCanvas.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = Card.CardName;
            newOpenedCard.gameObject.GetComponent<CardData>().Card = Card;
            openedCards.Add(newOpenedCard);
            InitializeAllCardsPositions();
        }

    }
    public void AddCardToDiscard(CardScr Card)
    {
        discardedCards.Add(Card);
    }
    public void InitializeAllCardsPositions()
    {
        int CardCount = transform.childCount;
        float startingXpos = -xPos * CardCount / 2 + 0.5f;
        for (int i = 0; i < CardCount; i++)
        {
            GameObject Card = transform.GetChild(i).gameObject;
            Card.GetComponent<SpriteRenderer>().sortingOrder = 2 * (CardCount - i);
            Card.transform.GetChild(0).GetComponent<Canvas>().sortingOrder = 2 * (CardCount - i) + 1;
            Card.transform.DOLocalMove(new Vector3(startingXpos + xPos * i, yPos, 0), 0.2f).SetEase(Ease.Linear);
        }

    }

    public void ActivatePutDiscard()
    {
        if (discardactivated)
        {
            if (GManager.Boss == 1)
            {
                StartCoroutine(ColorScr.ChangeColor(BossSoytariColor, 0.25f)); discardactivated = false;
            }
            else
            {
                StartCoroutine(ColorScr.ChangeColor(Color.white, 0.25f)); discardactivated = false;

            }
        }
        else { StartCoroutine(ColorScr.ChangeColor(DiscardColor1, 0.25f)); discardactivated = true; }

    }
    public IEnumerator PutDiscardThisCard(GameObject ChosenCardToDiscard)
    {
        for (int i = 0; i < openedCards.Count; i++)
        {
            if (ChosenCardToDiscard == openedCards[i])
            {
                openedCards.RemoveAt(i);
                break;
            }

        }

        RaycastHit2D hit = Physics2D.Raycast(closedCardPrefab.transform.position, Vector2.zero);
        Vector2 targetPosition = hit.point;
        ChosenCardToDiscard.transform.parent = null;
        ChosenCardToDiscard.tag ="Untagged";
        ChosenCardToDiscard.transform.DOMove(hit.point,0.2f);
        ChosenCardToDiscard.transform.DOScale(Vector3.one, 0.2f);
        
        yield return new WaitForSecondsRealtime(0.2f);
        InitializeAllCardsPositions();
        print(ChosenCardToDiscard.transform.position);
        discardedCards.Add(ChosenCardToDiscard.GetComponent<CardData>().Card);
        Destroy(ChosenCardToDiscard);

    }
    public void MakeThisCardUsingCard(bool trueorfalse, GameObject ChosenObject)
    {
        if (trueorfalse)
        {
            CardScr cardData = ChosenObject.GetComponent<CardData>().Card;
            if (discardactivated) { StartCoroutine(PutDiscardThisCard(ChosenObject)); }
            else if (cardData.IsThatBossCard) { BossManager.BossCardUsed(); DestroyThisCard(ChosenObject); }
            else
            {
                ChosenObject.transform.DOMove(new Vector3(6, 0.5f, 0), 0.25f).SetEase(Ease.Linear);
                ChosenObject.transform.parent = null;
                InitializeAllCardsPositions();
                if (cardData is CropScr)
                {
                    GManager.cropCardUsing = ChosenObject;
                    GManager.activeCardState = ActiveCardState.IN_HAND;
                    InitializeAllCardsPositions();
                }
                else if (cardData is ItemScr ItemCardData)
                {
                    GManager.itemCardUsing = ChosenObject;
                    GManager.activeCardState = ActiveCardState.IN_HAND;
                    for (int i = 0; i < openedCards.Count; i++)
                    {
                        if (openedCards[i] == GManager.itemCardUsing)
                        {
                            switch (ItemCardData.itemId)
                            {
                                case 1:
                                    for (int j = 0; j < 3; j++)
                                    {
                                        GManager.farmsScr.water(6, 4, true);
                                    }
                                    GManager.activeCardState = ActiveCardState.NONE;
                                    openedCards.RemoveAt(i);
                                    Destroy(GManager.itemCardUsing);
                                    break;
                                case 4:
                                    for (int j = 0; j < 3; j++)
                                    {
                                        TakeCardFromDiscard();
                                    }
                                    GManager.activeCardState = ActiveCardState.NONE;
                                    openedCards.RemoveAt(i);
                                    Destroy(GManager.itemCardUsing);
                                    break;
                                case 5:
                                    GManager.QuestionStart(ItemCardData);
                                    GManager.activeCardState = ActiveCardState.USING;
                                    openedCards.RemoveAt(i);
                                    Destroy(GManager.itemCardUsing);
                                    break;
                            }
                        }
                    }
                    InitializeAllCardsPositions();
                }
            }
        }
        else
        {
            GManager.itemCardUsing = null;
            GManager.cropCardUsing = null;
            ChosenObject.transform.parent = transform;
            ChosenObject.transform.DOScale(cardPrefab.transform.lossyScale, 0.2f);
            GManager.activeCardState = ActiveCardState.NONE;
            InitializeAllCardsPositions();
        }
        InitializeAllCardsPositions();
    }

    public void DestroyThisCard(GameObject card)
    {
        for (int i = 0; i < openedCards.Count; i++)
        {
            if (openedCards[i] == card)
            {
                openedCards.RemoveAt(i); Destroy(card);
            }

        }
        GManager.activeCardState = GameManager.ActiveCardState.NONE;
    }



}
