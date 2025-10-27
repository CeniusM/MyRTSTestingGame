using System.Collections.Generic;
using UnityEngine;

public class RallyPointRenderer : MonoBehaviour
{
    private PlayerData playerData;
    private EffectsRenderer effectsRenderer;

    void Start()
    {
        playerData = FindAnyObjectByType<PlayerData>();
        effectsRenderer = FindAnyObjectByType<EffectsRenderer>();
    }

    void Update()
    {
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
                    effectsRenderer.DrawLine(previousPoint, pathPoint, Color.red, 0.05f);
                    effectsRenderer.DrawHollowSquare(pathPoint, 0.2f, 0.1f, Color.magenta);
                    previousPoint = pathPoint;
                }
                effectsRenderer.DrawLine(previousPoint, moveCommands[0].TargetPos.Value, Color.blue, 0.05f);
            }
            else
            {
                effectsRenderer.DrawLine(unit.transform.position, moveCommands[0].TargetPos.Value, Color.magenta, 0.05f);
            }

            // Draw between subsequent targets
            for (int i = 0; i < moveCommands.Length - 1; i++)
            {
                effectsRenderer.DrawLine(moveCommands[i].TargetPos.Value, moveCommands[i + 1].TargetPos.Value, Color.yellow, 0.05f);
                effectsRenderer.DrawHollowSquare(moveCommands[i + 1].TargetPos.Value, 0.2f, 0.1f, Color.magenta);
            }
        }
    }
}
