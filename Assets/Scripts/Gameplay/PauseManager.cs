using UnityEngine;
using Redress.Management;

namespace GuilleUtils.Manager
{
    public class PauseManager : MonoBehaviour
    {

        [SerializeField] AK.Wwise.Event soundPausePlay;
        [SerializeField] AK.Wwise.Event soundPauseStop;
        [SerializeField] AK.Wwise.Event soundGamePlay;
        [SerializeField] AK.Wwise.Event soundGameStop;

        private bool gameIsPaused = false;

        public bool GameIsPaused { get => gameIsPaused; }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                if (gameIsPaused)
                {
                    Resume();
                }
                else
                {
                    Pause();
                }
            }
        }

        public void Resume()
        {
            Time.timeScale = 1f;
            gameIsPaused = false;
            soundPauseStop.Post(GameManager.Instance.gameObject);
            soundGamePlay.Post(GameManager.Instance.gameObject);
        }

        public void Pause()
        {
            Time.timeScale = 0f;
            gameIsPaused = true;
            soundPausePlay.Post(GameManager.Instance.gameObject);
            soundGameStop.Post(GameManager.Instance.gameObject);
        }
    }
}

