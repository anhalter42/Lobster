using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;

public class Test : MonoBehaviour
{
	public int textureWidth = 64;
	public int textureHeight = 64;

	RenderTexture texture;
	Camera renderCamera;
	RawImage rawImage;

	public string prefabsPath = "Prefabs";

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
		rawImage = GameObject.Find ("RawImage").GetComponent<RawImage> ();
		texture = new RenderTexture (textureWidth, textureHeight, 24);
		renderCamera = GameObject.Find ("RenderCamera").GetComponent<Camera> ();
		renderCamera.targetTexture = texture;
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	public void Generate ()
	{
		prefabsPath = "Prefabs";
		StartCoroutine ("DoGenerate");
	}

	public void GenerateDungeon ()
	{
		prefabsPath = "Dungeon/Prefabs";
		StartCoroutine ("DoGenerate");
	}

	public IEnumerator DoGenerate ()
	{
		Transform lParent = GameObject.Find ("Cell").transform;
		GameObject[] lGOs = Resources.LoadAll<GameObject> (prefabsPath);
		foreach (GameObject lGO in lGOs) {
			print (lGO.name);
			GameObject lO = Instantiate (lGO);
			lO.transform.SetParent (lParent, false);
			renderCamera.Render ();
			RenderTexture.active = texture;
			Texture2D lSprite = new Texture2D (texture.width, texture.height, TextureFormat.ARGB32, false);
			lSprite.ReadPixels (new Rect (0, 0, lSprite.width, lSprite.height), 0, 0);
			lSprite.Apply ();
			rawImage.texture = lSprite;
			//UnityEditor.AssetDatabase.AddObjectToAsset(lSprite,lGO.name);
			string lPath = Path.Combine (Application.temporaryCachePath, prefabsPath.Replace("Prefabs","Sprites"));
			Directory.CreateDirectory(lPath);
			lPath = Path.Combine (Application.temporaryCachePath, Path.Combine (prefabsPath.Replace("Prefabs","Sprites"), lGO.name + ".png"));
			print (lPath);
			File.WriteAllBytes (lPath, lSprite.EncodeToPNG ());
			Destroy (lO);
			yield return new WaitForSeconds (1.0f);
		}
	}
}
