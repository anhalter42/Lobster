using UnityEngine;
using System.Collections;

public class PrefabVariation : MonoBehaviour
{

	public enum Mode
	{
		ActivateOne,
		ActivateSome,
		ActivateSomeAtleastOne,
		DeactivateOne,
		DeactivateSome,
		DeactivateSomeAtleastOne,
	}

	public Mode mode = Mode.ActivateOne;
	public Behaviour[] behaviours = { };
	public GameObject[] objects = { };

	bool modified = false;

	public void Start ()
	{
		if (!modified) {
			modified = true;
			ModifyPrefab ();
		}
	}

	public void ModifyPrefab ()
	{
		switch (mode) {
		case Mode.ActivateOne:
			ModifyForActivateOne (true);
			break;
		case Mode.ActivateSome:
			ModifyForActivateSome (0, true);
			break;
		case Mode.ActivateSomeAtleastOne:
			ModifyForActivateSome (1, true);
			break;
		case Mode.DeactivateOne:
			ModifyForActivateOne (false);
			break;
		case Mode.DeactivateSome:
			ModifyForActivateSome (0, false);
			break;
		case Mode.DeactivateSomeAtleastOne:
			ModifyForActivateSome (1, false);
			break;
		}
	}

	void ModifyForActivateOne (bool aState = true)
	{
		if (behaviours.Length > 0) {
			behaviours [Random.Range (0, behaviours.Length - 1)].enabled = aState;
		}
		if (objects.Length > 0) {
			objects [Random.Range (0, objects.Length - 1)].SetActive (aState);
		}
	}

	int MinMax (int a, int b)
	{
		return a < b ? a : b;
	}

	void ModifyForActivateSome (int aMin = 0, bool aState = true)
	{
		if (behaviours.Length > 0) {
			int lCount = Random.Range (MinMax (aMin, behaviours.Length - 1), behaviours.Length + 1);
			int i = 0;
			while (i < lCount) {
				int j = Random.Range (0, behaviours.Length);
				if (behaviours [j].enabled != aState) {
					behaviours [j].enabled = aState;
					i++;
				}
			}
		}
		if (objects.Length > 0) {
			int lCount = Random.Range (MinMax (aMin, objects.Length - 1), objects.Length + 1);
			int i = 0;
			while (i < lCount) {
				int j = Random.Range (0, objects.Length);
				if (objects [j].activeSelf != aState) {
					objects [j].SetActive (aState);
					i++;
				}
			}
		}
	}

}
