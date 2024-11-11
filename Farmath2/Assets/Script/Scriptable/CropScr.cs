using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Crop", menuName = "ScriptableObjects/Crop")]
public class CropScr : CardScr
{
    public float Base;
    public int id;
    public string cropName;
    public int reqDayToGrow;
    public Sprite CropSprite;
    public float Cost;
    public GameManager GManager;
    public override void Use()
    {

    }

}
