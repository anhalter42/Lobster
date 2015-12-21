using UnityEngine;
using System.Collections;

public class LevelSettings {

	public int level = 1;
	public string levelName = "NO LEVELS";
	public string levelDescription = string.Empty;
	public int mazeWidth   = 5;
	public int mazeHeight  = 1;
	public int mazeDepth   = 5;
	public int breakWalls  = 0;
	public int maxTime     = 0; // in seconds, 0 means endless
	public int scoreForExit = 20; // 20 points to open the exit

}
