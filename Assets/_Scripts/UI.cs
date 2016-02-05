using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UI : MonoBehaviour
{
	public GameObject playerPrefab;
	public int defaultLevel = 0;

	// Use this for initialization
	void Start ()
	{
		AllLevels.Get ().audioGlobal.Stop ();
		if (playerPrefab) {
			AllLevels.Get ().playerPrefab = playerPrefab;
		}
		if (defaultLevel > 0) {
			AllLevels.Get ().SetLevel (defaultLevel, false);
		}
		if (AllLevels.Get ().currentLevelSettings != null) {
			AllLevels.Get ().levelController.Generate (AllLevels.Get ().currentLevelSettings);
			AllLevels.Get ().levelController.StartLevelIntro ();
		}
	}
}
