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
	}

	public LevelSettings settings;
	[SerializeField]
	public CellDirectionObjects prefabs;
	public MazeBuilder builder;

	public PlayerLevelSettings playerLevelSettings = new PlayerLevelSettings ();

	public Transform mazeParent;

	public GameObject player;

	public GameObject playerPrefab { get { return AllLevels.Get ().playerPrefab; } }

	public Text m_textScore;
	public Text m_textTime;
	public Text m_textLives;
	public Text m_textHealth;
	public Light m_mainLight;

	public bool isRunning = false;
	public bool isPause = false;

	// Use this for initialization
	void Awake ()
	{
		if (!mazeParent)
			mazeParent = GameObject.Find ("Maze").transform;
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
	}
	
	// Update is called once per frame
	void LateUpdate ()
	{
		m_textScore.text = string.Format ("Score: {0}/{1}", playerLevelSettings.score, settings.scoreForExit);
		if (isRunning) {
			if (playerLevelSettings.score < settings.scoreForExit && !isPause) {
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
	}

	public void CheckLOD ()
	{
		if (builder == null) return;
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

	public void StartLevel ()
	{
		isRunning = true;
		isPause = false;
		playerLevelSettings.startTime = Time.realtimeSinceStartup;
	}

	public void PauseLevel ()
	{
		isPause = true;
		playerLevelSettings.resumeTime = playerLevelSettings.time;
	}

	public void ResumeLevel ()
	{
		isPause = false;
		playerLevelSettings.startTime = Time.realtimeSinceStartup;
	}

	public void AddScore (int aScore)
	{
		playerLevelSettings.score += aScore;
	}

	public void TakeDamage (DamageData aDamage)
	{
		playerLevelSettings.health -= aDamage.Damage;
		// play audio on player position
		if (playerLevelSettings.health < 0) {
			playerLevelSettings.lives--;
			playerLevelSettings.health = 100;
		}
	}

}
