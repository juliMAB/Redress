using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Games.Generics.Character.Movement
{
    public class CharacterMovementSetter : CharacterController2D
    {
        [SerializeField] private float runSpeed = 40f;
        private PlatformEffector2D effector;
        private float horizontalMove = 0f;
        private bool isJumping = false;
        private bool isCrouching = false;

        private void Start()
        {
            effector = GetComponent<PlatformEffector2D>();
        }
        private void Update()
        {
            horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;

            if (Input.GetButtonDown("Jump"))
            {
                isJumping = true;
            }
            if (Input.GetButtonDown("Crouch"))
            {
                effector.rotationalOffset = 0f;
            }
            else if (Input.GetButtonUp("Crouch"))
            {
                effector.rotationalOffset = 180f;
            }
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            Move(horizontalMove * Time.fixedDeltaTime, isCrouching, isJumping);
            isJumping = false;
        }
    }
}

