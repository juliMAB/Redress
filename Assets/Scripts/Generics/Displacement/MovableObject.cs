using UnityEngine;

namespace Games.Generics.Displacement
{
    public class MovableObject : MonoBehaviour
    {
        [SerializeField] protected Vector2 halfSize = Vector2.zero;

        protected Vector3 direction = -Vector3.right;

        public Vector2 HalfSize { get => halfSize; }

        public void Move(float speed)
        {
            transform.position += direction * speed * Time.deltaTime;
        }

        public void SetSize()
        {
            halfSize.x = transform.lossyScale.x / 2f;
            halfSize.y = transform.lossyScale.y / 2f;
        }

        public void SetCustomSize(Vector2 size)
        {
            halfSize = size;
        }
    }
}

