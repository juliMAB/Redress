using UnityEngine;

using Games.Generics.Displacement;

namespace EndlessT4cos.Gameplay.Platforms
{
    public class PlatformsManager : MovableObjectsManager
    {
        struct InitialPlatform
        {
            public GameObject platformGo;
            public Vector3 position;
            public Row row;
        }

        private float halfPlatformHeight = 0f;
        private InitialPlatform[] initialActivePlatforms = null;

        [Header("Platform builiding configurations")]
        [SerializeField] private float[] ySpawnPositions = null;
        [SerializeField] private int amountPlatformRows = 3;
        [SerializeField] private float startYPos = 0.65f;
        [SerializeField] private float verticalDistanceBetweenPlatforms = 2.35f;

        [Header("Platform Spawn")]
        public float minDistance = 1;
        public float maxDistance = 2;

        public float HalfPlatformHeight { get => halfPlatformHeight; }
        public float[] YSpawnPositions { get => ySpawnPositions; set => ySpawnPositions = value; }
        public int AmountPlatformRows { get => amountPlatformRows; set => amountPlatformRows = value; }

        public void Reset()
        {
            SetInitialPlatforms();
        }

        private void Awake()
        {
            halfPlatformHeight = objects[0].transform.lossyScale.y / 2f;

            ySpawnPositions = new float[amountPlatformRows];

            for (int i = 0; i < amountPlatformRows; i++)
            {
                ySpawnPositions[i] = -i * verticalDistanceBetweenPlatforms + startYPos;
            }
        }

        protected override void Start()
        {
            base.Start();

            FindInitialActivePlatforms();
        }

        protected override void Update()
        {
            PlatformObject platform;

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

        private bool LastObjectIsFarEnough(Row row)
        {
            PlatformObject closerObject = null;

            for (int i = 0; i < objects.Length; i++)
            {
                closerObject = objects[i].GetComponent<PlatformObject>();

                if (!objects[i].activeSelf || closerObject.row != row)
                {
                    continue;
                }

                if (IsTheClosestToRightEdge(closerObject.row, closerObject))
                {
                    break;
                }
            }

            return IsFarEnoughForNewObjectToSpawn(closerObject);
        }

        private bool IsTheClosestToRightEdge(Row row, PlatformObject platform) //Means it was the last to spawn
        {
            PlatformObject closerObject = null;
            PlatformObject actualObject;

            float diference = 100;
            float newDiference;

            for (int i = 0; i < objects.Length; i++)
            {
                actualObject = objects[i].GetComponent<PlatformObject>();

                if (!objects[i].activeSelf || actualObject.row != row)
                {
                    continue;
                }

                newDiference = Mathf.Abs(actualObject.transform.position.x - actualObject.HalfSize.x - halfSizeOfScreen.x);

                if (newDiference < diference)
                {
                    diference = newDiference;
                    closerObject = actualObject;
                }
            }

            return closerObject == platform;
        }

        #region Initialization
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

                actualPlatform.gameObject.SetActive(true);
                actualPlatform.transform.position = initialActivePlatforms[i].position;
                actualPlatform.row = initialActivePlatforms[i].row;
            }
        }
        #endregion
    }
}
