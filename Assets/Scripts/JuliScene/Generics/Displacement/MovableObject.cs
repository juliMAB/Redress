using System;
using System.Collections.Generic;
using UnityEngine;

namespace Games.Generics.Displacement
{
    public class MovableObject : MonoBehaviour
    {
        [SerializeField] protected Vector2 halfSize = Vector2.zero;
        [SerializeField] protected float speed = 5f;

        public Vector2 HalfSize { get => halfSize; }
        public float Speed { get => speed; }

        public void Move()
        {
            transform.position -= Vector3.right * speed * Time.deltaTime;
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

