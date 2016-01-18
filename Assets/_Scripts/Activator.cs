using UnityEngine;
using System.Collections;

public class Activator : MonoBehaviour
{

	public enum When
	{
		OnTriggerEnter,
		OnTriggerLeave
	}

	public Behaviour[] m_Behaviours = { };
	public When when = When.OnTriggerEnter;
	public bool isActivated = false;
	public string levelControllerMethod = string.Empty;
	public float levelControllerMethodDelay = 0f;
	public float m_ObjectActivationDelay = 0f;
	public string[] tags = { "Player" };

	Animator m_animator;
	GameObject m_activationObject;

	// Use this for initialization
	void Start ()
	{
		if (m_animator == null) {
			m_animator = GetComponent<Animator> ();
		}
		if (m_activationObject == null) {
			Transform lT = transform.FindChild ("Activate");
			if (lT != null) {
				m_activationObject = lT.gameObject;
			}
		}
	}

	void OnEnable ()
	{
		if (isActivated) {
			ActivateIt (true);
		}
	}

	bool TagMatched (string aTag)
	{
		foreach (string lTag in tags) {
			if (lTag.Equals (aTag)) {
				return true;
			}
		}
		return false;
	}

	void OnTriggerEnter (Collider aOther)
	{
		if (when == When.OnTriggerEnter) {
			if (!isActivated) {
				if (TagMatched (aOther.gameObject.tag)) {
					ActivateIt ();
				}
			}
		}
	}

	void OnTriggerLeave (Collider aOther)
	{
		if (when == When.OnTriggerLeave) {
			if (!isActivated) {
				if (TagMatched (aOther.gameObject.tag)) {
					ActivateIt ();
				}
			}
		}
	}

	void ActivateIt (bool aRestore = false)
	{
		isActivated = true;
		if (!aRestore) {
			foreach (Behaviour lBeh in m_Behaviours) {
				lBeh.enabled = true;
				if (lBeh is Animation) {
					((Animation)lBeh).Play ();
				} else if (lBeh is Animator) {
					if (aRestore) {
						((Animator)lBeh).SetTrigger ("RestoreActivate");
					} else {
						((Animator)lBeh).SetTrigger ("Activate");
					}
				}
			}
		}
		if (m_activationObject) {
			// AudioSource Workaround
			if (aRestore) {
				AudioSource lAS = m_activationObject.GetComponent<AudioSource> ();
				if (lAS != null) {
					lAS.enabled = false;
				}
			}
			if (m_ObjectActivationDelay <= 0f) {
				ActivateObject ();
			} else {
				Invoke ("ActivateObject", m_ObjectActivationDelay);
			}
		}
		if (m_animator) {
			if (aRestore) {
				m_animator.SetTrigger ("RestoreActivate");
			} else {
				m_animator.SetTrigger ("Activate");
			}
		}
		if (!aRestore && !string.IsNullOrEmpty (levelControllerMethod)) {
			AllLevels.Get ().levelController.Invoke (levelControllerMethod, levelControllerMethodDelay);
		}
	}

	void ActivateObject ()
	{
		m_activationObject.SetActive (true);
	}
}
