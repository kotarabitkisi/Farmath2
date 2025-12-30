using UnityEngine;
using DG.Tweening;

public class SpriteCrossfade : MonoBehaviour
{
    public SpriteRenderer currentRenderer;
    public SpriteRenderer nextRenderer;

    public void FadeTo(Sprite newSprite, float duration = 1f)
    {
        if (nextRenderer.sprite == newSprite) { return; }
        nextRenderer.sprite = newSprite;
        nextRenderer.color = new Color(1, 1, 1, 0);
        nextRenderer.DOFade(1f, duration/3);
        currentRenderer.DOFade(0f, duration).OnComplete(() =>
        {
            currentRenderer.sprite = newSprite;
            currentRenderer.color = new Color(1, 1, 1, 1);
            nextRenderer.color = new Color(1, 1, 1, 0);
        });
    }
}

