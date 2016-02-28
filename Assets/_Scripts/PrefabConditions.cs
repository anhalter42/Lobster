using UnityEngine;
using System.Collections;

public class PrefabConditions : MonoBehaviour {

	public enum MeshCheckBoundsMode {
		NoCheck,
		CheckComplete,
		CheckWithoutWalls
	}

	public string[] ownTags;
	public string[] mustHaveTags;
	public string[] forbiddenTags;

	public int minAge = 0;
	public int maxAge = 999;

	public MeshCheckBoundsMode checkMeshBounds = MeshCheckBoundsMode.NoCheck;

}
