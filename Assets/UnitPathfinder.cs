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
    public bool DoSmoothingParse;
    public bool CanMoveDiagonallyThroughCornors;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pathFinder = new PathFinder(size, size);
    }

    // Update is called once per frame
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
        pathFinder.CanMoveDiagonallyThroughCornors = CanMoveDiagonallyThroughCornors;

        if (unit.commandQueue.ActiveCommand == null || unit.commandQueue.ActiveCommand.Type != CommandType.Move)
            return new List<Vector2>();

        Vector2 start = unit.transform.position;
        Vector2 end = unit.commandQueue.ActiveCommand.TargetPos.Value;
        Vector2 halfSize = new Vector2(size / 2f, size / 2f);
        // Transform
        start += halfSize;
        end += halfSize;
        List<Coord> cPath = pathFinder.SearchPath(
            start: new Coord { x = (int)Mathf.Floor(start.x), y = (int)Mathf.Floor(start.y) },
            end: new Coord { x = (int)Mathf.Floor(end.x), y = (int)Mathf.Floor(end.y) },
            doSmoothingParse: DoSmoothingParse
        );

        // Offset should later be based on the unit size
        Vector2 offset = new Vector2(0.5f, 0.5f); // So it is in the middle of the tiles
        List<Vector2> vPath = cPath.Select(c => new Vector2(c.x, c.y) - halfSize + offset).ToList();

        //// Debug their path
        //Debug.DrawLine(start - halfSize, vPath[0] + offset, Color.red, 0.1f, false);

        //// Draw between subsequent targets
        //for (int i = 0; i < vPath.Count - 1; i++)
        //{
        //    Debug.DrawLine(vPath[i] + offset, vPath[i + 1] + offset, Color.red, 0.1f, false);
        //}

        // Add the very end, and remove the last point if it is in the same tile as the end
        if (vPath.Count > 0)
        {
            Vector2 lastPathPoint = vPath[vPath.Count - 1];
            if (Mathf.FloorToInt(lastPathPoint.x) == Mathf.FloorToInt(end.x - halfSize.x) &&
                Mathf.FloorToInt(lastPathPoint.y) == Mathf.FloorToInt(end.y - halfSize.y))
            {
                Debug.Log("Removed last..");
                vPath.RemoveAt(vPath.Count - 1);
            }
        }
        vPath.Add(end - halfSize);

        return vPath;
    }
}
