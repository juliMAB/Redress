using System.Collections.Generic;
using UnityEngine;

using Games.Generics.PoolSystem;

namespace Games.Generics.Displacement
{
    public class MovableObjectsManager : PoolObjectsManager
    {
        [Header("Settings")]
        [SerializeField] protected Vector2 halfSizeOfScreen = Vector2.zero;
        [SerializeField] protected float distance = 2f;

        public float speed = 5f;

        protected virtual void Start()
        {
            objectsPool = new Queue<GameObject>();
            MovableObject movableObject;

            for (int i = 0; i < objects.Length; i++)
            {
                objectsPool.Enqueue(objects[i]);
                movableObject = objects[i].GetComponent<MovableObject>();
                movableObject.SetSize();
            }

            halfSizeOfScreen.x = 8.8f;
            halfSizeOfScreen.y = 5f;
        }

        protected virtual void Update()
        {
            MovableObject movableObject = null;

            for (int i = 0; i < objects.Length; i++)
            {
                if (!objects[i].activeSelf)
                {
                    continue;
                }

                movableObject = objects[i].GetComponent<MovableObject>();
                movableObject.Move(speed);

                if (IsOutOfScreen(movableObject))
                {
                    DeactivateObject(objects[i]);
                }
            }
        }

        protected void PlaceOnRightEnd(GameObject gObject, float yPosition)
        {
            MovableObject movableObject = gObject.GetComponent<MovableObject>();

            gObject.transform.position = new Vector3(halfSizeOfScreen.x + movableObject.HalfSize.x, yPosition, 1);
        }

        protected bool IsOutOfScreen(MovableObject movableObject)
        {
            return movableObject.transform.position.x + movableObject.HalfSize.x < -halfSizeOfScreen.x;
        }

        protected bool IsFarEnoughForNewObjectToSpawn(MovableObject movableObject)
        {
            return movableObject.transform.position.x + movableObject.HalfSize.x + distance < halfSizeOfScreen.x;
        }

        protected bool IsCompletelyOnScreen(MovableObject movableObject)
        {
            if (movableObject == null) return true;

            return //movableObject.transform.position.x - movableObject.HalfSize.x > -halfSizeOfScreen.x &&
                   movableObject.transform.position.x + movableObject.HalfSize.x < halfSizeOfScreen.x;
        }
    }
}
