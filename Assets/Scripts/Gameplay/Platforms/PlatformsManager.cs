using UnityEngine;

namespace EndlessT4cos.Gameplay.Platforms
{
    public class PlatformsManager : PlatformObjectsManager
    {
        [Header("Platform Spawn")]
        [SerializeField] private float minDistance = 1;
        [SerializeField] private float maxDistance = 2;
        private float halfPlatformHeight = 0f;

        public float HalfPlatformHeight { get => halfPlatformHeight; }

        protected override void Awake()
        {
            base.Awake();

            halfPlatformHeight = objects[0].transform.lossyScale.y / 2f;
        }

        protected override void Start()
        {
            base.Start();

            waitTimeTillNextObject = new float[amountPlatformRows];

            for (int i = 0; i < waitTimeTillNextObject.Length; i++)
            {
                waitTimeTillNextObject[i] = Random.Range(minSpawnTime, maxSpawnTime);
            }
        }

        protected override void Update()
        {
            PlatformObject platform = null;

            for (int i = 0; i < objects.Length; i++)
            {
                if (!objects[i].activeSelf)
                {
                    continue;
                }

                distance = Random.Range(minDistance, maxDistance) + Random.Range(1, 10) / 10f;

                platform = objects[i].GetComponent<PlatformObject>();
                platform.Move(speed);

                if (IsOutOfScreen(platform))
                {
                    DeactivateObject(objects[i]);
                }
                else if (LastObjectIsFarEnough(platform.row) && IsCompletelyOnScreen(platform))
                {
                    GameObject newPlatform = ActivateObject();
                    PlaceOnRightEnd(newPlatform, ySpawnPositions[(int)platform.row]);
                    newPlatform.GetComponent<PlatformObject>().row = platform.row;
                }
            }
        }
    }
}
