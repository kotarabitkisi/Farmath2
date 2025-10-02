using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public GameManager gameManager;
    public DeckPlacing DP;
    public List<CardScr> cardsOnShop = new List<CardScr>();
    public List<float> cardCosts;
    public Image[] allIcons;
    public GameObject[] ShopCardObj;
    public TextMeshProUGUI[] allShopNameTxt;
    public TextMeshProUGUI[] allShopCostTxt;
    public List<float> Qreward;
    public bool[] Shaking;
    public void BuyCard(int ShopIndex)
    {


        if (gameManager.money >= cardCosts[ShopIndex])
        {
            if (cardsOnShop[ShopIndex] == DP.allCardScr[11])
            {
                Qreward.Add(cardCosts[ShopIndex]);
            }
            gameManager.money -= cardCosts[ShopIndex];
            gameManager.InitializeMoneyText();
            DP.AddCardToDiscard(cardsOnShop[ShopIndex]);
            DeleteCard(ShopIndex);
            if (!gameManager.tutorialPlayed && !gameManager.logger.tutorials[1] && gameManager.logger.textingFinished)
            {
                gameManager.logger.StartDialouge(1);
            }
        }
        else
        {
            StartCoroutine(gameManager.ShakeTheObj(ShopCardObj[ShopIndex], 0.25f, 10f, 10f, true));
        }
    }
    public void AddCardToShop(int ShopIndex, int CardID)
    {

        bool AllHolyHoed = true;
        for (int i = 0; i < 12; i++)
        {
            if (!gameManager.farmsScr.frams[i].HolyHoed) { AllHolyHoed = false; break; }
        }
        if (AllHolyHoed)
        {
            while (CardID == 8)
            {
                CardID = Random.Range(6, DP.allCardScr.Length);
            }
        }

        allIcons[ShopIndex].GetComponent<Button>().interactable = true;
        CardScr chosenCard = DP.allCardScr[CardID];
        allIcons[ShopIndex].sprite = chosenCard.Icon;
        #region para belirleme
        float CardCost;
        CardCost = chosenCard.CardCost;
        if (CardID == 11)//soru kartýysa
        {
            CardCost = gameManager.money * 0.1f;
            if (CardCost <= 500) { CardCost = 500; }
        }
        if (gameManager.debuffs[0]) { CardCost *= (1 + gameManager.Day * 0.05f); }
        if (gameManager.farmers[1].GetComponent<FarmerInfo>().choosed) { CardCost *= 0.9f; }
        #endregion
        allShopNameTxt[ShopIndex].text = chosenCard.CardName;
        allShopCostTxt[ShopIndex].text = CardCost.ToString("0") + "$";
        cardsOnShop.Add(chosenCard);
        cardCosts.Add(CardCost);
    }
    public void DeleteCard(int ShopIndex)
    {
        cardsOnShop[ShopIndex] = null;
        cardCosts[ShopIndex] = 0;
        allIcons[ShopIndex].GetComponent<Button>().interactable = false;
    }

    public void DayPassedAddCard()
    {
        cardsOnShop.Clear();
        cardCosts.Clear();
        for (int i = 0; i < 6; i++)
        {
            if (i == 5)
            {
                AddCardToShop(i, Random.Range(6, gameManager.deckPlacing.allCardScr.Length));
            }
            else
            {
                AddCardToShop(i, Random.Range(0, 6));
            }
        }
    }
}
