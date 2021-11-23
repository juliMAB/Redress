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

        [SerializeField] AK.Wwise.Event soundGamePlay;
        [SerializeField] AK.Wwise.Event soundGameStop;
        [SerializeField] AK.Wwise.Event soundMenuPlay;
        [SerializeField] AK.Wwise.Event soundMenuStop;
        [SerializeField] AK.Wwise.Event soundPauseStop;

        [SerializeField] [Range(0, 1)] private float fxVolumen = 0;
        [SerializeField] [Range(0, 1)] private float musicVolumen = 0;

        public enum Scene { Menu, Tutorial, Game }

        private PlayerData playerData = null;

        private void Start()
        {
            soundMenuPlay.Post(gameObject);
        }
        public PlayerData PlayerData { get => playerData; }
        public float FxVolumen { get => fxVolumen; set => fxVolumen = value; }
        public float MusicVolumen { get => musicVolumen; set => musicVolumen = value; }

        public void GoToScene(Scene scene)
        {
            string stringSceneName;
            soundGameStop.Post(gameObject);
            soundMenuStop.Post(gameObject);
            soundPauseStop.Post(gameObject);
            switch (scene)
            {
                case Scene.Menu:
                    stringSceneName = "MainMenu";
                    soundMenuPlay.Post(gameObject);
                    break;
                case Scene.Tutorial:
                    stringSceneName = "Tutorial";
                    soundMenuPlay.Post(gameObject);
                    break;
                case Scene.Game:
                    stringSceneName = "Game";
                    soundGamePlay.Post(gameObject);
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
                soundGameStop.Post(gameObject);
                soundPauseStop.Post(gameObject);
                soundGamePlay.Post(gameObject);
            };
        }
    }
}
