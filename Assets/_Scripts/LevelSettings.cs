using UnityEngine;
using System.Collections;

[System.Serializable]
public class LevelSettings
{
	public class CellDescOverride
	{
		public string name;
		public int maxCount = -1;
		public int minCount = -1;

		public bool ReadLine (string aLine, string aFolder)
		{
			string[] lVs = aLine.Split (new string[] { ";" }, System.StringSplitOptions.RemoveEmptyEntries);
			if (lVs.Length > 0) {
				name = lVs [0];
			}
			if (lVs.Length > 2) {
				maxCount = int.Parse (lVs [1]);
				minCount = int.Parse (lVs [2]);
			}
			return true;
		}
	}

	public class Cell
	{
		public Maze.Point pos = new Maze.Point (-1, -1, -1);
		public string[] tags = { };
		public bool visited = false;

		public bool ReadLine (string aLine, string aFolder)
		{
			string[] lVs = aLine.Split (new string[] { ";" }, System.StringSplitOptions.RemoveEmptyEntries);
			if (lVs.Length > 0) {
				ReadMazePoint (ref pos, lVs [0]);
			}
			if (lVs.Length > 1) {
				tags = lVs [1].Split (new string[] { "," }, System.StringSplitOptions.RemoveEmptyEntries);
			}
			if (lVs.Length > 2) {
				visited = bool.Parse (lVs [2]);
			}
			return true;
		}
	}

	public class PrefabSet
	{
		public string name;
		public Maze.Point pos = new Maze.Point (-1, -1, -1);
		public GameObject prefab;

		public bool ReadLine (string aLine, string aFolder)
		{
			string[] lVs = aLine.Split (new char[] { ';' });
			if (lVs.Length > 0) {
				name = lVs [0];
				prefab = AllLevels.LoadResource<GameObject> (name, "Prefabs", aFolder);
			}
			if (lVs.Length > 1) {
				LevelSettings.ReadMazePoint (ref pos, lVs [1]);
			}
			return prefab != null;
		}
	}

	public class CellScore
	{
		public int score = 1;
		public int probability = 100;

		public bool ReadLine (string aLine)
		{
			string[] lVs = aLine.Split (new char[] { ';' });
			if (lVs.Length > 0) {
				score = int.Parse (lVs [0]);
			}
			if (lVs.Length > 1) {
				probability = int.Parse (lVs [1]);
			}
			return true;
		}
	}

	//<levelName>|NEXT;[<point>];[<item1>[:<item1_count>],...];[<prefab>]
	public class Exit
	{
		public string levelName;
		public Maze.Point pos = new Maze.Point (-1, -1, -1);
		public MultiActivator.InventoryItem[] items = { };
		public GameObject prefab;

		public bool ReadLine (string aLine, string aFolder)
		{
			string[] lVs = aLine.Split (new char[] { ';' });
			if (lVs.Length > 0) {
				levelName = lVs [0];
			}
			if (lVs.Length > 1) {
				LevelSettings.ReadMazePoint (ref pos, lVs [1]);
			}
			if (lVs.Length > 2) {
				string[] lIs = lVs [2].Split (new char[] { ',' });
				System.Array.Resize<MultiActivator.InventoryItem> (ref items, lIs.Length);
				int lIndex = 0;
				foreach (string lI in lIs) {
					MultiActivator.InventoryItem lItem = new MultiActivator.InventoryItem ();
					lItem.ReadLine (lI);
					items [lIndex] = lItem;
					lIndex++;
				}
			}
			if (lVs.Length > 3 && !string.IsNullOrEmpty (lVs [3])) {
				prefab = AllLevels.LoadResource<GameObject> (lVs [3], "Prefabs", aFolder);
			}
			return true;
		}
	}

	public string prefabs = "Simple";
	public int level = 1;
	public string levelName = "NO LEVELS";
	public string name;

	string m_worldName = string.Empty;

	public string worldName {
		get {
			if (string.IsNullOrEmpty (m_worldName) && !string.IsNullOrEmpty (prefabs)) {
				m_worldName = AllLevels.Get ().GetCellDescription (prefabs).worldName;
			}
			return m_worldName;
		}
		set { m_worldName = value; }
	}

	public string levelDescription = string.Empty;
	public string startupText = null;
	public string deathLevel;
	public bool isVisible = true;
	public int mazeWidth = 5;
	public int mazeHeight = 1;
	public int mazeDepth = 5;
	public int breakWalls = 0;
	public int maxTime = 0;
	public int lives = 1;
	// only for level mode
	// in seconds, 0 means endless
	public int scoreForExit = 20;
	// 20 points to open the exit
	public float scoreBonusFactor = 2f;
	// Faktor für extra Score
	public float scoreTimeBonusFactor = 5f;
	// Faktor für extra Time Bonus Score
	public Maze.Point playerStart = null;
	public Maze.Point exitPos = null;
	public float playerProtectionTime = 10f;
	public float ambientLight = 0.75f;
	public Color ambientLightColor = new Color (77 / 255, 77 / 255, 77 / 255);
	public UnityEngine.Rendering.AmbientMode ambientMode = UnityEngine.Rendering.AmbientMode.Skybox;
	public float dayLight = 0.75f;
	public Color dayLightColor = Color.white;
	public Color groundColor = Color.white;
	public Texture groundTexture = null;
	public AudioClip audioBackgroundPause;
	public AudioClip audioBackgroundMusic;
	public AudioClip audioBackgroundLevelEnd;
	public AudioClip audioBackgroundLevelStart;
	public AudioClip audioBackgroundLevelExitOpen;

	public Cell[] directCells = { };
	public PrefabSet[] directPrefabs = { };
	public CellDescOverride[] cellDescOverrides = { };
	public string[] cellDescs = { };
	public Exit[] exits = { };
	public CellScore[] cellScores = { };

	public LevelSettings ()
	{
	}

	public LevelSettings (LevelSettings aSrc)
	{
		prefabs = aSrc.prefabs;
		level = aSrc.level;
		levelName = aSrc.levelName;
		name = aSrc.name;
		m_worldName = aSrc.m_worldName;
		levelDescription = aSrc.levelDescription;
		startupText = aSrc.startupText;
		deathLevel = aSrc.deathLevel;
		isVisible = aSrc.isVisible;
		mazeWidth = aSrc.mazeWidth;
		mazeHeight = aSrc.mazeHeight;
		mazeDepth = aSrc.mazeDepth;
		breakWalls = aSrc.breakWalls;
		maxTime = aSrc.maxTime;
		lives = aSrc.lives;
		scoreForExit = aSrc.scoreForExit;
		scoreBonusFactor = aSrc.scoreBonusFactor;
		scoreTimeBonusFactor = aSrc.scoreTimeBonusFactor;
		playerStart = aSrc.playerStart;
		exitPos = aSrc.exitPos;
		playerProtectionTime = aSrc.playerProtectionTime;
		ambientLight = aSrc.ambientLight;
		ambientLightColor = aSrc.ambientLightColor;
		ambientMode = aSrc.ambientMode;
		dayLight = aSrc.dayLight;
		dayLightColor = aSrc.dayLightColor;
		groundColor = aSrc.groundColor;
		groundTexture = aSrc.groundTexture;
		audioBackgroundPause = aSrc.audioBackgroundPause;
		audioBackgroundMusic = aSrc.audioBackgroundMusic;
		audioBackgroundLevelEnd = aSrc.audioBackgroundLevelEnd;
		audioBackgroundLevelStart = aSrc.audioBackgroundLevelStart;
		audioBackgroundLevelExitOpen = aSrc.audioBackgroundLevelExitOpen;

		System.Array.Copy (aSrc.directCells, directCells, aSrc.directCells.Length);
		System.Array.Copy (aSrc.directPrefabs, directPrefabs, aSrc.directPrefabs.Length);
		System.Array.Copy (aSrc.cellDescOverrides, cellDescOverrides, aSrc.cellDescOverrides.Length);
		System.Array.Copy (aSrc.cellDescs, cellDescs, aSrc.cellDescs.Length);
		System.Array.Copy (aSrc.exits, exits, aSrc.exits.Length);
		System.Array.Copy (aSrc.cellScores, cellScores, aSrc.cellScores.Length);
	}

	public static bool ReadBool (ref bool aValue, string aLine, string aName)
	{
		if (aLine.StartsWith (aName)) {
			aValue = bool.Parse (aLine.Split (new string[] { "\t" }, System.StringSplitOptions.RemoveEmptyEntries) [1]);
			return true;
		} else {
			return false;
		}
	}

	public static bool ReadInt (ref int aValue, string aLine, string aName)
	{
		if (aLine.StartsWith (aName)) {
			aValue = int.Parse (aLine.Split (new string[] { "\t" }, System.StringSplitOptions.RemoveEmptyEntries) [1]);
			return true;
		} else {
			return false;
		}
	}

	public static bool ReadFloat (ref float aValue, string aLine, string aName)
	{
		if (aLine.StartsWith (aName)) {
			aValue = float.Parse (aLine.Split (new string[] { "\t" }, System.StringSplitOptions.RemoveEmptyEntries) [1]);
			return true;
		} else {
			return false;
		}
	}

	public static bool ReadString (ref string aValue, string aLine, string aName)
	{
		if (aLine.StartsWith (aName)) {
			aValue = aLine.Split (new string[] { "\t" }, System.StringSplitOptions.RemoveEmptyEntries) [1];
			return true;
		} else {
			return false;
		}
	}

	public static bool ReadStringArray (ref string[] aValue, string aLine, string aName)
	{
		if (aLine.StartsWith (aName)) {
			int lI = aLine.IndexOf ("\t");
			string lValue = aLine.Substring (lI + 1);
			if (!string.IsNullOrEmpty (lValue)) {
				System.Array.Resize<string> (ref aValue, aValue.Length + 1);
				aValue [aValue.Length - 1] = lValue;
			}
			return true;
		} else {
			return false;
		}
	}

	public static bool ReadAudioClip (ref AudioClip aValue, string aLine, string aName, string aFolder)
	{
		if (aLine.StartsWith (aName + "\t")) {
			string[] lArgs = aLine.Split (new string[] { "\t" }, System.StringSplitOptions.RemoveEmptyEntries);
			if (lArgs.Length > 1) {
				aValue = AllLevels.LoadResource<AudioClip> (lArgs [1], "Audio", aFolder);
			}
			return true;
		} else {
			return false;
		}
	}

	public static bool ReadTexture (ref Texture aValue, string aLine, string aName, string aFolder)
	{
		if (aLine.StartsWith (aName + "\t")) {
			string[] lArgs = aLine.Split (new string[] { "\t" }, System.StringSplitOptions.RemoveEmptyEntries);
			if (lArgs.Length > 1) {
				aValue = AllLevels.LoadResource<Texture> (lArgs [1], "Textures", aFolder);
			}
			return true;
		} else {
			return false;
		}
	}

	public static bool ReadColor (ref Color aValue, string aLine, string aName)
	{
		if (aLine.StartsWith (aName)) {
			string lV = aLine.Split (new string[] { "\t" }, System.StringSplitOptions.RemoveEmptyEntries) [1];
			string[] lVs = lV.Split (new string[] { "," }, System.StringSplitOptions.RemoveEmptyEntries);
			if (lVs.Length == 3) {
				aValue = new Color (int.Parse (lVs [0]) / 255f, int.Parse (lVs [1]) / 255f, int.Parse (lVs [2]) / 255f);
			} else if (lVs.Length == 4) {
				aValue = new Color (int.Parse (lVs [0]) / 255f, int.Parse (lVs [1]) / 255f, int.Parse (lVs [2]) / 255f, int.Parse (lVs [3]) / 255f);
			}
			return true;
		} else {
			return false;
		}
	}

	public static bool ReadMazePoint (ref Maze.Point aValue, string aLine, string aName = null)
	{
		if (aName == null || aLine.StartsWith (aName)) {
			string lV = aName == null ? aLine : aLine.Split (new string[] { "\t" }, System.StringSplitOptions.RemoveEmptyEntries) [1];
			string[] lVs = lV.Split (new string[] { "," }, System.StringSplitOptions.RemoveEmptyEntries);
			if (lVs.Length == 2) {
				aValue = new Maze.Point (int.Parse (lVs [0]), 0, int.Parse (lVs [1]));
			} else if (lVs.Length == 3) {
				aValue = new Maze.Point (int.Parse (lVs [0]), int.Parse (lVs [1]), int.Parse (lVs [2]));
			}
			return true;
		} else {
			return false;
		}
	}

	public static bool ReadVector3 (ref Vector3 aValue, string aLine, string aName = null)
	{
		if (aName == null || aLine.StartsWith (aName)) {
			string lV = aName == null ? aLine : aLine.Split (new string[] { "\t" }, System.StringSplitOptions.RemoveEmptyEntries) [1];
			string[] lVs = lV.Split (new string[] { "," }, System.StringSplitOptions.RemoveEmptyEntries);
			if (lVs.Length == 2) {
				aValue = new Vector3 (float.Parse (lVs [0]), 0, float.Parse (lVs [1]));
			} else if (lVs.Length == 3) {
				aValue = new Vector3 (float.Parse (lVs [0]), float.Parse (lVs [1]), float.Parse (lVs [2]));
			}
			return true;
		} else {
			return false;
		}
	}

	public static bool ReadAmbientMode (ref UnityEngine.Rendering.AmbientMode aValue, string aLine, string aName)
	{
		if (aLine.StartsWith (aName)) {
			string lV = aLine.Split (new string[] { "\t" }, System.StringSplitOptions.RemoveEmptyEntries) [1];
			if (!string.IsNullOrEmpty (lV)) {
				aValue = (UnityEngine.Rendering.AmbientMode)System.Enum.Parse (typeof(UnityEngine.Rendering.AmbientMode), lV, true);
				return true;
			} else {
				Debug.Log ("no ambient mode given!");
				return true;
			}
		} else {
			return false;
		}
	}

	public static bool ReadCellScores (ref CellScore[] aValue, string aLine, string aName)
	{
		if (aLine.StartsWith (aName)) {
			string lV = aLine.Split (new string[] { "\t" }, System.StringSplitOptions.RemoveEmptyEntries) [1];
			if (!string.IsNullOrEmpty (lV)) {
				CellScore lC = new CellScore ();
				if (lC.ReadLine (lV)) {
					System.Array.Resize<CellScore> (ref aValue, aValue.Length + 1);
					aValue [aValue.Length - 1] = lC;
				}
				return true;
			} else {
				Debug.Log ("No cellscore given!");
				return true;
			}
		} else {
			return false;
		}
	}

	public bool ReadPrefabSet (ref PrefabSet[] aValue, string aLine, string aName)
	{
		if (aLine.StartsWith (aName)) {
			string lV = aLine.Split (new string[] { "\t" }, System.StringSplitOptions.RemoveEmptyEntries) [1];
			if (!string.IsNullOrEmpty (lV)) {
				PrefabSet lP = new PrefabSet ();
				if (lP.ReadLine (lV, prefabs)) {
					System.Array.Resize<PrefabSet> (ref aValue, aValue.Length + 1);
					aValue [aValue.Length - 1] = lP;
				}
				return true;
			} else {
				Debug.Log ("No prefabset given!");
				return true;
			}
		} else {
			return false;
		}
	}

	public bool ReadCellDescOverride (ref CellDescOverride[] aValue, string aLine, string aName)
	{
		if (aLine.StartsWith (aName)) {
			string lV = aLine.Split (new string[] { "\t" }, System.StringSplitOptions.RemoveEmptyEntries) [1];
			if (!string.IsNullOrEmpty (lV)) {
				CellDescOverride lC = new CellDescOverride ();
				if (lC.ReadLine (lV, prefabs)) {
					System.Array.Resize<CellDescOverride> (ref aValue, aValue.Length + 1);
					aValue [aValue.Length - 1] = lC;
				}
				return true;
			} else {
				Debug.Log ("No celldescoverride given!");
				return true;
			}
		} else {
			return false;
		}
	}

	public bool ReadCells (ref Cell[] aValue, string aLine, string aName)
	{
		if (aLine.StartsWith (aName)) {
			string lV = aLine.Split (new string[] { "\t" }, System.StringSplitOptions.RemoveEmptyEntries) [1];
			if (!string.IsNullOrEmpty (lV)) {
				Cell lC = new Cell ();
				if (lC.ReadLine (lV, prefabs)) {
					System.Array.Resize<Cell> (ref aValue, aValue.Length + 1);
					aValue [aValue.Length - 1] = lC;
				}
				return true;
			} else {
				Debug.Log ("No cell given!");
				return true;
			}
		} else {
			return false;
		}
	}

	public bool ReadLevelName (ref string aValue, string aLine, string aName)
	{
		if (ReadString (ref aValue, aLine, aName)) {
			if (!aValue.Contains (".")) {
				aValue = string.Format ("{0}.{1}", worldName, aValue);
			}
			return true;
		} else {
			return false;
		}
	}

	public bool ReadExit (ref Exit[] aValue, string aLine, string aName)
	{
		if (aLine.StartsWith (aName)) {
			string lV = aLine.Split (new string[] { "\t" }, System.StringSplitOptions.RemoveEmptyEntries) [1];
			if (!string.IsNullOrEmpty (lV)) {
				Exit lE = new Exit ();
				if (lE.ReadLine (lV, prefabs)) {
					System.Array.Resize<Exit> (ref aValue, aValue.Length + 1);
					aValue [aValue.Length - 1] = lE;
				}
				return true;
			} else {
				Debug.Log ("No exit given!");
				return true;
			}
		} else {
			return false;
		}
	}

	public void ReadLine (string aLine)
	{
		if (!ReadString (ref prefabs, aLine, "prefabs"))
		if (!ReadString (ref name, aLine, "name"))
		if (!ReadLevelName (ref deathLevel, aLine, "deathLevel"))
		if (!ReadString (ref m_worldName, aLine, "world"))
		if (!ReadString (ref startupText, aLine, "startupText"))
		if (!ReadBool (ref isVisible, aLine, "isVisible"))
		if (!ReadFloat (ref playerProtectionTime, aLine, "playerProtectionTime"))
		if (!ReadAudioClip (ref audioBackgroundMusic, aLine, "audioBackgroundMusic", null))
		if (!ReadAudioClip (ref audioBackgroundPause, aLine, "audioBackgroundPause", null))
		if (!ReadAudioClip (ref audioBackgroundLevelEnd, aLine, "audioBackgroundLevelEnd", null))
		if (!ReadAudioClip (ref audioBackgroundLevelStart, aLine, "audioBackgroundLevelStart", null))
		if (!ReadAudioClip (ref audioBackgroundLevelExitOpen, aLine, "audioBackgroundLevelExitOpen", null))
		if (!ReadCells (ref directCells, aLine, "directCell"))
		if (!ReadPrefabSet (ref directPrefabs, aLine, "directPrefab"))
		if (!ReadCellDescOverride (ref cellDescOverrides, aLine, "cellDescOverride"))
		if (!ReadStringArray (ref cellDescs, aLine, "cellDescs"))
		if (!ReadTexture (ref groundTexture, aLine, "groundTexture", null))
		if (!ReadInt (ref mazeWidth, aLine, "mazeWidth"))
		if (!ReadInt (ref mazeHeight, aLine, "mazeHeight"))
		if (!ReadInt (ref mazeDepth, aLine, "mazeDepth"))
		if (!ReadInt (ref breakWalls, aLine, "breakWalls"))
		if (!ReadInt (ref maxTime, aLine, "maxTime"))
		if (!ReadInt (ref lives, aLine, "lives"))
		if (!ReadMazePoint (ref playerStart, aLine, "playerStart"))
		if (!ReadMazePoint (ref exitPos, aLine, "exitPos"))
		if (!ReadExit (ref exits, aLine, "exit"))
		if (!ReadColor (ref ambientLightColor, aLine, "ambientLightColor"))
		if (!ReadFloat (ref ambientLight, aLine, "ambientLight"))
		if (!ReadAmbientMode (ref ambientMode, aLine, "ambientMode"))
		if (!ReadColor (ref groundColor, aLine, "groundColor"))
		if (!ReadColor (ref dayLightColor, aLine, "dayLightColor"))
		if (!ReadFloat (ref dayLight, aLine, "dayLight"))
		if (!ReadFloat (ref scoreBonusFactor, aLine, "scoreBonusFactor"))
		if (!ReadFloat (ref scoreTimeBonusFactor, aLine, "scoreTimeBonusFactor"))
		if (!ReadCellScores (ref cellScores, aLine, "cellScore"))
		if (!ReadInt (ref scoreForExit, aLine, "scoreForExit")) {
			if (aLine.StartsWith ("//")) {
				if (string.IsNullOrEmpty (levelDescription)) {
					levelDescription = aLine.Substring (2);
				} else {
					levelDescription += "\n" + aLine.Substring (2);
				}
			}
		}
	}

	public void PrepareMaze (Maze aMaze)
	{
		foreach (Cell lC in directCells) {
			Maze.Cell lCell = aMaze.get (lC.pos);
			foreach (string lTag in lC.tags) {
				if (lTag.StartsWith ("No")) {
					int lDir = MazeBuilder.GetWallDir (lTag.Substring (2));
					if (lDir >= 0) {
						lCell.links [lDir].broken = true;
					}
				} else {
					int lDir = MazeBuilder.GetWallDir (lTag);
					if (lDir >= 0) {
						lCell.links [lDir].breakable = false;
					}
				}
			}
			lCell.visited = lC.visited;
		}
	}

	public CellDirectionObjects PreparePrefabs (CellDirectionObjects aPrefabs)
	{
		CellDirectionObjects lClone = aPrefabs.Clone ();
		foreach (string lLine in cellDescs) {
			lClone.ReadLine (lLine);
		}
		return lClone;
	}
}
