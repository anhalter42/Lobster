using UnityEngine;
using System.Collections;

public class MazeBuilder
{
	public CellDirectionObjects prefabs;
	public Transform parent;

	public LevelSettings settings;

	public Maze Maze;
	public Maze.Point exitPoint;

	protected void Init ()
	{
		Maze = new Maze (settings.mazeWidth, settings.mazeHeight, settings.mazeDepth);
		Maze.chanceForBreakWalls = settings.breakWalls;
		Maze.build ();
	}

	protected GameObject GetOneWall (int aDir)
	{
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
	}

	protected string GetWallTag (int aDir)
	{
		switch (aDir) {
		case Maze.DirectionTop:
			return "Top";
		case Maze.DirectionBottom:
			return "Bottom";
		case Maze.DirectionLeft:
			return "Left";
		case Maze.DirectionRight:
			return "Right";
		case Maze.DirectionForward:
			return "Forward";
		case Maze.DirectionBackward:
			return "Backward";
		default:
			return string.Empty;
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
		ArrayList lForLater = new ArrayList();
		GameObject[] lObjs = prefabs.GetSome (aObjs, aWithWall);
		int i;
		for (i = 0; i < lObjs.Length; i++) {
			CreateGameObject (lObjs [i], aParent, aNamePrefix + i.ToString (), lForLater);
		}
		for (int j = 0; j < lForLater.Count; j++, i++) {
			CreateGameObject (lForLater [j] as GameObject, aParent, aNamePrefix + i.ToString ());
		}
	}

	public void CreateLabyrinth (Transform aParent)
	{
		CreateLabyrinth (aParent, Vector3.zero);
	}

	public void CreateLabyrinth (Transform aParent, Vector3 aPos)
	{
		Debug.Log ("Creating Labyrinth...");
		Init ();
		parent = aParent;
		if (aParent) {
			for (int i = aParent.transform.childCount - 1; i >= 0; i--) {
				GameObject.DestroyObject (aParent.transform.GetChild (i).gameObject);
			}
		}
		GameObject lWallParent = new GameObject ("Walls");
		GameObject lMarkerParent = new GameObject ("Markers");
		if (aParent) {
			lWallParent.transform.SetParent (aParent, false);
			lMarkerParent.transform.SetParent (aParent, false);
		}
		Vector3 lPos = new Vector3 ();
		for (int y = 0; y < Maze.height; y++) {
			GameObject lLevelParent = new GameObject ("Level_" + y.ToString ());
			if (aParent) {
				lLevelParent.transform.SetParent (lWallParent.transform, false);
			}
			for (int z = 0; z < Maze.depth; z++) {
				for (int x = 0; x < Maze.width; x++) {
					lPos.x = aPos.x + 1.0f * x;
					lPos.y = aPos.y + 1.0f * y;
					lPos.z = aPos.z + 1.0f * z;
					GameObject lCellObj = new GameObject ("Cell_" + y.ToString () + "_" + x.ToString () + "_" + z.ToString ());
					Maze.Cell lCell = Maze.get (x, y, z);
					lCell.gameObject = lCellObj;
					MazeCellComponent lCellComp = lCellObj.AddComponent<MazeCellComponent> ();
					lCellComp.cell = lCell;
					lCellObj.transform.SetParent (lLevelParent.transform, false);
					lCellObj.transform.localPosition = lPos;
					for (int lDir = 0; lDir < 6; lDir++) {
						if (!Maze.get (x, y, z).links [lDir].broken) {
							if (lDir > 0 || y != (Maze.height - 1)) { // oberstes Dach weglassen
								CreateGameObject (GetOneWall (lDir), lCellObj.transform, "Wall_" + lDir.ToString ());
							}
							lCellComp.SetTag (GetWallTag (lDir));
							DropSome (lDir, lCellObj.transform, true);
						} else {
							DropSome (lDir, lCellObj.transform, false);
						}
					}
					GameObject lscore1Prefab = prefabs.GetOneForScore (prefabs.score, 1);
					if (lscore1Prefab) {
						GameObject lScore = CreateGameObject (lscore1Prefab, lCellObj.transform, "Score_1"); //, Vector3.down * 0.25f) as GameObject;
						lScore.GetComponent<PickupData> ().score = 1;
					}
					DropSome ("Prop_", lCellObj.transform, prefabs.props, false);
				}
			}
		}
		Debug.Log ("Labyrinth created.");
	}

	public void ActivateWayPoints (Maze.Point aFrom, Maze.Point aTo)
	{
		Maze.WayPoint[] lWay = Maze.FindWay (aFrom, aTo);
		for (int i = 0; i < lWay.Length; i++) {
			Maze.WayPoint lP = lWay [i];
			CreateGameObject (prefabs.GetOneForScore (prefabs.wayPoints, lP.direction), lWay [i].cell.gameObject.transform, "Way_" + i.ToString ());
		}
	}

	public void ActivateExits ()
	{
		exitPoint = new Maze.Point (Random.Range (0, Maze.width - 1),
			Random.Range (0, Maze.height - 1),
			Random.Range (0, Maze.depth - 1));
		CreateGameObject (prefabs.GetOne (prefabs.exit), Maze.get (exitPoint).gameObject.transform, "Exit");
	}

	public Maze.Point GetMazePointFromLocal (Vector3 aPos)
	{
		return new Maze.Point (
			(int)(aPos.x + 0.5f),
			(int)(aPos.y + 0.5f),
			(int)(aPos.z + 0.5f));
	}

	public Maze.Point GetMazePoint (Vector3 aPos)
	{
		return GetMazePointFromLocal (aPos - parent.position);
	}

	public Maze.Point GetPlayerMazePoint ()
	{
		return GetMazePoint (AllLevels.Get ().player.transform.position);
	}

	public GameObject CreateGameObject (GameObject aPrefab, Transform aParent, string aName, ArrayList aForLater = null)
	{
		return CreateGameObject (aPrefab, aParent, aName, Vector3.zero, Quaternion.identity, aForLater);
	}

	public GameObject CreateGameObject (GameObject aPrefab, Transform aParent, string aName, Vector3 aPos, ArrayList aForLater = null)
	{
		return CreateGameObject (aPrefab, aParent, aName, aPos, Quaternion.identity, aForLater);
	}

	public GameObject CreateGameObject (GameObject aPrefab, Transform aParent, string aName, Vector3 aPos, Quaternion aRotation, ArrayList aForLater = null)
	{
		PrefabConditions lCond = aPrefab.GetComponent<PrefabConditions> ();
		if (lCond) {
			MazeCellComponent lCell = aParent.GetComponent<MazeCellComponent> ();
			if (lCell) {
				if (lCell.ContainsTags (lCond.forbiddenTags)) {
					return null;
				}
				if (!lCell.ContainsTags (lCond.mustHaveTags)) {
					if (aForLater != null) {
						aForLater.Add (aPrefab);
					}
					return null;
				}
				lCell.SetTags (lCond.ownTags);
			}
		}
		GameObject lObj = GameObject.Instantiate (aPrefab, aPos, aRotation) as GameObject;
		lObj.transform.SetParent (aParent, false);
		lObj.name = aName;
		PrefabModifier lMod = lObj.GetComponent<PrefabModifier> ();
		if (lMod) {
			lMod.ModifyPrefab (lObj);
		}
		return lObj;
	}
}
