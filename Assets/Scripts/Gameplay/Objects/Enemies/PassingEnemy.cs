using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using EndlessT4cos.Gameplay.Platforms;
using Games.Generics.Interfaces;

namespace EndlessT4cos.Gameplay.Objects.Enemies
{
    public class PassingEnemy : MonoBehaviour
    {
        private bool activeEnemy = false;
        private bool movementPaused = false;
        private IEnumerator activateEnemyInst = null;

        [Header("Configuration")]
        [SerializeField] private Vector2 halfSize = Vector2.zero;
        [SerializeField] private float[] waitTimeLimits = null;
        [SerializeField] private float waitTime = 0;
        [SerializeField] private float speed = 0f;
        [SerializeField] private GameObject target = null;

        public float alertDuration = 0f;

        public void Reset()
        {
            if (activateEnemyInst != null)
            {
                StopCoroutine(activateEnemyInst);
            }

            waitTime = Random.Range(waitTimeLimits[0], waitTimeLimits[1]);
        }

        public void Pause(bool pause)
        {
            movementPaused = pause;
        }

        public void ActivateEnemy(Row row, float outOfScreenXValue)
        {
            IEnumerator AlertView()
            {
                activeEnemy = true;
                float time = 0f;

                while (time < alertDuration)
                {
                    if (!movementPaused)
                    {
                        time += Time.deltaTime;
                    }
                    
                    yield return null;
                }

                activateEnemyInst = PassByRow();
                StartCoroutine(activateEnemyInst);
            }

            IEnumerator PassByRow()
            {
                while (transform.position.x + halfSize.x > outOfScreenXValue)
                {
                    if (!movementPaused)
                    {
                        transform.position += Vector3.left * speed * Time.deltaTime;
                    }
                   
                    yield return null;
                }

                activeEnemy = false;
            }

            if (activateEnemyInst != null)
            {
                StopCoroutine(activateEnemyInst);
            }

            activateEnemyInst = AlertView();
            StartCoroutine(activateEnemyInst);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject == target)
            {
                IDamageable targetIDamageable = target.GetComponent<IDamageable>();
                targetIDamageable.TakeDamage();
            }
        }
    }
}