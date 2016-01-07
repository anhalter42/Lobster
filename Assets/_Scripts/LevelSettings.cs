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
	public int maxTime = 0;// in seconds, 0 means endless
	public int scoreForExit = 20;// 20 points to open the exit
	public float dayLight = 0.75f;
	public Color dayLightColor = Color.white;


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
			aValue = float.Parse(aLine.Split (new string[] { "\t" }, System.StringSplitOptions.RemoveEmptyEntries) [1]);
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

	public static bool ReadColor (ref Color aValue, string aLine, string aName)
	{
		if (aLine.StartsWith (aName)) {
			string lV = aLine.Split (new string[] { "\t" }, System.StringSplitOptions.RemoveEmptyEntries) [1];
			string[] lVs = lV.Split(new string[] { "," }, System.StringSplitOptions.RemoveEmptyEntries);
			if (lVs.Length == 3) {
				aValue = new Color(int.Parse(lVs[0])/255f,int.Parse(lVs[1])/255f,int.Parse(lVs[2])/255f);
			} else if (lVs.Length == 4) {
				aValue = new Color(int.Parse(lVs[0])/255f,int.Parse(lVs[1])/255f,int.Parse(lVs[2])/255f,int.Parse(lVs[3])/255f);
			}
			return true;
		} else {
			return false;
		}
	}

	public void ReadLine (string aLine)
	{
		if (!ReadString (ref prefabs, aLine, "prefabs"))
		if (!ReadInt (ref mazeWidth, aLine, "mazeWidth"))
		if (!ReadInt (ref mazeHeight, aLine, "mazeHeight"))
		if (!ReadInt (ref mazeDepth, aLine, "mazeDepth"))
		if (!ReadInt (ref breakWalls, aLine, "breakWalls"))
		if (!ReadInt (ref maxTime, aLine, "maxTime"))
		if (!ReadColor (ref dayLightColor, aLine, "dayLightColor"))
		if (!ReadFloat (ref dayLight, aLine, "dayLight"))
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
