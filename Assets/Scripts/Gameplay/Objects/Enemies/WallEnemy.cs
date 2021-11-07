using System;
using System.Collections.Generic;
using UnityEngine;

using Games.Generics.Interfaces;

namespace Redress.Gameplay.Objects.Enemies
{
    public class WallEnemy : Enemy
    {

        protected override void Update()
        {
            base.Update();
        }
        private void OnCollisionEnter(Collision collision)
        {
            IDamageable iDamageable = collision.gameObject.GetComponent<IDamageable>();

            if (iDamageable != null)
            {
                iDamageable.TakeDamage();
            }


        }

    }
}
