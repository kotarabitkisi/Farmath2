using UnityEngine;

public class Farms : MonoBehaviour
{
    const int direction = 4;
    public const int width = 6;
    public const int height = 4;
    public FarmInfo[] frams;
    public FarmInfo[,] FarmList = new FarmInfo[height, width];
    public int[] totalConnectedIds = new int[8];
    public FarmInfo ChosenFarm;
    public GameManager GameManager;
    private void Awake()
    {
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                FarmList[i, j] = frams[i * width + j];
            }
        }
    }
    public void CalculateTotalConnectedIds()
    {
        for (int i = 0; i < totalConnectedIds.Length; i++)
        {
            int totalConnectedId = 0;
            foreach (FarmInfo farmland in FarmList)
            {
                for (int l = 0; l < direction; l++)
                {
                    if (farmland.connectedFarmIds[l] == i)
                    {
                        totalConnectedId++;
                    }
                }
            }
            totalConnectedIds[i] = totalConnectedId / 2;
        }
        totalConnectedIds[0] = 0;
    }
    public void GrowAllFarms()
    {
        foreach (FarmInfo farmland in FarmList)
        {
            farmland.curDay++;
            farmland.InitializeSpriteAndEffect();
        }
    }
    public void CalculateConnectedCount()
    {

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if (i != 0) { GetConnected(FarmList[i, j], i - 1, j, 0); }
                if (i != height - 1) { GetConnected(FarmList[i, j], i + 1, j, 1); }
                if (j != 0) { GetConnected(FarmList[i, j], i, j - 1, 2); }
                if (j != width - 1) { GetConnected(FarmList[i, j], i, j + 1, 3); }
            }
        }
    }
    public bool isAllHolyHoed()
    {
        foreach (FarmInfo farmland in FarmList)
        {
            if (!farmland.HolyHoed)
            {
                return false;
            }
        }
        return true;
    }
    public void GetConnected(FarmInfo farm, int i, int j, int direction)
    {
        if (farm.Id >= 2 && FarmList[i, j].Id >= 2) { farm.connectedFarmIds[direction] = FarmList[i, j].Id; }
        else { farm.connectedFarmIds[direction] = 0; }
    }
    public void water(int width, int height, bool IsRandom)
    {
        if (IsRandom)
        {
            FarmInfo farm = FarmList[Random.Range(0, height), Random.Range(0, width)];
            farm.Watered = true;
            farm.ChangeScale(1.1f, 1);
            farm.InitializeSpriteAndEffect();
        }
        else
        {
            FarmInfo farm = FarmList[height, width];
            farm.Watered = true;
            farm.ChangeScale(1.1f, 1);
            farm.InitializeSpriteAndEffect();
        }
    }
}
