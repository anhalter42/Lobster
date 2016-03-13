using UnityEngine;
using System.Collections;

public class PrefabConditions : MonoBehaviour
{
	public string[] ownTags;
	public string[] mustHaveTags;
	public string[] forbiddenTags;

	public int minAge = 0;
	public int maxAge = 999;

	public MazeCellComponent.MeshCheckBoundsMode checkMeshBounds = MazeCellComponent.MeshCheckBoundsMode.NoCheck;
}
