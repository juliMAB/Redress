using UnityEngine;

using Games.Generics.Weapon;

namespace Redress.Gameplay.Objects.Enemies
{
    public class ShooterEnemy : Enemy
    {
        [SerializeField] private Gun gun = null;

         
        public Gun Gun { get => gun; }

        private void Start()
        {
            canDie = true;
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
                lookingAtTarget = false;
            }
        }

        public override void Die()
        {
            gun.ResetStats();
            base.Die();
        }
    }
}
