﻿using System.Collections;
using UnityEngine;

using Redress.Gameplay.Platforms;
using Games.Generics.Interfaces;

namespace Redress.Gameplay.Objects.Enemies
{
    public class PassingEnemy : MonoBehaviour
    {
        private bool activeEnemy = false;
        private bool movementPaused = false;
        private IEnumerator activateEnemyInst = null;
        private float time = 0f;
        private float outOfScreenXValue = 0f;
        private float[] yPositions;

        [Header("Configuration")]
        [SerializeField] private GameObject alert = null;
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

            activeEnemy = false;
            waitTime = Random.Range(waitTimeLimits[0], waitTimeLimits[1]);
            time = 0f;

            alert.GetComponent<Animator>().enabled = false;
            alert.SetActive(false);

            gameObject.SetActive(false);

            transform.position = new Vector3(outOfScreenXValue + halfSize.x * 2 , 0, 0);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject == target)
            {
                IDamageable targetIDamageable = target.GetComponent<IDamageable>();
                targetIDamageable.TakeDamage();
            }
        }

        public void SetOutOfScreenXValue(float value)
        {
            outOfScreenXValue = value;
        }

        public void Pause(bool pause)
        {
            movementPaused = pause;
        }

        public void ActivateEnemy(Row row)
        {
            IEnumerator AlertView()
            {
                activeEnemy = true;
                float time = 0f;

                alert.SetActive(true);
                Animator animator = alert.GetComponent<Animator>();
                animator.enabled = true;

                while (time < alertDuration)
                {
                    if (!movementPaused)
                    {
                        Vector3 position = alert.transform.position;
                        position.y = yPositions[(int)row];
                        alert.transform.position = position;

                        time += Time.deltaTime;
                    }
                    
                    yield return null;
                }

                animator.enabled = false;
                alert.SetActive(false);

                activateEnemyInst = PassByRow();
                StartCoroutine(activateEnemyInst);
            }

            IEnumerator PassByRow()
            {
                while (transform.position.x + halfSize.x > -outOfScreenXValue)
                {
                    if (!movementPaused)
                    {
                        Vector3 position = transform.position;
                        position.y = yPositions[(int)row];
                        transform.position = position;

                        transform.position += Vector3.left * speed * Time.deltaTime;
                    }
                   
                    yield return null;
                }

                Reset();
            }

            if (activateEnemyInst != null)
            {
                StopCoroutine(activateEnemyInst);
            }

            activateEnemyInst = AlertView();
            StartCoroutine(activateEnemyInst);
        }

        public void UpdatePassingEnemy(float[] yPositions)
        {
            if (activeEnemy)
            {
                return;
            }

            this.yPositions = yPositions;

            time += Time.deltaTime;

            if (time > waitTime)
            {
                gameObject.SetActive(true);
                ActivateEnemy((Row)Random.Range(0, 3));
            }
        }
    }
}