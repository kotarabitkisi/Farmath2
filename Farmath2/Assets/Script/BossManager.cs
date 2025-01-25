using UnityEngine;

public class BossManager : MonoBehaviour
{
    public GameObject Camera;
    public DeckPlacing DeckPlacing;
    public GameManager gameManager;
    public EffectColorChanging ECC;
    public CardScr[] AllBossCards;
    public FarmInfo[] AllFarms;
    public void BossStart()
    {
        StartCoroutine(ECC.ChangeColor(gameManager.deckPlacing.BossSoytariColor, 5));
        BossJoinAnim();
    }
    void BossJoinAnim()
    {






    }
    public void BossCardUsed()
    {
        StartCoroutine(gameManager.ShakeTheObj(Camera, 0.2f, 0.05f, 0, false));
        gameManager.money *= 0.995f;
        gameManager.InitializeMoneyText();
    }
    public void AddBossCard()
    {
        DeckPlacing.AddCardToDiscard(AllBossCards[Random.Range(0, AllBossCards.Length)]);
    }
    
    public void BossIsTalkingAboutSomething() { }
    public void BossPutVein()
    {
        print("a");
        for (int i = 0; i < AllFarms.Length; i++)
        {
            AllFarms[i].Negatived = false;
        }
        for (int i = 0; i < 12; i++)
        {
            bool worked = false;
            while (!worked)
            {
                FarmInfo chosenFarm = AllFarms[Random.Range(0,24)];
                print("b");
                if (!chosenFarm.Negatived) {chosenFarm.Negatived = true; worked = true; }
            }
        }
    }
}
