using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditor.UI;

[CustomEditor (typeof(MazeCellComponent))]
public class MazeCellComponentEditor : Editor
{
	public override void OnInspectorGUI ()
	{
		MazeCellComponent lCell = (MazeCellComponent)target;
		MazeCellComponent.GizmosOptions lOpt = MazeCellComponent.getGizmosOptions ();
		EditorGUILayout.LabelField ("Maze Cell Component", "For Debugging");
		lOpt.drawCellCube = EditorGUILayout.Toggle ("Show Cell Cube", lOpt.drawCellCube);
		lOpt.drawBounds = EditorGUILayout.Toggle ("Show Bounds", lOpt.drawBounds);
		lOpt.drawBoundsWired = EditorGUILayout.Toggle ("Show Bounds Wired", lOpt.drawBoundsWired);
		lOpt.checkIntersection = EditorGUILayout.Toggle ("Check Intersections", lOpt.checkIntersection);
		if (GUILayout.Button ("Update Bounds")) {
			lCell.UpdateBounds ();
		}

		// Show default inspector property editor
		DrawDefaultInspector ();
		MazeCellComponent.setGizmosOptions (lOpt);
	}
}
