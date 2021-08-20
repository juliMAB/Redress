using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

using EndlessT4cos.Gameplay.Manager;

namespace EndlessT4cos.Gameplay.UI
{
    public class UIGameplayMananger : MonoBehaviour
    {
        [SerializeField] private GameplayManager gameplayManager = null;
        [SerializeField] private TextMeshProUGUI scoreText = null;
        [SerializeField] private TextMeshProUGUI distanceText = null;
        [SerializeField] private TextMeshProUGUI velocityText = null;
        [SerializeField] Image[] lives = null;

        public void UpdateScore(int value)
        {
            scoreText.text = "Score: ";
            scoreText.text += value.ToString();
        }
        public void UpdateVelocity(int value)
        {
            velocityText.text = "Velocity: ";
            velocityText.text += value.ToString();
        }
        public void UpdateDistance(int value)
        {
            distanceText.text = "Distance: ";
            distanceText.text += value.ToString();
        }
        public void UpdateLives(int amountLives)
        {
            int maxLives = 5;
            if (amountLives > maxLives)
                amountLives = maxLives;
            else if (amountLives < 0)
                amountLives = 0;

            for (int i = 0; i < maxLives; i++)
            {
                if (lives[i].enabled)
                    lives[i].enabled = false;
            }

            for (int i = 0; i < amountLives; i++)
            {
                lives[i].enabled = true;
            }
        }

        private void Start()
        {
            gameplayManager.OnChangedScore += UpdateScore;
            gameplayManager.Player.OnLivesChanged += UpdateLives;
            UpdateVelocity((int)gameplayManager.EnemiesManager.Speed);
        }
        private void Update()
        {
            UpdateDistance((int)gameplayManager.Distance);
        }
    }
}

