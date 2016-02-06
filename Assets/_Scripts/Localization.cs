using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Localization
{
	public Dictionary<string,string> Texts = new Dictionary<string, string> ();

	public void Read ()
	{
		string lLangu = Application.systemLanguage.ToString();//System.Globalization.CultureInfo.InstalledUICulture.Name;
		string lName = string.Format ("Text.{0}.txt", lLangu);
		if (System.IO.File.Exists (lName)) {
			string lLines = System.IO.File.ReadAllText (lName);
			Debug.Log (string.Format ("loading localization for language code {0} from '{1}'.", lLangu, lName));
			Read (lLines);
		} else {
			TextAsset lText = Resources.Load<TextAsset> ("Text." + lLangu);
			if (lText) {
				Debug.Log (string.Format ("loading localization for language code {0}.", lLangu));
				Read (lText.text);
			} else {
				Debug.Log (string.Format ("localization for language code {0} not found.", lLangu));
			}
		}
	}

	public void Read (string aText)
	{
		string[] lLines = aText.Split (new string[] { "\n" }, System.StringSplitOptions.RemoveEmptyEntries);
		foreach (string lLine in lLines) {
			string lNewLine = lLine.Replace ("\r", "").Trim ();
			if (!lNewLine.StartsWith ("#") && !lNewLine.StartsWith (";") && !lNewLine.StartsWith ("/")) {
				string[] lParts = lLine.Split (new char[] { '=' });
				if (lParts.Length > 1) {
					Texts.Add (lParts [0], lParts [1]);
				}
			}
		}
	}

	public string GetText (string aKey)
	{
		if (Texts.ContainsKey (aKey)) {
			return Texts [aKey];
		} else {
			Debug.Log(string.Format("Text '{0}' not found!",aKey));
			return aKey;
		}
	}
}
