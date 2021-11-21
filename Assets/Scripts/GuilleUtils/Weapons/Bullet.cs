using System;
using UnityEngine;

using GuilleUtils.Interfaces;

namespace GuilleUtils.Weapon
{
    public class Bullet : MonoBehaviour
    {
        public Action<GameObject> OnCollided = null;
        public float speed = 1f;

        public void Move(float speed)
        {
            transform.position += transform.right * speed * Time.deltaTime;
        }

        private void Update()
        {
            Move(speed);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            IDamageable iDamageable = collision.GetComponent<IDamageable>();

            if (iDamageable != null)
            {
                iDamageable.TakeDamage(transform.position);
            }

            OnCollided?.Invoke(gameObject);
        }
    }
}

