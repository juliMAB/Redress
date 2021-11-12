using UnityEngine;

using Games.Generics.Weapon;

namespace Redress.Gameplay.Objects.Enemies
{
    public class ShooterEnemy : Enemy
    {
        private Animator anim = null;
        private bool animationEnded = false;

        [SerializeField] private Gun gun = null;

         
        public Gun Gun { get => gun; }

        private void Start()
        {
            canDie = true;

            anim = GetComponentInChildren<Animator>();
        }

        protected override void Update()
        {
            base.Update();

            if (lookingAtTarget && animationEnded)
            {
                gun.Shoot();
            }

            if (!IsTargetForward())
            {
                lookingAtTarget = false;
            }
        }

        protected override void OnDetectedPlayer()
        {
            anim.SetTrigger("Player Detected");
            animationEnded = false;
        }

        public override void Die()
        {
            gun.ResetStats();
            base.Die();
        }

        public void SetAnimationEnded()
        {
            animationEnded = true;
        }
    }
}
