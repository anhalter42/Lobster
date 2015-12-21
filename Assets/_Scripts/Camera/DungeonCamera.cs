using UnityEngine;
using System.Collections;

public class DungeonCamera : MonoBehaviour
{

	protected GameObject fTarget;
	public GameObject target { get { return GetTarget (); } set { SetTarget (value); } }

	public float damping = 5;
	public float rollDamping = 5;
	public Vector3 offset;

	// Use this for initialization
	void Start ()
	{
		SetTarget (target);
	}

	void LateUpdate ()
	{
		if (target != null) {
			Vector3 desiredPosition = target.transform.position + offset;
			Vector3 position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * damping);
			transform.position = position;
			Quaternion lRotation =  Quaternion.LookRotation(target.transform.position - position, Vector3.up);
			Quaternion.Lerp(transform.rotation, lRotation, Time.deltaTime * rollDamping);
			//transform.LookAt(target.transform.position);
		}
	}

	GameObject GetTarget ()
	{
		if (fTarget == null) {
			SetTarget (GameObject.FindWithTag ("Player"));
		}
		return fTarget;
	}

	void SetTarget (GameObject aTarget)
	{
		fTarget = aTarget;
		if (fTarget != null && offset == Vector3.zero) {
			offset = transform.position - target.transform.position;
		}
	}
}