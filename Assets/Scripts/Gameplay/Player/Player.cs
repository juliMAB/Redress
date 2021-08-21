using System;
using System.Collections;
using UnityEngine;

using Games.Generics.Interfaces;
using Games.Generics.Weapon;

namespace EndlessT4cos.Gameplay.User
{
    public class Player : MonoBehaviour, IDamageable
    {
        [SerializeField] private int lives = 5;
        [SerializeField] private float inmuneTime = 2f;
        [SerializeField] private Gun gun = null;

        private SpriteRenderer spriteRenderer;
        private Color normalColor;
        private Color inmortalColor=Color.red;
        private bool isInmune = false;
        public Action OnDie = null;
        public Action<int> OnLivesChanged = null;
        private Vector3 initialPos;
        public void Reset()
        {
            transform.position = initialPos;
            lives = 5;

        }

        private void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            normalColor = spriteRenderer.color;
            initialPos = transform.position;
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

            spriteRenderer.color = inmortalColor;

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

            if (lives == 0)
            {
                Die();
            }

            StartCoroutine(SetInmuneLifetime());

            OnLivesChanged?.Invoke(lives);
        }

        public void Die()
        {
            OnDie?.Invoke();
        }
    }
}
