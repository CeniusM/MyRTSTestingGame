using AStarPathfindingMaze;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class UnitPathfinder : MonoBehaviour
{
    public PathFinder pathFinder;
    public int size;
    public bool BlockCornorCutting;
    public bool RemoveRedundantCornors;

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
        Array.Clear(pathFinder.BlockedMap, 0, pathFinder.BlockedMap.Length);

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
    public List<Vector2> GetUnitPath(BaseUnit unit)
    {
        if (unit.commandQueue.ActiveCommand == null || unit.commandQueue.ActiveCommand.Type != CommandType.Move)
        {
            Debug.LogError("Tried to get a path for a unit that does not have an active move command");
            return new List<Vector2>();
        }

        Vector2 start = unit.transform.position;
        Vector2 end = unit.commandQueue.ActiveCommand.TargetPos.Value;

        return GeneratePath(start, end, RemoveRedundantCornors, unit.Attributes.Radius);
    }

    public List<Vector2> GeneratePath(Vector2 worldStart, Vector2 worldEnd, bool doRemoveRedundantNodes, float offsetMag = 0.5f)
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

        // Offset should later be based on the unit size
        Vector2 offset = new Vector2(offsetMag, offsetMag); // So it is in the middle of the tiles

        // Convert map coords to world space and offset so they are in the middle of the squares (Or so, still working on it)
        List<Vector2> vPath = cPath.Select(c => new Vector2(c.x, c.y) - halfSize + offset).ToList();

        // Remove last point and replace it with the end
        vPath[vPath.Count - 1] = worldEnd;

        return vPath;
    }

    // Will first remove all coords that is just in a straight line
    // Then it will try to cut corners by checking if a straight line between two points is walkable
    private List<Coord> RemoveRedundantNodes(List<Coord> path)
    {
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

    // Not a priority, the pathing works fine for now
    //private List<Vector2> SmoothPath(Vector2 startPos, List<Vector2> path)
    //{
    //    // Iterate over each cornor of the path.
    //    // Cast two parralel lines from the first point to the second past the conor point with an offset of the radius to either side of the unit
    //    // and check that none of thes lines intesects with a wall
    //Idk man, i say we try and make a lot of colliders out of the obstacals in the tilemap,
    //and than just cast rays in worldspace to see if they collide with anything.. but that is for another day
    //}
}
