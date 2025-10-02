using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class GameManager : MonoBehaviour
{
    public enum ActiveCardState
    {
        NONE,
        IN_HAND,
        USING
    };
    public Color colorNone;
    public static GameManager instance;
    [Header("Audio")]
    public AudioClip[] audioClips;
    public AudioSource Source;
    public AudioListener Listener;

    [Header("AnotherScripts")]
    public Logger logger;
    public Farms farmsScr;
    public EffectColorChanging ColorScr;
    public BossManager bossManager;
    public ShopManager ShopManagement;
    public DeckPlacing deckPlacing;
    public saveAndLoad saveAndLoad;

    [Header("MainStats")]
    public bool tutorialPlayed;
    public Sprite[] EventSprites;
    public int[] farmerCount;
    public CropStats[] crops;
    public float money;
    public int Day;
    public int Month;
    const float pageAnimTime = 0.5f;
    const int reqDay = 20;
    const int reqMonth = 3;
    public const int maxCardCountOnShop = 6;
    const int maxExploreCount = 6;
    const int reqMoneyToWin = 1000000;
    public bool pageopened;
    public bool pageopening;
    public int[] HarvestedCropCount;
    public int HoeCount;
    public int[] HoeCost;
    public ActiveCardState activeCardState;
    public int Boss;//0=boss yok 1=soytarý

    [Header("CanvasObjects")]
    public TextMeshProUGUI WarningText;
    public GameObject[] farmers;
    public TextMeshProUGUI moneyText, dayText, totalHarvestText;
    public GameObject WinPanel;
    public GameObject LosePanel;
    public GameObject ShopImage;
    public GameObject PageParent;
    public Image soundButtonImage;
    public Image EventImage;
    public Sprite[] soundSprite;
    public Image[] heroPlaces;

    [Header("Question")]
    public QuestionScr[] Questions;
    public GameObject questionPage;
    public GameObject[] questionChilds;
    public TMP_InputField questionTextArea;
    public TextMeshProUGUI questionTimeTxt;
    public float questionTime;
    public GameObject moneyObjPool;

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
    public GameObject debuffPage;
    public GameObject[] debuffChilds;
    public bool[] debuffs;
    //0:inflation 1:invasion 2:festival
    private void Awake()
    {
        instance = this;
    }
    public void InitializeLoad()
    {
        if (Month != 0)
        {
            EventImage.sprite = EventSprites[Month];
            EventImage.color = Color.white;
            EventImage.raycastTarget = true;
        }
        else { EventImage.color = colorNone; EventImage.raycastTarget = false; }
        DOTween.Init();
        int totalHarvest = 0;
        for (int i = 0; i < HarvestedCropCount.Length; i++)
        {
            totalHarvest += HarvestedCropCount[i];
        }
        totalHarvestText.text = totalHarvest.ToString();
        dayText.text = Day + "/" + Month;
        if (Boss == 1)
        {
            bossManager.BossStart();
        }
        foreach (FarmInfo farmland in farmsScr.FarmList)
        {
            farmland.InitializeSpriteAndEffect();
        }
        int heroCount = Mathf.Clamp(Month+1, 1, 3);
        for (int i = 0; i < heroCount; i++)
        {
            heroPlaces[i].raycastTarget = true;
            heroPlaces[i].color = Color.white;
        }
        if (!tutorialPlayed)
        {
            logger.StartDialouge(0);
        }
        ShopManagement.DayPassedAddCard();
        InitializeMoneyText();
    }
    private void Update()
    {
        if (questionTime > 0 && questionPage.activeSelf)
        {
            questionTime -= Time.deltaTime;
            questionTimeTxt.text = "Süre: " + questionTime.ToString("0");

            if (questionTime <= 0) { questionTextArea.text = ""; questionTime = 0; questionTimeTxt.text = "Süre: 0"; SolutionIsTrueOrNot(Questions[0], 0); }
        }
        #region TýklamaKontrolleri
#if UNITY_ANDROID || UNITY_IOS
        if (Input.touchCount > 0)
        {
            Touch touch = new Touch();
            touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began && activeCardState != ActiveCardState.USING && !pageopened)
            {
                if (!pageopened)
                {
                    Ray ray = Camera.main.ScreenPointToRay(touch.position);
                    RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
                    if (hit.collider != null && hit.collider.CompareTag("Farm"))
                    {
                        farmsScr.ChosenFarm = hit.collider.gameObject.GetComponent<FarmInfo>();
                        FarmInfo farm = hit.collider.gameObject.GetComponent<FarmInfo>();
                        if (deckPlacing.itemCardUsing != null || deckPlacing.cropCardUsing != null)
                        {
                            StartCoroutine(deckPlacing.UseThisCard(farm));
                        }
                        else if (farm.Id == 0) { farm.HoeImage.SetActive(true); farm.HoeCost.text = HoeCost[HoeCount].ToString("F2"); pageopened = true; }
                    }
                }
            }
            else if ((touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary) && activeCardState != ActiveCardState.USING && !pageopened)
            {
                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
                if (hit.collider != null && hit.collider.CompareTag("Farm"))
                {
                    farmsScr.ChosenFarm = hit.collider.gameObject.GetComponent<FarmInfo>();
                    FarmInfo farm = hit.collider.gameObject.GetComponent<FarmInfo>();
                    if (farm.curDay >= farm.reqDay && farm.Id >= 2 && farm.Id <= 7)
                    {
                        HarvestCrop(farm);
                    }
                }
            }
        }

#elif UNITY_STANDALONE_WIN
        if (Input.GetMouseButtonDown(0) && activeCardState != ActiveCardState.USING && !pageopened)
        {
            if (!pageopened)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
                if (hit.collider != null && hit.collider.CompareTag("Farm"))
                {
                    farmsScr.ChosenFarm = hit.collider.gameObject.GetComponent<FarmInfo>();
                    FarmInfo farm = hit.collider.gameObject.GetComponent<FarmInfo>();
                    if (deckPlacing.itemCardUsing != null || deckPlacing.cropCardUsing != null)
                    {
                        StartCoroutine(deckPlacing.UseThisCard(farm));
                    }
                    else if (farm.Id == 0) { farm.HoeImage.SetActive(true); farm.HoeCost.text = HoeCost[HoeCount].ToString("F2"); pageopened = true; }
                }

            }
        }
        else if (Input.GetMouseButton(0) && activeCardState != ActiveCardState.USING && !pageopened)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
            if (hit.collider != null && hit.collider.CompareTag("Farm"))
            {
                farmsScr.ChosenFarm = hit.collider.gameObject.GetComponent<FarmInfo>();
                FarmInfo farm = hit.collider.gameObject.GetComponent<FarmInfo>();
                if (farm.curDay >= farm.reqDay && farm.Id >= 2 && farm.Id <= 7)
                {
                    HarvestCrop(farm);
                }
            }
        }
#endif
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
            case < 1000: moneyText.text = money.ToString("F2"); break;
            case < 1000000: moneyText.text = (money / 1000).ToString("F2") + "K"; break;
            case < 1000000000: moneyText.text = (money / 1000000).ToString("F2") + "M"; break;
        }
    }
    public void HarvestCrop(FarmInfo farm)
    {
        farmsScr.CalculateConnectedCount();
        farmsScr.CalculateTotalConnectedIds();
        CropStats cropStats = crops[farm.Id];
        float baseRev = cropStats.base_;
        float MultipleRev = 1;
        HarvestedCropCount[farm.Id]++;


        if (cropStats.IsNeighbour(farm.connectedFarmIds))
        {
            int id = (farm.Id - 2) * maxExploreCount;
            if (!isExplored[id])
            {
                OpenExplore(id, true);
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
                OpenExplore(id, true);
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
                OpenExplore(id, true);
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
        if (cropStats.IsDayBetweenTheseNum(Day + (reqDay * Month)))
        {
            int id = (farm.Id - 2) * maxExploreCount + 3;
            if (!isExplored[id])
            {
                OpenExplore(id, true);
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
                OpenExplore(id, true);
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
        totalHarvestText.text = totalHarvest.ToString();
        if (cropStats.IsHarvestCount(false, totalHarvest))
        {
            int id = (farm.Id - 2) * maxExploreCount + 5;
            if (!isExplored[id])
            {
                OpenExplore(id, true);
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
        if (farm.Negatived) { MultipleRev *= -1; }
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
        if (Boss == 1) { farm.Id = 8; }
        farm.curDay = 0;
        farm.reqDay = 0;
        farm.InitializeSpriteAndEffect();
        float Revenue = baseRev * MultipleRev;
        money += Revenue;
        if (Revenue < 0 && logger.textingFinished)
        {
            logger.StartDialouge(7);
        }
        StartCoroutine(MoneyAnimPlay(moneyObjPool.transform.GetChild(0).gameObject, Revenue, farm.gameObject, 1));
        InitializeMoneyText();
    }
    public void SeedCrop(int id, FarmInfo ChosenFarm)
    {
        ChosenFarm.ChangeScale(1.1f, 1);
        ChosenFarm.Id = id;
        ChosenFarm.curDay = 0;
        ChosenFarm.reqDay = crops[id].reqDayToGrow;
        ChosenFarm.InitializeSpriteAndEffect();
    }
    public void Hoe(FarmInfo farm)
    {
        if (money >= HoeCost[HoeCount])
        {
            if (!tutorialPlayed && !logger.tutorials[3] && logger.CalculatePlayedDialogTutorial() >= 3 && logger.textingFinished)
            {
                logger.StartDialouge(3);
            }
            money -= HoeCost[HoeCount];
            farm.Id = 1;
            HoeCount++;
            PlaySound(3);
            farm.ChangeScale(1.1f, 1);
            farm.InitializeSpriteAndEffect();
        }
        CloseHoeMenuvoid(farm);
        InitializeMoneyText();
    }
    public void CloseHoeMenuvoid(FarmInfo farm)
    {
        farm.HoeImage.SetActive(false);
        pageopened = false;
    }
    public void UIPageAnim(GameObject Obj)
    {
        if (pageopening || (pageopened && !Obj.activeSelf)) return;
        pageopening = true;
        if (!pageopened && !Obj.activeSelf)
        {
            PlaySound(10);
            pageopened = true;
            Obj.SetActive(true);
            PageParent.GetComponent<RectTransform>().DOScale(Vector3.one, pageAnimTime).OnComplete(() => pageopening = false);
        }
        else if (Obj.activeSelf)
        {
            PlaySound(11);
            TurnOffColor();
            pageopened = false;
            Sequence seq = DOTween.Sequence();
            seq.Append(PageParent.GetComponent<RectTransform>().DOScale(Vector3.zero, pageAnimTime)).OnComplete(() => { Obj.SetActive(false); pageopening = false; });
        }

    }
    public void NextDay()
    {

        if (deckPlacing.itemCardUsing || deckPlacing.cropCardUsing)
        {
            StartCoroutine(ToggleWarning(4, "Kart kullanýrken gün geçemezsin.")); StartCoroutine(ShakeTheObj(bossManager.Camera, 0.2f, 0.05f, 0, false));
        }
        else if (pageopened) { StartCoroutine(ToggleWarning(4, "Gün Geçmeden önce sayfalarý kapat.")); StartCoroutine(ShakeTheObj(bossManager.Camera, 0.2f, 0.05f, 0, false)); }
        else
        {
            #region kaybetmekontrolü
            int isthecrop = 0;
            for (int i = 0; i < farmsScr.frams.Length; i++)
            {
                if (farmsScr.frams[i].Id >= 2 && farmsScr.frams[i].Id <= 7) { isthecrop++; }
            }
            if (money <= 0 && isthecrop == 0 && deckPlacing.openedCards.Count == 0)
            {
                Lose();
                return;
            }
            #endregion
            Day++;
            dayText.text = Day + "/" + Month;
            #region yeniayýnbaþlangýcýnýkontroletme
            if (reqDay < Day)
            {
                Month++;
                int heroCount = Mathf.Clamp(Month, 0, 3);
                for (int i = 0; i < heroCount; i++)
                {
                    heroPlaces[i].raycastTarget = true;
                    heroPlaces[i].color = Color.white;
                }
                EventImage.color = Color.white;
                EventImage.sprite = EventSprites[Month];
                EventImage.raycastTarget = true;
                Day = 1;
                dayText.text = Day + "/" + Month;
                if (reqMonth < Month)
                {
                    if (Boss != 0)
                    {
                        if (money >= reqMoneyToWin)
                        {
                            Win();
                            return;
                        }
                        else
                        {
                            Lose();
                            return;
                        }
                    }
                    bossManager.BossStart();
                    for (int i = 0; i < debuffs.Length; i++)
                    {
                        debuffs[i] = false;
                    }
                    Boss = 1;
                }
                else
                {
                    StartDebuff(Month - 1);
                    foreach (FarmInfo farmland in farmsScr.FarmList)
                    {
                        farmland.curDay++;
                        farmland.InitializeSpriteAndEffect();
                    }
                }
            }
            else
            {
                foreach (FarmInfo farmland in farmsScr.FarmList)
                {
                    farmland.curDay++;
                    farmland.InitializeSpriteAndEffect();
                }
            }
            #endregion


            if (farmers[2].GetComponent<FarmerInfo>().choosed)
            {
                farmerCount[2]--;
                if (farmerCount[2] <= 0)
                {
                    farmerCount[2] = 5;
                    for (int i = 0; i < 1000; i++)
                    {
                        int randomIndex = Random.Range(0, isExplored.Length);
                        if (!isExplored[randomIndex])
                        {
                            OpenExplore(randomIndex, true);
                            break;
                        }
                    }
                }
            }
            if (farmers[3].GetComponent<FarmerInfo>().choosed)
            {
                farmerCount[3]--;
                if (farmerCount[3] <= 0)
                {
                    farmerCount[3] = 3;
                    for (int i = 0; i < 1000; i++)
                    {
                        int randomIndex = Random.Range(0, farmsScr.FarmList.Length);
                        if (farmsScr.frams[randomIndex].Id == 8)
                        {
                            farmsScr.frams[randomIndex].Id = 1;
                            break;
                        }
                    }
                }
            }

            if (Boss == 1)
            {
                bossManager.AddBossCard();
                bossManager.BossPutNegative();
                bossManager.BossEffectCount = 1;
            }

            for (int i = 0; i < farmsScr.frams.Length; i++)
            {
                FarmInfo farm = farmsScr.frams[i];
                float probality = 15;
                if (debuffs[1])
                {
                    probality += 100;
                }
                for (int l = 0; l < farm.connectedFarmIds.Length; l++)
                {
                    if (farm.connectedFarmIds[l] == 8) { probality += 2.5f; }
                }
                if (farm.Id == 1 && Random.Range(0f, 100) <= probality) { farm.Id = 8; farm.InitializeSpriteAndEffect(); }
            }
            StartCoroutine(deckPlacing.DayPassedTakeCard());
            ShopManagement.DayPassedAddCard();
        }


    }
    public void Win()
    {
        saveAndLoad.SaveExploration();
        PlayerPrefs.SetInt("Level", 1);
        WinPanel.SetActive(true);
        pageopened = true;
        WinPanel.transform.DOScale(new Vector3(1, 1, 1) * 1, pageAnimTime);
        saveAndLoad.EraseSaves();
    }
    public void Lose()
    {
        saveAndLoad.SaveExploration();
        LosePanel.SetActive(true);
        pageopened = true;
        LosePanel.transform.DOScale(new Vector3(1, 1, 1) * 1, pageAnimTime);
        saveAndLoad.EraseSaves();
    }
    public void TurnOffColor()
    {
        for (int i = 0; i < ExploreText.Length; i++)
        {
            ExploreNotification.SetActive(false);
            ExploreNotificationTxt.text = "0";
            isTextColorful[i] = false;
            ExploreTexts[i].color = Color.black;
        }

    }

    public void StartDebuff(int DebuffType)
    {
        UIPageAnim(debuffPage);
        PlaySound(3);
        debuffChilds[0].GetComponent<TextMeshProUGUI>().text = DebuffTitle[DebuffType];
        debuffChilds[1].GetComponent<Image>().sprite = DebuffIcon[DebuffType];
        debuffChilds[2].GetComponent<TextMeshProUGUI>().text = DebuffDesc[DebuffType];
        for (int i = 0; i < debuffs.Length; i++)
        {
            debuffs[i] = false;
        }
        debuffs[DebuffType] = true;
        logger.StartDialouge(DebuffType + 8);
    }

    public void QuestionStart(ItemScr cardData)
    {

        QuestionScr ChosenQuestion = Questions[Random.Range(0, Questions.Length)];
        StartCoroutine(ColorScr.ChangeColor(Color.black, 1));
        UIPageAnim(questionPage);
        questionTime = ChosenQuestion.timeOfQuestion;
        questionChilds[0].GetComponent<TextMeshProUGUI>().text = ChosenQuestion.questionTitle;
        questionChilds[1].GetComponent<Image>().sprite = ChosenQuestion.QuestionIcon;
        Button btn = questionChilds[2].gameObject.GetComponent<Button>();

        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() =>
        {
            float reward = ShopManagement.Qreward[ShopManagement.Qreward.Count - 1];
            ShopManagement.Qreward.RemoveAt(ShopManagement.Qreward.Count - 1);
            SolutionIsTrueOrNot(ChosenQuestion, reward);
        }
        );
    }

    public void SolutionIsTrueOrNot(QuestionScr question, float reward)
    {

        switch (Boss)
        {
            case 0:
                StartCoroutine(ColorScr.ChangeColor(Color.white, 1));
                break;
            case 1:
                StartCoroutine(ColorScr.ChangeColor(bossManager.BossSoytariColor, 1));
                break;
        }

        print(question.Solution + " " + questionTextArea.text);
        if (question.Solution == questionTextArea.text)
        {
            float rewardmoney = reward * question.moneyMultiple;
            //logger.Addlog("Cevabýn doðru \n kazandýðýn para: " + rewardmoney);
            PlaySound(2);
            money += rewardmoney;
            InitializeMoneyText();
        }
        else
        {
            //logger.Addlog("Cevabýn yanlýþ \n doðru cevap: " + question.Solution);
        }
        UIPageAnim(questionPage);
        questionTextArea.text = "";
        activeCardState = ActiveCardState.NONE;
    }

    public IEnumerator MoneyAnimPlay(GameObject MoneyObj, float money, GameObject farmObj, float animTime)
    {
        PlaySound(2);
        MoneyObj.SetActive(true);
        MoneyObj.transform.position = farmObj.transform.position;
        MoneyObj.transform.parent = null;
        MoneyObj.transform.DOMoveY(farmObj.transform.position.y + 0.5f, animTime).SetEase(Ease.Linear);
        TextMeshProUGUI txt = MoneyObj.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
        Image image = MoneyObj.transform.GetChild(1).GetComponent<Image>();
        txt.text = money.ToString("F2");
        for (int i = 0; i < 101; i++)
        {
            txt.color = Color.Lerp(Color.yellow, colorNone, (float)i / 100);
            image.color = Color.Lerp(Color.white, colorNone, (float)i / 100);
            yield return new WaitForSecondsRealtime(animTime / 202);
        }
        MoneyObj.transform.parent = moneyObjPool.transform;
        MoneyObj.transform.localPosition = Vector3.zero;
        MoneyObj.SetActive(false);
    }

    private Dictionary<GameObject, bool> isShakingDict = new Dictionary<GameObject, bool>();

    public IEnumerator ShakeTheObj(GameObject Obj, float duration, float magnitudeX, float magnitudeY, bool IsInCanvas)
    {
        if (isShakingDict.ContainsKey(Obj) && isShakingDict[Obj]) yield break;
        isShakingDict[Obj] = true;

        if (IsInCanvas)
        {
            RectTransform transformm = Obj.GetComponent<RectTransform>();
            Vector3 originalPosition = transformm.anchoredPosition;
            float elapsed = 0.0f;

            while (elapsed < duration)
            {
                float offsetX = Random.Range(-1f, 1f) * magnitudeX;
                float offsetY = Random.Range(-1f, 1f) * magnitudeY;

                transformm.anchoredPosition = new Vector3(originalPosition.x + offsetX, originalPosition.y + offsetY, originalPosition.z);

                elapsed += Time.deltaTime;

                yield return null;
            }
            transformm.anchoredPosition = originalPosition;
        }
        else
        {
            Vector3 originalPosition = Obj.transform.position;
            float elapsed = 0.0f;

            while (elapsed < duration)
            {
                float offsetX = Random.Range(-1f, 1f) * magnitudeX;
                float offsetY = Random.Range(-1f, 1f) * magnitudeY;

                Obj.transform.position = new Vector3(originalPosition.x + offsetX, originalPosition.y + offsetY, originalPosition.z);

                elapsed += Time.deltaTime;

                yield return null;
            }
            Obj.transform.position = originalPosition;
        }
        isShakingDict[Obj] = false;
    }

    public IEnumerator ToggleWarning(float cooldown, string txt)
    {
        WarningText.gameObject.SetActive(true);
        WarningText.text = txt;
        yield return new WaitForSecondsRealtime(cooldown);
        WarningText.gameObject.SetActive(false);
    }
    public void OpenExplore(int id, bool iscolorful)
    {
        isExplored[id] = true;
        int a = 0;
        for (int i = 0; i < isExplored.Length; i++)
        {
            if (isExplored[i])
            {
                a++;
            }
        }
        ExploreNotificationTxt.GetComponent<TextMeshProUGUI>().text = a.ToString();
        ExploreTexts[id].text = ExploreText[id];
        isTextColorful[id] = iscolorful;
        ExploreNotification.SetActive(iscolorful);
    }

    public void SoundSet()
    {
        Listener.enabled = !Listener.enabled;
        Source.enabled = !Source.enabled;
        soundButtonImage.sprite = soundSprite[Listener.enabled ? 0 : 1];
    }
    public void PlaySound(int id)
    {
        Source.PlayOneShot(audioClips[id]);
    }
    public void OpenWeb(string url)
    {
        Application.OpenURL(url);
    }

}
