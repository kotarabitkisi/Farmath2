using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Logger : MonoBehaviour
{
    public GameObject logObj;
    public GameObject logParent;
    const int ChildCountbound=10;
    public void Addlog(string text)
    {
        GameObject LogObj= Instantiate(logObj);
        LogObj.transform.parent=logParent.transform;
        LogObj.transform.SetAsFirstSibling();
        LogObj.GetComponent<TextMeshProUGUI>().text = text;
        LayoutRebuilder.ForceRebuildLayoutImmediate(logParent.GetComponent<RectTransform>());
        print(logParent.transform.childCount);
       
    }
    private void Update()
    {
        if (logParent.transform.childCount >= 10)
        {
            Destroy(logParent.transform.GetChild(logParent.transform.childCount - 1).gameObject);
        }
    }
}
