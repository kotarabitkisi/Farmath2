using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightIntensityChanger : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    public bool night;
    Light2D Light_;
    [SerializeField] float changeSpeed;
    float randomint;
    private void Start()
    {
        randomint = Random.value * 1000;
        Light_ = GetComponent<Light2D>();
    }
    void Update()
    {

        if (gameManager.Boss != 0)
        {
            if (!night)
            {
                night = true;
                StartCoroutine(TurnOffSmoothly());
            }
        }
        else
        {
            Light_.intensity = Mathf.Lerp(0.25f, 1, Mathf.Sin(Time.time * changeSpeed + randomint));
        }
    }
    public IEnumerator TurnOffSmoothly()
    {
        for (float i = 0; i < 100; i++)
        {
            Light_.intensity = Mathf.Lerp(Light_.intensity, 0, i / 99);
            yield return new WaitForSecondsRealtime(0.03f);
        }
    }
}
