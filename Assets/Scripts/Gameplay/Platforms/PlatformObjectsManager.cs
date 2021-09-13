using System.Collections.Generic;
using UnityEngine;

using Games.Generics.Displacement;
using EndlessT4cos.Gameplay.Objects.Enemies;
using EndlessT4cos.Gameplay.Objects.PickUps;

namespace EndlessT4cos.Gameplay.Platforms
{
    public enum Row { Up, Middle, Down }

    public class PlatformObjectsManager : MovableObjectsManager
    {
        private float halfPlatformHeight = 0f;

        private Enemy[] enemies = null;
        private PickUp[] pickUps = null;

        [SerializeField] private PlatformsManager platformsManager = null;

        [Header("Spawn Settings")]
        [SerializeField] private int amountEnemiesBeforePickUp = 15;
        [SerializeField] private int amountEnemiesPassed = 0;

        [Header("Platform Collision Settings")]
        [SerializeField] protected LayerMask layer = 0;
        [SerializeField] protected PlatformObject largerObject = null;
        [SerializeField] protected float[] waitTimeTillNextObject = null;

        public float minSpawnTime = 1f;
        public float maxSpawnTime = 2f;

        public Enemy[] Enemies { get => enemies; }
        public PickUp[] PickUps { get => pickUps; }

        protected override void Start()
        {
            objectsPool = new Queue<GameObject>();
            MovableObject movableObject;

            halfSizeOfScreen.x = 8.8f;
            halfSizeOfScreen.y = 5f;

            //--------------------------------------

            int totalAmountEnemiesInArray = 0; //is also index of the start of pickUps
            int enemiesInQueue = 0;
            int pickUpsAdded = 0;

            for (int i = 0; i < objects.Length; i++)
            {
                if (objects[i].TryGetComponent(out Enemy enemy))
                {
                    totalAmountEnemiesInArray++;
                }
                else
                {
                    break;
                }
            }

            for (int i = 0; i < objects.Length; i++)
            {
                if (enemiesInQueue < amountEnemiesBeforePickUp)
                {
                    objectsPool.Enqueue(objects[i - pickUpsAdded]);
                    enemiesInQueue++;
                }
                else
                {
                    objectsPool.Enqueue(objects[totalAmountEnemiesInArray + pickUpsAdded]);
                    pickUpsAdded++;
                    
                    enemiesInQueue = 0;
                }

                movableObject = objects[i].GetComponent<MovableObject>();
                movableObject.SetSize();
            }

            //--------------------------------------
            
            halfPlatformHeight = platformsManager.HalfPlatformHeight;

            SetComponentsDynamicsArrays();

            waitTimeTillNextObject = new float[platformsManager.AmountPlatformRows];

            for (int i = 0; i < waitTimeTillNextObject.Length; i++)
            {
                waitTimeTillNextObject[i] = Random.Range(minSpawnTime, maxSpawnTime);
            }
        }

        public void PlatformObjectsManagerUpdate()
        {
            MovableObjectsManagerUpdate();

            for (int i = 0; i < waitTimeTillNextObject.Length; i++)
            {
                waitTimeTillNextObject[i] -= Time.deltaTime;

                Vector2 position = new Vector2(halfSizeOfScreen.x + largerObject.HalfSize.x, platformsManager.YSpawnPositions[i] + halfPlatformHeight * 2);

                if (waitTimeTillNextObject[i] < 0 && TheresEnoughFloorDown(position, halfPlatformHeight * 2, largerObject))
                {
                    waitTimeTillNextObject[i] = Random.Range(minSpawnTime, maxSpawnTime);

                    GameObject newObject = ActivateObject();

                    PlatformObject platformObjectComponent = newObject.GetComponent<PlatformObject>();
                    PlaceOnRightEnd(newObject, platformsManager.YSpawnPositions[i] + platformObjectComponent.HalfSize.y + halfPlatformHeight);
                    platformObjectComponent.row = (Row)i;

                    ResetObjectStats(newObject);
                }
            }
        }

        private bool TheresFloorDown(Vector2 position, float distance)
        {
            return Physics2D.Raycast(position, Vector2.down, distance, layer);
        }

        private bool TheresEnoughFloorDown(Vector2 position, float distance, PlatformObject enemy)
        {
            return TheresFloorDown(position, distance) &&
                   TheresFloorDown(position + Vector2.right * enemy.HalfSize.x, distance) &&
                   TheresFloorDown(position - Vector2.right * enemy.HalfSize.x, distance);
        }

        private void ResetObjectStats(GameObject newObject)
        {
            if (newObject.TryGetComponent(out Enemy enemyComponent))
            {
                enemyComponent.ResetStats();
            }
            
            if (newObject.TryGetComponent(out PickUp pickUpComponent))
            {
                pickUpComponent.ResetStats();
            }
        }

        private void SetComponentsDynamicsArrays()
        {
            int enemyCount = 0;
            int pickUpCount = 0;

            for (int i = 0; i < objects.Length; i++)
            {
                if (objects[i].TryGetComponent(out Enemy enemyComponent))
                {
                    enemyCount++;
                }

                if (objects[i].TryGetComponent(out PickUp pickUpComponent))
                {
                    pickUpCount++;
                }
            }

            enemies = new Enemy[enemyCount];
            pickUps = new PickUp[pickUpCount];

            for (int i = 0; i < objects.Length; i++)
            {
                if (objects[i].TryGetComponent(out Enemy enemyComponent))
                {
                    enemyCount--;
                    enemies[enemyCount] = enemyComponent;
                }

                if (objects[i].TryGetComponent(out PickUp pickUpComponent))
                {
                    pickUpCount--;
                    pickUps[pickUpCount] = pickUpComponent;
                }
            }
        }
    }
}