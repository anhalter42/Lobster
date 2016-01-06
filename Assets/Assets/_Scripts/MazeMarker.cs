using UnityEngine;
using System.Collections;

public class MazeMarker : MonoBehaviour
{

	public UI ui;
	public GameObject marker;

	protected GameObject fMarker;

	// Use this for initialization
	void Start ()
	{
		if (!ui) {
			ui = GameObject.Find ("UIScript").GetComponent<UI> ();
		}
		if (marker) {
			fMarker = Instantiate (marker, Vector3.zero, Quaternion.identity) as GameObject;
		}
	}

	void FixedUpdate ()
	{
		if (ui != null && fMarker != null && ui.mazeBuilder != null && ui.mazeBuilder.parent != null) {
			//Vector3 lRelPos = transform.position - ui.mazeBuilder.parent.transform.position;
			//lRelPos.x += 0.5f;
			//lRelPos.y += 0.5f;
			//lRelPos.z += 0.5f;
			//Maze.Point lMazePos = new Maze.Point ((int)lRelPos.x, (int)lRelPos.y, (int)lRelPos.z);
			Maze.Point lMazePos = ui.mazeBuilder.GetMazePoint(transform.position);
			fMarker.transform.SetParent (ui.mazeBuilder.Maze.get (lMazePos).gameObject.transform, false);
		}
	}

}
