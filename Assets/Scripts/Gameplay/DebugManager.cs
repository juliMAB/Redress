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

    private Vector2 initialPlayerPosToLose = Vector2.zero;

    private bool active = false;
    private bool slowTimeOn = false;

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
        if (!slowTimeOn)
        {
            GameplayManager.Instance.speedMultiplier = 0.05f;
        }
        else
        {
            GameplayManager.Instance.speedMultiplier = 1f;
        }

        slowTimeOn = !slowTimeOn;
    }

    public void MakeShake()
    {
        StartCoroutine (Camera.main.GetComponent<CameraShake>().Shake(.15f, .4f));
    }

    public void PlayerInvencible()
    {
        initialPlayerPosToLose = GameplayManager.Instance.PlayerPosToLose;
        GameplayManager.Instance.SetYPlayerPosToLose(new Vector2(-1000, -1000));
        FindObjectOfType<Player>().SetInmuneForTime(9000, Color.red, false);
    }

    public void PlayerRemoveInvulneravility()
    {
        FindObjectOfType<Player>().SetInmuneForTime(0, Color.red, false);
        GameplayManager.Instance.SetYPlayerPosToLose(initialPlayerPosToLose);
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
