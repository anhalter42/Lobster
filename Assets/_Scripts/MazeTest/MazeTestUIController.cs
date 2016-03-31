using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class MazeTestUIController : MonoBehaviour
{

	public int CellWidth = 33;

	Slider m_SliderWidth;
	Slider m_SliderHeight;
	Slider m_SliderBreakWalls;
	Slider m_SliderCellWidth;
	Slider m_SliderSpeed;
	RawImage m_ImageMaze;

	Maze m_Maze = null;

	// Use this for initialization
	void Start ()
	{
		m_SliderWidth = GameObject.Find ("SliderWidth").GetComponent<Slider> ();
		m_SliderHeight = GameObject.Find ("SliderHeight").GetComponent<Slider> ();
		m_SliderBreakWalls = GameObject.Find ("SliderBreakWalls").GetComponent<Slider> ();
		m_SliderCellWidth = GameObject.Find ("SliderCellWidth").GetComponent<Slider> ();
		m_SliderCellWidth.value = Mathf.RoundToInt ((CellWidth - 1) / 2);
		m_SliderSpeed = GameObject.Find ("SliderSpeed").GetComponent<Slider> ();
		m_ImageMaze = GameObject.Find ("ImageMaze").GetComponent<RawImage> ();
		GenerateLabyrinth ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetKey (KeyCode.Escape)) {
			AllLevels.Get ().StartNewGame ();
		}
	}

	public void UpdateTexture ()
	{
		if (m_Maze != null) {
			CellWidth = Mathf.RoundToInt (m_SliderCellWidth.value) * 2 + 1;
			Texture2D lTex = new Texture2D (m_Maze.width * CellWidth, m_Maze.depth * CellWidth, TextureFormat.ARGB32, false);
			TextureUtils.DrawMaze (lTex, m_Maze, CellWidth, Color.black);
			lTex.Apply ();
			m_ImageMaze.texture = lTex;
			m_ImageMaze.SetNativeSize ();
		}
	}

	public void GenerateLabyrinth ()
	{
		StopCoroutine ("MazeStepBuild");
		if (m_Maze != null) {
			m_Maze.Changed -= OnMazeChanged;
		}
		m_Maze = new Maze (Mathf.RoundToInt (m_SliderWidth.value), 1, Mathf.RoundToInt (m_SliderHeight.value));
		m_Maze.chanceForBreakWalls = Mathf.RoundToInt (m_SliderBreakWalls.value);
		if (m_SliderSpeed.value <= 0) {
			m_Maze.build ();
			UpdateTexture ();
		} else {
			UpdateTexture ();
			m_Maze.Changed += OnMazeChanged;
			StartCoroutine ("MazeStepBuild");
		}
	}

	Maze.Cell fBuildCell = null;

	protected void OnMazeChanged (Maze aMaze, Maze.MazeEventArgs aArgs)
	{
		if (aArgs.cell != null) {
			TextureUtils.DrawMazeCell ((Texture2D)m_ImageMaze.texture, aArgs.cell, CellWidth, Color.black);
		} else if (aArgs.link != null) {
			if (aArgs.link.a != null) {
				TextureUtils.DrawMazeCell ((Texture2D)m_ImageMaze.texture, aArgs.link.a, CellWidth, Color.black);
			}
			if (aArgs.link.b != null) {
				TextureUtils.DrawMazeCell ((Texture2D)m_ImageMaze.texture, aArgs.link.b, CellWidth, Color.black);
			}
		}
		((Texture2D)m_ImageMaze.texture).Apply ();
	}

	protected void UnDrawCurrentMazeCell ()
	{
		if (fBuildCell != null) {
			TextureUtils.DrawMazeCell ((Texture2D)m_ImageMaze.texture, fBuildCell, CellWidth, Color.black);
			fBuildCell = null;
		}
	}

	protected void DrawCurrentMazeCell ()
	{
		UnDrawCurrentMazeCell ();
		fBuildCell = m_Maze.getCurrentBuildCell ();
		if (fBuildCell != null) {
			TextureUtils.DrawRect ((Texture2D)m_ImageMaze.texture, fBuildCell.x * CellWidth + CellWidth / 3, fBuildCell.z * CellWidth + CellWidth / 3, CellWidth / 3, CellWidth / 3, Color.red);
		}
		((Texture2D)m_ImageMaze.texture).Apply ();
	}

	protected IEnumerator MazeStepBuild ()
	{
		if (m_Maze != null) {
			while (m_Maze.buildNextStep ()) {
				DrawCurrentMazeCell ();
				yield return new WaitForSeconds (m_SliderSpeed.value);
			}
			UnDrawCurrentMazeCell ();
		}
	}

	bool usePos1 = true;
	Vector2 pos1;
	Vector2 pos2;
	int lx, ly;

	public void OnPointerClick (BaseEventData aData) //PointerEventData aEventData)
	{
		PointerEventData aEventData = (PointerEventData)aData;
		//if (usePos1) {
		if (RectTransformUtility.ScreenPointToLocalPointInRectangle (m_ImageMaze.rectTransform, aEventData.pressPosition, aEventData.pressEventCamera, out pos1)) {
			usePos1 = false;
			pos1 -= m_ImageMaze.rectTransform.rect.min;
			int x = Mathf.RoundToInt (pos1.x);
			int y = Mathf.RoundToInt (pos1.y);
			TextureUtils.DrawLine ((Texture2D)m_ImageMaze.texture, lx, ly, x, y, Color.green);
			lx = x;
			ly = y;
			Debug.Log (string.Format ("({0},{1})", x, y));
			//((Texture2D)m_ImageMaze.texture).SetPixel (x, y, Color.green);
			((Texture2D)m_ImageMaze.texture).Apply ();
		}
//		} else {
//			if (RectTransformUtility.ScreenPointToLocalPointInRectangle (m_ImageMaze.rectTransform, aEventData.pressPosition, aEventData.pressEventCamera, out pos2)) {
//				usePos1 = true;
//				pos2 -= m_ImageMaze.rectTransform.rect.min;
//			}
//		}
	}
}
