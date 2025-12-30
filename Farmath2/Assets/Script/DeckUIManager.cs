using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

public class DeckUIManager : MonoBehaviour
{
    public bool showing;
    public GameObject mainPage;
    public DeckPlacing deckPlacing;
    public GameObject cardPrefab;
    public GameObject Content;
    public List<GameObject> cardObj = new List<GameObject>();
    public Sprite[] cardColors;
    public void ShowDeck()
    {
        showing = !showing;
        GameManager.instance.pageopened = showing;
        mainPage.SetActive(showing);
        GameManager.instance.PlaySound(9);
        if (showing)
        {
            InitializeCardList();
        }
    }
    public void InitializeCardList()
    {
        cardObj.Clear();
        int childCount = Content.transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Destroy(Content.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < deckPlacing.discardedCards.Count; i++)
        {
            GameObject spawnedCard = Instantiate(cardPrefab);

            spawnedCard.transform.parent = Content.transform;
            spawnedCard.transform.SetAsLastSibling();
            spawnedCard.transform.localScale = Vector3.one;
            cardObj.Add(spawnedCard);
            spawnedCard.GetComponent<Image>().sprite = deckPlacing.discardedCards[i] is ItemScr a ? cardColors[0] : cardColors[1];
            spawnedCard.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = deckPlacing.discardedCards[i].Icon;
            spawnedCard.transform.GetChild(1).gameObject.GetComponent<Image>().sprite = deckPlacing.discardedCards[i].Icon;
            spawnedCard.transform.GetChild(2).gameObject.GetComponent<Image>().sprite = deckPlacing.discardedCards[i].Icon;
            spawnedCard.transform.GetChild(3).gameObject.GetComponent<TextMeshProUGUI>().text = LanguageManager.instance.TurnToString(deckPlacing.discardedCards[i].CardName, null);
            spawnedCard.transform.GetChild(4).gameObject.GetComponent<Button>().onClick.RemoveAllListeners();
            int index = i;
            spawnedCard.transform.GetChild(4).gameObject.GetComponent<Button>().onClick.AddListener(() => { DeleteCard(index); });
        }
    }
    public void DeleteCard(int a)
    {
        bool choosed = GameManager.instance.farmers[0].GetComponent<FarmerInfo>().choosed;
        if (!choosed && GameManager.instance.money < 500)
        {
            StartCoroutine(GameManager.instance.ToggleWarning(4, new LocalizedString("MoneyNotEnough", "Translation-1"), null));
            StartCoroutine(GameManager.instance.ShakeTheObj(cardObj[a], 0.2f, 0.05f, 0, true));
            return;
        }
        if (!choosed) { GameManager.instance.money -= 500; }

        GameManager.instance.InitializeMoneyText();
        GameManager.instance.PlaySound(9);
        Destroy(cardObj[a]);
        cardObj.RemoveAt(a);
        deckPlacing.discardedCards.RemoveAt(a);
        InitializeCardList();
    }
}
