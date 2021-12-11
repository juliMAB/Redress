using UnityEngine;
using UnityEngine.UI;
using System;

using Redress.Gameplay.Management;

namespace Redress.Gameplay.UI
{
    public class UIGameplayMananger : MonoBehaviour
    {
        private bool panel1LighstActivated = true;

        [SerializeField] private GameplayManager gameplayManager = null;
        [SerializeField] private LevelProgressionManager levelProgressionManager = null;
        [SerializeField] private Text scoreText = null;
        [SerializeField] private Text distanceText = null;
        [SerializeField] private GameObject retryPanel = null;
        [SerializeField] private GameObject pausePanel = null;
        [SerializeField] private GameObject optionsPanel = null;
        [SerializeField] private GameObject pauseButton = null;
        [SerializeField] private GameObject gameOverPanel = null;
        [SerializeField] private Image[] lives = null;

        [Header("UI Animation Configuration")]
        [SerializeField] private float changeEffectTime = 0.5f;
        [SerializeField] private float time = 0f;
        [SerializeField] private GameObject[] lightsPanel1 = null;
        [SerializeField] private GameObject[] lightsPanel2 = null;

        public Action OnShowHighscorePanel = null;

        private void Start()
        {
            gameplayManager.OnChangedScore += UpdateScore;
            gameplayManager.Player.OnLivesChanged += UpdateLives;            
            gameplayManager.Player.OnDie+= ActivateGameOverPanel;
        }

        private void Update()
        {
            UpdateDistance((int)levelProgressionManager.Distance);
            if (Input.GetKeyDown(KeyCode.P))
            {
                ActivatePausePanel();
            }

            time += Time.unscaledDeltaTime;

            if (time > changeEffectTime)
            {
                ChangeLights();

                time = 0f;
            }
        }

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

        public void RetryButton()
        {
            UpdateLives(gameplayManager.Player.InitialLives);
            pausePanel.SetActive(false);
            pauseButton.SetActive(true);
        }

        private void ActivatePanelRetry()
        {
            pausePanel.SetActive(true);
            retryPanel.SetActive(true);
            pauseButton.SetActive(false);
            OnShowHighscorePanel?.Invoke();
        }
        public void ActivatePausePanel()
        {
            pausePanel.SetActive(!pausePanel.activeSelf);
            retryPanel.SetActive(pausePanel.activeSelf);
            optionsPanel.SetActive(false);
            OnShowHighscorePanel?.Invoke();
        }

        public void ActivateGameOverPanel()
        {
            pauseButton.SetActive(false);
            gameOverPanel.SetActive(true);
        }

        public void ActivateOptionsPanel()
        {
            retryPanel.SetActive(!retryPanel.activeSelf);
            optionsPanel.SetActive(!retryPanel.activeSelf);
        }

        private void ChangeLights()
        {
            panel1LighstActivated = !panel1LighstActivated;

            for (int i = 0; i < lightsPanel1.Length; i++)
            {
                lightsPanel1[i].SetActive(panel1LighstActivated);
            }

            for (int i = 0; i < lightsPanel2.Length; i++)
            {
                lightsPanel2[i].SetActive(!panel1LighstActivated);
            }
        }
    }
}

