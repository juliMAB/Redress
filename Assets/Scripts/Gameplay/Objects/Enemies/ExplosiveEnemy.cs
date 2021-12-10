using System;
using UnityEngine;

using GuilleUtils.Interfaces;

namespace Redress.Gameplay.Objects.Enemies
{
    public class ExplosiveEnemy : Enemy
    {
        private Animator anim = null;
        private bool animationEnded = false;

        [SerializeField] private float radiusOfDamage = 2f;
        
        public float additionalSpeed = 2.5f;
        public Action OnExplode = null;

        [SerializeField] AK.Wwise.Event soundOnDetect;
        [SerializeField] AK.Wwise.Event soundOnExplote;

        private void Start()
        {
            anim = GetComponentInChildren<Animator>();
        }

        protected override void Update()
        {
            base.Update();
            if (!boxCollider2D.enabled)
                return;

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
            soundOnExplote.Post(gameObject);
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
            soundOnDetect.Post(gameObject);
            animationEnded = false;
        }

        public override void ResetStats()
        {
            //lives = initialLives;
            //lookingAtTarget = false;
            //direction = -Vector3.right;
            //jumped = false;
            //if (TryGetComponent(out Rigidbody2D body))
            //{
            //    body.velocity = Vector2.zero;
            //}
            base.ResetStats();
            SetAnimationEnded();
        }

        public void SetAnimationEnded()
        {
            animationEnded = true;
        }
    }
}