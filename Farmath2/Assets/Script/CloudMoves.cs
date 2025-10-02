using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class CloudMoves : MonoBehaviour
{
    public List<GameObject> cloudParent;
    public void FadeClouds()
    {
        List<GameObject> clouds = new List<GameObject>();

        int level = 0;
        if (PlayerPrefs.HasKey("Level"))
        {
            level = PlayerPrefs.GetInt("Level");
        }

        for (int i = 0; i < level; i++)
        {
            cloudParent[i].SetActive(false);
        }
        for (int i = 0; i < cloudParent[0].transform.childCount; i++)
        {
            clouds.Add(cloudParent[level].transform.GetChild(i).gameObject);
        }
        foreach (GameObject cloud in clouds)
        {
            cloud.transform.DOMoveX(cloud.transform.position.x + Random.Range(-10, 10), 4);
            cloud.GetComponent<SpriteRenderer>().DOFade(0, 4);
        }
    }
}
