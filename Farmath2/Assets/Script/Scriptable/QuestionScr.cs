using UnityEngine;
[CreateAssetMenu(fileName = "Question", menuName = "Question/Question")]
public class QuestionScr : ScriptableObject
{
    public string questionTitle;
    public Sprite QuestionIcon;
    public string Solution;
    public float moneyMultiple;
    public float timeOfQuestion;
}
