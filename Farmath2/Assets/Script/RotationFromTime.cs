using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationFromTime : MonoBehaviour
{
    [SerializeField] Quaternion rot1, rot2;
    [SerializeField] float TimePeriod;
    void Update()
    {
        transform.rotation = Quaternion.Lerp(rot1,rot2,(Mathf.Sin(Time.time*360/TimePeriod)+1)/2);
    }
}
