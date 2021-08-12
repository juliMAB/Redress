using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EndlessT4cos.Gameplay.Platforms
{
    public enum Row { Up, Middle, Down }

    public class PlatformsManager : MonoBehaviour
    {
        [SerializeField] private GameObject[] platforms = null;
        [SerializeField] private Queue<GameObject> platformPool = null;
        [SerializeField] private LayerMask layer = 0;

        [SerializeField] private float halfSizeOfScreen = 8.9f;

        [SerializeField] private float[] yPosition = null;
        private float distance = 2f;
        private float minDistance = 1f;
        private float maxDistance = 2f;

        const int amountRows = 3;

        private bool[] rowReady = null;

        private void PlacePlatform(Row row, GameObject platformGO)
        {
            Platform platform = platformGO.GetComponent<Platform>();

            platform.transform.position = new Vector2(halfSizeOfScreen + platform.HalfSize.x, yPosition[(int)row]);
            platform.Row = row;
        }

        private void DeactivatePlatform(GameObject platform)
        {
            platform.SetActive(false);
            platformPool.Enqueue(platform);
        }

        private GameObject ActivatePlatform()
        {
            GameObject platform = platformPool.Dequeue();

            while (platform.activeSelf)
            {
                platformPool.Enqueue(platform);
                platform = platformPool.Dequeue();
            }
            
            platform.SetActive(true);

            return platform;
        }

        private bool IsTheClosestToRightEdge(Row row, Platform platform) //Means it was the last to spawn
        {
            Platform closerPlatform = null;
            Platform actualPlatform = null;

            float diference = 100;
            float newDiference = 0;

            for (int i = 0; i < platforms.Length; i++)
            {
                actualPlatform = platforms[i].GetComponent<Platform>();

                if (!platforms[i].activeSelf || actualPlatform.Row != row)
                {
                    continue;
                }

                newDiference = Mathf.Abs(actualPlatform.transform.position.x - actualPlatform.HalfSize.x - halfSizeOfScreen);

                if (newDiference < diference)
                {
                    diference = newDiference;
                    closerPlatform = actualPlatform;
                }
            }

            return closerPlatform == platform;
        }

        private bool IsOutOfScreen(Platform platform)
        {
            return platform.transform.position.x + platform.HalfSize.x < -halfSizeOfScreen;
        }

        private bool IsFarEnoughForNewPlatformToSpawn(Platform platform)
        {
            return platform.transform.position.x + platform.HalfSize.x + distance < halfSizeOfScreen;
        }

        private bool IsCompletelyOnScreen(Platform platform)
        {
            if (platform == null) return true;

            return platform.transform.position.x - platform.HalfSize.x > -halfSizeOfScreen &&
                   platform.transform.position.x + platform.HalfSize.x < halfSizeOfScreen;
        }

        private void Start()
        {
            platformPool = new Queue<GameObject>();
            Platform platform = null;

            for (int i = 0; i < platforms.Length; i++)
            {
                platformPool.Enqueue(platforms[i]);
                platform = platforms[i].GetComponent<Platform>();
                platform.SetSize();
            }
        }

        private void Update()
        {
            Platform platform = null;

            for (int i = 0; i < platforms.Length; i++)
            {
                if (!platforms[i].activeSelf)
                {
                    continue;
                }

                platform = platforms[i].GetComponent<Platform>();
                platform.Move();

                distance = Random.Range(minDistance, maxDistance) + Random.Range(1, 10) / 10f;

                if (IsOutOfScreen(platform))
                {
                    DeactivatePlatform(platforms[i]);
                }
                else if (IsTheClosestToRightEdge(platform.Row, platform) && IsFarEnoughForNewPlatformToSpawn(platform) && IsCompletelyOnScreen(platform))
                {
                   
                    GameObject newPlatform = ActivatePlatform();
                    PlacePlatform(platform.Row, newPlatform);
                }
            }
        }
    }
}
