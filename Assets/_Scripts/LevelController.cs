using UnityEngine;
using System.Collections;

public class LevelController : MonoBehaviour {

	[System.Serializable]
	public class PlayerLevelSettings {
		public int lives = 1;
		public int score = 0;
		public float health = 100.0f;
	}

	public LevelSettings settings;
	public MazeBuilder builder;

	public PlayerLevelSettings playerLevelSettings = new PlayerLevelSettings();

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
