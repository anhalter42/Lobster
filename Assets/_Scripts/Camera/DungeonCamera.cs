using UnityEngine;
using UnityEngine.Serialization;
using System.Collections;

public class DungeonCamera : MonoBehaviour
{
	public enum Mode
	{
		FollowTarget,
		Spectate
	}

	public Mode mode = Mode.FollowTarget;

	public GameObject m_Target;

	public GameObject target { get { return GetTarget (); } set { SetTarget (value); } }

	public float damping = 5;
	public float rollDamping = 5;

	//[FormerlySerializedAs ("offset")]
	public Vector3 m_Offset;

	public Vector3 offset { get { return GetOffset (); } set { SetOffset (value); } }

	// Use this for initialization
	void Start ()
	{
		SetTarget (target);
	}

	void LateUpdate ()
	{
		if (target != null) {
			switch (mode) {
			case Mode.FollowTarget:
				UpdatePositionAndRotation (true);
				break;
			case Mode.Spectate:
				transform.RotateAround (target.transform.position, Vector3.up, 20f * Time.deltaTime);
				break;
			}
		}
	}

	GameObject GetTarget ()
	{
		if (m_Target == null) {
			SetTarget (GameObject.FindWithTag ("Player"));
		}
		return m_Target;
	}

	void SetTarget (GameObject aTarget)
	{
		m_Target = aTarget;
		if (m_Target != null) {
			if (offset == Vector3.zero) {
				offset = transform.position - target.transform.position;
			}
			UpdatePositionAndRotation ();
		}
	}

	Vector3 GetOffset ()
	{
		if (m_Target != null && m_Offset == Vector3.zero) {
			offset = transform.position - target.transform.position;
		}
		return m_Offset;
	}

	void SetOffset (Vector3 aOffset)
	{
		m_Offset = aOffset;
		UpdatePositionAndRotation ();
	}

	void UpdatePositionAndRotation (bool aSmooth = true)
	{
		Vector3 lDesiredPosition = m_Target.transform.position + m_Offset;
		if (aSmooth) {
			transform.position = Vector3.Lerp (transform.position, lDesiredPosition, Time.deltaTime * damping);
			Quaternion lRotation = Quaternion.LookRotation (target.transform.position - transform.position, Vector3.up);
			transform.rotation = Quaternion.Lerp (transform.rotation, lRotation, Time.deltaTime * rollDamping);
		} else {
			transform.position = lDesiredPosition;
			transform.LookAt (m_Target.transform.position);
		}
	}
}