using UnityEngine;

//using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIStartController : MonoBehaviour
{

	public Material material;

	public InputField m_NewPlayerName;
	public InputField m_NewPlayerAge;
	public RectTransform m_NewPlayerPanel;
	public Dropdown m_DropdownPlayer;
	public Dropdown m_DropdownLanguage;

	Texture2D texture;

	// Use this for initialization
	void Start ()
	{
		if (!m_NewPlayerPanel) {
			m_NewPlayerPanel = GameObject.Find ("PanelNewPlayer").GetComponent<RectTransform> ();
		}
		if (!m_NewPlayerName) {
			m_NewPlayerName = GameObject.Find ("InputFieldName").GetComponent<InputField> ();
		}
		if (!m_NewPlayerAge) {
			m_NewPlayerAge = GameObject.Find ("InputFieldAge").GetComponent<InputField> ();
		}

		if (!m_DropdownPlayer) {
			m_DropdownPlayer = GameObject.Find ("DropdownPlayer").GetComponent<Dropdown> ();
		}
		if (!m_DropdownLanguage) {
			m_DropdownLanguage = GameObject.Find ("DropdownLanguage").GetComponent<Dropdown> ();
		}
		DontDestroyOnLoad (GameObject.Find ("Master"));
		texture = new Texture2D (500, 500);
		if (material) {
			material.mainTexture = texture;
		}
		px = x = Random.Range (dx, (texture.width - dx) / dx);
		py = y = Random.Range (dy, (texture.height - dy) / dy);
		dxx = new int[4];
		dyy = new int[4];
		dxx [0] = -1;
		dyy [0] = 0;
		dxx [1] = +1;
		dyy [1] = 0;
		dxx [2] = 0;
		dyy [2] = -1;
		dxx [3] = 0;
		dyy [3] = +1;
		di = new int[16];
		di [0] = 0;
		di [1] = 2;
		di [2] = 1;
		di [3] = 0;
		di [4] = 3;
		di [5] = 1;
		di [6] = 3;
		di [7] = 2;
		di [8] = 0;
		di [9] = 1;
		di [10] = 3;
		di [11] = 0;
		di [12] = 1;
		di [13] = 2;
		di [14] = 3;
		di [15] = 2;
		m_NewPlayerPanel.gameObject.SetActive (false);
		UpdateLanguageDrowpdown ();
		UpdatePlayerDrowpdown ();
	}

	void UpdatePlayerDrowpdown ()
	{
		string lName = PlayerPrefs.GetString ("PlayerName", "Winston");
		int i = 0, lIndex = 0;
		m_DropdownPlayer.ClearOptions ();
		List<string> lPlayers = new List<string> ();
		foreach (Player lP in AllLevels.Get().data.players) {
			lPlayers.Add (lP.name);
			if (lP.name == lName) {
				lIndex = i;
			}
			i++;
		}
		m_DropdownPlayer.AddOptions (lPlayers);
		m_DropdownPlayer.value = lIndex;
	}

	void UpdateLanguageDrowpdown ()
	{
		string lName = PlayerPrefs.GetString ("Language", AllLevels.Get ().language.ToString ());
		int i = 0, lIndex = 0;
		m_DropdownLanguage.ClearOptions ();
		List<string> lLangus = new List<string> ();
		foreach (SystemLanguage lL in AllLevels.Get().supportedLanguages) {
			lLangus.Add (lL.ToString ());
			if (lL.ToString () == lName) {
				lIndex = i;
			}
			i++;
		}
		m_DropdownLanguage.AddOptions (lLangus);
		m_DropdownLanguage.value = lIndex;
	}

	int x, y;
	int px, py;
	int dx = 4;
	int dy = 4;
	float dc = 0.5f;
	int[] dxx;
	int[] dyy;
	int[] di;
	// Update is called once per frame

	void FillPixels (int x1, int y1, int x2, int y2, Color c)
	{
		int minx = Mathf.Min (x1 * dx, x2 * dx);
		int miny = Mathf.Min (y1 * dy, y2 * dy);
		int maxx = Mathf.Max (x1 * dx, x2 * dx);
		int maxy = Mathf.Max (y1 * dy, y2 * dy);
		for (int lx = minx; lx <= maxx; lx++) {
			for (int ly = miny; ly <= maxy; ly++) {
				texture.SetPixel (lx, ly, c);
//				for (int ldx = 0; ldx < dx; ldx++) {
//					for (int ldy = 0; ldy < dy; ldy++) {
//						texture.SetPixel (lx * dx + ldx, ly * dy + ldy, c);
//					}
//				}
			}
		}
	}

	void Update ()
	{
		Color c = texture.GetPixel (x * dx, y * dy);
		if (c.g >= dc) {
			c.g = 0.0f;
			c.r = 0.0f;
			c.b = 0.0f;
			c.a = 1.0f;
			//texture.SetPixel (x * dx, y * dy, c);
			FillPixels (px, py, x, y, c);
			int nx, ny;
			Color nc;
			bool lf = false;
			int ii = Random.Range (0, di.Length / 2);
			for (int i = 0; i < dxx.Length; i++) {
				nx = x + dxx [di [ii + i]];
				ny = y + dyy [di [ii + i]];
				if (nx > 1 && nx < texture.width - 1 && ny > 1 && ny < texture.height - 1) {
					nc = texture.GetPixel (nx * dx, ny * dy);
					if (nc.g >= dc) {
						px = x;
						py = y;
						x = nx;
						y = ny;
						lf = true;
						break;
					}
				}
			}
			if (!lf) {
				px = x = Random.Range (dx, (texture.width - dx) / dx);
				py = y = Random.Range (dy, (texture.height - dy) / dy);
			}
		} else {
			px = x = Random.Range (dx, (texture.width - dx) / dx);
			py = y = Random.Range (dy, (texture.height - dy) / dy);
		}
		texture.Apply (false);
	}

	public void ButtonStartStory ()
	{
		AllLevels.Get ().StartChooseStory ();
	}

	public void ButtonStartLevel ()
	{
		AllLevels.Get ().StartChooseLevel ();
	}

	public void ButtonNewPlayer ()
	{
		m_NewPlayerPanel.gameObject.SetActive (true);
		m_NewPlayerName.ActivateInputField ();
	}

	public void ButtonNewPlayerCancel ()
	{
		m_NewPlayerPanel.gameObject.SetActive (false);
	}

	public void ButtonNewPlayerOK ()
	{
		AllLevels.Get ().SetPlayerName (m_NewPlayerName.text);
		if (!int.TryParse (m_NewPlayerAge.text, out AllLevels.Get ().currentPlayer.age))
			AllLevels.Get ().currentPlayer.age = 0;
		UpdatePlayerDrowpdown ();
		m_NewPlayerPanel.gameObject.SetActive (false);
	}

	public void ButtonProfile ()
	{
		AllLevels.Get ().StartProfile ();
	}

	public void ButtonHighscore ()
	{
		AllLevels.Get ().StartHighscore ();
	}

	public void DropdownPlayerChanged ()
	{
		AllLevels.Get ().SetPlayerName (m_DropdownPlayer.options [m_DropdownPlayer.value].text);
	}

	public void DropdownLanguageChanged ()
	{
		SystemLanguage lLanguage = (SystemLanguage)System.Enum.Parse (typeof(SystemLanguage), m_DropdownLanguage.options [m_DropdownLanguage.value].text);
		AllLevels.Get ().SetLanguage (lLanguage);
	}
}
