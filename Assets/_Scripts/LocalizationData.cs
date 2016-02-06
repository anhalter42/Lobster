using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LocalizationData : MonoBehaviour {

	public string m_LocalizationID;

	// Use this for initialization
	void Start () {
		if (!string.IsNullOrEmpty(m_LocalizationID)) {
			string lText = AllLevels.Get().local.GetText(m_LocalizationID);
			Text lLabel = GetComponent<Text>();
			if (lLabel) {
				lLabel.text = lText;
			}
		}
	}
}
