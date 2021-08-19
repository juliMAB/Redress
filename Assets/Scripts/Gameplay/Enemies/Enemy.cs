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
        [SerializeField] protected LayerMask targetLayer = 0;       //Layer of the target
        [SerializeField] protected LayerMask hittableLayer = 0;     //All layers the enemy can cause damage to
        [SerializeField] protected float viewDistance = 7f;
        [SerializeField] protected GameObject target = null;
        [SerializeField] protected bool lookingAtTarget = false;

        public Type type = Type.Static;
        public Action<GameObject> OnDie = null;

        //public bool LookingAtTarget { get => lookingAtTarget; }

        protected virtual void Update()
        {
            if (IsTargetForward())
            {
                lookingAtTarget = true;
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject == target)
            {
                IDamageable targetIDamageable = target.GetComponent<IDamageable>();
                targetIDamageable.TakeDamage();
            }
        }

        public void SetTarget(GameObject _target)
        {
            target = _target;
        }

        public void ResetStats()
        {
            lives = initialLives;
            lookingAtTarget = false;
            direction = -Vector3.right;
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

        protected bool IsTargetForward()
        {
            return Physics2D.Raycast(transform.position, transform.right, viewDistance, targetLayer);
        }
    }
}
