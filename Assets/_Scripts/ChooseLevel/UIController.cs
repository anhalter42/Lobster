using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class UIController : MonoBehaviour
{

	public Text m_textLevel;
	public Text m_textName;
	public Text m_textDescription;
	public Text m_textHits;
	public Text m_textWord;
	public int level = 1;
	public GameObject mazeWallPrefab;
	ChooseLevelMazeBuilder fBuilder = new ChooseLevelMazeBuilder ();

	// Use this for initialization
	void Start ()
	{
		if (!m_textLevel)
			m_textLevel = GameObject.Find ("TextLevel").GetComponent<Text> ();
		if (!m_textName)
			m_textName = GameObject.Find ("TextName").GetComponent<Text> ();
		if (!m_textDescription)
			m_textDescription = GameObject.Find ("TextDescription").GetComponent<Text> ();
		if (!m_textHits)
			m_textHits = GameObject.Find ("TextHits").GetComponent<Text> ();
		if (!m_textWord)
			m_textWord = GameObject.Find ("TextWorld").GetComponent<Text> ();
		
		if (AllLevels.Get ().currentLevelSettings != null) {
			level = AllLevels.Get ().currentLevelSettings.level;
		}
		fBuilder.mazeParent = GameObject.Find ("Maze");
		fBuilder.mazeWallPrefab = mazeWallPrefab;
		fBuilder.mazeWallScale = 0.125f;
		CreateLabyrinth ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (AllLevels.Get ().levelSettings != null && level <= AllLevels.Get ().levelSettings.Length) {
			LevelSettings lSet = AllLevels.Get ().levelSettings [level - 1];
			AllLevels.Get ().currentLevelSettings = lSet;
			m_textLevel.text = lSet.level.ToString ();
			m_textName.text = lSet.levelName;
			m_textDescription.text = lSet.levelDescription;
			m_textWord.text = AllLevels.Get ().GetCellDescription (lSet.prefabs).worldName;
			m_textHits.text = string.Format ("{0}x{1}x{2} Score: {3} Time: {4}", lSet.mazeWidth, lSet.mazeDepth, lSet.mazeHeight, lSet.scoreForExit, lSet.maxTime);
		}
		if (Input.GetKeyUp (KeyCode.LeftArrow)) {
			ButtonPrevious ();
		}
		if (Input.GetKeyUp (KeyCode.RightArrow)) {
			ButtonNext ();
		}
	}

	public void ButtonNext ()
	{
		if (AllLevels.Get ().hasLevels () && level < AllLevels.Get ().levelSettings.Length) {
			level++;
			CreateLabyrinth ();
		}
	}

	public void ButtonPrevious ()
	{
		if (AllLevels.Get ().hasLevels () && level > 1) {
			level--;
			CreateLabyrinth ();
		}
	}

	public void ButtonPlay ()
	{
		SceneManager.LoadScene ("Main", LoadSceneMode.Single);
	}

	void CreateLabyrinth ()
	{
		fBuilder.settings = AllLevels.Get ().levelSettings [level - 1];
		fBuilder.mazeParent.transform.localPosition = new Vector3 (
			-fBuilder.settings.mazeWidth / 2f,
			0f,
			-fBuilder.settings.mazeDepth / 2f);
		fBuilder.CreateLabyrinth ();

		/*
		RawImage lImage = GameObject.Find ("RawImage").GetComponent<RawImage> ();
		Texture2D lTex;
		if (lImage.texture == null) {
			lTex = new Texture2D (100, 100);
			lImage.texture = lTex;
		} else {
			lTex = lImage.texture as Texture2D;
		}
		for (int x = 0; x < 10; x++) {
			for (int z = 0; z < 10; z++) {
				lTex.SetPixel (x, z, Color.blue);
			}
		}
		lTex.Apply (false);
		lImage.SetNativeSize ();
		*/
	}
}
