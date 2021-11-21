using UnityEngine;

namespace GuilleUtils.Manager
{
    public class PauseManager : MonoBehaviour
    {
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
        }

        public void Pause()
        {
            Time.timeScale = 0f;
            gameIsPaused = true;
        }
    }
}

