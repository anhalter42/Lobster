using UnityEngine;
using UnityEngine.UI;

//using UnityEngine.SceneManagement;
using System.Collections;

public class UIPlayerProfileController : MonoBehaviour
{

	public InputField m_IFName;
	public InputField m_IFAge;
	public ScrollRect m_SVScores;
	public GameObject m_ScoreTemplate;
	public Vector3 m_ScoreOffset = new Vector3 ();

	public Player player;

	// Use this for initialization
	void Start ()
	{
		if (!m_IFName)
			m_IFName = GameObject.Find ("InputFieldName").GetComponent<InputField> ();
		if (!m_IFAge)
			m_IFAge = GameObject.Find ("InputFieldAge").GetComponent<InputField> ();
		if (!m_SVScores)
			m_SVScores = GameObject.Find ("ScrollViewScores").GetComponent<ScrollRect> ();
		player = AllLevels.Get ().currentPlayer;
		m_IFName.text = player.name;
		m_IFAge.text = player.age.ToString ();
		m_ScoreTemplate.SetActive (false);
		Vector3 lPos = m_ScoreOffset;
		player.levels.Sort (new PlayerLevelComparer ());
		foreach (PlayerLevel lL in player.levels) {
			GameObject lLine = Instantiate (m_ScoreTemplate) as GameObject;
			lLine.GetComponent<RectTransform> ().localPosition = lPos;
			lPos.y -= lLine.GetComponent<RectTransform> ().rect.height;
			lLine.SetActive (true);
			lLine.transform.SetParent (m_SVScores.content.gameObject.transform);
			lLine.transform.FindChild ("TextWorld").GetComponent<Text> ().text = lL.world;
			lLine.transform.FindChild ("TextLevel").GetComponent<Text> ().text = lL.level.ToString ();
			lLine.transform.FindChild ("TextLevelName").GetComponent<Text> ().text = lL.levelName;
			lLine.transform.FindChild ("TextScoreComplete").GetComponent<Text> ().text = lL.scoreComplete.ToString ();
			lLine.transform.FindChild ("TextMinTime").GetComponent<Text> ().text = Mathf.RoundToInt (lL.minTime).ToString ();
		}
	}

	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Escape) || Input.GetButtonDown ("Fire3")) {
			ButtonBack ();
		}
	}

	public void ButtonBack ()
	{
		AllLevels.Get ().StartNewGame ();
	}

	public void InputChanged ()
	{
		player.age = int.Parse (m_IFAge.text);
		player.name = m_IFName.text;
		AllLevels.Get ().SetPlayerName (player.name);
	}
}
