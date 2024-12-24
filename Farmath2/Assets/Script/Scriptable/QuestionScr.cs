using UnityEngine;
[CreateAssetMenu(fileName = "Question", menuName = "Question/Question")]
public class QuestionScr : ScriptableObject
{
    public string questionTitle, questionText;
    public Sprite QuestionIcon;
    public string Solution;
}
