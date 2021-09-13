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
        private Gun initialGun = null;

        [SerializeField] private int initialLives = 5;
        [SerializeField] private int lives = 5;
        [SerializeField] private float inmuneTime = 2f;
        [SerializeField] private Gun gun = null;
        [SerializeField] private bool controlActive = true;

        public Action OnDie = null;
        public Action<int> OnLivesChanged = null;

        public Gun InitialGun => initialGun;
        public int InitialLives => initialLives;
        public Gun Gun { get => gun; set => gun = value; }
        public bool ControlActive { set => controlActive = value; }

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

            ResetGun();
        }

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            normalColor = spriteRenderer.color;
            initialPosition = transform.position;
            initialGun = gun;
        }

        public void PlayerUpdate()
        {
            if (!controlActive)
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.K))
            {
                gun.Shoot();
            }
        }

        private IEnumerator SetInmuneLifetime(float time)
        {
            isInmune = true;

            spriteRenderer.color = inmuneColor;

            yield return new WaitForSeconds(time);
            
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

            setInmuneLifetimeInstance = SetInmuneLifetime(inmuneTime);
            StartCoroutine(setInmuneLifetimeInstance);
        }

        public void Die()
        {
            OnDie?.Invoke();
        }

        public void AddLife()
        {
            lives++;
            lives = Mathf.Clamp(lives, 1, initialLives);

            OnLivesChanged?.Invoke(lives);
        }

        public void SetInmuneForTime(float time)
        {
            if (setInmuneLifetimeInstance != null)
            {
                StopCoroutine(setInmuneLifetimeInstance);
            }

            setInmuneLifetimeInstance = SetInmuneLifetime(time);
            StartCoroutine(setInmuneLifetimeInstance);
        }

        public void ResetGun()
        {
            gun = initialGun;
            gun.enabled = true;
            gun.GetComponentInChildren<SpriteRenderer>().enabled = true;
        }
    }
}
