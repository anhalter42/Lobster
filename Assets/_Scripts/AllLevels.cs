﻿using UnityEngine;
using System.Collections;

public class AllLevels : MonoBehaviour
{
	public string Version = "0.3";
	public TextAsset levels;
	public TextAsset cellDescs;
	public CellDescription[] cellDescriptions;

	public LevelSettings currentLevelSettings;
	public LevelSettings[] levelSettings;

	protected static GameObject fMaster;

	public static GameObject GetMaster ()
	{
		if (!fMaster) {
			fMaster = GameObject.Find ("Master");
			if (!fMaster) {
				fMaster = new GameObject ("Master");
				fMaster.AddComponent<AllLevels> ().Start ();
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

	public bool hasLevels ()
	{
		return levelSettings != null && levelSettings.Length > 0;
	}

	// Use this for initialization
	void Start ()
	{
		UnityEngine.UI.Text lText = GameObject.Find ("Version").GetComponent<UnityEngine.UI.Text> ();
		if (lText) {
			lText.text = "Lobster " + Version;
		}
		if (!levels) {
			levels = Resources.Load<TextAsset> ("Levels");
		}
		if (levels) {
			ReadLevels (levels.text);
		}
		if (!cellDescs) {
			cellDescs = Resources.Load<TextAsset> ("CellDescs");
		}
		if (cellDescs) {
			ReadCellDescs (cellDescs.text);
		}
		if (hasLevels ()) {
			currentLevelSettings = levelSettings [0];
		} else {
			currentLevelSettings = new LevelSettings ();
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	public CellDescription GetCellDescription (string aName)
	{
		for (int i = 0; i < cellDescriptions.Length; i++) {
			if (cellDescriptions [i].name == aName) {
				return cellDescriptions [i];
			}
		}
		Debug.Log ("Cell Description '" + aName + "' not found!");
		return cellDescriptions [0];
	}

	void ReadInt (ref int aValue, string aLine, string aName)
	{
		if (aLine.StartsWith (aName)) {
			aValue = int.Parse (aLine.Split (new string[] { "\t" }, System.StringSplitOptions.RemoveEmptyEntries) [1]);
		}
	}

	void ReadCellDescs (string aText)
	{
		ArrayList lDescs = new ArrayList ();
		CellDescription lDesc = null;
		string[] lLines = aText.Split (new string[] { "\n" }, System.StringSplitOptions.RemoveEmptyEntries);
		foreach (string lLine in lLines) {
			if (lLine.StartsWith ("#")) {
				lDesc = new CellDescription ();
				lDesc.name = lLine.Substring (1);
				lDescs.Add (lDesc);
			} else if (lDesc != null) {
				lDesc.ReadLine (lLine);
			}
		}
		cellDescriptions = lDescs.ToArray (typeof(CellDescription)) as CellDescription[];
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
				lSetting.ReadLine (lLine);
			}
		}
		levelSettings = lSettings.ToArray (typeof(LevelSettings)) as LevelSettings[];
	}
}
