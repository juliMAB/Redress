﻿using UnityEngine;

namespace Games.Generics.Displacement
{
    public class MovableObject : MonoBehaviour
    {
        protected Vector3 direction = -Vector3.right;

        [SerializeField] protected Vector2 halfSize = Vector2.zero;

        public Vector2 HalfSize { get => halfSize; }

        public void Move(float speed)
        {
            transform.position += direction * speed * Time.deltaTime;
        }

        public void SetSize()
        {
            if (halfSize == Vector2.zero)
            {
                halfSize.x = transform.lossyScale.x / 2f;
                halfSize.y = transform.lossyScale.y / 2f;
            }
        }
    }
}

