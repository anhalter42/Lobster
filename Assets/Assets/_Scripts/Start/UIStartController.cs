using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class UIStartController : MonoBehaviour
{

	public Material material;


	Texture2D texture;

	// Use this for initialization
	void Start ()
	{
		DontDestroyOnLoad (GameObject.Find ("Master"));
		texture = new Texture2D (500, 500);
		if (material) {
			material.mainTexture = texture;
		}
		px = x = Random.Range (dx, (texture.width - dx) / dx);
		py = y = Random.Range (dy, (texture.height - dy) / dy);
		dxx = new int[4];
		dyy = new int[4];
		dxx [0] = -1;
		dyy [0] = 0;
		dxx [1] = +1;
		dyy [1] = 0;
		dxx [2] = 0;
		dyy [2] = -1;
		dxx [3] = 0;
		dyy [3] = +1;
		di = new int[16];
		di [0] = 0;
		di [1] = 2;
		di [2] = 1;
		di [3] = 0;
		di [4] = 3;
		di [5] = 1;
		di [6] = 3;
		di [7] = 2;
		di [8] = 0;
		di [9] = 1;
		di [10] = 3;
		di [11] = 0;
		di [12] = 1;
		di [13] = 2;
		di [14] = 3;
		di [15] = 2;
	}

	int x, y;
	int px, py;
	int dx = 4;
	int dy = 4;
	float dc = 0.5f;
	int[] dxx;
	int[] dyy;
	int[] di;
	// Update is called once per frame

	void FillPixels (int x1, int y1, int x2, int y2, Color c)
	{
		int minx = Mathf.Min (x1 * dx, x2 * dx);
		int miny = Mathf.Min (y1 * dy, y2 * dy);
		int maxx = Mathf.Max (x1 * dx, x2 * dx);
		int maxy = Mathf.Max (y1 * dy, y2 * dy);
		for (int lx = minx; lx <= maxx; lx++) {
			for (int ly = miny; ly <= maxy; ly++) {
				texture.SetPixel (lx, ly, c);
//				for (int ldx = 0; ldx < dx; ldx++) {
//					for (int ldy = 0; ldy < dy; ldy++) {
//						texture.SetPixel (lx * dx + ldx, ly * dy + ldy, c);
//					}
//				}
			}
		}
	}

	void Update ()
	{
		Color c = texture.GetPixel (x * dx, y * dy);
		if (c.g >= dc) {
			c.g = 0.0f;
			c.r = 0.0f;
			c.b = 0.0f;
			c.a = 1.0f;
			//texture.SetPixel (x * dx, y * dy, c);
			FillPixels (px, py, x, y, c);
			int nx, ny;
			Color nc;
			bool lf = false;
			int ii = Random.Range (0, di.Length / 2);
			for (int i = 0; i < dxx.Length; i++) {
				nx = x + dxx [di [ii + i]];
				ny = y + dyy [di [ii + i]];
				if (nx > 1 && nx < texture.width - 1 && ny > 1 && ny < texture.height - 1) {
					nc = texture.GetPixel (nx * dx, ny * dy);
					if (nc.g >= dc) {
						px = x;
						py = y;
						x = nx;
						y = ny;
						lf = true;
						break;
					}
				}
			}
			if (!lf) {
				px = x = Random.Range (dx, (texture.width - dx) / dx);
				py = y = Random.Range (dy, (texture.height - dy) / dy);
			}
		} else {
			px = x = Random.Range (dx, (texture.width - dx) / dx);
			py = y = Random.Range (dy, (texture.height - dy) / dy);
		}
		texture.Apply (false);
	}

	public void ButtonStart ()
	{
		SceneManager.LoadScene ("ChooseLevel", LoadSceneMode.Single);
	}
}
