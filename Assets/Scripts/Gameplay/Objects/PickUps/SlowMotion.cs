using UnityEngine;

using Redress.Gameplay.Management;

namespace Redress.Gameplay.Objects.PickUps
{
    public class SlowMotion : PickUp
    {
        [SerializeField] private float multiplier = 0.2f;

        protected override void Awake()
        {
            base.Awake();
            OnConsumed += ResetSpeedMultiplier;
        }

        protected override void OnPickedUp()
        {
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