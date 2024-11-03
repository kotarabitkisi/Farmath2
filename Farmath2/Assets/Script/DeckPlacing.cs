using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeckPlacing : MonoBehaviour
{
    [ExecuteInEditMode]
    public GameObject CardPrefab;
    public CardScr[] allCardScr;
    public bool takeRandomized;
    public List<CardScr> discardedCards;
    public List<GameObject> openedCards;
    public bool cardCollected;
    const float xPos = 1, yPos = 1;
    const int OpenedCardLimit = 3;
    public GameObject chosenCard;
    void Update()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Collider2D collider = Physics2D.OverlapPoint(mousePosition);
        if (chosenCard != null)
        {
            if (collider != null && collider.gameObject != chosenCard)
            {
                chosenCard.transform.DOScale(CardPrefab.transform.lossyScale, 0.25f);
                chosenCard.transform.DOLocalMoveY(yPos, 0.25f);
                InitializeAllCardsPositions();
                chosenCard = null;
            }
            else if (collider == null) {
                chosenCard.transform.DOScale(CardPrefab.transform.lossyScale, 0.25f);
                chosenCard.transform.DOLocalMoveY(yPos, 0.25f);
                InitializeAllCardsPositions();
                chosenCard = null;
            }
        }
        else
        {
            if (collider != null && collider.gameObject.CompareTag("Card"))
            {
                chosenCard = collider.gameObject;
                chosenCard.transform.DOLocalMoveY(yPos + 1.5f, 0.25f);
                chosenCard.transform.DOScale(CardPrefab.transform.lossyScale * 1.5f, 0.25f);
                chosenCard.GetComponent<SpriteRenderer>().sortingOrder = 50;
                chosenCard.transform.GetChild(0).GetComponent<Canvas>().sortingOrder = 51;
            }
        }


    }
    public void DeleteAllOfOpenedCards()
    {
        int Count=openedCards.Count;
        print(Count);
        for (int i = 0; i < Count; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
        openedCards.Clear();
    }
    public IEnumerator DayPassedTakeCard()
    {
        DeleteAllOfOpenedCards();
        for (int i = 0; i < OpenedCardLimit; i++)
        {
            TakeCardFromDiscard();
            yield return new WaitForSecondsRealtime(0.5f);
        }
    }
    public void TakeCardFromDiscard()
    {
        if (discardedCards.Count != 0)
        {
            if (!takeRandomized)
            {
                CardScr Card = discardedCards[0];
                discardedCards.RemoveAt(0);
                GameObject newOpenedCard = Instantiate(CardPrefab, new Vector2(6.75f, -3.2f), Quaternion.identity);
                newOpenedCard.transform.SetParent(transform, true);
                GameObject newOpenedCardCanvas = newOpenedCard.transform.GetChild(0).gameObject;
                newOpenedCardCanvas.transform.GetChild(0).GetComponent<Image>().sprite = Card.Icon;
                newOpenedCardCanvas.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = Card.CardName;
                openedCards.Add(newOpenedCard);
                InitializeAllCardsPositions();
            }
            else
            {
                int cardIndex = Random.Range(0, discardedCards.Count);
                CardScr Card = discardedCards[cardIndex];
                discardedCards.RemoveAt(cardIndex);
                GameObject newOpenedCard = Instantiate(CardPrefab, new Vector2(6.75f, -3.2f), Quaternion.identity);
                newOpenedCard.transform.SetParent(transform, true);
                GameObject newOpenedCardCanvas = newOpenedCard.transform.GetChild(0).gameObject;
                newOpenedCardCanvas.transform.GetChild(0).GetComponent<Image>().sprite = Card.Icon;
                newOpenedCardCanvas.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = Card.CardName;
                openedCards.Add(newOpenedCard);
                InitializeAllCardsPositions();

            }
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
}
