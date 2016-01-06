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
	public RectTransform controlPanel;
	public int defaultLevel = 0;

	// Use this for initialization
	void Start ()
	{
		if (!mazeUIWidth) {
			mazeUIWidth = GameObject.Find ("InputFieldWidth").GetComponent<InputField> ();
		}
		if (!mazeUIDepth) {
			mazeUIDepth = GameObject.Find ("InputFieldDepth").GetComponent<InputField> ();
		}
		if (!mazeUIHeight) {
			mazeUIHeight = GameObject.Find ("InputFieldHeight").GetComponent<InputField> ();
		}
		if (!controlPanel) {
			controlPanel = GameObject.Find ("ControlPanel").GetComponent<RectTransform> ();
			controlPanel.gameObject.SetActive (false);
		}
		if (playerPrefab) {
			AllLevels.Get ().playerPrefab = playerPrefab;
		}
		if (defaultLevel > 0) {
			AllLevels.Get ().SetLevel(defaultLevel, false);
		}
		if (AllLevels.Get ().currentLevelSettings != null) {
			AllLevels.Get ().levelController.Generate(AllLevels.Get ().currentLevelSettings);
			AllLevels.Get ().levelController.StartLevel();
		}
	}
	
	public void ButtonExit ()
	{
		SceneManager.LoadScene ("ChooseLevel", LoadSceneMode.Single);
	}
}
