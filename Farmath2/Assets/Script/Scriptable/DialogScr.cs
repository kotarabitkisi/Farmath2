using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "DialogScr", menuName = "Scriptable Objects/DialogScr")]
public class DialogScr : ScriptableObject
{
    public Dialog[] dialogs;
    public bool DisableLoggerWhenDialogfinished;
    [System.Serializable]
    public class Dialog
    {
        public int characterId;
        public float dialogDelay;
        public float timeToNextDialouge;
        public LocalizedString text;
    }
}
