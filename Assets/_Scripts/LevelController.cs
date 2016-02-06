using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelController : MonoBehaviour
{

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
	}

	public LevelSettings settings;
	[SerializeField]
	public CellDescription prefabs;
	public MazeBuilder builder;

	public PlayerLevelSettings playerLevelSettings = new PlayerLevelSettings ();

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
				m_playerInventory = player.GetComponent<PlayerInventory> ();
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

	public float effectVolume = 0.5f;

	public bool isRunning = false;
	public bool isPause = false;

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
		if (!m_audioSourceBackground)
			m_audioSourceBackground = GameObject.Find ("AudioSourceBackground").GetComponent<AudioSource> ();
		if (!m_audioSourceEffects)
			m_audioSourceEffects = GameObject.Find ("AudioSourceEffects").GetComponent<AudioSource> ();
		m_panelStart.gameObject.SetActive (false);
		m_panelPause.gameObject.SetActive (false);
		m_panelLevelFinished.gameObject.SetActive (false);
		m_panelToast.gameObject.SetActive (false);
	}

	public string GetLocalText(string aKey)
	{
		return AllLevels.Get().local.GetText(aKey);
	}
	
	// Update is called once per frame
	void LateUpdate ()
	{
		m_textScore.text = string.Format (GetLocalText("HUDScore"), playerLevelSettings.score, settings.scoreForExit);
		if (!playerLevelSettings.scoreReached) {
			if (!isPause) {
				if (settings.maxTime > 0) {
					playerLevelSettings.time = Mathf.Max (settings.maxTime - (playerLevelSettings.resumeTime + (Time.realtimeSinceStartup - playerLevelSettings.startTime)), 0f);
				} else {
					playerLevelSettings.time = playerLevelSettings.resumeTime + (Time.realtimeSinceStartup - playerLevelSettings.startTime);
				}
			}
			m_textTime.text = string.Format (GetLocalText("HUDTime"), Mathf.RoundToInt (playerLevelSettings.time));
		}
		m_textLives.text = string.Format (GetLocalText("HUDLives"), playerLevelSettings.lives);
		m_textHealth.text = string.Format (GetLocalText("HUDHealth"), playerLevelSettings.health);
		m_textInventory.text = playerInventory.forDisplay ();
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
		settings = aSettings;
		prefabs = AllLevels.Get ().GetCellDescription (settings.prefabs);
		string lStartUpText = settings.startupText;
		if (string.IsNullOrEmpty (lStartUpText)) {
			lStartUpText = string.Format (GetLocalText("MakeYouReadyForLevel"), settings.name);
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

	protected void CreateLabyrinth ()
	{
		builder = new MazeBuilder ();
		builder.positionScale = m_MazeCellSize;
		builder.settings = settings;
		builder.prefabs = prefabs;
		builder.CreateLabyrinth (mazeParent);
		isRunning = false;
		isPause = true;
		if (player) {
			Destroy (player);
		}
		Vector3 lPos;
		if (settings.playerStart != null) {
			lPos = new Vector3 (settings.playerStart.x, settings.playerStart.y, settings.playerStart.z);
		} else {
			lPos = new Vector3 (builder.Maze.width / 2, builder.Maze.height / 2, builder.Maze.depth / 2);
		}
		player = Instantiate (playerPrefab, mazeParent.TransformPoint (lPos), Quaternion.identity) as GameObject;
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
		m_panelPause.gameObject.SetActive (false);
		m_panelLevelFinished.gameObject.SetActive (false);
		m_textLevel.text = settings.level.ToString ();
		m_textName.text = settings.name;
		m_textDescription.text = settings.levelDescription;
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
		if (!playerLevelSettings.scoreReached && playerLevelSettings.score >= settings.scoreForExit) {
			playerLevelSettings.scoreReached = true;
			if (prefabs.audioScoreReached) {
				PlayAudioEffect (prefabs.audioScoreReached);
				PlayOnBackground (audioBackgroundLevelMusic);
				//AudioSource.PlayClipAtPoint (prefabs.audioScoreReached, player.transform.position, effectVolume);
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
			//AudioSource.PlayClipAtPoint (prefabs.audioLiveAdded, player.transform.position, effectVolume);
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
			//AudioSource.PlayClipAtPoint (lAudio, aPos, effectVolume);
		} else {
			Debug.Log (string.Format ("No audio for health {0}!", aHealth));
		}
	}

	public void AddHealth (float aHealth)
	{
		playerLevelSettings.health += aHealth;
		PlayHealthAudio (aHealth, player.transform.position);
	}

	public AudioClip GetAudioItemGet(string aType)
	{
		AudioClip lAudio = prefabs.GetAudioItemGet (aType);
		if (lAudio == null) {
			lAudio = AllLevels.Get().inventory.GetAudioItemGet (aType);
		}
		return lAudio;
	}

	public AudioClip GetAudioItemUse(string aType)
	{
		AudioClip lAudio = prefabs.GetAudioItemUse (aType);
		if (lAudio == null) {
			lAudio = AllLevels.Get().inventory.GetAudioItemUse (aType);
		}
		return lAudio;
	}

	public AudioClip GetAudioItemDrop(string aType)
	{
		AudioClip lAudio = prefabs.GetAudioItemDrop (aType);
		if (lAudio == null) {
			lAudio = AllLevels.Get().inventory.GetAudioItemDrop (aType);
		}
		return lAudio;
	}

	public void AddInventoryItem (PlayerInventory.InventoryItem aItem)
	{
		playerInventory.AddItem (aItem);
		AudioClip lAudio = GetAudioItemGet(aItem.type);
		if (lAudio) {
			PlayAudioEffect (lAudio);
		}
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

	public void TakeDamage (DamageData aDamage)
	{
		playerLevelSettings.health -= aDamage.Damage;
		PlayDamageAudio (aDamage, player.transform.position);
		if (playerLevelSettings.health < 0) {
			playerLevelSettings.lives--;
			if (prefabs.audioLiveLost) {
				PlayAudioEffect (prefabs.audioLiveLost);
				//AudioSource.PlayClipAtPoint (prefabs.audioLiveLost, player.transform.position, effectVolume);
			} else {
				Debug.Log ("No audio for live lost!");
			}
			playerLevelSettings.health = 100;
		}
	}

	public void PlayAudioEffect (AudioClip aClip)
	{
		m_audioSourceEffects.PlayOneShot (aClip);
	}

	public void PlayDamageAudio (DamageData aDamage, Vector3 aPos)
	{
		AudioClip lAudio = null;
		if (aDamage.Damage < 5) {
			lAudio = prefabs.audioDamageSmall;
		} else if (aDamage.Damage < 20) {
			lAudio = prefabs.audioDamageMedium;
		} else {
			lAudio = prefabs.audioDamageBig;
		}
		if (lAudio) {
			PlayAudioEffect (lAudio);
			//AudioSource.PlayClipAtPoint (lAudio, aPos, effectVolume);
		} else {
			Debug.Log (string.Format ("No audio for damage {0}!", aDamage.Damage));
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
			//AudioSource.PlayClipAtPoint (lAudio, aPos, effectVolume);
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
		if (settings.maxTime > 0 && playerLevelSettings.time > settings.maxTime) {
			playerLevelSettings.timeBonus = settings.maxTime - playerLevelSettings.time;
			playerLevelSettings.scoreTimeBonus = Mathf.RoundToInt (playerLevelSettings.timeBonus * settings.scoreTimeBonusFactor);
			m_textLFTimeBonus.gameObject.SetActive (true);
		} else {
			m_textLFTimeBonus.gameObject.SetActive (false);
		}
		m_textLFLevel.text = settings.level.ToString ();
		m_textLFName.text = settings.name;
		m_textLFScore.text = string.Format (GetLocalText("LevelEndScore"), playerLevelSettings.score.ToString ());
		m_textLFScoreBonus.text = string.Format (GetLocalText("LevelEndScoreBonus"), playerLevelSettings.scoreBonus.ToString ());
		m_textLFTime.text = string.Format (GetLocalText("LevelEndTime"), Mathf.RoundToInt (playerLevelSettings.time).ToString ());
		m_textLFTimeBonus.text = string.Format (GetLocalText("LevelEndTimeBonus"), playerLevelSettings.scoreTimeBonus.ToString ());
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
	public void PlayerHasExitReached ()
	{
		if (isRunning) {
			isPause = true;
			isRunning = false;
			Invoke ("StartLevelEndScreen", 1.5f);
		}
	}

	public void StartNextLevel ()
	{
		AllLevels.Get ().NextLevel ();
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
}
