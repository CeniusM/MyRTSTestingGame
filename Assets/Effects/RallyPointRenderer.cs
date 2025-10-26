using System.Collections.Generic;
using UnityEngine;

public class RallyPointRenderer : MonoBehaviour
{
    private PlayerData playerData;
    private List<LineRenderer> activeLines = new List<LineRenderer>();
    [SerializeField] private Material lineMaterial;
    [SerializeField] private float lineWidth = 0.05f;
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

            // Draw line from unit to first target
            DrawVisibleLine(unit.transform.position, moveCommands[0].TargetPos.Value);

            // Draw between subsequent targets
            for (int i = 0; i < moveCommands.Length - 1; i++)
            {
                DrawVisibleLine(moveCommands[i].TargetPos.Value, moveCommands[i + 1].TargetPos.Value);
            }
        }
    }

    private void DrawVisibleLine(Vector2 from, Vector2 to)
    {
        GameObject lineObj = new GameObject("RallyLine");
        LineRenderer lr = lineObj.AddComponent<LineRenderer>();
        lr.material = lineMaterial != null ? lineMaterial : new Material(Shader.Find("Sprites/Default"));
        lr.startWidth = lr.endWidth = lineWidth;
        lr.positionCount = 2;
        lr.SetPosition(0, from);
        lr.SetPosition(1, to);
        lr.startColor = lr.endColor = Color.green;
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
