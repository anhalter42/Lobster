using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MazeCellComponent : MonoBehaviour
{

	public Maze.Cell cell;

	public GameObject[] walls = new GameObject[6];

	public Dictionary<string,int> tags = new Dictionary<string, int> ();
	#if UNITY_EDITOR
	public string[] m_tags = { };
	#endif
		
	public bool ContainsTags (string[] aTags)
	{
		if (aTags.Length > 0) {
			for (int i = 0; i < aTags.Length; i++) {
				if (tags.ContainsKey (aTags [i])) {
					return true;
				}
			}
			return false;
		} else {
			return true;
		}
	}

	public bool ContainsAllTags (string[] aTags)
	{
		if (aTags.Length > 0) {
			for (int i = 0; i < aTags.Length; i++) {
				if (!tags.ContainsKey (aTags [i])) {
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
		return tags.ContainsKey (aTag);
	}

	public void SetTags (string[] aTags)
	{
		for (int i = 0; i < aTags.Length; i++) {
			SetTag (aTags [i]);
		}
	}

	public void SetTag (string aTag)
	{
		if (tags.ContainsKey (aTag)) {
			tags [aTag]++;
		} else {
			tags.Add (aTag, 1);
		}
		#if UNITY_EDITOR
		m_tags = new string[tags.Count];
		int i = 0;
		foreach (string lKey in tags.Keys) {
			m_tags [i++] = string.Format ("{0}:{1}", lKey, tags [lKey]);
		}
		#endif
	}

}
