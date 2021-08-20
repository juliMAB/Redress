using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using EndlessT4cos.Gameplay.Enemies;
using EndlessT4cos.Gameplay.User;
using EndlessT4cos.Gameplay.Platforms;

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

        [SerializeField] private int score = 0;
        [SerializeField] private float distance = 0;
        [SerializeField] private int scorePerKill = 0;
        [SerializeField] private EnemiesManager enemiesManager = null;
        [SerializeField] private Player player = null;
        [SerializeField] private PlatformsManager platformsManager = null;
        [SerializeField] private float yPlayerPosToLose = -5f;

        public Action<int> OnChangedScore = null;
        public Action OnGameplayEnded = null;

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
        }

        private void Update()
        {
            distance += platformsManager.Speed / 50;

            if (player.transform.position.y - player.transform.lossyScale.y / 2 < yPlayerPosToLose)
            {
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
    }
}