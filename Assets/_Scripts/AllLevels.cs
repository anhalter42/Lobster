using UnityEngine;
using System.Collections;

public class AllLevels : MonoBehaviour
{

	public TextAsset levels;

	public LevelSettings currentLevelSettings;
	public LevelSettings[] levelSettings;

	protected static GameObject fMaster;

	public static GameObject GetMaster ()
	{
		if (!fMaster) {
			fMaster = GameObject.Find ("Master");
			if (!fMaster) {
				fMaster = new GameObject ("Master");
				fMaster.AddComponent<AllLevels> ();
			}
		}
		return fMaster;
	}

	protected static AllLevels fAllLevels;

	public static AllLevels Get ()
	{
		if (!fAllLevels) {
			fAllLevels = GetMaster ().GetComponent<AllLevels> ();
		}
		return fAllLevels;
	}

	// Use this for initialization
	void Start ()
	{
		if (!levels) {
			levels = Resources.Load<TextAsset> ("Levels.txt");
		}
		if (levels) {
			ReadLevels (levels.text);
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	void ReadLevels (string aText)
	{
		ArrayList lSettings = new ArrayList ();
		LevelSettings lSetting = null;
		string[] lLines = aText.Split (new string[] { "\n" }, System.StringSplitOptions.RemoveEmptyEntries);
		foreach (string lLine in lLines) {
			if (lLine.StartsWith ("#")) {
				lSetting = new LevelSettings ();
				lSetting.levelName = lLine.Substring (1);
				lSettings.Add (lSetting);
				lSetting.level = lSettings.Count;
			} else if (lSetting != null) {
				if (lLine.StartsWith ("mazeWidth")) {
					lSetting.mazeWidth = int.Parse (lLine.Split (new string[] { "\t" }, System.StringSplitOptions.RemoveEmptyEntries) [1]);
				} else if (lLine.StartsWith ("mazeHeight")) {
					lSetting.mazeHeight = int.Parse (lLine.Split (new string[] { "\t" }, System.StringSplitOptions.RemoveEmptyEntries) [1]);
				} else if (lLine.StartsWith ("mazeDepth")) {
					lSetting.mazeDepth = int.Parse (lLine.Split (new string[] { "\t" }, System.StringSplitOptions.RemoveEmptyEntries) [1]);
				} else if (lLine.StartsWith ("//")) {
					if (string.IsNullOrEmpty (lSetting.levelDescription)) {
						lSetting.levelDescription = lLine.Substring (2);
					} else {
						lSetting.levelDescription += "\n" + lLine.Substring (2);
					}
				}
			}
		}
		levelSettings = lSettings.ToArray (typeof(LevelSettings)) as LevelSettings[];
	}
}
