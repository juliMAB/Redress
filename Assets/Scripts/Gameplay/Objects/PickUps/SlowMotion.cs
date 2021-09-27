using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using EndlessT4cos.Gameplay.Management;

namespace EndlessT4cos.Gameplay.Objects.PickUps
{
    public class SlowMotion : PickUp
    {
        [SerializeField] private GameObject visual = null;
        [SerializeField] private float multiplier = 0.2f;

        private void Start()
        {
            totalDurability = 5f;
            leftDurability = 5f;

            OnConsumed += ResetSpeedMultiplier;
        }

        protected override void OnPicked()
        {
            base.OnPicked();

            GameplayManager.Instance.speedMultiplier = multiplier;
            visual.SetActive(false);
        }

        private void ResetSpeedMultiplier(GameObject go)
        {
            GameplayManager.Instance.speedMultiplier = 1f;
        }

        public override void ResetStats()
        {
            base.ResetStats();
            visual.SetActive(true);
        }
    }
}