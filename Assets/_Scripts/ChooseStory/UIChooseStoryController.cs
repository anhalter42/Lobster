using UnityEngine;
using System.Collections;

public class UIChooseStoryController : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void ButtonBack()
	{
		AllLevels.Get().StartNewGame();
	}
}
