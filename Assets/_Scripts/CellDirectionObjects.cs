﻿using UnityEngine;
using System;
using System.Collections;

[Serializable]
public class GameObjectChance
{
	public int chance = 100;
	// 100% -> immer
	public int score = 0;
	public int live = 0;
	public bool wallNeeded = true;
	public GameObject prefab;

	//prefabname;chance;wallNeeded;score;live;
	public void ReadLine (string aLine)
	{
		string[] aProps = aLine.Split (new string[] { ";" }, System.StringSplitOptions.None);
		if (aProps.Length > 0) {
			prefab = Resources.Load (aProps [0]) as GameObject;
			if (!prefab) {
				prefab = Resources.Load ("Prefabs/" + aProps [0]) as GameObject;
			}
			if (prefab == null) {
				Debug.Log ("Could not find prefab '" + aProps [0] + "'!");
			}
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

	public CellDirectionObjects Clone (CellDirectionObjects aNew)
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

	public GameObjectChance[] ReadGameObjectChance (string aLine, GameObjectChance[] aGOC, string aName)
	{
		if (aLine.StartsWith (aName + "\t")) {
			string[] lArgs = aLine.Split (new string[] { "\t" }, System.StringSplitOptions.RemoveEmptyEntries);
			if (lArgs.Length > 1) {
				GameObjectChance lGOC = new GameObjectChance ();
				lGOC.ReadLine (lArgs [1]);
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

	public void ReadLine (string aLine)
	{
		top = ReadGameObjectChance (aLine, top, "top");
		bottom = ReadGameObjectChance (aLine, bottom, "bottom");
		left = ReadGameObjectChance (aLine, left, "left");
		right = ReadGameObjectChance (aLine, right, "right");
		forward = ReadGameObjectChance (aLine, forward, "forward");
		backward = ReadGameObjectChance (aLine, backward, "backward");
		props = ReadGameObjectChance (aLine, props, "props");
		score = ReadGameObjectChance (aLine, score, "score");
		exit = ReadGameObjectChance (aLine, exit, "exit");
		topProps = ReadGameObjectChance (aLine, topProps, "topProps");
		bottomProps = ReadGameObjectChance (aLine, bottomProps, "bottomProps");
		leftProps = ReadGameObjectChance (aLine, leftProps, "leftProps");
		rightProps = ReadGameObjectChance (aLine, rightProps, "rightProps");
		forwardProps = ReadGameObjectChance (aLine, forwardProps, "forwardProps");
		backwardProps = ReadGameObjectChance (aLine, backwardProps, "backwardProps");
		wayPoints = ReadGameObjectChance (aLine, wayPoints, "wayPoints");
	}
}

[Serializable]
public class CellDescription : CellDirectionObjects
{
	public string name;
}