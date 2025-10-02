using System.Collections;
using System.Threading;
using TMPro;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class FPSShower : MonoBehaviour
{
    public static FPSShower instance;
    public TMP_Text fpsText; // UI Text / TMP Text atanacak
    private float deltaTime = 0.0f;
    [Header("Frame Settings")]
    int TargetFrameRate = 90;
    void Awake()
    {
        instance = this;
        ChangeFocusFPS(TargetFrameRate);
    }
    private void Start()
    {
        ChangeFocusFPS(TargetFrameRate);
    }
    void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            ChangeFocusFPS(TargetFrameRate);
        }
    }
    public void ChangeFocusFPS(int fps) {
        TargetFrameRate = fps;
        Application.targetFrameRate = fps;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        QualitySettings.vSyncCount = 0;
    }
    void Update()
    {
        // DeltaTime’ýn hareketli ortalamasýný al
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;

        // FPS hesapla
        float fps = 1.0f / deltaTime;

        // Text’e yazdýr
        if (fpsText != null)
            fpsText.text = fps.ToString("0");
    }
}


