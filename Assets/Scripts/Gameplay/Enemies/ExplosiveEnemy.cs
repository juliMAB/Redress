using System;
using System.Collections.Generic;
using UnityEngine;

using Games.Generics.Interfaces;

namespace EndlessT4cos.Gameplay.Enemies
{
    public class ExplosiveEnemy : Enemy
    {
        [SerializeField] private float viewDistance = 7f;
        [SerializeField] private float minDistanceToExplode = 1.5f;
        [SerializeField] private LayerMask hittableLayer = 0;
        [SerializeField] private float radiusOfDamage = 1f;

        private bool lookingAtTarget = false;
        public float speed = 5f;
        public Action OnExplode = null;

        private void Update()
        {
            if (IsTargetForward())
            {
                lookingAtTarget = true;
            }

            if (lookingAtTarget)
            {
                FollowPlayer();
            }

            if (IsCloseToPLayer())
            {
                Explode();
            }
        }

        private bool IsTargetForward()
        {
            return Physics2D.Raycast(transform.position, transform.right, viewDistance, targetLayer);
        }

        private void FollowPlayer()
        {
            Vector3 dirVector = Vector3.Normalize(target.transform.position - transform.position);

            transform.position += dirVector * speed * Time.deltaTime;
        }

        private bool IsCloseToPLayer()
        {
            return Mathf.Abs(Vector3.Distance(transform.position, target.transform.position)) < minDistanceToExplode;
        }

        private void Explode()
        {
            Collider2D[] collidersToHit = Physics2D.OverlapCircleAll(transform.position, radiusOfDamage, hittableLayer);

            for (int i = 0; i < collidersToHit.Length; i++)
            {
                IDamageable iDamageable = collidersToHit[i].gameObject.GetComponent<IDamageable>();
                
                if (iDamageable != null)
                { 
                    iDamageable.TakeDamage(); 
                }
            }

            OnExplode?.Invoke();
            OnDie?.Invoke(gameObject);
        }
    }
}