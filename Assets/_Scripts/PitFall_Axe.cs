using UnityEngine;
using System.Collections;

public class PitFall_Axe : MonoBehaviour
{
	public bool isTriggered = false;
	private Animator animator;

	// Use this for initialization
	void Awake ()
	{
		animator = GetComponent<Animator>();
	}

	void Start()
	{
		if (isTriggered) {
			TriggerIt ();
		}
	}

	void OnEnable() 
	{
		if (isTriggered) {
			animator.SetTrigger("Activate");
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
