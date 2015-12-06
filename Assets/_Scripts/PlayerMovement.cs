using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
	public float speed = 20.0f;
	public float lookSpeed = 20.0f;
	public AudioClip pickupAudio;

	protected Rigidbody fRig;
	protected Camera fCam;
	protected CharacterController fCC;

	// Use this for initialization
	void Start ()
	{
		fRig = GetComponent<Rigidbody> ();
		fCam = GetComponentInChildren<Camera>();
		fCC = GetComponent<CharacterController>();
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	protected float fLastMouseX;
	protected float fLastMouseY;

	void FixedUpdate ()
	{
		/*
		Vector3 lMoveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
		lMoveDirection = transform.TransformDirection(lMoveDirection);
		lMoveDirection *= speed;
		fCC.Move(lMoveDirection * Time.deltaTime);
		*/

		float lV = Input.GetAxis ("Vertical");
		float lH = Input.GetAxis ("Horizontal");

		float lAngle = transform.rotation.eulerAngles.y;
		Vector3 lF = new Vector3 (Mathf.Sin (Mathf.Deg2Rad * lAngle), 0, Mathf.Cos (Mathf.Deg2Rad * lAngle));
		fRig.AddForce (lF * lV * Time.fixedDeltaTime * speed);
		transform.Rotate (new Vector3 (0, lH * Time.fixedDeltaTime * lookSpeed, 0));

		if (Input.GetMouseButtonDown(0)) {
			float lMX = Input.GetAxis("Mouse X");
			float lMY = Input.GetAxis("Mouse Y");
			if (fLastMouseX == 0 && fLastMouseY == 0) {
				fLastMouseX = lMX;
				fLastMouseY = lMY;
			} else {
				float lDeltaX = fLastMouseX - lMX;
				float lDeltaY = fLastMouseY - lMY;
				fCam.transform.Rotate(new Vector3(lDeltaY,lDeltaX,0));
				fLastMouseX = lMX;
				fLastMouseY = lMY;
			}
		}

	}

	void OnTriggerEnter(Collider aCollider) {
		PickupData lPickup = aCollider.GetComponent<PickupData>();
		if (lPickup) {
			int lScore = lPickup.score;
			lPickup.gameObject.SetActive(false);
			Destroy(lPickup.gameObject);
			GameObject.Find("UIScript").GetComponent<UI>().AddScore(lScore);
			if (pickupAudio != null) {
				AudioSource.PlayClipAtPoint(pickupAudio, aCollider.gameObject.transform.position);
			}
		}
	}
}
