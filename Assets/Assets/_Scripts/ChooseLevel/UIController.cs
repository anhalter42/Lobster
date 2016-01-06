using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class UIController : MonoBehaviour
{

	public Text textLevel;
	public int level = 1;
	public GameObject mazeWallPrefab;
	ChooseLevelMazeBuilder fBuilder = new ChooseLevelMazeBuilder();

	// Use this for initialization
	void Start ()
	{
		if (!textLevel) {
			textLevel = GameObject.Find ("TextLevel").GetComponent<Text> ();
		}
		if (AllLevels.Get ().currentLevelSettings != null) {
			level = AllLevels.Get ().currentLevelSettings.level;
		}
		fBuilder.mazeParent = GameObject.Find("Maze");
		fBuilder.mazeWallPrefab = mazeWallPrefab;
		fBuilder.mazeWallScale = 0.125f;
		CreateLabyrinth();
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
		if (Input.GetKeyUp(KeyCode.LeftArrow)) {
			ButtonPrevious();
		}
		if (Input.GetKeyUp(KeyCode.RightArrow)) {
			ButtonNext();
		}
	}

	public void ButtonNext ()
	{
		if (AllLevels.Get ().hasLevels () && level < AllLevels.Get ().levelSettings.Length) {
			level++;
			CreateLabyrinth();
		}
	}

	public void ButtonPrevious ()
	{
		if (AllLevels.Get ().hasLevels () && level > 1) {
			level--;
			CreateLabyrinth();
		}
	}

	public void ButtonPlay ()
	{
		SceneManager.LoadScene ("Main", LoadSceneMode.Single);
	}

	void CreateLabyrinth() {
		fBuilder.settings = AllLevels.Get().levelSettings[level-1];
/*		fBuilder.mazeParent.transform.localScale = new Vector3(
			fBuilder.settings.mazeWidth/10f,
			fBuilder.settings.mazeHeight/2f,
			fBuilder.settings.mazeDepth/10f);*/
		fBuilder.mazeParent.transform.localPosition = new Vector3(
			-fBuilder.settings.mazeWidth/2f,
			0f,
			-fBuilder.settings.mazeDepth/2f);
		fBuilder.CreateLabyrinth();
	}
}
