using UnityEngine;
using UnityEditor;
using System;
using System.Collections;

[Serializable]
public class GameObjectChance
{
	public int chance;
	public GameObject prefab;
}

public class GameObjectChanceComparer : IComparer {
	public int Compare(System.Object x, System.Object y)  {
		int a = 0, b = 0;
		if (x is GameObjectChance) a = ((GameObjectChance)x).chance;
		if (y is GameObjectChance) b = ((GameObjectChance)y).chance;
		//return a.CompareTo(b);
		return b.CompareTo(a); // higher values at first
	}
}

[Serializable]
public class CellDirectionObjects
{
	public GameObjectChance[] top;
	public GameObjectChance[] bottom;
	public GameObjectChance[] left;
	public GameObjectChance[] right;
	public GameObjectChance[] forward;
	public GameObjectChance[] backward;
	public GameObjectChance[] props;
	public GameObjectChance[] topProps;
	public GameObjectChance[] bottomProps;
	public GameObjectChance[] leftProps;
	public GameObjectChance[] rightProps;
	public GameObjectChance[] forwardProps;
	public GameObjectChance[] backwardProps;

	public GameObject GetOne (GameObjectChance[] aObjects, GameObject aDefault = null)
	{
		if (aObjects != null && aObjects.Length > 0) {
			Array.Sort(aObjects, new GameObjectChanceComparer());
			//Random.Range(0,aObjects.Length-1);
			for(int i = 0; i < aObjects.Length; i++) {
				if (UnityEngine.Random.Range(0,100) <= aObjects[i].chance) {
					return aObjects[i].prefab;
				}
			}
			return aDefault;
		} else {
			return aDefault;
		}
	}
}

[Serializable]
public class CellDescription : CellDirectionObjects
{
	public string name;
}
