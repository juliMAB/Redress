using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using EndlessT4cos.Gameplay.Platforms;


namespace EndlessT4cos.Gameplay.Enemies
{
    public class EnemiesManager : PlatformObjectsManager
    {
        [Header("Platform collision settings")]
        [SerializeField] private LayerMask layer = 0;
        [SerializeField] private PlatformsManager platformsManager = null;

        private Vector2 halfSizePlatform = Vector2.zero;

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Start()
        {
            base.Start();

            //ySpawnPositions = new float[platformsManager.AmountPlatformRows];
            //
            //for (int i = 0; i < ySpawnPositions.Length; i++)
            //{
            //    ySpawnPositions[i] = YSpawnPositions[i];
            //}
            //
            //halfSizePlatform = platformsManager.Objects[0].transform.lossyScale / 2f;
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

                    PlaceOnRightEnd(newEnemy, ySpawnPositions[(int)enemy.row] + enemyComponent.HalfSize.y);
                    enemyComponent.row = enemy.row;
                }
            }
        }

        private bool TheresFloorDown(Vector2 position, float distance)
        {
            return Physics.Raycast(position, Vector2.down, distance, layer, QueryTriggerInteraction.Ignore);
        }

        private bool EnemyCanSpawn(PlatformObject enemy)
        {
            Vector2 position = new Vector2(halfSizeOfScreen.x, ySpawnPositions[(int)enemy.row] + 2);

            return LastObjectIsFarEnough(enemy.row) && IsCompletelyOnScreen(enemy); //&&
                   //TheresFloorDown(position, 5);
        }
    }
}
