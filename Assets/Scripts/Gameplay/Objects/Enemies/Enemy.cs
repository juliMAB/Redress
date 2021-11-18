﻿using System;
using UnityEngine;

using Games.Generics.Displacement;
using Games.Generics.Interfaces;

namespace Redress.Gameplay.Objects.Enemies
{
    public enum Type { Static, Explosive, Shooter, Wall }

    public class Enemy : MovableObject, IDamageable
    {
        private bool jumped = false;

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
        protected string nameSound;
        public Type type = Type.Static;
        public Action<GameObject> OnDie = null;

        private void Start()
        {
            nameSound = SoundsManager.Get().EstaticoMuere;
            canDie = true;
            switch (type)
            {
                case Type.Static:
                    nameSound = SoundsManager.Get().EstaticoMuere;
                    break;
                case Type.Explosive:
                    break;
                case Type.Shooter:
                    nameSound = SoundsManager.Get().PistolaMuerte;
                    break;
                case Type.Wall:
                    nameSound = SoundsManager.Get().Pared;
                    break;
                default:
                    break;
            }
        }

        protected virtual void Update()
        {
            if (IsTargetForward())
            {
                if (!lookingAtTarget)
                {
                    OnDetectedPlayer();
                }
                lookingAtTarget = true;
            }
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.gameObject == target)
            {
                IDamageable targetIDamageable = target.GetComponent<IDamageable>();
                targetIDamageable.TakeDamage();
            }
        }
       //private void OnTriggerEnter2D(Collider2D collision)
       //{
       //    if (collision.gameObject == target)
       //    {
       //        IDamageable targetIDamageable = target.GetComponent<IDamageable>();
       //        targetIDamageable.TakeDamage();
       //    }
       //}

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject == target)
            {
                IDamageable targetIDamageable = target.GetComponent<IDamageable>();
                targetIDamageable.TakeDamage();                
            }
        }

        protected virtual void OnDetectedPlayer()
        {

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
            jumped = false;
            if (TryGetComponent(out Rigidbody2D body))
            {
                body.velocity = Vector2.zero;
            }
        }

        public void TakeDamage()
        {
            Debug.Log(gameObject.name + " a tomado daño.");
            AkSoundEngine.PostEvent(nameSound, gameObject);
            
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

        public void Jump()
        {
            if (!jumped)
            { 
                GetComponent<Rigidbody2D>().velocity += Vector2.up * 2;
                jumped = true;
            }
        }
    }
}
