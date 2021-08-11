using UnityEngine;
using UnityEngine.SceneManagement;

namespace TSDV_Final.Management
{
    public class GameManager : MonoBehaviour
    {
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

        public enum Scene { Menu, Game, ResultScreen }

        public void GoToScene(Scene scene)
        {
            string stringSceneName;

            switch (scene)
            {
                case Scene.Menu:
                    stringSceneName = "Menu";
                    break;
                case Scene.Game:
                    stringSceneName = "Game";
                    break;
                case Scene.ResultScreen:
                    stringSceneName = "ResultScreen";
                    break;
                default:
                    stringSceneName = "Menu";
                    break;
            }

            SceneManager.LoadScene(stringSceneName);

            if (scene == Scene.ResultScreen)
            {
                //*SetPlayerData();
            }
        }

        public void CloseGame()
        {
            Application.Quit();
        }
    }
}
