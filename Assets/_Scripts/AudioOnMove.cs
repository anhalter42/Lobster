using UnityEngine;
using System.Collections;

public class AudioOnMove : MonoBehaviour
{

	public float velocity = 0.2f;
	public float angularVelocity = 0.2f;

	Rigidbody m_Rigidbody;
	AudioSource m_AudioSource;

	// Use this for initialization
	void Start ()
	{
		m_Rigidbody = GetComponent<Rigidbody> ();
		m_AudioSource = GetComponent<AudioSource> ();
	}

	void FixedUpdate ()
	{
		if (m_Rigidbody == null || m_AudioSource == null)
			return;
		if (m_Rigidbody.angularVelocity.magnitude >= angularVelocity || m_Rigidbody.velocity.magnitude > velocity) {
			if (!m_AudioSource.isPlaying)
				m_AudioSource.Play ();
		} else {
			m_AudioSource.Stop ();
		}
	}
}
