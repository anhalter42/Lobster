using UnityEngine;
using System.Collections;

public class PlayerDamage : MonoBehaviour
{
	void OnTriggerEnter (Collider aOther)
	{
		DamageData lDamage = aOther.GetComponent<DamageData> ();
		if (lDamage && lDamage.enabled) {
			if (lDamage.minVelocity == 0f
			    || (aOther.attachedRigidbody != null && aOther.attachedRigidbody.velocity.magnitude >= lDamage.minVelocity)) {
				if (lDamage.onlyOneTime) {
					lDamage.enabled = false;
				}
				AllLevels.Get ().levelController.TakeDamage (lDamage);
			}
		}
	}
}
