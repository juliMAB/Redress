using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EndlessT4cos.Gameplay.Controllers
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private GameObject background = null;

        public void MoveCamera(float movement)
        {
            Camera.main.transform.position += Vector3.up * movement;
            background.transform.position += Vector3.up * movement;
        }

        public void Reset()
        {
            Camera.main.transform.position = Vector3.zero;
            background.transform.position = Vector3.zero;
        }
    }
}
