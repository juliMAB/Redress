﻿using System;
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
        //private ParticleSystem pickUpEffect = null;
        protected GameObject visual = null;
        protected Player player = null;
        [SerializeField] protected float totalDurability = 5f;

        public Action<GameObject> OnConsumed = null;

        public Player Player { set => player = value; }

        protected virtual void Awake()
        {
            //lightEffect = GetComponentInChildren<ParticleSystem>();
            lightEffect = GetComponentsInChildren<ParticleSystem>()[0];
            visual = GetComponentInChildren<Renderer>().gameObject;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            lightEffect.gameObject.SetActive(false);
            direction = Vector3.zero;
            OnPickedUp();
            Invoke(nameof(OnEndPickUp), totalDurability);
        }

        protected abstract void OnPickedUp();

        public virtual void ResetStats()
        {
            lightEffect.gameObject.SetActive(true);
            direction = Vector3.left;
            visual.gameObject.SetActive(true);
        }
        protected virtual void OnEndPickUp()
        {
            OnConsumed?.Invoke(gameObject);
        }
    }
}           
