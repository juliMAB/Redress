using GuilleUtils.PostProcessing;
using System;
using UnityEngine;

namespace Redress.Gameplay.Objects.PickUps
{
    public class SlowMotion : PickUp
    {
        [SerializeField] private float multiplier = 0.2f;

        [Header("Visual Configuration")]
        [SerializeField] private float lensDistortionValue = -62;
        [SerializeField] private float chromaticAberrationValue = 1;
        [SerializeField] private float vignetteValue = 0.619f;

        public Action<float> OnSpeedPercentageChanged = null;
        //[SerializeField] AK.Wwise.Event EndSoundSlowMotion;

        protected override void Awake()
        {
            base.Awake();
            OnConsumed += ResetSpeedMultiplier;
        }

        protected override void OnPickedUp()
        {
            const float durationTime = 1;
            const float timeOffSet = 0.1f;

            base.OnPickedUp();
            OnSpeedPercentageChanged?.Invoke(multiplier);
            visual.SetActive(false);

            PostProcessEffectsManager.Instance.SetLensDistortion(lensDistortionValue, durationTime, true, totalDurability - durationTime - timeOffSet);
            PostProcessEffectsManager.Instance.SetChromaticAberration(chromaticAberrationValue, durationTime, true, totalDurability - durationTime - timeOffSet, true);
            PostProcessEffectsManager.Instance.SetVignette(vignetteValue, durationTime, true, totalDurability - durationTime - timeOffSet, true);
        }

        protected override void OnEndPickUp()
        {
            //EndSoundSlowMotion.Post(gameObject);
            base.OnEndPickUp();
        }
        private void ResetSpeedMultiplier(GameObject go)
        {
            OnSpeedPercentageChanged?.Invoke(1);
        }

        public override void ResetStats()
        {
            base.ResetStats();
            visual.SetActive(true);
        }
    }
}