using System;
using System.Collections;
using UnityEngine;

using Games.Generics.Interfaces;

namespace EndlessT4cos.Gameplay.Player
{
    public class Player : MonoBehaviour, IDamageable
    {
        [SerializeField] private int lives = 5;
        [SerializeField] private int score = 0;
        [SerializeField] private float inmuneTime = 2f;

        private bool isInmune = false;
        public Action OnDie = null;

        private IEnumerator SetInmuneLifetime()
        {
            isInmune = true;

            yield return new WaitForSeconds(inmuneTime);
            
            isInmune = false;

            yield return null;
        }

        public void TakeDamage()
        {
            if (isInmune)
            {
                return;
            }

            lives--;

            if (lives == 0)
            {
                Die();
            }

            StartCoroutine(SetInmuneLifetime());
        }

        public void Die()
        {
            OnDie?.Invoke();
        }
    }
}
