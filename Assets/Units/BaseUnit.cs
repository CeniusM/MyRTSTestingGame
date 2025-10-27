using System.Collections.Generic;
using UnityEngine;

public class UnitAttributes
{
    public float Radius;
    public float MoveSpeed;
    public bool IsMoveable => MoveSpeed != 0;

    public UnitAttributes(float radius, float speed)
    {
        Radius = radius;
        MoveSpeed = speed;
    }
}

// This should maybe be the highest called entity, and then have a subclass called BaseUnit and BaseBuilding?

// All unit logic should probaly go to seperate system such that the logic is consistent and predictable for replayability
public abstract class BaseUnit : MonoBehaviour
{
    public readonly CommandQueue commandQueue = new CommandQueue();
    public bool Dead;
    public UnitPathfinder unitPathfinder;

    public UnitAttributes Attributes;

    public Queue<Vector2> CurrentPath = null;
    // Fall back to command target if no path points left
    public Vector2 CurrentTarget => CurrentPath.Count != 0 ? CurrentPath.Peek() : commandQueue.ActiveCommand.TargetPos.Value;

    public abstract void MoveTowards(Vector2 destination);

    public void AssignCommand(BaseCommand command)
    {
        commandQueue.AssignCommand(command);

        // A bit janky, but gotta handle path resets somewhere
        if (command.Type == CommandType.Move && !command.QueueThis)
        {
            CurrentPath = null;
        }
    }

    // Try and finish the current command, else, dequeue and execute next command
    public void HandleCommand()
    {
        if (commandQueue.ActiveCommand == null)
            return;

        BaseCommand command = commandQueue.ActiveCommand;
        switch (command.Type)
        {
            case CommandType.Move:
                if (CurrentPath == null)
                {
                    CurrentPath = new Queue<Vector2>(unitPathfinder.GetPath(this));
                }

                if (CurrentPath.Count == 0)
                {
                    // Should be finished now
                    // Check if we have reached the final target
                    if (IsAtPoint(command.TargetPos.Value))
                    {
                        commandQueue.CommandFinished();
                        CurrentPath = null;
                        break;
                    }
                    //else
                    //{
                    //    throw new System.Exception("Path exhausted but not at target");
                    //}
                }

                // Move to target
                MoveTowards(CurrentTarget);

                // Check if finished a point in the path
                if (IsAtPoint(CurrentTarget))
                {
                    CurrentPath.Dequeue();
                }

                //DebugCurrentPath();

                break;
            case CommandType.Stop:
                commandQueue.StopCommands();
                break;
            default:
                break;
        }
    }

    // Could slowly expand this threshold based on unit radius if it is stuck up against another unit
    public bool IsAtPoint(Vector2 target)
    {
        return Vector2.Distance(transform.position, target) < Attributes.Radius * 0.5f;
    }

    private void DebugCurrentPath()
    {
        if (CurrentPath == null || CurrentPath.Count == 0)
            return;

        var path = CurrentPath.ToArray();

        // Debug their path
        Debug.DrawLine(transform.position, path[0], Color.red, 0.1f, false);

        // Draw between subsequent targets
        for (int i = 0; i < path.Length - 1; i++)
        {
            Debug.DrawLine(path[i], path[i + 1], Color.red, 0.1f, false);
        }
    }
}
