using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class LevelSettings : MonoBehaviour
{
    public saveAndLoad saveandload;
    public static LevelSettings instance;
    public UniversalRenderPipelineAsset Asset;
    public TMP_Dropdown FpsDropdown;
    [Header("EnivornmentToggle")]
    public Toggle grassToggle;
    public Toggle shadowToggle;
    public Toggle lightToggle;
    public Toggle pollenToggle;
    [Header("VolumeToggle")]
    public Toggle volToggle;
    public Toggle bloomToggle;
    public Toggle colorAdjToggle;
    public Toggle vigToggle;
    public Toggle hdrToggle;

    [Space(3)]
    public Slider soundSlider;
    public Slider renderScaleSlider;

    [Header("VolumeSettings")]
    public Volume vol;
    public Bloom bloom;
    public ColorAdjustments colorAdjustments;
    public Vignette vignette;


    [Header("Environment")]
    public GameObject Grass;
    public GameObject Shadow, Lights;
    public GameObject Pollen;
    public AudioSource AudioSource;
    private void Awake()
    {
        instance = this;
    }
    public void ResetSettings()
    {
        FpsDropdown.value = saveandload.defaultSettings.FPS;
        pollenToggle.isOn = saveandload.defaultSettings.pollen;
        grassToggle.isOn = saveandload.defaultSettings.grass;
        shadowToggle.isOn = saveandload.defaultSettings.shadow;
        lightToggle.isOn = saveandload.defaultSettings.lights;
        volToggle.isOn = saveandload.defaultSettings.postProcessing;
        vigToggle.isOn = saveandload.defaultSettings.vignette;
        bloomToggle.isOn = saveandload.defaultSettings.Bloom;
        colorAdjToggle.isOn = saveandload.defaultSettings.colorAdj;
        hdrToggle.isOn = saveandload.defaultSettings.HDR;
        renderScaleSlider.value = saveandload.defaultSettings.renderScaleLevel;
        soundSlider.value = saveandload.defaultSettings.soundLevel;
        AllSettings();
    }
    public void AllSettings()
    {
        SetRenderScale();
        ToggleBloom();
        ToggleGrass();
        ToggleShadow();
        ToggleHDR();
        ToggleColorAdjustment();
        ToggleSunshine();
        ToggleVignette();
        ToggleVolume();
        ChangeFocusFPS();
        ChangeSoundLevel();
        InitializeVolume();
    }
    public void SetRenderScale()
    {
        Asset.renderScale = renderScaleSlider.value;
    }
    public void ToggleHDR()
    {
        Asset.supportsHDR = hdrToggle.isOn;
    }
    public void InitializeVolume()
    {
        Camera camera = Camera.main;
        camera.UpdateVolumeStack();
        VolumeManager.instance.Update(camera.transform, camera.GetComponent<UniversalAdditionalCameraData>().volumeLayerMask);
    }
    public void ChangeSoundLevel()
    {
        AudioSource.volume = soundSlider.value;
    }
    public void ToggleBloom()
    {
        if (vol.profile.TryGet(out bloom))
        {
            bloom.active = bloomToggle.isOn;
        }
    }
    public void ToggleColorAdjustment()
    {
        if (vol.profile.TryGet(out colorAdjustments))
        {
            colorAdjustments.active = colorAdjToggle.isOn;
        }
    }
    public void ToggleVignette()
    {
        if (vol.profile.TryGet(out vignette))
        {
            vignette.active = vigToggle.isOn;
        }
    }
    public void ToggleVolume()
    {
        vol.enabled = volToggle.isOn;
    }
    public void ToggleGrass()
    {
        Grass.SetActive(grassToggle.isOn);
    }
    public void ToggleShadow()
    {
        Shadow.SetActive(shadowToggle.isOn);
    }
    public void ToggleSunshine()
    {
        Lights.SetActive(lightToggle.isOn);
    }
    public void TogglePollen()
    {
        Pollen.SetActive(pollenToggle.isOn);
    }
    public void ChangeFocusFPS()
    {
        int value = FpsDropdown.value;
        switch (value)
        {
            case 0:
                FPSShower.instance.ChangeFocusFPS(30);
                break;
            case 1:
                FPSShower.instance.ChangeFocusFPS(60);
                break;
            case 2:
                FPSShower.instance.ChangeFocusFPS(90);
                break;
            case 3:
                FPSShower.instance.ChangeFocusFPS(144);
                break;
            case 4:
                FPSShower.instance.ChangeFocusFPS(9999);
                break;
        }

    }
}
