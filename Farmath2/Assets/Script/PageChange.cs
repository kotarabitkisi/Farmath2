using UnityEngine;

public class PageChange : MonoBehaviour
{
    public int pageIndex;
    public GameObject[] Pages;
    public GameObject leftButton, rightButton;
    public void ChangePage(int index)
    {
        pageIndex += index;
        for (int i = 0; i < Pages.Length; i++)
        {
            Pages[i].SetActive(false);
        }
        Pages[pageIndex].SetActive(true);
        leftButton.SetActive(pageIndex != 0);
        rightButton.SetActive(pageIndex != Pages.Length - 1);

    }
}
