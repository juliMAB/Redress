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
        private static GameplayManager instance = null;
        public static GameplayManager Instance { get => instance; }

        private void Awake()
        {
            instance = this;
        }

        private const float scoreChargeDecrease = 0.0001f;
        private float time = 0f;

        private PoolObjectsManager poolManager = null;

        [Header("Global variables")]
        [SerializeField] private int score = 0;
        [SerializeField] private float timeToChargeScore = 1;
        [SerializeField] private int scorePerKill = 0;

        [Header("Gameplay configuration")]
        [SerializeField] private Vector2 playerPosToLose = Vector2.zero;
        [SerializeField] private float halfPlayerHeight = 0.88f;
        [SerializeField] private Vector2 halfSizeScreen = Vector2.zero;

        [Header("Entities")]
        [SerializeField] private Player player = null;
        [SerializeField] private PlatformsManager platformsManager = null;
        [SerializeField] private PlatformObjectsManager objectsManager = null;
        [SerializeField] private PauseManager pauseManager = null;
        [SerializeField] private CameraController cameraController = null;
        [SerializeField] private HighscoreManager highscoreManager = null;

        [Header("Managers")]
        [SerializeField] private LevelProgressionManager levelProgressionManager = null;

        public Action<int> OnChangedScore = null;
        public Action OnGameplayEnded = null;
        public Action<int> OnNextState = null;
        public Action OnResetGame = null;

        public Vector2 PlayerPosToLose => playerPosToLose;
        public Player Player => player;
        public float HalfPlayerHeight  => halfPlayerHeight; 

        private void Start()
        {
            poolManager = PoolObjectsManager.Instance;

            AssignActionsAndTargetToEnemies();
            AssignPlayerAndActionToPickUp();
            SetSlowMotionActions();

            levelProgressionManager.Initialize();

            platformsManager.OnUnneveness += cameraController.PositionCamera;

            player.CharacterMovement.Controller.halfPlayerHeight = halfPlayerHeight;
        }

        private void Update()
        {
            if (pauseManager.GameIsPaused)
            {
                return;
            }

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

                SetPlayerInputLock();

                levelProgressionManager.SetLevelProgression();
            }
        }

        private void SetScoreUpdate()
        {
            time += Time.deltaTime;
            if (time > timeToChargeScore)
            {
                score += (int)platformsManager.Speed;
                OnChangedScore?.Invoke(score);
                timeToChargeScore -= scoreChargeDecrease;
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
            player.CharacterMovement.ControlActive = false;
            player.ControlActive = false;

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
            const float distance = 10;

            RaycastHit2D hit = Physics2D.Raycast(player.transform.position, Vector2.down, distance, platformsManager.LayerMask);

            bool CheckPlatformUnder(MovableObject platformUnder, out RaycastHit2D hit2)
            {
                Vector2 pos = new Vector2(player.transform.position.x, platformUnder.transform.position.y - 1);

                hit2 = Physics2D.Raycast(pos, Vector2.down, distance, platformsManager.LayerMask);

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