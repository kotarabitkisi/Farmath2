using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    const int maxCardCountOnShop=6;
    public ShopManager ShopManagement;
    public DeckPlacing deckPlacing;
    Queue<int> cards = new();//listeleme için kullan
    public GameObject WinPanel;
    public GameObject LosePanel;
    public int[] HarvestedCropCount;
    public int HoeCount;
    public int[] HoeCost;
    public GameObject ShopImage;
    public CropStats[] crops;
    public Farms farmsScr;
    public TextMeshProUGUI MoneyText, Daytext;
    public float money;
    public int Day;
    const int reqDay = 365;
    public bool pageopened;
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
            if (hit.collider != null)
            {
                farmsScr.ChosenFarm = hit.collider.gameObject.GetComponent<FarmInfo>();
                FarmInfo farm = hit.collider.gameObject.GetComponent<FarmInfo>();
                if (farm.Id == 0) { farm.HoeImage.SetActive(true); pageopened = true; }
                else if (farm.Id == 1) { OpenShopUIvoid(); }
                else if (farm.curDay >= farm.reqDay)
                {
                    HarvestCrop(farm);
                }
            }
        }
        #endregion
    }
    public void InitializeMoneyText()
    {
        switch (money)
        {
            case < 1000: MoneyText.text = "Money: " + money; break;
            case < 1000000: MoneyText.text = "Money: " + (money / 1000).ToString("F2") + "K"; break;
            case < 1000000000: MoneyText.text = "Money: " + (money / 1000000).ToString("F2") + "M"; break;
        }
    }
    public void HarvestCrop(FarmInfo farm)
    {
        switch (farm.Id)
        {
            case 2: money += (float)(50 + ((365 - Day) * 10.0f) + farmsScr.totalConnectedIds.Sum() * (5f + 0.5f * HarvestedCropCount.Sum())); HarvestedCropCount[2]++; break;
            case 3: money += (float)(100 + ((365 - Day) * 10.0f * (3f - 0.75f * HarvestedCropCount.Sum())) + 100 * farmsScr.totalConnectedIds.Sum()); HarvestedCropCount[3]++; break;
            case 4: money += (float)(150 + money * 0.01f); HarvestedCropCount[4]++; break;
            case 5: money += (float)(100 + money * (0.01f * HarvestedCropCount.Sum()) * (0.05f - 0.01f * farmsScr.totalConnectedIds.Sum())); HarvestedCropCount[5]++; break;
            case 6: money += (float)(150 + (Day * 10) + 40 * farmsScr.totalConnectedIds.Sum() * (1 + 0.02f * HarvestedCropCount.Sum())); HarvestedCropCount[6]++; break;
            case 7: money += (float)(200 + (Day * 30) + 10 * farmsScr.totalConnectedIds.Sum() * (1 + 0.1f * HarvestedCropCount.Sum())); HarvestedCropCount[7]++; break;
        }
        farm.Id = 1;
        farm.curDay = 0;
        farm.reqDay = 0;
        InitializeMoneyText();
    }
    public void BuyCrop(int id)
    {
        if (money >= crops[id].Cost)
        {
            farmsScr.ChosenFarm.Id = id;
            farmsScr.ChosenFarm.curDay = 0;
            farmsScr.ChosenFarm.reqDay = crops[id].reqDayToGrow;
            money -= crops[id].Cost;
        }

        CloseShopUIvoid();
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
    public void OpenShopUIvoid()
    {
        StartCoroutine(OpenShopUI());
    }
    public IEnumerator OpenShopUI()
    {
        ShopImage.SetActive(true);
        ShopImage.transform.DOScale(new Vector3(1, 1, 1) * 1, 0.5f);
        yield return new WaitForSecondsRealtime(0.5f);
        pageopened = true;
    }
    public void CloseShopUIvoid()
    {
        StartCoroutine(CloseShopUI());
    }
    public IEnumerator CloseShopUI()
    {
        ShopImage.transform.DOScale(Vector3.zero, 0.5f);
        yield return new WaitForSecondsRealtime(0.5f);
        ShopImage.SetActive(true);
        pageopened = false;
    }
    public void NextDay()
    {
        Day++;
        Daytext.text = "Day: " + Day + "/" + reqDay;
        StartCoroutine(deckPlacing.DayPassedTakeCard());
        for (int i = 0; i < maxCardCountOnShop; i++)
        {
            ShopManagement.AddCardToShop(i,Random.Range(0,deckPlacing.allCardScr.Length));
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
        WinPanel.transform.DOScale(new Vector3(1, 1, 1) * 1, 0.5f);
    }
    public void Lose()
    {
        LosePanel.SetActive(true);
        pageopened = true;
        LosePanel.transform.DOScale(new Vector3(1, 1, 1) * 1, 0.5f);
    }
}
