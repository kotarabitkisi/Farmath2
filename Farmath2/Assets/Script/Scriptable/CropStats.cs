using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

[CreateAssetMenu(fileName = "Crop", menuName = "ScriptableObjects/Crops")]
public class CropStats : ScriptableObject
{
    public float base_;
    public int id;
    public string cropName;
    public int reqDayToGrow;
    public float[] Rev;
    public int[] RevOperation;


    [Header("Crop_Conditions")]
    public int[] reqConnectedIds;
    public int reqHarvestDaymin;
    public int reqHarvestDaymax;
    public int reqMoneyMin;
    public int reqMoneyMax;
    public int reqdayMin;
    public int reqdayMax;
    public int reqHarvestCountmin;
    public int reqHarvestCountmax;
    public int reqHarvestCountOnSameTypemin;
    public int reqHarvestCountOnSameTypemax;
    public bool IsNeighbour(int[] connectedFarmIds)
    {
        bool[] conIds = new bool[] { false, false, false, false };
        List<int> AllconnectedIds= new List<int> { };
        for (int  i = 0;  i <connectedFarmIds.Length;  i++)
        {
            AllconnectedIds.Add(connectedFarmIds[i]);
        }

        for (int i = 0; i < reqConnectedIds.Length; i++)
        {
            if (reqConnectedIds[i] == 0)
            {
                conIds[i] = true;
                Debug.Log("empty");
            }
            else
            {
                for (int j = 0; j < AllconnectedIds.Count; j++)
                {

                    if (AllconnectedIds[j] == reqConnectedIds[i])
                    {
                        Debug.Log("true");
                        conIds[i] = true;
                        AllconnectedIds.Remove(j);
                        break;

                    }
                    else
                    {
                        Debug.Log("false");
                    }
                }
            }

        }
        for (int i = 0; i < conIds.Length; i++)
        {
            if (!conIds[i]) { Debug.Log("" + conIds[0] + conIds[1] + conIds[2] + conIds[3] + "False"); return false; }
        }
        Debug.Log("" + conIds[0] + conIds[1] + conIds[2] + conIds[3] + "True");
        return true;
    }
    public bool IsHarvestDayPassed(FarmInfo farmI)
    {
        int diff = farmI.curDay - farmI.reqDay;
        if (reqHarvestDaymin <= diff && diff <= reqHarvestDaymax)
        {
            return true;
        }
        else
        {
            return false;
        }

    }
    public bool IsMoneyBetweenTheseNum(float money)
    {
        if (money >= reqMoneyMin && money <= reqMoneyMax)
        {
            return true;
        }
        else { return false; }
    }
    public bool IsDayBetweenTheseNum(int day)
    {
        if (day >= reqdayMin && day <= reqdayMax)
        {
            return true;
        }
        else { return false; }
    }
    public bool IsHarvestCount(bool istype, int harvestCount)
    {
        if (istype)
        {
            if (harvestCount >= reqHarvestCountOnSameTypemin && harvestCount <= reqHarvestCountOnSameTypemax)
            {
                return true;
            }
            else { return false; }
        }
        else
        {
            if (harvestCount >= reqHarvestCountmin && harvestCount <= reqHarvestCountmax)
            {
                return true;
            }
            else { return false; }
        }


    }
}
