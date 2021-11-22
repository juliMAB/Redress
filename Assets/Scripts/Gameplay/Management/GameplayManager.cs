using System;
using System.Collections.Generic;
using UnityEngine;

using Redress.Gameplay.Objects.Enemies;
using Redress.Gameplay.User;
using Redress.Gameplay.Platforms;
using GuilleUtils.Manager;
using Redress.Gameplay.Objects.PickUps;
using Redress.Gameplay.Controllers;
using Redress.Gameplay.Data;
using GuilleUtils.PoolSystem;
using GuilleUtils.Displacement;


namespace Redress.Gameplay.Management
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

        private float time = 0f;

        private PoolObjectsManager poolManager = null;

        //[Header("Initial values")]
        //[SerializeField] private float initialSpeed = 5;
        //[SerializeField] private float[] initialSpawnTimeLimits = null;
        //[SerializeField] private float[] initialSpawnDistanceLimits = null;

        [Header("Global variables")]
        //[SerializeField] private float speed = 5f;
        [SerializeField] private int score = 0;
        [SerializeField] private float timeToChargeScore = 1;
        //[SerializeField] private float distance = 0;
        [SerializeField] private int scorePerKill = 0;

        [Header("Gameplay configuration")]
        [SerializeField] private Vector2 playerPosToLose = Vector2.zero;
        //[SerializeField] private float bulletSpeedMultiplier = 2;
        //[SerializeField] private float speedDivider = 40f; // Make little to speed up the general speed more rapidly.
        //[SerializeField] private float layerSpeedDiff = 0.1f;
        [SerializeField] private float halfPlayerHeight = 0.88f;
        [SerializeField] private Vector2 halfSizeScreen = Vector2.zero;

        [Header("Entities")]
        [SerializeField] private Player player = null;
        //[SerializeField] private CharacterMovementSeter playerControl = null;
        [SerializeField] private PlatformsManager platformsManager = null;
        [SerializeField] private PlatformObjectsManager objectsManager = null;
        [SerializeField] private PauseManager pauseManager = null;
        //[SerializeField] private ParallaxManager background = null;
        [SerializeField] private CameraController cameraController = null;
        [SerializeField] private HighscoreManager highscoreManager = null;

        [Header("Managers")]
        [SerializeField] private LevelProgressionManager levelProgressionManager = null;

        //public float speedMultiplier = 1f;

        public Action<int> OnChangedScore = null;
        public Action OnGameplayEnded = null;
        public Action<int> OnNextState = null;
        public Action OnResetGame = null;

        //public int Score { get => score; set => score = value; }
        //public float Distance => distance;
        public Vector2 PlayerPosToLose => playerPosToLose;
        public Player Player => player;
        //public PlatformsManager PlatformsManager => platformsManager;

        private void Start()
        {
            poolManager = PoolObjectsManager.Instance;

            AssignActionsAndTargetToEnemies();
            AssignPlayerAndActionToPickUp();
            SetSlowMotionActions();

            levelProgressionManager.Initialize();

            //platformsManager.SetValues(speed, initialSpawnDistanceLimits[0], initialSpawnDistanceLimits[1], true);
            //objectsManager.SetValues(speed, initialSpawnDistanceLimits[0], initialSpawnDistanceLimits[1], true);

            //SetBulletsSpeed(speed * bulletSpeedMultiplier, true);
            //background.SetSpeed(speed, layerSpeedDiff);

            platformsManager.OnUnneveness += cameraController.PositionCamera;
        }

        private void Update()
        {
            if (pauseManager.GameIsPaused)
            {
                return;
            }

            //distance += platformsManager.Speed / speedDivider;

            SetScoreUpdate();

            if (!IsPlayerAlive())
            {
                player.Die();
                StartEnding();
            }
            else
            {
                player.PlayerUpdate();
                levelProgressionManager.SetLevelUpdate();

                //objectsManager.PlatformObjectsUpdate();
                //platformsManager.PlatformsUpdate();
                //background.UpdateBackground();

                SetPlayerInputLock();

                levelProgressionManager.SetLevelProgression();
                //SetLevelProgression();
            }
        }

        private void SetScoreUpdate()
        {
            time += Time.deltaTime;
            if (time > timeToChargeScore)
            {
                score += (int)platformsManager.Speed;
                OnChangedScore?.Invoke(score);
                timeToChargeScore -= 0.0001f;
                time = 0f;
            }
        }

        public void SetYPlayerPosToLose(Vector2 pos)
        {
            playerPosToLose = pos;
        }

        private void AddScore(GameObject go)
        {
            score += scorePerKill;
            OnChangedScore?.Invoke(score);            
        }

        public void ChangePause()
        {
            if (pauseManager.GameIsPaused)
            { 
                pauseManager.Resume();
            }
            else
            { 
                pauseManager.Pause();
            }

            //objectsManager.Pause(pauseManager.GameIsPaused);
            //platformsManager.PauseMovement(pauseManager.GameIsPaused);
            //cameraController.PauseCameraMovement(pauseManager.GameIsPaused);
            highscoreManager.SetScore(score);
        }

        public void EndGameplay()
        {
            if (Time.timeScale != 1)
            {
                Time.timeScale = 1;
            }

            OnGameplayEnded?.Invoke();
        }

        public void StartEnding()
        {
            //quitarle el control al player.
            player.CharacterMovement.ControlActive = false;
            player.ControlActive = false;

            //frenar las plataformas y enemigos.
            //speed = 0;
            //layerSpeedDiff = 0;
            //background.SetSpeed(0, 0);

            //pausar los managers.           
            //objectsManager.enabled = false;
            //platformsManager.enabled = false;
            //SetBulletsSpeed(0, true);

            //platformsManager.PauseMovement(true);
            //cameraController.PauseCameraMovement(true);
            pauseManager.Pause();
            highscoreManager.SetScore(score);
        }

        public void ResetGame()
        {
            Time.timeScale = 1f;
            OnResetGame?.Invoke();
        }

        public void SetSpeedMultiplier(float speed)
        {
            levelProgressionManager.SetSpeedMultiplier(speed);
            cameraController.speedMultiplier = speed;
        }

        private void SetPlayerInputLock()
        {
            RaycastHit2D hit = Physics2D.Raycast(player.transform.position, Vector2.down, 10, platformsManager.LayerMask);

            bool CheckPlatformUnder(MovableObject platformUnder, out RaycastHit2D hit2)
            {
                Vector2 pos = new Vector2(player.transform.position.x, platformUnder.transform.position.y - 1);

                hit2 = Physics2D.Raycast(pos, Vector2.down, 10, platformsManager.LayerMask);

                return hit2;
            }

            if (hit)
            {
                hit.collider.TryGetComponent(out MovableObject platformUnder);

                if (platformUnder)
                {
                    if (CheckPlatformUnder(platformUnder, out hit))
                    {
                        player.CharacterMovement.lockGoDown = false;
                    }
                    else
                    {
                        player.CharacterMovement.lockGoDown = platformUnder.row == Row.Down;
                    }
                }
                else
                {
                    player.CharacterMovement.lockGoDown = false;
                }

                return;
            }

            player.CharacterMovement.lockGoDown = false;
        }

        private bool IsPlayerAlive()
        {
            return !Input.GetKey(KeyCode.Keypad9) && 
                    player.transform.position.y - halfPlayerHeight > Camera.main.transform.position.y - halfSizeScreen.y  &&
                    player.transform.position.x + player.transform.lossyScale.x / 2 > playerPosToLose.x &&
                    player.Lives > 0;
        }

        //private void SetLevelProgression()
        //{
        //    float speedProgression = Time.deltaTime * speedProgressionMultiplier * speedMultiplier;
        //    float distanceProgression = Time.deltaTime * distanceProgressionMultiplier * speedMultiplier;
        //
        //    speed += speedProgression;
        //
        //    float minSpawnTime = initialSpawnTimeLimits[0];
        //    float maxSpawnTime = initialSpawnTimeLimits[1];
        //
        //    if (speedMultiplier < 1)
        //    {
        //        minSpawnTime *= 2;
        //        maxSpawnTime *= 2;
        //    }
        //
        //    objectsManager.SetValues(speed * speedMultiplier, minSpawnTime, maxSpawnTime, false);
        //    platformsManager.SetValues(speed * speedMultiplier, platformsManager.DistanceLimits[0] + distanceProgression, platformsManager.DistanceLimits[1] + distanceProgression, false);
        //
        //    SetBulletsSpeed(speed * bulletSpeedMultiplier * speedMultiplier, speedMultiplier + Mathf.Epsilon > 1f);
        //    background.SetSpeed(initialSpeed * speedMultiplier, layerSpeedDiff * speedMultiplier);
        //
        //    AssingCooldownToGuns();
        //}

        //private void SetBulletsSpeed(float speed, bool playerBulletsToo)
        //{
        //    Gun[] allGuns = FindObjectsOfType<Gun>();
        //    for (int i = 0; i < allGuns.Length; i++)
        //    {
        //        if (playerBulletsToo || allGuns[i] != player.Gun)
        //        {
        //            allGuns[i].bulletSpeed = speed;
        //        }
        //    }
        //
        //    for (int i = 0; i < poolManager.Bullets.objects.Length; i++)
        //    {
        //        poolManager.Bullets.objects[i].GetComponent<Bullet>().speed = speed;
        //    }
        //
        //    if (playerBulletsToo)
        //    {
        //        for (int i = 0; i < poolManager.Arrows.objects.Length; i++)
        //        {
        //            poolManager.Arrows.objects[i].GetComponent<Bullet>().speed = speed;
        //        }
        //    }
        //}

        private void AssignActionsAndTargetToEnemies()
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
                        explosiveEnemy.OnExplode += cameraController.ActivateCameraShake;
                        break;
                    case Objects.Enemies.Type.Shooter:
                        shooterEnemy = enemy.GetComponent<ShooterEnemy>();
                        enemy.OnDie += AddScore;
                        break;
                    default:
                        break;
                }

                enemy.OnDie += poolManager.DeactivateObject;
                enemy.SetTarget(player.gameObject);
            }
        }

        private void SetSlowMotionActions()
        {
            for (int i = 0; i < poolManager.PlatformObjects.objects.Length; i++)
            {
                GameObject go = poolManager.PlatformObjects.objects[i];

                if (go.TryGetComponent(out SlowMotion slowMotion))
                {
                    slowMotion.OnSpeedPercentageChanged = SetSpeedMultiplier;
                }
            }
        }

        //private void AssingCooldownToGuns()
        //{
        //    Gun[] guns = FindObjectsOfType<Gun>();
        //
        //    for (int i = 0; i < guns.Length; i++)
        //    {
        //        guns[i].coolDownMultiplier = 1 / speedMultiplier;
        //    }
        //}
        private void AssignPlayerAndActionToPickUp()
        {
            PickUp pickUp;

            for (int i = 0; i < objectsManager.PickUps.Length; i++)
            {
                pickUp = objectsManager.PickUps[i];
                pickUp.Player = player;

                pickUp.OnConsumed += poolManager.DeactivateObject;
            }
        }
    }
}