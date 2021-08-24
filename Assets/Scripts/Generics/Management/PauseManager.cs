using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Games.Generics.Manager
{
    public class PauseManager : MonoBehaviour
    {
        bool GameIsPaused=false;
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                if (GameIsPaused)
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
            GameIsPaused = false;
        }
        public void Pause()
        {
            Time.timeScale = 0f;
            GameIsPaused = true;
        }
    }
}

