using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public GameManager GameManag;
    public DeckPlacing DP;
    public List<CardScr> cardsOnShop = new List<CardScr>();
    public List<float> cardCosts;
    public Image[] allIcons;
    public TextMeshProUGUI[] allShopNameTxt;
    public TextMeshProUGUI[] allShopCostTxt;


    public void BuyCard(int ShopIndex)
    {
        if (GameManag.money >= cardCosts[ShopIndex])
        {
            GameManag.money -= cardCosts[ShopIndex];
            GameManag.InitializeMoneyText();
            DP.AddCardToDiscard(cardsOnShop[ShopIndex]);
            DeleteCard(ShopIndex);
        }
        else { print("fakir"); }
    }
    public void AddCardToShop(int ShopIndex, int CardID)
    {
        allIcons[ShopIndex].GetComponent<Button>().interactable = true;
        CardScr chosenCard = DP.allCardScr[CardID];
        allIcons[ShopIndex].sprite = chosenCard.Icon;
        float CardCost;
        if (GameManag.debuffs[0]) { CardCost = chosenCard.CardCost*(1+GameManag.Day*0.05f); }
        else { CardCost = chosenCard.CardCost; }
        allShopNameTxt[ShopIndex].text = chosenCard.CardName;
        allShopCostTxt[ShopIndex].text = CardCost.ToString("0");
        cardsOnShop.Add(chosenCard);
        cardCosts.Add(CardCost);
    }
    public void DeleteCard(int ShopIndex)
    {
        cardsOnShop[ShopIndex] = null;
        cardCosts[ShopIndex] = 0;
        allIcons[ShopIndex].GetComponent<Button>().interactable = false;
    }
}
