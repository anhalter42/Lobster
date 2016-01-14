using UnityEngine;

namespace MAHN42
{
	[RequireComponent (typeof(Rigidbody))]
	[RequireComponent (typeof(CapsuleCollider))]
	[RequireComponent (typeof(Animator))]
	public class ThirdPersonCharacter : MonoBehaviour
	{
		[SerializeField] bool m_MAHN42 = true;
		[SerializeField] float m_MovingTurnSpeed = 360;
		[SerializeField] float m_StationaryTurnSpeed = 180;
		[SerializeField] float m_JumpPower = 2f;
		[Range (1f, 4f)][SerializeField] float m_GravityMultiplier = 2f;
		[SerializeField] float m_RunCycleLegOffset = 0.2f;
		//specific to the character in sample assets, will need to be modified to work with others
		[SerializeField] float m_MoveSpeedMultiplier = 1f;
		[SerializeField] float m_AnimSpeedMultiplier = 1f;
		[SerializeField] float m_GroundCheckDistance = 0.1f;
		[SerializeField] float m_FootOffsetUp = 0.01f;
		[SerializeField] Transform m_FootLeft;
		[SerializeField] Transform m_FootRight;
		[SerializeField] Transform m_Head;

		Rigidbody m_Rigidbody;
		Animator m_Animator;
		bool m_IsGrounded;
		bool m_IsLeftGrounded;
		bool m_IsRightGrounded;
		float m_OrigGroundCheckDistance;
		const float k_Half = 0.5f;
		float m_TurnAmount;
		float m_ForwardAmount;
		Vector3 m_GroundNormal;
		float m_CapsuleHeight;
		Vector3 m_CapsuleCenter;
		CapsuleCollider m_Capsule;
		bool m_Crouching;


		void CheckComponents ()
		{
			if (!m_Animator)
				m_Animator = GetComponent<Animator> ();
			if (!m_Rigidbody)
				m_Rigidbody = GetComponent<Rigidbody> ();
			if (!m_Capsule)
				m_Capsule = GetComponent<CapsuleCollider> ();
		}

		/*
		void Update ()
		{
			transform.rotation = (-transform.localRotation.x, 0f, -transform.localRotation.y);
		}
		*/

		void Start ()
		{
			CheckComponents ();
			m_CapsuleHeight = m_Capsule.height;
			m_CapsuleCenter = m_Capsule.center;

			m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
			m_OrigGroundCheckDistance = m_GroundCheckDistance;
		}


		public void Move (Vector3 move, bool crouch, bool jump)
		{
			if (!m_Animator)
				return;
			// convert the world relative moveInput vector into a local-relative
			// turn amount and forward amount required to head in the desired
			// direction.
			if (move.magnitude > 1f)
				move.Normalize ();
			move = transform.InverseTransformDirection (move);
			ScaleCapsule ();
			CheckGroundStatus ();
			move = Vector3.ProjectOnPlane (move, m_GroundNormal);
			m_TurnAmount = Mathf.Atan2 (move.x, move.z);
			m_ForwardAmount = move.z;

			ApplyExtraTurnRotation ();

			// control and velocity handling is different when grounded and airborne:
			if (m_IsGrounded) {
				HandleGroundedMovement (crouch, jump);
			} else {
				HandleAirborneMovement ();
			}

			// send input and other state parameters to the animator
			UpdateAnimator (move);
		}

		void UpdateAnimator (Vector3 move)
		{
			// update the animator parameters
			m_Animator.SetFloat ("Forward", m_ForwardAmount, 0.1f, Time.deltaTime);
			m_Animator.SetFloat ("Turn", m_TurnAmount, 0.1f, Time.deltaTime);
			m_Animator.SetBool ("Crouch", m_Crouching);
			m_Animator.SetBool ("OnGround", m_IsGrounded);
			m_Animator.SetBool ("OnGroundLeft", m_IsLeftGrounded);
			m_Animator.SetBool ("OnGroundRight", m_IsRightGrounded);
			if (!m_IsGrounded) {
				m_Animator.SetFloat ("Jump", m_Rigidbody.velocity.y);
			}

			// calculate which leg is behind, so as to leave that leg trailing in the jump animation
			// (This code is reliant on the specific run cycle offset in our animations,
			// and assumes one leg passes the other at the normalized clip times of 0.0 and 0.5)
			float runCycle =
				Mathf.Repeat (
					m_Animator.GetCurrentAnimatorStateInfo (0).normalizedTime + m_RunCycleLegOffset, 1);
			float jumpLeg = (runCycle < k_Half ? 1 : -1) * m_ForwardAmount;
			if (m_IsGrounded) {
				m_Animator.SetFloat ("JumpLeg", jumpLeg);
			}

			// the anim speed multiplier allows the overall speed of walking/running to be tweaked in the inspector,
			// which affects the movement speed because of the root motion.
			if (m_IsGrounded && move.magnitude > 0) {
				m_Animator.speed = m_AnimSpeedMultiplier;
			} else {
				// don't use that while airborne
				m_Animator.speed = 1;
			}
		}

		void ScaleCapsule ()
		{
			if (m_Head && m_FootLeft && m_FootRight) {
				float lMinX = Mathf.Min (m_FootLeft.position.x, m_FootRight.position.x, m_Head.position.x);
				float lMaxX = Mathf.Max (m_FootLeft.position.x, m_FootRight.position.x, m_Head.position.x);
				float lMinY = Mathf.Min (m_FootLeft.position.y, m_FootRight.position.y, m_Head.position.y);
				float lMaxY = Mathf.Max (m_FootLeft.position.y, m_FootRight.position.y, m_Head.position.y);
				float lMinZ = Mathf.Min (m_FootLeft.position.z, m_FootRight.position.z, m_Head.position.z);
				float lMaxZ = Mathf.Max (m_FootLeft.position.z, m_FootRight.position.z, m_Head.position.z);
				m_Capsule.center = m_Capsule.transform.InverseTransformPoint (lMinX + (lMaxX - lMinX) / 2f, lMinY + (lMaxY - lMinY) / 2f, lMinZ + (lMaxZ - lMinZ) / 2f);
				m_Capsule.height = m_Capsule.transform.InverseTransformPoint (0, lMaxY - lMinY, 0).y + m_Capsule.radius * 2;
				//m_Capsule.radius = m_Capsule.transform.InverseTransformPoint(lMaxX - lMinX, 0, lMaxZ - lMinZ).magnitude;
			}
		}


		void HandleAirborneMovement ()
		{
			// apply extra gravity from multiplier:
			Vector3 extraGravityForce = (Physics.gravity * m_GravityMultiplier) - Physics.gravity;
			m_Rigidbody.AddForce (extraGravityForce);

			m_GroundCheckDistance = m_Rigidbody.velocity.y < 0 ? m_OrigGroundCheckDistance : 0.01f;
		}


		void HandleGroundedMovement (bool crouch, bool jump)
		{
			// check whether conditions are right to allow a jump:
			if (jump && !crouch && m_Animator.GetCurrentAnimatorStateInfo (0).IsName ("Grounded")) {
				// jump!
				m_Rigidbody.velocity = new Vector3 (m_Rigidbody.velocity.x, m_JumpPower, m_Rigidbody.velocity.z);
				m_IsGrounded = false;
				m_Animator.applyRootMotion = false;
				m_GroundCheckDistance = 0.1f;
			}
		}

		void ApplyExtraTurnRotation ()
		{
			// help the character turn faster (this is in addition to root rotation in the animation)
			float turnSpeed = Mathf.Lerp (m_StationaryTurnSpeed, m_MovingTurnSpeed, m_ForwardAmount);
			//transform.Rotate (-transform.rotation.x, m_TurnAmount * turnSpeed * Time.deltaTime, -transform.rotation.z);
			transform.Rotate (0f, m_TurnAmount * turnSpeed * Time.deltaTime, 0f);
		}

		public void OnAnimatorMove ()
		{
			// we implement this function to override the default root motion.
			// this allows us to modify the positional speed before it's applied.
			if (m_IsGrounded && Time.deltaTime > 0) {
				Vector3 v = (m_Animator.deltaPosition * m_MoveSpeedMultiplier) / Time.deltaTime;

				// we preserve the existing y part of the current velocity.
				v.y = m_Rigidbody.velocity.y;
				m_Rigidbody.velocity = v;
			}
		}

		void CheckGroundStatus ()
		{
			bool lFoot = false;
			RaycastHit hitInfo;
			if (m_FootLeft) {
				lFoot = true;
				m_IsLeftGrounded = Physics.Raycast (m_FootLeft.position + (Vector3.up * m_FootOffsetUp), Vector3.down, out hitInfo, m_GroundCheckDistance);
				#if UNITY_EDITOR
				Debug.DrawLine (m_FootLeft.position + (Vector3.up * m_FootOffsetUp), m_FootLeft.position + (Vector3.up * m_FootOffsetUp) + (Vector3.down * m_GroundCheckDistance));
				#endif
			}
			if (m_FootRight) {
				lFoot = true;
				m_IsRightGrounded = Physics.Raycast (m_FootRight.position + (Vector3.up * m_FootOffsetUp), Vector3.down, out hitInfo, m_GroundCheckDistance);
				#if UNITY_EDITOR
				Debug.DrawLine (m_FootRight.position + (Vector3.up * m_FootOffsetUp), m_FootRight.position + (Vector3.up * m_FootOffsetUp) + (Vector3.down * m_GroundCheckDistance));
				#endif
			}
			if (!lFoot) {
				// it is also good to note that the transform position in the sample assets is at the base of the character
				m_IsGrounded = Physics.Raycast (transform.position + (Vector3.up * m_FootOffsetUp), Vector3.down, out hitInfo, m_GroundCheckDistance);
				m_IsLeftGrounded = m_IsGrounded;
				m_IsRightGrounded = m_IsGrounded;
			} else {
				m_IsGrounded = m_IsLeftGrounded || m_IsRightGrounded;
			}
			m_GroundNormal = Vector3.up; //hitInfo.normal;
			m_Animator.applyRootMotion = m_IsGrounded;
		}
	}
}
