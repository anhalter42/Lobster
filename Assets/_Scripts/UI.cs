using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class UI : MonoBehaviour
{
	public GameObject playerPrefab;
	public InputField mazeUIWidth;
	public InputField mazeUIDepth;
	public InputField mazeUIHeight;
	public int defaultLevel = 0;

	// Use this for initialization
	void Start ()
	{
		if (playerPrefab) {
			AllLevels.Get ().playerPrefab = playerPrefab;
		}
		if (defaultLevel > 0) {
			AllLevels.Get ().SetLevel(defaultLevel, false);
		}
		if (AllLevels.Get ().currentLevelSettings != null) {
			AllLevels.Get ().levelController.Generate(AllLevels.Get ().currentLevelSettings);
			AllLevels.Get ().levelController.StartLevelIntro();
		}
	}
	
	public void ButtonExit ()
	{
		SceneManager.LoadScene ("ChooseLevel", LoadSceneMode.Single);
	}
}
