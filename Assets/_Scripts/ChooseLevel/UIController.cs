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

		RawImage lImage = GameObject.Find ("RawImage").GetComponent<RawImage> ();
		Texture2D lTex;
		int w = 11;
		lTex = new Texture2D (fBuilder.Maze.width * w + 1, fBuilder.Maze.depth * w + 1);
		lImage.texture = lTex;
		for (int x = 0; x < fBuilder.Maze.width; x++) {
			for (int z = 0; z < fBuilder.Maze.depth; z++) {
				for (int lDir = 2; lDir < 6; lDir++) {
					if (!fBuilder.Maze.get (x, 0, z).links [lDir].broken) {
						int x1 = 0, x2 = 0, y1 = 0, y2 = 0;
						//int h = fBuilder.Maze.depth - 1;
						int d = (w - 1) / 2;
						switch (lDir) {
						case Maze.DirectionLeft:
							x1 = d + x * w - d;
							x2 = x1;
							y1 = d + z * w - d;
							y2 = d + z * w + d + 1;
							break;
						case Maze.DirectionRight:
							x1 = d + x * w + d;
							x2 = x1;
							y1 = d + z * w - d;
							y2 = d + z * w + d + 1;
							break;
						case Maze.DirectionForward:
							x1 = d + x * w - d;
							x2 = d + x * w + d + 1;
							y1 = d + z * w + d;
							y2 = y1;
							break;
						case Maze.DirectionBackward:
							x1 = d + x * w - d;
							x2 = d + x * w + d + 1;
							y1 = d + z * w - d;
							y2 = y1;
							break;
						}
						DrawLine (lTex, 1 + x1, 1 + y1, 1 + x2, 1 + y2, Color.black);
					}
				}
			}
		}
		lTex.Apply (false);
		lImage.SetNativeSize ();
	}

	void DrawLine (Texture2D tex, int x0, int y0, int x1, int y1, Color col)
	{
		int dy = (int)(y1 - y0);
		int dx = (int)(x1 - x0);
		int stepx, stepy;

		if (dy < 0) {
			dy = -dy;
			stepy = -1;
		} else {
			stepy = 1;
		}
		if (dx < 0) {
			dx = -dx;
			stepx = -1;
		} else {
			stepx = 1;
		}
		dy <<= 1;
		dx <<= 1;

		float fraction = 0;

		tex.SetPixel (x0, y0, col);
		if (dx > dy) {
			fraction = dy - (dx >> 1);
			while (Mathf.Abs (x0 - x1) > 1) {
				if (fraction >= 0) {
					y0 += stepy;
					fraction -= dx;
				}
				x0 += stepx;
				fraction += dy;
				tex.SetPixel (x0, y0, col);
			}
		} else {
			fraction = dx - (dy >> 1);
			while (Mathf.Abs (y0 - y1) > 1) {
				if (fraction >= 0) {
					x0 += stepx;
					fraction -= dy;
				}
				y0 += stepy;
				fraction += dx;
				tex.SetPixel (x0, y0, col);
			}
		}
	}
}
