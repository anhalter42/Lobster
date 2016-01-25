using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class AllLevels : MonoBehaviour
{
	public string Version = "0.3";
	public TextAsset levels;
	public TextAsset cellDescs;
	public CellDescription[] cellDescriptions;

	public PlayersData data = new PlayersData ();

	public string playerName = string.Empty;
	public Player currentPlayer;

	public LevelSettings currentLevelSettings;
	public LevelSettings[] levelSettings;

	private LevelController m_levelController;

	public LevelController levelController {
		get {
			if (m_levelController == null) {
				GameObject lObj = GameObject.Find ("LevelController") as GameObject;
				if (lObj) {
					m_levelController = lObj.GetComponent<LevelController> ();
					if (!m_levelController) {
						m_levelController = lObj.AddComponent<LevelController> ();
					}
				} else {
					m_levelController = new GameObject ().AddComponent<LevelController> ();
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
		GameObject lObj = GameObject.Find ("Version") as GameObject;
		if (lObj) {
			UnityEngine.UI.Text lText = lObj.GetComponent<UnityEngine.UI.Text> ();
			if (lText) {
				lText.text = "Lobster " + Version;
			}
		}
		if (!cellDescs) {
			cellDescs = Resources.Load<TextAsset> ("CellDescs");
		}
		if (System.IO.File.Exists ("CellDescs.txt")) {
			string lCellDescs = System.IO.File.ReadAllText ("CellDescs.txt");
			ReadCellDescs (lCellDescs);
		} else {
			if (cellDescs) {
				ReadCellDescs (cellDescs.text);
			}
		}
		if (!levels) {
			levels = Resources.Load<TextAsset> ("Levels");
		}
		if (System.IO.File.Exists ("Levels.txt")) {
			string lLevels = System.IO.File.ReadAllText ("Levels.txt");
			ReadLevels (lLevels);
		} else {
			if (levels) {
				ReadLevels (levels.text);
			}
		}
		LoadData ();
		if (string.IsNullOrEmpty (playerName)) {
			playerName = PlayerPrefs.GetString ("PlayerName", "Winston");
		}
		if (!string.IsNullOrEmpty (playerName)) {
			SetPlayerName (playerName);
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

	void ReadCellDescs (string aText)
	{
		ArrayList lDescs = new ArrayList ();
		CellDescription lDesc = null;
		string lFolder = null;
		string[] lLines = aText.Split (new string[] { "\n" }, System.StringSplitOptions.RemoveEmptyEntries);
		foreach (string lLine in lLines) {
			string lNewLine = lLine.Replace ("\r", "");
			if (lNewLine.StartsWith ("#")) {
				if (lDesc != null) {
					lDesc.FinishedReading ();
				}
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
		if (lDesc != null) {
			lDesc.FinishedReading ();
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
		currentLevelSettings = levelSettings [aLevel - 1]; // level numbers are from 1...
		if (aLoad) {
			SceneManager.LoadScene ("Main", LoadSceneMode.Single);
		}
	}

	public static T LoadResource<T> (string aName, string aMainFolder, string aSubFolder) where T : UnityEngine.Object
	{
		T lObj = null;
		if (!string.IsNullOrEmpty (aSubFolder)) {
			lObj = Resources.Load<T> (aMainFolder + "/" + aSubFolder + "/" + aName);
		}
		if (lObj == null) {
			lObj = Resources.Load<T> (aMainFolder + "/" + aName);
		}
		if (lObj == null) {
			lObj = Resources.Load<T> (aName);
		}
		if (lObj == null) {
			Debug.Log (string.Format ("Could not find {0} '{1}' (Folder '{2}')!", aMainFolder, aName, aSubFolder));
		}
		return lObj;
	}

	public Player GetPlayer (string aName, bool aCreate = false)
	{
		return data.GetPlayer (aName, aCreate);
	}

	public void SetPlayerName (string aName)
	{
		currentPlayer = GetPlayer (aName, true);
	}

	public void LoadData ()
	{
		string lPath = Path.Combine (Application.persistentDataPath, "data.dat");
		if (File.Exists (lPath)) {
			BinaryFormatter lBf = new BinaryFormatter ();
			FileStream lFile = File.OpenRead (lPath);
			data = lBf.Deserialize (lFile) as PlayersData;
			lFile.Close ();
		}
	}

	public void SaveData ()
	{
		BinaryFormatter lBf = new BinaryFormatter ();
		FileStream lFile = File.Create (Path.Combine (Application.persistentDataPath, "data.dat"));
		lBf.Serialize (lFile, data);
		lFile.Close ();
	}
}
