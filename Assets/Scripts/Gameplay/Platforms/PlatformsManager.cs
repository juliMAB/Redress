using UnityEngine;

namespace EndlessT4cos.Gameplay.Platforms
{
    public class PlatformsManager : PlatformObjectsManager
    {
        struct InitialPlatform
        {
            public GameObject platformGo;
            public Vector3 position;
            public Row row;
        }

        private float halfPlatformHeight = 0f;
        private InitialPlatform[] initialActivePlatforms = null;

        [Header("Platform Spawn")]
        public float minDistance = 1;
        public float maxDistance = 2;

        public float HalfPlatformHeight { get => halfPlatformHeight; }

        public void Reset()
        {
            SetInitialPlatforms();
        }

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

            FindInitialActivePlatforms();
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

        private void FindInitialActivePlatforms()
        {
            int amountActivePlatforms = 0;

            for (int i = 0; i < objects.Length; i++)
            {
                if (!objects[i].activeSelf)
                {
                    continue;
                }
                amountActivePlatforms++;
            }

            initialActivePlatforms = new InitialPlatform[amountActivePlatforms];

            int index = 0;

            for (int i = 0; i < objects.Length; i++)
            {
                if (!objects[i].activeSelf)
                {
                    continue;
                }

                PlatformObject actualPlatform = objects[i].GetComponent<PlatformObject>();

                initialActivePlatforms[index].platformGo = objects[i];
                initialActivePlatforms[index].position = objects[i].transform.position;
                initialActivePlatforms[index].row = actualPlatform.row;
                index++;
            }
        }

        private void SetInitialPlatforms()
        {
            // deactivate all platforms
            for (int i = 0; i < objects.Length; i++)
            {
                if (objects[i].activeSelf)
                {
                    DeactivateObject(objects[i]);
                }
            }

            // activate and place initial platforms
            for (int i = 0; i < initialActivePlatforms.Length; i++)
            {
                PlatformObject actualPlatform = initialActivePlatforms[i].platformGo.GetComponent<PlatformObject>();

                initialActivePlatforms[i].platformGo.SetActive(true);
                initialActivePlatforms[i].platformGo.transform.position = initialActivePlatforms[i].position;
                initialActivePlatforms[i].row = actualPlatform.row;
            }
        }
    }
}
