using UnityEngine;
using System.Collections;

public class ExitNextLevel : MonoBehaviour
{
	public Animator m_animator;

	// Use this for initialization
	void Start ()
	{
		if (!m_animator) {
			m_animator = GetComponent<Animator> ();
		}
	}

	void OnTriggerEnter (Collider aCollider)
	{
		if (aCollider.gameObject.tag == "Player") {
			if (m_animator) {
				m_animator.SetTrigger ("Activate");
			}
			AllLevels.Get ().levelController.PlayerHasExitReached ();
		}
	}

}
