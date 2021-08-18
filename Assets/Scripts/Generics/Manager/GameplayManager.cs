using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EndlessT4cos.Gameplay.Enemies;
using EndlessT4cos.Gameplay.Player;
using EndlessT4cos.Gameplay.UI;
namespace Games.Generics.Manager
{
    public class GameplayManager : MonoBehaviour
    {
        [SerializeField] private int score;
        [SerializeField] private int scorePerKill;
        [SerializeField] private EnemiesManager enemiesManager;
        [SerializeField] private Player player;
        [SerializeField] private UIGameplayMananger ui;
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
            ui.ScoreUpdate(score);
        }
    }

}