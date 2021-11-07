using System;
using System.Collections.Generic;
using UnityEngine;

using EndlessT4cos.Gameplay.Platforms;
using EndlessT4cos.Gameplay.User;

namespace EndlessT4cos.Gameplay.Objects.PickUps
{
    public abstract class PickUp : PlatformObject
    {
        private ParticleSystem particles = null;
        protected Player player = null;
        protected float totalDurability = 5f;
        protected float leftDurability = 5f;
        protected bool picked = false;
        protected bool consumed = false;

        public Action<GameObject> OnConsumed = null;
        public Action<GameObject> OnPicked = null;

        public Player Player { set => player = value; }
        public bool Picked { get => picked; }

        protected virtual void Start()
        {
            particles = GetComponentInChildren<ParticleSystem>();
            
        }

        protected virtual void Update()
        {
            if (consumed)
            {
                Debug.Log("consumed: " + consumed);
                return;
            }

            if (picked)
            {
                particles.gameObject.SetActive(false);

                if (!particles)
                {
                    Debug.Log(name + " doesn't has particles");
                }

                if (leftDurability < 0)
                {
                    consumed = true;
                    OnConsumed?.Invoke(gameObject);
                }

                leftDurability -= Time.deltaTime;
            }
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            picked = true;
            direction = Vector3.zero;
            OnPicked?.Invoke(gameObject);
            OnPickedUp();
        }

        protected abstract void OnPickedUp();

        public virtual void ResetStats()
        {
            if (!particles)
            {
                Debug.Log(name + " doesn't has particles");
                particles = GetComponentInChildren<ParticleSystem>();
            }
            else
            {
                particles.gameObject.SetActive(true);

            }
            leftDurability = totalDurability;
            picked = false;
            consumed = false;
            direction = Vector3.left;
        }
    }
}           
