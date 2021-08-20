using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Games.Generics.Weapon;

namespace EndlessT4cos.Gameplay.Enemies
{
    public class ShooterEnemy : Enemy
    {
        [SerializeField] private Gun gun = null;

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
    }
}
