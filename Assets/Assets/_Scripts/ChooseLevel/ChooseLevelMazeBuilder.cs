using UnityEngine;
using System.Collections;

public class ChooseLevelMazeBuilder {

	public GameObject mazeWallPrefab;
	public GameObject mazeWallLeftPrefab;
	public GameObject mazeWallRightPrefab;
	public GameObject mazeWallForwardPrefab;
	public GameObject mazeWallBackwardPrefab;
	public GameObject mazeWallTopPrefab;
	public GameObject mazeWallBottomPrefab;
	public GameObject mazeMarkerPrefab;
	public GameObject score1Prefab;
	public GameObject mazeParent;

	public float mazeWallScale = 1f;

	public LevelSettings settings;

	public Maze Maze;

	protected void Init()
	{
		fDirectionScales [Maze.DirectionTop] = new Vector3 (1, mazeWallScale, 1);
		fDirectionScales [Maze.DirectionBottom] = new Vector3 (1, mazeWallScale, 1);
		fDirectionScales [Maze.DirectionRight] = new Vector3 (mazeWallScale, 1, 1);
		fDirectionScales [Maze.DirectionLeft] = new Vector3 (mazeWallScale, 1, 1);
		fDirectionScales [Maze.DirectionForward] = new Vector3 (1, 1, mazeWallScale);
		fDirectionScales [Maze.DirectionBackward] = new Vector3 (1, 1, mazeWallScale);
		fDirectionPrefabs [Maze.DirectionTop] = mazeWallTopPrefab ? mazeWallTopPrefab : mazeWallPrefab;
		fDirectionPrefabs [Maze.DirectionBottom] = mazeWallBottomPrefab ? mazeWallBottomPrefab : mazeWallPrefab;
		fDirectionPrefabs [Maze.DirectionRight] = mazeWallRightPrefab ? mazeWallRightPrefab : mazeWallPrefab;
		fDirectionPrefabs [Maze.DirectionLeft] = mazeWallLeftPrefab ? mazeWallLeftPrefab : mazeWallPrefab;
		fDirectionPrefabs [Maze.DirectionForward] = mazeWallForwardPrefab ? mazeWallForwardPrefab : mazeWallPrefab;
		fDirectionPrefabs [Maze.DirectionBackward] = mazeWallBackwardPrefab ? mazeWallBackwardPrefab : mazeWallPrefab;
		Maze = new Maze (settings.mazeWidth, settings.mazeHeight, settings.mazeDepth);
		Maze.chanceForBreakWalls = settings.breakWalls;
		Maze.build ();
	}

	protected GameObject DropWall (GameObject aWallPrefab, Transform aParent, Vector3 aPos, Vector3 aScale)
	{
		//RaycastHit lHit;
		Vector3 lPos = aPos;
		//if (Physics.Raycast (aPos, Vector3.down, out lHit, 120.0f)) {
		//	lPos = new Vector3 (lHit.point.x, lHit.point.y + 0.4f, lHit.point.z);
		//}
		Quaternion lQ = new Quaternion ();
		//lQ.eulerAngles = new Vector3(Random.Range(-10.0f,10.0f),Random.Range(-10.0f,10.0f),Random.Range(-10.0f,10.0f));
		//Network.Instantiate(mazeWallPrefab, lPos, lQ, NetworkManager.GROUP_WORLD);
		GameObject lObj = GameObject.Instantiate (aWallPrefab ? aWallPrefab : mazeWallPrefab, lPos, lQ) as GameObject;
		lObj.transform.localScale = aScale;
		if (aParent) {
			lObj.transform.SetParent (aParent, false);
		}
		return lObj;
	}

	protected Vector3[] fDirectionScales = new Vector3[6];
	protected GameObject[] fDirectionPrefabs = new GameObject[6];

	public void CreateLabyrinth ()
	{
		Debug.Log ("Creating Labyrinth...");
		Init();
		if (mazeParent) {
			for (int i = mazeParent.transform.childCount - 1; i >= 0; i--) {
				GameObject.DestroyObject (mazeParent.transform.GetChild (i).gameObject);
			}
		}
		GameObject lWallParent = new GameObject ("Walls");
		GameObject lMarkerParent = new GameObject ("Markers");
		GameObject lScoreParent = new GameObject ("Scores");
		if (mazeParent) {
			lWallParent.transform.SetParent (mazeParent.transform, false);
			lMarkerParent.transform.SetParent (mazeParent.transform, false);
			lScoreParent.transform.SetParent (mazeParent.transform, false);
		}
		Vector3 lPos = new Vector3 ();
		for (int z = 0; z< Maze.depth; z++) {
			for (int y = 0; y < Maze.height; y++) {
				for (int x = 0; x < Maze.width; x++) {
					lPos.x = 1.0f * x;
					lPos.y = 1.0f * y;
					lPos.z = 1.0f * z;
					for (int lDir = 0; lDir<6; lDir++) {
						if (lDir > 0 || y != (Maze.height - 1)) { // oberstes Dach weglassen
							if (!Maze.get (x, y, z).links [lDir].broken) {
								GameObject lWall = DropWall (
									fDirectionPrefabs [lDir],
									lWallParent.transform,
									lPos + Maze.getDirectionVector (lDir) * (0.5f - mazeWallScale / 2),
									fDirectionScales [lDir]);
								lWall.name = "Wall_" + y.ToString () + "_" + x.ToString () + "_" + z.ToString () + "_" + lDir.ToString ();
							}
						}
					}
					if (mazeMarkerPrefab) {
						GameObject lMarker = GameObject.Instantiate (mazeMarkerPrefab, lPos, Quaternion.identity) as GameObject;
						lMarker.transform.SetParent (lMarkerParent.transform, false);
						lMarker.name = "M_" + y.ToString () + "_" + x.ToString () + "_" + z.ToString ();
					}
					if (score1Prefab) {
						GameObject lScore = GameObject.Instantiate (score1Prefab, lPos + Vector3.down * 0.25f, Quaternion.identity) as GameObject;
						lScore.transform.SetParent (lScoreParent.transform, false);
						lScore.name = "S_" + y.ToString () + "_" + x.ToString () + "_" + z.ToString ();
					}
				}
			}
		}
		Debug.Log ("Labyrinth created.");
	}
}
