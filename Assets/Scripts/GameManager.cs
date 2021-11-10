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

        private void Start()
        {
            Invoke(nameof(addReferencesGpM), .1f);
        }
        public enum Scene { Menu, Tutorial, Game, ResultScreen }

        private PlayerData playerData = null;

        public PlayerData PlayerData { get => playerData; }

        public void GoToScene(Scene scene)
        {
            SceneManager.LoadScene((int)scene);
            if (scene == Scene.Game)
            {
                Invoke(nameof(addReferencesGpM), 1);
            }

        public void GoToMenu()
        {
            GoToScene(Scene.Menu);
        }

        public void CloseGame()
        {
            Application.Quit();
        }

        private void SetPlayerData()
        {
            GameplayManager gameplayManager = GameplayManager.Instance;

            int score = gameplayManager.Score;
            float velocity = gameplayManager.PlatformsManager.Speed;
            float traveledDistance = gameplayManager.Distance;

            playerData = new PlayerData(score, velocity, traveledDistance);
        }
        private void addReferencesGpM()
        {
            
            if (GameplayManager.Instance)
            {
                Debug.Log("existe la referencia de la instancia del GpM");
                GameplayManager.Instance.OnSelectScene = GoToScene;
            }
            else
            {
                Debug.Log("no existe instancia de GpM");
            }
            
        }
    }
}
