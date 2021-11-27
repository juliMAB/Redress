using UnityEngine;
using System;

using Redress.Gameplay.Platforms;
using Redress.Gameplay.Controllers;

namespace Redress.Gameplay.Management
{
    public class LevelProgressionManager : MonoBehaviour
    {
        private float speed = 5f;
        private float distance = 0;
        private float speedMultiplier = 1f;

        [Header("Initial values")]
        [SerializeField] private float initialSpeed = 5;
        [SerializeField] private float[] initialSpawnTimeLimits = null;
        [SerializeField] private float[] initialSpawnDistanceLimits = null;

        [Header("Level Progression Configuration")]
        [SerializeField] private float speedProgressionMultiplier = 0.02f;
        [SerializeField] private float distanceProgressionMultiplier = 0.1f;
        [SerializeField] private float bulletSpeedMultiplier = 2;
        [SerializeField] private float speedDivider = 40f; // Make little to speed up the general speed more rapidly.
        [SerializeField] private float layerSpeedDiff = 0.1f;

        [Header("Entities")]
        [SerializeField] private PlatformsManager platformsManager = null;
        [SerializeField] private PlatformObjectsManager objectsManager = null;
        [SerializeField] private ParallaxManager background = null;
        [SerializeField] private GunsController gunsController = null;

        public float Distance => distance;

        public void Initialize()
        {
            speed = initialSpeed;

            platformsManager.SetValues(speed, initialSpawnDistanceLimits[0], initialSpawnDistanceLimits[1], true);
            objectsManager.SetValues(speed, initialSpawnDistanceLimits[0], initialSpawnDistanceLimits[1], true);
            background.SetSpeed(speed, layerSpeedDiff);

            gunsController.Initialize(speed * bulletSpeedMultiplier);
        }

        public void SetLevelUpdate()
        {
            objectsManager.PlatformObjectsUpdate();
            platformsManager.PlatformsUpdate();
            background.UpdateBackground();
        }

        public void SetLevelProgression()
        {
            float speedProgression = Time.deltaTime * speedProgressionMultiplier * speedMultiplier;
            float distanceProgression = Time.deltaTime * distanceProgressionMultiplier * speedMultiplier;

            distance += platformsManager.Speed / speedDivider;
            AkSoundEngine.SetRTPCValue("Tiempo_Juego", distance);
            speed += speedProgression;

            float minSpawnTime = initialSpawnTimeLimits[0];
            float maxSpawnTime = initialSpawnTimeLimits[1];

            if (speedMultiplier < 1)
            {
                minSpawnTime *= 2;
                maxSpawnTime *= 2;
            }

            objectsManager.SetValues(speed * speedMultiplier, minSpawnTime, maxSpawnTime, false);
            platformsManager.SetValues(speed * speedMultiplier, platformsManager.DistanceLimits[0] + distanceProgression, platformsManager.DistanceLimits[1] + distanceProgression, false);

            gunsController.SetBulletsSpeed(speed * bulletSpeedMultiplier * speedMultiplier, speedMultiplier + Mathf.Epsilon > 1f);
            background.SetSpeed(initialSpeed * speedMultiplier, layerSpeedDiff * speedMultiplier);
        }

        public void SetSpeedMultiplier(float speedMultiplier)
        {
            this.speedMultiplier = speedMultiplier;

            gunsController.speedMultiplier = speedMultiplier;

            gunsController.AssingCooldownToGuns();
        }
    }
}