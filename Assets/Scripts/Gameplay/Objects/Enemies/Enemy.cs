using System;
using System.Collections.Generic;
using UnityEngine;

using EndlessT4cos.Gameplay.Platforms;
using Games.Generics.Interfaces;

namespace EndlessT4cos.Gameplay.Objects.Enemies
{
    public enum Type { Static, Explosive, Shooter }

    public class Enemy : PlatformObject, IDamageable
    {
        [Header("Enemy")]
        [SerializeField] protected bool canDie = false;
        [SerializeField] protected int initialLives = 1;
        [SerializeField] protected int lives = 1;
        [SerializeField] protected LayerMask targetLayer = 0;       //Layer of the target
        [SerializeField] protected LayerMask hittableLayer = 0;     //All layers the enemy can cause damage to
        [SerializeField] protected float viewDistance = 7;
        [SerializeField] protected GameObject target = null;
        [SerializeField] protected bool lookingAtTarget = false;
        [SerializeField] protected float minDistanceToTarget = 1;

        public Type type = Type.Static;
        public Action<GameObject> OnDie = null;

        private void Start()
        {
            canDie = true;
        }

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
            if (!canDie)
            {
                return;
            }

            lives--;

            if (lives == 0)
            {
                Die();
            }
        }

        public virtual void Die()
        {
            OnDie?.Invoke(gameObject);
        }

        protected bool IsTargetForward()
        {
            return Physics2D.Raycast(transform.position, transform.right, viewDistance, targetLayer);
        }

        protected bool IsCloseToPLayer()
        {
            return Mathf.Abs(Vector2.Distance(transform.position, target.transform.position)) < minDistanceToTarget;
        }
    }
}
