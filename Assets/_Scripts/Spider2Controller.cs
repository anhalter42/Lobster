using UnityEngine;
using System.Collections;

public class Spider2Controller : MonoBehaviour {

	public Vector3 target;

	// Use this for initialization
	void Start () {
		target = transform.position;
	}

	// Update is called once per frame
	void Update () {
		if (target == transform.position) {
		}
	}
}
