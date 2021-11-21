using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AK;
using UnityEngine.UI;

public class AudioController : MonoBehaviour
{
    [SerializeField] Slider slider;
   public void OnCall()
    {
        AkSoundEngine.SetRTPCValue("VolumenFx", slider.value * 10);
        AkSoundEngine.PostEvent("play_laser", gameObject);
    }
}
