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

    // State
    public Vector2 lastMousePos = new Vector2();

    public Vector2 dragStartPos = Vector2.negativeInfinity;
    public bool isDragging => dragStartPos != Vector2.negativeInfinity;

    // Settings
    public float dragThreshold = 5f; // How long the mouse has to move while pressed to start a drag

    // Called at the start of each FixedUpdate in WorldManager
    public void InterpretInput()
    {
        var inputs = inputTracker.LastFrameInputsHistory;

        foreach (var input in inputs)
        {
            if (input.InputType == UserInputType.MouseClick)
                HandleMouseClick(input);
            else if (input.InputType == UserInputType.MouseMovement)
                HandleMouseMovement(input);
            else if (input.InputType == UserInputType.KeyChange)
                HandleKeyChange(input);
        }
    }

    private void HandleMouseClick(UserInput input)
    {
        if (input.IsPressed)
        {
            if (input.MouseButton == MouseButtonType.LeftClick)
            {
                dragStartPos = input.MousePosition;
            }
            else if (input.MouseButton == MouseButtonType.RightClick)
            {
                // Right click action
                worldManager.ClickedAt(input.MousePosition, input.MouseButton);
            }
        }
        else
        {
            if (isDragging)
            {
                float dragDistance = Vector2.Distance(dragStartPos, input.MousePosition);
                if (dragDistance >= dragThreshold)
                {
                    // End drag action
                    worldManager.FinishedDrag(dragStartPos, input.MousePosition);
                }
                else
                {
                    // It was just a click, not a drag
                    worldManager.ClickedAt(input.MousePosition, input.MouseButton);
                }
            }
        }

        lastMousePos = input.MousePosition;
    }

    private void HandleMouseMovement(UserInput input)
    {


        lastMousePos = input.MousePosition;
    }

    private void HandleKeyChange(UserInput input)
    {
    }
}