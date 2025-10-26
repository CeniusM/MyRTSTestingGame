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
public abstract class BaseUnit : MonoBehaviour
{
    public readonly CommandQueue commandQueue = new CommandQueue();
    public bool Dead;

    public UnitAttributes Attributes;

    public abstract void MoveTowards(Vector2 destination);

    public void AssignCommand(BaseCommand command)
    {
        commandQueue.AssignCommand(command);
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
                // Action
                MoveTowards(command.TargetPos.Value);

                // Test if done
                if (Vector2.Distance(transform.position, command.TargetPos.Value) < 0.1)
                    commandQueue.CommandFinished();
                break;
            case CommandType.Stop:
                commandQueue.StopCommands();
                break;
            default:
                break;
        }
    }

    //public void OnSelected()
    //{

    //}

    //public void OnDeselected()
    //{

    //}
}
