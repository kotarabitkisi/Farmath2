using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "DefCard", menuName = "Cards/DefCard")]
public class CardScr : ScriptableObject
{
    public string CardName;
    public Sprite Icon;
    public float CardCost;
    public bool IsThatBossCard;
    public virtual void Use() { 
    
    }
    public virtual void Use(int ItemIcon)
    {

    }
}
