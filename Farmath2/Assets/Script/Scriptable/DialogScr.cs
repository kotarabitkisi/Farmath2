using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "DialogScr", menuName = "Scriptable Objects/DialogScr")]
public class DialogScr : ScriptableObject
{
    public Dialog[] dialogs;

    [System.Serializable]
    public class Dialog
    {
        public int characterId;
        public string text;
        public float dialogDelay;
        public float timeToNextDialouge;
    }
}
