using System;
using System.Collections;
using UnityEngine;

using Games.Generics.Interfaces;
using Games.Generics.Weapon;

namespace EndlessT4cos.Gameplay.User
{
    public class Player : MonoBehaviour, IDamageable
    {
        [SerializeField] private int initialLives = 5;
        [SerializeField] private int lives = 5;
        [SerializeField] private float inmuneTime = 2f;
        [SerializeField] private Gun gun = null;

        private SpriteRenderer spriteRenderer = null;
        private Color normalColor = Color.white;
        private Color inmuneColor = Color.red;
        private bool isInmune = false;
        private Vector3 initialPosition = Vector3.zero;

        public Action OnDie = null;
        public Action<int> OnLivesChanged = null;
        
        public void Reset()
        {
            transform.position = initialPosition;
            lives = initialLives;
            OnLivesChanged?.Invoke(lives);
        }

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            normalColor = spriteRenderer.color;
            initialPosition = transform.position;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                gun.Shoot();
            }
        }

        private IEnumerator SetInmuneLifetime()
        {
            isInmune = true;

            spriteRenderer.color = inmuneColor;

            yield return new WaitForSeconds(inmuneTime);
            
            isInmune = false;

            spriteRenderer.color = normalColor;

            yield return null;
        }

        public void TakeDamage()
        {
            if (isInmune)
            {
                return;
            }

            lives--;
            OnLivesChanged?.Invoke(lives);

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
