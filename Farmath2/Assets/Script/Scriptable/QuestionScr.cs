using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Question", menuName = "Question/Question")]
public class QuestionScr : ScriptableObject
{
    public int questionreward;
    public string questionTitle, questionText;
    public Sprite QuestionIcon;
    public string Solution;
}
