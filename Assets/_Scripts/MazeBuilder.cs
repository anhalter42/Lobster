﻿using UnityEngine;
using System.Collections;

public class MazeBuilder
{
	public CellDirectionObjects prefabs;
	public GameObject mazeWallPrefab;
	public GameObject mazeWallLeftPrefab;
	public GameObject mazeWallRightPrefab;
	public GameObject mazeWallForwardPrefab;
	public GameObject mazeWallBackwardPrefab;
	public GameObject mazeWallTopPrefab;
	public GameObject mazeWallBottomPrefab;
	public GameObject mazeMarkerPrefab;
	public GameObject mazeBarrelPrefab;
	public GameObject mazeArchPrefab;
	public GameObject score1Prefab;
	public float mazeWallScale = 1f;

	public LevelSettings settings;

	public Maze Maze;

	protected void Init ()
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

	protected GameObject GetOneWall (int aDir)
	{
		if (prefabs != null) {
			switch (aDir) {
			case Maze.DirectionTop:
				return prefabs.GetOne (prefabs.top);
			case Maze.DirectionBottom:
				return prefabs.GetOne (prefabs.bottom);
			case Maze.DirectionLeft:
				return prefabs.GetOne (prefabs.left);
			case Maze.DirectionRight:
				return prefabs.GetOne (prefabs.right);
			case Maze.DirectionForward:
				return prefabs.GetOne (prefabs.forward);
			case Maze.DirectionBackward:
				return prefabs.GetOne (prefabs.backward);
			default:
				return null;
			}
		} else {
			return fDirectionPrefabs [aDir];
		}
	}

	protected void DropSome (int aDir, Transform aParent, bool aWithWall)
	{
		switch (aDir) {
		case Maze.DirectionTop:
			DropSome ("Top_", aParent, prefabs.topProps, aWithWall);
			break;
		case Maze.DirectionBottom:
			DropSome ("Bottom_", aParent, prefabs.bottomProps, aWithWall);
			break;
		case Maze.DirectionLeft:
			DropSome ("Left_", aParent, prefabs.leftProps, aWithWall);
			break;
		case Maze.DirectionRight:
			DropSome ("Right_", aParent, prefabs.rightProps, aWithWall);
			break;
		case Maze.DirectionForward:
			DropSome ("Forward_", aParent, prefabs.forwardProps, aWithWall);
			break;
		case Maze.DirectionBackward:
			DropSome ("Backward_", aParent, prefabs.backwardProps, aWithWall);
			break;
		}
	}

	protected void DropSome (string aNamePrefix, Transform aParent, GameObjectChance[] aObjs, bool aWithWall)
	{
		GameObject[] lObjs = prefabs.GetSome (aObjs, aWithWall);
		for (int i = 0; i < lObjs.Length; i++) {
			GameObject lObj = GameObject.Instantiate (lObjs [i], Vector3.zero, Quaternion.identity) as GameObject;
			lObj.transform.SetParent (aParent, false);
			lObj.name = aNamePrefix + i.ToString ();
		}
	}

	public void CreateLabyrinth (Transform aParent, Vector3 aPos)
	{
		Debug.Log ("Creating Labyrinth...");
		Init ();
		if (aParent) {
			for (int i = aParent.transform.childCount - 1; i >= 0; i--) {
				GameObject.DestroyObject (aParent.transform.GetChild (i).gameObject);
			}
		}
		GameObject lWallParent = new GameObject ("Walls");
		GameObject lMarkerParent = new GameObject ("Markers");
		//GameObject lScoreParent = new GameObject ("Scores");
		if (aParent) {
			lWallParent.transform.SetParent (aParent, false);
			lMarkerParent.transform.SetParent (aParent, false);
			//lScoreParent.transform.SetParent (aParent, false);
		}
		Vector3 lPos = new Vector3 ();
		for (int y = 0; y < Maze.height; y++) {
			GameObject lLevelParent = new GameObject ("Level_" + y.ToString ());
			if (aParent) {
				lLevelParent.transform.SetParent (lWallParent.transform, false);
			}
			for (int z = 0; z < Maze.depth; z++) {
				for (int x = 0; x < Maze.width; x++) {
					lPos.x = aPos.x + 1.0f * x; //2.5f
					lPos.y = aPos.y + 1.0f * y; // 0;
					lPos.z = aPos.z + 1.0f * z; //2.5f
					GameObject lCellObj = new GameObject ("Cell_" + y.ToString () + "_" + x.ToString () + "_" + z.ToString ());
					Maze.Cell lCell = Maze.get (x, y, z);
					lCellObj.AddComponent<MazeCellComponent> ().cell = lCell;
					lCellObj.transform.SetParent (lLevelParent.transform, false);
					lCellObj.transform.localPosition = lPos;
					for (int lDir = 0; lDir < 6; lDir++) {
						if (lDir > 0 || y != (Maze.height - 1)) { // oberstes Dach weglassen
							if (!Maze.get (x, y, z).links [lDir].broken) {
								GameObject lWall = DropWall (
									                   GetOneWall (lDir), // fDirectionPrefabs [lDir],
									                   lCellObj.transform, // lWallParent.transform,
									//lPos + Maze.getDirectionVector (lDir) * (0.5f - mazeWallScale / 2),
									                   Maze.getDirectionVector (lDir) * (0.5f - mazeWallScale / 2),
									                   fDirectionScales [lDir]);
								//lWall.name = "Wall_" + y.ToString () + "_" + x.ToString () + "_" + z.ToString () + "_" + lDir.ToString ();
								lWall.name = "Wall_" + lDir.ToString ();
								DropSome (lDir, lCellObj.transform, true);
							} else {
								DropSome (lDir, lCellObj.transform, false);
							}
						}
					}
					if (mazeMarkerPrefab) {
						GameObject lMarker = GameObject.Instantiate (mazeMarkerPrefab, lPos, Quaternion.identity) as GameObject;
						lMarker.transform.SetParent (lMarkerParent.transform, false);
						lMarker.name = "M_" + y.ToString () + "_" + x.ToString () + "_" + z.ToString ();
					}
					if (prefabs != null) {
						GameObject lscore1Prefab = prefabs.GetOneForScore (prefabs.score, 1);
						if (lscore1Prefab) {
							GameObject lScore = GameObject.Instantiate (lscore1Prefab, Vector3.down * 0.25f, Quaternion.identity) as GameObject;
							//lScore.transform.SetParent (lScoreParent.transform, false);
							lScore.transform.SetParent (lCellObj.transform, false);
							lScore.name = "Score_" + lScore.GetComponent<PickupData> ().score.ToString ();
						}
						DropSome ("Prop_", lCellObj.transform, prefabs.props, false);
					} else {
						if (score1Prefab) {
							GameObject lScore = GameObject.Instantiate (score1Prefab, Vector3.down * 0.25f, Quaternion.identity) as GameObject;
							//lScore.transform.SetParent (lScoreParent.transform, false);
							lScore.transform.SetParent (lCellObj.transform, false);
							lScore.name = "Score_" + lScore.GetComponent<PickupData> ().score.ToString ();
						}
						if (mazeBarrelPrefab && Random.Range (0, 100) > 75) {
							GameObject lBarrel = GameObject.Instantiate (mazeBarrelPrefab, Vector3.zero, Quaternion.identity) as GameObject;
							lBarrel.transform.SetParent (lCellObj.transform, false);
							lBarrel.name = "Barrel";
						}
					}
				}
			}
		}
		Debug.Log ("Labyrinth created.");
	}
}
