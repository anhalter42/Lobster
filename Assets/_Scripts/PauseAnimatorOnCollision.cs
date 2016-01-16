using UnityEngine;
using System.Collections;

public class PauseAnimatorOnCollision : MonoBehaviour
{

	public Animator m_Animator;
	public float m_ResumeDelay = 1f;
	public bool m_OnTrigger = true;
	public bool m_OnCollision = true;
	float m_OriginalSpeed;
	bool triggered = false;
	int count = 0;

	// Use this for initialization
	void Start ()
	{
		if (!m_Animator)
			m_Animator = GetComponent<Animator> ();
	}

	void Update ()
	{
		if (triggered && count == 0) {
			triggered = false;
			Invoke ("ResumePlayback", m_ResumeDelay);
		}
	}

	void OnCollisionEnter (Collision aCollision)
	{
		if (m_OnCollision) {
			if (count == 0 && m_Animator.speed != 0) {
				m_OriginalSpeed = m_Animator.speed;
				m_Animator.speed = 0;// StopPlayback
			}
			triggered = true;
			count++;
		}
	}

	void OnCollisionExit (Collision aCollisionInfo)
	{
		if (m_OnCollision) {
			count = Mathf.Max (0, count - 1);
		}
	}

	void OnTriggerEnter (Collider aOther)
	{
		if (m_OnTrigger) {
			if (count == 0 && m_Animator.speed != 0) {
				m_OriginalSpeed = m_Animator.speed;
				m_Animator.speed = 0;// StopPlayback
			}
			triggered = true;
			count++;
		}
	}

	void OnTriggerExit (Collider aOther)
	{
		if (m_OnTrigger) {
			count = Mathf.Max (0, count - 1);
		}
	}

	void ResumePlayback ()
	{
		if (count == 0) {
			m_Animator.speed = m_OriginalSpeed;
		}
	}
}
