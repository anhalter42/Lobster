using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MiniMap : MonoBehaviour
{
	public int Range = 3;
	public int CellWidth = 17;
	LevelController controller;
	Texture2D m_texture;
	RawImage m_Image;

	int m_cellWidth;

	// Use this for initialization
	void Start ()
	{
		controller = AllLevels.Get ().levelController;
		controller.builder.Maze.Changed += OnMazeChanged;
		m_Image = GetComponent<RawImage> ();
		CreateTexture ();
	}

	void CreateTexture ()
	{
		m_texture = new Texture2D (controller.builder.Maze.width * CellWidth, controller.builder.Maze.depth * CellWidth);
		m_Image.texture = m_texture;
		m_cellWidth = CellWidth;
		TextureUtils.DrawMaze (m_texture, controller.builder.Maze, m_cellWidth, Color.black);
		m_texture.Apply ();
	}

	protected void OnMazeChanged (Maze aMaze, Maze.MazeEventArgs aArgs)
	{
		if (aArgs.cell != null) {
			TextureUtils.DrawMazeCell (m_texture, aArgs.cell, m_cellWidth, Color.black);
		} else if (aArgs.link != null) {
			if (aArgs.link.a != null) {
				TextureUtils.DrawMazeCell (m_texture, aArgs.link.a, m_cellWidth, Color.black);
			}
			if (aArgs.link.b != null) {
				TextureUtils.DrawMazeCell (m_texture, aArgs.link.b, m_cellWidth, Color.black);
			}
		}
		m_texture.Apply ();
	}
	
	// Update is called once per frame
	void LateUpdate ()
	{
		if (controller) {
			if (m_cellWidth != CellWidth) {
				CreateTexture ();
			}
			Vector3 lPos = controller.builder.GetPlayerMazePointV ();
			int rw = Range * 2 + 1;
			float w = m_cellWidth;
			float lW = w / ((float)m_texture.width);
			float lH = w / ((float)m_texture.height);
			float lX = Mathf.Min (Mathf.Max (0f, lPos.x - Range), (float)controller.builder.Maze.width - rw);
			float lY = Mathf.Min (Mathf.Max (0f, lPos.z - Range), (float)controller.builder.Maze.depth - rw);
			m_Image.uvRect = new Rect (lX * lW, lY * lH, rw * lW, rw * lH);
		}
	}

	public void SaveMap ()
	{
		byte[] lDatas = m_texture.EncodeToPNG ();
		string lPath = System.IO.Path.Combine (Application.persistentDataPath, controller.settings.levelName + ".png");
		System.IO.File.WriteAllBytes (lPath, lDatas);
		Debug.Log (string.Format ("Saved under '{0}' ({1} bytes)", lPath, lDatas.Length));
	}
}
