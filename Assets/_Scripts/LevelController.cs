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
		public float health = 100.0f;
		public float resumeTime = 0f;
		public float startTime = 0f;
		public float time = 0f;
		public bool scoreReached = false;
	}

	public LevelSettings settings;
	[SerializeField]
	public CellDescription prefabs;
	public MazeBuilder builder;

	public PlayerLevelSettings playerLevelSettings = new PlayerLevelSettings ();

	public Transform mazeParent;

	public GameObject player;

	public GameObject playerPrefab { get { return AllLevels.Get ().playerPrefab; } }

	public GameObject m_planeGround;
	public Text m_textScore;
	public Text m_textTime;
	public Text m_textLives;
	public Text m_textHealth;
	public Light m_mainLight;
	public RectTransform m_panelPause;
	public RectTransform m_panelLevelFinished;
	public Text m_textDescription;
	public Text m_textName;
	public Text m_textLevel;

	public Text m_textLFName;
	public Text m_textLFLevel;
	public Text m_textLFScore;
	public Text m_textLFScoreBonus;
	public Text m_textLFTime;
	public Text m_textLFTimeBonus;

	public AudioSource m_audioSourceBackground;

	public AudioClip audioBackgroundPause { get { return settings.audioBackgroundPause == null ? prefabs.audioBackgroundPause : settings.audioBackgroundPause; } }

	public AudioClip audioBackgroundMusic { get { return settings.audioBackgroundMusic == null ? prefabs.audioBackgroundMusic : settings.audioBackgroundMusic; } }

	public AudioClip audioBackgroundLevelEnd { get { return settings.audioBackgroundLevelEnd == null ? prefabs.audioBackgroundLevelEnd : settings.audioBackgroundLevelEnd; } }

	public float effectVolume = 0.5f;

	public bool isRunning = false;
	public bool isPause = false;

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
		if (!m_mainLight)
			m_mainLight = GameObject.Find ("MainDirectionalLight").GetComponent<Light> ();
		if (!m_panelPause)
			m_panelPause = GameObject.Find ("PanelPause").GetComponent<RectTransform> ();
		if (!m_panelLevelFinished)
			m_panelLevelFinished = GameObject.Find ("PanelLevelFinished").GetComponent<RectTransform> ();
		if (!m_textDescription)
			m_textDescription = GameObject.Find ("TextDescription").GetComponent<Text> ();
		if (!m_textName)
			m_textName = GameObject.Find ("TextName").GetComponent<Text> ();
		if (!m_textLevel)
			m_textLevel = GameObject.Find ("TextLevel").GetComponent<Text> ();
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
		m_panelPause.gameObject.SetActive (false);
		m_panelLevelFinished.gameObject.SetActive (false);
	}
	
	// Update is called once per frame
	void LateUpdate ()
	{
		m_textScore.text = string.Format ("Score: {0}/{1}", playerLevelSettings.score, settings.scoreForExit);
		if (!playerLevelSettings.scoreReached) {
			if (!isPause) {
				if (settings.maxTime > 0) {
					playerLevelSettings.time = Mathf.Max (settings.maxTime - (playerLevelSettings.resumeTime + (Time.realtimeSinceStartup - playerLevelSettings.startTime)), 0f);
				} else {
					playerLevelSettings.time = playerLevelSettings.resumeTime + (Time.realtimeSinceStartup - playerLevelSettings.startTime);
				}
			}
			m_textTime.text = string.Format ("Time: {0}", Mathf.RoundToInt (playerLevelSettings.time));
		}
		m_textLives.text = string.Format ("Lives: {0}", playerLevelSettings.lives);
		m_textHealth.text = string.Format ("Health: {0}", playerLevelSettings.health);
		CheckLOD ();
		if (Input.GetKeyUp (KeyCode.Escape)) {
			if (isPause) {
				ResumeLevel ();
			} else {
				PauseLevel ();
			}
		}
	}

	public void CheckLOD ()
	{
		if (builder == null)
			return;
		//deactivate far cells and activate near by cells
		Maze.Point lPoint = builder.GetPlayerMazePoint ();
		Maze.Point lMin = new Maze.Point (lPoint.x - 6, lPoint.y - 6, lPoint.z - 6);
		Maze.Point lMax = new Maze.Point (lPoint.x + 6, lPoint.y + 6, lPoint.z + 6);
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
		CreateLabyrinth ();
		m_mainLight.color = settings.dayLightColor;
		m_mainLight.intensity = settings.dayLight;
		m_planeGround.GetComponent<Renderer> ().material.color = settings.groundColor;
		if (settings.groundTexture != null) {
			m_planeGround.GetComponent<Renderer> ().material.SetTexture ("_MainTex", settings.groundTexture);
		}

	}

	protected void CreateLabyrinth ()
	{
		builder = new MazeBuilder ();
		builder.settings = settings;
		builder.prefabs = prefabs;
		builder.CreateLabyrinth (mazeParent);
		isRunning = false;
		isPause = true;
		if (player) {
			Destroy (player);
		}
		player = Instantiate (playerPrefab, new Vector3 (builder.Maze.width / 2, builder.Maze.height / 2, builder.Maze.depth / 2), Quaternion.identity) as GameObject;
	}

	public void PlayOnBackground (AudioClip aClip)
	{
		m_audioSourceBackground.Stop ();
		if (aClip) {
			m_audioSourceBackground.clip = aClip;
			m_audioSourceBackground.Play ();
		}
	}

	public void StartLevel ()
	{
		isRunning = true;
		isPause = false;
		playerLevelSettings.startTime = Time.realtimeSinceStartup;
		m_panelPause.gameObject.SetActive (false);
		m_panelLevelFinished.gameObject.SetActive (false);
		m_textLevel.text = settings.level.ToString ();
		m_textName.text = settings.levelName;
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
		PlayOnBackground (audioBackgroundMusic);
		m_panelPause.gameObject.SetActive (false);
	}

	public void AddScore (int aScore)
	{
		playerLevelSettings.score += aScore;
		PlayScoreAudio (aScore, player.transform.position);
		if (!playerLevelSettings.scoreReached && playerLevelSettings.score >= settings.scoreForExit) {
			playerLevelSettings.scoreReached = true;
			if (prefabs.audioScoreReached) {
				AudioSource.PlayClipAtPoint (prefabs.audioScoreReached, player.transform.position, effectVolume);
			} else {
				Debug.Log ("No audio for score reached!");
			}
			builder.ActivateExits ();
			builder.ActivateWayPoints (builder.GetPlayerMazePoint (), builder.exitPoint);
		}
	}

	public void AddLives (int aLives)
	{
		playerLevelSettings.lives += aLives;
		if (prefabs.audioLiveAdded) {
			AudioSource.PlayClipAtPoint (prefabs.audioLiveAdded, player.transform.position, effectVolume);
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
			AudioSource.PlayClipAtPoint (lAudio, aPos, effectVolume);
		} else {
			Debug.Log (string.Format ("No audio for health {0}!", aHealth));
		}
	}

	public void AddHealth (float aHealth)
	{
		playerLevelSettings.health += aHealth;
		PlayHealthAudio (aHealth, player.transform.position);
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
	}

	public void TakeDamage (DamageData aDamage)
	{
		playerLevelSettings.health -= aDamage.Damage;
		PlayDamageAudio (aDamage, player.transform.position);
		if (playerLevelSettings.health < 0) {
			playerLevelSettings.lives--;
			if (prefabs.audioLiveLost) {
				AudioSource.PlayClipAtPoint (prefabs.audioLiveLost, player.transform.position, effectVolume);
			} else {
				Debug.Log ("No audio for live lost!");
			}
			playerLevelSettings.health = 100;
		}
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
			AudioSource.PlayClipAtPoint (lAudio, aPos, effectVolume);
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
			AudioSource.PlayClipAtPoint (lAudio, aPos, effectVolume);
		} else {
			Debug.Log (string.Format ("No audio for score {0}!", aScore));
		}
	}

	public void StartLevelEndScreen()
	{
		m_panelLevelFinished.gameObject.SetActive (true);
		PlayOnBackground (audioBackgroundLevelEnd);
	}

	public void PlayerHasExitReached ()
	{
		isPause = true;
		isRunning = false;
		Invoke("StartLevelEndScreen", 1.5f);
	}

	public void StartNextLevel ()
	{
		AllLevels.Get ().NextLevel ();
	}

}
