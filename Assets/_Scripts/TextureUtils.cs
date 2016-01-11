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

	public static void DrawMaze (Texture2D aTexture, Maze aMaze, int w, Color aColor)
	{
		for (int x = 0; x < aMaze.width; x++) {
			for (int z = 0; z < aMaze.depth; z++) {
				for (int lDir = 2; lDir < 6; lDir++) {
					if (!aMaze.get (x, 0, z).links [lDir].broken) {
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
	}
}
