using UnityEngine;
using System.Collections;

public class ExitNextLevel : MonoBehaviour
{
	public Animator m_animator;
	public GameObject m_activationObject;

	// Use this for initialization
	void Start ()
	{
		if (!m_animator) {
			m_animator = GetComponent<Animator> ();
		}
		if (!m_activationObject) {
			Transform lT = transform.FindChild ("Activate");
			if (lT) {
				m_activationObject = lT.gameObject;
			}
		}
	}

	void OnTriggerEnter (Collider aCollider)
	{
		if (aCollider.gameObject.tag == "Player") {
			if (m_activationObject) {
				m_activationObject.SetActive (true);
			}
			if (m_animator) {
				m_animator.SetTrigger ("Activate");
			}
			AllLevels.Get ().levelController.PlayerHasExitReached ();
		}
	}

}
