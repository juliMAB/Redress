using UnityEngine;

namespace Games.Generics.Interfaces
{
    public interface IDamageable
    {
        void TakeDamage(Vector3 origin);

        void Die();        
    }
}