using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MiniMap : MonoBehaviour {

	public UI ui;
	Texture2D m_texture;
	Image m_Image;

	Maze.Point m_lastPoint = null;
	Maze.Point m_CurrentPoint = null;

	// Use this for initialization
	void Start () {
		if (!ui) {
			ui = GameObject.Find ("UIScript").GetComponent<UI> ();
		}
		m_Image = GetComponent<Image>();
		m_texture = new Texture2D(100,100);
		m_Image.material.mainTexture = m_texture;
	}
	
	// Update is called once per frame
	void LateUpdate () {
		if (ui) {
			m_CurrentPoint = ui.mazeBuilder.GetPlayerMazePoint();
			if (m_CurrentPoint != m_lastPoint) {
				m_texture.SetPixel(m_CurrentPoint.x * 2, m_CurrentPoint.y * 2, Color.black);
				m_texture.Apply(false);
				m_lastPoint = m_CurrentPoint;
			}
		}
	}
}
