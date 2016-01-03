using UnityEngine;
using System.Collections;

/**
 *
 * @author andre
 */

public class Maze
{
	public class Point
	{
		public int x, y, z;

		public Point ()
		{
		}

		public Point (int aX, int aY, int aZ)
		{
			x = aX;
			y = aY;
			z = aZ;
		}

		public override bool Equals(System.Object aObj)
		{
			if (aObj is Point) {
				return Equals((Point)aObj);
			} else {
				return false;
			}
		}

		public bool Equals(Point aP)
		{
			return aP.x == x && aP.y == y && aP.z == z;
		}

		public override int GetHashCode ()
		{
			return x ^ y ^ z;
		}

		public override string ToString() 
		{
			return System.String.Format("({0},{1},{2})",x,y,z);
		}

		public static bool operator ==(Point a, Point b)
		{
			// If both are null, or both are same instance, return true.
			if (System.Object.ReferenceEquals(a, b))
			{
				return true;
			}

			// If one is null, but not both, return false.
			if (((object)a == null) || ((object)b == null))
			{
				return false;
			}

			// Return true if the fields match:
			return a.x == b.x && a.y == b.y && a.z == b.z;
		}

		public static bool operator !=(Point a, Point b)
		{
			return !(a == b);
		}

	}

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
		public Point pos = new Point ();

		public int x { get { return pos.x; } }

		public int y { get { return pos.y; } }

		public int z { get { return pos.z; } }

		public bool visited = false;
		public GameObject gameObject;

		public Cell (int aX, int aY, int aZ)
		{
			pos.x = aX;
			pos.y = aY;
			pos.z = aZ;
		}
	}

	public int width;
	public int height;
	public int depth;
	public int chanceForUpDown = 50;
	// 50%
	public int chanceForBreakWalls = 0;
	public Cell[] cells;
	public const int DirectionTop = 0;
	public const int DirectionBottom = 1;
	public const int DirectionRight = 2;
	public const int DirectionLeft = 3;
	public const int DirectionForward = 4;
	public const int DirectionBackward = 5;
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

	public Cell get (Point aP)
	{
		return get (aP.x, aP.y, aP.z);
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
		ClearVisited ();
	}

	public void ClearVisited ()
	{
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				for (int z = 0; z < depth; z++) {
					get (x, y, z).visited = false;
				}
			}
		}
	}

	public class WayPoint
	{
		public Cell cell;
		public int direction;

		public WayPoint (Cell aCell, int aDir)
		{
			cell = aCell;
			direction = aDir;
		}
	}

	protected bool CheckWay (Cell aCurrent, Cell aTarget, Stack aStack)
	{
		bool lFound = false;
		aCurrent.visited = true;
		if (aCurrent != aTarget) {
			for (int lDir = 0; lDir < 6; lDir++) {
				if (aCurrent.links [lDir].broken) {
					Cell lNext = aCurrent.links [lDir].to (aCurrent);
					if (!lNext.visited) {
						aStack.Push (new WayPoint (aCurrent, lDir));
						lFound = CheckWay (lNext, aTarget, aStack);
						if (!lFound) {
							aStack.Pop ();
						} else {
							return true;
						}
					}
				}
			}
			return false;
		} else {
			return true;
		}
	}

	public WayPoint[] FindWay (Point aFrom, Point aTo)
	{
		Stack lStack = new Stack ();
		ClearVisited ();
		Cell lCurrent = get (aFrom);
		Cell lTarget = get (aTo);
		CheckWay (lCurrent, lTarget, lStack);
		ClearVisited ();
		WayPoint[] lResult = new WayPoint[lStack.Count];
		for (int i = 0; i < lResult.Length; i++) {
			lResult [lResult.Length - i - 1] = lStack.Pop () as WayPoint;
		}
		return lResult;
	}
}