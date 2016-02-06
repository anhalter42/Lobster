using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Inventory
{
	public enum ItemMode
	{
		Level,
		Global,
		Story
	}

	[System.Serializable]
	public class Item
	{
		public string Name;
		public bool VisibleInUI = true;
		public ItemMode Mode = ItemMode.Level;
		public GameObject Prefab;
		public AudioClip AudioGet;
		public AudioClip AudioUse;
		public AudioClip AudioDrop;

		public void ReadLine (string aLine)
		{
			string[] lParts = aLine.Split (new char[] { '\t' }, System.StringSplitOptions.RemoveEmptyEntries);
			if (lParts.Length > 0) {
				Name = lParts [0];
			}
			if (lParts.Length > 1) {
				string[] lValues = lParts [1].Split (new char[] { ';' });
				if (lValues.Length > 0) {
					VisibleInUI = bool.Parse (lValues [0]);
				}
				if (lValues.Length > 1) {
					Mode = (ItemMode)System.Enum.Parse (typeof(ItemMode), lValues [1]);
				}
				if (lValues.Length > 2) {
					if (!string.IsNullOrEmpty (lValues [2])) {
						Prefab = AllLevels.LoadResource<GameObject> (lValues [2], "Inventory");
					}
				}
				if (lValues.Length > 3) {
					if (!string.IsNullOrEmpty (lValues [3])) {
						AudioGet = AllLevels.LoadResource<AudioClip> (lValues [3], "Audio");
					}
				}
				if (lValues.Length > 4) {
					if (!string.IsNullOrEmpty (lValues [4])) {
						AudioUse = AllLevels.LoadResource<AudioClip> (lValues [4], "Audio");
					}
				}
				if (lValues.Length > 5) {
					if (!string.IsNullOrEmpty (lValues [5])) {
						AudioDrop = AllLevels.LoadResource<AudioClip> (lValues [5], "Audio");
					}
				}
			}
		}
	}

	public Dictionary<string, Item> Items = new Dictionary<string, Item> ();

	public void ReadInventory (string aText)
	{
		string[] lLines = aText.Split (new string[] { "\n" }, System.StringSplitOptions.RemoveEmptyEntries);
		foreach (string lLine in lLines) {
			string lNewLine = lLine.Replace ("\r", "").Trim ();
			if (!lNewLine.StartsWith ("#") && !lNewLine.StartsWith ("/") && !lNewLine.StartsWith (";") && lNewLine.Contains ("\t")) {
				Item lItem = new Item ();
				lItem.ReadLine (lNewLine);
				if (!string.IsNullOrEmpty (lItem.Name)) {
					Items.Add (lItem.Name, lItem);
				}
			}
		}
	}

	public Item Get(string aType)
	{
		if (Items.ContainsKey(aType)) {
			return Items[aType];
		} else {
			Debug.Log(string.Format("Invenory '{0}' not defined!", aType));
			return new Item() { Name = aType, VisibleInUI = false, Mode = ItemMode.Level };
		}
	}

	public AudioClip GetAudioItemGet (string aType)
	{
		return Items.ContainsKey (aType) ? Items [aType].AudioGet : null;
	}

	public AudioClip GetAudioItemUse (string aType)
	{
		return Items.ContainsKey (aType) ? Items [aType].AudioUse : null;
	}

	public AudioClip GetAudioItemDrop (string aType)
	{
		return Items.ContainsKey (aType) ? Items [aType].AudioDrop : null;
	}
}
