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
	public void ReadLine (string aLine, ref GameObjectChance[] aArray, string aFolder = null)
	{
		string[] aProps = aLine.Split (new string[] { ";" }, System.StringSplitOptions.None);
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
		if (aProps.Length > 0) {
			string[] lPrefabNames = aProps [0].Split (new char[] { ',' });
			foreach (string lName in lPrefabNames) {
				prefab = AllLevels.LoadResource<GameObject> (lName, "Prefabs", aFolder);
				if (prefab != null) {
					int lIndex = Find (aArray, prefab, wallNeeded);
					if (lIndex >= 0) {
						aArray [lIndex] = Clone ();
					} else {
						System.Array.Resize<GameObjectChance> (ref aArray, aArray.Length + 1);
						aArray [aArray.Length - 1] = Clone ();
					}
				}
			}
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

	public static int Find (GameObjectChance[] aList, GameObject aPrefab, bool aWithWall)
	{
		int lIndex = 0;
		foreach (GameObjectChance lG in aList) {
			if (lG.prefab == aPrefab && lG.wallNeeded == aWithWall) {
				return lIndex;
			}
			lIndex++;
		}
		return -1;
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

	public virtual CellDirectionObjects Create ()
	{
		return new CellDirectionObjects ();
	}

	public virtual CellDirectionObjects Clone (CellDirectionObjects aNew = null)
	{
		CellDirectionObjects lNew = aNew == null ? Create () : aNew;
		lNew.top = GameObjectChance.CloneArray (top);
		lNew.bottom = GameObjectChance.CloneArray (bottom);
		lNew.left = GameObjectChance.CloneArray (left);
		lNew.right = GameObjectChance.CloneArray (right);
		lNew.forward = GameObjectChance.CloneArray (forward);
		lNew.backward = GameObjectChance.CloneArray (backward);
		lNew.props = GameObjectChance.CloneArray (props);
		lNew.score = GameObjectChance.CloneArray (score);
		lNew.exit = GameObjectChance.CloneArray (exit);
		lNew.topProps = GameObjectChance.CloneArray (topProps);
		lNew.bottomProps = GameObjectChance.CloneArray (bottomProps);
		lNew.leftProps = GameObjectChance.CloneArray (leftProps);
		lNew.rightProps = GameObjectChance.CloneArray (rightProps);
		lNew.forwardProps = GameObjectChance.CloneArray (forwardProps);
		lNew.backwardProps = GameObjectChance.CloneArray (backwardProps);
		lNew.wayPoints = GameObjectChance.CloneArray (wayPoints);
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

	public bool ReadGameObjectChance (string aLine, ref GameObjectChance[] aGOC, string aName, string aFolder = null)
	{
		if (aLine.StartsWith (aName + "\t")) {
			string[] lArgs = aLine.Split (new string[] { "\t" }, System.StringSplitOptions.RemoveEmptyEntries);
			if (lArgs.Length > 1) {
				GameObjectChance lGOC = new GameObjectChance ();
				lGOC.ReadLine (lArgs [1], ref aGOC, aFolder);
			}
			return true;
		} else {
			return false;
		}
	}

	public virtual void ReadLine (string aLine, string aFolder = null)
	{
		if (!ReadGameObjectChance (aLine, ref top, "top", aFolder))
		if (!ReadGameObjectChance (aLine, ref bottom, "bottom", aFolder))
		if (!ReadGameObjectChance (aLine, ref left, "left", aFolder))
		if (!ReadGameObjectChance (aLine, ref right, "right", aFolder))
		if (!ReadGameObjectChance (aLine, ref forward, "forward", aFolder))
		if (!ReadGameObjectChance (aLine, ref backward, "backward", aFolder))
		if (!ReadGameObjectChance (aLine, ref props, "props", aFolder))
		if (!ReadGameObjectChance (aLine, ref score, "score", aFolder))
		if (!ReadGameObjectChance (aLine, ref exit, "exit", aFolder))
		if (!ReadGameObjectChance (aLine, ref topProps, "topProps", aFolder))
		if (!ReadGameObjectChance (aLine, ref bottomProps, "bottomProps", aFolder))
		if (!ReadGameObjectChance (aLine, ref leftProps, "leftProps", aFolder))
		if (!ReadGameObjectChance (aLine, ref rightProps, "rightProps", aFolder))
		if (!ReadGameObjectChance (aLine, ref forwardProps, "forwardProps", aFolder))
		if (!ReadGameObjectChance (aLine, ref backwardProps, "backwardProps", aFolder))
			ReadGameObjectChance (aLine, ref wayPoints, "wayPoints", aFolder);
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

	public class AudioItem
	{
		public string type = string.Empty;
		public AudioClip audioGet;
		public AudioClip audioUse;
		public AudioClip audioDrop;

		public void ReadLine (string aLine, string aFolder)
		{
			string[] aProps = aLine.Split (new string[] { ";" }, System.StringSplitOptions.None);
			if (aProps.Length > 1) {
				type = aProps [0];
				audioGet = AllLevels.LoadResource<AudioClip> (aProps [1], "Audio", aFolder);
				audioUse = audioGet;
				audioDrop = audioGet;
			}
			if (aProps.Length > 2) {
				audioUse = AllLevels.LoadResource<AudioClip> (aProps [2], "Audio", aFolder);
			}
			if (aProps.Length > 3) {
				audioDrop = AllLevels.LoadResource<AudioClip> (aProps [3], "Audio", aFolder);
			}
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
	public AudioClip audioBackgroundLevelStart;
	public AudioClip audioBackgroundLevelExitOpen;
	public Vector3 prefabSize = new Vector3 (1f, 1f, 1f);

	public AudioItem[] audioItems = { };
	protected System.Collections.Generic.Dictionary<string, AudioItem> fAudioItems;

	public override CellDirectionObjects Create ()
	{
		return new CellDescription ();
	}

	public override CellDirectionObjects Clone (CellDirectionObjects aNew)
	{
		CellDirectionObjects lCD = base.Clone (aNew);
		if (lCD is CellDescription) {
			CellDescription lNew = (CellDescription)lCD;
			lNew.name = name;
			lNew.worldName = worldName;
			lNew.audioScore = new AudioScore[audioScore.Length];
			System.Array.Copy (audioScore, lNew.audioScore, lNew.audioScore.Length);
			lNew.audioScoreReached = audioScoreReached;
			lNew.audioLiveLost = audioLiveLost;
			lNew.audioLiveAdded = audioLiveAdded;
			lNew.audioDamageSmall = audioDamageSmall;
			lNew.audioDamageMedium = audioDamageMedium;
			lNew.audioDamageBig = audioDamageBig;
			lNew.audioGameOver = audioGameOver;
			lNew.audioHealthSmall = audioHealthSmall;
			lNew.audioHealthMedium = audioHealthMedium;
			lNew.audioHealthBig = audioHealthBig;
			lNew.audioBackgroundPause = audioBackgroundPause;
			lNew.audioBackgroundMusic = audioBackgroundMusic;
			lNew.audioBackgroundLevelEnd = audioBackgroundLevelEnd;
			lNew.audioBackgroundLevelStart = audioBackgroundLevelStart;
			lNew.audioBackgroundLevelExitOpen = audioBackgroundLevelExitOpen;
			lNew.audioItems = new AudioItem[audioItems.Length];
			System.Array.Copy (audioItems, lNew.audioItems, lNew.audioItems.Length);
		}
		return lCD;
	}

	public AudioClip GetAudioItemGet (string aType)
	{
		return fAudioItems.ContainsKey (aType) ? fAudioItems [aType].audioGet : null;
	}

	public AudioClip GetAudioItemUse (string aType)
	{
		return fAudioItems.ContainsKey (aType) ? fAudioItems [aType].audioUse : null;
	}

	public AudioClip GetAudioItemDrop (string aType)
	{
		return fAudioItems.ContainsKey (aType) ? fAudioItems [aType].audioDrop : null;
	}

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

	public AudioItem[] ReadAudioItem (string aLine, AudioItem[] aSrc, string aName, string aFolder)
	{
		if (aLine.StartsWith (aName + "\t")) {
			string[] lArgs = aLine.Split (new string[] { "\t" }, System.StringSplitOptions.RemoveEmptyEntries);
			if (lArgs.Length > 1) {
				AudioItem lAI = new AudioItem ();
				lAI.ReadLine (lArgs [1], aFolder);
				if (lAI.audioGet != null) {
					ArrayList lList = new ArrayList (aSrc);
					lList.Add (lAI);
					return lList.ToArray (typeof(AudioItem)) as AudioItem[];
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
	/*
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
	*/

	public override void ReadLine (string aLine, string aFolder)
	{
		base.ReadLine (aLine, aFolder);
		//worldName = ReadString (aLine, aFolder, "world", aFolder);
		audioScore = ReadAudioScore (aLine, audioScore, "audioScore", aFolder);
		audioItems = ReadAudioItem (aLine, audioItems, "audioItem", aFolder);
		if (!LevelSettings.ReadString (ref worldName, aLine, "world"))
		if (!LevelSettings.ReadVector3 (ref prefabSize, aLine, "prefabSize"))
		if (!LevelSettings.ReadAudioClip (ref audioLiveLost, aLine, "audioLiveLost", aFolder))
		if (!LevelSettings.ReadAudioClip (ref audioLiveAdded, aLine, "audioLiveAdded", aFolder))
		if (!LevelSettings.ReadAudioClip (ref audioDamageSmall, aLine, "audioDamageSmall", aFolder))
		if (!LevelSettings.ReadAudioClip (ref audioDamageMedium, aLine, "audioDamageMedium", aFolder))
		if (!LevelSettings.ReadAudioClip (ref audioDamageBig, aLine, "audioDamageBig", aFolder))
		if (!LevelSettings.ReadAudioClip (ref audioScoreReached, aLine, "audioScoreReached", aFolder))
		if (!LevelSettings.ReadAudioClip (ref audioGameOver, aLine, "audioGameOver", aFolder))
		if (!LevelSettings.ReadAudioClip (ref audioHealthSmall, aLine, "audioHealthSmall", aFolder))
		if (!LevelSettings.ReadAudioClip (ref audioHealthMedium, aLine, "audioHealthMedium", aFolder))
		if (!LevelSettings.ReadAudioClip (ref audioHealthBig, aLine, "audioHealthBig", aFolder))
		if (!LevelSettings.ReadAudioClip (ref audioBackgroundPause, aLine, "audioBackgroundPause", aFolder))
		if (!LevelSettings.ReadAudioClip (ref audioBackgroundMusic, aLine, "audioBackgroundMusic", aFolder))
		if (!LevelSettings.ReadAudioClip (ref audioBackgroundLevelEnd, aLine, "audioBackgroundLevelEnd", aFolder))
		if (!LevelSettings.ReadAudioClip (ref audioBackgroundLevelStart, aLine, "audioBackgroundLevelStart", aFolder))
			LevelSettings.ReadAudioClip (ref audioBackgroundLevelExitOpen, aLine, "audioBackgroundLevelExitOpen", aFolder);
		/*
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
		audioBackgroundLevelStart = ReadAudioClip (aLine, audioBackgroundMusic, "audioBackgroundLevelStart", aFolder);
		audioBackgroundLevelExitOpen = ReadAudioClip (aLine, audioBackgroundLevelEnd, "audioBackgroundLevelExitOpen", aFolder);
		*/
	}

	public void FinishedReading ()
	{
		Array.Sort (audioScore, new AudioScoreComparer ());
		if (string.IsNullOrEmpty (worldName)) {
			worldName = name;
		}
		fAudioItems = new System.Collections.Generic.Dictionary<string, AudioItem> ();
		foreach (AudioItem lItem in audioItems) {
			fAudioItems.Add (lItem.type, lItem);
		}
	}
}
