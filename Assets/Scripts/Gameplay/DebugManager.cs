using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using EndlessT4cos.Gameplay.Management;
using EndlessT4cos.Gameplay.User;

public class DebugManager : MonoBehaviour
{
    [SerializeField] private GameObject debugPanel = null;
    [SerializeField] private GameObject pausePanel = null;
    [SerializeField] private GameObject text = null;

    private bool active = false;

    public void RestartGame()
    {
        if (pausePanel.activeSelf)
        {
            pausePanel.SetActive(false);
        }
        GameplayManager.Instance.StartEnding();
        GameplayManager.Instance.ResetGame();
    }

    public void SlowTime()
    {
        StartCoroutine( FindObjectOfType<TimeManager>().timeSlow(.2f));
    }

    public void MakeShake()
    {
        StartCoroutine (Camera.main.GetComponent<CameraShake>().Shake(.15f, .4f));
    }

    public void PlayerInvencible()
    {
        FindObjectOfType<Player>().SetInmuneForTime(9000);
    }

    public void PlayerRemoveInvulneravility()
    {
        FindObjectOfType<Player>().SetInmuneForTime(0);
    }

    public void PlayerAddLife()
    {
        FindObjectOfType<Player>().AddLife();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.T))
        {
            active = !active;
            debugPanel.SetActive(active);
            text.SetActive(!active);
        }
    }
}
