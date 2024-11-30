using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeckPlacing : MonoBehaviour
{
    [ExecuteInEditMode]

    public TextMeshProUGUI cardCountTxt;
    public GameObject closedCardPrefab;
    public GameObject cardPrefab;
    public CardScr[] allCardScr;
    public bool takeRandomized;
    public List<CardScr> discardedCards;
    public List<GameObject> openedCards;
    public bool cardCollected;
    public float Ypos2;
    const float xPos = 1, yPos = 1;
    const int OpenedCardLimit = 3;
    public float rotationValue;
    public GameObject chosenMovingCard;
    public GameManager GManager;
    public bool discardactivated;

    void Update()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Collider2D collider = Physics2D.OverlapPoint(mousePosition);
        if (!GManager.cropCardUsing && !GManager.itemCardUsing)
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
                    MakeThisCardUsingCard(true, chosenMovingCard);
                    chosenMovingCard = null;
                }
            }
            else
            {
                if (collider != null && collider.gameObject.CompareTag("Card"))
                {
                    chosenMovingCard = collider.gameObject;
                    chosenMovingCard.transform.DOLocalMoveY(yPos + Ypos2, 0.25f);
                    chosenMovingCard.transform.DOScale(cardPrefab.transform.lossyScale * 1.5f, 0.25f);
                    //chosenCard.transform.DORotate(new Vector3(0, 0, 0), 0.2f).SetEase(Ease.Linear);
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
        for (int i = 0; i < OpenedCardLimit; i++)
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
            print(Camera.main.ScreenToWorldPoint(closedCardPrefab.transform.position));
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
            //Card.transform.DORotate(new Vector3(0, 0, rotationValue * (i - CardCount / 2) / CardCount), 0.2f).SetEase(Ease.Linear);

        }

    }

    public void ActivatePutDiscard()
    {
        discardactivated = !discardactivated;
    }
    public IEnumerator PutDiscardThisCard(GameObject ChosenCardToDiscard)
    {
        for (int i = 0; i < openedCards.Count; i++)
        {

            if (ChosenCardToDiscard == openedCards[i])
            {
                openedCards.RemoveAt(i);
                RaycastHit2D hit = Physics2D.Raycast(closedCardPrefab.transform.position, Vector2.zero);
                ChosenCardToDiscard.transform.parent = null;
                ChosenCardToDiscard.transform.DOMove(hit.point, 0.2f).SetEase(Ease.Linear);
                ChosenCardToDiscard.transform.DOScale(new Vector3(1, 1, 1), 0.2f).SetEase(Ease.Linear);
                InitializeAllCardsPositions();
                yield return new WaitForSecondsRealtime(0.2f);
                discardedCards.Add(ChosenCardToDiscard.GetComponent<CardData>().Card);
                Destroy(ChosenCardToDiscard);
                //GManager.money -=**;
                break;
            }
        }
    }
    public void MakeThisCardUsingCard(bool trueorfalse, GameObject ChosenObject)
    {
        if (trueorfalse)
        {
            CardScr cardData = ChosenObject.GetComponent<CardData>().Card;
            if (discardactivated) { StartCoroutine(PutDiscardThisCard(ChosenObject)); }
            else
            {
                ChosenObject.transform.DOMove(new Vector3(6, 0.5f, 0), 0.25f);
                //chosenCard.transform.DORotate(new Vector3(0, 0, 0), 0.2f).SetEase(Ease.Linear);
                ChosenObject.transform.parent = null;
                InitializeAllCardsPositions();
            }
            if (cardData is CropScr)
            {
                cardData.Use();
                GManager.cropCardUsing = ChosenObject;
                ChosenObject.transform.parent = null;
                InitializeAllCardsPositions();
            }
            else if (cardData is ItemScr ItemCardData)
            {
                GManager.itemCardUsing = ChosenObject;
                cardData.Use();
                switch (ItemCardData.itemId)
                {

                    case 1:
                        for (int i = 0; i < 3; i++)
                        {
                            GManager.farmsScr.water(0, 0, true); Destroy(GManager.itemCardUsing);
                        }
                        for (int i = 0; i < openedCards.Count; i++)
                        {
                            if (openedCards[i] == GManager.itemCardUsing) { openedCards.RemoveAt(i); break; }
                        }
                        break;
                    case 4:
                        for (int i = 0; i < 3; i++)
                        {
                            TakeCardFromDiscard();
                        }
                        for (int i = 0; i < openedCards.Count; i++)
                        {
                            if (openedCards[i] == GManager.itemCardUsing) { openedCards.RemoveAt(i); break; }
                        }
                        Destroy(GManager.itemCardUsing);
                        break;
                }
                ChosenObject.transform.parent = null;
            }

        }
        else
        {
            GManager.itemCardUsing = null;
            GManager.cropCardUsing = null;
            ChosenObject.transform.parent = transform;
            ChosenObject.transform.DOScale(cardPrefab.transform.lossyScale, 0.2f);
        }
        InitializeAllCardsPositions();
    }
}
