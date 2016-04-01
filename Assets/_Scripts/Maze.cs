using UnityEngine;
using System.Collections;

/**
 *
 * @author andre
 */

public class Maze
{
	public class MazeEventArgs : System.EventArgs
	{
		public Cell cell;
		public Link link;
	}

	public delegate void ChangedEventHandler (Maze aMaze,MazeEventArgs aMazeEvent);

	[System.Serializable]
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

		public override bool Equals (System.Object aObj)
		{
			if (aObj is Point) {
				return Equals ((Point)aObj);
			} else {
				return false;
			}
		}

		public bool Equals (Point aP)
		{
			return aP.x == x && aP.y == y && aP.z == z;
		}

		public override int GetHashCode ()
		{
			return x ^ y ^ z;
		}

		public override string ToString ()
		{
			return System.String.Format ("({0},{1},{2})", x, y, z);
		}

		public static bool IsNullOrEmpty (Point aP)
		{
			return aP == null || (aP.x == -1 && aP.y == -1 && aP.z == -1);
		}

		public static bool operator == (Point a, Point b)
		{
			// If both are null, or both are same instance, return true.
			if (System.Object.ReferenceEquals (a, b)) {
				return true;
			}

			// If one is null, but not both, return false.
			if (((object)a == null) || ((object)b == null)) {
				return false;
			}

			// Return true if the fields match:
			return a.x == b.x && a.y == b.y && a.z == b.z;
		}

		public static bool operator != (Point a, Point b)
		{
			return !(a == b);
		}

	}

	public class Link
	{
		public Cell a;
		public Cell b;

		public Cell cell { get { return a != null ? a : b; } }

		protected bool fBreakable = true;

		public bool breakable {
			get { return fBreakable; }
			set {
				if (fBreakable != value) {
					fBreakable = value;
					cell.maze.OnChanged (new MazeEventArgs () { link = this });
				}
			}
		}

		protected bool fBroken = false;

		public bool broken {
			get{ return fBroken; }
			set {
				if (fBroken != value) {
					fBroken = value;
					cell.maze.OnChanged (new MazeEventArgs () { link = this });
				}
			}
		}

		protected bool fIsBorder = false;

		public bool isBorder {
			get{ return fIsBorder; }
			set {
				if (fIsBorder != value) {
					fIsBorder = value;
					cell.maze.OnChanged (new MazeEventArgs (){ link = this });
				}
			}
		}

		public Link (Cell aCell)
		{
			a = aCell;
		}

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
		public Maze maze;
		public Link[] links = new Link[6];
		public Point pos = new Point ();

		public int x { get { return pos.x; } }

		public int y { get { return pos.y; } }

		public int z { get { return pos.z; } }

		protected bool fVisited = false;

		public bool visited {
			get { return fVisited; }
			set {
				if (fVisited != value) {
					fVisited = value;
					maze.OnChanged (new MazeEventArgs () { cell = this });
				}
			}
		}

		public GameObject gameObject;

		protected bool fPlayerHasVisited = false;

		public bool playerHasVisited {
			get { return fPlayerHasVisited; }
			set {
				if (fPlayerHasVisited != value) {
					fPlayerHasVisited = value;
					maze.OnChanged (new MazeEventArgs () { cell = this });
				}
			}
		}

		protected bool fFailVisited = false;

		public bool failVisited {
			get { return fFailVisited; }
			set {
				if (fFailVisited != value) {
					fFailVisited = value;
					maze.OnChanged (new MazeEventArgs () { cell = this });
				}
			}
		}

		public Cell (Maze aMaze, int aX, int aY, int aZ)
		{
			maze = aMaze;
			pos.x = aX;
			pos.y = aY;
			pos.z = aZ;
		}
	}

	public event ChangedEventHandler Changed;

	public void OnChanged (MazeEventArgs aArgs)
	{
		if (Changed != null) {
			Changed (this, aArgs);
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

	public enum Direction
	{
		No = -1,
		Top = DirectionTop,
		Bottom = DirectionBottom,
		Right = DirectionRight,
		Left = DirectionLeft,
		Forward = DirectionForward,
		Backward = DirectionBackward
	}

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
					Cell lCell = new Cell (this, x, y, z);
					if (y == (height - 1)) {
						Link lLink = new Link (lCell);
						lLink.breakable = false;
						lLink.broken = false;
						lLink.isBorder = true;
						//lLink.a = lCell;
						lCell.links [0] = lLink;
					}
					if (y == 0) {
						Link lLink = new Link (lCell);
						lLink.breakable = false;
						lLink.broken = false;
						lLink.isBorder = true;
						//lLink.a = lCell;
						lCell.links [1] = lLink;
					}
					if (x == (width - 1)) {
						Link lLink = new Link (lCell);
						lLink.breakable = false;
						lLink.broken = false;
						lLink.isBorder = true;
						//lLink.a = lCell;
						lCell.links [2] = lLink;
					}
					if (x == 0) {
						Link lLink = new Link (lCell);
						lLink.breakable = false;
						lLink.broken = false;
						lLink.isBorder = true;
						//lLink.a = lCell;
						lCell.links [3] = lLink;
					}
					if (z == (depth - 1)) {
						Link lLink = new Link (lCell);
						lLink.breakable = false;
						lLink.broken = false;
						lLink.isBorder = true;
						//lLink.a = lCell;
						lCell.links [4] = lLink;
					}
					if (z == 0) {
						Link lLink = new Link (lCell);
						lLink.breakable = false;
						lLink.broken = false;
						lLink.isBorder = true;
						//lLink.a = lCell;
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
								lLink = new Link (lCell);
								//lLink.a = lCell;
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

	public bool PosInMaze (int aX, int aY, int aZ)
	{
		return aX >= 0 && aX < width && aY >= 0 && aY < height && aZ >= 0 && aZ < depth;
	}

	public Cell get (int aX, int aY, int aZ)
	{
		if (PosInMaze (aX, aY, aZ)) {
			return cells [aX + width * aY + (width * height) * aZ];
		} else {
			return null;
		}
	}

	protected int nextRandomInt (int aMax)
	{
		//return Mathf.FloorToInt (Random.Range (0.0f, (float)aMax));
		return Random.Range (0, aMax); // aMax will never returned
	}

	protected class BuildStep
	{
		public Cell lCurrent = null;
		//Cells
		public Stack lStack = new Stack ();
		public int lVisitedCells = 1;
		public int lTotalCells;
		//Links
		public ArrayList lNeighbors = new ArrayList ();
		//Links
		public ArrayList lNeighborXZs = new ArrayList ();
		public int x = 0;
		public int y = 0;
		public int z = 0;

		public int w, h, d;

		public BuildStep (int width, int height, int depth)
		{
			w = width;
			h = height;
			d = depth;
			lTotalCells = width * height * depth;
		}
	}

	protected BuildStep fStep = null;

	public Cell getCurrentBuildCell ()
	{
		return fStep == null ? null : fStep.lCurrent;
	}

	protected bool doBuildStep ()
	{
		if (fStep == null) {
			fStep = new BuildStep (width, height, depth);
			while (fStep.lCurrent == null || fStep.lCurrent.visited) {
				fStep.lCurrent = get (Random.Range (0, width), Random.Range (0, height), Random.Range (0, depth));
			}
		}
		if (fStep.lVisitedCells < fStep.lTotalCells) {
			fStep.lCurrent.visited = true;
			fStep.lNeighbors.Clear ();
			fStep.lNeighborXZs.Clear ();
			for (int d = 0; d < 6; d++) {
				Link lNeighbor = fStep.lCurrent.links [d];
				if (lNeighbor.breakable && !lNeighbor.to (fStep.lCurrent).visited) {
					fStep.lNeighbors.Add (lNeighbor);
					if (d > 1) {
						fStep.lNeighborXZs.Add (lNeighbor);
					}
				}
			}
			if (!(fStep.lNeighbors.Count == 0)) {
				Link lLink;
				if (chanceForUpDown >= 100) {
					lLink = fStep.lNeighbors [nextRandomInt (fStep.lNeighbors.Count)] as Link;
				} else {
					if ((fStep.lNeighborXZs.Count == 0) || (nextRandomInt (200) < chanceForUpDown)) {
						lLink = fStep.lNeighbors [(nextRandomInt (Mathf.Min (1, fStep.lNeighbors.Count)))] as Link;
					} else {
						lLink = fStep.lNeighborXZs [(nextRandomInt (fStep.lNeighborXZs.Count))] as Link;
					}
				}
				Cell lNext = lLink.to (fStep.lCurrent);
				lLink.broken = true;
				fStep.lStack.Push (fStep.lCurrent);
				fStep.lCurrent = lNext;
				fStep.lCurrent.visited = true;
				fStep.lVisitedCells++;
			} else {
				if (fStep.lStack.Count == 0) {
					fStep.lVisitedCells = fStep.lTotalCells; // exit
				} else {
					fStep.lCurrent = fStep.lStack.Pop () as Cell;
				}
			}
			bool lNotReady = chanceForBreakWalls > 0 || fStep.lVisitedCells < fStep.lTotalCells;
			if (!lNotReady) {
				ClearVisited();
			}
			return lNotReady;
		} else { //if (fStep.lVisitedCells >= fStep.lTotalCells) {
			/* break more walls to create cycles */
			if (chanceForBreakWalls > 0) {
				fStep.lCurrent = get (fStep.x, fStep.y, fStep.z);
				int lBrokenCount = 0;
				for (int d = 0; d < 6; d++) {
					if (fStep.lCurrent.links [d].broken) {
						lBrokenCount++;
					}
				}
				if (lBrokenCount < 3) {
					for (int d = 0; d < 6; d++) {
						if (!fStep.lCurrent.links [d].broken && fStep.lCurrent.links [d].breakable) {
							if (nextRandomInt (100) <= chanceForBreakWalls) {
								fStep.lCurrent.links [d].broken = true;
							}
						}
					}
				}
				fStep.x++;
				if (fStep.x >= fStep.w) {
					fStep.x = 0;
					fStep.z++;
					if (fStep.z >= fStep.d) {
						fStep.z = 0;
						fStep.y++;
						if (fStep.y >= fStep.h) {
							ClearVisited ();
							return false;
						}
					}
				}
				return true;
			} else {
				ClearVisited ();
				return false;
			}
		}
	}

	public bool buildNextStep ()
	{
		return doBuildStep ();
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

		while (doBuildStep ())
			;
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
			// waren wir schon in einer nachbar zelle?
			// --> dann war das ein Umweg
			/*
			for (int lDir = 0; lDir < 6; lDir++) {
				if (aCurrent.links [lDir].broken) {
					Cell lNext = aCurrent.links [lDir].to (aCurrent);
					if (lNext.visited && lNext != ((WayPoint)aStack.Peek ()).cell) {
						aCurrent.visited = false;
						aCurrent.failVisited = true;
						return false;
					}
				}
			}
			*/
			for (int lDir = 0; lDir < 6; lDir++) {
				if (aCurrent.links [lDir].broken) {
					Cell lNext = aCurrent.links [lDir].to (aCurrent);
					if (!lNext.visited) {
						aStack.Push (new WayPoint (aCurrent, lDir));
						lFound = CheckWay (lNext, aTarget, aStack);
						if (!lFound) {
							lNext.failVisited = true;
							aStack.Pop ();
						} else {
							return true;
						}
					}
				}
			}
			aCurrent.failVisited = true;
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