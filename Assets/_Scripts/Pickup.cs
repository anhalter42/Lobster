using UnityEngine;
using System.Collections;

public class Pickup : MonoBehaviour
{
	public ParticleSystem pickupParticle;
	public AudioClip pickupAudio;

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
			PickupData lPickup = GetComponent<PickupData> ();
			int lScore = 0;
			if (lPickup) {
				lScore = lPickup.score;
				//GameObject.Find ("UIScript").GetComponent<UI> ().AddScore (lScore);
				AllLevels.Get ().levelController.AddScore (lScore);
			}
			lPickup.gameObject.SetActive (false);
			Destroy (lPickup.gameObject);
			if (pickupParticle) {
				Instantiate (pickupParticle, transform.position, Quaternion.identity);
			}
			if (pickupAudio != null) {
				AudioSource.PlayClipAtPoint (pickupAudio, aCollider.gameObject.transform.position);
			} else {
				AllLevels.Get().levelController.PlayScoreAudio(lScore, transform.position);
			}
		}
	}
}
