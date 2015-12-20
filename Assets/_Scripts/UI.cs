using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class UI : MonoBehaviour
{
	public GameObject playerPrefab;
	public GameObject mazeWallPrefab;
	public GameObject mazeWallLeftPrefab;
	public GameObject mazeWallRightPrefab;
	public GameObject mazeWallForwardPrefab;
	public GameObject mazeWallBackwardPrefab;
	public GameObject mazeWallTopPrefab;
	public GameObject mazeWallBottomPrefab;
	public GameObject mazeMarkerPrefab;
	public GameObject score1Prefab;
	public AudioClip scoreReachedAudio;
	public GameObject mainCamera;
	public InputField mazeUIWidth;
	public InputField mazeUIDepth;
	public InputField mazeUIHeight;
	public Text scoreUIText;
	public Text timeUIText;
	public float mazeWallScale = 0.125f;
	public int chanceForBreakWalls = 0;

	// Use this for initialization
	void Start ()
	{
		if (!mazeUIWidth) {
			mazeUIWidth = GameObject.Find ("InputFieldWidth").GetComponent<InputField> ();
		}
		if (!mazeUIDepth) {
			mazeUIDepth = GameObject.Find ("InputFieldDepth").GetComponent<InputField> ();
		}
		if (!mazeUIHeight) {
			mazeUIHeight = GameObject.Find ("InputFieldHeight").GetComponent<InputField> ();
		}
		if (!scoreUIText) {
			scoreUIText = GameObject.Find ("TextScore").GetComponent<Text> ();
		}
		if (!timeUIText) {
			timeUIText = GameObject.Find ("TextTime").GetComponent<Text> ();
		}
		if (!mainCamera) {
			mainCamera = GameObject.FindWithTag ("MainCamera");
		}
		if (AllLevels.Get ().currentLevelSettings != null) {
			LevelSettings lSet = AllLevels.Get ().currentLevelSettings;
			mazeUIWidth.text = lSet.mazeWidth.ToString ();
			mazeUIHeight.text = lSet.mazeHeight.ToString ();
			mazeUIDepth.text = lSet.mazeDepth.ToString ();
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (fRunning) {
			timeUIText.text = "Time: " + Mathf.RoundToInt ((Time.realtimeSinceStartup - fStartTime)).ToString ();
			scoreUIText.text = "Score: " + fScore.ToString () + "/" + fGoodScore.ToString ();
			if (fScore >= fGoodScore) {
				fRunning = false;
				if (scoreReachedAudio) {
					AudioSource.PlayClipAtPoint (scoreReachedAudio, fPlayer.transform.position);
				}
			}
		}
	}

	public void CreateLabyrinth (Transform aTransform = null)
	{
		CreateLabyrinth (aTransform, new Vector3 ());
	}

	protected void CreateLabyrinth (Transform aParent, Vector3 aPos)
	{
		MazeBuilder lBuilder = new MazeBuilder ();
		lBuilder.chanceForBreakWalls = chanceForBreakWalls;
		lBuilder.mazeWidth = int.Parse (mazeUIWidth.text);
		lBuilder.mazeHeight = int.Parse (mazeUIHeight.text);
		lBuilder.mazeDepth = int.Parse (mazeUIDepth.text);
		lBuilder.mazeMarkerPrefab = mazeMarkerPrefab;
		lBuilder.mazeWallBackwardPrefab = mazeWallBackwardPrefab;
		lBuilder.mazeWallBottomPrefab = mazeWallBottomPrefab;
		lBuilder.mazeWallForwardPrefab = mazeWallForwardPrefab;
		lBuilder.mazeWallLeftPrefab = mazeWallLeftPrefab;
		lBuilder.mazeWallPrefab = mazeWallPrefab;
		lBuilder.mazeWallRightPrefab = mazeWallRightPrefab;
		lBuilder.mazeWallScale = mazeWallScale;
		lBuilder.mazeWallTopPrefab = mazeWallTopPrefab;
		lBuilder.score1Prefab = score1Prefab;
		lBuilder.CreateLabyrinth (aParent, aPos);
		fGoodScore = Mathf.RoundToInt (0.75f * lBuilder.mazeDepth * lBuilder.mazeHeight * lBuilder.mazeWidth);
		fRunning = false;
	}

	protected float fStartTime;
	protected bool fRunning = false;
	protected int fScore = 0;
	protected int fGoodScore = 0;
	protected GameObject fPlayer;

	public void RunGame ()
	{
		if (fPlayer) {
			Destroy (fPlayer);
		}
		fPlayer = Instantiate (playerPrefab, new Vector3 (int.Parse (mazeUIWidth.text) / 2f, 0.5f, int.Parse (mazeUIDepth.text) / 2f), Quaternion.identity) as GameObject;
		//mainCamera.gameObject.SetActive (false);
		mainCamera.GetComponent<UnityStandardAssets.Cameras.AutoCam> ().SetTarget (fPlayer.transform);
		GameObject.Find ("Markers").SetActive (false);
		fStartTime = Time.realtimeSinceStartup;
		fScore = 0;
		fRunning = true;
		//timeUIText.text = "0";
		//scoreUIText.text = "0";
	}

	public void AddScore (int aScore)
	{
		fScore += aScore;
	}

	public void ButtonExit ()
	{
		SceneManager.LoadScene ("ChooseLevel", LoadSceneMode.Single);
	}
}
