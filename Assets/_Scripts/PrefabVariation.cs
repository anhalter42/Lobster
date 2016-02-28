using UnityEngine;
using System.Collections;

public class PrefabVariation : MonoBehaviour
{

	public enum Mode
	{
		ActivateZeroOrOne = 6,
		ActivateOne = 0,
		ActivateSome = 1,
		ActivateSomeAtleastOne = 2,
		DeactivateZeroOrOne = 7,
		DeactivateOne = 3,
		DeactivateSome = 4,
		DeactivateSomeAtleastOne = 5,
	}

	public int probability = 50;
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
		case Mode.ActivateZeroOrOne:
			ModifyForActivateZeroOrOne (true);
			break;
		case Mode.ActivateOne:
			ModifyForActivateOne (true);
			break;
		case Mode.ActivateSome:
			ModifyForActivateSome (0, true);
			break;
		case Mode.ActivateSomeAtleastOne:
			ModifyForActivateSome (1, true);
			break;
		case Mode.DeactivateZeroOrOne:
			ModifyForActivateZeroOrOne (false);
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

	void ModifyForActivateZeroOrOne (bool aState = true)
	{
		if (Random.Range (0, 100) <= probability) {
			ModifyForActivateOne (aState);
		}
	}

	void ModifyForActivateOne (bool aState = true)
	{
		if (behaviours.Length > 0) {
			behaviours [Random.Range (0, behaviours.Length - 1)].enabled = aState;
		}
		if (objects.Length > 0) {
			int j = Random.Range (0, objects.Length - 1);
			objects [j].SetActive (aState);
			/*
			if (aState) {
				PrefabModifier[] lMods = objects [j].GetComponentsInChildren<PrefabModifier> ();
				foreach (PrefabModifier lMod in lMods) {
					lMod.ModifyPrefab (lMod.gameObject);
				}
			}
			*/
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
			int i = 0, x = 0;
			while (i < lCount) {
				int j = Random.Range (0, behaviours.Length);
				if (behaviours [j].enabled != aState) {
					if (Random.Range (0, 100) <= probability) {
						behaviours [j].enabled = aState;
					}
					i++;
				} else {
					x++;
					if (x > 1000)
						break;
				}
			}
		}
		if (objects.Length > 0) {
			int lCount = Random.Range (MinMax (aMin, objects.Length - 1), objects.Length + 1);
			int i = 0, x = 0;
			while (i < lCount) {
				int j = Random.Range (0, objects.Length);
				if (objects [j].activeSelf != aState) {
					if (Random.Range (0, 100) <= probability) {
						objects [j].SetActive (aState);
					}
					i++;
				} else {
					x++;
					if (x > 1000)
						break;
				}
			}
		}
	}

}
