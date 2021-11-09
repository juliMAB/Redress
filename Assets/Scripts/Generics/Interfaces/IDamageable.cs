using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Games.Generics.Interfaces
{
    public interface IDamageable
    {
        void TakeDamage();

        void TakeDamage(Vector3 other);

        void Die();        
    }
}