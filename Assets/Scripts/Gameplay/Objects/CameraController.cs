using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EndlessT4cos.Gameplay.Controllers
{
    public class CameraController : MonoBehaviour
    {
        private IEnumerator positionCameraInst = null;
        private bool pauseMovement = false;

        [SerializeField] private GameObject background = null;
        [SerializeField] private float movementSpeed = 1f;

        public void PositionCamera(float movement, float totalTime)
        {
            Vector3 position = Camera.main.transform.position + Vector3.up * movement;

            IEnumerator MoveCamera()
            {
                Vector3 initialPosition = Camera.main.transform.position;
                float time = 0f;
                while (time < totalTime)
                {
                    if (pauseMovement)
                    {
                        yield return null;
                    }
                    else
                    {
                        time += Time.deltaTime * movementSpeed;

                        Vector3 pos = Vector3.Lerp(initialPosition, position, time / totalTime);

                        Camera.main.transform.position = pos;
                        background.transform.position = pos;

                        yield return null;
                    }
                }

                positionCameraInst = null;
            }

            if (positionCameraInst != null)
            {
                StopCoroutine(positionCameraInst);
            }

            positionCameraInst = MoveCamera();
            StartCoroutine(positionCameraInst);
        }

        public void PauseCameraMovement(bool pause)
        {
            pauseMovement = pause;
        }

        public void Reset()
        {
            Camera.main.transform.position = Vector3.zero;
            background.transform.position = Vector3.zero;
            pauseMovement = false;

            if (positionCameraInst != null)
            {
                StopCoroutine(positionCameraInst);
            }
        }
    }
}
