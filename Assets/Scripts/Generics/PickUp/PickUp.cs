﻿using System;
using System.Collections.Generic;
using UnityEngine;

using EndlessT4cos.Gameplay.Platforms;
using EndlessT4cos.Gameplay.User;

namespace EndlessT4cos.Gameplay.Objects.PickUps
{
    public class PickUp : PlatformObject
    {
        protected Player player = null;
        protected float totalDurability = 5f;
        protected float leftDurability = 5f;
        protected bool picked = false;
        protected bool consumed = false;

        public Action<GameObject> OnConsumed = null;

        public Player Player { set => player = value; }

        protected virtual void Update()
        {
            if (consumed)
            {
                return;
            }

            if (picked)
            {
                leftDurability -= Time.deltaTime;

                if (leftDurability < 0)
                {
                    consumed = true;
                    OnConsumed?.Invoke(gameObject);
                }
            }
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            OnPicked();
        }

        protected virtual void OnPicked()
        {
            picked = true;
        }

        public void ResetStats()
        {
            leftDurability = totalDurability;
            picked = false;
            consumed = false;
        }
    }
}           