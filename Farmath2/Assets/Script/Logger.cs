using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Logger : MonoBehaviour
{
    public GameObject[] Buttons;
    public bool[] tutorials;
    public Image logImage;
    public Sprite[] charImages;
    public DialogScr[] dialogScr;
    public TextMeshProUGUI dialogText;
    public bool textingFinished;
    public int dialogQueue;
    public int dialogNumber;
    private void Update()
    {

#if UNITY_STANDALONE_WIN
        if (dialogQueue != -1 && textingFinished && (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.KeypadEnter))) { StartCoroutine(SampleDialog(dialogNumber, dialogQueue + 1)); }
#elif UNITY_ANDROID
        if (dialogQueue != -1 && textingFinished && Input.touchCount > 0) { SampleDialog(dialogNumber, dialogQueue + 1); }
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
    public void StartDialouge(int dialognumber)
    {
        if ((dialognumber == 5 || dialognumber == 4))
        {
            if (!tutorials[dialognumber] && textingFinished && CalculatePlayedDialogTutorial() >= dialognumber)
            {
                tutorials[dialognumber] = true;
                StartCoroutine(SampleDialog(dialognumber, 0));
            }
            return;
        }
        dialogQueue = 0;
        StartCoroutine(SampleDialog(dialognumber, 0));
    }
    public IEnumerator SampleDialog(int dialognumber_, int dialogQueue_)
    {
        if (!GameManager.instance.tutorialPlayed && dialogNumber < 6)
        {
            tutorials[dialognumber_] = true;
        }
        if (dialogQueue_ == dialogScr[dialognumber_].dialogs.Length)
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
                ControlAllTutorialPlayed();
            }
            yield break;
        }
        DialogScr.Dialog dialog = dialogScr[dialognumber_].dialogs[dialogQueue_];
        textingFinished = false;
        dialogNumber = dialognumber_;
        dialogText.text = "";
        dialogQueue = dialogQueue_;

        float delay = dialog.dialogDelay;
        int charnumber = dialog.characterId;
        logImage.sprite = charImages[charnumber];
        foreach (char c in dialog.text)
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
