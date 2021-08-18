﻿using UnityEngine;
using UnityEngine.Events;

namespace Games.Generics.Character.Movement
{
	public class CharacterController2D : MonoBehaviour
	{
		[SerializeField] private float m_JumpForce = 400f;								// Amount of force added when the player jumps.
		[Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f;				// Amount of maxSpeed applied to crouching movement. 1 = 100%
		[Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;		// How much to smooth out the movement
		[SerializeField] private bool m_AirControl = false;								// Whether or not a player can steer while jumping;
		[SerializeField] private LayerMask m_WhatIsGround = 0;                          // A mask determining what is ground to the character
		[SerializeField] private Transform m_GroundCheck = null;                        // A position marking where to check if the player is grounded.
		[SerializeField] private Transform m_GroundCheck2 = null;                        // A position marking where to check if the player is grounded.
		[SerializeField] private Transform m_CeilingCheck = null;                       // A position marking where to check for ceilings
		//[SerializeField] private Collider2D m_CrouchDisableCollider = null;             // A collider that will be disabled when crouching

		const float k_GroundedRadius = .002f;			// Radius of the overlap circle to determine if grounded
		const float k_CeilingRadius = .2f;			// Radius of the overlap circle to determine if the player can stand up
		private bool m_Grounded = false;            // Whether or not the player is grounded.		
		private Rigidbody2D m_Rigidbody2D = null;
		private bool m_FacingRight = true;			// For determining which way the player is currently facing.
		private Vector3 m_Velocity = Vector3.zero;

		[Header("Events")]
		[Space]

		public UnityEvent OnLandEvent;
		public UnityEvent OnJumpEvent;
		public UnityEvent OnCrouchEvent;
		public UnityEvent OnStopCrEvent;

		
		private bool m_wasCrouching = false;

		private void Awake()
		{
			m_Rigidbody2D = GetComponent<Rigidbody2D>();

			if (OnLandEvent == null)
				OnLandEvent = new UnityEvent();

			if (OnJumpEvent == null)
				OnJumpEvent = new UnityEvent();

			if (OnCrouchEvent == null)
				OnCrouchEvent = new UnityEvent();

			if (OnStopCrEvent == null)
				OnStopCrEvent = new UnityEvent();
		}

		protected virtual void FixedUpdate()
		{
			bool wasGrounded = m_Grounded;
			m_Grounded = false;
            if (m_Rigidbody2D.velocity.y<0)
            {
				Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck2.position, k_GroundedRadius, m_WhatIsGround);
				for (int i = 0; i < colliders.Length; i++)
				{
					if (colliders[i].gameObject != gameObject)
					{
						m_Grounded = true;
						if (!wasGrounded)
						{
							OnLandEvent.Invoke();
						}
					}
				}
			}
		}

		public void Move(float move, bool crouch, bool jump)
		{
			//only control the player if grounded or airControl is turned on
			if (m_Grounded || m_AirControl)
			{

				// If crouching
				if (crouch)
				{
					if (!m_wasCrouching)
					{
						m_wasCrouching = true;
						OnCrouchEvent.Invoke();
					}
					
					
					// Reduce the speed by the crouchSpeed multiplier
					//move *= m_CrouchSpeed;

					// Disable one of the colliders when crouching
					//if (m_CrouchDisableCollider != null)
					//	m_CrouchDisableCollider.enabled = false;
				}
				else
				{
					// Enable the collider when not crouching
					//if (m_CrouchDisableCollider != null)
					//	m_CrouchDisableCollider.enabled = true;

					if (m_wasCrouching)
					{
						m_wasCrouching = false;
						OnStopCrEvent.Invoke();
					}
				}

				// Move the character by finding the target velocity
				Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
				// And then smoothing it out and applying it to the character
				m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

				// If the input is moving the player right and the player is facing left...
				if (move > 0 && !m_FacingRight)
				{
					// ... flip the player.
					Flip();
				}
				// Otherwise if the input is moving the player left and the player is facing right...
				else if (move < 0 && m_FacingRight)
				{
					// ... flip the player.
					Flip();
				}
			}
			// If the player should jump...
			if (m_Grounded && jump)
			{
				// Add a vertical force to the player.
				m_Grounded = false;
				m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
				OnJumpEvent.Invoke();
				
			}
		}

		private void Flip()
		{
			// Switch the way the player is labelled as facing.
			m_FacingRight = !m_FacingRight;

			// Multiply the player's x local scale by -1.
			//Vector3 theScale = transform.localScale;
			//theScale.x *= -1;
			//transform.localScale = theScale;
			Vector3 rotation = Vector3.zero;

			if (!m_FacingRight)
			{ 
				rotation = new Vector3(0, -180, 0); 
			}

			transform.rotation = Quaternion.Euler(rotation);
		}
	}
}
