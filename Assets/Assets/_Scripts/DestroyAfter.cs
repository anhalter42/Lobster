using UnityEngine;
using System.Collections;

public class DestroyAfter : MonoBehaviour {

	public float time;

	// Use this for initialization
	void Start () {
		DestroyObject(gameObject, time);
	}
	
}
