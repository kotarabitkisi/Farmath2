using UnityEngine;
using UnityEngine.Localization;
[CreateAssetMenu(fileName = "DefCard", menuName = "Cards/DefCard")]
public class CardScr : ScriptableObject
{
    public LocalizedString CardName;
    public Sprite Icon;
    public float CardCost;
    public bool IsThatBossCard;
    public bool IsThatOneUse;
    public virtual void Use()
    {

    }
    public virtual void Use(int ItemIcon)
    {

    }
}
