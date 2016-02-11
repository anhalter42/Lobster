using UnityEngine;
using System.Collections;

[System.Serializable]
public class Story
{
	[System.Serializable]
	public class Part
	{
		public string levelName;
		public string[] settings = { };

		public void ReadLine (string aLine)
		{
			System.Array.Resize<string> (ref settings, settings.Length + 1);
			settings [settings.Length - 1] = aLine;
		}
	}

	public string name;
	public Part[] parts = { };

	public void ReadLine (string aLine)
	{
		Part lPart = null;
		if (aLine.StartsWith ("*")) {
			lPart = new Part ();
			lPart.levelName = aLine.Substring (1).Trim ();
			System.Array.Resize<Part> (ref parts, parts.Length + 1);
			parts [parts.Length - 1] = lPart;
		} else if (lPart != null) {
			lPart.ReadLine (aLine);	
		}
	}

	public static Story[] Read (string aText)
	{
		Story[] lStories = { };
		Story lStory = null;
		string[] lLines = aText.Split (new string[] { "\n" }, System.StringSplitOptions.RemoveEmptyEntries);
		foreach (string lLine in lLines) {
			string lNewLine = lLine.Replace ("\r", "").Trim ();
			if (!lNewLine.StartsWith (";") && !lNewLine.StartsWith ("/")) {
				if (lNewLine.StartsWith ("#")) {
					lStory = new Story ();
					lStory.name = lNewLine.Substring (1).Trim ();
					System.Array.Resize<Story> (ref lStories, lStories.Length + 1);
					lStories [lStories.Length - 1] = lStory;
				} else if (lStory != null && !string.IsNullOrEmpty (lNewLine)) {
					lStory.ReadLine (lNewLine);
				}
			}
		}
		return lStories;
	}
}