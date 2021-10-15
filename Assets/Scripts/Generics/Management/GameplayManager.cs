using System;
using System.Collections;
using UnityEngine;

using EndlessT4cos.Gameplay.Objects.Enemies;
using EndlessT4cos.Gameplay.User;
using EndlessT4cos.Gameplay.Platforms;
using Games.Generics.Character.Movement;
using Games.Generics.Weapon;
using Games.Generics.Manager;
using EndlessT4cos.Gameplay.Objects.PickUps;
using EndlessT4cos.Gameplay.Controllers;

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

        private IEnumerator setDistanceScoreInst = null;

        [Header("Initial values")]
        [SerializeField] private float initialSpeed = 5;
        [SerializeField] private float initialIayerSpeedDiff = 0.1f;
        [SerializeField] private float initialMinSpawnTime = 1;
        [SerializeField] private float initialMaxSpawnTime = 2;
        [SerializeField] private float initialMinSpawnDistance = 1;
        [SerializeField] private float initialMaxSpawnDistance = 2;

        [Header("Global variables")]
        [SerializeField] private float speed = 5f;
        [SerializeField] private int score = 0;
        [SerializeField] private float timeToChargeScore = 1;
        [SerializeField] private float distance = 0;
        [SerializeField] private int scorePerKill = 0;

        [Header("Gameplay configuration")]
        [SerializeField] private int distanceToNextState = 1000;
        [SerializeField] private Vector2 playerPosToLose = Vector2.zero;
        [SerializeField] private float speedProgressionMultiplier = 0.02f;
        [SerializeField] private float distanceProgressionMultiplier = 0.1f;
        [SerializeField] private float bulletSpeedMultiplier = 2;
        [SerializeField] private float speedDivider = 40f; // Make little to speed up the general speed more rapidly.
        //[SerializeField] private int[] scorePerLevel = null;
        [SerializeField] private int actualLvl = 0;
        [SerializeField] private float layerSpeedDiff = 0.1f;

        [Header("Entities")]
        [SerializeField] private Player player = null;
        [SerializeField] private CharacterMovementSeter playerControl = null;
        [SerializeField] private PlatformsManager platformsManager = null;
        [SerializeField] private PlatformObjectsManager objectsManager = null;
        [SerializeField] private PauseManager pauseManager = null;
        [SerializeField] private CameraShake cameraShake = null;
        [SerializeField] private ParallaxManager background = null;
        [SerializeField] private CameraController cameraController = null;

        [Header("Enemies")]
        [SerializeField] private GameObject target = null;

        [Header("Objects")]
        [SerializeField] private Gun[] allGuns = null;
        [SerializeField] private Bullet[] allBullets = null;

        public float speedMultiplier = 1f;

        public Action<int> OnChangedScore = null;
        public Action OnGameplayEnded = null;
        public Action<int> OnNextState = null;

        public int Score { get => score; set => score = value; }
        public float Distance => distance;
        public Vector2 PlayerPosToLose => playerPosToLose;
        public Player Player => player;
        public PlatformsManager PlatformsManager => platformsManager;

        private void Start()
        {
            //pplication.targetFrameRate = 30;
            playerPosToLose.y = -5;
            playerPosToLose.x = -8.8f;

            AssignEnemiesTypes();
            AssignActionsAndTarget();
            AssignPlayerAndActionToPickUp();

            SetPlatformObjectsManagerValues(objectsManager, initialSpeed, initialMinSpawnTime, initialMaxSpawnTime);
            SetPlatformsManagerValues(speed, initialMinSpawnDistance, initialMaxSpawnDistance);
            SetBulletsSpeed(speed * bulletSpeedMultiplier, true);
            background.SetSpeed(speed, layerSpeedDiff);

            platformsManager.OnUnneveness += cameraController.MoveCamera;
        }

        private void Update()
        {
            if (pauseManager.GameIsPaused)
            { 
                return; 
            }

            distance += platformsManager.speed / speedDivider;

            if (setDistanceScoreInst == null)
            {
                setDistanceScoreInst = SetDistanceScore(timeToChargeScore);
                StartCoroutine(setDistanceScoreInst);
                timeToChargeScore -= 0.0001f;
            }            

            if (!IsPlayerAlive())
            {
                player.Die();
                StartEnding();
            }
            else
            {
                player.PlayerUpdate();
                objectsManager.PlatformObjectsManagerUpdate();
                platformsManager.PlatformsUpdate();
                background.UpdateBackground();
            }

            SetPlayerInputLock();
            SetLevelProgression();
        }

        private void FixedUpdate()
        {
            playerControl.CharacterMovementSeterUpdate();
        }

        public void SetYPlayerPosToLose(Vector2 pos)
        {
            playerPosToLose = pos;
        }

        void CrazyFunc()
        {
            StartCoroutine(cameraShake.Shake(.15f, .4f));
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
            if (Time.timeScale == 0)
            {
                Time.timeScale = 1;
            }

            OnGameplayEnded?.Invoke();
        }

        public void StartEnding()
        {
            //quitarle el control al player.
            playerControl.ControlActive = false;
            player.ControlActive = false;

            //frenar las plataformas y enemigos.
            speed = 0;
            layerSpeedDiff = 0;
            background.SetSpeed(0, 0);

            //pausar los managers.           
            objectsManager.enabled = false;
            platformsManager.enabled = false;
            SetBulletsSpeed(0, true);

            pauseManager.Pause();
        }

        public void ResetGame()
        {
           // backgroundChanger.MyReset();
            speedMultiplier = 1f;
            score = 0;
            OnChangedScore?.Invoke(score);

            distance = 0;
            player.Reset();
            playerControl.ControlActive = true;
            player.ControlActive = true;

            speed = initialSpeed;
            layerSpeedDiff = initialIayerSpeedDiff;

            foreach (var platfomrObject in objectsManager.Objects)
            {
                platfomrObject.gameObject.SetActive(false);
            }

            objectsManager.enabled = true;
            platformsManager.enabled = true;

            DeactivateAllBullets();

            SetPlatformObjectsManagerValues(objectsManager, speed, initialMinSpawnTime, initialMaxSpawnTime);
            SetPlatformsManagerValues(speed, initialMinSpawnDistance, initialMaxSpawnDistance);
            SetBulletsSpeed(speed * bulletSpeedMultiplier, true);
            background.SetSpeed(initialSpeed, layerSpeedDiff);

            platformsManager.Reset();
            background.Reset();
            cameraController.Reset();

            pauseManager.Resume();
        }

        private void SetPlayerInputLock()
        {
            playerControl.lockGoDown = player.transform.position.y < platformsManager.YSpawnPositions[1];
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
            platformsManager.distanceLimits[0] = minDistance;
            platformsManager.distanceLimits[1] = maxDistance;
        }

        private bool IsPlayerAlive()
        {
            return !Input.GetKey(KeyCode.Keypad9) && 
                    player.transform.position.y - player.transform.lossyScale.y / 2 > Camera.main.transform.position.y + playerPosToLose.y &&
                    player.transform.position.x + player.transform.lossyScale.x / 2 > playerPosToLose.x &&
                    player.Lives > 0;
        }

        private void SetLevelProgression()
        {
            float speedProgression = Time.deltaTime * speedProgressionMultiplier * speedMultiplier;
            float distanceProgression = Time.deltaTime * distanceProgressionMultiplier * speedMultiplier;

            speed += speedProgression;

            objectsManager.minSpawnTime = initialMinSpawnTime;
            objectsManager.maxSpawnTime = initialMaxSpawnTime;

            if (speedMultiplier < 1)
            {
                objectsManager.minSpawnTime = initialMinSpawnTime * 2;
                objectsManager.maxSpawnTime = initialMaxSpawnTime * 2;
            }

            SetPlatformObjectsManagerValues(objectsManager, speed * speedMultiplier, objectsManager.minSpawnTime, objectsManager.maxSpawnTime);
            SetPlatformsManagerValues(speed * speedMultiplier, platformsManager.distanceLimits[0] + distanceProgression, platformsManager.distanceLimits[1] + distanceProgression);
            SetBulletsSpeed(speed * bulletSpeedMultiplier * speedMultiplier, speedMultiplier + Mathf.Epsilon > 1f);
            background.SetSpeed(initialSpeed * speedMultiplier, layerSpeedDiff * speedMultiplier);
        }
        private void SetBulletsSpeed(float speed, bool playerBulletsToo)
        {
            for (int i = 0; i < allGuns.Length; i++)
            {
                if (allGuns[i] != player.Gun)
                {
                    allGuns[i].bulletSpeed = speed;
                }
            }

            int index = playerBulletsToo ? 0 : player.Gun.Objects.Length;

            for (int i = index; i < allBullets.Length; i++)
            {
                allBullets[i].speed = speed;
            }
        }

        private void DeactivateAllBullets()
        {
            for (int i = 0; i < allGuns.Length; i++)
            {
                allGuns[i].DeactivateAllBullets();
            }
        }

        private IEnumerator SetDistanceScore(float timeToChargeScore)
        {
            yield return new WaitForSeconds(timeToChargeScore);

            score += (int)platformsManager.speed;
            OnChangedScore?.Invoke(score);

            setDistanceScoreInst = null;
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
                        enemy.OnDie += AddScore;
                        break;
                    case Objects.Enemies.Type.Explosive:
                        explosiveEnemy = enemy.GetComponent<ExplosiveEnemy>();
                        explosiveEnemy.OnExplode += CrazyFunc;
                        break;
                    case Objects.Enemies.Type.Shooter:
                        shooterEnemy = enemy.GetComponent<ShooterEnemy>();
                        enemy.OnDie += AddScore;
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