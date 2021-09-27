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
        public Color inmuneColor = Color.red;
       // public Color inmuneColorShield = Color.blue;
        private bool isInmune = false;
        private Vector3 initialPosition = Vector3.zero;
        private IEnumerator setInmuneLifetimeInstance = null;
        private IEnumerator varyBetweenColorsInstance = null;
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
        public int Lives => lives;
        public Gun Gun { get => gun; set => gun = value; }
        public bool ControlActive { set => controlActive = value; }
        
        public void Reset()
        {
            if (setInmuneLifetimeInstance != null)
            {
                StopCoroutine(setInmuneLifetimeInstance);
            }

            if (varyBetweenColorsInstance != null)
            {
                StopCoroutine(varyBetweenColorsInstance);
            }

            transform.position = initialPosition;
            lives = initialLives;
            isInmune = false;
            spriteRenderer.color = normalColor;

            OnLivesChanged?.Invoke(lives);

            ResetGun();

           transform.rotation = Quaternion.identity;
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

        private IEnumerator SetInmuneLifetime(float duration)
        {
            Color inmuneColor;
            isInmune = true;
            float time = 0;

            while (time < duration)
            {
                time += Time.deltaTime;
                yield return null;
            }
            
            isInmune = false;
        }

        private IEnumerator VaryBetweenColors(float duration, Color inmuneColor)
        {
            float time = 0;
            bool isInmuneColor = true;

            spriteRenderer.color = inmuneColor;

            while (time < duration)
            {
                time += Time.deltaTime;

                if (time % 0.4f < Time.deltaTime)
                {
                    if (isInmuneColor)
                    {
                        spriteRenderer.color = normalColor;
                    }
                    else
                    {
                        spriteRenderer.color = inmuneColor;
                    }

                    isInmuneColor = !isInmuneColor;
                }

                yield return null;
            }

            spriteRenderer.color = normalColor;
        }

        public void TakeDamage()
        {
            if (isInmune)
            {
                Debug.Log(" el player es inmune ");
                return;
            }
            Debug.Log(" el player a tomado daño ");
            lives--;
            OnLivesChanged?.Invoke(lives);

            if (lives == 0)
            {
                Die();
            }

            SetInmuneForTime(inmuneTime, inmuneColor);
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

        public void SetInmuneForTime(float time, Color inmuneColor)
        {
            if (setInmuneLifetimeInstance != null)
            {
                StopCoroutine(setInmuneLifetimeInstance);
            }

            setInmuneLifetimeInstance = SetInmuneLifetime(time);
            StartCoroutine(setInmuneLifetimeInstance);

            if (varyBetweenColorsInstance != null)
            {
                StopCoroutine(varyBetweenColorsInstance);
            }

            varyBetweenColorsInstance = VaryBetweenColors(time, inmuneColor);
            StartCoroutine(varyBetweenColorsInstance);
        }

        public void ResetGun()
        {
            gun = initialGun;
            gun.enabled = true;
            gun.GetComponentInChildren<SpriteRenderer>().enabled = true;
        }
    }
}
