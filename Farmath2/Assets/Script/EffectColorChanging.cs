using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class EffectColorChanging : MonoBehaviour
{
    public LevelSettings levelSettings;
    public Volume GlobalVol;
    public ColorAdjustments colorAdjustments;

    public IEnumerator ChangeColor(Color ChosenColor,float time)
    {
        if (GlobalVol.profile.TryGet(out colorAdjustments))
        {
            Color curColor = (Color)colorAdjustments.colorFilter;
            for (int i = 0; i < 101; i++)
            {
                colorAdjustments.colorFilter.value = Color.Lerp(curColor, ChosenColor, (float)i / 100);
                levelSettings.InitializeVolume();
                yield return new WaitForSecondsRealtime(0.01f * time);
            }
        }
    }
}
