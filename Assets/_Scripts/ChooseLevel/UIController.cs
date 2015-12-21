using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class UIController : MonoBehaviour
{

	public Text textLevel;
	public int level = 1;

	// Use this for initialization
	void Start ()
	{
		if (!textLevel) {
			textLevel = GameObject.Find ("TextLevel").GetComponent<Text> ();
		}
		if (AllLevels.Get ().currentLevelSettings != null) {
			level = AllLevels.Get ().currentLevelSettings.level;
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (AllLevels.Get ().levelSettings != null && level <= AllLevels.Get ().levelSettings.Length) {
			LevelSettings lSet = AllLevels.Get ().levelSettings [level - 1];
			AllLevels.Get ().currentLevelSettings = lSet;
			textLevel.text = lSet.level.ToString () + " " + lSet.levelName + "\n" + lSet.levelDescription;
		} else {
			textLevel.text = "NO LEVELS";
		}
	}

	public void ButtonNext ()
	{
		if (AllLevels.Get ().hasLevels() && level < AllLevels.Get ().levelSettings.Length) {
			level++;
		}
	}

	public void ButtonPrevious ()
	{
		if (AllLevels.Get ().hasLevels() && level > 1) {
			level--;
		}
	}

	public void ButtonPlay ()
	{
		SceneManager.LoadScene ("Main", LoadSceneMode.Single);
	}
}
