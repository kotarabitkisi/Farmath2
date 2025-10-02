using System.Collections.Generic;
using System.IO;
using UnityEngine;
public class saveAndLoad : MonoBehaviour
{
    public SettingsData defaultSettings;
    public GameManager GManager;
    public DeckPlacing OpenedDeck;
    public FarmInfo[] FarmInfos;
    public GameObject[] FarmerPlace;
    public CardScr[] ItemCards;
    public CardScr[] CropCards;
    public Data data;
    public ExplorationData exploration;
    public SettingsData settings;
    public LevelSettings levelSettings;
    private void Start()
    {
        Load();
    }
#if UNITY_ANDROID || UNITY_IOS
    private void OnApplicationPause()
    {
        if (GManager.Month <= 4)
        {
            Save();
            SaveExploration();
            SaveSettings();
        }
    }
#endif
    private void OnApplicationQuit()
    {
        if (GManager.Month <= 4)
        {
            Save();
            SaveExploration();
            SaveSettings();
        }
    }
    public void Save()
    {
        #region kartlar
        data.OpenedCardIds.Clear();
        data.OpenedCardType.Clear();
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
        data.DiscardedCardIds.Clear();
        data.DiscardedCardType.Clear();
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
        }
        #endregion
        #region genel datalar
        data.tutorialFinished = GManager.tutorialPlayed;
        data.Boss = GManager.Boss;
        data.money = GManager.money;
        data.Day = GManager.Day;
        data.Month = GManager.Month;
        List<float> Qrewards = GManager.ShopManagement.Qreward;
        data.Qreward.Clear();
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
        data.farmerChoosed.Clear();
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

    public void SaveSettings()
    {
        SettingsData SettingsData = new SettingsData();
        #region ayarlar
        SettingsData.pollen = levelSettings.pollenToggle.isOn;
        SettingsData.FPS = levelSettings.FpsDropdown.value;
        SettingsData.grass = levelSettings.grassToggle.isOn;
        SettingsData.shadow = levelSettings.shadowToggle.isOn;
        SettingsData.lights = levelSettings.lightToggle.isOn;
        SettingsData.postProcessing = levelSettings.volToggle.isOn;
        SettingsData.vignette = levelSettings.vigToggle.isOn;
        SettingsData.Bloom = levelSettings.bloomToggle.isOn;
        SettingsData.colorAdj = levelSettings.colorAdjToggle.isOn;
        SettingsData.HDR = levelSettings.hdrToggle.isOn;
        SettingsData.renderScaleLevel = levelSettings.renderScaleSlider.value;
        SettingsData.soundLevel = levelSettings.soundSlider.value;
        #endregion
        string json3 = JsonUtility.ToJson(SettingsData, true);
        string SettingsPath = Application.persistentDataPath + "/Settings.json";
        File.WriteAllText(SettingsPath, json3);
        print("Settings Saved");
    }
    public void Load()
    {

        string path = Application.persistentDataPath + "/playerData.json";
        string Explorationpath = Application.persistentDataPath + "/playerExplorations.json";
        string SettingsPath = Application.persistentDataPath + "/Settings.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);

            data = JsonUtility.FromJson<Data>(json);
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

            #endregion
            #region anastatlar
            GManager.tutorialPlayed = data.tutorialFinished;
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
            }
            #endregion

        }
        if (File.Exists(SettingsPath))
        {
            string json3 = File.ReadAllText(SettingsPath);
            settings = JsonUtility.FromJson<SettingsData>(json3);
            #region ayarlar
            levelSettings.FpsDropdown.value = settings.FPS;
            levelSettings.pollenToggle.isOn = settings.pollen;
            levelSettings.grassToggle.isOn = settings.grass;
            levelSettings.shadowToggle.isOn = settings.shadow;
            levelSettings.lightToggle.isOn = settings.lights;
            levelSettings.volToggle.isOn = settings.postProcessing;
            levelSettings.vigToggle.isOn = settings.vignette;
            levelSettings.bloomToggle.isOn = settings.Bloom;
            levelSettings.colorAdjToggle.isOn = settings.colorAdj;
            levelSettings.hdrToggle.isOn = settings.HDR;
            levelSettings.renderScaleSlider.value = settings.renderScaleLevel;
            levelSettings.soundSlider.value = settings.soundLevel;
            levelSettings.AllSettings();
            #endregion
        }
        else
        {
            levelSettings.FpsDropdown.value = defaultSettings.FPS;
            levelSettings.pollenToggle.isOn = defaultSettings.pollen;
            levelSettings.grassToggle.isOn = defaultSettings.grass;
            levelSettings.shadowToggle.isOn = defaultSettings.shadow;
            levelSettings.lightToggle.isOn = defaultSettings.lights;
            levelSettings.volToggle.isOn = defaultSettings.postProcessing;
            levelSettings.vigToggle.isOn = defaultSettings.vignette;
            levelSettings.bloomToggle.isOn = defaultSettings.Bloom;
            levelSettings.colorAdjToggle.isOn = defaultSettings.colorAdj;
            levelSettings.hdrToggle.isOn = defaultSettings.HDR;
            levelSettings.renderScaleSlider.value = defaultSettings.renderScaleLevel;
            levelSettings.soundSlider.value = defaultSettings.soundLevel;
            levelSettings.AllSettings();
        }
        if (File.Exists(Explorationpath))
        {
            string json2 = File.ReadAllText(Explorationpath);
            exploration = JsonUtility.FromJson<ExplorationData>(json2);
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
    public bool tutorialFinished;
    public float money;
    public int Day;
    public int Month;
    public bool[] debuffs;
    public int[] HarvestedCropCount;
    public int hoeCount;
    public int Boss;
    public List<float> Qreward = new List<float>();
    [Space(3)]
    [Header("FarmInfo")]
    public int[] farmId;
    public int[] curDay;
    public int[] reqDay;
    public bool[] watered;
    public bool[] holyHoed;
    public bool[] negatived;
    [Space(3)]
    [Header("CardInfo")]
    public List<int> OpenedCardIds = new List<int>();
    public List<int> DiscardedCardIds = new List<int>();
    public List<int> OpenedCardType = new List<int>();
    public List<int> DiscardedCardType = new List<int>();
    [Space(3)]
    [Header("FarmerInfo")]
    public List<bool> farmerChoosed = new List<bool>();
    public int[] farmerCount;
}
[System.Serializable]
public class SettingsData
{
    [Header("Settings")]
    public int FPS;
    public bool grass, shadow;
    public bool lights;
    public bool pollen;
    public float renderScaleLevel;
    public float soundLevel;
    public bool postProcessing, Bloom, colorAdj, vignette;
    public bool HDR;
}
[System.Serializable]
public class ExplorationData
{
    public bool[] isExplored;
}


