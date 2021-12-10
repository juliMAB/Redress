using System;
using UnityEngine;

using GuilleUtils.Displacement;
using Redress.Gameplay.User;

namespace Redress.Gameplay.Objects.PickUps
{
    public abstract class PickUp : MovableObject
    {
        [SerializeField] private ParticleSystem lightEffect = null;
        [SerializeField] private ParticleSystem grabEffect = null;
        protected GameObject visual = null;
        protected Player player = null;
        [SerializeField] protected float totalDurability = 5f;
        private BoxCollider2D boxCollider;

        public Action<GameObject> OnConsumed = null;

        public Player Player { set => player = value; }

        [SerializeField] AK.Wwise.Event soundPickUp;

        protected virtual void Awake()
        {
            boxCollider = GetComponent<BoxCollider2D>();
            visual = GetComponentInChildren<Renderer>().gameObject;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            lightEffect.gameObject.SetActive(false);
            direction = Vector3.zero;
            OnPickedUp();
            Invoke(nameof(OnEndPickUp), totalDurability);
        }

        protected virtual void OnPickedUp()
        {
            soundPickUp.Post(gameObject);
            boxCollider.enabled = false;
            grabEffect.Play();

            

            Invoke("StopParticles", 0.5f);
        }

        private void StopParticles()
        {
            grabEffect.Stop(false, ParticleSystemStopBehavior.StopEmitting);
        }

        public virtual void ResetStats()
        {
            lightEffect.gameObject.SetActive(true);
            direction = Vector3.left;
            visual.gameObject.SetActive(true);
            boxCollider.enabled = true;
        }

        protected virtual void OnEndPickUp()
        {
            OnConsumed?.Invoke(gameObject);
        }
    }
}           
