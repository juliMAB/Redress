using System;
using UnityEngine;

using Games.Generics.Interfaces;

namespace Redress.Gameplay.Objects.Enemies
{
    public class ExplosiveEnemy : Enemy
    {
        private Animator anim = null;
        private bool animationEnded = false;

        [SerializeField] private float radiusOfDamage = 2f;
        
        public float additionalSpeed = 2.5f;
        public Action OnExplode = null;

        private void Start()
        {
            anim = GetComponentInChildren<Animator>();
        }

        protected override void Update()
        {
            base.Update();

            if (lookingAtTarget && animationEnded)
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
                    iDamageable.TakeDamage(transform.position); 
                }
                
            }

            OnExplode?.Invoke();
            OnDie?.Invoke(gameObject);
        }

        protected override void OnDetectedPlayer()
        {
            anim.SetTrigger("Player Detected");
            animationEnded = false;
        }

        public void SetAnimationEnded()
        {
            animationEnded = true;
        }
    }
}