using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelStoryAnim : MonoBehaviour
{
    public GameObject[] movingObjects;
    public Vector3[] whereItComes;
    public float[] Cooldown;
    public float[] timeformove;
    public float timeLeft;
    public int Index;
    private void Update()
    {
        if (Index != movingObjects.Length)
        {
            timeLeft -= Time.deltaTime;
            if (timeLeft <= 0)
            {
                timeLeft = Cooldown[Index];
                playObjAnim(Index);
                Index++;
            }
        }

    }
    public void playObjAnim(int Index) 
    {
        movingObjects[Index].SetActive(true);
        movingObjects[Index].GetComponent<RectTransform>().DOMove(whereItComes[Index], timeformove[Index]);
        CanvasGroup canvasGroup = movingObjects[Index].AddComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        canvasGroup.DOFade(1f, timeformove[Index]);
    }





}
