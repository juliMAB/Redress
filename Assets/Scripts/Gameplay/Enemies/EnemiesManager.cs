using UnityEngine;

using EndlessT4cos.Gameplay.Platforms;


namespace EndlessT4cos.Gameplay.Enemies
{
    public enum Type { Static, Explosive, Shooter}
    
    public class EnemiesManager : PlatformObjectsManager
    {
        [Header("Enemy management")]
        [SerializeField] private GameObject target = null;
        [SerializeField] private PlatformsManager platformsManager = null;

        private float halfPlatformHeight = 0f;

        protected override void Start()
        {
            base.Start();

            halfPlatformHeight = platformsManager.HalfPlatformHeight;

            AssignTypes();
            AssignActionsAndTarget();

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
                    enemyComponent.ResetStats();
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

        private void AssignActionsAndTarget()
        {
            Enemy enemy;
            ExplosiveEnemy explosiveEnemy;
            ShooterEnemy shooterEnemy;

            for (int i = 0; i < objects.Length; i++)
            {
                enemy = objects[i].GetComponent<Enemy>();

                switch (enemy.type)
                {
                    case Type.Static:
                        break;
                    case Type.Explosive:
                        explosiveEnemy = enemy.GetComponent<ExplosiveEnemy>();
                        break;
                    case Type.Shooter:
                        break;
                    default:
                        break;
                }

                enemy.OnDie += DeactivateObject;
                enemy.SetTarget(target);                
            }
        }
    }
}
