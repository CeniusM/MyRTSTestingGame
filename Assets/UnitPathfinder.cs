using AStarPathfindingMaze;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class UnitPathfinder : MonoBehaviour
{
    public PathFinder pathFinder;
    public int size;
    public bool CanMoveDiagonallyThroughCornors;
    public bool RemoveRedundantCornors;
    public bool DoSmoothingParse;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pathFinder = new PathFinder(size, size);
    }

    // Update is called once per frame
    // This should just be updated when ever a new building is placed or removed, or something like that
    void FixedUpdate()
    {
        // I just hope this runs before any of the units use the pathfinder, otherwise we proably get some nasty bugs

        // Find the "Nonwalkable" tilemap
        var nonWalkableGO = GameObject.Find("Nonwalkable");
        if (nonWalkableGO == null)
        {
            Debug.LogError("No GameObject named 'Nonwalkable' found!");
            return;
        }

        var nonWalkableTilemap = nonWalkableGO.GetComponent<Tilemap>();
        if (nonWalkableTilemap == null)
        {
            Debug.LogError("'Nonwalkable' GameObject has no Tilemap component!");
            return;
        }

        // Clear existing map
        Array.Clear(pathFinder.Map, 0, pathFinder.Map.Length);

        // Get the bounds of the tilemap
        BoundsInt bounds = nonWalkableTilemap.cellBounds;

        // Iterate through all cells in the tilemap
        foreach (var pos in bounds.allPositionsWithin)
        {
            TileBase tile = nonWalkableTilemap.GetTile(pos);
            if (tile != null)
            {
                // Convert tilemap position to your pathfinder grid coords if needed
                int x = pos.x;
                int y = pos.y;

                pathFinder.SetCellWalkable(x + (size / 2), y + (size / 2), false);
            }
        }
    }

    // Only returns a list of coordinates for the path, if the units active command is a move command
    public List<Vector2> GetPath(BaseUnit unit)
    {
        if (unit.commandQueue.ActiveCommand == null || unit.commandQueue.ActiveCommand.Type != CommandType.Move)
            return new List<Vector2>();

        Vector2 start = unit.transform.position;
        Vector2 end = unit.commandQueue.ActiveCommand.TargetPos.Value;

        return GeneratePath(start, end, RemoveRedundantCornors, DoSmoothingParse, unit.Attributes.Radius);

        //Vector2 halfSize = new Vector2(size / 2f, size / 2f);
        //// Transform
        //start += halfSize;
        //end += halfSize;
        //List<Coord> cPath = pathFinder.SearchPath(
        //    start: new Coord { x = (int)Mathf.Floor(start.x), y = (int)Mathf.Floor(start.y) },
        //    end: new Coord { x = (int)Mathf.Floor(end.x), y = (int)Mathf.Floor(end.y) },
        //    doSmoothingParse: DoSmoothingParse
        //);

        //// Offset should later be based on the unit size
        //Vector2 offset = new Vector2(0.5f, 0.5f); // So it is in the middle of the tiles
        //List<Vector2> vPath = cPath.Select(c => new Vector2(c.x, c.y) - halfSize + offset).ToList();

        ////// Debug their path
        ////Debug.DrawLine(start - halfSize, vPath[0] + offset, Color.red, 0.1f, false);

        ////// Draw between subsequent targets
        ////for (int i = 0; i < vPath.Count - 1; i++)
        ////{
        ////    Debug.DrawLine(vPath[i] + offset, vPath[i + 1] + offset, Color.red, 0.1f, false);
        ////}

        //// Add the very end, and remove the last point if it is in the same tile as the end
        //if (vPath.Count > 0)
        //{
        //    // I think this is allways the case?
        //    Vector2 lastPathPoint = vPath[vPath.Count - 1];
        //    if (Mathf.FloorToInt(lastPathPoint.x) == Mathf.FloorToInt(end.x - halfSize.x) &&
        //        Mathf.FloorToInt(lastPathPoint.y) == Mathf.FloorToInt(end.y - halfSize.y))
        //    {
        //        //Debug.Log("Removed last..");
        //        vPath.RemoveAt(vPath.Count - 1);
        //    }
        //}
        //vPath.Add(end - halfSize);

        //if (DoSmoothingParse)
        //{
        //    // Here we try and straightenout 

        //}

        //return vPath;
    }

    public List<Vector2> GeneratePath(Vector2 worldStart, Vector2 worldEnd, bool doRemoveRedundantNodes, bool doSmoothPathing, float offsetMag = 0.5f)
    {
        Vector2 halfSize = new Vector2(size / 2f, size / 2f);

        Vector2 mapStart = worldStart + halfSize;
        Vector2 mapEnd = worldEnd + halfSize;

        List<Coord> cPath = pathFinder.SearchPath(
            new Coord { x = (int)Mathf.Floor(mapStart.x), y = (int)Mathf.Floor(mapStart.y) },
            new Coord { x = (int)Mathf.Floor(mapEnd.x), y = (int)Mathf.Floor(mapEnd.y) }
        );

        if (doRemoveRedundantNodes)
            cPath = RemoveRedundantNodes(cPath);

        // Convert cPath to vertors
        List<Vector2> vPath = cPath.Select(c=>new Vector2(c.x, c.y)).ToList();

        // Remove last point and replace it with the end
        vPath[vPath.Count - 1] = mapEnd;

        if (doSmoothPathing)
            vPath = SmoothPath(mapStart, vPath);



        //// Offset should later be based on the unit size
        Vector2 offset = new Vector2(offsetMag, offsetMag); // So it is in the middle of the tiles
        //// Convert coords to world and offset so they are in the middle of the squares (Or so, still working on it)
        //List<Vector2> vPath = cPath.Select(c => new Vector2(c.x, c.y) - halfSize + offset).ToList();


        // Convert path back to world space
        for (int i = 0; i < vPath.Count; i++)
        {
            vPath[i] = vPath[i] - halfSize + offset;
        }
        vPath[vPath.Count - 1] -= offset;

        return vPath;
    }

    // Will first remove all coords that is just in a straight line
    // Then it will try to cut corners by checking if a straight line between two points is walkable
    private List<Coord> RemoveRedundantNodes(List<Coord> path)
    {
        /*
        
        Idk man, i say we try and make a lot of colliders out of the obstacals in the tilemap,
        and than just cast rays in worldspace to see if they collide with anything.. but that is for another day


        */


        for (int i = 1; i < path.Count - 1; i++)
        {
            Coord prev = path[i - 1];
            Coord current = path[i];
            Coord next = path[i + 1];

            // Check if current is in a straight line between prev and next
            if ((prev.x == current.x && current.x == next.x) ||
                (prev.y == current.y && current.y == next.y) ||
                (Math.Abs(prev.x - current.x) == Math.Abs(prev.y - current.y) &&
                 Math.Abs(current.x - next.x) == Math.Abs(current.y - next.y) &&
                 (next.x - current.x) * (current.y - prev.y) == (next.y - current.y) * (current.x - prev.x)))
            {
                path.RemoveAt(i);
                i--;
            }
        }

        return path;
    }

    private List<Vector2> SmoothPath(Vector2 startPos, List<Vector2> path)
    {
        // Can cast two parralel lines from the first point to the second point with an offset of the radius to either side of the unit
        // and check that none of thes lines intesects with a wall

        bool[,] map = pathFinder.Map;

        // We check each cornor and see if we can remove it
        for (int i = 0; i < path.Count - 1; i++)
        {
            Vector2 start = i == 0 ? startPos : path[i - 1];
            Vector2 cornor = path[i];
            Vector2 end = path[i + 1];

            // Convert back to map coord
            bool cornorCanBeCut = CanCutCorner(start, end, map);

            if (cornorCanBeCut)
            {
                path.RemoveAt(i);
                i--;
            }
        }

        return path;
    }

    // Can possibly make the ai think it can walk through walls and other stuff
    private bool CanCutCorner(Vector2 start, Vector2 end, bool[,] map)
    {
        // Use Bresenham's line algorithm to check if the line between start and end intersects any walls
        int x0 = Mathf.FloorToInt(start.x);
        int y0 = Mathf.FloorToInt(start.y);
        int x1 = Mathf.FloorToInt(end.x);
        int y1 = Mathf.FloorToInt(end.y);

        int dx = Mathf.Abs(x1 - x0);
        int dy = Mathf.Abs(y1 - y0);
        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;
        int err = dx - dy;

        while (true)
        {
            // Check if the current point is a wall
            if (x0 >= 0 && y0 >= 0 && x0 < map.GetLength(0) && y0 < map.GetLength(1) && map[x0, y0])
            {
                return false; // Line intersects a wall
            }

            // If we reached the end point, the line is clear
            if (x0 == x1 && y0 == y1)
            {
                break;
            }

            int e2 = 2 * err;
            if (e2 > -dy)
            {
                err -= dy;
                x0 += sx;
            }
            if (e2 < dx)
            {
                err += dx;
                y0 += sy;
            }
        }

        return true; // No walls intersect the line
    }
}
