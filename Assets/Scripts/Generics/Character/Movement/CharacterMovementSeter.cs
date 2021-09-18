
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Games.Generics.Character.Movement
{
	public class CharacterMovementSeter : MonoBehaviour
	{
		[HideInInspector]
		private float normalizedHorizontalSpeed = 0;

		private CharacterController2D _controller;
		private Animator _animator;
		private Vector3 _velocity;
		private KeyCode lastKey;

		[Header("Dash")]
		[SerializeField] private float dashCooldownT;
		[SerializeField] private float resetDash = 0.5f;
		[SerializeField] private float resetDashT;
		[SerializeField] private Slider slider; // no esta bien, pero no sabia como agarrarlo.
		[SerializeField] private bool controlActive = true;

		// movement config
		public float gravity = -25f;
		public float runSpeed = 8f;
		public float groundDamping = 20f; // how fast do we change direction? higher means faster
		public float inAirDamping = 5f;
		public float jumpHeight = 3f;
		public float dashCooldown = 3f;
		public float dashPower = 5f;

		public bool ControlActive { set { controlActive = value; } }

		private void Awake()
		{
			_animator = GetComponent<Animator>();
			_controller = GetComponent<CharacterController2D>();
		}

        private void Start()
        {
			dashCooldownT = -1;
			resetDashT = resetDash;
			slider.maxValue = dashCooldown;
		}

		public void CharacterMovementSeterUpdate()
        {
			if (!controlActive)
			{
				return;
			}

			if (_controller.isGrounded) //this is for grounded.
			{
				_velocity.y = 0;
			}

			//this is for normal move.
			SetNormalMovementUpdate();

			//this is for dash.
			SetDashUpdate();

			// we can only jump whilst grounded
			if (_controller.isGrounded && Input.GetAxisRaw("Jump")!=0)
			{
				AkSoundEngine.PostEvent("Jump", gameObject);
				_velocity.y = Mathf.Sqrt(2f * jumpHeight * -gravity);
				_animator.Play(Animator.StringToHash("Jump2"));
			}

			// apply horizontal speed smoothing it. dont really do this with Lerp. Use SmoothDamp or something that provides more control
			var smoothedMovementFactor = _controller.isGrounded ? groundDamping : inAirDamping; // how fast do we change direction?
			_velocity.x = Mathf.Lerp(_velocity.x, normalizedHorizontalSpeed * runSpeed, Time.deltaTime * smoothedMovementFactor);
			
			// apply gravity before moving
			_velocity.y += gravity * Time.deltaTime;

			// if holding down bump up our movement amount and turn off one way platform detection for a frame.
			// this lets us jump down through one way platforms
			if (_controller.isGrounded && Input.GetAxisRaw("GoDown") != 0)
			{
				_velocity.y *= 3f;
				_controller.ignoreOneWayPlatformsThisFrame = true;
			}

			_controller.move(_velocity * Time.deltaTime);

			// grab our current _velocity to use as a base for all calculations
			_velocity = _controller.velocity;
		}
		private void SaveLastKey(KeyCode key)
		{
			if (resetDashT > 0)
			{
				lastKey = key;
			}
			else
			{
				lastKey = 0;
			}
			resetDashT = resetDash;
		}
        public void UpdateDash()
        {
            resetDashT -= Time.deltaTime;

			SaveLastKey(KeyCode.A);
			SaveLastKey(KeyCode.RightArrow);
			SaveLastKey(KeyCode.LeftArrow);
			SaveLastKey(KeyCode.D);
		}
        private void moveRightLeft()
        {
			normalizedHorizontalSpeed = Input.GetAxis("Horizontal");
			if (normalizedHorizontalSpeed > 0f)
			{
				transform.eulerAngles = new Vector3(0, 0, 0);
			}
            else if (normalizedHorizontalSpeed<0f)
            {
				transform.eulerAngles = new Vector3(0, 180, 0);
			}
			if (_controller.isGrounded)
			{
				_animator.Play(Animator.StringToHash("Run2"));
			}
		}
        private void SetNormalMovementUpdate()
        {
			UpdateDash();
			moveRightLeft();
			if (Input.GetAxis("Horizontal")==0)
			{
				normalizedHorizontalSpeed = 0;

				if (_controller.isGrounded)
				{
					//_animator.Play(Animator.StringToHash("Idle2"));
				}
			}
		}
		private void TakeTheDash(KeyCode key,short dir) 
		{
			if (Input.GetKeyDown(key))
			{
				if (lastKey == key)
				{
					normalizedHorizontalSpeed = dashPower*dir;
					dashCooldownT = dashCooldown;
					if (transform.eulerAngles.y > 0f)
					{
						transform.eulerAngles = new Vector3(0, 0, 0);
					}
					if (_controller.isGrounded)
					{
						_animator.Play(Animator.StringToHash("Run2"));
					}
					lastKey = 0;
				}
			}
		}
		private void SetDashUpdate()
        {
			if (dashCooldownT > 0)
			{
				dashCooldownT -= Time.deltaTime;
				slider.value = dashCooldownT;
				if (dashCooldownT <= 0)
				{
					slider.gameObject.SetActive(false);
				}
				else
				{
					slider.gameObject.SetActive(true);
				}

			}
			if (lastKey != 0 && resetDashT > 0)
			{

				if (dashCooldownT < 0)
				{
					TakeTheDash(KeyCode.RightArrow, +1);
					TakeTheDash(KeyCode.LeftArrow, -1);
					TakeTheDash(KeyCode.D, +1);
					TakeTheDash(KeyCode.A, -1);
				}
			}
		}
    }
}