using UnityEngine;

namespace Games.Generics.Character.Movement
{
	public class CharacterMovementSeter : MonoBehaviour
	{
		private bool soundPlayed = false;
		[HideInInspector]
		private float normalizedHorizontalSpeed = 0;

		private CharacterController2D _controller;
		private Animator _animator;
		private Vector3 _velocity;

		private bool controlActive = true;

		// movement config
		public float gravity = -25f;
		public float runSpeed = 8f;
		public float groundDamping = 20f; // how fast do we change direction? higher means faster
		public float inAirDamping = 5f;
		public float jumpHeight = 3f;
		public bool lockGoDown = false;

		public bool ControlActive { set { controlActive = value; } }

		private void Awake()
		{
			_animator = GetComponentInChildren<Animator>();
			_controller = GetComponent<CharacterController2D>();
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
   //         else if (_controller.collisionState.becameGroundedThisFrame)
   //         {
			//	_animator.SetTrigger("StartGround");
			//}
            else
            {
                if (_velocity.y<0)
                {
					_animator.SetTrigger("StartFall");
				}
            }
			//this is for normal move.
			SetNormalMovementUpdate();

			//this is for dash.
			//SetDashUpdate();

			// we can only jump whilst grounded
			if (_controller.isGrounded && Input.GetAxisRaw("Jump")!=0)
			{
				AkSoundEngine.PostEvent("play_salto", gameObject);
				_velocity.y = Mathf.Sqrt(2f * jumpHeight * -gravity);
				_animator.SetTrigger("Jump");
				//_animator.Play(Animator.StringToHash("Jump"));
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

				if (!soundPlayed)
                {
					AkSoundEngine.PostEvent(SoundsManager.Get().Bajar, gameObject);
					soundPlayed = true;
				}
				
				if (!lockGoDown)
                {
					_controller.ignoreOneWayPlatformsThisFrame = true;
				}
			}
			else
			{
				soundPlayed = false;
			}

			_controller.move(_velocity * Time.deltaTime);

			// grab our current _velocity to use as a base for all calculations
			_velocity = _controller.velocity;
		}
		
        private void moveRightLeft()
        {
			normalizedHorizontalSpeed = Input.GetAxisRaw("Horizontal");
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
				_animator.Play(Animator.StringToHash("Run"));
			}
		}
        private void SetNormalMovementUpdate()
        {
			moveRightLeft();
			if (Input.GetAxisRaw("Horizontal")==0)
			{
				normalizedHorizontalSpeed = 0;
			}
		}
    }
}