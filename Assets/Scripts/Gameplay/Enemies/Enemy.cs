using System;
using System.Collections.Generic;
using UnityEngine;

using EndlessT4cos.Gameplay.Platforms;
using Games.Generics.Interfaces;

namespace EndlessT4cos.Gameplay.Enemies
{
    public class Enemy : PlatformObject, IDamageable
    {
        [Header("Enemy")]
        [SerializeField] protected int initialLives = 1;
        [SerializeField] protected int lives = 1;
        [SerializeField] protected LayerMask targetLayer = 0;

        protected GameObject target = null;
        public Type type = Type.Static;
        public Action<GameObject> OnDie = null;

        public void SetTarget(GameObject _target)
        {
            target = _target;
        }

        public void ResetLives()
        {
            lives = initialLives;
        }

        public void TakeDamage()
        {
            lives--;

            if (lives == 0)
            {
                Die();
            }
        }

        public void Die()
        {
            OnDie?.Invoke(gameObject);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject == target)
            {
                IDamageable targetIDamageable = target.GetComponent<IDamageable>();
                targetIDamageable.TakeDamage();
            }
        }
    }
}
