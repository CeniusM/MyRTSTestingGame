using AStarPathfindingMaze;
using System.Collections.Generic;
using Timer = System.Diagnostics.Stopwatch;
using System.Linq;
using UnityEngine;

public class PathfinderDebugger : MonoBehaviour
{
    public int Size;
    public GameObject PathStart;
    public GameObject PathEnd;
    public UnitPathfinder Pathfinder;
    public bool On;
    public bool RemoveRedundantCornors;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PathStart = GameObject.Find("PathStart");
        PathEnd = GameObject.Find("PathEnd");
        Pathfinder = null;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!On)
            return;

        if (Pathfinder == null)
            Pathfinder = FindFirstObjectByType<UnitPathfinder>();

        Vector2 start = PathStart.transform.position;
        Vector2 end = PathEnd.transform.position;

        List<Vector2> vPath = Pathfinder.GeneratePath(start, end, RemoveRedundantCornors);

        // Draw path
        Debug.DrawLine(start, vPath[0], Color.red);
        for (int i = 0; i < vPath.Count - 1; i++)
        {
            Debug.DrawLine(vPath[i], vPath[i + 1], Color.red);
            DrawWaypoint(vPath[i + 1], 0.4f, Color.blue);
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
        Debug.DrawLine(SW, NW, color, 0.1f, false);
    }
}
