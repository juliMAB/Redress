using System;
using System.Collections.Generic;
using UnityEngine;

namespace EndlessT4cos.Gameplay.Platforms
{
    public class Platform : MonoBehaviour
    {
        [SerializeField] private Row row = Row.Middle;
        private Vector2 halfSize = Vector2.zero;
        private Action<GameObject> onOutOfScreen = null;
        private float speed = 5f;

        public Row Row { get => row; set => row = value; }
        public Vector2 HalfSize { get => halfSize; set => halfSize = value; }
        public Action<GameObject> OnOutOfScreen { get => onOutOfScreen; set => onOutOfScreen = value; }
        public float Speed { get => speed; set => speed = value; }

        public void Move()
        {
            transform.position -= Vector3.right * speed * Time.deltaTime;
        }

        public void SetSize()
        {
            halfSize.x = transform.lossyScale.x / 2f;
            halfSize.y = transform.lossyScale.y / 2f;
        }
    }
}
