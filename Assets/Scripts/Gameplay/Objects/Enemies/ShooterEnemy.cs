using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Games.Generics.Weapon;

namespace Redress.Gameplay.Objects.Enemies
{
    public class ShooterEnemy : Enemy
    {
        [SerializeField] private Gun gun = null;
        private Animator animator = null;


        private void Start()
        {
            canDie = true;
        }
        private void Awake()
        {
            animator = GetComponentInChildren<Animator>();
        }

        protected override void Update()
        {
            base.Update();

            if (lookingAtTarget)
            {
                gun.Shoot();
            }

            if (!IsTargetForward())
            {
                animator.SetBool("Detect", true);
                lookingAtTarget = false;
            }
        }

        public override void Die()
        {
            animator.SetBool("Detect", false);
            gun.ResetStats();
            base.Die();
        }
    }
}
