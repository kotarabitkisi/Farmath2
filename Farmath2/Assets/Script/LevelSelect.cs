using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelect : MonoBehaviour
{
    public Vector2[] Levels;
    public GameObject[] levelSelects;
    public CameraMove camMove;
    public AnimationVar[] Animations;
    public int level;
    public int curAnimationId;
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (curAnimationId == -1)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.GetRayIntersection(ray);
                if (hit.collider != null)
                {
                    for (int i = 0; i < levelSelects.Length; i++)
                    {
                        if (hit.collider.gameObject == levelSelects[i])
                        {
                            LevelAnimStart(i, 0);
                        }
                    }
                }
            }
            else
            {
                LevelAnimStart(level, curAnimationId+1);
            }
        }
    }
    public void LevelAnimStart(int level_, int animId)
    {
        if (animId == 0) { level = level_; Camera.main.transform.DOMove(Animations[level].Transforms[0], Animations[level].moveTime[0]); }
        else if (animId == Animations[level].Transforms.Length) { SceneManager.LoadScene(level); }
        else
        {
            Camera.main.transform.DOMove(Animations[level].Transforms[animId], Animations[level].moveTime[animId]);
        }
    }
    [System.Serializable]
    public class AnimationVar
    {
        public Vector2[] Transforms;
        public float[] moveTime;
    }
}
