// Could use a class like this to translate the user inputs to something the game world could use
// But for now the WorldManager takes care of it all
using UnityEngine;

public class UserInputInterpreter
{
    WorldManager worldManager;
    PlayerData playerData;
    FrameInputTracker inputTracker;

    public UserInputInterpreter(WorldManager worldManager, PlayerData playerData, FrameInputTracker inputTracker)
    {
        this.inputTracker = inputTracker;
        this.worldManager = worldManager;
        this.playerData = playerData;
    }

    public Vector2 dragStartPos = new Vector2();
    public bool isDragging = false;

    // Called at the start of each FixedUpdate in WorldManager
    public void InterpretInput()
    {
        var inputs = inputTracker.LastFrameInputsHistory;

        foreach (var input in inputs)
        {

        }
    }
}