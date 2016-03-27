using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MiniMap : MonoBehaviour
{
	public int Range = 3;
	LevelController controller;
	Texture2D m_texture;
	RawImage m_Image;

	Maze.Point m_lastPoint = null;
	Maze.Point m_CurrentPoint = null;
	int m_SameCount = 0;

	// Use this for initialization
	void Start ()
	{
		controller = AllLevels.Get ().levelController;
		m_Image = GetComponent<RawImage> ();
	}
	
	// Update is called once per frame
	void LateUpdate ()
	{
		if (controller) {
			if (controller.mazeMapTexture) {
				if (m_texture == null) {
					m_texture = Instantiate (controller.mazeMapTexture) as Texture2D;
					m_Image.texture = m_texture;
				}
				m_CurrentPoint = controller.builder.GetPlayerMazePoint ();
				if (m_CurrentPoint != m_lastPoint || m_SameCount < 3) {
					if (m_CurrentPoint == m_lastPoint) {
						m_SameCount++;
					} else {
						m_SameCount = 0;
					}
					int rw = Range * 2 + 1;
					m_lastPoint = m_CurrentPoint;
					int w = 11;
					TextureUtils.DrawMaze (m_texture, controller.builder.Maze, w, Color.black);
					m_texture.Apply ();
					float lW = ((float)w) / ((float)controller.mazeMapTexture.width);
					float lH = ((float)w) / ((float)controller.mazeMapTexture.height);
					int lX = Mathf.Min (Mathf.Max (0, m_CurrentPoint.x - Range), controller.builder.Maze.width - rw);
					int lY = Mathf.Min (Mathf.Max (0, m_CurrentPoint.z - Range), controller.builder.Maze.depth - rw);
					m_Image.uvRect = new Rect (lX * lW, lY * lH, rw * lW, rw * lH);
				}
			}
		}
	}
}
