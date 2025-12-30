using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class CloudMoves : MonoBehaviour
{
    public List<Transform> cloudTransform;
    public List<Vector2> Transformx;
    public List<Vector2> offset;
    public float Period,MoveScale;
    public List<float> timeDelay;
    public List<GameObject> cloudParent;
    public int level;
    private void Start()
    {
        if (PlayerPrefs.HasKey("Level"))
        {
            level = PlayerPrefs.GetInt("Level");
        }
        for (int i = 0; i < level; i++)
        {
            FadeClouds(i);
        }
        for (int i = 0; i < cloudTransform.Count; i++)
        {
            Transformx.Add(cloudTransform[i].position);
            offset.Add(new Vector2(Random.Range(-MoveScale, MoveScale), 0));
            timeDelay.Add(Period*Random.Range(0,360));
        }
    }
    private void Update()
    {
        for (int i = 0; i < cloudTransform.Count; i++)
        {
            cloudTransform[i].transform.position = Vector3.Lerp(Transformx[i] - offset[i]*MoveScale, Transformx[i] + offset[i] * MoveScale, (Mathf.Sin((Time.time + timeDelay[i])/Period)+1)/2);
        }
    }
    public void FadeClouds(int level)
    {
        List<GameObject> clouds = new List<GameObject>();
        for (int i = 0; i < level; i++)
        {
            cloudParent[i].SetActive(false);
        }
        for (int i = 0; i < cloudParent[level].transform.childCount; i++)
        {
            clouds.Add(cloudParent[level].transform.GetChild(i).gameObject);
        }
        foreach (GameObject cloud in clouds)
        {
            cloud.transform.DOMoveX(cloud.transform.position.x + Random.Range(-10, 10), 4);
            cloud.GetComponent<SpriteRenderer>().DOFade(0, 4);
        }
    }
    public void FadeClouds()
    {
        List<GameObject> clouds = new List<GameObject>();
        for (int i = 0; i < level; i++)
        {
            cloudParent[i].SetActive(false);
        }
        for (int i = 0; i < cloudParent[level].transform.childCount; i++)
        {
            clouds.Add(cloudParent[level].transform.GetChild(i).gameObject);
        }
        foreach (GameObject cloud in clouds)
        {
            cloud.transform.DOMoveX(cloud.transform.position.x + Random.Range(-10, 10), 4);
            cloud.GetComponent<SpriteRenderer>().DOFade(0, 4);
        }
    }
    [ContextMenu("EraseLevel")]
    public void SetLevelSave()
    {
        PlayerPrefs.DeleteKey("Level");
    }
}
