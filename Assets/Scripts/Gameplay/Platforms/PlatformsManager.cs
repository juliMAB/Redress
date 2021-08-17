using UnityEngine;

namespace EndlessT4cos.Gameplay.Platforms
{
    public class PlatformsManager : PlatformObjectsManager
    {
        private float halfPlatformHeight = 0f;

        public float HalfPlatformHeight { get => halfPlatformHeight; }

        protected override void Awake()
        {
            base.Awake();

            halfPlatformHeight = objects[0].transform.lossyScale.y / 2f;
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
