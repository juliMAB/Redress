using UnityEngine;
using UnityEngine.SceneManagement;

using Redress.Gameplay.User;
using Redress.Gameplay.Management;

namespace Redress.Management
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

        public enum Scene { Menu, Tutorial, Game }

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
                case Scene.Tutorial:
                    stringSceneName = "Tutorial";
                    break;
                case Scene.Game:
                    stringSceneName = "Game";
                    break;
                default:
                    stringSceneName = "MainMenu";
                    break;
            }

            SceneManager.LoadScene(stringSceneName);

            if (scene == Scene.Game)
            {
                Invoke("SetGameplayReturnToMenu", 0.2f);
                Invoke("SetResetGameplayActions", 0.2f);
            }
        }

        public void GoToMenu()
        {
            GoToScene(Scene.Menu);
        }

        public void CloseGame()
        {
            Application.Quit();
        }

        private void SetGameplayReturnToMenu()
        {
            GameplayManager.Instance.OnGameplayEnded = GoToMenu;
        }

        private void SetResetGameplayActions()
        {
            GameplayManager.Instance.OnResetGame = () => 
            { 
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); 
                Invoke("SetGameplayReturnToMenu", 0.2f);
                Invoke("SetResetGameplayActions", 0.2f);
            };
        }
    }
}
