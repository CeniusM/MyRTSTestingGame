using UnityEngine;

// This is where the main logic of the game comes from
public class WorldManager : MonoBehaviour
{
    public LayerMask UnitLayer;
    public LayerMask GroundLayer;

    private FrameInputTracker _inputTracker;
    private UserInputInterpreter _inputInterpreter;
    private PlayerData _playerData;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _inputTracker = FindAnyObjectByType<FrameInputTracker>();

        if (_inputTracker == null)
        {
            Debug.LogError("InputTracker not found in the scene!");
        }
        else
        {
            Debug.Log("WorldManager connected to InputTracker.");
        }

        _playerData = FindAnyObjectByType<PlayerData>();

        if (_playerData == null)
        {
            Debug.LogError("PlayerData not found in the scene!");
        }
        else
        {
            Debug.Log("WorldManager connected to PlayerData.");
        }

        _inputInterpreter = new UserInputInterpreter(this, _playerData, _inputTracker);
    }

    void FixedUpdate()
    {
        _inputInterpreter.InterpretInput();
    }

    // Maybe move to other class later
    public LayerMask unitLayer;
    public LayerMask groundLayer;
    public Color selectionBoxColor = new Color(0, 1, 0, 0.2f); // translucent green
    void OnGUI()
    {
        Rect GetScreenRect(Vector2 screenPosition1, Vector2 screenPosition2)
        {
            // Move origin from bottom left to top left
            screenPosition1.y = Screen.height - screenPosition1.y;
            screenPosition2.y = Screen.height - screenPosition2.y;
            var topLeft = Vector2.Min(screenPosition1, screenPosition2);
            var bottomRight = Vector2.Max(screenPosition1, screenPosition2);
            return Rect.MinMaxRect(topLeft.x, topLeft.y, bottomRight.x, bottomRight.y);
        }

        void DrawScreenRect(Rect rect, Color color)
        {
            GUI.color = color;
            GUI.DrawTexture(rect, Texture2D.whiteTexture);
            GUI.color = Color.white;
        }

        void DrawScreenRectBorder(Rect rect, float thickness, Color color)
        {
            // Top
            DrawScreenRect(new Rect(rect.xMin, rect.yMin, rect.width, thickness), color);
            // Left
            DrawScreenRect(new Rect(rect.xMin, rect.yMin, thickness, rect.height), color);
            // Right
            DrawScreenRect(new Rect(rect.xMax - thickness, rect.yMin, thickness, rect.height), color);
            // Bottom
            DrawScreenRect(new Rect(rect.xMin, rect.yMax - thickness, rect.width, thickness), color);
        }

        if (_inputInterpreter.isDragging)
        {
            var rect = GetScreenRect(_inputInterpreter.dragStartPos, Input.mousePosition);
            DrawScreenRect(rect, selectionBoxColor);
            DrawScreenRectBorder(rect, 2, Color.green);
        }
    }
}




//// Test UserInputs
//if (_inputTracker == null || _inputTracker.LastFrameInputsHistory == null)
//    return;

//var inputs = _inputTracker.LastFrameInputsHistory;

////if (inputs.Length > 0)
////{
////    Debug.Log("Frame Input Events:");
////    foreach (var input in inputs)
////    {
////        //if (input.InputType == UserInputType.MouseMovement)
////        Debug.Log(" * " + input);
////    }
////}