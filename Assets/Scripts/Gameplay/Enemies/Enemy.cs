using System;
using System.Collections.Generic;
using UnityEngine;

using EndlessT4cos.Gameplay.Platforms;
using Games.Generics.Interfaces;

namespace EndlessT4cos.Gameplay.Enemies
{
    public class Enemy : PlatformObject, IDamageable
    {
        [SerializeField] private int initialLives = 1;
        [SerializeField] private int lives = 1;
        [SerializeField] private int score = 0;

        private GameObject target = null;
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
