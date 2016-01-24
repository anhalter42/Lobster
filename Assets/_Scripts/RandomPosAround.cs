using UnityEngine;
using System.Collections;

public class RandomPosAround : MonoBehaviour
{

	public Vector3 min;
	public Vector3 max;
	public float speed = 0.1f;

	Vector3 orgPos;
	Vector3 newPos;
	Vector3 nextPos;

	// Use this for initialization
	void Start ()
	{
		orgPos = transform.position;
		newPos = orgPos;
		nextPos = orgPos;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (nextPos != transform.position) {
			orgPos = transform.position;
		}
		if (nextPos == newPos) { // finished, calc next new Pos
			newPos = new Vector3 (orgPos.x + Random.Range (min.x, max.x), orgPos.y + Random.Range (min.y, max.y), orgPos.z + Random.Range (min.z, max.z));
		}
		nextPos = Vector3.Lerp (transform.position, newPos, speed * Time.deltaTime);
		transform.position = nextPos;
	}
}
