using UnityEngine;
using System.Collections;

public class PrefabModifier : MonoBehaviour
{

	public Vector3 minScale = Vector3.one;
	public Vector3 maxScale = Vector3.one;
	public Vector3 minMove = Vector3.zero;
	public Vector3 maxMove = Vector3.zero;
	public Vector3 minRotation = Vector3.zero;
	public Vector3 maxRotation = Vector3.zero;

	public void ModifyPrefab (GameObject aPrefab)
	{
		Vector3 lScale = new Vector3 (
			                 Random.Range (minScale.x, maxScale.x),
			                 Random.Range (minScale.y, maxScale.y),
			                 Random.Range (minScale.z, maxScale.z));
		Vector3 lMove = new Vector3 (
			                Random.Range (minMove.x, maxMove.x),
			                Random.Range (minMove.y, maxMove.y),
			                Random.Range (minMove.z, maxMove.z));
		Vector3 lRotation = new Vector3 (
			                       Random.Range (minRotation.x, maxRotation.x),
			                       Random.Range (minRotation.y, maxRotation.y),
			                       Random.Range (minRotation.z, maxRotation.z));
		aPrefab.transform.localScale = new Vector3(
			aPrefab.transform.localScale.x * lScale.x,
			aPrefab.transform.localScale.y * lScale.y,
			aPrefab.transform.localScale.z * lScale.z
		);
		aPrefab.transform.localPosition = aPrefab.transform.localPosition + lMove;
		aPrefab.transform.Rotate(lRotation);
	}
}
