using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class LevelController : MonoBehaviour
{
	[System.Serializable]
	public class PlayerSettings
	{
		public int lives = 1;
		public int score = 0;
	}

	[System.Serializable]
	public class PlayerLevelSettings
	{
		public int lives = 1;
		public int score = 0;
		public int scoreBonus = 0;
		public int scoreTimeBonus = 0;
		public float health = 100.0f;
		public float resumeTime = 0f;
		public float startTime = 0f;
		public float time = 0f;
		public float timeBonus = 0f;
		public bool scoreReached = false;
		public bool isProtected = false;
		public float levelRuntime = 0f;
		public float protectionTime = 0f;
		public string nextLevelName = "NEXT";

		public PlayerLevelSettings ()
		{
		}

		public PlayerLevelSettings (PlayerLevelSettings aSrc)
		{
			lives = aSrc.lives;
			score = aSrc.score;
			scoreBonus = aSrc.scoreBonus;
			scoreTimeBonus = aSrc.scoreTimeBonus;
			health = aSrc.health;
			resumeTime = aSrc.resumeTime;
			startTime = aSrc.startTime;
			time = aSrc.time;
			timeBonus = aSrc.timeBonus;
			scoreReached = aSrc.scoreReached;
			isProtected = aSrc.isProtected;
			levelRuntime = aSrc.levelRuntime;
			protectionTime = aSrc.protectionTime;
			nextLevelName = aSrc.nextLevelName;
		}
	}

	public class LevelStackItem
	{
		public LevelSettings settings;
		public CellDescription prefabs;
		public MazeBuilder builder;
		public PlayerLevelSettings playerLevelSettings;
		public GameObject mazeParent;
		public Vector3 playerPos;

		public LevelStackItem ()
		{
		}

		public LevelStackItem (LevelController aController)
		{
			settings = aController.settings;
			prefabs = aController.prefabs;
			builder = aController.builder;
			playerLevelSettings = new PlayerLevelSettings (aController.playerLevelSettings);
			mazeParent = aController.m_MainMazeParent;
			playerPos = aController.player.transform.position;
		}

		public void Restore (LevelController aController)
		{
			aController.settings = settings;
			aController.prefabs = prefabs;
			aController.builder = builder;
			aController.playerLevelSettings = playerLevelSettings;
			aController.m_MainMazeParent = mazeParent;
			aController.player.transform.position = playerPos;
		}
	}

	public Stack<LevelStackItem> levelStack = new Stack<LevelStackItem> ();

	public LevelSettings settings;
	[SerializeField]
	public CellDescription prefabs;
	public MazeBuilder builder;

	public PlayerLevelSettings playerLevelSettings;

	public PlayerSettings playerSettings = new PlayerSettings ();

	public Transform mazeParent;

	GameObject m_player;

	public GameObject player {
		get {
			if (!m_player) {
				m_player = GameObject.FindWithTag ("Player");
			}
			return m_player;
		}
		set { m_player = value; }
	}

	PlayerInventory m_playerInventory;

	public PlayerInventory playerInventory {
		get {
			if (!m_playerInventory) {
				if (player) {
					m_playerInventory = player.GetComponent<PlayerInventory> ();
				} else {
					return null;
				}
			}
			return m_playerInventory;
		}
	}

	public GameObject playerPrefab { get { return AllLevels.Get ().playerPrefab; } }

	public GameObject m_planeGround;
	public Text m_textScore;
	public Text m_textTime;
	public Text m_textLives;
	public Text m_textHealth;
	public Text m_textInventory;
	public Light m_mainLight;
	public RectTransform m_panelToast;
	public RectTransform m_panelPause;
	public RectTransform m_panelLevelFinished;
	public RectTransform m_panelStart;
	public RectTransform m_panelMiniMap;
	public RawImage m_imageMiniMap;
	public Text m_textDescription;
	public Text m_textName;
	public Text m_textLevel;

	public Text m_textLFName;
	public Text m_textLFLevel;
	public Text m_textLFScore;
	public Text m_textLFScoreBonus;
	public Text m_textLFTime;
	public Text m_textLFTimeBonus;

	public Text m_textSDescription;
	public Text m_textSName;
	public Text m_textSLevel;
	public Text m_textSAchievements;

	public Text m_textToastTitle;
	public Text m_textToastText;

	public Text m_textFPS;

	public AudioSource m_audioSourceBackground;
	public AudioSource m_audioSourceEffects;

	public AudioClip audioBackgroundPause { get { return settings.audioBackgroundPause == null ? prefabs.audioBackgroundPause : settings.audioBackgroundPause; } }

	public AudioClip audioBackgroundMusic { get { return settings.audioBackgroundMusic == null ? prefabs.audioBackgroundMusic : settings.audioBackgroundMusic; } }

	public AudioClip audioBackgroundLevelEnd { get { return settings.audioBackgroundLevelEnd == null ? prefabs.audioBackgroundLevelEnd : settings.audioBackgroundLevelEnd; } }

	public AudioClip audioBackgroundLevelStart { get { return settings.audioBackgroundLevelStart == null ? prefabs.audioBackgroundLevelStart : settings.audioBackgroundLevelStart; } }

	public AudioClip audioBackgroundLevelExitOpen { get { return settings.audioBackgroundLevelExitOpen == null ? prefabs.audioBackgroundLevelExitOpen : settings.audioBackgroundLevelExitOpen; } }

	public AudioClip audioBackgroundLevelMusic {
		get {
			if (!isRunning) {
				return audioBackgroundLevelStart ? audioBackgroundLevelStart : audioBackgroundPause;
			} else if (isPause) {
				return audioBackgroundPause;
			} else if (playerLevelSettings.scoreReached) {
				return audioBackgroundLevelExitOpen ? audioBackgroundLevelExitOpen : audioBackgroundMusic;
			} else {
				return audioBackgroundMusic;
			}
		}
	}

	//public float effectVolume { get { return AllLevels.Get ().options.EffectVolume; } set { AllLevels.Get ().options.EffectVolume = value; } }

	public bool isRunning = false;
	public bool isPause = false;

	public bool isDeath { get { return player.GetComponent<MAHN42.ThirdPersonCharacter> ().GetDeath (); } set { player.GetComponent<MAHN42.ThirdPersonCharacter> ().SetDeath (value); } }

	public Vector3 m_CameraOffsetForward = new Vector3 (0f, 2.5f, -1.2f);
	public Vector3 m_CameraOffsetFocus = new Vector3 (0, 1.5f, -0.5f);
	public Vector3 m_CameraOffsetSpectate = new Vector3 (0f, 4f, -3f);

	public Maze.Point m_minLOD = new Maze.Point (4, 4, 4);
	public Maze.Point m_spectateLOD = new Maze.Point (6, 6, 6);

	public Vector3 m_MazeCellSize = new Vector3 (1f, 1f, 1f);

	DungeonCamera m_dungeonCamera;

	public DungeonCamera dungeonCamera {
		get {
			if (!m_dungeonCamera)
				m_dungeonCamera = Camera.main.GetComponent<DungeonCamera> ();
			return m_dungeonCamera;
		}
	}

	GameObject m_playerProtectionEffect;
	public GameObject m_MainMazeParent;
	public Transform m_InventoryParent;

	// Use this for initialization
	void Awake ()
	{
		if (!mazeParent)
			mazeParent = GameObject.Find ("Maze").transform;
		if (!m_planeGround)
			m_planeGround = GameObject.Find ("Ground");
		if (!m_textScore)
			m_textScore = GameObject.Find ("TextScore").GetComponent<Text> ();
		if (!m_textTime)
			m_textTime = GameObject.Find ("TextTime").GetComponent<Text> ();
		if (!m_textLives)
			m_textLives = GameObject.Find ("TextLives").GetComponent<Text> ();
		if (!m_textHealth)
			m_textHealth = GameObject.Find ("TextHealth").GetComponent<Text> ();
		if (!m_textInventory)
			m_textInventory = GameObject.Find ("TextInventory").GetComponent<Text> ();
		if (!m_mainLight)
			m_mainLight = GameObject.Find ("MainDirectionalLight").GetComponent<Light> ();
		if (!m_panelToast)
			m_panelToast = GameObject.Find ("PanelToast").GetComponent<RectTransform> ();
		if (!m_panelPause)
			m_panelPause = GameObject.Find ("PanelPause").GetComponent<RectTransform> ();
		if (!m_panelLevelFinished)
			m_panelLevelFinished = GameObject.Find ("PanelLevelFinished").GetComponent<RectTransform> ();
		if (!m_panelMiniMap)
			m_panelMiniMap = GameObject.Find ("PanelMiniMap").GetComponent<RectTransform> ();
		if (!m_imageMiniMap)
			m_imageMiniMap = GameObject.Find ("ImageMiniMap").GetComponent<RawImage> ();
		if (!m_panelStart)
			m_panelStart = GameObject.Find ("PanelStart").GetComponent<RectTransform> ();
		// Start Panel
		if (!m_textSDescription)
			m_textSDescription = GameObject.Find ("TextSDescription").GetComponent<Text> ();
		if (!m_textSName)
			m_textSName = GameObject.Find ("TextSName").GetComponent<Text> ();
		if (!m_textSLevel)
			m_textSLevel = GameObject.Find ("TextSLevel").GetComponent<Text> ();
		if (!m_textSAchievements)
			m_textSAchievements = GameObject.Find ("TextSAchievements").GetComponent<Text> ();
		// Toast Panel
		if (!m_textToastTitle)
			m_textToastTitle = GameObject.Find ("TextToastTitle").GetComponent<Text> ();
		if (!m_textToastText)
			m_textToastText = GameObject.Find ("TextToastText").GetComponent<Text> ();
		// Pause Panel
		if (!m_textDescription)
			m_textDescription = GameObject.Find ("TextDescription").GetComponent<Text> ();
		if (!m_textName)
			m_textName = GameObject.Find ("TextName").GetComponent<Text> ();
		if (!m_textLevel)
			m_textLevel = GameObject.Find ("TextLevel").GetComponent<Text> ();
		// Level End Panel
		if (!m_textLFLevel)
			m_textLFLevel = GameObject.Find ("TextLFLevel").GetComponent<Text> ();
		if (!m_textLFName)
			m_textLFName = GameObject.Find ("TextLFName").GetComponent<Text> ();
		if (!m_textLFScore)
			m_textLFScore = GameObject.Find ("TextLFScore").GetComponent<Text> ();
		if (!m_textLFTime)
			m_textLFTime = GameObject.Find ("TextLFTime").GetComponent<Text> ();
		if (!m_textLFScoreBonus)
			m_textLFScoreBonus = GameObject.Find ("TextLFScoreBonus").GetComponent<Text> ();
		if (!m_textLFTimeBonus)
			m_textLFTimeBonus = GameObject.Find ("TextLFTimeBonus").GetComponent<Text> ();
		if (!m_textFPS)
			m_textFPS = GameObject.Find ("TextFPS").GetComponent<Text> ();
		if (!m_InventoryParent)
			m_InventoryParent = GameObject.Find ("Inventory").transform;
		if (!m_audioSourceBackground)
			m_audioSourceBackground = GameObject.Find ("AudioSourceBackground").GetComponent<AudioSource> ();
		if (!m_audioSourceEffects)
			m_audioSourceEffects = GameObject.Find ("AudioSourceEffects").GetComponent<AudioSource> ();
	}

	public void Start ()
	{
		ChangeEffectVolume (AllLevels.Get ().options.EffectVolume);
		ChangeMusicVolume (AllLevels.Get ().options.MusicVolume);
		SetShowFPS (AllLevels.Get ().options.ShowFPS);
		m_panelStart.gameObject.SetActive (false);
		m_panelPause.gameObject.SetActive (false);
		m_panelLevelFinished.gameObject.SetActive (false);
		m_panelToast.gameObject.SetActive (false);
		CreateMainMazeParent ("Main");
		InitUIInventory ();
	}

	public string GetLocalText (string aKey)
	{
		return AllLevels.Get ().local.GetText (aKey);
	}
	
	// Update is called once per frame
	void LateUpdate ()
	{
		m_textScore.text = string.Format (GetLocalText ("HUDScore"), playerLevelSettings.score, settings.scoreForExit);
		if (!playerLevelSettings.scoreReached) {
			if (!isPause) {
				if (settings.maxTime > 0) {
					playerLevelSettings.time = Mathf.Max (settings.maxTime - (playerLevelSettings.resumeTime + (Time.realtimeSinceStartup - playerLevelSettings.startTime)), 0f);
					if (playerLevelSettings.time <= 0) {
						TakeDamage (Time.deltaTime * 200f); //TODO Faktor für Rest Stärke Aufwertung wenn Zeit abgelaufen
					}
				} else {
					playerLevelSettings.time = playerLevelSettings.resumeTime + (Time.realtimeSinceStartup - playerLevelSettings.startTime);
				}
			}
			m_textTime.text = string.Format (GetLocalText ("HUDTime"), Mathf.RoundToInt (playerLevelSettings.time));
		}
		playerLevelSettings.levelRuntime = playerLevelSettings.resumeTime + (Time.realtimeSinceStartup - playerLevelSettings.startTime);
		m_textLives.text = string.Format (GetLocalText ("HUDLives"), playerLevelSettings.lives);
		m_textHealth.text = string.Format (GetLocalText ("HUDHealth"), Mathf.RoundToInt (playerLevelSettings.health));
		if (playerInventory) {
			m_textInventory.text = playerInventory.forDisplay ();
		}
		CheckLOD ();
		if (isRunning) {
			if (Input.GetKeyUp (KeyCode.Escape)) {
				if (isPause) {
					ResumeLevel ();
				} else {
					PauseLevel ();
				}
			}
		}
		if (m_panelStart.gameObject.activeSelf) {
			if (Input.GetKeyUp (KeyCode.Space) || Input.GetKeyUp (KeyCode.Return) || Input.GetButtonDown ("Fire1")) {
				CancelInvoke ("StartLevel");
				StartLevel ();
			}
		} else if (m_panelLevelFinished.gameObject.activeSelf) {
			if (Input.GetKeyUp (KeyCode.Escape)) {
				StartChooseLevel ();
			}
			if (Input.GetKeyUp (KeyCode.Space) || Input.GetKeyUp (KeyCode.Return) || Input.GetButtonDown ("Fire2")) {
				StartNextLevel ();
			}
		}
		Maze.Cell lCell = builder.Maze.get (builder.GetPlayerMazePoint ());
		if (lCell != null) {
			lCell.playerHasVisited = true;
		}
		CheckPlayerProtection ();
	}

	public void CheckLOD ()
	{
		if (builder == null)
			return;
		//deactivate far cells and activate near by cells
		Maze.Point lPoint = builder.GetPlayerMazePoint ();
		Maze.Point lLOD;
		if (dungeonCamera.mode == DungeonCamera.Mode.Spectate) {
			lLOD = m_spectateLOD;
		} else {
			lLOD = m_minLOD;
		}
		Maze.Point lMin = new Maze.Point (lPoint.x - lLOD.x, lPoint.y - lLOD.y, lPoint.z - lLOD.z);
		Maze.Point lMax = new Maze.Point (lPoint.x + lLOD.x, lPoint.y + lLOD.y, lPoint.z + lLOD.z);
		for (int y = 0; y < builder.Maze.height; y++) {
			for (int z = 0; z < builder.Maze.depth; z++) {
				for (int x = 0; x < builder.Maze.width; x++) {
					Maze.Cell lCell = builder.Maze.get (x, y, z);
					if (x >= lMin.x && x <= lMax.x && y >= lMin.y && y <= lMax.y && z >= lMin.z && z <= lMax.z) {
						if (!lCell.gameObject.activeSelf) {
							lCell.gameObject.SetActive (true);
						}
					} else {
						if (lCell.gameObject.activeSelf) {
							lCell.gameObject.SetActive (false);
						}
					}
				}
			}
		}
	}

	public void Generate (LevelSettings aSettings)
	{
		playerLevelSettings = new PlayerLevelSettings ();
		settings = aSettings;
		prefabs = AllLevels.Get ().GetCellDescription (settings.prefabs);
		string lStartUpText = settings.startupText;
		if (string.IsNullOrEmpty (lStartUpText)) {
			lStartUpText = string.Format (GetLocalText ("MakeYouReadyForLevel"), settings.name);
		}
		ShowToast (settings.name, lStartUpText);
		CreateLabyrinth ();
		SetupScene ();
	}

	public void SetupScene ()
	{
		m_mainLight.color = settings.dayLightColor;
		m_mainLight.intensity = settings.dayLight;
		m_planeGround.GetComponent<Renderer> ().material.color = settings.groundColor;
		if (settings.groundTexture != null) {
			m_planeGround.GetComponent<Renderer> ().material.SetTexture ("_MainTex", settings.groundTexture);
		}
		RenderSettings.ambientMode = settings.ambientMode;
		RenderSettings.ambientLight = settings.ambientLightColor;
		RenderSettings.ambientIntensity = settings.ambientLight;
		dungeonCamera.transform.position = player.transform.position + m_CameraOffsetSpectate;
	}

	protected void CreateMainMazeParent (string aName)
	{
		m_MainMazeParent = new GameObject ();
		m_MainMazeParent.name = aName;
		m_MainMazeParent.transform.SetParent (mazeParent.transform, false);
	}

	protected void CreateLabyrinth ()
	{
		builder = new MazeBuilder ();
		if (m_MazeCellSize.x == 1 && m_MazeCellSize.y == 1 && m_MazeCellSize.z == 1) {
			builder.positionScale = prefabs.prefabSize;
		} else {
			builder.positionScale = m_MazeCellSize;
		}
		builder.settings = settings;
		builder.prefabs = prefabs;
		builder.CreateLabyrinth (m_MainMazeParent.transform);
		isRunning = false;
		isPause = true;
		if (player) {
			Destroy (player);
		}
		Maze.Point lPStart = settings.playerStart;
		if (Maze.Point.IsNullOrEmpty (settings.playerStart)) {
			lPStart = new Maze.Point (settings.mazeWidth / 2, settings.mazeHeight / 2, settings.mazeDepth / 2);
		}
		Vector3 lPos = builder.GetVectorFromMazePoint (lPStart);
		player = Instantiate (playerPrefab, mazeParent.TransformPoint (lPos), Quaternion.identity) as GameObject;
		CreateMazeMapTexture ();
	}

	public Texture2D mazeMapTexture;

	protected void CreateMazeMapTexture ()
	{
		int w = 11;
		mazeMapTexture = new Texture2D (builder.Maze.width * w, builder.Maze.depth * w);
		TextureUtils.DrawMaze (mazeMapTexture, builder.Maze, w, Color.black);
		mazeMapTexture.Apply (false);
	}

	public void PlayOnBackground (AudioClip aClip)
	{
		m_audioSourceBackground.Stop ();
		if (aClip) {
			m_audioSourceBackground.clip = aClip;
			m_audioSourceBackground.Play ();
		}
	}

	public void StartLevelIntro ()
	{
		StartLevelStartScreen ();
		dungeonCamera.mode = DungeonCamera.Mode.Spectate;
		Invoke ("StartLevel", 5f);
	}

	public void StartLevel ()
	{
		CloseLevelStartScreen ();
		dungeonCamera.mode = DungeonCamera.Mode.FollowTarget;
		ResetCamera ();
		isRunning = true;
		isPause = false;
		playerLevelSettings.startTime = Time.realtimeSinceStartup;
		playerLevelSettings.levelRuntime = 0f;
		//TODO if in level mode
		playerLevelSettings.lives = settings.lives;
		m_panelPause.gameObject.SetActive (false);
		m_panelLevelFinished.gameObject.SetActive (false);
		m_textLevel.text = settings.level.ToString ();
		m_textName.text = settings.name;
		m_textDescription.text = settings.levelDescription;
		PlayerAwake ();
		PlayOnBackground (audioBackgroundMusic);
	}

	public void PauseLevel ()
	{
		isPause = true;
		playerLevelSettings.resumeTime = playerLevelSettings.time;
		m_panelPause.gameObject.SetActive (true);
		PlayOnBackground (audioBackgroundPause);
	}

	public void ResumeLevel ()
	{
		isPause = false;
		playerLevelSettings.startTime = Time.realtimeSinceStartup;
		PlayOnBackground (audioBackgroundLevelMusic);
		m_panelPause.gameObject.SetActive (false);
	}

	public void AddScore (int aScore)
	{
		playerLevelSettings.score += aScore;
		PlayScoreAudio (aScore, player.transform.position);
		if (!playerLevelSettings.scoreReached && settings.scoreForExit > 0 && playerLevelSettings.score >= settings.scoreForExit) {
			playerLevelSettings.scoreReached = true;
			if (prefabs.audioScoreReached) {
				PlayAudioEffect (prefabs.audioScoreReached);
				PlayOnBackground (audioBackgroundLevelMusic);
			} else {
				Debug.Log ("No audio for score reached!");
			}
			if (builder != null) {
				builder.ActivateExits ();
				builder.ActivateWayPoints (builder.GetPlayerMazePoint (), builder.exitPoint);
			}
		}
	}

	public void AddLives (int aLives)
	{
		playerLevelSettings.lives += aLives;
		if (prefabs.audioLiveAdded) {
			PlayAudioEffect (prefabs.audioLiveAdded);
		} else {
			Debug.Log ("No audio for live added!");
		}
	}

	public void PlayHealthAudio (float aHealth, Vector3 aPos)
	{
		AudioClip lAudio = null;
		if (aHealth < 5f) {
			lAudio = prefabs.audioHealthSmall;
		} else if (aHealth < 20f) {
			lAudio = prefabs.audioHealthMedium;
		} else {
			lAudio = prefabs.audioHealthBig;
		}
		if (lAudio) {
			PlayAudioEffect (lAudio);
		} else {
			Debug.Log (string.Format ("No audio for health {0}!", aHealth));
		}
	}

	public void AddHealth (float aHealth)
	{
		playerLevelSettings.health += aHealth;
		PlayHealthAudio (aHealth, player.transform.position);
	}

	public AudioClip GetAudioItemGet (string aType)
	{
		AudioClip lAudio = prefabs.GetAudioItemGet (aType);
		if (lAudio == null) {
			lAudio = AllLevels.Get ().inventory.GetAudioItemGet (aType);
		}
		return lAudio;
	}

	public AudioClip GetAudioItemUse (string aType)
	{
		AudioClip lAudio = prefabs.GetAudioItemUse (aType);
		if (lAudio == null) {
			lAudio = AllLevels.Get ().inventory.GetAudioItemUse (aType);
		}
		return lAudio;
	}

	public AudioClip GetAudioItemDrop (string aType)
	{
		AudioClip lAudio = prefabs.GetAudioItemDrop (aType);
		if (lAudio == null) {
			lAudio = AllLevels.Get ().inventory.GetAudioItemDrop (aType);
		}
		return lAudio;
	}

	[System.Serializable]
	public class UIInventory
	{
		public Text count;
		public Transform parent;
		public Light light;
		public Canvas canvas;
		public GameObject currentItem;

		public UIInventory (Transform aParent)
		{
			parent = aParent;
			currentItem = null;
			canvas = parent.FindChild ("Canvas").GetComponent<Canvas> ();
			count = canvas.transform.FindChild ("TextCount").GetComponent<Text> ();
			light = parent.FindChild ("Light").GetComponent<Light> ();
		}

		public void Clear ()
		{
			if (currentItem) {
				Destroy (currentItem);
			}
			count.text = string.Empty;
			light.enabled = false;
		}

		public void SetItem (PlayerInventory.InventoryItem aItem)
		{
			Clear ();
			if (aItem != null) {
				GameObject lPrefab = AllLevels.Get ().inventory.Get (aItem.type).Prefab;
				if (lPrefab) {
					currentItem = Instantiate (lPrefab) as GameObject;
					currentItem.transform.SetParent (parent, false);
				}
				if (aItem.count > 1) {
					count.text = aItem.count.ToString ();
				}
				light.enabled = true;
			}
		}
	}

	public UIInventory[] UIInventoryItems = { };

	public void InitUIInventory ()
	{
		int lPlaceIndex = 1;
		do {
			Transform lPlace = m_InventoryParent.FindChild ("Place" + lPlaceIndex.ToString ());
			if (lPlace == null) {
				break;
			}
			System.Array.Resize<UIInventory> (ref UIInventoryItems, UIInventoryItems.Length + 1);
			UIInventoryItems [lPlaceIndex - 1] = new UIInventory (lPlace);
			UIInventoryItems [lPlaceIndex - 1].Clear ();
			lPlaceIndex++;
		} while(true);
	}

	public void UpdateInventoryUI ()
	{
		int lPlaceIndex = 0;
		foreach (PlayerInventory.InventoryItem lItem in playerInventory.m_Items) {
			if (lItem.isVisibleInUI) {
				UIInventoryItems [lPlaceIndex].SetItem (lItem);
				lPlaceIndex++;
			}
		}
	}

	public void AddInventoryItem (PlayerInventory.InventoryItem aItem)
	{
		playerInventory.AddItem (aItem);
		AudioClip lAudio = GetAudioItemGet (aItem.type);
		if (lAudio) {
			PlayAudioEffect (lAudio);
		}
		UpdateInventoryUI ();
	}

	public void AddInventoryItems (PlayerInventory.InventoryItem[] aItems)
	{
		foreach (PlayerInventory.InventoryItem lItem in aItems) {
			AddInventoryItem (lItem);
		}
	}

	public void SubInventoryItem (PlayerInventory.InventoryItem aItem)
	{
		playerInventory.SubItem (aItem);
		AudioClip lAudio = GetAudioItemUse (aItem.type);
		if (lAudio) {
			PlayAudioEffect (lAudio);
		}
		UpdateInventoryUI ();
	}

	public void SubInventoryItems (PlayerInventory.InventoryItem[] aItems)
	{
		foreach (PlayerInventory.InventoryItem lItem in aItems) {
			SubInventoryItem (lItem);
		}
	}

	public void AddPickupData (PickupData aPickup)
	{
		if (aPickup.score > 0) {
			AddScore (aPickup.score);
		}
		if (aPickup.health > 0f) {
			AddHealth (aPickup.health);
		}
		if (aPickup.lives > 0) {
			AddLives (aPickup.lives);
		}
		if (aPickup.items.Length > 0) {
			AddInventoryItems (aPickup.items);
		}
	}

	void CheckPlayerProtection ()
	{
		if (playerLevelSettings.isProtected && playerLevelSettings.protectionTime <= playerLevelSettings.levelRuntime) {
			DeactivatePlayerProtection ();
		}
	}

	public void DeactivatePlayerProtection ()
	{
		playerLevelSettings.isProtected = false;
		if (m_playerProtectionEffect != null) {
			Destroy (m_playerProtectionEffect);
		}

	}

	public void ActivatePlayerProtection ()
	{
		playerLevelSettings.isProtected = true;
		playerLevelSettings.protectionTime = playerLevelSettings.levelRuntime + settings.playerProtectionTime;
		GameObject lEffectPrefab = AllLevels.LoadResource<GameObject> ("PlayerProtectionEffect", "Prefabs");
		m_playerProtectionEffect = Instantiate (lEffectPrefab, Vector3.up * 0.5f, Quaternion.identity) as GameObject;
		m_playerProtectionEffect.transform.SetParent (player.transform, false);
	}

	public void TakeDamage (float aDamage)
	{
		if (isRunning && !isPause && !playerLevelSettings.isProtected && !isDeath) {
			playerLevelSettings.health = Mathf.Max (playerLevelSettings.health - aDamage, 0f);
			PlayDamageAudio (aDamage, player.transform.position);
			if (playerLevelSettings.health <= 0) {
				PlayerDied ();
			}
		}
	}

	public void TakeDamage (DamageData aDamage)
	{
		TakeDamage (aDamage.Damage);
	}

	public void PlayerAwake ()
	{
		player.GetComponent<MAHN42.ThirdPersonCharacter> ().SetDeath (false);
		playerLevelSettings.health = 100f;
		ActivatePlayerProtection ();
	}

	public void PlayerDied ()
	{
		if (!isDeath) {
			isDeath = true;
			if (prefabs.audioLiveLost) {
				PlayAudioEffect (prefabs.audioLiveLost);
			} else {
				Debug.Log ("No audio for live lost!");
			}
			if (playerLevelSettings.lives > 1) {
				playerLevelSettings.lives--;
				Invoke ("PlayerAwake", 4f);
			} else {
				Invoke ("PlayerInDeathMode", 4f);
			}
		}
	}

	public void PlayerInDeathMode ()
	{
		//TODO: Death Mode
		if (!string.IsNullOrEmpty (settings.deathLevel)) {
			playerLevelSettings.resumeTime = playerLevelSettings.time;
			levelStack.Push (new LevelStackItem (this));
			m_MainMazeParent.SetActive (false);
			CreateMainMazeParent ("Death_" + levelStack.Count.ToString ());
			settings = AllLevels.Get ().GetLevel (settings.deathLevel);
			Generate (settings);
			StartLevelIntro ();
		} else {
			player.GetComponent<MAHN42.ThirdPersonCharacter> ().SetDeath (true);
			ShowToast (settings.name, GetLocalText ("GameOver"), 5f);
			Invoke ("StartChooseLevel", 5f);
		}
	}

	public void PlayAudioEffect (AudioClip aClip)
	{
		m_audioSourceEffects.PlayOneShot (aClip);
	}

	public void PlayDamageAudio (float aDamage, Vector3 aPos)
	{
		AudioClip lAudio = null;
		if (aDamage < 5) {
			lAudio = prefabs.audioDamageSmall;
		} else if (aDamage < 20) {
			lAudio = prefabs.audioDamageMedium;
		} else {
			lAudio = prefabs.audioDamageBig;
		}
		if (lAudio) {
			PlayAudioEffect (lAudio);
		} else {
			Debug.Log (string.Format ("No audio for damage {0}!", aDamage));
		}
	}

	public void PlayScoreAudio (int aScore, Vector3 aPos)
	{
		AudioClip lAudio = null;
		for (int i = 0; i < prefabs.audioScore.Length; i++) {
			if (aScore >= prefabs.audioScore [i].score) {
				lAudio = prefabs.audioScore [i].audio;
				break;
			}
		}
		if (lAudio) {
			PlayAudioEffect (lAudio);
		} else {
			Debug.Log (string.Format ("No audio for score {0}!", aScore));
		}
	}

	public void StartLevelStartScreen ()
	{
		m_textSLevel.text = settings.level.ToString ();
		m_textSName.text = settings.name;
		m_textSDescription.text = settings.levelDescription;
		m_textSAchievements.text = string.Format ("Score: {0} Time: {1} ", settings.scoreForExit, Mathf.RoundToInt (settings.maxTime));
		m_panelStart.gameObject.SetActive (true);
		PlayOnBackground (audioBackgroundLevelStart);
	}

	public void CloseLevelStartScreen ()
	{
		m_panelStart.gameObject.SetActive (false);
	}

	public void StartLevelEndScreen ()
	{
		if (playerLevelSettings.score > settings.scoreForExit) {
			playerLevelSettings.scoreBonus = Mathf.RoundToInt ((playerLevelSettings.score - settings.scoreForExit) * settings.scoreBonusFactor);
		}
		if (settings.maxTime > 0 && playerLevelSettings.time > 0) {
			playerLevelSettings.timeBonus = playerLevelSettings.time;
			playerLevelSettings.scoreTimeBonus = Mathf.RoundToInt (playerLevelSettings.timeBonus * settings.scoreTimeBonusFactor);
			m_textLFTimeBonus.gameObject.SetActive (true);
		} else {
			m_textLFTimeBonus.gameObject.SetActive (false);
		}
		m_textLFLevel.text = settings.level.ToString ();
		m_textLFName.text = settings.name;
		m_textLFScore.text = string.Format (GetLocalText ("LevelEndScore"), playerLevelSettings.score.ToString ());
		m_textLFScoreBonus.text = string.Format (GetLocalText ("LevelEndScoreBonus"), playerLevelSettings.scoreBonus.ToString ());
		m_textLFTime.text = string.Format (GetLocalText ("LevelEndTime"), Mathf.RoundToInt (playerLevelSettings.time).ToString ());
		m_textLFTimeBonus.text = string.Format (GetLocalText ("LevelEndTimeBonus"), playerLevelSettings.scoreTimeBonus.ToString ());
		m_panelLevelFinished.gameObject.SetActive (true);
		PlayOnBackground (audioBackgroundLevelEnd);
		PlayerLevel lL = AllLevels.Get ().currentPlayer.GetLevel (settings.worldName, settings.levelName, true);
		lL.level = settings.level;
		int lScoreComplete = playerLevelSettings.score + playerLevelSettings.scoreBonus + playerLevelSettings.scoreTimeBonus;
		if (lScoreComplete > lL.scoreComplete) {
			lL.score = playerLevelSettings.score;
			lL.scoreBonus = playerLevelSettings.scoreBonus;
			lL.timeBonusScore = playerLevelSettings.scoreTimeBonus;
			lL.time = playerLevelSettings.time;
			lL.scoreComplete = lScoreComplete;
		}
		if (lL.minTime <= 0 || playerLevelSettings.time < lL.minTime) {
			lL.minTime = playerLevelSettings.time;
		}
		AllLevels.Get ().SaveData ();
	}

	/* for controlled methods */
	public void PlayerHasExitReached (string lLevelName = null)
	{
		if (isRunning) {
			isPause = true;
			isRunning = false;
			playerLevelSettings.nextLevelName = lLevelName;
			Invoke ("StartLevelEndScreen", 1.5f);
		}
	}

	public void StartNextLevel ()
	{
		if (levelStack.Count == 0) {
			AllLevels.Get ().NextLevel (playerLevelSettings.nextLevelName);
		} else {
			m_panelLevelFinished.gameObject.SetActive (false);
			m_MainMazeParent.SetActive (false);
			Destroy (m_MainMazeParent);
			levelStack.Pop ().Restore (this);
			m_MainMazeParent.SetActive (true);
			isRunning = true;
			SetupScene ();
			ResumeLevel ();
			PlayerAwake ();
		}
	}

	/* for controlled methods */
	public void ResetCamera ()
	{
		if (dungeonCamera) {
			dungeonCamera.offset = m_CameraOffsetForward;
		}
	}

	/* for controlled methods */
	public void FocusPlayer ()
	{
		if (dungeonCamera) {
			dungeonCamera.offset = m_CameraOffsetFocus;
		}
	}

	/* for controlled methods */
	public void FocusPlayerForShort ()
	{
		if (dungeonCamera) {
			FocusPlayer ();
			Invoke ("ResetCamera", 5f);
		}
	}

	public void StartChooseLevel ()
	{
		AllLevels.Get ().StartChooseLevel ();
	}

	/* for controlled methods */
	public void ShowToast (string aTitle, string aText, float aTime = 2f)
	{
		m_textToastTitle.text = aTitle;
		m_textToastText.text = aText;
		m_panelToast.gameObject.SetActive (true);
		Invoke ("CloseToast", aTime);
	}

	public void CloseToast ()
	{
		m_panelToast.gameObject.SetActive (false);
	}

	public void ChangeEffectVolume (float aValue)
	{
		AllLevels.Get ().options.EffectVolume = aValue;
		AllLevels.Get ().options.Save ();
		m_audioSourceEffects.volume = aValue;
	}

	public void ChangeMusicVolume (float aValue)
	{
		AllLevels.Get ().options.MusicVolume = aValue;
		AllLevels.Get ().options.Save ();
		m_audioSourceBackground.volume = aValue;
	}

	public void SetShowFPS (bool aShow)
	{
		AllLevels.Get ().options.ShowFPS = aShow;
		AllLevels.Get ().options.Save ();
		m_textFPS.gameObject.SetActive (aShow);
	}

	public bool GetShowFPS ()
	{
		return AllLevels.Get ().options.ShowFPS;
	}

	public bool GetShowMiniMap ()
	{
		return m_panelMiniMap.gameObject.activeSelf;
	}

	public void SetShowMiniMap (bool aShow)
	{
		m_panelMiniMap.gameObject.SetActive (aShow);
	}
}
