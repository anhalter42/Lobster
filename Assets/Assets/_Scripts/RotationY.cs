using UnityEngine;
using System.Collections;

public class RotationY : MonoBehaviour {

	public float rotation = 1;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(new Vector3(0,rotation,0));
	}
}
