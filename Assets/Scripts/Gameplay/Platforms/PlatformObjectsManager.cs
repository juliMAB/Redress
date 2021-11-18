using UnityEngine;

using Games.Generics.Displacement;
using Redress.Gameplay.Objects.Enemies;
using Redress.Gameplay.Objects.PickUps;
using Games.Generics.PoolSystem;

namespace Redress.Gameplay.Platforms
{
    public enum Row { Up, Middle, Down }
    public enum SpawnLine { First, Second, Third, Fourth, Fifth}

    public class PlatformObjectsManager : MovableObjectsManager
    {
        private float halfPlatformHeight = 0f;
        private float initialMinSpawnTime = 0f;
        private float initialMaxSpawnTime = 0f;

        private float minSpawnTime = 1f;
        private float maxSpawnTime = 2f;

        private Enemy[] enemies = null;
        private PickUp[] pickUps = null;

        private PoolObjectsManager poolManager = null;

        [Header("Platforms Configuration")]
        [SerializeField] private PlatformsManager platformsManager = null;
        [SerializeField] private string platformsTag = "Platform";

        [Header("Spawn Settings")]
        [SerializeField] private int amountEnemiesBeforePickUp = 15;
        [SerializeField] private int amountEnemiesPassed = 0;

        [Header("Platform Collision Settings")]
        [SerializeField] protected LayerMask layer = 0;
        [SerializeField] protected MovableObject largerObject = null;
        [SerializeField] protected float[] waitTimeTillNextObject = null;

        [Header("Passing Enemy")]
        [SerializeField] private PassingEnemy passingEnemy = null;

        public Enemy[] Enemies => enemies;
        public PickUp[] PickUps => pickUps; 
        public float MinSpawnTime => minSpawnTime; 
        public float MaxSpawnTime => maxSpawnTime;

        protected override void Start()
        {
            base.Start();

            MovableObject movableObject;
            poolManager = PoolObjectsManager.Instance;

            for (int i = 0; i < poolManager.Platforms.objects.Length; i++)
            {
                movableObject = poolManager.Platforms.objects[i].GetComponent<MovableObject>();
                movableObject.SetSize();
            }

            //--------------------------------------

            int totalAmountEnemiesInArray = 0; //is also index of the start of pickUps
            int enemiesInQueue = 0;
            int pickUpsAdded = 0;

            for (int i = 0; i < poolManager.PlatformObjects.objects.Length; i++)
            {
                if (poolManager.PlatformObjects.objects[i].TryGetComponent(out Enemy enemy))
                {
                    totalAmountEnemiesInArray++;
                }
                else
                {
                    break;
                }
            }

            poolManager.PlatformObjects.pool.Clear();

            for (int i = 0; i < poolManager.PlatformObjects.objects.Length; i++)
            {
                if (enemiesInQueue < amountEnemiesBeforePickUp)
                {
                    poolManager.PlatformObjects.pool.Enqueue(poolManager.PlatformObjects.objects[i - pickUpsAdded]);
                    enemiesInQueue++;
                }
                else
                {
                    poolManager.PlatformObjects.pool.Enqueue(poolManager.PlatformObjects.objects[totalAmountEnemiesInArray + pickUpsAdded]);
                    pickUpsAdded++;
                    
                    enemiesInQueue = 0;
                }

                movableObject = poolManager.PlatformObjects.objects[i].GetComponent<MovableObject>();
                movableObject.SetSize();
            }

            //--------------------------------------
            
            halfPlatformHeight = platformsManager.HalfPlatformHeight;

            SetComponentsDynamicsArrays();

            waitTimeTillNextObject = new float[platformsManager.SpawnPositions.Length];

            for (int i = 0; i < waitTimeTillNextObject.Length; i++)
            {
                waitTimeTillNextObject[i] = Random.Range(minSpawnTime, maxSpawnTime);
            }

            passingEnemy.SetOutOfScreenXValue(halfSizeOfScreen.x);
        }

        public void Pause(bool pause)
        {
            passingEnemy.Pause(pause);
        }

        public void SetValues(float speed, float minSpawnTime, float maxSpawnTime, bool setAsInitialValues)
        {
            if (setAsInitialValues)
            {
                initialSpeed = speed;
                initialMinSpawnTime = minSpawnTime;
                initialMaxSpawnTime = maxSpawnTime;
            }

            this.speed = speed;
            this.minSpawnTime = minSpawnTime;
            this.maxSpawnTime = maxSpawnTime;
        }

        public void PlatformObjectsUpdate()
        {
            MovableObjectsUpdate();

            for (int i = 0; i < platformsManager.AmountPlatformRows; i++)
            {
                int waitTimeIndex = platformsManager.ActualmiddleRow - 1 + i;

                waitTimeTillNextObject[waitTimeIndex] -= Time.deltaTime;

                Vector2 position = new Vector2(halfSizeOfScreen.x + largerObject.HalfSize.x, platformsManager.YSpawnPositions[i] + halfPlatformHeight * 2);

                if (waitTimeTillNextObject[waitTimeIndex] < 0 && 
                    TheresEnoughFloorDown(position, halfPlatformHeight * 2, largerObject, out Transform platform) &&
                    TheresEnoughSpaceInBetweenPlatforms(position, 100, largerObject, platform.position))
                {
                    waitTimeTillNextObject[waitTimeIndex] = Random.Range(minSpawnTime, maxSpawnTime);

                    GameObject newObject = poolManager.ActivatePlatformObject();

                    MovableObject platformObjectComponent = newObject.GetComponent<MovableObject>();
                    PlaceOnRightEnd(newObject, platform.position.y + platformObjectComponent.HalfSize.y + halfPlatformHeight);
                    platformObjectComponent.row = (Row)i;

                    ResetObjectStats(newObject);
                }
            }

            float[] yPositions = new float[platformsManager.YSpawnPositions.Length];

            for (int i = 0; i < yPositions.Length; i++)
            {
                yPositions[i] = platformsManager.YSpawnPositions[i] + platformsManager.VerticalDistanceBetweenPlatforms / 2f;
            }

            passingEnemy.UpdatePassingEnemy(yPositions);
        }

        private bool TheresFloorDown(Vector2 position, float distance, out Transform platform)
        {
            platform = null;
            RaycastHit2D hit = Physics2D.Raycast(position, Vector2.down, distance, layer);

            if (Physics2D.Raycast(position, Vector2.down, distance, layer))
            {
                platform = hit.collider.transform.tag == platformsTag ? hit.collider.transform : null;
                return true; 
            }
            else
            {
                return false;
            }
        }

        private void MovableObjectsUpdate()
        {
            MovableObject movableObject = null;

            for (int i = 0; i < poolManager.PlatformObjects.objects.Length; i++)
            {
                if (!poolManager.PlatformObjects.objects[i].activeSelf)
                {
                    continue;
                }

                movableObject = poolManager.PlatformObjects.objects[i].GetComponent<MovableObject>();
                movableObject.Move(speed);

                if (IsOutOfScreen(movableObject))
                {
                    poolManager.DeactivateObject(poolManager.PlatformObjects.objects[i]);
                }
            }
        }

        private bool TheresEnoughFloorDown(Vector2 position, float distance, MovableObject enemy, out Transform platform)
        {
            return TheresFloorDown(position, distance, out platform) &&
                   TheresFloorDown(position + Vector2.right * enemy.HalfSize.x, distance, out platform) &&
                   TheresFloorDown(position + Vector2.left * enemy.HalfSize.x, distance, out platform);
        }

        private bool TheresSpaceInBetweenPlatforms(Vector2 position, float distance, MovableObject enemy, Vector3 downPlatformPosition)
        {
            RaycastHit2D hit = Physics2D.Raycast(position, Vector2.up, distance, layer);
            Transform upPlatform;

            if (Physics2D.Raycast(position, Vector2.up, distance, layer))
            {
                upPlatform = hit.collider.transform.tag == platformsTag ? hit.collider.transform : null;

                if (upPlatform && upPlatform.position.y - downPlatformPosition.y > enemy.HalfSize.y * 2 + platformsManager.HalfPlatformHeight * 2)
                {
                    return true;
                }
            }
            else
            {
                return true;
            }

            return false;
        }

        private bool TheresEnoughSpaceInBetweenPlatforms(Vector2 position, float distance, MovableObject enemy, Vector3 downPlatformPosition)
        {
            return TheresSpaceInBetweenPlatforms(position, distance, enemy, downPlatformPosition) &&
                   TheresSpaceInBetweenPlatforms(position + Vector2.right * enemy.HalfSize.x, distance, enemy, downPlatformPosition) &&
                   TheresSpaceInBetweenPlatforms(position + Vector2.left * enemy.HalfSize.x, distance, enemy, downPlatformPosition);
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

            for (int i = 0; i < poolManager.PlatformObjects.objects.Length; i++)
            {
                if (poolManager.PlatformObjects.objects[i].TryGetComponent(out Enemy enemyComponent))
                {
                    enemyCount++;
                }

                if (poolManager.PlatformObjects.objects[i].TryGetComponent(out PickUp pickUpComponent))
                {
                    pickUpCount++;
                }
            }

            enemies = new Enemy[enemyCount];
            pickUps = new PickUp[pickUpCount];

            for (int i = 0; i < poolManager.PlatformObjects.objects.Length; i++)
            {
                if (poolManager.PlatformObjects.objects[i].TryGetComponent(out Enemy enemyComponent))
                {
                    enemyCount--;
                    enemies[enemyCount] = enemyComponent;
                }

                if (poolManager.PlatformObjects.objects[i].TryGetComponent(out PickUp pickUpComponent))
                {
                    pickUpCount--;
                    pickUps[pickUpCount] = pickUpComponent;
                }
            }
        }
    }
}