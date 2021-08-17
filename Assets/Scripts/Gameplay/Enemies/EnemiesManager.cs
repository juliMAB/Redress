using UnityEngine;

using EndlessT4cos.Gameplay.Platforms;


namespace EndlessT4cos.Gameplay.Enemies
{
    public class EnemiesManager : PlatformObjectsManager
    {
        [Header("Platform collision settings")]
        [SerializeField] private LayerMask layer = 0;
        [SerializeField] private PlatformsManager platformsManager = null;

        private float halfPlatformHeight = 0f;

        protected override void Start()
        {
            base.Start();

            halfPlatformHeight = platformsManager.HalfPlatformHeight;
        }

        protected override void Update()
        {
            base.Update();

            PlatformObject enemy = null;

            for (int i = 0; i < objects.Length; i++)
            {
                if (!objects[i].activeSelf)
                {
                    continue;
                }

                distance = Random.Range(minDistance, maxDistance) + Random.Range(1, 10) / 10f;

                enemy = objects[i].GetComponent<PlatformObject>();

                if (IsOutOfScreen(enemy))
                {
                    DeactivateObject(objects[i]);
                }
                else if (EnemyCanSpawn(enemy))
                {
                    GameObject newEnemy = ActivateObject();
                    PlatformObject enemyComponent = newEnemy.GetComponent<PlatformObject>();

                    PlaceOnRightEnd(newEnemy, ySpawnPositions[(int)enemy.row] + enemyComponent.HalfSize.y + halfPlatformHeight);
                    enemyComponent.row = enemy.row;
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

        private bool EnemyCanSpawn(PlatformObject enemy)
        {
            Vector2 position = new Vector2(halfSizeOfScreen.x + enemy.HalfSize.x, ySpawnPositions[(int)enemy.row] + halfPlatformHeight * 2);

            return LastObjectIsFarEnough(enemy.row) && IsCompletelyOnScreen(enemy) &&
                   TheresEnoughFloorDown(position, halfPlatformHeight * 2, enemy);
        }
    }
}
