using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MazeCellComponent : MonoBehaviour
{
	public enum MeshCheckBoundsMode
	{
		NoCheck,
		CheckComplete,
		CheckWithoutWalls
	}

	public struct GizmosOptions
	{
		public bool drawCellCube;
		public bool drawBounds;
		public bool drawBoundsWired;
		public bool checkIntersection;
	}

	[System.Serializable]
	public class PrefabBound
	{
		public GameObject prefab;
		public Bounds bounds;
	}

	public Maze.Cell cell;

	public GameObject[] walls = new GameObject[6];

	Dictionary<string,int> tags = new Dictionary<string, int> ();
	#if UNITY_EDITOR
	public string[] m_tags = { };
	#endif
		
	public bool ContainsSomeTags (string[] aTags, bool aDefault = false)
	{
		if (aTags.Length > 0) {
			for (int i = 0; i < aTags.Length; i++) {
				if (tags.ContainsKey (aTags [i].Trim ().ToUpper ())) {
					return true;
				}
			}
			return false;
		} else {
			return aDefault;
		}
	}

	public bool ContainsAllTags (string[] aTags)
	{
		if (aTags.Length > 0) {
			for (int i = 0; i < aTags.Length; i++) {
				if (!tags.ContainsKey (aTags [i].Trim ().ToUpper ())) {
					return false;
				}
			}
			return true;
		} else {
			return true;
		}
	}

	public bool ContainsTag (string aTag)
	{
		return tags.ContainsKey (aTag.ToUpper ());
	}

	public void SetTags (string[] aTags)
	{
		for (int i = 0; i < aTags.Length; i++) {
			SetTag (aTags [i]);
		}
	}

	public void SetTag (string aTag)
	{
		string lTag = aTag.Trim ().ToUpper ();
		if (tags.ContainsKey (lTag)) {
			tags [lTag]++;
		} else {
			tags.Add (lTag, 1);
		}
		#if UNITY_EDITOR
		m_tags = new string[tags.Count];
		int i = 0;
		foreach (string lKey in tags.Keys) {
			m_tags [i++] = string.Format ("{0}:{1}", lKey, tags [lKey]);
		}
		#endif
	}

	public PrefabBound[] bounds = { };

	protected void _GetTransformBounds (List<PrefabBound> aBounds, GameObject aPrefab, Transform aT)
	{
		if (aT.gameObject.activeInHierarchy) {
			int lC = aT.childCount;
			while (lC > 0) {
				lC--;
				_GetTransformBounds (aBounds, aPrefab, aT.GetChild (lC));
			}
			Mesh lMesh = aT.GetComponent<Mesh> ();
			if (lMesh == null) {
				MeshFilter lF = aT.GetComponent<MeshFilter> ();
				if (lF != null) {
					lMesh = lF.sharedMesh;
				}
			}
			if (lMesh != null) {
				PrefabBound lBound = new PrefabBound ();
				lBound.prefab = aPrefab;
				lBound.bounds.extents = aT.TransformVector (lMesh.bounds.extents);
				lBound.bounds.center = aT.position + aT.TransformVector (lMesh.bounds.center);
				aBounds.Add (lBound);
			}
		}
	}

	public List<PrefabBound> GetTransformBounds (GameObject aPrefab)
	{
		List<PrefabBound> lBounds = new List<PrefabBound> ();
		_GetTransformBounds (lBounds, aPrefab, aPrefab.transform);
		return lBounds;
	}

	public void PrefabInserted (GameObject aPrefab)
	{
		List<PrefabBound> lBounds = GetTransformBounds (aPrefab);
		if (lBounds.Count > 0) {
			int lIndex = bounds.Length;
			System.Array.Resize<PrefabBound> (ref bounds, bounds.Length + lBounds.Count);
			foreach (PrefabBound lBound in lBounds) {
				bounds [lIndex] = lBound;
				lIndex++;
			}
		}
	}

	protected GizmosOptions fGizmosOptions;

	public GizmosOptions getGizmosOptions ()
	{
		return fGizmosOptions;
	}

	public void setGizmosOptions (GizmosOptions aOptions)
	{
		fGizmosOptions = aOptions;
	}

	public void UpdateBounds ()
	{
		System.Array.Resize<PrefabBound> (ref bounds, 0);
		for (int lI = 0; lI < transform.childCount; lI++) {
			PrefabInserted (transform.GetChild (lI).gameObject);
		}
	}

	public bool IsWall (GameObject aPrefab)
	{
		foreach (GameObject lObj in walls) {
			if (lObj == aPrefab) {
				return true;
			}
		}
		return false;
	}

	public bool IntersectsWithBounds (GameObject aPrefab, MeshCheckBoundsMode aMode)
	{
		List<PrefabBound> lBounds = GetTransformBounds (aPrefab);
		foreach (PrefabBound lBound in lBounds) {
			foreach (PrefabBound lCBound in bounds) {
				if (lCBound.prefab != aPrefab
				    && (aMode == MeshCheckBoundsMode.CheckComplete || (aMode == MeshCheckBoundsMode.CheckWithoutWalls && !IsWall (lCBound.prefab)))
				    && lCBound.bounds.Intersects (lBound.bounds)) {
					return true;
				}
			}
		}
		return false;
	}

	public void OnDrawGizmosSelected ()
	{
		List<GameObject> lIntersects = new List<GameObject> ();
		if (fGizmosOptions.drawCellCube) {
			Gizmos.color = new Color (1f, 1f, 1f, 0.25f);
			Gizmos.DrawCube (transform.position, new Vector3 (1f, 1f, 1f));
		}
		if (fGizmosOptions.checkIntersection) {
			for (int lI = 0; lI < transform.childCount; lI++) {
				if (IntersectsWithBounds (transform.GetChild (lI).gameObject, MeshCheckBoundsMode.CheckWithoutWalls)) {
					lIntersects.Add (transform.GetChild (lI).gameObject);
				}
			}
		}
		if (fGizmosOptions.drawBounds) {
			foreach (PrefabBound lB in bounds) {
				bool lWrong = lIntersects.Contains (lB.prefab);
				if (!fGizmosOptions.drawBoundsWired) {
					Gizmos.color = lWrong ? new Color (1f, 0.1f, 0.1f, 0.5f) : new Color (1f, 0.75f, 0.5f, 0.5f);
					Gizmos.DrawCube (lB.bounds.center, lB.bounds.size);
				}
				Gizmos.color = lWrong ? new Color (1f, 0.1f, 0.1f, 1f) : new Color (1f, 0.75f, 0.5f, 1f);
				Gizmos.DrawWireCube (lB.bounds.center, lB.bounds.size);
			}
		}
	}

}
