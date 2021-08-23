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
        #region Singleton
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
        #endregion

        [Header("Initial values")]
        [SerializeField] private float initialSpeed = 5;
        [SerializeField] private float initialMinSpawnTime = 1;
        [SerializeField] private float initialMaxSpawnTime = 2;
        [SerializeField] private float initialMinSpawnDistance = 1;
        [SerializeField] private float initialMaxSpawnDistance = 2;

        [Header("Global variables")]
        [SerializeField] private float speed = 5f;
        [SerializeField] private int score = 0;
        [SerializeField] private float distance = 0;
        [SerializeField] private int scorePerKill = 0;

        [Header("Gameplay configuration")]
        [SerializeField] private int distanceToNextState = 1000;
        [SerializeField] private float yPlayerPosToLose = -5f;
        [SerializeField] private float speedProgressionMultiplier = 0.1f;
        [SerializeField] private float distanceProgressionMultiplier = 0.1f;

        [Header("Entities")]
        [SerializeField] private EnemiesManager enemiesManager = null;
        [SerializeField] private Player player = null;
        [SerializeField] private CharacterMovementSeter playerControl = null;
        [SerializeField] private PlatformsManager platformsManager = null;
        [SerializeField] private BackgroundsManager[] backgroundsManager = null;
        [SerializeField] private BackgroundChanger backgroundChanger = null;

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
            player.OnDie += EndGameplay;
            Enemy enemy;

            for (int i = 0; i < enemiesManager.Objects.Length; i++)
            {
                enemy = enemiesManager.Objects[i].GetComponent<Enemy>();
                enemy.OnDie += AddScore;
            }

            OnGameplayEnded += StartEnding;
            OnNextState += backgroundChanger.UpdateSprite;

            SetPlatformObjectsManagerValues(enemiesManager, initialSpeed, initialMinSpawnTime, initialMaxSpawnTime);
            SetPlatformObjectsManagerValues(platformsManager, initialSpeed, initialMinSpawnTime, initialMaxSpawnTime);
            SetPlatformsManagerValues(initialMinSpawnDistance, initialMaxSpawnDistance);
        }

        private void Update()
        {
            distance += platformsManager.speed / 50;

            if ((int)distance % distanceToNextState == 0 && (int)distance != 0)
            {
                backgroundChanger.UpdateSprite((int)distance / distanceToNextState);
            }

            if (!IsPlayerAlive())
            {
                player.Die();
            }

            SetLevelProgression();
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

            //frenar las plataformas y enemigos.
            speed = 0;

            //frenar los enemigos.
            foreach (var background in backgroundsManager)
            {
                background.enabled = false;
            }

            //pausar los managers.           
            enemiesManager.enabled = false;
            platformsManager.enabled = false;
        }

        public void ResetGame()
        {
            score = 0;
            distance = 0;
            player.Reset();
            playerControl.ControlActive = true;

            speed = initialSpeed;

            foreach (var background in backgroundsManager)
            {
                background.enabled = true;
            }

            foreach (var enemy in enemiesManager.Objects)
            {
                enemy.SetActive(false);
            }

            enemiesManager.enabled = true;
            platformsManager.enabled = true;

            SetPlatformObjectsManagerValues(enemiesManager, speed, initialMinSpawnTime, initialMaxSpawnTime);
            SetPlatformObjectsManagerValues(platformsManager, speed, initialMinSpawnTime, initialMaxSpawnTime);
            SetPlatformsManagerValues(initialMinSpawnDistance, initialMaxSpawnDistance);

            platformsManager.Reset();
        }

        private void SetPlatformObjectsManagerValues(PlatformObjectsManager platformObjectsManager, float speed, float minSpawnTime, float maxSpawnTime)
        {
            platformObjectsManager.speed = speed;
            platformObjectsManager.minSpawnTime = minSpawnTime;
            platformObjectsManager.maxSpawnTime = maxSpawnTime;
        }

        private void SetPlatformsManagerValues(float minDistance, float maxDistance)
        {
            platformsManager.minDistance = minDistance;
            platformsManager.maxDistance = maxDistance;
        }

        private bool IsPlayerAlive()
        {
            return !Input.GetKey(KeyCode.Keypad9) && player.transform.position.y - player.transform.lossyScale.y / 2 > yPlayerPosToLose;
        }

        private void SetLevelProgression()
        {
            float speedProgression = Time.deltaTime * speedProgressionMultiplier;
            float distanceProgression = Time.deltaTime * distanceProgressionMultiplier;

            speed += speedProgression;

            SetPlatformObjectsManagerValues(enemiesManager, speed, enemiesManager.minSpawnTime, enemiesManager.maxSpawnTime);
            SetPlatformObjectsManagerValues(platformsManager, speed, platformsManager.minSpawnTime, platformsManager.maxSpawnTime);
            SetPlatformsManagerValues(platformsManager.minDistance + distanceProgression, platformsManager.maxDistance + distanceProgression);
        }
    }
}