using System;
using System.Collections;
using UnityEngine;

using Games.Generics.Interfaces;
using Games.Generics.Weapon;

namespace EndlessT4cos.Gameplay.User
{
    public class Player : MonoBehaviour, IDamageable
    {
        private SpriteRenderer spriteRenderer = null;
        private Color normalColor = Color.white;
        private Color inmuneColor = Color.red;
        private bool isInmune = false;
        private Vector3 initialPosition = Vector3.zero;
        private IEnumerator setInmuneLifetimeInstance = null;

        [SerializeField] private int initialLives = 5;
        [SerializeField] private int lives = 5;
        [SerializeField] private float inmuneTime = 2f;
        [SerializeField] private Gun gun = null;        

        public Action OnDie = null;
        public Action<int> OnLivesChanged = null;

        public int InitialLives { get => initialLives; }

        public void Reset()
        {
            if (setInmuneLifetimeInstance != null)
            {
                StopCoroutine(setInmuneLifetimeInstance);
            }

            transform.position = initialPosition;
            lives = initialLives;
            isInmune = false;
            spriteRenderer.color = normalColor;

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

            setInmuneLifetimeInstance = SetInmuneLifetime();
            StartCoroutine(setInmuneLifetimeInstance);
        }

        public void Die()
        {
            OnDie?.Invoke();
        }
    }
}
