using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UI : MonoBehaviour
{
	public GameObject mazeWallPrefab;
	public GameObject mazeWallTopPrefab;
	public GameObject mazeWallBottomPrefab;
	public GameObject mazeMarkerPrefab;
	public InputField mazeUIWidth;
	public InputField mazeUIDepth;
	public InputField mazeUIHeight;
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
	}
	
	// Update is called once per frame
	void Update ()
	{
	
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
		fDirectionScales [Maze.DirectionTop] = new Vector3 (1, mazeWallScale, 1);
		fDirectionScales [Maze.DirectionBottom] = new Vector3 (1, mazeWallScale, 1);
		fDirectionScales [Maze.DirectionRight] = new Vector3 (mazeWallScale, 1, 1);
		fDirectionScales [Maze.DirectionLeft] = new Vector3 (mazeWallScale, 1, 1);
		fDirectionScales [Maze.DirectionForward] = new Vector3 (1, 1, mazeWallScale);
		fDirectionScales [Maze.DirectionBackward] = new Vector3 (1, 1, mazeWallScale);
		fDirectionPrefabs [Maze.DirectionTop] = mazeWallTopPrefab ? mazeWallTopPrefab : mazeWallPrefab;
		fDirectionPrefabs [Maze.DirectionBottom] = mazeWallBottomPrefab ? mazeWallBottomPrefab : mazeWallPrefab;
		fDirectionPrefabs [Maze.DirectionRight] = mazeWallPrefab;
		fDirectionPrefabs [Maze.DirectionLeft] = mazeWallPrefab;
		fDirectionPrefabs [Maze.DirectionForward] = mazeWallPrefab;
		fDirectionPrefabs [Maze.DirectionBackward] = mazeWallPrefab;
		if (aParent) {
			for (int i = aParent.transform.childCount - 1; i >= 0; i--) {
				DestroyObject (aParent.transform.GetChild (i).gameObject);
			}
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
									aParent,
									lPos + lMaze.getDirectionVector (lDir) * ( 0.5f - mazeWallScale / 2 ),
									fDirectionScales [lDir]);
								lWall.name = "Wall_" + y.ToString () + "_" + x.ToString () + "_" + z.ToString () + "_" + lDir.ToString ();
							}
						}
					}
					if (mazeMarkerPrefab) {
						GameObject lMarker = Instantiate (mazeMarkerPrefab, lPos, Quaternion.identity) as GameObject;
						if (aParent) {
							lMarker.transform.SetParent (aParent, false);
						}
						lMarker.name = "M_" + y.ToString () + "_" + x.ToString () + "_" + z.ToString ();
					}
				}
			}
		}
		Debug.Log ("Labyrinth created.");
	}
}
