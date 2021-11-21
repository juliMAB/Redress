using System;
using UnityEngine;

using GuilleUtils.Displacement;
using Redress.Gameplay.User;

namespace Redress.Gameplay.Objects.PickUps
{
    public abstract class PickUp : MovableObject
    {
        private ParticleSystem lightEffect = null;
        protected GameObject visual = null;
        protected Player player = null;
        [SerializeField] protected float totalDurability = 5f;

        public Action<GameObject> OnConsumed = null;

        public Player Player { set => player = value; }

        protected virtual void Awake()
        {
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
