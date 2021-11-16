using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Games.Generics.Animations
{
    public class Scale : MonoBehaviour
    {
        private Vector3 scale = Vector3.zero;

        [SerializeField] private bool scalingBig = true;
        [SerializeField] private float maxScaleValue = 2;
        [SerializeField] private float minScaleValue = 0.5f;
        [SerializeField] private float speed = 5;

        private void Awake()
        {
            scale = transform.localScale;
        }

        private void Update()
        {
            ScaleBothAxis(scalingBig ? -Time.deltaTime * speed : Time.deltaTime * speed);

            transform.localScale = scale;

            if (scale.x > maxScaleValue || scale.x < minScaleValue)
            {
                scalingBig = !scalingBig;
            }
        }

        private void ScaleBothAxis(float addedValue)
        {
            scale.x += addedValue;
            scale.y += addedValue;
        }
    }
}
