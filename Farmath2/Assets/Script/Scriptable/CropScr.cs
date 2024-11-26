using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CropCard", menuName = "Cards/CropCard")]
public class CropScr : CardScr
{
    public int id;
    public string cropName;
    public Sprite CropSprite;
    public GameManager GManager;
    public override void Use()
    {

    }

}
