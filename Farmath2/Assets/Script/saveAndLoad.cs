using System.Collections.Generic;
using System.IO;
using UnityEngine;
public class saveAndLoad : MonoBehaviour
{
    public GameManager GManager;
    public DeckPlacing OpenedDeck;
    public FarmInfo[] FarmInfos;
    public GameObject[] FarmerPlace;
    public CardScr[] ItemCards;
    public CardScr[] CropCards;
    public Data data;
    public ExplorationData exploration;
    private void Start()
    {
        Load();
    }
    private void OnApplicationQuit()
    {
        Save();
        SaveExploration();
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
            data.watered[i] = FarmInfos[i].Watered;
            data.holyHoed[i] = FarmInfos[i].HolyHoed;
            data.negatived[i] = FarmInfos[i].Negatived;
            data.firstBossEffected[i] = FarmInfos[i].firstBossEffected;
        }
        #endregion
        #region genel datalar
        data.Boss = GManager.Boss;
        data.money = GManager.money;
        data.Day = GManager.Day;
        data.Month = GManager.Month;
        data.firstBossEffectCount = GManager.bossManager.BossEffectCount;
        List<float> Qrewards = GManager.ShopManagement.Qreward;
        for (int i = 0; i < Qrewards.Count; i++)
        {
            data.Qreward.Add(Qrewards[i]);
        }
        for (int i = 0; i < data.debuffs.Length; i++)
        {
            data.debuffs[i] = GManager.debuffs[i];
        }

        for (int i = 0; i < data.HarvestedCropCount.Length; i++)
        {
            data.HarvestedCropCount[i] = GManager.HarvestedCropCount[i];
        }
        data.hoeCount = GManager.HoeCount;

        #endregion
        #region Çiftçiler
        for (int i = 0; i < GManager.farmers.Length; i++)
        {
            data.farmerChoosed.Add(GManager.farmers[i].GetComponent<FarmerInfo>().choosed);
        }
        for (int i = 0; i < data.farmerCount.Length; i++)
        {
            data.farmerCount[i] = GManager.farmerCount[i];
        }


        #endregion

        string json = JsonUtility.ToJson(data, true);

        string path = Application.persistentDataPath + "/playerData.json";

        File.WriteAllText(path, json);

        Debug.Log("Data saved to: " + path);
    }
    public void SaveExploration()
    {
        for (int i = 0; i < exploration.isExplored.Length; i++)
        {
            exploration.isExplored[i] = GManager.isExplored[i];
        }
        string json2 = JsonUtility.ToJson(exploration, true);
        string Explorationpath = Application.persistentDataPath + "/playerExplorations.json";
        File.WriteAllText(Explorationpath, json2);
        print("Exploration Saved");
    }
    public void Load()
    {
        string path = Application.persistentDataPath + "/playerData.json";
        string Explorationpath = Application.persistentDataPath + "/playerExplorations.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);

            Data data = JsonUtility.FromJson<Data>(json);
            #region çiftçiler
            int choosedCount = 0;
            for (int i = 0; i < data.farmerChoosed.Count; i++)
            {
                if (data.farmerChoosed[i] && choosedCount < 3)
                {
                    GManager.farmers[i].GetComponent<FarmerInfo>().HireHero(GManager.farmers[i], FarmerPlace[choosedCount]);
                    choosedCount++;
                }
                GManager.farmers[i].GetComponent<FarmerInfo>().choosed = data.farmerChoosed[i];
            }
            for (int i = 0; i < data.farmerCount.Length; i++)
            {
                GManager.farmerCount[i] = data.farmerCount[i];
            }
            #endregion
            #region kartstatlarý
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
                int CardType = data.OpenedCardType[i];
                print("type=" + CardType + "\nCardId=" + cardId);
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
            #endregion
            #region anastatlar
            GManager.bossManager.BossEffectCount = data.firstBossEffectCount;
            GManager.HoeCount = data.hoeCount;
            GManager.money = data.money;
            GManager.Day = data.Day;
            GManager.Month = data.Month;
            GManager.Boss = data.Boss;
            for (int i = 0; i < data.Qreward.Count; i++)
            {
                GManager.ShopManagement.Qreward.Add(data.Qreward[i]);
            }
            for (int i = 0; i < data.debuffs.Length; i++)
            {
                GManager.debuffs[i] = data.debuffs[i];
            }

            for (int i = 0; i < data.HarvestedCropCount.Length; i++)
            {
                GManager.HarvestedCropCount[i] = data.HarvestedCropCount[i];
            }
            #endregion
            #region farmbilgileri
            for (int i = 0; i < FarmInfos.Length; i++)
            {
                FarmInfos[i].Id = data.farmId[i];
                FarmInfos[i].curDay = data.curDay[i];
                FarmInfos[i].reqDay = data.reqDay[i];
                FarmInfos[i].Watered = data.watered[i];
                FarmInfos[i].HolyHoed = data.holyHoed[i];
                FarmInfos[i].Negatived = data.negatived[i];
                FarmInfos[i].firstBossEffected = data.firstBossEffected[i];
            }
            #endregion

        }
        if (File.Exists(Explorationpath))
        {
            string json2 = File.ReadAllText(Explorationpath);
            ExplorationData exploration = JsonUtility.FromJson<ExplorationData>(json2);
            for (int i = 0; i < exploration.isExplored.Length; i++)
            {
                GManager.isExplored[i] = exploration.isExplored[i];
                if (GManager.isExplored[i])
                {
                    GManager.OpenExplore(i, true);
                }
            }
        }
        else
        {
            Debug.LogError("Save file not found!");

        }
        GManager.InitializeLoad();
    }
    public void EraseSaves()
    {
        string dosyaYolu = Application.persistentDataPath + "/playerData.json";

        if (File.Exists(dosyaYolu))
        {
            File.Delete(dosyaYolu);
            Debug.Log("Kayýt dosyasý baþarýyla silindi.");
        }
        else
        {
            Debug.LogError("Silinecek dosya bulunamadý.");
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
    public int[] HarvestedCropCount;
    public int hoeCount;
    public int Boss;
    public int firstBossEffectCount;
    public List<float> Qreward;
    [Space(3)]
    [Header("FarmInfo")]
    public int[] farmId;
    public int[] curDay;
    public int[] reqDay;
    public bool[] watered;
    public bool[] holyHoed;
    public bool[] negatived;
    public bool[] firstBossEffected;
    [Space(3)]
    [Header("CardInfo")]
    public List<int> OpenedCardIds;
    public List<int> DiscardedCardIds;
    public List<int> OpenedCardType;
    public List<int> DiscardedCardType;
    [Space(3)]
    [Header("FarmerInfo")]
    public List<bool> farmerChoosed;
    public int[] farmerCount;
}
[System.Serializable]
public class ExplorationData
{
    public bool[] isExplored;
}


