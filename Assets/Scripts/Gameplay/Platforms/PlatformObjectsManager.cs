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
        private Life[] lives = null;

        [SerializeField] private PlatformsManager platformsManager = null;

        [Header("Platform collision settings")]
        [SerializeField] protected LayerMask layer = 0;
        [SerializeField] protected PlatformObject largerObject = null;
        [SerializeField] protected float[] waitTimeTillNextObject = null;

        public float minSpawnTime = 1f;
        public float maxSpawnTime = 2f;

        public Enemy[] Enemies { get => enemies; }
        public Life[] Lives { get => lives; }

        protected override void Start()
        {
            base.Start();

            halfPlatformHeight = platformsManager.HalfPlatformHeight;

            SetComponentsDynamicsArrays();

            waitTimeTillNextObject = new float[platformsManager.AmountPlatformRows];

            for (int i = 0; i < waitTimeTillNextObject.Length; i++)
            {
                waitTimeTillNextObject[i] = Random.Range(minSpawnTime, maxSpawnTime);
            }
        }

        protected override void Update()
        {
            base.Update();

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
        }

        private void SetComponentsDynamicsArrays()
        {
            int enemyCount = 0;
            int livesCount = 0;

            for (int i = 0; i < objects.Length; i++)
            {
                if (objects[i].TryGetComponent(out Enemy enemyComponent))
                {
                    enemyCount++;
                }

                if (objects[i].TryGetComponent(out Life lifeComponent))
                {
                    livesCount++;
                }
            }

            enemies = new Enemy[enemyCount];
            lives = new Life[livesCount];

            for (int i = 0; i < objects.Length; i++)
            {
                if (objects[i].TryGetComponent(out Enemy enemyComponent))
                {
                    enemyCount--;
                    enemies[enemyCount] = enemyComponent;
                }

                if (objects[i].TryGetComponent(out Life lifeComponent))
                {
                    livesCount--;
                    lives[livesCount] = lifeComponent;
                }
            }
        }
    }
}