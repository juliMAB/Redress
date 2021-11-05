using UnityEngine;
using System;
using System.Collections;

using Games.Generics.Displacement;
using EndlessT4cos.Gameplay.Management;

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

        private float[] initialDistanceLimits = new float[2];
        
        private float[] distanceLimits = new float[2];
        private float halfPlatformHeight = 0f;
        //private float unnevenes = 0f;
        private float unnevenesDuration = 0f;
        //private float normalEvenessDuration = 0f;
        private bool unnevenessActivated = false;
        //private IEnumerator unevennessInst = null;
        private InitialPlatform[] initialActivePlatforms = null;
        private bool pausePlatformMovement = false;
        private short actualYPos = 0;

        [Header("Platform Builiding Configurations")]
        [SerializeField] private float[] ySpawnPositions = null;
        [SerializeField] private int amountPlatformRows = 3;
        [SerializeField] private float startYPos = 0.65f;
        [SerializeField] private float verticalDistanceBetweenPlatforms = 2.35f;

        [Header("Platform Spawn Configurations")]
        [SerializeField] private float[] unnevenessDurationLimits = new float[2];
        //[SerializeField] private float[] unnevenesValuesLimits = null;
       // [SerializeField] private float[] normalEvenessDurationLimits = new float[2];

        public Action<float, float> OnUnneveness = null;

        public float HalfPlatformHeight => halfPlatformHeight; 
        public float[] YSpawnPositions => ySpawnPositions;
        public int AmountPlatformRows => amountPlatformRows;
        public float[] DistanceLimits => distanceLimits;
        public float VerticalDistanceBetweenPlatforms => verticalDistanceBetweenPlatforms; 

        private void Awake()
        {
            halfPlatformHeight = objects[0].transform.lossyScale.y / 2f;

            ySpawnPositions = new float[amountPlatformRows];

            for (int i = 0; i < amountPlatformRows; i++)
            {
                ySpawnPositions[i] = -i * verticalDistanceBetweenPlatforms + startYPos;
            }

           // normalEvenessDuration = UnityEngine.Random.Range(normalEvenessDurationLimits[0], normalEvenessDurationLimits[1]);
        }

        protected override void Start()
        {
            base.Start();

            FindInitialActivePlatforms();
        }

        public void Reset()
        {
            speed = initialSpeed;
            distanceLimits = initialDistanceLimits;

            SetInitialPlatforms();

            for (int i = 0; i < amountPlatformRows; i++)
            {
                ySpawnPositions[i] = -i * verticalDistanceBetweenPlatforms + startYPos;
            }

            //unnevenes = 0f;
            unnevenesDuration = 0f;
            //normalEvenessDuration = UnityEngine.Random.Range(normalEvenessDurationLimits[0], normalEvenessDurationLimits[1]);
            unnevenessActivated = false;
            pausePlatformMovement = false;

            //if (unevennessInst != null)
            //{
            //    StopCoroutine(unevennessInst);
            //}
        }

        public void SetValues(float speed, float minDistance, float maxDistance, bool setAsInitialValues)
        {
            if (setAsInitialValues)
            {
                initialSpeed = speed;
                initialDistanceLimits[0] = minDistance;
                initialDistanceLimits[1] = maxDistance;
            }

            this.speed = speed;
            distanceLimits[0] = minDistance;
            distanceLimits[1] = maxDistance;
        }

        
        public void PlatformsUpdate()
        {
            PlatformObject platform;

            for (int i = 0; i < objects.Length; i++)
            {
                if (!objects[i].activeSelf)
                {
                    continue;
                }

                distance = UnityEngine.Random.Range(distanceLimits[0], distanceLimits[1]) + UnityEngine.Random.Range(1, 10) / 10f;

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

            if(!unnevenessActivated && unnevenesDuration < 0)
            {
                SetPlatformsUnevennes();
            }
            else if (!unnevenessActivated)
            {
                unnevenesDuration -= Time.deltaTime;
            }
        }

        public void PauseMovement(bool pause)
        {
            pausePlatformMovement = pause;
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

        #region Unevenness
        private void SetPlatformsUnevennes()
        {
           //IEnumerator SetUnevenness()
           //{
           //    float time = 0f;
           //    float[] initialYpositions = new float[ySpawnPositions.Length];
           //
           //    for (int i = 0; i < amountPlatformRows; i++)
           //    {
           //        initialYpositions[i] = ySpawnPositions[i];
           //    }
           //
           //    while (time < unnevenesDuration)
           //    {
           //        if (pausePlatformMovement)
           //        {
           //            yield return null;
           //        }
           //        else
           //        {
           //            time += Time.deltaTime * GameplayManager.Instance.speedMultiplier;
           //            for (int i = 0; i < amountPlatformRows; i++)
           //            {
           //                ySpawnPositions[i] = Mathf.Lerp(initialYpositions[i], initialYpositions[i] + unnevenes, time / unnevenesDuration);
           //            }
           //
           //            yield return null;
           //        }
           //    }
           //
           //    //unnevenessActivated = false;
           //    //unevennessInst = null;
           //    //normalEvenessDuration = UnityEngine.Random.Range(normalEvenessDurationLimits[0], normalEvenessDurationLimits[1]);
           //}

            bool up = UnityEngine.Random.Range(0, 2) == 1;

            //if (up)
            //{
            //    actualYPos++;
            //}
            //else
            //{
            //    actualYPos--;
            //}

            float unnevennes = up ? verticalDistanceBetweenPlatforms : -verticalDistanceBetweenPlatforms;

            for (int i = 0; i < amountPlatformRows; i++)
            {
                ySpawnPositions[i] += unnevennes;
            }

            //unnevenessActivated = true;
            unnevenesDuration = UnityEngine.Random.Range(unnevenessDurationLimits[0], unnevenessDurationLimits[1]);
           // unnevenes = UnityEngine.Random.Range(unnevenesValuesLimits[0], unnevenesValuesLimits[1]);

            OnUnneveness?.Invoke(unnevennes, 2);

           // if (unevennessInst != null)
           // {
           //    // StopCoroutine(unevennessInst);
           // }

           // unevennessInst = SetUnevenness();
           // StartCoroutine(unevennessInst);
        }
        #endregion

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
