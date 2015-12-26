using UnityEngine;
using System.Collections;

/**
 *
 * @author andre
 */

public class Maze
{
	public class Link
	{
		public Cell a;
		public Cell b;
		public bool breakable = true;
		public bool broken = false;
		public bool isBorder = false;

		public Cell to (Cell aFrom)
		{
			if (a == aFrom) {
				return b;
			} else if (b == aFrom) {
				return a;
			} else {
				return null;
			}
		}
	}

	public class Cell
	{
		public Link[] links = new Link[6];
		public int x, y, z;
		public bool visited = false;
		public GameObject gameObject;

		public Cell (int aX, int aY, int aZ)
		{
			x = aX;
			y = aY;
			z = aZ;
		}
	}

	public int width;
	public int height;
	public int depth;
	public int chanceForUpDown = 50;
	// 50%
	public int chanceForBreakWalls = 0;
	public Cell[] cells;
	public static int DirectionTop = 0;
	public static int DirectionBottom = 1;
	public static int DirectionRight = 2;
	public static int DirectionLeft = 3;
	public static int DirectionForward = 4;
	public static int DirectionBackward = 5;
	protected int[] fDx = new int[6];
	protected int[] fDy = new int[6];
	protected int[] fDz = new int[6];
	protected int[] fRev = new int[6];

	protected void initfdxyz ()
	{
		fDx [0] = 0;
		fDy [0] = 1;
		fDz [0] = 0;
		fRev [0] = 1;
		fDx [1] = 0;
		fDy [1] = -1;
		fDz [1] = 0;
		fRev [1] = 0;
		fDx [2] = 1;
		fDy [2] = 0;
		fDz [2] = 0;
		fRev [2] = 3;
		fDx [3] = -1;
		fDy [3] = 0;
		fDz [3] = 0;
		fRev [3] = 2;
		fDx [4] = 0;
		fDy [4] = 0;
		fDz [4] = 1;
		fRev [4] = 5;
		fDx [5] = 0;
		fDy [5] = 0;
		fDz [5] = -1;
		fRev [5] = 4;
	}

	public int getDeltaX (int aMazeDirection)
	{
		return fDx [aMazeDirection];
	}

	public int getDeltaY (int aMazeDirection)
	{
		return fDy [aMazeDirection];
	}

	public int getDeltaZ (int aMazeDirection)
	{
		return fDz [aMazeDirection];
	}

	public Vector3 getDirectionVector (int aMazeDirection)
	{
		return new Vector3 (fDx [aMazeDirection], fDy [aMazeDirection], fDz [aMazeDirection]);
	}

	public int getReverseDirection (int aMazeDirection)
	{
		return fRev [aMazeDirection];
	}

	public Maze (int aWidth, int aHeight, int aDepth)
	{
		initfdxyz ();
		width = aWidth;
		height = aHeight;
		depth = aDepth;
		cells = new Cell[width * height * depth];
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				for (int z = 0; z < depth; z++) {
					Cell lCell = new Cell (x, y, z);
					if (y == (height - 1)) {
						Link lLink = new Link ();
						lLink.breakable = false;
						lLink.broken = false;
						lLink.isBorder = true;
						lLink.a = lCell;
						lCell.links [0] = lLink;
					}
					if (y == 0) {
						Link lLink = new Link ();
						lLink.breakable = false;
						lLink.broken = false;
						lLink.isBorder = true;
						lLink.a = lCell;
						lCell.links [1] = lLink;
					}
					if (x == (width - 1)) {
						Link lLink = new Link ();
						lLink.breakable = false;
						lLink.broken = false;
						lLink.isBorder = true;
						lLink.a = lCell;
						lCell.links [2] = lLink;
					}
					if (x == 0) {
						Link lLink = new Link ();
						lLink.breakable = false;
						lLink.broken = false;
						lLink.isBorder = true;
						lLink.a = lCell;
						lCell.links [3] = lLink;
					}
					if (z == (depth - 1)) {
						Link lLink = new Link ();
						lLink.breakable = false;
						lLink.broken = false;
						lLink.isBorder = true;
						lLink.a = lCell;
						lCell.links [4] = lLink;
					}
					if (z == 0) {
						Link lLink = new Link ();
						lLink.breakable = false;
						lLink.broken = false;
						lLink.isBorder = true;
						lLink.a = lCell;
						lCell.links [5] = lLink;
					}
					cells [x + width * y + (width * height) * z] = lCell;
				}
                
			}
		}
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				for (int z = 0; z < depth; z++) {
					Cell lCell = get (x, y, z);
					for (int d = 0; d < 6; d++) {
						if (lCell.links [d] == null) {
							Cell lNeighbor = get (x + fDx [d], y + fDy [d], z + fDz [d]);
							//int dd = d ^ 1;
							int dd = fRev [d];
							Link lLink = lNeighbor.links [dd];
							if (lLink != null) {
								lCell.links [d] = lLink;
								lLink.b = lCell;
							} else {
								lLink = new Link ();
								lLink.a = lCell;
								lLink.b = lNeighbor;
								lCell.links [d] = lLink;
								lNeighbor.links [dd] = lLink;
							}
						}
					}
				}
                
			}
		}
	}

	public Cell get (int aX, int aY, int aZ)
	{
		return cells [aX + width * aY + (width * height) * aZ];
	}

	protected int nextRandomInt (int aMax)
	{
		return Mathf.FloorToInt (Random.Range (0.0f, (float)aMax));
	}

	public void build ()
	{
		/*
            create a CellStack (LIFO) to hold a list of cell locations  
            set TotalCells = number of cells in grid  
            choose a cell at random and call it CurrentCell  
            set VisitedCells = 1  

            while VisitedCells < TotalCells 
             find all neighbors of CurrentCell with all walls intact   
             if one or more found 
                choose one at random  
                knock down the wall between it and CurrentCell  
                push CurrentCell location on the CellStack  
                make the new cell CurrentCell  
                add 1 to VisitedCells
             else 
                pop the most recent cell entry off the CellStack  
                make it CurrentCell
             endIf
            endwhile
            */
		//Random lRnd = new Random();
		Cell lCurrent = get (width / 2, height / 2, depth / 2); // first from center
		//Stack<Cell> lStack = new Stack<Cell>();
		Stack lStack = new Stack (); //Cell
		int lVisitedCells = 1;
		int lTotalCells = width * height * depth;
		//ArrayList<Link> lNeighbors = new ArrayList<Link>();
		//ArrayList<Link> lNeighborXZs = new ArrayList<Link>();
		ArrayList lNeighbors = new ArrayList (); //Link
		ArrayList lNeighborXZs = new ArrayList (); //Link
		lCurrent.visited = true;
		while (lVisitedCells < lTotalCells) {
			lNeighbors.Clear ();
			lNeighborXZs.Clear ();
			for (int d = 0; d < 6; d++) {
				Link lNeighbor = lCurrent.links [d];
				;
				if (lNeighbor.breakable && !lNeighbor.to (lCurrent).visited) {
					lNeighbors.Add (lNeighbor);
					if (d > 1) {
						lNeighborXZs.Add (lNeighbor);
					}
				}
			}
			if (!(lNeighbors.Count == 0)) {
				Link lLink;
				if (chanceForUpDown >= 100) {
					lLink = lNeighbors [nextRandomInt (lNeighbors.Count)] as Link;
				} else {
					if ((lNeighborXZs.Count == 0) || (nextRandomInt (200) < chanceForUpDown)) {
						lLink = lNeighbors [(nextRandomInt (Mathf.Min (1, lNeighbors.Count)))] as Link;
					} else {
						lLink = lNeighborXZs [(nextRandomInt (lNeighborXZs.Count))] as Link;
					}
				}
				Cell lNext = lLink.to (lCurrent);
				lLink.broken = true;
				lStack.Push (lCurrent);
				lCurrent = lNext;
				lCurrent.visited = true;
				lVisitedCells++;
			} else {
				if (lStack.Count == 0) {
					break;
				} else {
					lCurrent = lStack.Pop () as Cell;
				}
			}
		}
		/* break more walls to create cycles */
		if (chanceForBreakWalls > 0) {
			for (int x = 0; x < width; x++) {
				for (int z = 0; z < depth; z++) {
					for (int y = 0; y < height; y++) {
						Cell lCell = get (x, y, z);
						int lBrokenCount = 0;
						for (int d = 0; d < 6; d++) {
							if (lCell.links [d].broken) {
								lBrokenCount++;
							}
						}
						if (lBrokenCount < 3) {
							for (int d = 0; d < 6; d++) {
								if (!lCell.links [d].broken && lCell.links [d].breakable) {
									if (nextRandomInt (100) <= chanceForBreakWalls) {
										lCell.links [d].broken = true;
									}
								}
							}
						}
					}
				}
			}
		}
	}
}
