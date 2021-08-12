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

        [SerializeField] private float halfSizeOfScreen = 8.9f;

        [SerializeField] private float[] yPosition = null;
        private float distance = 1f;

        private void PlacePlatform(Row row, GameObject platformGO)
        {
            Platform platform = platformGO.GetComponent<Platform>();

            platform.transform.position = new Vector2(halfSizeOfScreen + platform.HalfSize.x + distance, yPosition[(int)row]);
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

        private void Start()
        {
            platformPool = new Queue<GameObject>();

            for (int i = 0; i < platforms.Length; i++)
            {
                platformPool.Enqueue(platforms[i]);
            }
        }

        private void Update()
        {
            Platform platform = null;

            for (int i = 0; i < platforms.Length; i++)
            {
                if (platforms[i].activeSelf)
                {
                    platform = platforms[i].GetComponent<Platform>();
                    platform.Move();

                    if (platforms[i].transform.position.x + platform.HalfSize.x < -halfSizeOfScreen)
                    {
                        DeactivatePlatform(platforms[i]);

                        GameObject newPlatform = ActivatePlatform();
                        PlacePlatform(platform.Row, newPlatform);
                    }
                }
            }
        }
    }
}
