using UnityEngine;
using System.Collections;

public class PitFall_Axe : MonoBehaviour
{
	public Transform pivot;
	public Vector3 rotation;
	public bool isTriggered = false;
	private Animator animator;

	// Use this for initialization
	void Awake ()
	{
		animator = GetComponent<Animator>();
	}

	void Start()
	{
		if (!pivot) {
			pivot = transform.parent.FindChild ("Pivot");
		}
		if (isTriggered) {
			TriggerIt ();
		}
	}
	
	void OnTriggerEnter (Collider aOther) {
		if (!isTriggered) {
			if (aOther.gameObject.tag == "Player") {
				TriggerIt();
			}
		}
	}

	void TriggerIt ()
	{
		if (!isTriggered) {
			isTriggered = true;
			animator.SetTrigger("Activate");
		}
	}
}
