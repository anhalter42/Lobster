using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MazeCellComponent : MonoBehaviour
{

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
				if (tags.ContainsKey (aTags [i].Trim().ToUpper())) {
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
				if (!tags.ContainsKey (aTags [i].Trim().ToUpper())) {
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
		return tags.ContainsKey (aTag.ToUpper());
	}

	public void SetTags (string[] aTags)
	{
		for (int i = 0; i < aTags.Length; i++) {
			SetTag (aTags [i]);
		}
	}

	public void SetTag (string aTag)
	{
		string lTag = aTag.Trim().ToUpper();
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

}
