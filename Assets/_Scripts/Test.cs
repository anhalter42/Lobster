using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;

public class Test : MonoBehaviour
{
	RenderTexture texture;
	Camera renderCamera;
	RawImage rawImage;

	// Use this for initialization
	void Start ()
	{
		print ("All " + Resources.FindObjectsOfTypeAll (typeof(UnityEngine.Object)).Length);
		print ("Textures " + Resources.FindObjectsOfTypeAll (typeof(Texture)).Length);
		print ("AudioClips " + Resources.FindObjectsOfTypeAll (typeof(AudioClip)).Length);
		print ("Meshes " + Resources.FindObjectsOfTypeAll (typeof(Mesh)).Length);
		print ("Materials " + Resources.FindObjectsOfTypeAll (typeof(Material)).Length);
		print ("GameObjects " + Resources.FindObjectsOfTypeAll (typeof(GameObject)).Length);
		print ("Components " + Resources.FindObjectsOfTypeAll (typeof(Component)).Length);
		rawImage = GameObject.Find("RawImage").GetComponent<RawImage>();
		texture = new RenderTexture (512, 512, 24);
		renderCamera = GameObject.Find("RenderCamera").GetComponent<Camera>();
		renderCamera.targetTexture = texture;
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	public void Generate ()
	{
		StartCoroutine("DoGenerate");
	}

	public IEnumerator DoGenerate ()
	{
		Transform lParent = GameObject.Find ("Cell").transform;
		GameObject[] lGOs = Resources.LoadAll<GameObject> ("Prefabs");
		foreach (GameObject lGO in lGOs) {
			print (lGO.name);
			GameObject lO = Instantiate (lGO);
			lO.transform.SetParent (lParent, false);
			renderCamera.Render();
			RenderTexture.active = texture;
			Texture2D lSprite = new Texture2D(texture.width,texture.height, TextureFormat.RGB24, false);
			lSprite.ReadPixels(new Rect(0, 0, lSprite.width, lSprite.height), 0, 0);
			lSprite.Apply();
			rawImage.texture = lSprite;
			//UnityEditor.AssetDatabase.AddObjectToAsset(lSprite,lGO.name);
			string lPath = Path.Combine(Application.temporaryCachePath, lGO.name + ".png");
			print(lPath);
			File.WriteAllBytes(lPath, lSprite.EncodeToPNG());
			Destroy(lO);
			yield return new WaitForSeconds(1.0f);
		}
	}
}
