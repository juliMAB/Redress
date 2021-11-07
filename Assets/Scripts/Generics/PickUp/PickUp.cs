using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Redress.Gameplay.Platforms;
using Redress.Gameplay.User;

namespace Redress.Gameplay.Objects.PickUps
{
    public abstract class PickUp : PlatformObject
    {
        private ParticleSystem lightEffect = null;
        private ParticleSystem pickUpEffect = null;
        protected Player player = null;
        protected float totalDurability = 5f;
        protected float leftDurability = 5f;
        protected bool picked = false;
        protected bool consumed = false;

        public Action<GameObject> OnConsumed = null;
        public Action<GameObject> OnPicked = null;

        public Player Player { set => player = value; }
        public bool Picked { get => picked; }

        protected virtual void Awake()
        {
            //lightEffect = GetComponentInChildren<ParticleSystem>();
            lightEffect = GetComponentsInChildren<ParticleSystem>()[0];
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
                lightEffect.gameObject.SetActive(false);

                if (!lightEffect)
                {
                    Debug.Log(name + " doesn't have particles");
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
          // if (!particles)
          // {
          //     Debug.Log(name + " doesn't has particles");
          //     particles = GetComponentInChildren<ParticleSystem>();
          // }
          // else
          // {
                lightEffect.gameObject.SetActive(true);

          //  }
            leftDurability = totalDurability;
            picked = false;
            consumed = false;
            direction = Vector3.left;
        }
    }
}           
