using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static GameManager;

public class DeckPlacing : MonoBehaviour
{

    public static DeckPlacing Instance;
    public GameObject OpenedCardCanvas;
    [Header("Audio")]
    public AudioClip[] audioClips;//0:kart çekme 1:kartý yerine koyma
    public AudioSource Source;
    [Header("AnotherScripts")]
    public GameManager GManager;
    public BossManager BossManager;
    public EffectColorChanging ColorScr;
    [Header("GameObjects")]
    public TextMeshProUGUI cardCountTxt;
    public GameObject discardedCardPrefab;
    public GameObject cardPrefab;
    public GameObject cropCardUsing;
    public GameObject itemCardUsing;
    public List<GameObject> openedCards;
    public GameObject chosenMovingCard;
    [Header("MainStats")]
    public CardScr[] allCardScr;
    public bool takeRandomized;
    public bool cardCollected;
    public float Ypos2;
    const float xPos = 1.25f, yPos = 1;
    const int OpenedCardLimit = 3;
    public float rotationValue;
    public int chooseCardToDestroy;
    public int maxOpenedCardCount;
    [Header("Discard")]
    public List<CardScr> discardedCards;
    public Color DiscardColor1, DiscardColor2;
    public Sprite[] backCardIcon;
    [Header("Mobile")]
    public float WaitForDiscard;
    private void Awake()
    {
        Instance = this;
    }
    void Update()
    {
        if (chooseCardToDestroy > 0)
        {
            for (int i = 0; i < openedCards.Count; i++)
            {
                openedCards[i].transform.GetChild(1).gameObject.SetActive(true);
            }
        }//getchild performans için sorunlu



#if UNITY_ANDROID || UNITY_IOS

        Vector2 TouchPosition_;
        Collider2D collider = null;
        Touch touch;
        if (Input.touchCount > 0)
        {
            touch = Input.GetTouch(0);
            TouchPosition_ = Camera.main.ScreenToWorldPoint(touch.position);
            collider = Physics2D.OverlapPoint(TouchPosition_);
            if (touch.phase == TouchPhase.Began) { WaitForDiscard = 2; }
            if (GManager.activeCardState == ActiveCardState.NONE && !GManager.pageopened)
            {
                if (chosenMovingCard != null)
                {
                    if ((collider != null && collider.gameObject != chosenMovingCard) || (collider == null))
                    {
                        chosenMovingCard.transform.DOScale(cardPrefab.transform.lossyScale, 0.25f);
                        chosenMovingCard.transform.DOLocalMoveY(yPos, 0.25f);
                        //chosenCard.transform.DORotate(new Vector3(0, 0, 0), 0.2f).SetEase(Ease.Linear);
                        chosenMovingCard.GetComponent<BoxCollider2D>().offset = new Vector2(0, 0);
                        chosenMovingCard.GetComponent<BoxCollider2D>().size = new Vector2(1, 1f);
                        chosenMovingCard = null;
                        InitializeAllCardsPositions();
                    }
                    if (touch.phase == TouchPhase.Ended)
                    {
                        #region festival
                        if (chooseCardToDestroy > 0)
                        {
                            for (int i = 0; i < openedCards.Count; i++)
                            {
                                if (chosenMovingCard == openedCards[i])
                                {
                                    GManager.activeCardState = ActiveCardState.NONE;
                                    chosenMovingCard.GetComponent<CardData>().FadeCard(false);
                                    chosenMovingCard = null;
                                    chooseCardToDestroy--;
                                    InitializeAllCardsPositions();
                                }
                            }
                            if (chooseCardToDestroy == 0)
                            {
                                for (int i = 0; i < openedCards.Count; i++)
                                {
                                    openedCards[i].transform.GetChild(1).gameObject.SetActive(false);
                                }
                            }
                        }
                        #endregion
                        else
                        {
                            MakeThisCardUsingCard(true, chosenMovingCard);
                            chosenMovingCard = null;
                        }

                    }
                    else if (touch.phase == TouchPhase.Stationary)
                    {
                        WaitForDiscard -= Time.deltaTime;
                        if (WaitForDiscard <= 0)
                        {
                            WaitForDiscard = 2;
                            StartCoroutine(PutDiscardThisCard(chosenMovingCard));
                            chosenMovingCard = null;
                            GManager.activeCardState = ActiveCardState.NONE;
                        }

                    }
                }
                else
                {
                    if (collider != null && collider.gameObject.CompareTag("Card"))
                    {
                        chosenMovingCard = collider.gameObject;
                        chosenMovingCard.transform.DOLocalMoveY(yPos + Ypos2, 0.25f);
                        chosenMovingCard.transform.DOScale(cardPrefab.transform.lossyScale * 1.5f, 0.25f);
                        chosenMovingCard.GetComponent<CardData>().SortLayers(50);
                        chosenMovingCard.GetComponent<BoxCollider2D>().offset = new Vector2(0, -0.165f);
                        chosenMovingCard.GetComponent<BoxCollider2D>().size = new Vector2(1, 1.35f);
                    }
                }
            }
            else if (GManager.activeCardState == ActiveCardState.IN_HAND && !GManager.pageopened)
            {
                if (touch.phase == TouchPhase.Ended)
                {
                    WaitForDiscard = 1;
                    if (collider != null && collider.CompareTag("Card"))
                    {
                        if (collider.gameObject == itemCardUsing || collider.gameObject == cropCardUsing)
                        {
                            if (itemCardUsing != null && GManager.activeCardState == ActiveCardState.IN_HAND)
                            {
                                MakeThisCardUsingCard(false, itemCardUsing);
                            }
                            else if (cropCardUsing != null && GManager.activeCardState == ActiveCardState.IN_HAND)
                            {
                                MakeThisCardUsingCard(false, cropCardUsing);
                            }
                        }
                        else
                        {
                            if (itemCardUsing != null && GManager.activeCardState == ActiveCardState.IN_HAND)
                            {
                                MakeThisCardUsingCard(false, itemCardUsing);
                                MakeThisCardUsingCard(true, collider.gameObject);
                            }
                            else if (cropCardUsing != null && GManager.activeCardState == ActiveCardState.IN_HAND)
                            {
                                MakeThisCardUsingCard(false, cropCardUsing);
                                MakeThisCardUsingCard(true, collider.gameObject);
                            }

                        }


                    }
                }

            }

        }


#elif UNITY_STANDALONE_WIN
        Vector2 mousePosition;
        Collider2D collider;
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        collider = Physics2D.OverlapPoint(mousePosition);
        if (GManager.activeCardState == ActiveCardState.NONE && !GManager.pageopened)
        {
            if (chosenMovingCard != null)
            {
                if ((collider != null && collider.gameObject != chosenMovingCard) || (collider == null))
                {
                    chosenMovingCard.transform.DOScale(cardPrefab.transform.lossyScale, 0.25f);
                    chosenMovingCard.transform.DOLocalMoveY(yPos, 0.25f);
                    chosenMovingCard.GetComponent<BoxCollider2D>().offset = new Vector2(0, 0);
                    chosenMovingCard.GetComponent<BoxCollider2D>().size = new Vector2(1, 1f);
                    chosenMovingCard = null;
                    InitializeAllCardsPositions();
                }
                if (Input.GetMouseButtonDown(0))
                {
                    if (chooseCardToDestroy > 0)
                    {
                        for (int i = 0; i < openedCards.Count; i++)
                        {
                            if (chosenMovingCard == openedCards[i])
                            {
                                openedCards.RemoveAt(i);
                                GManager.activeCardState = ActiveCardState.IN_HAND;
                                chosenMovingCard.GetComponent<CardData>().FadeCard(true);
                                chooseCardToDestroy--;
                                InitializeAllCardsPositions();
                            }
                        }
                        if (chooseCardToDestroy == 0)
                        {
                            for (int i = 0; i < openedCards.Count; i++)
                            {
                                openedCards[i].transform.GetChild(1).gameObject.SetActive(false);
                            }
                        }
                    }
                    else
                    {
                        MakeThisCardUsingCard(true, chosenMovingCard);
                        chosenMovingCard = null;
                    }

                }
                else if (Input.GetMouseButtonDown(1))
                {
                    StartCoroutine(PutDiscardThisCard(chosenMovingCard));
                    chosenMovingCard = null;
                }

            }
            else
            {
                if (collider != null && collider.gameObject.CompareTag("Card"))
                {
                    chosenMovingCard = collider.gameObject;
                    chosenMovingCard.transform.DOLocalMoveY(yPos + Ypos2, 0.25f);
                    chosenMovingCard.transform.DOScale(cardPrefab.transform.lossyScale * 1.5f, 0.25f);
                    chosenMovingCard.GetComponent<CardData>().SortLayers(20);
                    chosenMovingCard.GetComponent<BoxCollider2D>().offset = new Vector2(0, -0.165f);
                    chosenMovingCard.GetComponent<BoxCollider2D>().size = new Vector2(1, 1.35f);
                }
            }
        }
        else if (GManager.activeCardState == ActiveCardState.IN_HAND && !GManager.pageopened)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (collider != null && collider.CompareTag("Card"))
                {
                    if (collider.gameObject == itemCardUsing || collider.gameObject == cropCardUsing)
                    {
                        if (itemCardUsing != null && GManager.activeCardState == ActiveCardState.IN_HAND)
                        {
                            MakeThisCardUsingCard(false, itemCardUsing);
                        }
                        else if (cropCardUsing != null && GManager.activeCardState == ActiveCardState.IN_HAND)
                        {
                            MakeThisCardUsingCard(false, cropCardUsing);
                        }
                    }
                    else
                    {

                        if (itemCardUsing != null && GManager.activeCardState == ActiveCardState.IN_HAND)
                        {
                            MakeThisCardUsingCard(false, itemCardUsing);
                            MakeThisCardUsingCard(true, collider.gameObject);
                        }
                        else if (cropCardUsing != null && GManager.activeCardState == ActiveCardState.IN_HAND)
                        {
                            MakeThisCardUsingCard(false, cropCardUsing);
                            MakeThisCardUsingCard(true, collider.gameObject);
                        }

                    }


                }
            }
            else if (Input.GetMouseButtonDown(1))
            {
                if (itemCardUsing != null)
                {
                    MakeThisCardUsingCard(false, itemCardUsing);
                }
                else if (cropCardUsing != null)
                {
                    MakeThisCardUsingCard(false, cropCardUsing);
                }
                GManager.activeCardState = ActiveCardState.NONE;
            }

        }
#endif

        discardedCardPrefab.SetActive(discardedCards.Count > 0);
        cardCountTxt.text = "Kart: " + discardedCards.Count.ToString();
    }
    public IEnumerator DeleteAllOfOpenedCards()
    {
        int Count = openedCards.Count;
        if (Count != 0)
        {
            for (int i = 0; i < Count; i++)
            {
                GameObject cardObj = transform.GetChild(0).gameObject;
                CardScr card_ = cardObj.GetComponent<CardData>().Card;
                #region Question Kartý ise Reward elemanýný sil

                if (card_ == GManager.deckPlacing.allCardScr[11])
                {
                    float rewardAmount = GManager.ShopManagement.Qreward[0];
                    GManager.ShopManagement.Qreward.RemoveAt(0);
                    GManager.ShopManagement.Qreward.Add(rewardAmount);

                }
                #endregion

                yield return StartCoroutine(PutDiscardThisCard(cardObj));
            }
        }
    }
    public IEnumerator DayPassedTakeCard()
    {

        yield return StartCoroutine(DeleteAllOfOpenedCards());
        if (GManager.debuffs[2])
        {
            for (int i = 0; i < 6; i++)
            {
                TakeCardFromDiscard();
                yield return new WaitForSecondsRealtime(0.1f);
            }
            chooseCardToDestroy = 2;
        }
        else
        {
            for (int i = 0; i < OpenedCardLimit; i++)
            {
                TakeCardFromDiscard();
                yield return new WaitForSecondsRealtime(0.1f);
            }
        }
        if (GManager.farmers[0].GetComponent<FarmerInfo>().choosed)
        {
            TakeCardFromDiscard();
            yield return new WaitForSecondsRealtime(0.1f);
        }
    }
    public void TakeCardFromDiscard()
    {
        if (GManager.logger.textingFinished && !GManager.logger.tutorials[2] && GManager.logger.CalculatePlayedDialogTutorial() >= 2)
        {
            GManager.logger.StartDialouge(2);
        }
        if (openedCards.Count >= maxOpenedCardCount)
        {
            StartCoroutine(GManager.ToggleWarning(4, "Açýk Kart Sayýsý en fazla " + maxOpenedCardCount + " olabilir."));
            StartCoroutine(GManager.ShakeTheObj(Camera.main.gameObject, 0.2f, 0.05f, 0, false));
        }
        if (discardedCards.Count != 0 && openedCards.Count < maxOpenedCardCount)
        {
            int cardIndex = 0;

            if (takeRandomized)
            {
                cardIndex = Random.Range(0, discardedCards.Count);
            }
            GManager.PlaySound(0);
            CardScr Card = discardedCards[cardIndex];
            discardedCards.RemoveAt(cardIndex);
            RaycastHit2D hit = Physics2D.Raycast(discardedCardPrefab.transform.position, Vector2.zero);
            GameObject newOpenedCard = Instantiate(cardPrefab, hit.point, Quaternion.identity);
            newOpenedCard.transform.SetParent(transform, true);
            newOpenedCard.gameObject.GetComponent<CardData>().Card = Card;
            newOpenedCard.GetComponent<CardData>().InitializeImages();
            if (discardedCards.Count != 0)
            {
                if (discardedCards[0] is CropScr)
                {
                    discardedCardPrefab.GetComponent<Image>().sprite = backCardIcon[0];
                }
                else if (discardedCards[0] is ItemScr)
                {
                    discardedCardPrefab.GetComponent<Image>().sprite = backCardIcon[1];
                }
            }
            openedCards.Add(newOpenedCard);
            InitializeAllCardsPositions();
        }
    }
    public void AddCardToDiscard(CardScr Card)
    {
        discardedCards.Add(Card);
        if (discardedCards[0] is CropScr)
        {
            discardedCardPrefab.GetComponent<Image>().sprite = backCardIcon[0];
        }
        else if (discardedCards[0] is ItemScr)
        {
            discardedCardPrefab.GetComponent<Image>().sprite = backCardIcon[1];
        }
    }
    public void InitializeAllCardsPositions()
    {
        List<GameObject> Cards = new List<GameObject>();

        for (int i = 0; i < openedCards.Count; i++)
        {
            if (openedCards[i] != itemCardUsing && openedCards[i] != cropCardUsing)
            {
                Cards.Add(openedCards[i]);
            }
        }
        int CardCount = Cards.Count;
        float startingXpos = -xPos * CardCount / 2 + 0.5f;
        for (int i = 0; i < Cards.Count; i++)
        {
            GameObject Card = Cards[i];
            CardData _CardScript = Card.GetComponent<CardData>();
            if (Card != null)
            {
                _CardScript.SortLayers(CardCount - i);
                Card.transform.DOLocalMove(new Vector3(startingXpos + xPos * i, yPos, 0), 0.2f).SetEase(Ease.Linear);
            }
        }

    }


    public IEnumerator PutDiscardThisCard(GameObject ChosenCardToDiscard)
    {
        for (int i = 0; i < openedCards.Count; i++)
        {
            if (ChosenCardToDiscard == openedCards[i])
            {
                openedCards.RemoveAt(i);
                break;
            }
        }
        GManager.PlaySound(1);
        RaycastHit2D hit = Physics2D.Raycast(discardedCardPrefab.transform.position, Vector2.zero);
        Vector2 targetPosition = hit.point;
        print(targetPosition);
        ChosenCardToDiscard.transform.parent = null;
        ChosenCardToDiscard.tag = "Untagged";
        ChosenCardToDiscard.transform.DOKill();
        ChosenCardToDiscard.transform.DOMove(hit.point, 0.1f);
        ChosenCardToDiscard.transform.DOScale(Vector3.one, 0.1f);
        yield return new WaitForSecondsRealtime(0.1f);
        InitializeAllCardsPositions();
        AddCardToDiscard(ChosenCardToDiscard.GetComponent<CardData>().Card);
        Destroy(ChosenCardToDiscard);

    }
    public void MakeThisCardUsingCard(bool trueorfalse, GameObject ChosenObject)
    {
        if (trueorfalse)
        {
            CardScr cardData = ChosenObject.GetComponent<CardData>().Card;
            if (cardData.IsThatBossCard) { BossManager.BossCardUsed(); ChosenObject.GetComponent<CardData>().FadeCard(true); InitializeAllCardsPositions(); }
            else
            {
                if (cardData is CropScr)
                {
                    cropCardUsing = ChosenObject;
                    GManager.activeCardState = ActiveCardState.IN_HAND;
                    MakeThisCardUsingCardAnim(ChosenObject);
                }
                else if (cardData is ItemScr ItemCardData)
                {
                    itemCardUsing = ChosenObject;
                    GManager.activeCardState = ActiveCardState.IN_HAND;
                    for (int i = 0; i < openedCards.Count; i++)
                    {
                        if (openedCards[i] == itemCardUsing)
                        {
                            switch (ItemCardData.itemId)
                            {
                                case 1:
                                    for (int j = 0; j < 3; j++)
                                    {
                                        GManager.farmsScr.water(6, 4, true);

                                    }
                                    GManager.PlaySound(6);
                                    GManager.activeCardState = ActiveCardState.NONE;
                                    itemCardUsing.GetComponent<CardData>().FadeCard(false);
                                    InitializeAllCardsPositions();
                                    break;
                                case 4:
                                    for (int j = 0; j < 3; j++)
                                    {
                                        TakeCardFromDiscard();
                                    }
                                    GManager.ShopManagement.DayPassedAddCard();
                                    GManager.PlaySound(8);
                                    GManager.activeCardState = ActiveCardState.NONE;
                                    itemCardUsing.GetComponent<CardData>().FadeCard(false);
                                    InitializeAllCardsPositions();
                                    break;
                                case 5:
                                    GManager.QuestionStart(ItemCardData);
                                    GManager.activeCardState = ActiveCardState.USING;
                                    itemCardUsing.GetComponent<CardData>().FadeCard(false);
                                    InitializeAllCardsPositions();
                                    break;
                                default:
                                    MakeThisCardUsingCardAnim(ChosenObject);
                                    break;
                            }


                        }
                    }
                }
            }
        }
        else
        {
            if (itemCardUsing != null) { itemCardUsing = null; }
            if (cropCardUsing != null)
            {
                cropCardUsing = null;
                for (int i = 0; i < GManager.farmsScr.frams.Length; i++)
                {
                    GManager.farmsScr.frams[i].farmImage.color = Color.white;
                }
            }
            ChosenObject.transform.parent = transform;
            ChosenObject.transform.DOScale(cardPrefab.transform.lossyScale, 0.2f);
            GManager.activeCardState = ActiveCardState.NONE;
            InitializeAllCardsPositions();
        }
    }
    public void MakeThisCardUsingCardAnim(GameObject ChosenObject)
    {
        ChosenObject.transform.parent = OpenedCardCanvas.transform;
        ChosenObject.transform.DOKill();
        ChosenObject.transform.DOMove(new Vector3(6, 0.5f, 0), 0.25f).SetEase(Ease.Linear);
        ChosenObject.transform.DOScale(cardPrefab.transform.lossyScale * 1.5f, 0.25f);
        InitializeAllCardsPositions();
    }
    public void DestroyThisCard(GameObject card)
    {
        for (int i = 0; i < openedCards.Count; i++)
        {
            if (openedCards[i] == card)
            {
                openedCards.RemoveAt(i);
            }
        }
        Destroy(card);
        GManager.activeCardState = ActiveCardState.NONE;
    }

    public IEnumerator UseThisCard(FarmInfo farm)
    {
        float cardAnimTime = 0.5f;
        if (farm.Id == 1 && cropCardUsing)
        {
            GameObject chosenCardUsing = cropCardUsing;
            GManager.activeCardState = ActiveCardState.USING;
            CardScr card = cropCardUsing.GetComponent<CardData>().Card;
            UseThisCardAnim(chosenCardUsing, farm);
            yield return new WaitForSecondsRealtime(cardAnimTime);
            yield return new WaitForSecondsRealtime(0.2f);
            if (card is CropScr CropCard)
            {
                GManager.PlaySound(4);
                GManager.SeedCrop(CropCard.id, farm);
                for (int i = 0; i < GManager.farmsScr.frams.Length; i++)
                {
                    GManager.farmsScr.frams[i].farmImage.color = Color.white;
                }
                GManager.activeCardState = ActiveCardState.NONE;
            }

            if (!card.IsThatOneUse) { AddCardToDiscard(card); }
            DestroyThisCard(chosenCardUsing);
        }
        else if (itemCardUsing)
        {
            CardScr card = itemCardUsing.GetComponent<CardData>().Card;
            if (card is ItemScr ItemCard)
            {
                GameObject chosenCardUsing = null;
                switch (ItemCard.itemId)
                {
                    case 0://gübre
                        if (8 > farm.Id && farm.Id > 0)
                        {
                            GManager.activeCardState = ActiveCardState.USING;
                            chosenCardUsing = itemCardUsing;
                            UseThisCardAnim(chosenCardUsing, farm);
                            yield return new WaitForSecondsRealtime(cardAnimTime);
                            farm.curDay = farm.reqDay;
                            GManager.activeCardState = ActiveCardState.NONE;
                            farm.ChangeScale(1.1f, 1);
                            farm.InitializeSpriteAndEffect();
                        }
                        else { yield break; }

                        break;
                    case 2://kutsal çapa
                        if (farm.Id < 2)
                        {
                            GManager.activeCardState = ActiveCardState.USING;
                            chosenCardUsing = itemCardUsing;
                            UseThisCardAnim(chosenCardUsing, farm);
                            yield return new WaitForSecondsRealtime(cardAnimTime);
                            GManager.PlaySound(3);
                            farm.HolyHoed = true;
                            farm.Id = 1;
                            GManager.activeCardState = ActiveCardState.NONE;
                            farm.ChangeScale(1.1f, 1);
                            farm.InitializeSpriteAndEffect();
                        }
                        else { yield break; }

                        break;
                    case 3://makas
                        if (farm.Id > 0)
                        {
                            GManager.activeCardState = ActiveCardState.USING;
                            chosenCardUsing = itemCardUsing;
                            UseThisCardAnim(chosenCardUsing, farm);
                            yield return new WaitForSecondsRealtime(cardAnimTime);
                            farm.Id = 1;
                            farm.curDay = 0;
                            farm.reqDay = 0;
                            GManager.activeCardState = ActiveCardState.NONE;
                            farm.ChangeScale(1.1f, 1);
                            farm.InitializeSpriteAndEffect();
                        }
                        else { yield break; }

                        break;
                }

                if (!card.IsThatOneUse) { AddCardToDiscard(card); }
                DestroyThisCard(chosenCardUsing);

            }
        }

    }
    public void UseThisCardAnim(GameObject chosenCardUsing, FarmInfo farm)
    {
        float cardAnimTime = 0.5f;
        Sequence seq = DOTween.Sequence();
        chosenCardUsing.transform.DORotate(new Vector3(0, 0, 720), 0.2f, RotateMode.FastBeyond360);
        chosenCardUsing.transform.DOMoveX(farm.transform.position.x, cardAnimTime).SetEase(Ease.Linear);
        seq.Append(chosenCardUsing.transform.DOMoveY(farm.transform.position.y + 1f, cardAnimTime / 2));
        seq.Append(chosenCardUsing.transform.DOMoveY(farm.transform.position.y, cardAnimTime / 2));
        chosenCardUsing.transform.DOScale(Vector3.zero, cardAnimTime).OnComplete(() => DestroyThisCard(chosenCardUsing));

    }


}
