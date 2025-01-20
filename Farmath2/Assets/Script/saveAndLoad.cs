using System.Collections.Generic;
using System.IO;
using UnityEngine;
public class saveAndLoad : MonoBehaviour
{
    public GameManager GManager;
    public DeckPlacing OpenedDeck;
    public Data data;
    public FarmInfo[] FarmInfos;


    public CardScr[] ItemCards;
    public CardScr[] CropCards;
    private void Start()
    {
        Load();
    }
    private void OnApplicationQuit()
    {
        Save();
    }

    public void Save()
    {

        #region kartlar
        for (int i = 0; i < OpenedDeck.openedCards.Count; i++)
        {
            CardScr card = OpenedDeck.openedCards[i].GetComponent<CardData>().Card;
            if (card is CropScr CropCard)
            {
                data.OpenedCardIds.Add(CropCard.id);
                data.OpenedCardType.Add(0);
            }
            if (card is ItemScr ItemCard)
            {
                data.OpenedCardIds.Add(ItemCard.itemId);
                data.OpenedCardType.Add(1);
            }
        }
        for (int i = 0; i < OpenedDeck.discardedCards.Count; i++)
        {
            CardScr card = OpenedDeck.discardedCards[i];
            if (card is CropScr CropCard)
            {
                data.DiscardedCardIds.Add(CropCard.id);
                data.DiscardedCardType.Add(0);
            }
            if (card is ItemScr ItemCard)
            {
                data.DiscardedCardIds.Add(ItemCard.itemId);
                data.DiscardedCardType.Add(1);
            }
        }
        #endregion


        #region farmlar
        for (int i = 0; i < FarmInfos.Length; i++)
        {
            data.farmId[i] = FarmInfos[i].Id;
            data.curDay[i] = FarmInfos[i].curDay;
            data.reqDay[i] = FarmInfos[i].reqDay;
            data.Watered[i] = FarmInfos[i].Watered;
            data.holyHoed[i] = FarmInfos[i].HolyHoed;
        }
        #endregion







        #region genel datalar
        data.money = GManager.money;
        data.Day = GManager.Day;
        data.Month = GManager.Month;
        for (int i = 0; i < data.debuffs.Length; i++)
        {
            data.debuffs[i] = GManager.debuffs[i];
        }
        for (int i = 0; i < data.isExplored.Length; i++)
        {
            data.isExplored[i] = GManager.isExplored[i];
        }
        for (int i = 0; i < data.HarvestedCropCount.Length; i++)
        {
            data.HarvestedCropCount[i] = GManager.HarvestedCropCount[i];
        }
        #endregion


        string json = JsonUtility.ToJson(data, true);
        string path = Application.persistentDataPath + "/playerData.json";
        File.WriteAllText(path, json);
        Debug.Log("Data saved to: " + path);
    }
    public void Load()
    {




        string path = Application.persistentDataPath + "/playerData.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            Data data = JsonUtility.FromJson<Data>(json);



            for (int i = 0; i < data.DiscardedCardIds.Count; i++)
            {
                int cardId = data.DiscardedCardIds[i];
                int CardType = data.DiscardedCardType[i];
                if (CardType == 0)
                {
                    OpenedDeck.AddCardToDiscard(CropCards[cardId]);

                }
                else { OpenedDeck.AddCardToDiscard(ItemCards[cardId]); }
            }


            for (int i = 0; i < data.OpenedCardIds.Count; i++)
            {
                int cardId = data.OpenedCardIds[i];
                int CardType = data.OpenedCardIds[i];
                if (CardType == 0)
                {
                    OpenedDeck.AddCardToDiscard(CropCards[cardId]);
                    OpenedDeck.TakeCardFromDiscard();
                }
                else
                {
                    OpenedDeck.AddCardToDiscard(ItemCards[cardId]);
                    OpenedDeck.TakeCardFromDiscard();
                }
            }







            GManager.money = data.money;
            GManager.Day = data.Day;
            GManager.Month = data.Month;
            for (int i = 0; i < data.debuffs.Length; i++)
            {
                GManager.debuffs[i] = data.debuffs[i];
            }
            for (int i = 0; i < data.isExplored.Length; i++)
            {
                GManager.isExplored[i] = data.isExplored[i];
            }
            for (int i = 0; i < data.HarvestedCropCount.Length; i++)
            {
                GManager.HarvestedCropCount[i] = data.HarvestedCropCount[i];
            }
            for (int i = 0; i < FarmInfos.Length; i++)
            {
                FarmInfos[i].Id = data.farmId[i];
                FarmInfos[i].curDay = data.curDay[i];
                FarmInfos[i].reqDay = data.reqDay[i];
                FarmInfos[i].Watered = data.Watered[i];
                FarmInfos[i].HolyHoed = data.holyHoed[i];
            }

            return;
        }
        else
        {
            Debug.LogError("Save file not found!");
            return;
        }
    }
}
[System.Serializable]
public class Data
{
    [Header("MainStats")]
    public float money;
    public int Day;
    public int Month;
    public bool[] debuffs;
    public bool[] isExplored;
    public int[] HarvestedCropCount;
    public int HoeCount;
    [Space(3)]
    [Header("FarmInfo")]
    public int[] farmId;
    public int[] curDay;
    public int[] reqDay;
    public bool[] Watered;
    public bool[] holyHoed;
    [Space(3)]
    [Header("CardInfo")]
    public List<int> OpenedCardIds;
    public List<int> DiscardedCardIds;
    public List<int> OpenedCardType;
    public List<int> DiscardedCardType;

}


