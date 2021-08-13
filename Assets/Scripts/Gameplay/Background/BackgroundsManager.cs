using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Games.Generics.Displacement;

namespace EndlessT4cos.Gameplay.Background
{
    public class BackgroundsManager : MovableObjectsManager
    {
        protected override void Start()
        {
            base.Start();

            MovableObject movableObject = null;

            for (int i = 0; i < objects.Length; i++)
            {
                movableObject = objects[i].GetComponent<MovableObject>();
                movableObject.SetCustomSize(halfSizeOfScreen);
            }

            distance = 0;
        }

        protected override void Update()
        {
            base.Update();

            MovableObject background = null;

            for (int i = 0; i < objects.Length; i++)
            {
                if (!objects[i].activeSelf)
                {
                    continue;
                }

                background = objects[i].GetComponent<MovableObject>();

                if (IsOutOfScreen(background))
                {
                    DeactivateObject(objects[i]);

                    GameObject newBackground = ActivateObject();
                    PlaceOnRightEnd(newBackground, 0);
                }
            }
        }
    }
}

