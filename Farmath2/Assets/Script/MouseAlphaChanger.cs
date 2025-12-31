using UnityEngine;

public class MouseAlphaChanger : MonoBehaviour
{
    public float minAlpha;
    public Vector2 offset;
    public RectTransform rectTransform;
    public float minDistance, MaxDistance;
    CanvasGroup canvasGroup;
    private void Start()
    {
        if (!GetComponent<CanvasGroup>()) { canvasGroup = gameObject.AddComponent<CanvasGroup>(); }
        else { canvasGroup = GetComponent<CanvasGroup>(); }
    }
    void Update()
    {
        Vector2 mousePos = Input.mousePosition;
        Vector2 ObjPosition = RectTransformUtility.WorldToScreenPoint(null, rectTransform.position) + offset;

        float distance = Vector2.Distance(mousePos, ObjPosition);
        float alpha = Mathf.InverseLerp(minDistance, MaxDistance, distance);
        alpha = Mathf.Clamp(alpha, minAlpha, 1);
        canvasGroup.alpha = alpha;

    }
}
