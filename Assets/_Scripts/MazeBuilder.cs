using UnityEngine;
using System.Collections;

public class MazeBuilder
{
	public CellDirectionObjects prefabs;
	public Transform parent;

	public Vector3 positionScale = new Vector3 (1f, 1f, 1f);

	public LevelSettings settings;

	public Maze Maze;
	public Maze.Point exitPoint;

	GameObject[] levels;
	GameObject[] exits;

	protected void Init ()
	{
		Maze = new Maze (settings.mazeWidth, settings.mazeHeight, settings.mazeDepth);
		Maze.chanceForBreakWalls = settings.breakWalls;
		prefabs = settings.PreparePrefabs (prefabs);
		settings.PrepareMaze (Maze);
		Maze.build ();
	}

	protected GameObject GetOneWall (int aDir, MazeCellComponent aCellComp)
	{
		switch (aDir) {
		case Maze.DirectionTop:
			return prefabs.GetOne (prefabs.top, aCellComp);
		case Maze.DirectionBottom:
			return prefabs.GetOne (prefabs.bottom, aCellComp);
		case Maze.DirectionLeft:
			return prefabs.GetOne (prefabs.left, aCellComp);
		case Maze.DirectionRight:
			return prefabs.GetOne (prefabs.right, aCellComp);
		case Maze.DirectionForward:
			return prefabs.GetOne (prefabs.forward, aCellComp);
		case Maze.DirectionBackward:
			return prefabs.GetOne (prefabs.backward, aCellComp);
		default:
			return null;
		}
	}

	public static string GetWallTag (int aDir)
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
			return "Prop";
		}
	}

	public static int GetWallDir (string aTag)
	{
		switch (aTag) {
		case "Top":
			return Maze.DirectionTop;
		case "Bottom":
			return Maze.DirectionBottom;
		case "Left":
			return Maze.DirectionLeft;
		case "Right":
			return Maze.DirectionRight;
		case "Forward":
			return Maze.DirectionForward;
		case "Backward":
			return Maze.DirectionBackward;
		default:
			return -1;
		}
	}

	protected void DropSome (int aDir, Transform aParent, bool aWithWall, ArrayList aForLater = null)
	{
		string lPrefix = GetWallTag (aDir) + "_";
		GameObjectChance[] lGOCs = null;
		switch (aDir) {
		case Maze.DirectionTop:
			lGOCs = prefabs.topProps;
			break;
		case Maze.DirectionBottom:
			lGOCs = prefabs.bottomProps;
			break;
		case Maze.DirectionLeft:
			lGOCs = prefabs.leftProps;
			break;
		case Maze.DirectionRight:
			lGOCs = prefabs.rightProps;
			break;
		case Maze.DirectionForward:
			lGOCs = prefabs.forwardProps;
			break;
		case Maze.DirectionBackward:
			lGOCs = prefabs.backwardProps;
			break;
		}
		if (lGOCs != null) {
			DropSome (lPrefix, aParent, lGOCs, aWithWall, aForLater);
		}
	}

	public class ForLater
	{
		public string prefix;
		public Transform parent;
		public GameObject prefab;

		public ForLater (string aPrefix, Transform aParent, GameObject aPrefab)
		{
			prefix = aPrefix;
			parent = aParent;
			prefab = aPrefab;
		}
	}

	protected void DropSome (string aNamePrefix, Transform aParent, GameObjectChance[] aObjs, bool aWithWall, ArrayList aForLater = null)
	{
		ArrayList lForLater = new ArrayList ();
		ArrayList lForLater2 = new ArrayList ();
		GameObject[] lObjs = prefabs.GetSome (aObjs, aWithWall, aParent.GetComponent<MazeCellComponent> ());
		int i;
		for (i = 0; i < lObjs.Length; i++) {
			CreateGameObject (lObjs [i], aParent, aNamePrefix + i.ToString (), lForLater);
		}
		for (int j = 0; j < lForLater.Count; j++, i++) {
			CreateGameObject (lForLater [j] as GameObject, aParent, aNamePrefix + i.ToString (), lForLater2);
		}
		if (aForLater != null) {
			for (int j = 0; j < lForLater2.Count; j++, i++) {
				aForLater.Add (new ForLater (aNamePrefix, aParent, lForLater2 [j] as GameObject));
			}
		}
	}

	public void CreateLabyrinth (Transform aParent)
	{
		CreateLabyrinth (aParent, Vector3.zero);
	}

	void CreateCellObjects (Transform aParent, Vector3 aPos)
	{
		levels = new GameObject[Maze.height];
		Vector3 lPos = new Vector3 ();
		for (int y = 0; y < Maze.height; y++) {
			GameObject lLevelParent = new GameObject ("Level_" + y.ToString ());
			levels [y] = lLevelParent;
			if (aParent) {
				lLevelParent.transform.SetParent (aParent, false);
			}
			for (int z = 0; z < Maze.depth; z++) {
				for (int x = 0; x < Maze.width; x++) {
					lPos.x = aPos.x + positionScale.x * x;
					lPos.y = aPos.y + positionScale.y * y;
					lPos.z = aPos.z + positionScale.z * z;
					GameObject lCellObj = new GameObject ("Cell_" + y.ToString () + "_" + x.ToString () + "_" + z.ToString ());
					Maze.Cell lCell = Maze.get (x, y, z);
					lCell.gameObject = lCellObj;
					MazeCellComponent lCellComp = lCellObj.AddComponent<MazeCellComponent> ();
					lCellComp.cell = lCell;
					lCellObj.transform.SetParent (lLevelParent.transform, false);
					lCellObj.transform.localPosition = lPos;
					bool lWallSet = false;
					for (int lDir = 0; lDir < 6; lDir++) {
						if (!Maze.get (x, y, z).links [lDir].broken) {
							lCellComp.SetTag (GetWallTag (lDir));
							lWallSet = true;
						} else {
							lCellComp.SetTag ("No" + GetWallTag (lDir));
						}
					}
					if (lWallSet) {
						lCellComp.SetTag ("Wall");
					}
				}
			}
		}
	}

	public void CreateLabyrinth (Transform aParent, Vector3 aPos)
	{
		Init ();
		int[] lDirs = { 0, 1, 2, 3, 4, 5 };
		parent = aParent;
		if (aParent) {
			for (int i = aParent.transform.childCount - 1; i >= 0; i--) {
				GameObject.DestroyObject (aParent.transform.GetChild (i).gameObject);
			}
		}
		CreateCellObjects (aParent, aPos);
		ArrayList lForLater = new ArrayList ();
		for (int y = 0; y < Maze.height; y++) {
			for (int z = 0; z < Maze.depth; z++) {
				for (int x = 0; x < Maze.width; x++) {
					Maze.Cell lCell = Maze.get (x, y, z);
					GameObject lCellObj = lCell.gameObject;
					MazeCellComponent lCellComp = lCellObj.GetComponent<MazeCellComponent> ();
					for (int lD = 0; lD < 6; lD++) {
						int lDir = lDirs [lD];
						if (!lCell.links [lDir].broken) {
							if (lDir > 0 || y != (Maze.height - 1)) { // oberstes Dach weglassen
								bool lSkipWall = false;
								if (!lCell.links [lDir].isBorder
								    && lCell.links [lDir].to (lCell).gameObject
								    && lCell.links [lDir].to (lCell).gameObject.GetComponent<MazeCellComponent> ().walls [Maze.getReverseDirection (lDir)]) {
									PrefabWallConditions lWC = lCell.links [lDir].to (lCell).gameObject.GetComponent<MazeCellComponent> ().walls [Maze.getReverseDirection (lDir)].GetComponent<PrefabWallConditions> ();
									if (lWC != null) {
										if (lWC.deleteOpositeWall) {
											lSkipWall = true;
										}
									}
								}
								if (!lSkipWall) {
									lCellComp.walls [lDir] = CreateGameObject (GetOneWall (lDir, lCellComp), lCellObj.transform, "Wall_" + lDir.ToString ());
									if (lCellComp.walls [lDir]) {
										PrefabWallConditions lWC = lCellComp.walls [lDir].GetComponent<PrefabWallConditions> ();
										if (lWC != null) {
											if (lWC.deleteOpositeWall) {
												if (!lCell.links [lDir].isBorder
												    && lCell.links [lDir].to (lCell).gameObject
												    && lCell.links [lDir].to (lCell).gameObject.GetComponent<MazeCellComponent> ().walls [Maze.getReverseDirection (lDir)]) {
													GameObject.Destroy (lCell.links [lDir].to (lCell).gameObject.GetComponent<MazeCellComponent> ().walls [Maze.getReverseDirection (lDir)]);
													lCell.links [lDir].to (lCell).gameObject.GetComponent<MazeCellComponent> ().walls [Maze.getReverseDirection (lDir)] = null;
												}
											}
										}
									} else {
										Debug.Log (string.Format ("Wall for direction {0} not found or has conditions!", GetWallTag (lDir)));
									}
								}
							}
							DropSome (lDir, lCellObj.transform, true, lForLater);
						} else {
							DropSome (lDir, lCellObj.transform, false, lForLater);
						}
					}

					// shift directions
					int lSwap = lDirs [0];
					for (int i = 0; i < (lDirs.Length - 1); i++) {
						lDirs [i] = lDirs [i + 1];
					}
					lDirs [lDirs.Length - 1] = lSwap;

					GameObject lscore1Prefab = prefabs.GetOneForScore (prefabs.score, 1);
					if (lscore1Prefab) {
						GameObject lScore = CreateGameObject (lscore1Prefab, lCellObj.transform, "Score_1");
						lScore.GetComponent<PickupData> ().score = 1;
					}
					DropSome ("Prop_", lCellObj.transform, prefabs.props, false, lForLater);
				}
			}
		}
		for (int j = 0; j < lForLater.Count; j++) {
			ForLater lFL = lForLater [j] as ForLater;
			CreateGameObject (lFL.prefab, lFL.parent, lFL.prefix + "L_" + j.ToString ());
		}
		CreateExits ();
	}

	protected void CreateExits ()
	{
		System.Array.Resize<GameObject> (ref exits, settings.exits.Length + 1);
		int lIndex = 0;
		foreach (LevelSettings.Exit lE in settings.exits) {
			exits [lIndex] = CreateExit (lE.pos, lE.prefab, lE.levelName);
			exits [lIndex].SetActive (settings.scoreForExit == 0);
			MultiActivator lMA = exits [lIndex].GetComponent<MultiActivator> ();
			if (!lMA) {
				lMA = exits [lIndex].AddComponent<MultiActivator> ();
			}
			lMA.inventoryItems = lE.items;
			lIndex++;
		}
		exits [lIndex] = CreateExit (exitPoint);
		if (exits [lIndex]) {
			exits [lIndex].SetActive (settings.scoreForExit == 0);
			exitPoint = exits [lIndex].transform.parent.GetComponent<MazeCellComponent> ().cell.pos;
		}
	}


	public void ActivateWayPoints (Maze.Point aFrom, Maze.Point aTo)
	{
		Maze.WayPoint[] lWay = Maze.FindShortestWay (aFrom, aTo);
		for (int i = 0; i < lWay.Length; i++) {
			Maze.WayPoint lP = lWay [i];
			CreateGameObject (prefabs.GetOneForScore (prefabs.wayPoints, lP.direction), lWay [i].cell.gameObject.transform, "Way_" + i.ToString ());
		}
	}

	public GameObject CreateExit (Maze.Point aPoint, GameObject aPrefab = null, string aLevelName = "NEXT")
	{
		Maze.Point lP;
		if (!Maze.Point.IsNullOrEmpty (aPoint)) {
			lP = aPoint;
		} else {
			lP = new Maze.Point (Random.Range (0, Maze.width),
				Random.Range (0, Maze.height),
				Random.Range (0, Maze.depth));
		}
		Transform lParent = Maze.get (lP).gameObject.transform;
		GameObject lPrefab = aPrefab != null ? aPrefab : prefabs.GetOne (prefabs.exit, lParent.GetComponent<MazeCellComponent> ());
		if (lPrefab) {
			GameObject lExit = CreateGameObject (lPrefab, lParent, "Exit");
			MultiActivator lMA = lExit.GetComponent<MultiActivator> ();
			if (!lMA) {
				lMA = lExit.AddComponent<MultiActivator> ();
			}
			System.Array.Resize<MultiActivator.ControlledMethod> (ref lMA.controlledMethods, lMA.controlledMethods.Length + 1);
			lMA.controlledMethods [lMA.controlledMethods.Length - 1] = new MultiActivator.ControlledMethod ()
				{ isRepeatable = false, method = string.Format ("Exit;", aLevelName) };
			return lExit;
		} else {
			Debug.Log ("There is no EXIT Prefab defined!");
			return null;
		}
	}

	public void ActivateExits ()
	{
		foreach (GameObject lE in exits) {
			lE.SetActive (true);
		}
	}

	public Vector3 GetVectorFromMazePoint (Maze.Point aPoint)
	{
		return new Vector3 (aPoint.x * positionScale.x, aPoint.y * positionScale.y, aPoint.z * positionScale.z);
	}

	public Maze.Point GetMazePointFromLocal (Vector3 aPos)
	{
		return new Maze.Point (
			(int)(aPos.x / positionScale.x + 0.5f),
			(int)(aPos.y / positionScale.y + 0.5f),
			(int)(aPos.z / positionScale.z + 0.5f));
	}

	public Maze.Point GetMazePoint (Vector3 aPos)
	{
		return GetMazePointFromLocal (aPos - parent.position);
	}

	public Maze.Point GetPlayerMazePoint ()
	{
		return GetMazePoint (AllLevels.Get ().player.transform.position);
	}

	public Vector3 GetPlayerMazePointV ()
	{
		Maze.Point lP = GetMazePoint (AllLevels.Get ().player.transform.position);
		Vector3 lV = AllLevels.Get ().player.transform.position - GetVectorFromMazePoint (lP);
		return new Vector3 (lP.x + lV.x / positionScale.x, lP.y + lV.y / positionScale.y, lP.z + lV.z / positionScale.z);
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
		MazeCellComponent lCell = aParent.GetComponent<MazeCellComponent> ();
		if (lCell != null && !lCell.CheckPrefabConditions (aPrefab, aForLater)) {
			return null;
		}
		PrefabConditions lCond = aPrefab.GetComponent<PrefabConditions> ();
		GameObject lObj = GameObject.Instantiate (aPrefab, aPos, aRotation) as GameObject;
		lObj.transform.SetParent (aParent, false);
		lObj.name = aName;
		if (lCell) {
			RunPrefabModifier (lObj);
			if (lCond && lCond.checkMeshBounds != MazeCellComponent.MeshCheckBoundsMode.NoCheck) {
				if (lCell.IntersectsWithBounds (lObj, lCond.checkMeshBounds)) {
					GameObject.DestroyImmediate (lObj);
					return null;
				}
			}
			if (lCond) {
				lCell.SetTags (lCond.ownTags);
			}
			lCell.PrefabInserted (lObj);
		}
		return lObj;
	}

	public void RunPrefabModifier (GameObject aObj)
	{
		PrefabVariation[] lVars = aObj.GetComponentsInChildren<PrefabVariation> ();
		foreach (PrefabVariation lVar in lVars) {
			if (!lVar.isModified) {
				lVar.ModifyIt ();
			}
		}
		PrefabModifier[] lMods = aObj.GetComponentsInChildren<PrefabModifier> ();
		foreach (PrefabModifier lMod in lMods) {
			if (!lMod.isModified) {
				lMod.ModifyIt ();
			}
		}
	}
}
