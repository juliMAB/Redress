using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using EndlessT4cos.Gameplay.Enemies;
using EndlessT4cos.Gameplay.User;
using EndlessT4cos.Gameplay.Platforms;
using Games.Generic.Character.Movement;
using EndlessT4cos.Gameplay.Background;
using UnityEngine.Events;

namespace EndlessT4cos.Gameplay.Management
{
    public class GameplayManager : MonoBehaviour
    {
        private static GameplayManager instance = null;
        public static GameplayManager Instance { get => instance; }

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                instance = this;
            }
        }
        [SerializeField] private int velocity = 0;
        [SerializeField] private int spawnTime = 0;
        [SerializeField] private int score = 0;
        [SerializeField] private float distance = 0;
        [SerializeField] private int scorePerKill = 0;
        [SerializeField] private int distanceToNextState = 1000;
        [SerializeField] private EnemiesManager enemiesManager = null;
        [SerializeField] private Player player = null;
        [SerializeField] private CharacterMovementSeter playerControl = null;
        [SerializeField] private PlatformsManager platformsManager = null;
        [SerializeField] private BackgroundsManager[] backgroundsManager = null;
        [SerializeField] private BackgroundChanger backgroundChanger = null;
        [SerializeField] private float yPlayerPosToLose = -5f;


        public Action<int> OnChangedScore = null;
        public Action OnGameplayEnded = null;
        public Action<int> OnNextState = null;
        public int Score { get => score; set => score = value; }
        public float Distance { get => distance; }
        public EnemiesManager EnemiesManager { get => enemiesManager; }
        public Player Player { get => player; }
        public PlatformsManager PlatformsManager { get => platformsManager; }
        
        private void Start()
        {
            Enemy enemy;

            for (int i = 0; i < enemiesManager.Objects.Length; i++)
            {
                enemy = enemiesManager.Objects[i].GetComponent<Enemy>();
                enemy.OnDie += AddScore;
            }

            OnGameplayEnded += StartEnding;
            OnNextState += backgroundChanger.updateSprite;
        }

        private void Update()
        {
            distance += platformsManager.Speed / 50;
            if ((int)distance% distanceToNextState == 0)
            {
                Debug.Log("LLEGA AL MANAGER.");
                backgroundChanger.updateSprite((int)distance / distanceToNextState);
            }
            if (player.transform.position.y - player.transform.lossyScale.y / 2 < yPlayerPosToLose)
            {
                player.OnDie?.Invoke();
                EndGameplay();
            }
            if (Input.GetKey(KeyCode.Keypad9))
            {
                player.OnDie?.Invoke();
                EndGameplay();
            }
        }

        private void AddScore(GameObject go)
        {
            score += scorePerKill;
            OnChangedScore?.Invoke(score);
            
        }

        private void EndGameplay()
        {
            OnGameplayEnded?.Invoke();
        }

        private void StartEnding()
        {
            //quitarle el control al player.
            playerControl.ControlActive = false;
            //frenar las plataformas.
            platformsManager.Speed = 0;
            //frenar los enemigos.
            foreach (var item in backgroundsManager)
            {
                item.enabled = false;
            }

            enemiesManager.Speed = 0;
            //mostar el retry.
            //pausar los managers.
           
            enemiesManager.enabled = false;
        }

        public void ResetGame()
        {
            score = 0;
            distance = 0;
            player.Reset();
            playerControl.ControlActive = true;

            foreach (var item in backgroundsManager)
            {
                item.enabled = true;
            }

            foreach (var item in enemiesManager.Objects)
            {
                item.SetActive(false);
            }

            enemiesManager.enabled = true;
            platformsManager.Speed = 5;
            enemiesManager.Speed = 5;
        }
    }
}