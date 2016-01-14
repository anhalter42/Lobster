using UnityEngine;
using System.Collections;

public class PauseAnimatorOnCollision : MonoBehaviour
{

	public Animator m_Animator;
	public float m_ResumeDelay = 1f;
	float m_OriginalSpeed;
	bool m_InCollision = false;

	// Use this for initialization
	void Start ()
	{
		if (!m_Animator)
			m_Animator = GetComponent<Animator> ();
	}

	void OnCollisionEnter (Collision aCollision)
	{
		if (m_Animator.speed != 0) {
			m_OriginalSpeed = m_Animator.speed;
			m_Animator.speed = 0;// StopPlayback
		}
		m_InCollision = true;
	}

	void OnCollisionExit (Collision aCollisionInfo)
	{
		m_InCollision = false;
		Invoke("ResumePLayback", m_ResumeDelay);
	}

	void ResumePLayback()
	{
		if (!m_InCollision) {
			m_Animator.speed = m_OriginalSpeed;
		}
	}
}
