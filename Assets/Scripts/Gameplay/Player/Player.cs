using System;
using System.Collections;
using UnityEngine;

using Games.Generics.Interfaces;
using Games.Generics.Weapon;
using Games.Generics.Character.Movement;
namespace Redress.Gameplay.User
{
    public class Player : MonoBehaviour, IDamageable
    {
        private SpriteRenderer spriteRenderer = null;
        private CharacterMovementSeter characterMovement = null;
        private Color normalColor = Color.white;
        private Color inmuneColor = Color.red;
        private bool isInmune = false;
        private Vector3 initialPosition = Vector3.zero;
        private Gun initialGun = null;

        [SerializeField] private int initialLives = 5;
        [SerializeField] private int lives = 5;
        [SerializeField] private float inmuneTime = 2f;
        [SerializeField] private float force;
        private ParticleSystem particleSystem= null;
        private CharacterController2D controller2D;
        private Gun gun = null;
        private bool controlActive = true;

        public Action OnDie = null;
        public Action<int> OnLivesChanged = null;

        public Gun InitialGun => initialGun;
        public int InitialLives => initialLives;
        public int Lives => lives;
        public Gun Gun { get => gun; set => gun = value; }
        public bool ControlActive { set => controlActive = value; }
        public Color InmuneColor { set => inmuneColor = value; }

        private void Awake()
        {
            particleSystem = GetComponentInChildren<ParticleSystem>();
            characterMovement = GetComponent<CharacterMovementSeter>();
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            normalColor = spriteRenderer.color;
            initialPosition = transform.position;
            gun = GetComponentInChildren<Gun>();
            initialGun = gun;
            controller2D = GetComponent<CharacterController2D>();
        }

        public void PlayerUpdate()
        {
            if (!controlActive)
            {
                return;
            }
            //characterMovement.CharacterMovementSeterUpdate();
            if (Input.GetKeyDown(KeyCode.K))
            {
                gun.Shoot();
            }
        }
        private void FixedUpdate()
        {
            if (!controlActive)
                return;
            
            characterMovement.CharacterMovementSeterUpdate();
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
            particleSystem.Play();
            if (isInmune)
            {
                Debug.Log(" el player es inmune ");
                return;
            }
            Debug.Log(" el player a tomado daño ");
            lives--;
            
            if (lives > 0)
            {
                AkSoundEngine.PostEvent(SoundsManager.Get().Daño, gameObject);
            }
            OnLivesChanged?.Invoke(lives);

            if (lives == 0)
            {
                AkSoundEngine.PostEvent(SoundsManager.Get().Muerte, gameObject);
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
            StopAllCoroutines();
            StartCoroutine(SetInmuneLifetime(time));
            StartCoroutine(VaryBetweenColors(time, inmuneColor));
        }

        public void ResetGun()
        {
            gun = initialGun;
            gun.enabled = true;
            gun.GetComponentInChildren<SpriteRenderer>().enabled = true;
        }

        public void TakeDamage(Vector3 other)
        {
            particleSystem.Play();
            if (isInmune)
            {
                Debug.Log(" el player es inmune ");
                return;
            }
            Debug.Log(" el player a tomado daño ");
            lives--;
            controller2D.move((other-transform.position)* force);
            if (lives > 0)
            {
                AkSoundEngine.PostEvent(SoundsManager.Get().Daño, gameObject);
            }
            OnLivesChanged?.Invoke(lives);

            if (lives == 0)
            {
                AkSoundEngine.PostEvent(SoundsManager.Get().Muerte, gameObject);
                Die();
            }

            SetInmuneForTime(inmuneTime, inmuneColor);
        }
    }
}
