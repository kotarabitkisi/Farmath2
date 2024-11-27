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
    const float xPos = 1, yPos = 1;
    const int OpenedCardLimit = 3;
    public GameObject chosenCard;
    public GameManager GManager;
    public bool discardactivated;

    void Update()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Collider2D collider = Physics2D.OverlapPoint(mousePosition);
        if (!GManager.cropCardUsing && !GManager.itemCardUsing)
        {
            if (chosenCard != null)
            {
                if ((collider != null && collider.gameObject != chosenCard) || (collider == null))
                {
                    chosenCard.transform.DOScale(cardPrefab.transform.lossyScale, 0.25f);
                    chosenCard.transform.DOLocalMoveY(yPos, 0.25f);
                    chosenCard.GetComponent<BoxCollider2D>().offset = new Vector2(0, 0);
                    chosenCard.GetComponent<BoxCollider2D>().size = new Vector2(1, 1f);
                    InitializeAllCardsPositions();
                    chosenCard = null;
                }
                if (Input.GetMouseButtonDown(0))
                {
                    CardScr cardData = chosenCard.GetComponent<CardData>().Card;
                    if (discardactivated) { PutDiscardThisCard(chosenCard); }
                    else
                    {
                        chosenCard.transform.DOMove(new Vector3(6, 0.5f, 0), 0.25f);
                        chosenCard.transform.parent = null;
                        InitializeAllCardsPositions();
                    }


                    if (cardData is CropScr)
                    {
                        cardData.Use();
                        GManager.cropCardUsing = chosenCard;
                        chosenCard.transform.parent = null;
                        InitializeAllCardsPositions();
                    }
                    else if (cardData is ItemScr ItemCardData)
                    {
                        GManager.itemCardUsing = chosenCard;
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
                        chosenCard.transform.parent = null;
                        InitializeAllCardsPositions();
                    }

                }
            }
            else
            {
                if (collider != null && collider.gameObject.CompareTag("Card"))
                {
                    chosenCard = collider.gameObject;
                    chosenCard.transform.DOLocalMoveY(yPos + 1.5f, 0.25f);
                    chosenCard.transform.DOScale(cardPrefab.transform.lossyScale * 1.5f, 0.25f);
                    chosenCard.GetComponent<SpriteRenderer>().sortingOrder = 50;
                    chosenCard.transform.GetChild(0).GetComponent<Canvas>().sortingOrder = 51;
                    chosenCard.GetComponent<BoxCollider2D>().offset = new Vector2(0, -0.165f);
                    chosenCard.GetComponent<BoxCollider2D>().size = new Vector2(1, 1.35f);

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

            Vector2 cardSpawnPosition;
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
        }
    }

    public void ActivatePutDiscard()
    {
        discardactivated = !discardactivated;
    }
    public void PutDiscardThisCard(GameObject ChosenCardToDiscard)
    {
        for (int i = 0; i < openedCards.Count; i++)
        {

            if (ChosenCardToDiscard == openedCards[i])
            {
                discardedCards.Add(openedCards[i].GetComponent<CardData>().Card);
                openedCards.RemoveAt(i);
                Destroy(ChosenCardToDiscard);
                //GManager.money -=**;

                break;
            }
        }
    }
}
