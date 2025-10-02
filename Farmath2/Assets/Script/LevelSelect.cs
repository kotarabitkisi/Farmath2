using DG.Tweening;
using UnityEngine;

public class LevelSelect : MonoBehaviour
{
    public GameObject[] Levels;
    public GameObject[] levelSelects;
    public CameraMove camMove;
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray);
            print(hit.collider);
            if (hit.collider != null)
            {
                for (int i = 0; i < levelSelects.Length; i++)
                {
                    if (hit.collider.gameObject == levelSelects[i])
                    {
                        LevelAnimStart(i);
                    }
                }
            }


        }
    }
    public void LevelAnimStart(int level)
    {
        Levels[level].SetActive(true);
        Levels[level].transform.DOScale(Vector3.one, 1);
        camMove.enabled = false;
    }
}
