using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using EndlessT4cos.Gameplay.Enemies;
using EndlessT4cos.Gameplay.User;
using EndlessT4cos.Gameplay.UI;

namespace EndlessT4cos.Gameplay.Manager
{
    public class GameplayManager : MonoBehaviour
    {
        [SerializeField] private int score = 0;
        [SerializeField] private int scorePerKill = 0;
        [SerializeField] private EnemiesManager enemiesManager = null;
        [SerializeField] private Player player = null;

        public Action<int> OnChangedScore = null;

        public EnemiesManager EnemiesManager { get => enemiesManager; }
        public Player Player { get => player; }

        private void Start()
        {
            Enemy enemy;

            for (int i = 0; i < enemiesManager.Objects.Length; i++)
            {
                enemy = enemiesManager.Objects[i].GetComponent<Enemy>();
                enemy.OnDie += AddScore;
            }
        }

        private void AddScore(GameObject go)
        {
            score += scorePerKill;
            OnChangedScore?.Invoke(score);
        }
    }

}