using UnityEngine;

namespace Games.Generics.Displacement
{
    public class MovableObjectsManager : MonoBehaviour
    {
        protected float initialSpeed = 0f;
        protected float speed = 5f;

        [Header("Settings")]
        [SerializeField] protected Vector2 halfSizeOfScreen = Vector2.zero;
        [SerializeField] protected float distance = 2f;

        public float Speed => speed;

        protected virtual void Start()
        {
            halfSizeOfScreen.x = 8.8f;
            halfSizeOfScreen.y = 5f;
        }

        protected void PlaceOnRightEnd(GameObject gObject, float yPosition)
        {
            MovableObject movableObject = gObject.GetComponent<MovableObject>();

            gObject.transform.position = new Vector3(halfSizeOfScreen.x + movableObject.HalfSize.x, yPosition, 1);
        }

        protected bool IsOutOfScreen(MovableObject movableObject)
        {
            return movableObject.transform.position.x + movableObject.HalfSize.x < -halfSizeOfScreen.x - 1;
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
