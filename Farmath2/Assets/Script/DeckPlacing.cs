using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static GameManager;

public class DeckPlacing : MonoBehaviour
{

    [Header("AnotherScripts")]
    public GameManager GManager;
    public BossManager BossManager;
    public EffectColorChanging ColorScr;
    [Header("GameObjects")]
    public TextMeshProUGUI cardCountTxt;
    public GameObject closedCardPrefab;
    public GameObject cardPrefab;
    public GameObject cropCardUsing;
    public GameObject itemCardUsing;
    public List<GameObject> openedCards;
    public GameObject chosenMovingCard;
    [Header("MainStats")]
    public CardScr[] allCardScr;
    public bool takeRandomized;
    public bool cardCollected;
    public float Ypos2;
    const float xPos = 1.25f, yPos = 1;
    const int OpenedCardLimit = 3;
    public float rotationValue;
    public int chooseCardToDestroy;
    [Header("Discard")]
    public bool discardactivated;
    public List<CardScr> discardedCards;
    public Color DiscardColor1, DiscardColor2, BossSoytariColor;



    void Update()
    {

        Vector2 mousePosition;
        Collider2D collider;
        //if (Application.platform == RuntimePlatform.Android && Input.touchCount >= 1)
        //{
        //    Touch touch = Input.GetTouch(0);
        //    mousePosition = Camera.main.ScreenToWorldPoint(touch.position);
        //    collider = Physics2D.OverlapPoint(mousePosition);
        //}
        //else
        //{
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        collider = Physics2D.OverlapPoint(mousePosition);
        //}




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

        else if (GManager.activeCardState == ActiveCardState.IN_HAND)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (collider != null && collider.CompareTag("Card"))
                {
                    if (collider.gameObject == itemCardUsing || collider.gameObject == cropCardUsing)
                    {
                        if (itemCardUsing != null && GManager.activeCardState == ActiveCardState.IN_HAND)
                        {
                            MakeThisCardUsingCard(false, itemCardUsing);
                        }
                        else if (cropCardUsing != null && GManager.activeCardState == ActiveCardState.IN_HAND)
                        {
                            MakeThisCardUsingCard(false, cropCardUsing);
                        }
                    }
                    else
                    {

                        if (itemCardUsing != null && GManager.activeCardState == ActiveCardState.IN_HAND)
                        {
                            MakeThisCardUsingCard(false, itemCardUsing);
                            MakeThisCardUsingCard(true, collider.gameObject);
                        }
                        else if (cropCardUsing != null && GManager.activeCardState == ActiveCardState.IN_HAND)
                        {
                            MakeThisCardUsingCard(false, cropCardUsing);
                            MakeThisCardUsingCard(true, collider.gameObject);
                        }

                    }


                }
            }
            else if (Input.GetMouseButtonDown(1))
            {
                if (itemCardUsing != null)
                {
                    MakeThisCardUsingCard(false, itemCardUsing);
                }
                else if (cropCardUsing != null)
                {
                    MakeThisCardUsingCard(false, cropCardUsing);
                }
                GManager.activeCardState = ActiveCardState.NONE;
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
            chooseCardToDestroy = 2;
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
        ChosenCardToDiscard.tag = "Untagged";
        ChosenCardToDiscard.transform.DOMove(hit.point, 0.2f);
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

                ChosenObject.transform.parent = null;
                print(ChosenObject.transform.parent);
                ChosenObject.transform.DOMove(new Vector3(6, 0.5f, 0), 0.25f).SetEase(Ease.Linear);
                ChosenObject.transform.DOScale(cardPrefab.transform.lossyScale * 1.5f, 0.25f);
                InitializeAllCardsPositions();
                if (cardData is CropScr)
                {
                    cropCardUsing = ChosenObject;
                    GManager.activeCardState = ActiveCardState.IN_HAND;
                }
                else if (cardData is ItemScr ItemCardData)
                {
                    itemCardUsing = ChosenObject;
                    GManager.activeCardState = ActiveCardState.IN_HAND;
                    for (int i = 0; i < openedCards.Count; i++)
                    {
                        if (openedCards[i] == itemCardUsing)
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
                                    Destroy(itemCardUsing);
                                    break;
                                case 4:
                                    for (int j = 0; j < 3; j++)
                                    {
                                        TakeCardFromDiscard();
                                    }
                                    GManager.activeCardState = ActiveCardState.NONE;
                                    openedCards.RemoveAt(i);
                                    Destroy(itemCardUsing);
                                    break;
                                case 5:
                                    GManager.QuestionStart(ItemCardData);
                                    GManager.activeCardState = ActiveCardState.USING;
                                    openedCards.RemoveAt(i);
                                    Destroy(itemCardUsing);
                                    break;
                            }
                        }
                    }
                }
            }
        }
        else
        {
            if (itemCardUsing != null) { itemCardUsing = null; }
            if (cropCardUsing != null)
            {
                cropCardUsing = null;
                for (int i = 0; i < GManager.farmsScr.frams.Length; i++)
                {
                    GManager.farmsScr.frams[i].farmImage.color = Color.white;
                }
            }
            ChosenObject.transform.parent = transform;
            ChosenObject.transform.DOScale(cardPrefab.transform.lossyScale, 0.2f);
            GManager.activeCardState = ActiveCardState.NONE;
            InitializeAllCardsPositions();
        }
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

    public IEnumerator UseThisCard(FarmInfo farm)
    {
        float cardAnimTime = 0.5f;
        if (farm.Id == 1 && cropCardUsing)
        {
            GameObject chosenCardUsing = cropCardUsing;
            GManager.activeCardState = ActiveCardState.USING;
            CardScr card = cropCardUsing.GetComponent<CardData>().Card;
            chosenCardUsing.transform.DOMove(farm.transform.position, cardAnimTime);
            chosenCardUsing.transform.DOScale(Vector3.zero, cardAnimTime);
            yield return new WaitForSecondsRealtime(cardAnimTime);
            yield return new WaitForSecondsRealtime(0.2f);
            if (card is CropScr CropCard)
            {
                GManager.SeedCrop(CropCard.id, farm);
                DestroyThisCard(chosenCardUsing);
                for (int i = 0; i < GManager.farmsScr.frams.Length; i++)
                {
                    GManager.farmsScr.frams[i].farmImage.color = Color.white;
                }
                GManager.activeCardState = ActiveCardState.NONE;
            }
        }
        else if (itemCardUsing)
        {
            CardScr card = itemCardUsing.GetComponent<CardData>().Card;
            if (card is ItemScr ItemCard)
            {
                GameObject chosenCardUsing;
                switch (ItemCard.itemId)
                {
                    case 0:
                        GManager.activeCardState = ActiveCardState.USING;
                        chosenCardUsing = itemCardUsing;
                        chosenCardUsing.transform.DOMove(farm.transform.position, cardAnimTime);
                        chosenCardUsing.transform.DOScale(Vector3.zero, cardAnimTime);
                        yield return new WaitForSecondsRealtime(cardAnimTime);
                        farm.reqDay /= 2;
                        DestroyThisCard(chosenCardUsing);
                        GManager.activeCardState = ActiveCardState.NONE;
                        break;
                    case 2:
                        if (farm.Id < 2)
                        {
                            GManager.activeCardState = ActiveCardState.USING;
                            chosenCardUsing = itemCardUsing;
                            chosenCardUsing.transform.DOMove(farm.transform.position, cardAnimTime);
                            chosenCardUsing.transform.DOScale(Vector3.zero, cardAnimTime);
                            yield return new WaitForSecondsRealtime(cardAnimTime);
                            farm.HolyHoed = true;
                            farm.Id = 1;
                            DestroyThisCard(chosenCardUsing);
                            GManager.activeCardState = ActiveCardState.NONE;
                        }
                        break;
                    case 3:
                        GManager.activeCardState = ActiveCardState.USING;
                        chosenCardUsing = itemCardUsing;
                        chosenCardUsing.transform.DOMove(farm.transform.position, cardAnimTime);
                        chosenCardUsing.transform.DOScale(Vector3.zero, cardAnimTime);
                        yield return new WaitForSecondsRealtime(cardAnimTime);
                        farm.Id = 1;
                        farm.curDay = 0;
                        farm.reqDay = 0;
                        DestroyThisCard(chosenCardUsing);
                        GManager.activeCardState = ActiveCardState.NONE;
                        break;
                }
            }

        }
    }

}
