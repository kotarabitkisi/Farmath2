using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Logger : MonoBehaviour
{
    public List<Coroutine> coroutines=new List<Coroutine>();
    public static Logger instance;
    public GameObject LogObj;
    public GameObject[] Buttons;
    public bool[] tutorials;
    public Image logImage;
    public Sprite[] charImages;
    public DialogScr[] dialogScr;
    public TextMeshProUGUI dialogText;
    public bool textingFinished;
    public int dialogQueue;
    public int dialogNumber;
    private void Awake()
    {
        instance = this;
    }
    private void Update()
    {

#if UNITY_STANDALONE_WIN
        if (dialogQueue != -1 && textingFinished && (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.KeypadEnter))) {coroutines.Add( StartCoroutine(SampleDialog(dialogNumber, dialogQueue + 1))); }
#elif UNITY_ANDROID
        if (dialogQueue != -1 && textingFinished && Input.touchCount > 0) { coroutines.Add( StartCoroutine(SampleDialog(dialogNumber, dialogQueue + 1))); }
#endif

    }
    public void ControlAllTutorialPlayed()
    {
        for (int i = 0; i < 6; i++)
        {
            if (!tutorials[i])
            {
                return;
            }
        }
        GameManager.instance.tutorialPlayed = true;

        StartDialouge(6);
    }
    public void StartDialougeCondition(int conditionId)//0:buy Crop 1:Boss Coming 2:boss Uses Vines 3:hoe dirt 4:Open Shop 5:Use Card 6:Solve Question 7:take from discard 8:eugene skill 9:sheep skill
    {
        if (!GameManager.instance.tutorialPlayed) { return; }
        List<DialogWithConditions> dialogWithConditionList = new List<DialogWithConditions>();
        foreach (DialogScr dialogScr_ in dialogScr)
        {
            if (dialogScr_ is DialogWithConditions DialogWithCondition)
            {
                dialogWithConditionList.Add(DialogWithCondition);
            }
        }
        DialogScr chosenDialog = null;
        int chosenId = Random.Range(0, dialogWithConditionList.Count);
        for (int i = 0; i < 1000; i++)
        {
            //düzelt
            if (dialogWithConditionList[chosenId].hero >= 2)
            {
                if (dialogWithConditionList[chosenId].ConditionId == conditionId && GameManager.instance.farmers[dialogWithConditionList[chosenId].hero - 2].GetComponent<FarmerInfo>().choosed)
                {
                    chosenDialog = dialogWithConditionList[chosenId];
                    break;
                }
            }
            else if(dialogWithConditionList[chosenId].ConditionId == conditionId)
            {
                chosenDialog = dialogWithConditionList[chosenId];
                break;
            }

        }
        if (chosenDialog == null) { return; }
        for (int i = 0; i < dialogScr.Length; i++)
        {
            if (dialogScr[i] == chosenDialog)
            {
                StartDialouge(i);
                break;
            }
        }
    }
    public void StartDialouge(int dialognumber)
    {
        if (dialognumber == 5 || dialognumber == 4)
        {
            if (!tutorials[dialognumber] && CalculatePlayedDialogTutorial() >= dialognumber)
            {
                StopAllCoroutines_();
                LogObj.SetActive(true);
                tutorials[dialognumber] = true;
                coroutines.Add(StartCoroutine(SampleDialog(dialognumber, 0)));
            }
            return;
        }
        else
        {
            StopAllCoroutines_();
            LogObj.SetActive(true);
            dialogQueue = 0;
            coroutines.Add(StartCoroutine(SampleDialog(dialognumber, 0)));
        }
    }
    public void StopAllCoroutines_()
    {
        if (coroutines.Count == 0) { return; }
        foreach(Coroutine coroutine_ in coroutines)
        {
          StopCoroutine(coroutine_);
        }
        coroutines.Clear();
    }
    public IEnumerator SampleDialog(int dialognumber_, int dialogQueue_)
    {
        if (!GameManager.instance.tutorialPlayed && dialogNumber < 6)
        {
            tutorials[dialognumber_] = true;
        }
        if (dialogQueue_ == dialogScr[dialognumber_].dialogs.Length)
        {
            FinishDialog(dialognumber_);
            yield break;
        }
        DialogScr.Dialog dialog = dialogScr[dialognumber_].dialogs[dialogQueue_];
        textingFinished = false;
        dialogNumber = dialognumber_;
        dialogText.text = "";
        dialogQueue = dialogQueue_;

        float delay = dialog.dialogDelay;
        int charnumber = dialog.characterId;
        Sprite beforeImage = logImage.sprite;
        logImage.sprite = charImages[charnumber];//0:boy 1:girl 2:betty 3:bert 4:eugene 5:sheep 6:soytari
        string dialogTxt = LanguageManager.instance.TurnToString(dialog.text, null);

        Sequence seq = DOTween.Sequence();
        seq.Append(dialogText.transform.parent.DOScale(Vector3.one * 1.1f, 0.25f));

        if (beforeImage != logImage.sprite)
        {
            seq.Join(logImage.transform.DOScale(Vector3.one * 1.1f, 0.25f));
        }
        seq.Append(dialogText.transform.parent.DOScale(Vector3.one * 1f, 0.25f));
        if (beforeImage != logImage.sprite)
        {
            seq.Join(logImage.transform.DOScale(Vector3.one * 1f, 0.25f));
        }

        foreach (char c in dialogTxt)
        {
            dialogText.text += c;
            yield return new WaitForSeconds(delay);
        }
        if (dialog.timeToNextDialouge != -1)
        {
            yield return new WaitForSecondsRealtime(dialog.timeToNextDialouge);
            StartCoroutine(SampleDialog(dialogNumber, dialogQueue + 1));
        }
        else
        {
            textingFinished = true;
        }
    }
    public void FinishDialog(int dialognumber_)
    {
        dialogQueue = -1;
        textingFinished = true;
        if (!GameManager.instance.tutorialPlayed)
        {
            switch (dialognumber_)
            {
                case 0:
                    Buttons[0].SetActive(true);//ShopBtn
                    break;
                case 1:
                    Buttons[1].SetActive(true);//NextDayBtn
                    break;
                case 3:
                    Buttons[2].SetActive(true);//InfoBtn
                    break;
                case 4:
                    Buttons[3].SetActive(true);//FarmerBtn
                    break;
            }
        }
        if (dialogScr[dialognumber_].DisableLoggerWhenDialogfinished)
        {
            LogObj.SetActive(false);
        }
        if (!GameManager.instance.tutorialPlayed)
        {
            ControlAllTutorialPlayed();
        }


    }
    public int CalculatePlayedDialogTutorial()
    {
        int a = 0;
        for (int i = 0; i < tutorials.Length; i++)
        {
            if (tutorials[i]) { a++; }
        }
        return a;
    }
}
