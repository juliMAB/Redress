using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Games.Generics.Displacement;

namespace EndlessT4cos.Gameplay.Platforms
{
    public enum Row { Up, Middle, Down }

    public class PlatformsManager : MovableObjectsManager
    {
        [Header("Start position")]
        [SerializeField] private float[] yPosition = null;

        private float minDistance = 1f;
        private float maxDistance = 2f;

        private bool IsTheClosestToRightEdge(Row row, Platform platform) //Means it was the last to spawn
        {
            Platform closerPlatform = null;
            Platform actualPlatform = null;

            float diference = 100;
            float newDiference = 0;

            for (int i = 0; i < objects.Length; i++)
            {
                actualPlatform = objects[i].GetComponent<Platform>();

                if (!objects[i].activeSelf || actualPlatform.row != row)
                {
                    continue;
                }

                newDiference = Mathf.Abs(actualPlatform.transform.position.x - actualPlatform.HalfSize.x - halfSizeOfScreen.x);

                if (newDiference < diference)
                {
                    diference = newDiference;
                    closerPlatform = actualPlatform;
                }
            }

            return closerPlatform == platform;
        }

        private bool PlatformCanSpawn(Platform platform)
        {
            return !IsOutOfScreen(platform) && IsTheClosestToRightEdge(platform.row, platform) &&
                    IsFarEnoughForNewObjectToSpawn(platform) && IsCompletelyOnScreen(platform);
        }

        protected override void Start()
        {
            base.Start();
        }

        protected override void Update()
        {
            base.Update();

            Platform platform = null;

            for (int i = 0; i < objects.Length; i++)
            {
                if (!objects[i].activeSelf)
                {
                    continue;
                }

                distance = Random.Range(minDistance, maxDistance) + Random.Range(1, 10) / 10f;

                platform = objects[i].GetComponent<Platform>();

                if (IsOutOfScreen(platform))
                {
                    DeactivateObject(objects[i]);
                }
                else if (PlatformCanSpawn(platform))
                {
                    GameObject newPlatform = ActivateObject();
                    PlaceOnRightEnd(newPlatform, yPosition[(int)platform.row]);
                    newPlatform.GetComponent<Platform>().row = platform.row;
                }
            }
        }
    }
}
