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
    public GameObject[] ShopCardObj;
    public TextMeshProUGUI[] allShopNameTxt;
    public TextMeshProUGUI[] allShopCostTxt;
    public List<float> Qreward;
    public bool[] Shaking;
    public void BuyCard(int ShopIndex)
    {
        if (cardsOnShop[ShopIndex] == DP.allCardScr[11])
        {
            Qreward.Add(cardCosts[ShopIndex]);
        }
        if (GameManag.money >= cardCosts[ShopIndex])
        {
            GameManag.money -= cardCosts[ShopIndex];
            GameManag.InitializeMoneyText();
            DP.AddCardToDiscard(cardsOnShop[ShopIndex]);
            DeleteCard(ShopIndex);
        }
        else
        {
            StartCoroutine(GameManag.ShakeTheObj(ShopCardObj[ShopIndex], 0.25f, 10f, 10f,true));
            

        }
    }
    public void AddCardToShop(int ShopIndex, int CardID)
    {
        bool AllHolyHoed=true;
        for (int i = 0; i < 24; i++)
        {
            if (!GameManag.farmsScr.frams[i].HolyHoed) { AllHolyHoed = false; break; }
        }
        if(AllHolyHoed) {
            while (CardID == 8)
            {
                CardID = Random.Range(0, DP.allCardScr.Length);
            }
        }
        
        allIcons[ShopIndex].GetComponent<Button>().interactable = true;
        CardScr chosenCard = DP.allCardScr[CardID];
        allIcons[ShopIndex].sprite = chosenCard.Icon;
        #region para belirleme
        float CardCost;
        CardCost = chosenCard.CardCost;
        if (CardID == 11)
        {
            CardCost = GameManag.money * 0.05f;
            if (CardCost <= 100) { CardCost = 100; }
        }
        if (GameManag.debuffs[0]) { CardCost *= (1 + GameManag.Day * 0.05f); }
        if (GameManag.farmers[1].GetComponent<FarmerInfo>().choosed) { CardCost *= 0.9f; }
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
            AddCardToShop(i, Random.Range(0, GameManag.deckPlacing.allCardScr.Length));
        }
    }
}
