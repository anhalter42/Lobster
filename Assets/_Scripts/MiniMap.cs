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

	Maze.Point m_lastPoint = null;
	Maze.Point m_CurrentPoint = null;
	int m_SameCount = 0;
	int m_cellWidth;

	// Use this for initialization
	void Start ()
	{
		controller = AllLevels.Get ().levelController;
		m_Image = GetComponent<RawImage> ();
		CreateTexture();
	}

	void CreateTexture()
	{
		m_texture = new Texture2D (controller.builder.Maze.width * CellWidth, controller.builder.Maze.depth * CellWidth);
		m_Image.texture = m_texture;
		m_cellWidth = CellWidth;
	}
	
	// Update is called once per frame
	void LateUpdate ()
	{
		if (controller) {
			m_CurrentPoint = controller.builder.GetPlayerMazePoint ();
			if (m_CurrentPoint != m_lastPoint || m_SameCount < 3) {
				if (m_cellWidth != CellWidth) {
					CreateTexture();
				}
				if (m_CurrentPoint == m_lastPoint) {
					m_SameCount++;
				} else {
					m_SameCount = 0;
				}
				int rw = Range * 2 + 1;
				m_lastPoint = m_CurrentPoint;
				int w = CellWidth;
				TextureUtils.DrawMaze (m_texture, controller.builder.Maze, w, Color.black);
				m_texture.Apply ();
				float lW = ((float)w) / ((float)m_texture.width);
				float lH = ((float)w) / ((float)m_texture.height);
				int lX = Mathf.Min (Mathf.Max (0, m_CurrentPoint.x - Range), controller.builder.Maze.width - rw);
				int lY = Mathf.Min (Mathf.Max (0, m_CurrentPoint.z - Range), controller.builder.Maze.depth - rw);
				m_Image.uvRect = new Rect (lX * lW, lY * lH, rw * lW, rw * lH);
			}
		}
	}
}
