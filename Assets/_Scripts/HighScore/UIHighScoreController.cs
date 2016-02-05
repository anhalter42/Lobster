using UnityEngine;
using UnityEngine.UI;

//using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class UIHighScoreController : MonoBehaviour
{

	public Dropdown m_DDWorld;
	public Dropdown m_DDLevel;

	// Use this for initialization
	void Start ()
	{
		if (!m_DDWorld)
			m_DDWorld = GameObject.Find ("DropdownWorld").GetComponent<Dropdown> ();
		if (!m_DDLevel)
			m_DDLevel = GameObject.Find ("DropdownLevel").GetComponent<Dropdown> ();

		m_DDWorld.AddOptions (new List<string> (AllLevels.Get ().worlds));
		UpdateLevelDropdown ();
	}

	void UpdateLevelDropdown ()
	{
		m_DDLevel.ClearOptions ();
		List<string> lLs = new List<string> ();
		foreach (LevelSettings lS in AllLevels.Get().levelSettings) {
			if (string.Equals (lS.worldName, m_DDWorld.options [m_DDWorld.value].text, System.StringComparison.OrdinalIgnoreCase)) {
				lLs.Add (lS.levelName);
			}
		}
		m_DDLevel.AddOptions (lLs);
		m_DDLevel.value = 0;
	}

	public void WorldChanged ()
	{
		UpdateLevelDropdown ();
	}

	public void ButtonBack ()
	{
		AllLevels.Get ().StartNewGame ();
		//SceneManager.LoadScene("Start", LoadSceneMode.Single);
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Escape)) {
			ButtonBack ();
		}
	}
}
