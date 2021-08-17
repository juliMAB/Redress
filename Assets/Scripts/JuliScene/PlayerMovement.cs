using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Games.Generics.Character.Movement;

namespace EndlessT4cos.Gameplay.Player
{
    public class PlayerMovement : CharacterController2D
    {
        //[SerializeField] private CharacterController2D controller;
        [SerializeField] private float runSpeed = 40f;

        private float horizontalMove = 0f;
        private bool isJumping = false;
        private bool isCrouching = false;

        private void Update()
        {
            horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;

            if (Input.GetButtonDown("Jump"))
            {
                isJumping = true;
            }
            if (Input.GetButtonDown("Crouch"))
            {
                isCrouching = true;
            }
            else if (Input.GetButtonUp("Crouch"))
            {
                isCrouching = false;
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

