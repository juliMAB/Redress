using System;
using System.Collections.Generic;
using UnityEngine;

namespace EndlessT4cos.Gameplay.Enemies
{
    public class ExplosiveEnemy : Enemy
    {
        [SerializeField] private float viewDistance = 7f;
        [SerializeField] private float minDistanceToExplode = 1.5f;

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
            return Physics.Raycast(transform.position, transform.right, viewDistance, targetLayer, QueryTriggerInteraction.Collide);
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
            OnExplode?.Invoke();
        }
    }
}