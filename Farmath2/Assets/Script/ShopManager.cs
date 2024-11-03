using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
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
            DP.AddCardToDiscard(cardsOnShop[ShopIndex]);
            DeleteCard(ShopIndex);
        }
        else { print("fakir"); }
    }
    public void AddCardToShop(int ShopIndex,int CardID)
    {
        allIcons[ShopIndex].GetComponent<Button>().interactable = true;
        CardScr chosenCard = DP.allCardScr[CardID];
        allIcons[ShopIndex].sprite = chosenCard.Icon;
        allShopNameTxt[ShopIndex].text = chosenCard.CardName;
        allShopCostTxt[ShopIndex].text = chosenCard.CardCost.ToString("0");
        cardsOnShop.Add(DP.allCardScr[ShopIndex]);
        cardCosts.Add(chosenCard.CardCost);
    }
    public void DeleteCard(int ShopIndex) {
        cardsOnShop[ShopIndex]=null;
        cardCosts[ShopIndex] = 0;
        allIcons[ShopIndex].GetComponent<Button>().interactable = false;
    }
}
