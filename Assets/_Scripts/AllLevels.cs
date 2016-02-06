﻿using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class AllLevels : MonoBehaviour
{
	public string Version = "0.3";

	public CellDescription[] cellDescriptions = { };

	public PlayersData data = new PlayersData ();

	public string playerName = string.Empty;
	public Player currentPlayer;

	public LevelSettings currentLevelSettings;
	public LevelSettings[] levelSettings;

	public Inventory inventory = new Inventory ();

	public Localization local = new Localization ();

	string[] m_worlds = null;

	public string[] worlds {
		get {  
			if (m_worlds == null || m_worlds.Length == 0) {
				ArrayList lList = new ArrayList ();
				foreach (LevelSettings lS in levelSettings) {
					if (!lList.Contains (lS.worldName)) {
						lList.Add (lS.worldName);
					}
				}
				m_worlds = lList.ToArray (typeof(string)) as string[];
			}
			return m_worlds;
		}
	}

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

	public AudioSource audioGlobal;

	protected static GameObject fMaster;

	public static GameObject GetMaster ()
	{
		if (!fMaster) {
			fMaster = GameObject.Find ("Master");
			if (!fMaster) {
				fMaster = new GameObject ("Master");
				DontDestroyOnLoad (fMaster);
			}
			if (fMaster.GetComponent<AllLevels> () == null) {
				fAllLevels = fMaster.AddComponent<AllLevels> ();
			}
			if (fMaster.GetComponent<AudioSource> () == null) {
				Get ().audioGlobal = fMaster.AddComponent<AudioSource> ();
				Get ().audioGlobal.volume = 0.25f;
				Get ().audioGlobal.clip = LoadResource<AudioClip> ("Background_Journey_to_Golden_ark", "Audio");
				Get ().audioGlobal.Play ();
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
		local.Read ();
		if (System.IO.File.Exists ("Inventory.txt")) {
			string lText = System.IO.File.ReadAllText ("Inventory.txt");
			inventory.ReadInventory (lText);
		} else {
			TextAsset lText = Resources.Load<TextAsset> ("Inventory");
			if (lText) {
				inventory.ReadInventory (lText.text);
			}
		}
		if (System.IO.File.Exists ("CellDescs.txt")) {
			string lCellDescs = System.IO.File.ReadAllText ("CellDescs.txt");
			ReadCellDescs (lCellDescs);
		} else {
			TextAsset lText = Resources.Load<TextAsset> ("CellDescs");
			if (lText) {
				ReadCellDescs (lText.text);
			}
		}
		if (System.IO.File.Exists ("Levels.txt")) {
			string lLevels = System.IO.File.ReadAllText ("Levels.txt");
			ReadLevels (lLevels);
		} else {
			TextAsset lText = Resources.Load<TextAsset> ("Levels");
			if (lText) {
				ReadLevels (lText.text);
			}
		}
		string lFName = string.Format ("Levels.{0}.txt", Application.systemLanguage);
		if (System.IO.File.Exists (lFName)) {
			string lLevels = System.IO.File.ReadAllText (lFName);
			MergeLevels (lLevels);
		} else {
			TextAsset lText = Resources.Load<TextAsset> ("Levels." + Application.systemLanguage);
			if (lText) {
				MergeLevels (lText.text);
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
				cellDescriptions = lDescs.ToArray (typeof(CellDescription)) as CellDescription[];
				lFolder = null;
			} else if (lNewLine.StartsWith ("Package=")) {
				lFolder = lNewLine.Split (new string[] { "=" }, System.StringSplitOptions.RemoveEmptyEntries) [1];
			} else if (lNewLine.StartsWith ("BasedOn=")) {
				string lBaseDescName = lNewLine.Split (new string[] { "=" }, System.StringSplitOptions.RemoveEmptyEntries) [1];
				CellDescription lBaseDesc = GetCellDescription (lBaseDescName);
				if (lBaseDesc == null) {
					Debug.Log (string.Format ("BasedOn '{0}' {1} not found!", lBaseDescName, lDesc.name));
				} else {
					string lName = lDesc.name;
					lBaseDesc.Clone (lDesc);
					lDesc.name = lName;
				}
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
				lSetting.name = lSetting.levelName;
				lSettings.Add (lSetting);
				lSetting.level = lSettings.Count;
			} else if (lSetting != null) {
				lSetting.ReadLine (lNewLine);
			}
		}
		levelSettings = lSettings.ToArray (typeof(LevelSettings)) as LevelSettings[];
	}

	void MergeLevels (string aText)
	{
		LevelSettings lSetting = null;
		string[] lLines = aText.Split (new string[] { "\n" }, System.StringSplitOptions.RemoveEmptyEntries);
		foreach (string lLine in lLines) {
			string lNewLine = lLine.Replace ("\r", "");
			if (lNewLine.StartsWith ("#")) {
				int lI = lNewLine.IndexOf ('.');
				if (lI > 1) {
					string lWorld = lNewLine.Substring (1, lI - 1);
					string lName = lNewLine.Substring (lI + 1);
					lSetting = GetLevel (lWorld, lName);
				}
				if (lSetting == null) {
					Debug.Log (string.Format ("Can't find level '{0}'!", lNewLine));
				}
			} else if (lSetting != null) {
				lSetting.ReadLine (lNewLine);
			}
		}
	}

	public LevelSettings GetLevel (string aWorld, string aLevelName)
	{
		foreach (LevelSettings lS in levelSettings) {
			if (lS.levelName == aLevelName && lS.worldName == aWorld) {
				return lS;
			}
		}
		return null;
	}

	public void NextLevel ()
	{
		if (currentLevelSettings.level < levelSettings.Length - 1) {
			currentLevelSettings = levelSettings [currentLevelSettings.level/* + 1*/];
			StartLevel ();
		}
	}

	public void SetLevel (int aLevel, bool aLoad = true)
	{
		currentLevelSettings = levelSettings [aLevel - 1]; // level numbers are from 1...
		if (aLoad) {
			StartLevel ();
		}
	}

	public void StartLevel ()
	{
		audioGlobal.Stop ();
		SceneManager.LoadScene ("Main", LoadSceneMode.Single);
	}

	public void StartChooseLevel ()
	{
		if (!audioGlobal.isPlaying)
			audioGlobal.Play ();
		SceneManager.LoadScene ("ChooseLevel", LoadSceneMode.Single);
	}

	public void StartNewGame ()
	{
		if (!audioGlobal.isPlaying)
			audioGlobal.Play ();
		SceneManager.LoadScene ("Start", LoadSceneMode.Single);
	}

	public void StartProfile ()
	{
		if (!audioGlobal.isPlaying)
			audioGlobal.Play ();
		SceneManager.LoadScene ("PlayerProfile", LoadSceneMode.Single);
	}

	public void StartHighscore ()
	{
		if (!audioGlobal.isPlaying)
			audioGlobal.Play ();
		SceneManager.LoadScene ("Highscore", LoadSceneMode.Single);
	}

	public static T LoadResource<T> (string aName, string aMainFolder, string aSubFolder = null) where T : UnityEngine.Object
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
		PlayerPrefs.SetString ("PlayerName", currentPlayer.name);
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
