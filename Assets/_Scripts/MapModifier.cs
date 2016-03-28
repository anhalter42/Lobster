using UnityEngine;
using System.Collections;

public class MapModifier : MonoBehaviour {

	public enum Mode {
		Maze = 0,
		Texture = 1
	}

	public Mode mode = Mode.Maze;
	public Maze.Direction direction = Maze.Direction.No;
	public Color color = Color.white;
	public Texture2D texture;
	public bool ShowIfPlayerVisited = false;

}
