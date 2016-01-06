using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class AllLevels : MonoBehaviour
{
	public string Version = "0.3";
	public TextAsset levels;
	public TextAsset cellDescs;
	public CellDescription[] cellDescriptions;

	public LevelSettings currentLevelSettings;
	public LevelSettings[] levelSettings;

	private LevelController m_levelController;

	public LevelController levelController {
		get {
			if (m_levelController == null) {
				GameObject lObj = GameObject.Find ("LevelController") as GameObject;
				if (lObj) {
					m_levelController = lObj.GetComponent<LevelController> ();
				}
			}
			return m_levelController;
		}
	}

	public GameObject playerPrefab;

	public GameObject player { get { return levelController.player; } }

	protected static GameObject fMaster;

	public static GameObject GetMaster ()
	{
		if (!fMaster) {
			fMaster = GameObject.Find ("Master");
			if (!fMaster) {
				fMaster = new GameObject ("Master");
				DontDestroyOnLoad (fMaster);
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

	public bool hasLevels ()
	{
		return levelSettings != null && levelSettings.Length > 0;
	}

	// Use this for initialization
	void Awake ()
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
		if (!playerPrefab) {
			playerPrefab = Resources.Load ("Prefabs/Player") as GameObject;
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
		string lFolder = null;
		string[] lLines = aText.Split (new string[] { "\n" }, System.StringSplitOptions.RemoveEmptyEntries);
		foreach (string lLine in lLines) {
			string lNewLine = lLine.Replace ("\r", "");
			if (lNewLine.StartsWith ("#")) {
				lDesc = new CellDescription ();
				lDesc.name = lNewLine.Substring (1);
				lDescs.Add (lDesc);
				lFolder = null;
			} else if (lNewLine.StartsWith ("Package=")) {
				lFolder = lNewLine.Split (new string[] { "=" }, System.StringSplitOptions.RemoveEmptyEntries) [1];
			} else if (lDesc != null) {
				lDesc.ReadLine (lNewLine, lFolder);
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
			string lNewLine = lLine.Replace ("\r", "");
			if (lNewLine.StartsWith ("#")) {
				lSetting = new LevelSettings ();
				lSetting.levelName = lNewLine.Substring (1);
				lSettings.Add (lSetting);
				lSetting.level = lSettings.Count;
			} else if (lSetting != null) {
				lSetting.ReadLine (lNewLine);
			}
		}
		levelSettings = lSettings.ToArray (typeof(LevelSettings)) as LevelSettings[];
	}

	public void NextLevel ()
	{
		if (currentLevelSettings.level < levelSettings.Length - 1) {
			currentLevelSettings = levelSettings [currentLevelSettings.level/* + 1*/];
			SceneManager.LoadScene ("Main", LoadSceneMode.Single);
		}
	}

	public void SetLevel (int aLevel, bool aLoad = true)
	{
		currentLevelSettings = levelSettings [aLevel];
		if (aLoad) {
			SceneManager.LoadScene ("Main", LoadSceneMode.Single);
		}
	}
}
