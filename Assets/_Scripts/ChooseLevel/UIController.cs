using UnityEngine;
using UnityEngine.UI;

//using UnityEngine.SceneManagement;
using System.Collections;

public class UIController : MonoBehaviour
{

	public Text m_textLevel;
	public Text m_textName;
	public Text m_textDescription;
	public Text m_textHits;
	public Text m_textWord;
	public GameObject mazeWallPrefab;
	public LevelSettings settings;
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
			settings = AllLevels.Get ().currentLevelSettings;
		}
		fBuilder.mazeParent = GameObject.Find ("Maze");
		fBuilder.mazeWallPrefab = mazeWallPrefab;
		fBuilder.mazeWallScale = 0.125f;
		CreateLabyrinth ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (settings != null) {
			m_textLevel.text = settings.level.ToString ();
			m_textName.text = settings.name;
			m_textDescription.text = settings.levelDescription;
			m_textWord.text = AllLevels.Get ().GetCellDescription (settings.prefabs).worldName;
			m_textHits.text = string.Format ("{0}x{1}x{2} Score: {3} Time: {4}", settings.mazeWidth, settings.mazeDepth, settings.mazeHeight, settings.scoreForExit, settings.maxTime);
		}
		if (Input.GetKeyUp (KeyCode.LeftArrow)) {
			ButtonPrevious ();
		}
		if (Input.GetKeyUp (KeyCode.RightArrow)) {
			ButtonNext ();
		}
		if (Input.GetKeyUp (KeyCode.Space)) {
			ButtonPlay ();
		}
		if (Input.GetKeyUp (KeyCode.Escape)) {
			ButtonBack ();
		}
	}

	public void ButtonNext ()
	{
		LevelSettings lNext = AllLevels.Get ().GetNextLevel(settings);
		if (lNext != null) {
			settings = lNext;
			CreateLabyrinth ();
		}
	}

	public void ButtonPrevious ()
	{
		LevelSettings lPrev = AllLevels.Get ().GetPreviousLevel(settings);
		if (lPrev != null) {
			settings = lPrev;
			CreateLabyrinth ();
		}
	}

	public void ButtonPlay ()
	{
		AllLevels.Get ().currentLevelSettings = settings;
		AllLevels.Get ().StartLevel ();
	}

	public void ButtonBack ()
	{
		AllLevels.Get ().StartNewGame ();
	}

	public void ButtonProfile ()
	{
		AllLevels.Get ().StartProfile ();
	}

	void CreateLabyrinth ()
	{
		fBuilder.settings = settings;
		fBuilder.mazeParent.transform.localPosition = new Vector3 (
			-fBuilder.settings.mazeWidth / 2f + 0.5f,
			0f,
			-fBuilder.settings.mazeDepth / 2f + 0.5f);
		fBuilder.CreateLabyrinth ();

		RawImage lImage = GameObject.Find ("RawImage").GetComponent<RawImage> ();
		Texture2D lTex;
		int w = 7;
		lTex = new Texture2D (fBuilder.Maze.width * w, fBuilder.Maze.depth * w);
		lImage.texture = lTex;
		TextureUtils.DrawMaze (lTex, fBuilder.Maze, w, Color.black);
		lTex.Apply (false);
		lImage.SetNativeSize ();
	}
}
