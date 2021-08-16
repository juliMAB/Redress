using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Games.Generics.Displacement;

namespace EndlessT4cos.Gameplay.Platforms
{
    public enum Row { Up, Middle, Down }

    public class PlatformObjectsManager : MovableObjectsManager
    {
        [Header("Start position")]
        [SerializeField] protected float[] ySpawnPositions = null;
        [SerializeField] protected float minDistance = 1f;
        [SerializeField] protected float maxDistance = 2f;

        [SerializeField] protected int amountPlatformRows = 3;

        public float[] YSpawnPositions { get => ySpawnPositions; }
        public int AmountPlatformRows { get => amountPlatformRows; }

        protected virtual void Awake()
        {
            float verticalDistanceBetweenPlatforms = 1.7f;

            ySpawnPositions = new float[amountPlatformRows];

            for (int i = 0; i < amountPlatformRows; i++)
            {
                ySpawnPositions[i] = -i * verticalDistanceBetweenPlatforms;
            }
        }

        protected bool LastObjectIsFarEnough(Row row)
        {
            PlatformObject closerObject = null;

            for (int i = 0; i < objects.Length; i++)
            {
                closerObject = objects[i].GetComponent<PlatformObject>();

                if (!objects[i].activeSelf || closerObject.row != row)
                {
                    continue;
                }

                if (IsTheClosestToRightEdge(closerObject.row, closerObject))
                {
                    break;
                }
            }

            return IsFarEnoughForNewObjectToSpawn(closerObject);
        }

        protected bool IsTheClosestToRightEdge(Row row, MovableObject platform) //Means it was the last to spawn
        {
            PlatformObject closerObject = null;
            PlatformObject actualObject = null;

            float diference = 100;
            float newDiference = 0;

            for (int i = 0; i < objects.Length; i++)
            {
                actualObject = objects[i].GetComponent<PlatformObject>();

                if (!objects[i].activeSelf || actualObject.row != row)
                {
                    continue;
                }

                newDiference = Mathf.Abs(actualObject.transform.position.x - actualObject.HalfSize.x - halfSizeOfScreen.x);

                if (newDiference < diference)
                {
                    diference = newDiference;
                    closerObject = actualObject;
                }
            }

            return closerObject == platform;
        }
    }
}