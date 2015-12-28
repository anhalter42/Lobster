using UnityEngine;
using System.Collections;

[System.Serializable]
public class LevelSettings
{

	public string prefabs = "Simple";
	public int level = 1;
	public string levelName = "NO LEVELS";
	public string levelDescription = string.Empty;
	public int mazeWidth = 5;
	public int mazeHeight = 1;
	public int mazeDepth = 5;
	public int breakWalls = 0;
	public int maxTime = 0;
	// in seconds, 0 means endless
	public int scoreForExit = 20;
	// 20 points to open the exit

	public static bool ReadInt (ref int aValue, string aLine, string aName)
	{
		if (aLine.StartsWith (aName)) {
			aValue = int.Parse (aLine.Split (new string[] { "\t" }, System.StringSplitOptions.RemoveEmptyEntries) [1]);
			return true;
		} else {
			return false;
		}
	}

	public void ReadLine (string aLine)
	{
		if (!ReadInt (ref mazeWidth, aLine, "mazeWidth"))
		if (!ReadInt (ref mazeHeight, aLine, "mazeHeight"))
		if (!ReadInt (ref mazeDepth, aLine, "mazeDepth"))
		if (!ReadInt (ref breakWalls, aLine, "breakWalls"))
		if (!ReadInt (ref maxTime, aLine, "maxTime"))
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
}
