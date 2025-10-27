using AStarPathfindingMaze;
using System.Collections.Generic;
using Timer = System.Diagnostics.Stopwatch;
using System.Linq;
using UnityEngine;
using System;
using UnityEngine.Tilemaps;

public class PathfinderDebugger : MonoBehaviour
{
    public int Size;
    public GameObject PathStart;
    public GameObject PathEnd;
    public PathFinder Pathfinder;
    public bool On;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PathStart = GameObject.Find("PathStart");
        PathEnd = GameObject.Find("PathEnd");
        Pathfinder = new PathFinder(Size, Size);
    }

    // AI GEN
    void UpdatePathFinderMap()
    {
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
        Array.Clear(Pathfinder.Map, 0, Pathfinder.Map.Length);

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

                Pathfinder.SetCellWalkable(x + (Size/2), y + (Size / 2), false);
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!On)
            return;

        // Fill the pathfinder grid with all the objects in the Grid->Nonwalkable tilemap
        UpdatePathFinderMap();

        Vector2 start = PathStart.transform.position;
        Vector2 end = PathEnd.transform.position;

        Vector2 halfSize = new Vector2(Size / 2f, Size / 2f);

        // Transform
        start += halfSize;
        end += halfSize;

        Timer sw = Timer.StartNew();
        List<Coord> path = Pathfinder.SearchPath(
            start: new Coord() { x = (int)Mathf.Floor(start.x), y = (int)Mathf.Floor(start.y) },
            end: new Coord() { x = (int)Mathf.Floor(end.x), y = (int)Mathf.Floor(end.y) }
        );
        double timeMs = sw.Elapsed.TotalMilliseconds;
        //Debug.Log("Pathfinder timer: " + timeMs + " ms, Path length: " + path.Count);

        List<Vector2> vPath = path.Select(c => new Vector2(c.x, c.y) - halfSize).ToList();

        // Draw path
        Vector2 offset = new Vector2(0.5f, 0.5f);
        Debug.DrawLine(start - halfSize, vPath[0] + offset, Color.red);
        for (int i = 0; i < vPath.Count - 1; i++)
        {
            Debug.DrawLine(vPath[i] + offset, vPath[i + 1] + offset, Color.red);
            DrawWaypoint(vPath[i+1] + offset, 0.5f, Color.blue);
        }
    }

    private void DrawWaypoint(Vector2 p, float size, Color color)
    {
        Vector2 NW = new Vector2(p.x - size / 2, p.y + size / 2);
        Vector2 NE = new Vector2(p.x + size / 2, p.y + size / 2);
        Vector2 SW = new Vector2(p.x - size / 2, p.y - size / 2);
        Vector2 SE = new Vector2(p.x + size / 2, p.y - size / 2);
        Debug.DrawLine(NW, NE, color, 0.1f, false);
        Debug.DrawLine(NE, SE, color, 0.1f, false);
        Debug.DrawLine(SE, SW, color, 0.1f, false);
        Debug.DrawLine(SW, SE, color, 0.1f, false);
    }
}
