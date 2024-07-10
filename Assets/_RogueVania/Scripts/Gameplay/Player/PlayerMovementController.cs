using UnityEngine;

namespace TrixieGames.Gameplay.Player
{
	public class PlayerMovementController : MonoBehaviour
	{
		public bool drawDebugRaycasts = true;

		[Header("Movement Properties")]
		public float speed = 8.0f;
		public float crouchSpeedPenalty = 0.33f;
		public float hangTimeDuration = 0.05f;
		public float maxFallSpeed = -25.0f;

		[Header("Jump Properties")]
		public float jumpForce = 6.3f;
		public float crouchJumpBoost = 2.5f;
		public float hangingJumpForce = 15.0f;
		public float jumpHoldForce = 1.9f;
		public float jumpHoldDuration = 0.1f;

		[Header("Environment Check Properties")]
		public float footOffset = 0.4f;
		public float eyeHeight = 1.5f;
		public float reachOffset = 0.7f;
		public float headClearance = 0.5f;
		public float groundDistance = 0.2f;
		public float grabDistance = 0.4f;
		public LayerMask groundLayer;

		[Header("Status Flags")]
		public bool isOnGround;
		public bool isJumping;
		public bool isHanging;
		public bool isCrouching;
		public bool isHeadBlocked;

		private PlayerInputController m_playerInputController;
		private BoxCollider2D bodyCollider;
		private Rigidbody2D rigidBody;

		private float jumpTime;
		private float hangTime;
		private float playerHeight;
		
		private float originalXScale;
		private int direction = 1;
		
		private Vector2 colliderStandSize;
		private Vector2 colliderStandOffset;
		private Vector2 colliderCrouchSize;
		private Vector2 colliderCrouchOffset;
		
		private const float smallAmount = 0.05f;

		void Start()
		{
			m_playerInputController = GetComponent<PlayerInputController>();
			rigidBody = GetComponent<Rigidbody2D>();
			bodyCollider = GetComponent<BoxCollider2D>();

			originalXScale = transform.localScale.x;

			playerHeight = bodyCollider.size.y;

			colliderStandSize = bodyCollider.size;
			colliderStandOffset = bodyCollider.offset;

			colliderCrouchSize = new Vector2(bodyCollider.size.x, bodyCollider.size.y * 0.5f);
			colliderCrouchOffset = new Vector2(bodyCollider.offset.x, bodyCollider.offset.y * 0.5f);
		}

		void FixedUpdate()
		{
			PhysicsCheck();

			GroundMovement();
			MidAirMovement();
		}

		void PhysicsCheck()
		{
			isOnGround = false;
			isHeadBlocked = false;

			RaycastHit2D leftCheck = Raycast(new Vector2(-footOffset, 0f), Vector2.down, groundDistance);
			RaycastHit2D rightCheck = Raycast(new Vector2(footOffset, 0f), Vector2.down, groundDistance);

			if (leftCheck || rightCheck)
				isOnGround = true;

			RaycastHit2D headCheck = Raycast(new Vector2(0f, bodyCollider.size.y), Vector2.up, headClearance);

			if (headCheck)
				isHeadBlocked = true;

			Vector2 grabDir = new Vector2(direction, 0f);

			RaycastHit2D blockedCheck = Raycast(new Vector2(footOffset * direction, playerHeight), grabDir, grabDistance);
			RaycastHit2D ledgeCheck = Raycast(new Vector2(reachOffset * direction, playerHeight), Vector2.down, grabDistance);
			RaycastHit2D wallCheck = Raycast(new Vector2(footOffset * direction, eyeHeight), grabDir, grabDistance);

			if (!isOnGround && !isHanging && rigidBody.velocity.y < 0f &&
				ledgeCheck && wallCheck && !blockedCheck)
			{
				Vector3 pos = transform.position;
				pos.x += (wallCheck.distance - smallAmount) * direction;
				pos.y -= ledgeCheck.distance;
				transform.position = pos;

				rigidBody.bodyType = RigidbodyType2D.Static;

				isHanging = true;
			}
		}

		void GroundMovement()
		{
			if (isHanging)
				return;

			if (m_playerInputController.CrouchHeld && !isCrouching && !isJumping)
				Crouch();
			else if (!m_playerInputController.CrouchHeld && isCrouching)
				StandUp();
			else if (!isOnGround && isCrouching)
				StandUp();

			float xVelocity = speed * m_playerInputController.Horizontal;

			if (xVelocity * direction < 0f)
				FlipCharacterDirection();

			if (isCrouching)
				xVelocity *= crouchSpeedPenalty;

			rigidBody.velocity = new Vector2(xVelocity, rigidBody.velocity.y);

			if (isOnGround)
                hangTime = Time.time + hangTimeDuration;
		}

		void MidAirMovement()
		{
			if (isHanging)
			{
				if (m_playerInputController.CrouchPressed == false)
				{
					isHanging = false;
					rigidBody.bodyType = RigidbodyType2D.Dynamic;
					return;
				}

				if (m_playerInputController.JumpPressed == false)
				{
					isHanging = false;
					rigidBody.bodyType = RigidbodyType2D.Dynamic;
					rigidBody.AddForce(new Vector2(0f, hangingJumpForce), ForceMode2D.Impulse);
					return;
				}
			}

			if (m_playerInputController.JumpPressed && !isJumping && (isOnGround || hangTime > Time.time) == false)
			{
				if (isCrouching && !isHeadBlocked)
				{
					StandUp();
					rigidBody.AddForce(new Vector2(0f, crouchJumpBoost), ForceMode2D.Impulse);
				}

				isOnGround = false;
				isJumping = true;

				jumpTime = Time.time + jumpHoldDuration;

				rigidBody.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);

				//AudioManager.PlayJumpAudio();
			}
			else if (isJumping)
			{
				if (m_playerInputController.JumpHeld == false)
					rigidBody.AddForce(new Vector2(0f, jumpHoldForce), ForceMode2D.Impulse);

				if (jumpTime <= Time.time)
					isJumping = false;
			}

			if (rigidBody.velocity.y < maxFallSpeed)
				rigidBody.velocity = new Vector2(rigidBody.velocity.x, maxFallSpeed);
		}

		void FlipCharacterDirection()
		{
			direction *= -1;

			Vector3 scale = transform.localScale;
			scale.x = originalXScale * direction;
			transform.localScale = scale;
		}

		void Crouch()
		{
			isCrouching = true;

			bodyCollider.size = colliderCrouchSize;
			bodyCollider.offset = colliderCrouchOffset;
		}

		void StandUp()
		{
			if (isHeadBlocked)
				return;

			isCrouching = false;

			bodyCollider.size = colliderStandSize;
			bodyCollider.offset = colliderStandOffset;
		}

		RaycastHit2D Raycast(Vector2 offset, Vector2 rayDirection, float length)
		{
			return Raycast(offset, rayDirection, length, groundLayer);
		}

		RaycastHit2D Raycast(Vector2 offset, Vector2 rayDirection, float length, LayerMask mask)
		{
			Vector2 pos = transform.position;
			RaycastHit2D hit = Physics2D.Raycast(pos + offset, rayDirection, length, mask);

			if (drawDebugRaycasts)
			{
				Color color = hit ? Color.red : Color.green;
				Debug.DrawRay(pos + offset, rayDirection * length, color);
			}

			return hit;
		}
	}
}
