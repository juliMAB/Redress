using UnityEngine;
using System;

namespace Redress.Gameplay.Objects.PickUps
{
    public class SlowMotion : PickUp
    {
        [SerializeField] private float multiplier = 0.2f;

        public Action<float> OnSpeedPercentageChanged = null;

        protected override void Awake()
        {
            base.Awake();
            OnConsumed += ResetSpeedMultiplier;
        }

        protected override void OnPickedUp()
        {
            OnSpeedPercentageChanged?.Invoke(multiplier);
            //GameplayManager.Instance.speedMultiplier = multiplier;
            visual.SetActive(false);
        }

        private void ResetSpeedMultiplier(GameObject go)
        {
            OnSpeedPercentageChanged?.Invoke(1);
           // GameplayManager.Instance.speedMultiplier = 1f;
        }

        public override void ResetStats()
        {
            base.ResetStats();
            visual.SetActive(true);
        }
    }
}