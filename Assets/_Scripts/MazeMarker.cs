using UnityEngine;
using System.Collections;

public class MazeMarker : MonoBehaviour
{

	LevelController controller;
	public GameObject marker;

	protected GameObject fMarker;

	// Use this for initialization
	void Start ()
	{
		controller = AllLevels.Get().levelController;
		if (marker) {
			fMarker = Instantiate (marker, Vector3.zero, Quaternion.identity) as GameObject;
		}
	}

	void FixedUpdate ()
	{
		if (controller.mazeParent != null) {
			Maze.Point lMazePos = controller.builder.GetMazePoint(transform.position);
			fMarker.transform.SetParent (controller.builder.Maze.get (lMazePos).gameObject.transform, false);
		}
	}

}
