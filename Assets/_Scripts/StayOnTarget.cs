using UnityEngine;
using System.Collections;

public class StayOnTarget : MonoBehaviour {

	public Transform target;
	public Vector3 offset = Vector3.zero;
	public bool onPlayer = true;

	void LateUpdate () {
		if (target == null) {
			if (onPlayer) {
				GameObject lTarget = GameObject.FindWithTag("Player") as GameObject;
				if (lTarget) {
					target = lTarget.transform;
				}
			}
		}
		if (target) {
			transform.position = target.position + offset;
		}
	}
}
