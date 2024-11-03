using UnityEngine;

[CreateAssetMenu(fileName = "Crop", menuName = "ScriptableObjects/Crops")]
public class CropStats : ScriptableObject
{
    public float Base;
    public int id;
    public string cropName;
    public int reqDayToGrow;
    public Sprite CropSprite;
    public float Cost;
}
