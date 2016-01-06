using UnityEngine;
using System.Collections;

public class PlayerDamage : MonoBehaviour
{
	void OnTriggerEnter (Collider aOther)
	{
		DamageData lDamage = aOther.GetComponent<DamageData> ();
		if (lDamage) {
			AllLevels.Get ().levelController.TakeDamage (lDamage);
		}
	}
}
