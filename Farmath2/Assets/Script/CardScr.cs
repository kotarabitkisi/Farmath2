using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Card", menuName = "ScriptableObjects/Card")]
public class CardScr : ScriptableObject
{
    public string CardName;
    public Sprite Icon;
    public float CardCost;
    public virtual void Use() {  }
}
