using UnityEngine;
using System.Collections;

public class ExitNextLevel : MonoBehaviour
{

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	void OnTriggerEnter (Collider aCollider)
	{
		if (aCollider.gameObject.tag == "Player") {
			AllLevels.Get ().NextLevel ();
		}
	}

}
