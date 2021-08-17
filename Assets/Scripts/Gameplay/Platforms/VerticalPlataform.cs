using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EndlessT4cos.Gameplay.Platforms
{
    public class VerticalPlataform : MonoBehaviour
    {
        private Collider2D Collider;

        private void Start()
        {
            Collider = GetComponent<Collider2D>();
        }

        private void Update()
        {
            if (Input.GetButton("Crouch"))
            {
                Collider.isTrigger = true;
            }
            else
            {
                Collider.isTrigger = false;
            }
        }
    }
}


