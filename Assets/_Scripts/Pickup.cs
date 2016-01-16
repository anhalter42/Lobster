using UnityEngine;
using System.Collections;

public class Pickup : MonoBehaviour
{
	public ParticleSystem pickupParticle;
	public AudioClip pickupAudio;

	void OnTriggerEnter (Collider aCollider)
	{
		if (aCollider.gameObject.tag == "Player") {
			PickupData lPickup = GetComponent<PickupData> ();
			if (lPickup) {
				AllLevels.Get ().levelController.AddPickupData (lPickup);
			}
			lPickup.gameObject.SetActive (false);
			Destroy (lPickup.gameObject);
			if (pickupParticle) {
				Instantiate (pickupParticle, transform.position, Quaternion.identity);
			}
			if (pickupAudio != null) {
				AudioSource.PlayClipAtPoint (pickupAudio, aCollider.gameObject.transform.position);
			}
		}
	}
}
