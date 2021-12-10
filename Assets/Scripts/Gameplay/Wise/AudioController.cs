using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AK;
using UnityEngine.UI;
using Redress.Management;

public class AudioController : MonoBehaviour
{
    [SerializeField] Slider sliderMusic;
    [SerializeField] Slider sliderFx;
    private void Start()
    {
        sliderFx.value = GameManager.Get().FxVolumen;
        sliderMusic.value = GameManager.Get().MusicVolumen;
    }
    public void OnCallVolumen()
    {
        AkSoundEngine.SetRTPCValue("VolumenMusic", sliderMusic.value * 100);
        GameManager.Get().MusicVolumen = sliderMusic.value;
    }
    public void OnCallFx()
    {
        AkSoundEngine.SetRTPCValue("VolumenFx", sliderFx.value * 100);
        GameManager.Get().FxVolumen = sliderFx.value;
    }
}
