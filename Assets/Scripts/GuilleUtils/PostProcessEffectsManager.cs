using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;


namespace GuilleUtils.PostProcessing
{
    public class PostProcessEffectsManager : MonoBehaviour
    {
#region Singleton
        private static PostProcessEffectsManager instance = null;
        public static PostProcessEffectsManager Instance => instance; 
        private void Awake()
        {
            instance = this;
            chromatic = (ChromaticAberration)volume.profile.settings[1];
            //depthOfField = volume.GetComponent<DepthOfField>();
            //motionBlur = volume.GetComponent<MotionBlur>(); 
            lensDistortion = (LensDistortion)volume.profile.settings[2];
            vignette = (Vignette)bgVolume.profile.settings[4];
            bgChromatic = (ChromaticAberration)bgVolume.profile.settings[1];
            bgLensDistortion = (LensDistortion)bgVolume.profile.settings[2];
            bgVignette = (Vignette)bgVolume.profile.settings[4];
        }
#endregion

        [Header("PostProcessing")]
        [SerializeField] private PostProcessVolume volume = null;
        [SerializeField] private PostProcessVolume bgVolume = null;

        private ChromaticAberration chromatic = null;
        private DepthOfField depthOfField = null;
        private MotionBlur motionBlur = null;
        private LensDistortion lensDistortion = null;
        private Vignette vignette = null;
        private ChromaticAberration bgChromatic = null;
        private DepthOfField bgDepthOfField = null;
        private MotionBlur bgMotionBlur = null;
        private LensDistortion bgLensDistortion = null;
        private Vignette bgVignette = null;

        public void SetLensDistortion(float value, float duration, bool rebounce = false, float holdTime = 0, bool justBG = false, bool affectBG = false)
        {
            if (justBG)
            {
                StartCoroutine(SetLensDistortion(value, duration, rebounce, holdTime, bgLensDistortion));
            }
            else
            {
                StartCoroutine(SetLensDistortion(value, duration, rebounce, holdTime, lensDistortion));
                if (!affectBG)
                {
                    StartCoroutine(SetLensDistortion(-value, duration, rebounce, holdTime, bgLensDistortion));
                }
            }
        }

        public void SetChromaticAberration(float value, float duration, bool rebounce = false, float holdTime = 0, bool justBG = false)
        {
            if (justBG)
            {
                StartCoroutine(SetChromaticAberration(value, duration, rebounce, holdTime, bgChromatic));
            }
            else
            {
                StartCoroutine(SetChromaticAberration(value, duration, rebounce, holdTime, chromatic));
            }
        }

        public void SetVignette(float value, float duration, bool rebounce = false, float holdTime = 0, bool justBG = false)
        {
            if (justBG)
            {
                StartCoroutine(SetVignette(value, duration, rebounce, holdTime, bgVignette));
            }
            else
            {
                StartCoroutine(SetVignette(value, duration, rebounce, holdTime, vignette));
            }
        }

        private IEnumerator SetLensDistortion(float _value, float _duration, bool _rebounce, float _holdTime, LensDistortion ld)
        {
            float time = 0;
            float initialValue = ld.intensity.value;

            while (time < _duration)
            {
                time += Time.deltaTime;

                ld.intensity.value = Mathf.Lerp(initialValue, _value, time / _duration);

                yield return null;
            }

            if (_rebounce)
            {
                yield return new WaitForSeconds(_holdTime);

                StartCoroutine(SetLensDistortion(initialValue, _duration, false, 0, ld));
            }
        }

        private IEnumerator SetChromaticAberration(float _value, float _duration, bool _rebounce, float _holdTime, ChromaticAberration c)
        {
            float time = 0;
            float initialValue = c.intensity.value;

            while (time < _duration)
            {
                time += Time.deltaTime;

                c.intensity.value = Mathf.Lerp(initialValue, _value, time / _duration);

                yield return null;
            }

            if (_rebounce)
            {
                yield return new WaitForSeconds(_holdTime);

                StartCoroutine(SetChromaticAberration(initialValue, _duration, false, 0, c));
            }
        }

        private IEnumerator SetVignette(float _value, float _duration, bool _rebounce, float _holdTime, Vignette v)
        {
            float time = 0;
            float initialValue = v.intensity.value;

            while (time < _duration)
            {
                time += Time.deltaTime;

                v.intensity.value = Mathf.Lerp(initialValue, _value, time / _duration);

                yield return null;
            }

            if (_rebounce)
            {
                yield return new WaitForSeconds(_holdTime);

                StartCoroutine(SetVignette(initialValue, _duration, false, 0, v));
            }
        }




        public IEnumerator Blur()
        {
            float time = 0f;

            while (time < 1f)
            {
                time += Time.deltaTime;

                depthOfField.focalLength.value = time * 100 * 1.5f;
                lensDistortion.intensity.value = -time * 0.3f;
                yield return null;
            }
        }
    }
}


