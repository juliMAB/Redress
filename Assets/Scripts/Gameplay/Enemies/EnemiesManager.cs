using UnityEngine;

using EndlessT4cos.Gameplay.Platforms;


namespace EndlessT4cos.Gameplay.Enemies
{
    public class EnemiesManager : PlatformObjectsManager
    {
        [SerializeField] private GameObject target = null;
        [SerializeField] private PlatformsManager platformsManager = null;

        private float halfPlatformHeight = 0f;

        protected override void Start()
        {
            base.Start();

            halfPlatformHeight = platformsManager.HalfPlatformHeight;

            Enemy enemy = null;

            for (int i = 0; i < objects.Length; i++)
            {
                enemy = objects[i].GetComponent<Enemy>();
                enemy.SetTarget(target);
                enemy.OnDie += DeactivateObject;
            }

            waitTimeTillNextObject = new float[amountPlatformRows];

            for (int i = 0; i < waitTimeTillNextObject.Length; i++)
            {
                waitTimeTillNextObject[i] = Random.Range(minSpawnTime, maxSpawnTime);
            }
        }

        protected override void Update()
        {
            base.Update();

            /*PlatformObject enemy = null;

            for (int i = 0; i < objects.Length; i++)
            {
                if (!objects[i].activeSelf)
                {
                    continue;
                }

                enemy = objects[i].GetComponent<PlatformObject>();

                if (IsOutOfScreen(enemy))
                {
                    DeactivateObject(objects[i]);
                }
            }*/

            for (int i = 0; i < waitTimeTillNextObject.Length; i++)
            {
                waitTimeTillNextObject[i] -= Time.deltaTime;

                Vector2 position = new Vector2(halfSizeOfScreen.x + largerObject.HalfSize.x, ySpawnPositions[i] + halfPlatformHeight * 2);

                if (waitTimeTillNextObject[i] < 0 && TheresEnoughFloorDown(position, halfPlatformHeight * 2, largerObject))
                { 
                    waitTimeTillNextObject[i] = Random.Range(minSpawnTime, maxSpawnTime);

                    GameObject newEnemy = ActivateObject();
                    Enemy enemyComponent = newEnemy.GetComponent<Enemy>();

                    PlaceOnRightEnd(newEnemy, ySpawnPositions[i] + enemyComponent.HalfSize.y + halfPlatformHeight);
                    enemyComponent.row = (Row)i;
                    enemyComponent.ResetLives();
                }
            }
        }

        /*private bool EnemyCanSpawn(PlatformObject enemy)
        {
            Vector2 position = new Vector2(halfSizeOfScreen.x + enemy.HalfSize.x, ySpawnPositions[(int)enemy.row] + halfPlatformHeight * 2);

            return LastObjectIsFarEnough(enemy.row) && IsCompletelyOnScreen(enemy) &&
                   TheresEnoughFloorDown(position, halfPlatformHeight * 2, enemy);
        }

        private bool EnemyCanSpawn(PlatformObject enemy)
        {
            Vector2 position = new Vector2(halfSizeOfScreen.x + enemy.HalfSize.x, ySpawnPositions[(int)enemy.row] + halfPlatformHeight * 2);

            return LastObjectIsFarEnough(enemy.row) && IsCompletelyOnScreen(enemy) &&
                   TheresEnoughFloorDown(position, halfPlatformHeight * 2, enemy);
        }

        private bool CheckAnyActiveEnemyOnRow(Row row)
        {
            for (int i = 0; i < objects.Length; i++)
            {
                PlatformObject enemy = objects[i].GetComponent<PlatformObject>();

                if (objects[i].activeSelf && enemy.row == row)
                {
                    return true;
                }
            }

            return false;
        }*/
    }
}
