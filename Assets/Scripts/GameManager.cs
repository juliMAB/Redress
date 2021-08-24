using UnityEngine;
using UnityEngine.SceneManagement;

using EndlessT4cos.Gameplay.User;
using EndlessT4cos.Gameplay.Management;

namespace EndlessT4cos.Management
{
    public class GameManager : MonoBehaviour
    {
        #region Singleton
        private static GameManager instance = null;
        public static GameManager Instance { get => instance; }
        
        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }
        #endregion

        public enum Scene { Menu, Game, ResultScreen }

        private PlayerData playerData = null;

        public PlayerData PlayerData { get => playerData; }

        public void GoToScene(Scene scene)
        {
            string stringSceneName;

            switch (scene)
            {
                case Scene.Menu:
                    stringSceneName = "MainMenu";
                    break;
                case Scene.Game:
                    stringSceneName = "Game";
                    break;
                case Scene.ResultScreen:
                    { 
                        stringSceneName = "ResultScreen";
                        SetPlayerData();
                    }
                    break;
                default:
                    stringSceneName = "MainMenu";
                    break;
            }

            SceneManager.LoadScene(stringSceneName);

            if (scene == Scene.Game)
            {
                Invoke("SetGameplayReturnToMenu", 1f);
            }
        }

        private void GoToMenu()
        {
            GoToScene(Scene.Menu);
        }

        public void SetGameplayReturnToMenu()
        {
            GameplayManager.Instance.OnGameplayEnded += GoToMenu;
        }

        public void CloseGame()
        {
            Application.Quit();
        }

        private void SetPlayerData()
        {
            GameplayManager gameplayManager = GameplayManager.Instance;

            int score = gameplayManager.Score;
            float velocity = gameplayManager.PlatformsManager.speed;
            float traveledDistance = gameplayManager.Distance;

            playerData = new PlayerData(score, velocity, traveledDistance);
        }
    }
}
