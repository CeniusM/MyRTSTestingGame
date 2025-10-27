using System.Collections.Generic;
using UnityEngine;

public class RallyPointRenderer : MonoBehaviour
{
    private PlayerData playerData;
    private List<LineRenderer> activeLines = new List<LineRenderer>();
    [SerializeField] private Material lineMaterial;
    [SerializeField] private float lineWidth = 0.1f;
    [SerializeField] private GameObject arrowPrefab; // Optional for arrows

    void Start()
    {
        playerData = FindAnyObjectByType<PlayerData>();
    }

    void Update()
    {
        ClearOldLines();

        foreach (var unit in playerData.activeGroup.UnitsInGroup)
        {
            BaseCommand[] moveCommands = unit.commandQueue.GetAllCommandsOfType(CommandType.Move);
            if (moveCommands.Length == 0)
                continue;

            // Draw line from unit to first target with its path
            if (unit.CurrentPath != null && unit.CurrentPath.Count > 0)
            {
                Vector2 previousPoint = unit.transform.position;
                foreach (var pathPoint in unit.CurrentPath)
                {
                    DrawVisibleLine(previousPoint, pathPoint, Color.blue);
                    DrawWaypoint(pathPoint, 0.2f, Color.red);
                    previousPoint = pathPoint;
                }
                DrawVisibleLine(previousPoint, moveCommands[0].TargetPos.Value, Color.blue);
            }
            else
            {
                DrawVisibleLine(unit.transform.position, moveCommands[0].TargetPos.Value, Color.green);
            }

            // Draw between subsequent targets
            for (int i = 0; i < moveCommands.Length - 1; i++)
            {
                DrawVisibleLine(moveCommands[i].TargetPos.Value, moveCommands[i + 1].TargetPos.Value, Color.green);
                DrawWaypoint(moveCommands[i + 1].TargetPos.Value, 0.2f, Color.red);
            }
        }
    }

    private void DrawWaypoint(Vector2 p, float size, Color color)
    {
        Vector2 NW = new Vector2(p.x - size / 2, p.y + size / 2);
        Vector2 NE = new Vector2(p.x + size / 2, p.y + size / 2);
        Vector2 SW = new Vector2(p.x - size / 2, p.y - size / 2);
        Vector2 SE = new Vector2(p.x + size / 2, p.y - size / 2);
        DrawVisibleLine(NW, NE, color);
        DrawVisibleLine(NE, SE, color);
        DrawVisibleLine(SE, SW, color);
        DrawVisibleLine(SW, SE, color);
    }

    private void DrawVisibleLine(Vector2 from, Vector2 to, Color lineColor)
    {
        GameObject lineObj = new GameObject("RallyLine");
        LineRenderer lr = lineObj.AddComponent<LineRenderer>();
        lr.material = lineMaterial != null ? lineMaterial : new Material(Shader.Find("Sprites/Default"));
        lr.startWidth = lr.endWidth = lineWidth;
        lr.positionCount = 2;
        lr.SetPosition(0, from);
        lr.SetPosition(1, to);
        lr.startColor = lr.endColor = lineColor;
        lr.sortingOrder = 10; // Make sure it's visible above ground

        activeLines.Add(lr);

        // Optional: Add arrow at the end
        if (arrowPrefab)
        {
            Vector2 dir = (to - from).normalized;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            GameObject arrow = Instantiate(arrowPrefab, to, Quaternion.Euler(0, 0, angle - 90f));
            activeLines.Add(arrow.GetComponent<LineRenderer>());
        }
    }

    private void ClearOldLines()
    {
        foreach (var lr in activeLines)
        {
            if (lr != null)
                Destroy(lr.gameObject);
        }
        activeLines.Clear();
    }
}
