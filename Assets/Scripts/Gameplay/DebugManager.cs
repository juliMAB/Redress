using UnityEngine;

using Redress.Gameplay.Management;
using Redress.Gameplay.User;

public class DebugManager : MonoBehaviour
{
    [SerializeField] private GameObject debugPanel = null;
    [SerializeField] private GameObject pausePanel = null;
    [SerializeField] private GameObject text = null;

    private Vector2 initialPlayerPosToLose = Vector2.zero;

    private bool active = false;
    private bool slowTimeOn = false;
    private bool playerInvencible = false;

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
            GameplayManager.Instance.speedMultiplier = 0.3f;
        }
        else
        {
            GameplayManager.Instance.speedMultiplier = 1f;
        }

        slowTimeOn = !slowTimeOn;
    }

    public void PlayerInvencible()
    {
        initialPlayerPosToLose = GameplayManager.Instance.PlayerPosToLose;
        GameplayManager.Instance.SetYPlayerPosToLose(new Vector2(-1000, -1000));
        FindObjectOfType<Player>().SetInmuneForTime(9000, Color.red);
        playerInvencible = true;
    }

    public void PlayerRemoveInvulneravility()
    {
        FindObjectOfType<Player>().SetInmuneForTime(0, Color.red);
        GameplayManager.Instance.SetYPlayerPosToLose(initialPlayerPosToLose);
        playerInvencible = false;
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

        if (playerInvencible)
        {
            FindObjectOfType<Player>().transform.position = new Vector3(-5, 5, FindObjectOfType<Player>().transform.position.y); 
        }
    }
}
