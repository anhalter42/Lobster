using UnityEngine;
using UnityEngine.UI;
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
	public Camera mainCamera;
	public InputField mazeUIWidth;
	public InputField mazeUIDepth;
	public InputField mazeUIHeight;
	public Text scoreUIText;
	public Text timeUIText;
	public float mazeWallScale = 0.125f;

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
			mainCamera = GameObject.Find ("Main Camera").GetComponent<Camera> ();
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

	protected GameObject DropWall (GameObject aWallPrefab, Transform aParent, Vector3 aPos, Vector3 aScale)
	{
		//RaycastHit lHit;
		Vector3 lPos = aPos;
		//if (Physics.Raycast (aPos, Vector3.down, out lHit, 120.0f)) {
		//	lPos = new Vector3 (lHit.point.x, lHit.point.y + 0.4f, lHit.point.z);
		//}
		Quaternion lQ = new Quaternion ();
		//lQ.eulerAngles = new Vector3(Random.Range(-10.0f,10.0f),Random.Range(-10.0f,10.0f),Random.Range(-10.0f,10.0f));
		//Network.Instantiate(mazeWallPrefab, lPos, lQ, NetworkManager.GROUP_WORLD);
		GameObject lObj = Instantiate (aWallPrefab ? aWallPrefab : mazeWallPrefab, lPos, lQ) as GameObject;
		lObj.transform.localScale = aScale;
		if (aParent) {
			lObj.transform.SetParent (aParent, false);
		}
		return lObj;
	}

	protected Vector3[] fDirectionScales = new Vector3[6];
	protected GameObject[] fDirectionPrefabs = new GameObject[6];

	protected void CreateLabyrinth (Transform aParent, Vector3 aPos)
	{
		Debug.Log ("Creating Labyrinth...");
		fRunning = false;
		fDirectionScales [Maze.DirectionTop] = new Vector3 (1, mazeWallScale, 1);
		fDirectionScales [Maze.DirectionBottom] = new Vector3 (1, mazeWallScale, 1);
		fDirectionScales [Maze.DirectionRight] = new Vector3 (mazeWallScale, 1, 1);
		fDirectionScales [Maze.DirectionLeft] = new Vector3 (mazeWallScale, 1, 1);
		fDirectionScales [Maze.DirectionForward] = new Vector3 (1, 1, mazeWallScale);
		fDirectionScales [Maze.DirectionBackward] = new Vector3 (1, 1, mazeWallScale);
		fDirectionPrefabs [Maze.DirectionTop] = mazeWallTopPrefab ? mazeWallTopPrefab : mazeWallPrefab;
		fDirectionPrefabs [Maze.DirectionBottom] = mazeWallBottomPrefab ? mazeWallBottomPrefab : mazeWallPrefab;
		fDirectionPrefabs [Maze.DirectionRight] = mazeWallRightPrefab ? mazeWallRightPrefab : mazeWallPrefab;
		fDirectionPrefabs [Maze.DirectionLeft] = mazeWallLeftPrefab ? mazeWallLeftPrefab : mazeWallPrefab;
		fDirectionPrefabs [Maze.DirectionForward] = mazeWallForwardPrefab ? mazeWallForwardPrefab : mazeWallPrefab;
		fDirectionPrefabs [Maze.DirectionBackward] = mazeWallBackwardPrefab ? mazeWallBackwardPrefab : mazeWallPrefab;
		if (aParent) {
			for (int i = aParent.transform.childCount - 1; i >= 0; i--) {
				DestroyObject (aParent.transform.GetChild (i).gameObject);
			}
		}
		GameObject lWallParent = new GameObject ("Walls");
		GameObject lMarkerParent = new GameObject ("Markers");
		GameObject lScoreParent = new GameObject ("Scores");
		if (aParent) {
			lWallParent.transform.SetParent (aParent, false);
			lMarkerParent.transform.SetParent (aParent, false);
			lScoreParent.transform.SetParent (aParent, false);
		}
		Maze lMaze = new Maze (int.Parse (mazeUIWidth.text), int.Parse (mazeUIHeight.text), int.Parse (mazeUIDepth.text));
		lMaze.build ();
		Vector3 lPos = new Vector3 ();
		for (int z = 0; z< lMaze.depth; z++) {
			for (int y = 0; y < lMaze.height; y++) {
				for (int x = 0; x < lMaze.width; x++) {
					lPos.x = aPos.x + 1.0f * x; //2.5f
					lPos.y = aPos.y + 1.0f * y; // 0;
					lPos.z = aPos.z + 1.0f * z; //2.5f
					for (int lDir = 0; lDir<6; lDir++) {
						if (lDir > 0 || y != (lMaze.height - 1)) {
							if (!lMaze.get (x, y, z).links [lDir].broken) {
								GameObject lWall = DropWall (
									fDirectionPrefabs [lDir],
									lWallParent.transform,
									lPos + lMaze.getDirectionVector (lDir) * (0.5f - mazeWallScale / 2),
									fDirectionScales [lDir]);
								lWall.name = "Wall_" + y.ToString () + "_" + x.ToString () + "_" + z.ToString () + "_" + lDir.ToString ();
							}
						}
					}
					if (mazeMarkerPrefab) {
						GameObject lMarker = Instantiate (mazeMarkerPrefab, lPos, Quaternion.identity) as GameObject;
						lMarker.transform.SetParent (lMarkerParent.transform, false);
						lMarker.name = "M_" + y.ToString () + "_" + x.ToString () + "_" + z.ToString ();
					}
					if (score1Prefab) {
						GameObject lScore = Instantiate (score1Prefab, lPos + Vector3.down * 0.25f, Quaternion.identity) as GameObject;
						lScore.transform.SetParent (lScoreParent.transform, false);
						lScore.name = "S_" + y.ToString () + "_" + x.ToString () + "_" + z.ToString ();
					}
				}
			}
		}
		fGoodScore = Mathf.RoundToInt (0.75f * lMaze.depth * lMaze.height * lMaze.width);
		Debug.Log ("Labyrinth created.");
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
		mainCamera.gameObject.SetActive (false);
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
}
