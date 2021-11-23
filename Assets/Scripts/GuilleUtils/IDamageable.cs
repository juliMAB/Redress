using UnityEngine;

namespace GuilleUtils.Interfaces
{
    public interface IDamageable
    {
        void TakeDamage(Vector3 origin);

        bool StayAlive();

        void Die();        
    }
}