using UnityEngine;

using EndlessT4cos.Gameplay.Platforms;


namespace EndlessT4cos.Gameplay.Enemies
{
    public enum Type { Static, Explosive, Shooter}
    
    public class EnemiesManager : PlatformObjectsManager
    {
        [SerializeField] private GameObject target = null;
        [SerializeField] private PlatformsManager platformsManager = null;

        private float halfPlatformHeight = 0f;

        protected override void Start()
        {
            base.Start();

            halfPlatformHeight = platformsManager.HalfPlatformHeight;

            Enemy enemy;

            for (int i = 0; i < objects.Length; i++)
            {
                enemy = objects[i].GetComponent<Enemy>();
                enemy.SetTarget(target);
                enemy.OnDie += DeactivateObject;
            }

            AssignTypes();

            waitTimeTillNextObject = new float[amountPlatformRows];

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

        private void AssignTypes()
        {
            Enemy enemy;
            Type type;

            for (int i = 0; i < objects.Length; i++)
            {
                enemy = objects[i].GetComponent<Enemy>();

                type = Type.Static;

                if (objects[i].TryGetComponent(out ExplosiveEnemy explosiveEnemy))
                {
                    type = Type.Explosive;
                }
                else if (objects[i].TryGetComponent(out ShooterEnemy shooterEnemy))
                {
                    type = Type.Shooter;
                }

                enemy.type = type;
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
