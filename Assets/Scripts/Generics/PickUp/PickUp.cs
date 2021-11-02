using System;
using System.Collections.Generic;
using UnityEngine;

using EndlessT4cos.Gameplay.Platforms;
using EndlessT4cos.Gameplay.User;

namespace EndlessT4cos.Gameplay.Objects.PickUps
{
    public abstract class PickUp : PlatformObject
    {
        protected Player player = null;
        protected float totalDurability = 5f;
        protected float leftDurability = 5f;
        protected bool picked = false;
        protected bool consumed = false;

        public Action<GameObject> OnConsumed = null;
        public Action<GameObject> OnPicked = null;

        public Player Player { set => player = value; }
        public bool Picked { get => picked; }

        protected virtual void Update()
        {
            if (consumed)
            {
                Debug.Log("consumed: " + consumed);
                return;
            }

            if (picked)
            {
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
            leftDurability = totalDurability;
            picked = false;
            consumed = false;
            direction = Vector3.left;
        }
    }
}           
