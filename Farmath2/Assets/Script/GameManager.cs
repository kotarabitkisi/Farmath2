using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public enum ActiveCardState
    {
        NONE,
        IN_HAND,
        USING
    };
    public ActiveCardState activeCardState;
    public GameObject cropCardUsing;
    public GameObject itemCardUsing;
    public GameObject[] farmers;
    public Color colorNone;
    public EffectColorChanging ColorScr;

    public QuestionScr[] Questions;
    [Header("MainStats")]
    public CropStats[] crops;
    public float money;
    public int Day;
    public int Month;
    const float pageAnimTime = 0.5f;
    const int reqDay = 30;
    const int reqMonth = 30;
    const int maxCardCountOnShop = 6;
    const int maxExploreCount = 6;
    const int moneyBound = 100;
    public bool pageopened;
    public bool heropageopened;
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
    public GameObject debuffPage;
    public GameObject[] debuffChilds;
    [Header("Question")]
    public GameObject questionPage;
    public GameObject[] questionChilds;
    public TMP_InputField questionTextArea;
    public TextMeshProUGUI questionTimeTxt;
    public float questionTime;
    public GameObject moneyObjPool;



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
    [Header("Debuffs")]
    public string[] DebuffTitle;
    public Sprite[] DebuffIcon;
    public string[] DebuffDesc;

    public bool[] debuffs;
    //0:inflation


    private void Start()
    {
        InitializeMoneyText();
        NextDay();
    }
    private void Update()
    {
        if (questionTime > 0 && questionPage.activeSelf)
        {

            questionTime -= Time.deltaTime; 
            questionTimeTxt.text = "Süre: " + questionTime.ToString("F1");

            if (questionTime <= 0) { questionTextArea.text = ""; questionTime = 0; questionTimeTxt.text = "Süre: 0"; SolutionIsTrueOrNot(Questions[0], 0); }
        }


        #region TýklamaKontrolleri
        if (Input.GetMouseButtonDown(0))
        {
            if (!pageopened)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
                if (hit.collider != null && hit.collider.CompareTag("Farm"))
                {
                    farmsScr.ChosenFarm = hit.collider.gameObject.GetComponent<FarmInfo>();
                    FarmInfo farm = hit.collider.gameObject.GetComponent<FarmInfo>();
                    if (itemCardUsing != null || cropCardUsing != null)
                    {
                        StartCoroutine(UseThisCard(farm));
                    }
                    else if (farm.Id == 0) { farm.HoeImage.SetActive(true); farm.HoeCost.text = (10 * Mathf.Pow(1.45f, HoeCount)).ToString("F2"); pageopened = true; }
                    else if (farm.curDay >= farm.reqDay && farm.Id >= 2 && farm.Id <= 7)
                    {
                        HarvestCrop(farm);
                    }
                }
                else if (hit.collider != null && hit.collider.CompareTag("Card"))
                {
                    if (hit.collider.gameObject == itemCardUsing || hit.collider.gameObject == cropCardUsing)
                    {
                        if (itemCardUsing != null)
                        {
                            deckPlacing.MakeThisCardUsingCard(false, itemCardUsing);
                        }
                        else if (cropCardUsing != null)
                        {
                            deckPlacing.MakeThisCardUsingCard(false, cropCardUsing);
                        }
                    }
                    else
                    {
                        if (itemCardUsing != null && activeCardState == ActiveCardState.IN_HAND)
                        {
                            deckPlacing.MakeThisCardUsingCard(false, itemCardUsing);
                            deckPlacing.MakeThisCardUsingCard(true, hit.collider.gameObject);
                        }
                        else if (cropCardUsing != null && activeCardState == ActiveCardState.IN_HAND)
                        {
                            deckPlacing.MakeThisCardUsingCard(false, cropCardUsing);
                            deckPlacing.MakeThisCardUsingCard(true, hit.collider.gameObject);
                        }
                    }
                }
            }
            else if (heropageopened)
            {

            }
        }
        else if (Input.GetMouseButtonDown(1) && activeCardState == ActiveCardState.IN_HAND)
        {
            if (itemCardUsing != null)
            {
                deckPlacing.MakeThisCardUsingCard(false, itemCardUsing);
            }
            else if (cropCardUsing != null)
            {
                deckPlacing.MakeThisCardUsingCard(false, cropCardUsing);
            }
            activeCardState = ActiveCardState.NONE;
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
                    MultipleRev *= cropStats.Rev[0];
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
                    MultipleRev *= cropStats.Rev[1];
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
                    MultipleRev *= cropStats.Rev[2];
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
                    MultipleRev *= cropStats.Rev[3];
                    break;
            }
        }
        if (cropStats.IsHarvestCount(true, HarvestedCropCount[farm.Id]))
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
                    MultipleRev *= cropStats.Rev[4];
                    break;
            }
        }
        int totalHarvest = 0;
        for (int i = 0; i < HarvestedCropCount.Length; i++)
        {
            totalHarvest += HarvestedCropCount[i];
        }
        if (cropStats.IsHarvestCount(false, totalHarvest))
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
                    MultipleRev *= cropStats.Rev[5];
                    break;
            }
        }
        if (farm.Watered) { MultipleRev *= 2; farm.Watered = false; }
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
        StartCoroutine(MoneyAnimPlay(moneyObjPool.transform.GetChild(0).gameObject, baseRev * MultipleRev, farm.gameObject, 1));
        InitializeMoneyText();
    }
    public void SeedCrop(int id, FarmInfo ChosenFarm)
    {
        ChosenFarm.Id = id;
        ChosenFarm.curDay = 0;
        ChosenFarm.reqDay = crops[id].reqDayToGrow;
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
        if (!pageopened && !Obj.activeSelf)
        {
            Obj.SetActive(true);
            PageParent.GetComponent<RectTransform>().DOMoveX(Screen.width / 2, pageAnimTime);
            yield return new WaitForSecondsRealtime(pageAnimTime);
            this.pageopened = true;
        }
        else if (Obj.activeSelf)
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
        if (itemCardUsing || cropCardUsing)
        { print("you cant pass day you choosed an item"); }
        else if (pageopened) { print("turn off the pages before day pass"); }
        else
        {
            Day++;
            Daytext.text = "Day: " + Day + "/" + reqDay;

            int isthecrop = 0;
            for (int i = 0; i < farmsScr.frams.Length; i++)
            {
                if (farmsScr.frams[i].Id >= 2 && farmsScr.frams[i].Id <= 7) { isthecrop++; }
            }
            if (money <= 0 && isthecrop == 0 && deckPlacing.openedCards.Count == 0 && money < moneyBound)
            {
                Lose();
            }


            for (int i = 0; i < farmsScr.frams.Length; i++)
            {
                FarmInfo farm = farmsScr.frams[i];
                float probality = 5;
                if (debuffs[1])
                {
                    probality += 5;
                }
                for (int l = 0; l < farm.connectedFarmIds.Length; l++)
                {
                    if (farm.connectedFarmIds[l] == 8) { probality += 2.5f; }
                }
                if (farm.Id == 1 && Random.Range(0f, 100) <= probality) { farm.Id = 8; }
            }


            StartCoroutine(deckPlacing.DayPassedTakeCard());
            ShopManagement.cardsOnShop.Clear();
            ShopManagement.cardCosts.Clear();
            for (int i = 0; i < maxCardCountOnShop; i++)
            {
                ShopManagement.AddCardToShop(i, Random.Range(0, deckPlacing.allCardScr.Length));
            }
            if (reqDay < Day)
            {
                Month++;
                Day = 1;
                Daytext.text = "Day: " + Day + "/" + reqDay;
                StartDebuff(Month - 1);
                if (reqMonth < Month)
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
            else
            {
                foreach (FarmInfo farmland in farmsScr.FarmList)
                {
                    farmland.curDay++;
                }
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
    public IEnumerator UseThisCard(FarmInfo farm)
    {
        float cardAnimTime = 0.5f;
        if (farm.Id == 1 && cropCardUsing)
        {
            GameObject chosenCardUsing = cropCardUsing;
            activeCardState = ActiveCardState.USING;
            CardScr card = cropCardUsing.GetComponent<CardData>().Card;
            chosenCardUsing.transform.DOMove(farm.transform.position, cardAnimTime);
            chosenCardUsing.transform.DOScale(Vector3.zero, cardAnimTime);
            yield return new WaitForSecondsRealtime(cardAnimTime);
            yield return new WaitForSecondsRealtime(0.2f);
            if (card is CropScr CropCard)
            {
                SeedCrop(CropCard.id, farm);
                deckPlacing.DestroyThisCard(chosenCardUsing);
                activeCardState = ActiveCardState.NONE;
            }
        }
        else if (itemCardUsing)
        {
            CardScr card = itemCardUsing.GetComponent<CardData>().Card;
            if (card is ItemScr ItemCard)
            {
                GameObject chosenCardUsing;
                switch (ItemCard.itemId)
                {
                    case 0:
                        activeCardState = ActiveCardState.USING;
                        chosenCardUsing = itemCardUsing;
                        farm.reqDay /= 2;
                        chosenCardUsing.transform.DOMove(farm.transform.position, cardAnimTime);
                        chosenCardUsing.transform.DOScale(Vector3.zero, cardAnimTime);
                        yield return new WaitForSecondsRealtime(cardAnimTime);
                        deckPlacing.DestroyThisCard(chosenCardUsing);
                        activeCardState = ActiveCardState.NONE;
                        break;
                    case 2:
                        if (farm.Id < 2)
                        {
                            activeCardState = ActiveCardState.USING;
                            chosenCardUsing = itemCardUsing;
                            chosenCardUsing.transform.DOMove(farm.transform.position, cardAnimTime);
                            chosenCardUsing.transform.DOScale(Vector3.zero, cardAnimTime);
                            yield return new WaitForSecondsRealtime(cardAnimTime);
                            farm.HolyHoed = true;
                            farm.Id = 1;
                            deckPlacing.DestroyThisCard(chosenCardUsing);
                            activeCardState = ActiveCardState.NONE;
                        }
                        break;
                    case 3:
                        activeCardState = ActiveCardState.USING;
                        chosenCardUsing = itemCardUsing;
                        chosenCardUsing.transform.DOMove(farm.transform.position, cardAnimTime);
                        chosenCardUsing.transform.DOScale(Vector3.zero, cardAnimTime);
                        yield return new WaitForSecondsRealtime(cardAnimTime);
                        farm.Id = 1;
                        farm.curDay = 0;
                        farm.reqDay = 0;
                        deckPlacing.DestroyThisCard(chosenCardUsing);
                        activeCardState = ActiveCardState.NONE;
                        break;
                }
            }

        }
    }
    public void StartDebuff(int DebuffType)
    {
        UIPageAnimVoid(debuffPage);
        debuffChilds[0].GetComponent<TextMeshProUGUI>().text = DebuffTitle[DebuffType];
        debuffChilds[1].GetComponent<Image>().sprite = DebuffIcon[DebuffType];
        debuffChilds[2].GetComponent<TextMeshProUGUI>().text = DebuffDesc[DebuffType];
        for (int i = 0; i < debuffs.Length; i++)
        {
            debuffs[i] = false;
        }
        debuffs[DebuffType] = true;
    }

    public void QuestionStart(ItemScr cardData)
    {
        QuestionScr ChosenQuestion = Questions[Random.Range(0, Questions.Length)];
        StartCoroutine(ColorScr.ChangeColor(Color.black,1));
        UIPageAnimVoid(questionPage);
        questionTime = ChosenQuestion.timeOfQuestion;
        questionChilds[0].GetComponent<TextMeshProUGUI>().text = ChosenQuestion.questionTitle;
        questionChilds[1].GetComponent<Image>().sprite = ChosenQuestion.QuestionIcon;
        Button btn = questionChilds[2].gameObject.GetComponent<Button>();

        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() =>
        {
            float reward = cardData.CardCost;
            if (debuffs[0]) { reward *= 1 + Day * 0.05f; }
            SolutionIsTrueOrNot(ChosenQuestion, reward);
        }
        );
    }

    public void SolutionIsTrueOrNot(QuestionScr question, float reward)
    {
        StartCoroutine(ColorScr.ChangeColor(Color.white, 1));
        print(question.Solution + " " + questionTextArea.text);
        if (question.Solution == questionTextArea.text)
        {
            money += reward*question.moneyMultiple;
            InitializeMoneyText();
        }
        UIPageAnimVoid(questionPage);
        questionTextArea.text = "";
        activeCardState = ActiveCardState.NONE;
    }

    public IEnumerator MoneyAnimPlay(GameObject MoneyObj, float money, GameObject farmObj, float animTime)
    {
        MoneyObj.SetActive(true);
        MoneyObj.transform.position = farmObj.transform.position;
        MoneyObj.transform.parent = null;
        MoneyObj.transform.DOMoveY(farmObj.transform.position.y + 0.5f, animTime).SetEase(Ease.Linear);
        TextMeshProUGUI txt = MoneyObj.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
        Image ýmage = MoneyObj.transform.GetChild(1).GetComponent<Image>();
        txt.text = money.ToString("F2");
        for (int i = 0; i < 101; i++)
        {
            txt.color = Color.Lerp(Color.yellow, colorNone, (float)i / 100);
            ýmage.color = Color.Lerp(Color.white, colorNone, (float)i / 100);
            yield return new WaitForSecondsRealtime(animTime / 202);
        }
        MoneyObj.transform.parent = moneyObjPool.transform;
        MoneyObj.transform.localPosition = Vector3.zero;
        MoneyObj.SetActive(false);
    }



}
