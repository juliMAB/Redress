using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EndlessT4cos.Gameplay.Animations
{
    public class AnimationController : MonoBehaviour
    {
        private List<IEnumerator> animationInstances = new List<IEnumerator>();
        private Vector3[] initialHeartsScale = null;
        [SerializeField] public Animator[] animationControllers = null;
        [SerializeField] private GameObject[] hearts = null;
        [SerializeField] private float speed = 2f;

        private bool animationsOn = true;

        public void StartAnimations()
        {
            initialHeartsScale = new Vector3[hearts.Length];
            for (int i = 0; i < hearts.Length; i++)
            {
                IEnumerator animInstance = ChangeObjectSize(hearts[i], 0.2f, 0.2f);
                initialHeartsScale[i] = hearts[i].transform.localScale;
                StartCoroutine(animInstance);
                animationInstances.Add(animInstance);
            }
        }

        public void PauseAnimations()
        {
            animationsOn = false;
            foreach (var item in animationControllers)
            {
                item.enabled = false;
            }
        }

        public void ReanudeAnimations()
        {
            animationsOn = true;
            foreach (var item in animationControllers)
            {
                item.enabled = true;
            }
        }

        public void Reset()
        {
            ReanudeAnimations();
            for (int i = 0; i < hearts.Length; i++)
            {
                hearts[i].transform.localScale = initialHeartsScale[i];
            }

            animationsOn = true;

            for (int i = 0; i < animationInstances.Count; i++)
            {
                StopCoroutine(animationInstances[i]);
            }

            animationInstances.Clear();

            StartAnimations();
        }

        private IEnumerator ChangeObjectSize(GameObject gameObject, float sizeDownGrade, float sizeUpgrade)
        {
            bool gettingSmaller = false;
            Vector2 initialScale = gameObject.transform.localScale;

            while (animationsOn)
            {
                Vector3 verticalScale = gettingSmaller ? Vector3.down : Vector3.up;
                Vector3 horizontalScale = gettingSmaller ? Vector3.left : Vector3.right;

                gameObject.transform.localScale += verticalScale * Time.deltaTime * speed + horizontalScale * Time.deltaTime * speed;

                if (gettingSmaller)
                {
                    if (gameObject.transform.localScale.x <= initialScale.x - sizeDownGrade)
                    {
                        gettingSmaller = false;
                    }
                }
                else
                {
                    if (gameObject.transform.localScale.x >= initialScale.x + sizeUpgrade)
                    {
                        gettingSmaller = true;
                    }
                }

                yield return null;
            }
        }
    }
}
