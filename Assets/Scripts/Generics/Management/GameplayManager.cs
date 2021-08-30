using System;
using UnityEngine;

using EndlessT4cos.Gameplay.Objects.Enemies;
using EndlessT4cos.Gameplay.User;
using EndlessT4cos.Gameplay.Platforms;
using Games.Generics.Character.Movement;
using EndlessT4cos.Gameplay.Background;
using Games.Generics.Weapon;
using Games.Generics.Manager;
using EndlessT4cos.Gameplay.Objects.PickUps;

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
        [SerializeField] private float speedProgressionMultiplier = 0.02f;
        [SerializeField] private float distanceProgressionMultiplier = 0.1f;
        [SerializeField] private float bulletSpeedMultiplier = 2;

        [Header("Entities")]
        [SerializeField] private Player player = null;
        [SerializeField] private CharacterMovementSeter playerControl = null;
        [SerializeField] private PlatformsManager platformsManager = null;
        [SerializeField] private BackgroundsManager[] backgroundsManager = null;
        [SerializeField] private BackgroundChanger backgroundChanger = null;
        [SerializeField] private PlatformObjectsManager objectsManager = null;
        [SerializeField] private PauseManager pauseManager = null;

        [Header("Enemies")]
        [SerializeField] private GameObject target = null;

        public Action<int> OnChangedScore = null;
        public Action OnGameplayEnded = null;
        public Action<int> OnNextState = null;

        public int Score { get => score; set => score = value; }
        public float Distance { get => distance; }
        public Player Player { get => player; }
        public PlatformsManager PlatformsManager { get => platformsManager; }
        
        private void Start()
        {
            Enemy enemy;

            for (int i = 0; i < objectsManager.Enemies.Length; i++)
            {
                enemy = objectsManager.Enemies[i].GetComponent<Enemy>();
                enemy.OnDie += AddScore;
            }

            AssignEnemiesTypes();
            AssignActionsAndTarget();
            AssignPlayerAndActionToPickUp();

            OnNextState += backgroundChanger.UpdateSprite;

            SetPlatformObjectsManagerValues(objectsManager, initialSpeed, initialMinSpawnTime, initialMaxSpawnTime);
            SetPlatformsManagerValues(speed, initialMinSpawnDistance, initialMaxSpawnDistance);
            SetBulletsSpeed(speed * bulletSpeedMultiplier);
        }

        private void Update()
        {
            if (pauseManager.GameIsPaused)
            { 
                return; 
            }

            distance += platformsManager.speed / 50f;

            if ((int)distance % distanceToNextState == 0 && (int)distance != 0)
            {
                //backgroundChanger.UpdateSprite((int)distance / distanceToNextState);
            }

            if (!IsPlayerAlive())
            {
                player.Die();
                StartEnding();
            }

            SetLevelProgression();
        }

        private void AddScore(GameObject go)
        {
            score += scorePerKill;
            OnChangedScore?.Invoke(score);
            
        }

        public void ChangePause()
        {
            if (pauseManager.GameIsPaused)
                pauseManager.Resume();
            else
                pauseManager.Pause();
        }

        public void EndGameplay()
        {
            OnGameplayEnded?.Invoke();
        }

        public void StartEnding()
        {
            //quitarle el control al player.
            playerControl.ControlActive = false;
            player.ControlActive = false;

            //frenar las plataformas y enemigos.
            speed = 0;

            //frenar los enemigos.
            foreach (var background in backgroundsManager)
            {
                background.enabled = false;
            }

            //pausar los managers.           
            objectsManager.enabled = false;
            platformsManager.enabled = false;

            pauseManager.Pause();
        }

        public void ResetGame()
        {
            score = 0;
            OnChangedScore?.Invoke(score);

            distance = 0;
            player.Reset();
            playerControl.ControlActive = true;
            player.ControlActive = true;

            speed = initialSpeed;

            foreach (var background in backgroundsManager)
            {
                background.enabled = true;
            }

            foreach (var platfomrObject in objectsManager.Objects)
            {
                platfomrObject.gameObject.SetActive(false);
            }

            objectsManager.enabled = true;
            platformsManager.enabled = true;

            SetPlatformObjectsManagerValues(objectsManager, speed, initialMinSpawnTime, initialMaxSpawnTime);
            SetPlatformsManagerValues(speed, initialMinSpawnDistance, initialMaxSpawnDistance);
            SetBulletsSpeed(speed * bulletSpeedMultiplier);

            platformsManager.Reset();

            pauseManager.Resume();
        }

        private void SetPlatformObjectsManagerValues(PlatformObjectsManager platformObjectsManager, float speed, float minSpawnTime, float maxSpawnTime)
        {
            platformObjectsManager.speed = speed;
            platformObjectsManager.minSpawnTime = minSpawnTime;
            platformObjectsManager.maxSpawnTime = maxSpawnTime;
        }

        private void SetPlatformsManagerValues(float speed, float minDistance, float maxDistance)
        {
            platformsManager.speed = speed;
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

            SetPlatformObjectsManagerValues(objectsManager, speed, objectsManager.minSpawnTime, objectsManager.maxSpawnTime);
            SetPlatformsManagerValues(speed, platformsManager.minDistance + distanceProgression, platformsManager.maxDistance + distanceProgression);
            SetBulletsSpeed(speed * bulletSpeedMultiplier);
        }

        private void SetBulletsSpeed(float speed)
        {
            Gun[] allGuns = FindObjectsOfType<Gun>();

            for (int i = 0; i < allGuns.Length; i++)
            {
                allGuns[i].bulletSpeed = speed;
            }
        }

        #region Enemies_Related_Functions
        private void AssignEnemiesTypes()
        {
            Enemy enemy;
            Objects.Enemies.Type type;

            for (int i = 0; i < objectsManager.Enemies.Length; i++)
            {
                enemy = objectsManager.Enemies[i];

                type = Objects.Enemies.Type.Static;

                if (enemy.TryGetComponent(out ExplosiveEnemy explosiveEnemy))
                {
                    type = Objects.Enemies.Type.Explosive;
                }
                else if (enemy.TryGetComponent(out ShooterEnemy shooterEnemy))
                {
                    type = Objects.Enemies.Type.Shooter;
                }

                enemy.type = type;
            }
        }

        private void AssignActionsAndTarget()
        {
            Enemy enemy;
            ExplosiveEnemy explosiveEnemy;
            ShooterEnemy shooterEnemy;

            for (int i = 0; i < objectsManager.Enemies.Length; i++)
            {
                enemy = objectsManager.Enemies[i];

                switch (enemy.type)
                {
                    case Objects.Enemies.Type.Static:
                        break;
                    case Objects.Enemies.Type.Explosive:
                        explosiveEnemy = enemy.GetComponent<ExplosiveEnemy>();
                        break;
                    case Objects.Enemies.Type.Shooter:
                        shooterEnemy = enemy.GetComponent<ShooterEnemy>();
                        break;
                    default:
                        break;
                }

                enemy.OnDie += objectsManager.DeactivateObject;
                enemy.SetTarget(target);
            }
        }
        #endregion

        #region PickUps_Related_Functions
        private void AssignPlayerAndActionToPickUp()
        {
            PickUp pickUp;

            for (int i = 0; i < objectsManager.PickUps.Length; i++)
            {
                pickUp = objectsManager.PickUps[i];
                pickUp.Player = player;

                pickUp.OnConsumed += objectsManager.DeactivateObject;
            }
        }
        #endregion
    }
}