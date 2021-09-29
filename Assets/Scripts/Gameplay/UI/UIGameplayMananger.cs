using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

using EndlessT4cos.Gameplay.Management;

namespace EndlessT4cos.Gameplay.UI
{
    public class UIGameplayMananger : MonoBehaviour
    {
        [SerializeField] private GameplayManager gameplayManager = null;
        [SerializeField] private Text scoreText = null;
        [SerializeField] private Text distanceText = null;
        [SerializeField] private GameObject retryPanel = null;
        [SerializeField] private GameObject pausePanel = null;
        [SerializeField] private GameObject pauseButton = null;
        [SerializeField] private Image[] lives = null;

        public void UpdateScore(int value)
        {
            scoreText.text = "Score: ";
            scoreText.text += value.ToString();
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
            gameplayManager.Player.OnDie+= ActivatePanelRetry;
        }

        private void Update()
        {
            UpdateDistance((int)gameplayManager.Distance);
            if (Input.GetKeyDown(KeyCode.P))
            {
                ActivatePausePanel();
            }
        }

        public void RetryButton()
        {
            UpdateLives(gameplayManager.Player.InitialLives);
            pausePanel.SetActive(false);
            pauseButton.SetActive(true);
        }

        private void ActivatePanelRetry()
        {
            retryPanel.SetActive(true);
            pauseButton.SetActive(false);
        }
        public void ActivatePausePanel()
        {
            pausePanel.SetActive(!pausePanel.activeSelf);
            retryPanel.SetActive(pausePanel.activeSelf);
        }
    }
}

