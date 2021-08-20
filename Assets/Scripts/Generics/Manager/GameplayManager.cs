﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using EndlessT4cos.Gameplay.Enemies;
using EndlessT4cos.Gameplay.User;
using EndlessT4cos.Gameplay.Platforms;

namespace EndlessT4cos.Gameplay.Manager
{
    public class GameplayManager : MonoBehaviour
    {
        [SerializeField] private int score = 0;
        [SerializeField] private float distance = 0;
        [SerializeField] private int scorePerKill = 0;
        [SerializeField] private EnemiesManager enemiesManager = null;
        [SerializeField] private Player player = null;
        [SerializeField] private PlatformsManager platformsManager = null;

        public Action<int> OnChangedScore = null;

        public EnemiesManager EnemiesManager { get => enemiesManager; }
        public Player Player { get => player; }
        public float Distance { get => distance; }
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
        }
        private void AddScore(GameObject go)
        {
            score += scorePerKill;
            OnChangedScore?.Invoke(score);
        }
    }

}