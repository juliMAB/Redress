using System;
using System.Collections.Generic;
using UnityEngine;

using Games.Generics.Interfaces;

namespace EndlessT4cos.Gameplay.Enemies
{
    public class ExplosiveEnemy : Enemy
    {
        [SerializeField] private float radiusOfDamage = 2f;
        
        public float additionalSpeed = 5f;
        public Action OnExplode = null;

        protected override void Update()
        {
            base.Update();

            if (lookingAtTarget)
            {
                direction = Vector3.Normalize(target.transform.position - transform.position) * additionalSpeed;
            }

            if (IsCloseToPLayer())
            {
                Explode();
            }
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