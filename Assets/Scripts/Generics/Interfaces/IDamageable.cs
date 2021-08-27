using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Games.Generics.Interfaces
{
    public interface IDamageable
    {
        void TakeDamage();

        void Die();        
    }
}