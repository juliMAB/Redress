using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundsButtonsManager : MonoBehaviour
{
    [SerializeField] AK.Wwise.Event onHover;
    [SerializeField] AK.Wwise.Event onclick;
    [SerializeField] AK.Wwise.Event onPlay;
    public void OnHoverSound()
    {
        onHover.Post(gameObject);
    }
    public void OnClickSound()
    {
        onclick.Post(gameObject);
    }
    public void OnPlay()
    {
        onPlay.Post(gameObject);
    }
}

