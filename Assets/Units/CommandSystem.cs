using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum CommandType
{
    Move,
    //Attack,
    Stop,
    //Patrol,
    //HoldPosition,
    //UseAbility,
    // Add more command types as needed
}

public /*abstract*/ class BaseCommand
{
    // For example move command or attack command + shift
    // Otherwise we clear the queue and set this as the active command
    public bool QueueThis;

    // Very simple system to start with
    public CommandType Type;

    // For move and attack commands
    public Vector2? TargetPos;

    public BaseCommand(bool queueThis, CommandType type)
    {
        QueueThis = queueThis;
        Type = type;
    }
}

public class CommandQueue
{
    public readonly Queue<BaseCommand> Queue = new Queue<BaseCommand>();
    public BaseCommand ActiveCommand;

    public IEnumerable<BaseCommand> AllCommands
    {
        get
        {
            if (ActiveCommand != null)
                yield return ActiveCommand;
            foreach (var command in Queue)
            {
                yield return command;
            }
        }
    }

    public void AssignCommand(BaseCommand command)
    {
        if (ActiveCommand == null)
        {
            ActiveCommand = command;
            return;
        }

        if (command.QueueThis)
        {
            Queue.Enqueue(command);
        }
        else
        {
            Queue.Clear();
            ActiveCommand = command;
        }
    }

    public void StopCommands()
    {
        Queue.Clear();
        ActiveCommand = null;
    }

    public void CommandFinished()
    {
        if (Queue.Count > 0)
        {
            ActiveCommand = Queue.Dequeue();
        }
        else
        {
            ActiveCommand = null;
        }
    }

    public BaseCommand[] GetAllCommandsOfType(CommandType commandType)
    {
        if (!AllCommands.Any(c => c.Type == commandType))
            return Array.Empty<BaseCommand>();

        return AllCommands.Where(c => c.Type == commandType).ToArray();
    }
}

//public static class CommandFactory
//{

//}