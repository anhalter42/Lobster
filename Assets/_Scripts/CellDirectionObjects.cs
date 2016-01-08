using UnityEngine;
using System;
using System.Collections;

[Serializable]
public class GameObjectChance
{
	public int chance = 100;
	// 100% -> immer
	public int score = 0;
	public int live = 0;
	public float health = 0f;
	public bool wallNeeded = true;
	public GameObject prefab;

	//prefabname;chance;wallNeeded;score;live;
	public void ReadLine (string aLine, string aFolder = null)
	{
		string[] aProps = aLine.Split (new string[] { ";" }, System.StringSplitOptions.None);
		if (aProps.Length > 0) {
			prefab = AllLevels.LoadResource<GameObject> (aProps [0], "Prefabs", aFolder);
		}
		if (aProps.Length > 1) {
			chance = int.Parse (aProps [1]);
		}
		if (aProps.Length > 2) {
			wallNeeded = Convert.ToBoolean (aProps [2]);
		}
		if (aProps.Length > 3) {
			score = int.Parse (aProps [3]);
		}
		if (aProps.Length > 4) {
			live = int.Parse (aProps [4]);
		}
		if (aProps.Length > 5) {
			health = float.Parse (aProps [5]);
		}
	}

	public GameObjectChance Clone (GameObjectChance aNew = null)
	{
		GameObjectChance lNew = aNew == null ? new GameObjectChance () : aNew;
		lNew.prefab = prefab;
		lNew.chance = chance;
		lNew.wallNeeded = wallNeeded;
		lNew.score = score;
		lNew.live = live;
		return lNew;
	}

	public static GameObjectChance[] CloneArray (GameObjectChance[] aNew)
	{
		GameObjectChance[] lNew = new GameObjectChance[aNew.Length];
		for (int i = 0; i < aNew.Length; i++) {
			lNew [i] = aNew [i].Clone ();
		}
		return lNew;
	}
}

public class GameObjectChanceComparer : IComparer
{
	public int Compare (System.Object x, System.Object y)
	{
		int a = 0, b = 0;
		if (x is GameObjectChance)
			a = ((GameObjectChance)x).chance;
		if (y is GameObjectChance)
			b = ((GameObjectChance)y).chance;
		//return a.CompareTo(b);
		return b.CompareTo (a); // higher values at first
	}
}

[Serializable]
public class CellDirectionObjects
{
	public GameObjectChance[] top = { };
	public GameObjectChance[] bottom = { };
	public GameObjectChance[] left = { };
	public GameObjectChance[] right = { };
	public GameObjectChance[] forward = { };
	public GameObjectChance[] backward = { };
	public GameObjectChance[] props = { };
	public GameObjectChance[] score = { };
	public GameObjectChance[] exit = { };
	public GameObjectChance[] topProps = { };
	public GameObjectChance[] bottomProps = { };
	public GameObjectChance[] leftProps = { };
	public GameObjectChance[] rightProps = { };
	public GameObjectChance[] forwardProps = { };
	public GameObjectChance[] backwardProps = { };
	public GameObjectChance[] wayPoints = { };

	public virtual CellDirectionObjects Clone (CellDirectionObjects aNew)
	{
		CellDirectionObjects lNew = new CellDirectionObjects ();
		lNew.top = GameObjectChance.CloneArray (aNew.top);
		lNew.bottom = GameObjectChance.CloneArray (aNew.bottom);
		lNew.left = GameObjectChance.CloneArray (aNew.left);
		lNew.right = GameObjectChance.CloneArray (aNew.right);
		lNew.forward = GameObjectChance.CloneArray (aNew.forward);
		lNew.backward = GameObjectChance.CloneArray (aNew.backward);
		lNew.props = GameObjectChance.CloneArray (aNew.props);
		lNew.score = GameObjectChance.CloneArray (aNew.score);
		lNew.exit = GameObjectChance.CloneArray (aNew.exit);
		lNew.topProps = GameObjectChance.CloneArray (aNew.topProps);
		lNew.bottomProps = GameObjectChance.CloneArray (aNew.bottomProps);
		lNew.leftProps = GameObjectChance.CloneArray (aNew.leftProps);
		lNew.rightProps = GameObjectChance.CloneArray (aNew.rightProps);
		lNew.forwardProps = GameObjectChance.CloneArray (aNew.forwardProps);
		lNew.backwardProps = GameObjectChance.CloneArray (aNew.backwardProps);
		lNew.wayPoints = GameObjectChance.CloneArray (aNew.wayPoints);
		return lNew;
	}

	public GameObject[] GetSome (GameObjectChance[] aObjects, bool aWithWall = true, GameObject[] aDefault = null)
	{
		ArrayList lList = new ArrayList ();
		if (aObjects != null && aObjects.Length > 0) {
			Array.Sort (aObjects, new GameObjectChanceComparer ());
			for (int i = 0; i < aObjects.Length; i++) {
				if ((aObjects [i].wallNeeded == aWithWall)
				    && UnityEngine.Random.Range (0, 100) <= aObjects [i].chance) {
					lList.Add (aObjects [i].prefab);
				}
			}
		}
		if (aDefault != null) {
			for (int i = 0; i < aDefault.Length; i++) {
				lList.Add (aDefault [i]);
			}
		}
		return lList.ToArray (typeof(GameObject)) as GameObject[];
	}

	public GameObject GetOne (GameObjectChance[] aObjects, GameObject aDefault = null)
	{
		if (aObjects != null && aObjects.Length > 0) {
			Array.Sort (aObjects, new GameObjectChanceComparer ());
			for (int i = 0; i < aObjects.Length; i++) {
				if (UnityEngine.Random.Range (0, 100) <= aObjects [i].chance) {
					return aObjects [i].prefab;
				}
			}
			return aDefault == null ? aObjects [0].prefab : aDefault;
		}
		return aDefault;
	}

	public GameObject GetOneForScore (GameObjectChance[] aObjects, int aScore)
	{
		if (aObjects != null && aObjects.Length > 0) {
			Array.Sort (aObjects, new GameObjectChanceComparer ());
			int lScoreObjects = 0;
			for (int i = 0; i < aObjects.Length; i++) {
				if (aObjects [i].score == aScore) {
					lScoreObjects++;
				}
			}
			if (lScoreObjects > 0) {
				GameObject lPrefabDefault = null;
				for (int i = 0; i < aObjects.Length; i++) {
					if (lPrefabDefault == null && aObjects [i].score == aScore) {
						lPrefabDefault = aObjects [i].prefab;
					}
					if (aObjects [i].score == aScore && UnityEngine.Random.Range (0, 100) <= aObjects [i].chance) {
						return aObjects [i].prefab;
					}
				}
				return lPrefabDefault;
			}
			return null;
		}
		return null;
	}

	public GameObjectChance[] ReadGameObjectChance (string aLine, GameObjectChance[] aGOC, string aName, string aFolder = null)
	{
		if (aLine.StartsWith (aName + "\t")) {
			string[] lArgs = aLine.Split (new string[] { "\t" }, System.StringSplitOptions.RemoveEmptyEntries);
			if (lArgs.Length > 1) {
				GameObjectChance lGOC = new GameObjectChance ();
				lGOC.ReadLine (lArgs [1], aFolder);
				if (lGOC.prefab != null) {
					ArrayList lList = new ArrayList (aGOC);
					lList.Add (lGOC);
					return lList.ToArray (typeof(GameObjectChance)) as GameObjectChance[];
				} else {
					return aGOC;
				}
			} else {
				return aGOC;
			}
		} else {
			return aGOC;
		}
	}

	public virtual void ReadLine (string aLine, string aFolder = null)
	{
		top = ReadGameObjectChance (aLine, top, "top", aFolder);
		bottom = ReadGameObjectChance (aLine, bottom, "bottom", aFolder);
		left = ReadGameObjectChance (aLine, left, "left", aFolder);
		right = ReadGameObjectChance (aLine, right, "right", aFolder);
		forward = ReadGameObjectChance (aLine, forward, "forward", aFolder);
		backward = ReadGameObjectChance (aLine, backward, "backward", aFolder);
		props = ReadGameObjectChance (aLine, props, "props", aFolder);
		score = ReadGameObjectChance (aLine, score, "score", aFolder);
		exit = ReadGameObjectChance (aLine, exit, "exit", aFolder);
		topProps = ReadGameObjectChance (aLine, topProps, "topProps", aFolder);
		bottomProps = ReadGameObjectChance (aLine, bottomProps, "bottomProps", aFolder);
		leftProps = ReadGameObjectChance (aLine, leftProps, "leftProps", aFolder);
		rightProps = ReadGameObjectChance (aLine, rightProps, "rightProps", aFolder);
		forwardProps = ReadGameObjectChance (aLine, forwardProps, "forwardProps", aFolder);
		backwardProps = ReadGameObjectChance (aLine, backwardProps, "backwardProps", aFolder);
		wayPoints = ReadGameObjectChance (aLine, wayPoints, "wayPoints", aFolder);
	}
}

[Serializable]
public class CellDescription : CellDirectionObjects
{
	public class AudioScore
	{
		public AudioClip audio;
		public int score = 1;

		public void ReadLine (string aLine, string aFolder)
		{
			string[] aProps = aLine.Split (new string[] { ";" }, System.StringSplitOptions.None);
			if (aProps.Length > 0) {
				audio = AllLevels.LoadResource<AudioClip> (aProps [0], "Audio", aFolder);
			}
			if (aProps.Length > 1) {
				score = int.Parse (aProps [1]);
			}
		}
	}

	public class AudioScoreComparer : IComparer
	{
		public int Compare (System.Object x, System.Object y)
		{
			int a = 0, b = 0;
			if (x is AudioScore)
				a = ((AudioScore)x).score;
			if (y is AudioScore)
				b = ((AudioScore)y).score;
			return b.CompareTo (a); // higher values at first
		}
	}

	public string name;
	public string worldName;
	public AudioScore[] audioScore = { };
	public AudioClip audioScoreReached;
	public AudioClip audioLiveLost;
	public AudioClip audioLiveAdded;
	public AudioClip audioDamageSmall;
	public AudioClip audioDamageMedium;
	public AudioClip audioDamageBig;
	public AudioClip audioGameOver;
	public AudioClip audioHealthSmall;
	public AudioClip audioHealthMedium;
	public AudioClip audioHealthBig;
	public AudioClip audioBackgroundPause;
	public AudioClip audioBackgroundMusic;
	public AudioClip audioBackgroundLevelEnd;

	public AudioScore[] ReadAudioScore (string aLine, AudioScore[] aSrc, string aName, string aFolder)
	{
		if (aLine.StartsWith (aName + "\t")) {
			string[] lArgs = aLine.Split (new string[] { "\t" }, System.StringSplitOptions.RemoveEmptyEntries);
			if (lArgs.Length > 1) {
				AudioScore lAS = new AudioScore ();
				lAS.ReadLine (lArgs [1], aFolder);
				if (lAS.audio != null) {
					ArrayList lList = new ArrayList (aSrc);
					lList.Add (lAS);
					return lList.ToArray (typeof(AudioScore)) as AudioScore[];
				} else {
					return aSrc;
				}
			} else {
				return aSrc;
			}
		} else {
			return aSrc;
		}
	}

	public static AudioClip ReadAudioClip (string aLine, AudioClip aSrc, string aName, string aFolder)
	{
		if (aLine.StartsWith (aName + "\t")) {
			string[] lArgs = aLine.Split (new string[] { "\t" }, System.StringSplitOptions.RemoveEmptyEntries);
			if (lArgs.Length > 1) {
				return AllLevels.LoadResource<AudioClip> (lArgs [1], "Audio", aFolder);
			} else {
				return aSrc;
			}
		} else {
			return aSrc;
		}
	}

	public string ReadString (string aLine, string aSrc, string aName, string aFolder)
	{
		if (aLine.StartsWith (aName + "\t")) {
			string[] lArgs = aLine.Split (new string[] { "\t" }, System.StringSplitOptions.RemoveEmptyEntries);
			if (lArgs.Length > 1) {
				return lArgs [1];
			} else {
				return aSrc;
			}
		} else {
			return aSrc;
		}
	}

	public override void ReadLine (string aLine, string aFolder)
	{
		base.ReadLine (aLine, aFolder);
		worldName = ReadString (aLine, aFolder, "world", aFolder);
		audioScore = ReadAudioScore (aLine, audioScore, "audioScore", aFolder);
		audioLiveLost = ReadAudioClip (aLine, audioLiveLost, "audioLiveLost", aFolder);
		audioLiveAdded = ReadAudioClip (aLine, audioLiveAdded, "audioLiveAdded", aFolder);
		audioDamageSmall = ReadAudioClip (aLine, audioDamageSmall, "audioDamageSmall", aFolder);
		audioDamageMedium = ReadAudioClip (aLine, audioDamageMedium, "audioDamageMedium", aFolder);
		audioDamageBig = ReadAudioClip (aLine, audioDamageBig, "audioDamageBig", aFolder);
		audioScoreReached = ReadAudioClip (aLine, audioScoreReached, "audioScoreReached", aFolder);
		audioGameOver = ReadAudioClip (aLine, audioGameOver, "audioGameOver", aFolder);
		audioHealthSmall = ReadAudioClip (aLine, audioHealthSmall, "audioHealthSmall", aFolder);
		audioHealthMedium = ReadAudioClip (aLine, audioHealthMedium, "audioHealthMedium", aFolder);
		audioHealthBig = ReadAudioClip (aLine, audioHealthBig, "audioHealthBig", aFolder);
		audioBackgroundPause = ReadAudioClip (aLine, audioBackgroundPause, "audioBackgroundPause", aFolder);
		audioBackgroundMusic = ReadAudioClip (aLine, audioBackgroundMusic, "audioBackgroundMusic", aFolder);
		audioBackgroundLevelEnd = ReadAudioClip (aLine, audioBackgroundLevelEnd, "audioBackgroundLevelEnd", aFolder);
	}

	public void FinishedReading ()
	{
		Array.Sort (audioScore, new AudioScoreComparer ());
		if (string.IsNullOrEmpty(worldName)) {
			worldName = name;
		}
	}
}
