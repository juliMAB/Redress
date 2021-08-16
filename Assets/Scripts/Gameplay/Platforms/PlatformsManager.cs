using UnityEngine;

using Games.Generics.Displacement;

namespace EndlessT4cos.Gameplay.Platforms
{
    public class PlatformsManager : PlatformObjectsManager
    {
        protected override void Awake()
        {
            base.Awake();
        } 

        protected override void Start()
        {
            base.Start();
        }

        protected override void Update()
        {
            base.Update();

            PlatformObject platform = null;

            for (int i = 0; i < objects.Length; i++)
            {
                if (!objects[i].activeSelf)
                {
                    continue;
                }

                distance = Random.Range(minDistance, maxDistance) + Random.Range(1, 10) / 10f;

                platform = objects[i].GetComponent<PlatformObject>();

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
