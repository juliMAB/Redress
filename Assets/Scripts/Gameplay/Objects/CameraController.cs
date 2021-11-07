using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Redress.Gameplay.Management;

namespace Redress.Gameplay.Controllers
{
    public class CameraController : MonoBehaviour
    {
        private IEnumerator positionCameraInst = null;
        private IEnumerator cameraShakeInst = null;
        private bool pauseMovement = false;
        private bool cameraShakeActive = false;
        private bool movementActive = false;
        private float yInitialPos = 0f;
        private Vector2 initialPos = Vector2.zero;
        private Vector2 cameraShakeDifference = Vector2.zero;

        [Header("Main Configurations")]
        [SerializeField] private GameObject background = null;
        [SerializeField] private float movementSpeed = 1f;
        [SerializeField] private float downSpeedMultiplier = 1f;

        [Header("Camera Shake Configurations")]
        [SerializeField] private float duration = 0.15f;
        [SerializeField] private float magnitude = 0.2f;

        public void PositionCamera(float movement, float totalTime)
        {

            float yFinalPosition = cameraShakeActive ? initialPos.y + movement : Camera.main.transform.position.y + movement;

            IEnumerator MoveCamera()
            {
                float yInitialPosition = cameraShakeActive ? initialPos.y : Camera.main.transform.position.y;
                float time = 0f;

                bool addSpeedDownMultiplier = yFinalPosition < yInitialPosition;
                movementActive = true;

                while (time < totalTime)
                {
                    if (pauseMovement)
                    {
                        yield return null;
                    }
                    else
                    {
                        //normal movement
                        time += Time.deltaTime * movementSpeed * GameplayManager.Instance.speedMultiplier * (addSpeedDownMultiplier ? downSpeedMultiplier : 1);

                        float yPos = Mathf.Lerp(yInitialPosition, yFinalPosition, time / totalTime);

                        Vector3 pos = Camera.main.transform.position;
                        pos.y = yPos + cameraShakeDifference.y;
                        pos.x += cameraShakeDifference.x;

                        if (!cameraShakeActive)
                        {
                            pos.x = 0;
                        }

                        if (time / totalTime > 1)
                        {
                            pos.y = yFinalPosition;
                        }

                        //aplication
                        Camera.main.transform.position = pos;
                        background.transform.position = pos;

                        yield return null;
                    }
                }

                positionCameraInst = null;
                movementActive = false;
            }

            if (positionCameraInst != null)
            {
                StopCoroutine(positionCameraInst);
            }

            positionCameraInst = MoveCamera();
            StartCoroutine(positionCameraInst);
        }

        private IEnumerator Shake(float duration, float magnitude)
        {
            float elapsed = 0.0f;

            initialPos.y = Camera.main.transform.position.y;
            initialPos.x = Camera.main.transform.position.x;

            while (elapsed < duration)
            {
                if (!pauseMovement)
                {
                    cameraShakeDifference.x = UnityEngine.Random.Range(-1f, 1f) * magnitude;
                    cameraShakeDifference.y = UnityEngine.Random.Range(-1f, 1f) * magnitude;

                    if (!movementActive)
                    {
                        Vector3 pos = Camera.main.transform.position;
                        pos.y += cameraShakeDifference.y;
                        pos.x += cameraShakeDifference.x;

                        Camera.main.transform.position = pos;
                        background.transform.position = pos;
                    }

                    elapsed += Time.deltaTime;
                }
                else
                {
                    if (!movementActive)
                    {
                        Camera.main.transform.position = initialPos;
                        background.transform.position = initialPos;
                    }
                }

                yield return null;
            }

            if (!movementActive)
            {
                Camera.main.transform.position = initialPos;
                background.transform.position = initialPos;
            }

            cameraShakeDifference.x = 0f;
            cameraShakeDifference.y = 0f;
            cameraShakeActive = false;
        }

        public void PauseCameraMovement(bool pause)
        {
            pauseMovement = pause;
        }

        public void ActivateCameraShake()
        {
            cameraShakeActive = true;

            if (cameraShakeInst != null)
            {
                StopCoroutine(cameraShakeInst);
            }

            cameraShakeInst = Shake(duration, magnitude);
            StartCoroutine(cameraShakeInst);
        }

        public void Reset()
        {
            Vector3 pos = Vector3.zero;
            pos.z = -10;
            Camera.main.transform.position = pos;

            background.transform.position = Vector3.zero;
            pauseMovement = false;
            cameraShakeActive = false;
            cameraShakeDifference = Vector2.zero;

            if (positionCameraInst != null)
            {
                StopCoroutine(positionCameraInst);
                movementActive = false;
            }

            if (cameraShakeInst != null)
            {
                StopCoroutine(cameraShakeInst);
                cameraShakeActive = false;
            }
        }
    }
}
