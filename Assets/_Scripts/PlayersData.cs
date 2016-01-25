using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class PlayerLevel
{
	public string world;
	public string levelName;

	// completed with max scoreComplete
	public float time;
	public int score;
	public int timeBonusScore;
	public int scoreBonus;
	public int scoreComplete;

	// overrall min time
	public float minTime;
}

[Serializable]
public class Player
{
	public string name;
	public List<PlayerLevel> levels = new List<PlayerLevel>();

	public PlayerLevel GetLevel(string aWorld, string aName, bool aCreate = false)
	{
		foreach(PlayerLevel lL in levels) {
			if (lL.world == aWorld && lL.levelName == aName) {
				return lL;
			}
		}
		if (aCreate) {
			PlayerLevel lL = new PlayerLevel();
			lL.world = aWorld;
			lL.levelName = aName;
			levels.Add(lL);
			return lL;
		}
		return null;
	}
}

[Serializable]
public class PlayersData
{
	public List<Player> players = new List<Player>();

	public Player GetPlayer(string aName, bool aCreate = false)
	{
		foreach(Player lP in players) {
			if (lP.name == aName) {
				return lP;
			}
		}
		if (aCreate) {
			Player lP = new Player();
			lP.name = aName;
			players.Add(lP);
			return lP;
		}
		return null;
	}
}
