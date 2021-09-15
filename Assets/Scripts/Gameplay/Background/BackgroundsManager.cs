using System.Collections.Generic;
using UnityEngine;

using Games.Generics.Displacement;

namespace EndlessT4cos.Gameplay.Background
{
    public class BackgroundsManager : MovableObjectsManager
    {
       struct InitialBackgrounds
       {
           public GameObject backgroundsGo;
           public Vector3 position;
       }
       
       private InitialBackgrounds[] initialActiveBackgrounds = null;


        protected override void Start()
        {
            objectsPool = new Queue<GameObject>();
            MovableObject movableObject;

            halfSizeOfScreen.x = 8.8f;
            halfSizeOfScreen.y = 5f;

            for (int i = 0; i < objects.Length; i++)
            {
                objectsPool.Enqueue(objects[i]);
                movableObject = objects[i].GetComponent<MovableObject>();
                movableObject.SetCustomSize(halfSizeOfScreen);
            }

            distance = 0;

           // FindInitialActiveBackgrounds();
        }

        private void Update()
        {
            MovableObject background;

            for (int i = 0; i < objects.Length; i++)
            {
                if (!objects[i].activeSelf)
                {
                    continue;
                }

                background = objects[i].GetComponent<MovableObject>();
                background.Move(speed);

                if (IsOutOfScreen(background))
                {
                    GameObject newBackground = ActivateObject();
                    PlaceOnRightEnd(newBackground, 0);

                    DeactivateObject(objects[i]);
                }
            }
        }

        public void Reset()
        {
            //SetInitialBackgrounds();
            //
            //MovableObject movableObject;
            //
            //while (objectsPool.Count > 0)
            //{
            //    objectsPool.Dequeue();
            //}
            //
            //for (int i = 0; i < objects.Length; i++)
            //{
            //    objectsPool.Enqueue(objects[i]);
            //    movableObject = objects[i].GetComponent<MovableObject>();
            //}
        }

        private void FindInitialActiveBackgrounds()
        {
            int amountActiveBackgrounds = 0;

            for (int i = 0; i < objects.Length; i++)
            {
                if (!objects[i].activeSelf)
                {
                    continue;
                }
                amountActiveBackgrounds++;
            }

            initialActiveBackgrounds = new InitialBackgrounds[amountActiveBackgrounds];

            int index = 0;

            for (int i = 0; i < objects.Length; i++)
            {
                if (!objects[i].activeSelf)
                {
                    continue;
                }

                initialActiveBackgrounds[i].backgroundsGo = objects[i];
                initialActiveBackgrounds[index].position = objects[i].transform.position;
                index++;
            }
        }

        private void SetInitialBackgrounds()
        {
            // deactivate all backgrounds
            for (int i = 0; i < objects.Length; i++)
            {
                if (objects[i].activeSelf)
                {
                    DeactivateObject(objects[i]);
                }
            }

            // activate and place initial backgrounds
            for (int i = 0; i < initialActiveBackgrounds.Length; i++)
            {
                initialActiveBackgrounds[i].backgroundsGo.SetActive(true);
                initialActiveBackgrounds[i].backgroundsGo.transform.position = initialActiveBackgrounds[i].position;
            }
        }
    }
}

