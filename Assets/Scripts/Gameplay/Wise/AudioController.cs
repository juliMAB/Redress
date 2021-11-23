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
        sliderFx.value = GameManager.Instance.FxVolumen;
        sliderMusic.value = GameManager.Instance.MusicVolumen;
    }
    public void OnCallVolumen()
    {
        AkSoundEngine.SetRTPCValue("VolumenMusic", sliderMusic.value * 100);
        GameManager.Instance.MusicVolumen = sliderMusic.value;
    }
    public void OnCallFx()
    {
        AkSoundEngine.SetRTPCValue("VolumenFx", sliderFx.value * 100);
        GameManager.Instance.FxVolumen = sliderFx.value;
    }
}
