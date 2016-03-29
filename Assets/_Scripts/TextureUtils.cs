using UnityEngine;
using System.Collections;

public class TextureUtils
{
	
	public static void DrawLine (Texture2D tex, int x0, int y0, int x1, int y1, Color col)
	{
		int dy = (int)(y1 - y0);
		int dx = (int)(x1 - x0);
		int stepx, stepy;

		if (dy < 0) {
			dy = -dy;
			stepy = -1;
		} else {
			stepy = 1;
		}
		if (dx < 0) {
			dx = -dx;
			stepx = -1;
		} else {
			stepx = 1;
		}
		dy <<= 1;
		dx <<= 1;

		float fraction = 0;

		tex.SetPixel (x0, y0, col);
		if (dx > dy) {
			fraction = dy - (dx >> 1);
			while (Mathf.Abs (x0 - x1) > 1) {
				if (fraction >= 0) {
					y0 += stepy;
					fraction -= dx;
				}
				x0 += stepx;
				fraction += dy;
				tex.SetPixel (x0, y0, col);
			}
		} else {
			fraction = dx - (dy >> 1);
			while (Mathf.Abs (y0 - y1) > 1) {
				if (fraction >= 0) {
					x0 += stepx;
					fraction -= dy;
				}
				y0 += stepy;
				fraction += dx;
				tex.SetPixel (x0, y0, col);
			}
		}
		tex.SetPixel (x1, y1, col);
	}

	public static void DrawRect (Texture2D aTexture, int x, int y, int w, int h, Color aColor)
	{
		for (int ly = y; ly < y + h; ly++) {
			DrawLine (aTexture, x, ly, x + w, ly, aColor);
		}
	}

	public static void DrawMaze (Texture2D aTexture, Maze aMaze, int w, Color aColor)
	{
		for (int x = 0; x < aMaze.width; x++) {
			for (int z = 0; z < aMaze.depth; z++) {
				DrawMazeCell (aTexture, aMaze.get (x, 0, z), w, aColor);
			}
		}
	}

	public static void DrawMazeCell (Texture2D aTexture, Maze.Cell lCell, int w, Color aColor)
	{
		int x = lCell.x;
		int z = lCell.z;
		Color lCellColor = Color.white;
		if (lCell.playerHasVisited) {
			lCellColor = Color.gray;
		} else if (lCell.visited) {
			lCellColor = new Color (0.8f, 0.8f, 1f);
		}
		DrawRect (aTexture, x * w, z * w, w, w, lCellColor);
		if (lCell.gameObject) {
			MapModifier[] lMods = lCell.gameObject.GetComponentsInChildren<MapModifier> (false);
			foreach (MapModifier lMod in lMods) {
				if (!lMod.ShowIfPlayerVisited || (lMod.ShowIfPlayerVisited && lCell.playerHasVisited)) {
					int x1 = 0, x2 = 0, y1 = 0, y2 = 0;
					int d = (w - 1) / 2;
					switch (lMod.mode) {
					case MapModifier.Mode.Maze:
						switch (lMod.direction) {
						case Maze.Direction.Left:
							x1 = d + x * w - d;
							x2 = x1;
							y1 = d + z * w - d;
							y2 = d + z * w + d + 1;
							DrawLine (aTexture, x1, y1, x2, y2, lMod.color);
							break;
						case Maze.Direction.Right:
							x1 = d + x * w + d;
							x2 = x1;
							y1 = d + z * w - d;
							y2 = d + z * w + d + 1;
							DrawLine (aTexture, x1, y1, x2, y2, lMod.color);
							break;
						case Maze.Direction.Forward:
							x1 = d + x * w - d;
							x2 = d + x * w + d + 1;
							y1 = d + z * w + d;
							y2 = y1;
							DrawLine (aTexture, x1, y1, x2, y2, lMod.color);
							break;
						case Maze.Direction.Backward:
							x1 = d + x * w - d;
							x2 = d + x * w + d + 1;
							y1 = d + z * w - d;
							y2 = y1;
							DrawLine (aTexture, x1, y1, x2, y2, lMod.color);
							break;
						case Maze.Direction.Top:
						case Maze.Direction.Bottom:
						case Maze.Direction.No:
							DrawRect (aTexture, x * w, z * w, w, w, lMod.color);
							break;
						}
						break;
					case MapModifier.Mode.Texture:
						if (lMod.texture) {
							x1 = d + x * w - d;
							y1 = d + z * w - d;
							Color32[] lCols = lMod.texture.GetPixels32 ();
							aTexture.SetPixels32 (x1, y1, lMod.texture.width, lMod.texture.height, lCols);
						}
						break;
					}
				}
			}
		}
		for (int lDir = 2; lDir < 6; lDir++) {
			if (!lCell.links [lDir].broken) {
				int x1 = 0, x2 = 0, y1 = 0, y2 = 0;
				int d = (w - 1) / 2;
				switch (lDir) {
				case Maze.DirectionLeft:
					x1 = d + x * w - d;
					x2 = x1;
					y1 = d + z * w - d;
					y2 = d + z * w + d + 1;
					break;
				case Maze.DirectionRight:
					x1 = d + x * w + d;
					x2 = x1;
					y1 = d + z * w - d;
					y2 = d + z * w + d + 1;
					break;
				case Maze.DirectionForward:
					x1 = d + x * w - d;
					x2 = d + x * w + d + 1;
					y1 = d + z * w + d;
					y2 = y1;
					break;
				case Maze.DirectionBackward:
					x1 = d + x * w - d;
					x2 = d + x * w + d + 1;
					y1 = d + z * w - d;
					y2 = y1;
					break;
				}
				DrawLine (aTexture, x1, y1, x2, y2, aColor);
			}
		}
	}
}
