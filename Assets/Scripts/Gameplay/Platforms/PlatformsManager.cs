﻿using UnityEngine;
using System;
using System.Collections;

using GuilleUtils.Displacement;
using Redress.Gameplay.Management;
using GuilleUtils.PoolSystem;

namespace Redress.Gameplay.Platforms
{
    public class PlatformsManager : MovableObjectsManager
    {
        public struct SpawnYPosition
        {
            public int id;
            public float y;
            public bool on;
        }

        private float[] initialDistanceLimits = new float[2];
        
        private float[] distanceLimits = new float[2];
        private float halfPlatformHeight = 0f;
        private float unnevenesDuration = 0f;
        private bool unnevenessActivated = false;
        private short actualmiddleRow = 0;

        private PoolObjectsManager poolManager = null;

        [Header("Platform Builiding Configurations")]
        [SerializeField] private float[] actualSpawnPositions = null;
        [SerializeField] private SpawnYPosition[] spawnPositions = null;
        [SerializeField] private int amountPlatformRows = 3;
        [SerializeField] private float startYPos = 0.65f;
        [SerializeField] private float verticalDistanceBetweenPlatforms = 2.35f;
        [SerializeField] private LayerMask layerMask = 0;

        [Header("Platform Spawn Configurations")]
        [SerializeField] private float[] unnevenessDurationLimits = new float[2];

        public Action<float, float> OnUnneveness = null;

        public float HalfPlatformHeight => halfPlatformHeight; 
        public float[] YSpawnPositions => actualSpawnPositions;
        public SpawnYPosition[] SpawnPositions => spawnPositions;
        public int AmountPlatformRows => amountPlatformRows;
        public float[] DistanceLimits => distanceLimits;
        public float VerticalDistanceBetweenPlatforms => verticalDistanceBetweenPlatforms;
        public short ActualmiddleRow => actualmiddleRow;
        public LayerMask LayerMask => layerMask; 

        private void Awake()
        {
            void ConfigureSpawnPoint(int id, float y, bool on)
            {
                spawnPositions[id].id = id;
                spawnPositions[id].y = y;
                spawnPositions[id].on = on;
            }

            halfPlatformHeight = PoolObjectsManager.Instance.Platforms.objects[0].transform.lossyScale.y / 2f;

            actualSpawnPositions = new float[amountPlatformRows];
            spawnPositions = new SpawnYPosition[amountPlatformRows + 2];

            for (int i = 0; i < amountPlatformRows; i++)
            {
                actualSpawnPositions[i] = -i * verticalDistanceBetweenPlatforms + startYPos;

                ConfigureSpawnPoint(i + 1, actualSpawnPositions[i], true);
            }

            ConfigureSpawnPoint(0, actualSpawnPositions[0] + verticalDistanceBetweenPlatforms, false);
            ConfigureSpawnPoint(spawnPositions.Length - 1, actualSpawnPositions[actualSpawnPositions.Length - 1] - verticalDistanceBetweenPlatforms, false);

            actualmiddleRow = 2;

            unnevenesDuration = UnityEngine.Random.Range(unnevenessDurationLimits[0], unnevenessDurationLimits[1]);
        }

        protected override void Start()
        {
            base.Start();
            MovableObject movableObject;
            poolManager = PoolObjectsManager.Instance;

            for (int i = 0; i < poolManager.Platforms.objects.Length; i++)
            {
                movableObject = poolManager.Platforms.objects[i].GetComponent<MovableObject>();
                movableObject.SetSize();
            }
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
            MovableObject platform;

            for (int i = 0; i < spawnPositions.Length; i++)
            {
                if (!spawnPositions[i].on)
                {
                    continue;
                }

                if (LastObjectIsFarEnough((SpawnLine)i, out platform) && IsCompletelyOnScreen(platform) && GetYPosRow((SpawnLine)i, out Row row))
                {
                    GameObject newPlatform = poolManager.ActivatePlatform();
                    PlaceOnRightEnd(newPlatform, spawnPositions[i].y);
                    newPlatform.GetComponent<MovableObject>().row = row;
                    newPlatform.GetComponent<MovableObject>().spawnLine = (SpawnLine)i;
                }
            }

            for (int i = 0; i < poolManager.Platforms.objects.Length; i++)
            {
                if (!poolManager.Platforms.objects[i].activeSelf)
                {
                    continue;
                }

                distance = UnityEngine.Random.Range(distanceLimits[0], distanceLimits[1]) + UnityEngine.Random.Range(1, 10) / 10f;

                platform = poolManager.Platforms.objects[i].GetComponent<MovableObject>();
                platform.Move(speed);

                if (IsOutOfScreen(platform))
                {
                    poolManager.DeactivateObject(poolManager.Platforms.objects[i]);
                }
            }

            //for (int i = 0; i < objects.Length; i++)
            //{
            //    if (!objects[i].activeSelf)
            //    {
            //        continue;
            //    }
            //
            //    distance = UnityEngine.Random.Range(distanceLimits[0], distanceLimits[1]) + UnityEngine.Random.Range(1, 10) / 10f;
            //
            //    platform = objects[i].GetComponent<PlatformObject>();
            //    platform.Move(speed);
            //
            //    if (IsOutOfScreen(platform))
            //    {
            //        DeactivateObject(objects[i]);
            //    }
            //    else if (LastObjectIsFarEnough(platform.spawnLine) && IsCompletelyOnScreen(platform) && GetYPosRow(platform.spawnLine, out Row row))
            //    {
            //        GameObject newPlatform = ActivateObject();
            //        PlaceOnRightEnd(newPlatform, spawnPositions[(int)platform.spawnLine].y);
            //        newPlatform.GetComponent<PlatformObject>().row = row;
            //        newPlatform.GetComponent<PlatformObject>().spawnLine = platform.spawnLine;
            //    }
            //}

            if (!unnevenessActivated && unnevenesDuration < 0)
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
            //pausePlatformMovement = pause;
        }

        private bool GetYPosRow(SpawnLine spawnLine, out Row row)
        {
            row = Row.Up;

            if (!spawnPositions[(int)spawnLine].on)
            {
                return false;
            }

            if (actualmiddleRow == (int)spawnLine)
            {
                row = Row.Middle;
            }
            else
            {
                row = actualmiddleRow < (int)spawnLine ? Row.Down : Row.Up;
            }

            return true;
        }

        private bool LastObjectIsFarEnough(SpawnLine spawnLine, out MovableObject closerObject)
        {
            closerObject = null;
            bool found = false;
            //float yPos = actualSpawnPositions[(int)row];
            //outRow = Row.Middle;
            //bool found = false;
            //
            //for (int i = 0; i < spawnPositions.Length; i++)
            //{
            //    if (spawnPositions[i].y == yPos && spawnPositions[i].on)
            //    {
            //        for (int j = 0; j < actualSpawnPositions.Length; j++)
            //        {
            //            if (actualSpawnPositions[j] == yPos)
            //            {
            //                outRow = (Row)j;
            //                found = true;
            //            }
            //        }
            //    }
            //}

            //if (Physics2D.Raycast(new Vector2(halfSizeOfScreen.x, yPos), Vector3.left, 100, layerMask))
            //    {
            //closerObject = Physics2D.Raycast(new Vector2(halfSizeOfScreen.x, yPos), Vector3.left, 100, layerMask).collider.GetComponent<PlatformObject>();
            //Debug.DrawRay(new Vector3(halfSizeOfScreen.x, yPos, 0), Vector3.left, Color.red, 2);

            for (int i = 0; i < poolManager.Platforms.objects.Length; i++)
            {
                closerObject = poolManager.Platforms.objects[i].GetComponent<MovableObject>();

                if (!poolManager.Platforms.objects[i].activeSelf)
                {
                    continue;
                }

                if (IsTheClosestToRightEdge(spawnLine, closerObject))
                {
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                return true;
            }

            // for (int i = 0; i < objects.Length; i++)
            // {
            //     closerObject = objects[i].GetComponent<PlatformObject>();
            // 
            //     if (!objects[i].activeSelf || closerObject.row != row)
            //     {
            //         continue;
            //     }
            // 
            //     if (IsTheClosestToRightEdge(closerObject.row, closerObject))
            //     {
            //         break;
            //     }
            // }

            //if (!closerObject || !found)
            //{
            //    return false;
            //}
            //outRow = closerObject.row;
            return IsFarEnoughForNewObjectToSpawn(closerObject);
        }

        private bool IsTheClosestToRightEdge(SpawnLine spawnLine, MovableObject platform) //Means it was the last to spawn
        {
            MovableObject closerObject = null;
            MovableObject actualObject;

            float diference = 100;
            float newDiference;

            for (int i = 0; i < poolManager.Platforms.objects.Length; i++)
            {
                actualObject = poolManager.Platforms.objects[i].GetComponent<MovableObject>();

                if (!poolManager.Platforms.objects[i].activeSelf || actualObject.spawnLine != spawnLine)
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
            short previousMiddleRow = actualmiddleRow;

            if (actualmiddleRow == 1) 
            {
                actualmiddleRow++;
            }
            else if (actualmiddleRow == 3)
            {
                actualmiddleRow--;
            }
            else
            {
                bool up = UnityEngine.Random.Range(0, 2) == 1;

                actualmiddleRow += up ? (short)-1 : (short)1;
            }

            for (int i = 0; i < spawnPositions.Length; i++)
            {
                spawnPositions[i].on = false;
            }

            for (int i = 0; i < amountPlatformRows; i++)
            {
                int spawnPositionIndex = actualmiddleRow - 1 + i;

                actualSpawnPositions[i] = spawnPositions[spawnPositionIndex].y;
                spawnPositions[spawnPositionIndex].on = true;
            }

            float unnevennes = previousMiddleRow > actualmiddleRow ? verticalDistanceBetweenPlatforms : -verticalDistanceBetweenPlatforms;

           // for (int i = 0; i < amountPlatformRows; i++)
            //{
           //     actualSpawnPositions[i] += unnevennes;
            //}

            unnevenesDuration = UnityEngine.Random.Range(unnevenessDurationLimits[0], unnevenessDurationLimits[1]);

            OnUnneveness?.Invoke(unnevennes, 2);
        }
        #endregion

        #region Initialization
        //private void FindInitialActivePlatforms()
        //{
        //    int amountActivePlatforms = 0;
        //
        //    for (int i = 0; i < objects.Length; i++)
        //    {
        //        if (!objects[i].activeSelf)
        //        {
        //            continue;
        //        }
        //
        //        amountActivePlatforms++;
        //    }
        //
        //    initialActivePlatforms = new InitialPlatform[amountActivePlatforms];
        //
        //    int index = 0;
        //
        //    for (int i = 0; i < objects.Length; i++)
        //    {
        //        if (!objects[i].activeSelf)
        //        {
        //            continue;
        //        }
        //
        //        PlatformObject actualPlatform = objects[i].GetComponent<PlatformObject>();
        //
        //        initialActivePlatforms[index].platformGo = objects[i];
        //        initialActivePlatforms[index].position = objects[i].transform.position;
        //        initialActivePlatforms[index].row = actualPlatform.row;
        //        index++;
        //    }
        //}

       //private void SetInitialPlatforms()
       //{
       //    // deactivate all platforms
       //    for (int i = 0; i < objects.Length; i++)
       //    {
       //        if (objects[i].activeSelf)
       //        {
       //            DeactivateObject(objects[i]);
       //        }
       //    }
       //
       //    // activate and place initial platforms
       //    for (int i = 0; i < initialActivePlatforms.Length; i++)
       //    {
       //        PlatformObject actualPlatform = initialActivePlatforms[i].platformGo.GetComponent<PlatformObject>();
       //
       //        actualPlatform.gameObject.SetActive(true);
       //        actualPlatform.transform.position = initialActivePlatforms[i].position;
       //        actualPlatform.row = initialActivePlatforms[i].row;
       //    }
       //}
        #endregion
    }
}
