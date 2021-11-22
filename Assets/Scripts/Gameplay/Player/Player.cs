using System;
using System.Collections;
using UnityEngine;

using GuilleUtils.Interfaces;
using GuilleUtils.Weapon;
using GuilleUtils.Character.Movement;
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
        private ParticleSystem particleSystem = null;
        private Gun gun = null;
        private bool controlActive = true;

        [SerializeField] private int initialLives = 5;
        [SerializeField] private int lives = 5;
        [SerializeField] private float inmuneTime = 2f;
        [SerializeField] private float force;
        
        public Action OnDie = null;
        public Action<int> OnLivesChanged = null;

        public Gun InitialGun => initialGun;
        public int InitialLives => initialLives;
        public int Lives => lives;
        public Gun Gun { get => gun; set => gun = value; }
        public bool ControlActive { set => controlActive = value; }
        public Color InmuneColor { set => inmuneColor = value; }
        public CharacterMovementSeter CharacterMovement => characterMovement;

        [SerializeField] AK.Wwise.Event soundDamage;
        [SerializeField] AK.Wwise.Event soundDie;

        private void Awake()
        {
            particleSystem = GetComponentInChildren<ParticleSystem>();
            characterMovement = GetComponent<CharacterMovementSeter>();
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            normalColor = spriteRenderer.color;
            initialPosition = transform.position;
            gun = GetComponentInChildren<Gun>();
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

        public void TakeDamage(Vector3 origin)
        {
            IEnumerator TakeHit()
            {
                float time = 0;
                Vector3 initialPos = transform.position;
                origin.y = initialPos.y;
                Vector3 dir = (initialPos - origin).normalized;
                float speed = 5;

                while (time < 1)
                {
                    time += Time.deltaTime * speed;
                    initialPos.y = transform.position.y;

                    transform.position = Vector3.Lerp(initialPos, initialPos + dir * 3, time);
                    yield return null;
                }
            }

            particleSystem.Play();
            if (isInmune)
            {
                return;
            }

            lives--;
            
            if (lives > 0)
            {
                soundDamage.Post(gameObject);
            }
            OnLivesChanged?.Invoke(lives);

            if (lives == 0)
            {
                soundDie.Post(gameObject);
                Die();
            }

            SetInmuneForTime(inmuneTime, inmuneColor);
            StartCoroutine(TakeHit());
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
    }
}
