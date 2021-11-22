using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AK;
using UnityEngine.UI;

public class AudioController : MonoBehaviour
{
    [SerializeField] Slider sliderMusic;
    [SerializeField] Slider sliderFx;
    public void OnCallVolumen()
    {
        AkSoundEngine.SetRTPCValue("VolumenMusic", sliderMusic.value * 100); 
    }
    public void OnCallFx()
    {
        AkSoundEngine.SetRTPCValue("VolumenFx", sliderFx.value * 100);
    }
}
