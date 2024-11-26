using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject cropCardUsing;
    public GameObject itemCardUsing;
    [Header("MainStats")]
    public CropStats[] crops;
    public float money;
    public int Day;
    const float pageAnimTime = 0.5f;
    const int reqDay = 365;
    const int maxCardCountOnShop = 6;
    const int maxExploreCount = 6;
    public bool pageopened;
    [Space(10)]

    public ShopManager ShopManagement;
    public DeckPlacing deckPlacing;

    public GameObject WinPanel;
    public GameObject LosePanel;
    public int[] HarvestedCropCount;
    public int HoeCount;
    public int[] HoeCost;
    public GameObject ShopImage;
    public GameObject PageParent;

    public Farms farmsScr;
    public TextMeshProUGUI MoneyText, Daytext;
    [Header("Explore")]
    public float ColorSpeed;
    public Color textColor, textColor2;
    public GameObject ExploreNotification;
    public TextMeshProUGUI ExploreNotificationTxt;
    public bool[] isExplored;
    public string[] ExploreText;
    public TextMeshProUGUI[] ExploreTexts;
    public bool[] isTextColorful;
    public GameObject ExplorePage;
    private void Start()
    {
        InitializeMoneyText();
    }
    private void Update()
    {
        #region TýklamaKontrolleri
        if (Input.GetMouseButtonDown(0) && !pageopened)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
            print(hit.collider.gameObject);
            if (hit.collider != null && hit.collider.CompareTag("Farm"))
            {
                farmsScr.ChosenFarm = hit.collider.gameObject.GetComponent<FarmInfo>();
                FarmInfo farm = hit.collider.gameObject.GetComponent<FarmInfo>();
                if (farm.Id == 1 && cropCardUsing)
                {
                    CardScr card = cropCardUsing.GetComponent<CardData>().Card;
                    if (card is CropScr CropCard)
                    {
                        SeedCrop(CropCard.id);
                    }
                    for (int i = 0; i < deckPlacing.openedCards.Count; i++)
                    {
                        if (deckPlacing.openedCards[i] == cropCardUsing) { deckPlacing.openedCards.RemoveAt(i); }
                    }
                    Destroy(cropCardUsing);
                }
                else if (itemCardUsing)
                {
                    CardScr card = itemCardUsing.GetComponent<CardData>().Card;
                    if (card is ItemScr ItemCard)
                    {
                        switch (ItemCard.itemId)
                        {
                            case 0: farm.reqDay /= 2; break;
                            case 2:
                                if (farm.Id < 2)
                                {
                                    farm.HolyHoed = true;
                                }
                                else { }
                                break;
                            case 3:
                                farm.Id = 1;
                                farm.curDay = 0;
                                farm.reqDay = 0;
                                break;


                        }
                    }
                    for (int i = 0; i < deckPlacing.openedCards.Count; i++)
                    {
                        if (deckPlacing.openedCards[i] == itemCardUsing) { deckPlacing.openedCards.RemoveAt(i); }
                    }
                    Destroy(itemCardUsing);
                }
                if (farm.Id == 0) { farm.HoeImage.SetActive(true); pageopened = true; }
                else if (farm.curDay >= farm.reqDay&&farm.Id>=3)
                {
                    HarvestCrop(farm);
                }
            }
            else
            {

            }
        }
        else if (Input.GetMouseButtonDown(1) && cropCardUsing)
        {
            cropCardUsing = null;
            deckPlacing.chosenCard.transform.parent = deckPlacing.transform;
        }
        #endregion
        for (int i = 0; i < isTextColorful.Length; i++)
        {
            if (isTextColorful[i])
            {
                ExploreTexts[i].color = Color.Lerp(textColor, textColor2, (Mathf.Sin(Time.time * ColorSpeed) + 1) / 2);
            }
        }
    }
    public void InitializeMoneyText()
    {
        switch (money)
        {
            case < 1000: MoneyText.text = money.ToString("F2"); break;
            case < 1000000: MoneyText.text = (money / 1000).ToString("F2") + "K"; break;
            case < 1000000000: MoneyText.text = (money / 1000000).ToString("F2") + "M"; break;
        }
    }
    public void HarvestCrop(FarmInfo farm)
    {
        CropStats cropStats = crops[farm.Id];
        float baseRev = cropStats.base_;
        float MultipleRev = 1;



        if (cropStats.IsNeighbour(farm.connectedFarmIds))
        {
            int id = (farm.Id - 2) * maxExploreCount;
            if (!isExplored[id])
            {
                isExplored[id] = true;
                ExploreTexts[id].text = ExploreText[id];
                isTextColorful[id] = true;
                ExploreNotification.SetActive(true);
            }

            switch (cropStats.RevOperation[0])
            {
                case 0:
                    baseRev += cropStats.Rev[0];
                    break;
                case 1:
                    MultipleRev += cropStats.Rev[0];
                    break;
            }
        }
        if (cropStats.IsHarvestDayPassed(farm))
        {
            int id = (farm.Id - 2) * maxExploreCount + 1;
            if (!isExplored[id])
            {
                isExplored[id] = true;
                ExploreTexts[id].text = ExploreText[id];
                isTextColorful[id] = true;
                ExploreNotification.SetActive(true);

            }
            switch (cropStats.RevOperation[1])
            {
                case 0:
                    baseRev += cropStats.Rev[1];
                    break;
                case 1:
                    MultipleRev += cropStats.Rev[1];
                    break;
            }
        }
        if (cropStats.IsMoneyBetweenTheseNum(money))
        {
            int id = (farm.Id - 2) * maxExploreCount + 2;
            if (!isExplored[id])
            {
                isExplored[id] = true;
                ExploreTexts[id].text = ExploreText[id];
                isTextColorful[id] = true;
                ExploreNotification.SetActive(true);
            }
            switch (cropStats.RevOperation[2])
            {
                case 0:
                    baseRev += cropStats.Rev[2];
                    break;
                case 1:
                    MultipleRev += cropStats.Rev[2];
                    break;
            }
        }
        if (cropStats.IsDayBetweenTheseNum(Day))
        {
            int id = (farm.Id - 2) * maxExploreCount + 3;
            if (!isExplored[id])
            {
                isExplored[id] = true;
                ExploreTexts[id].text = ExploreText[id];
                isTextColorful[id] = true;
                ExploreNotification.SetActive(true);
            }
            switch (cropStats.RevOperation[3])
            {
                case 0:
                    baseRev += cropStats.Rev[3];
                    break;
                case 1:
                    MultipleRev += cropStats.Rev[3];
                    break;
            }
        }
        if (cropStats.IsHarvestCount(true, Day))
        {
            int id = (farm.Id - 2) * maxExploreCount + 4;
            if (!isExplored[id])
            {
                isExplored[id] = true;
                ExploreTexts[id].text = ExploreText[id];
                isTextColorful[id] = true;
                ExploreNotification.SetActive(true);
            }
            switch (cropStats.RevOperation[4])
            {
                case 0:
                    baseRev += cropStats.Rev[4];
                    break;
                case 1:
                    MultipleRev += cropStats.Rev[4];
                    break;
            }
        }
        if (cropStats.IsHarvestCount(false, Day))
        {
            int id = (farm.Id - 2) * maxExploreCount + 5;
            if (!isExplored[id])
            {
                isExplored[id] = true;
                ExploreTexts[id].text = ExploreText[id];
                isTextColorful[id] = true;
                ExploreNotification.SetActive(true);
            }
            switch (cropStats.RevOperation[5])
            {
                case 0:
                    baseRev += cropStats.Rev[5];
                    break;
                case 1:
                    MultipleRev += cropStats.Rev[5];
                    break;
            }
        }
        if (farm.Watered) { MultipleRev *= 2; }
        if (farm.HolyHoed) { MultipleRev *= 2; }
        #region explorebildirimi
        int ExploreNotificationTextCount = 0;
        for (int i = 0; i < ExploreTexts.Length; i++)
        {
            if (isTextColorful[i])
            {
                ExploreNotificationTextCount++;
            }
        }
        ExploreNotificationTxt.text = ExploreNotificationTextCount.ToString();
        #endregion
        farm.Id = 1;
        farm.curDay = 0;
        farm.reqDay = 0;
        money += baseRev * MultipleRev;
        InitializeMoneyText();
    }
    public void SeedCrop(int id)
    {
        farmsScr.ChosenFarm.Id = id;
        farmsScr.ChosenFarm.curDay = 0;
        farmsScr.ChosenFarm.reqDay = crops[id].reqDayToGrow;
        InitializeMoneyText();
    }
    public void Hoe(FarmInfo farm)
    {
        if (money >= 10 * Mathf.Pow(1.45f, HoeCount))
        {
            money -= 10 * Mathf.Pow(1.45f, HoeCount);
            farm.Id = 1;
            HoeCount++;
        }


        CloseHoeMenuvoid(farm);
        InitializeMoneyText();
    }
    public void CloseHoeMenuvoid(FarmInfo farm)
    {
        farm.HoeImage.SetActive(false);
        pageopened = false;
    }
    public void UIPageAnimVoid(GameObject Obj)
    {

        StartCoroutine(UIPageAnim(Obj, pageopened));
    }
    public IEnumerator UIPageAnim(GameObject Obj, bool pageopened)
    {
        if (!pageopened)
        {
            Obj.SetActive(true);
            PageParent.GetComponent<RectTransform>().DOMoveX(Screen.width / 2, pageAnimTime);
            yield return new WaitForSecondsRealtime(pageAnimTime);
            this.pageopened = true;
        }
        else
        {
            PageParent.GetComponent<RectTransform>().DOMoveX(-Obj.GetComponent<RectTransform>().rect.width / 2, pageAnimTime);
            yield return new WaitForSecondsRealtime(pageAnimTime);
            Obj.SetActive(false);
            this.pageopened = false;
            TurnOffColor();
        }

    }
    public void NextDay()
    {
        Day++;
        Daytext.text = "Day: " + Day + "/" + reqDay;
        StartCoroutine(deckPlacing.DayPassedTakeCard());
        ShopManagement.cardsOnShop.Clear();
        ShopManagement.cardCosts.Clear();
        for (int i = 0; i < maxCardCountOnShop; i++)
        {
            ShopManagement.AddCardToShop(i, Random.Range(0, deckPlacing.allCardScr.Length));
        }
        if (reqDay <= Day)
        {
            if (money >= 1000000) { Win(); }
            else
            {
                Lose();
            }
        }
        else
        {
            foreach (FarmInfo farmland in farmsScr.FarmList)
            {
                farmland.curDay++;
            }
        }
    }
    public void Win()
    {
        WinPanel.SetActive(true);
        pageopened = true;
        WinPanel.transform.DOScale(new Vector3(1, 1, 1) * 1, pageAnimTime);
    }
    public void Lose()
    {
        LosePanel.SetActive(true);
        pageopened = true;
        LosePanel.transform.DOScale(new Vector3(1, 1, 1) * 1, pageAnimTime);
    }
    public void TurnOffColor()
    {
        for (int i = 0; i < ExploreText.Length; i++)
        {
            ExploreNotification.SetActive(false);
            isTextColorful[i] = false;
            ExploreTexts[i].color = Color.white;
        }

    }
}
