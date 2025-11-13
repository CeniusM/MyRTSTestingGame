using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

public enum UserInputType
{
    KeyChange,
    MouseClick,
    MouseMovement
}

// Can use abstraction, but like... idk
public class UserInput
{
    // For all
    public UserInputType InputType;
    public double TimeStamp;

    // General
    public bool IsPressed;

    // Keyboard
    public Key KeyCode;

    // Mouse
    public Vector2 MousePosition;
    public Vector2 PrevMousePosition; // Tracked in InputTracker
    public Vector2 MouseDelta => MousePosition - PrevMousePosition;
    public int MouseButton = -1; // -1=undefined, 0=Leftclick, 1=Middleclick, 2=Rightclick, 3=fowardbutton, 4=backwardsbutton
    public string MouseButtonName = string.Empty;

    public bool IsLeftClick => MouseButton == 0;
    public bool IsMiddleClick => MouseButton == 1;
    public bool IsRightClick => MouseButton == 2;

    public override string ToString()
    {
        if (InputType == UserInputType.KeyChange)
            return $"[{TimeStamp:F3}] Key: {KeyCode} {(IsPressed ? "Pressed" : "Released")}";
        else
            return $"[{TimeStamp:F3}] Mouse: ({MouseButtonName}) {(IsPressed ? "Pressed" : "Released")} at {MousePosition}";
    }
}

// We get all the inputs so that the game logic can run through all the events since the last fixedframe such that we dont drop any inputs
// When the next fixedframe starts, the game logic can get the current history and let a new one accumulate for next fixedframe
public class InputTracker : MonoBehaviour
{
    // We get multible x and y positions independently, so we can just combine them all when they are in a streak between two button clicks
    //[SerializeField] public bool CombineMouseMovements = true;

    // A collection of the userinputs from last frame. The history sorted from 0->first to N->last
    public UserInput[] LastFrameInputsHistory;

    // Accumulates each frame for game logic to use on each fixed update
    private List<UserInput> _inputList;

    private IDisposable _keyboardObserver;
    private IDisposable _mouseObserver;

    private Vector2 _savedMousePosition = -Vector2.one;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LastFrameInputsHistory = Array.Empty<UserInput>();

        _inputList = new List<UserInput>();

        _keyboardObserver = InputSystem.onEvent.ForDevice(Keyboard.current).Call(HandleKeyboardRawEvent);
        _mouseObserver = InputSystem.onEvent.ForDevice(Mouse.current).Call(HandleMouseRawEvent);
    }

    // Remove inputsystem onevent listiner when program closes
    void OnApplicationQuit()
    {
        _keyboardObserver.Dispose();
        _mouseObserver.Dispose();
    }

    // Update is called once per frame
    void Update()
    {
    }

    // This will run first, so all the other GameObjects can use the UserInputHistory
    void FixedUpdate()
    {
        // Store the new input history sorted so the new ones can be added and shown next frame

        lock (this)
        {
            //if (_inputList.Count > 3)
            //    Debug.Log("");

            LastFrameInputsHistory = _inputList.ToArray();

            // Hope this is right :)
            Array.Sort(LastFrameInputsHistory, (x, y) => x.TimeStamp < y.TimeStamp ? -1 : 1);

            _inputList.Clear();
        }
    }

    public void AddUserInputEvent(UserInput evt)
    {
        lock (this)
        {
            _inputList.Add(evt);
        }
    }

    private void HandleKeyboardRawEvent(InputEventPtr eventPtr)
    {
        if (!eventPtr.valid)
        {
            Debug.LogWarning("Some invalid keyboard event was given");
            return;
        }

        if (eventPtr.type != new FourCC("STAT"))
            return;

        int changesCount = 0;
        double timeStamp = eventPtr.time;

        foreach (var change in eventPtr.EnumerateChangedControls())
        {
            if (change is KeyControl keyControl)
            {
                AddUserInputEvent(new UserInput()
                {
                    InputType = UserInputType.KeyChange,
                    TimeStamp = timeStamp,
                    KeyCode = keyControl.keyCode,
                    IsPressed = keyControl.magnitude == 0, // 0==down, 1==up
                    //IsPressed = !keyControl.isPressed, // FOR SOME REASON INVERTED?
                });

                changesCount++;
            }
            else
            {
                Debug.LogWarning("There was a keyboard change that did not involve a keycontrol");
                continue;
            }
        }

        if (changesCount > 1)
            Debug.LogWarning("There was somehow more than 1 event during the same timestamp");
    }

    private void HandleMouseRawEvent(InputEventPtr eventPtr)
    {
        if (!eventPtr.valid)
        {
            Debug.LogWarning("Some invalid keyboard event was given");
            return;
        }

        bool hasPositionValue = Mouse.current.position.ReadUnprocessedValueFromEvent(eventPtr, out var currentPosition);

        if (!hasPositionValue)
            Debug.LogWarning("Mouse event aint got no position");

        Vector2 prevPosition = _savedMousePosition;

        if (prevPosition == -Vector2.one) // No position has been saved yet
            prevPosition = currentPosition;

        //if (eventPtr.type != new FourCC("STAT"))
        //    return;

        double timeStamp = eventPtr.time;

        foreach (var change in eventPtr.EnumerateChangedControls())
        {
            Debug.Log(change.GetType().FullName);
            if (change is ButtonControl buttonControl)
            {
                AddUserInputEvent(
                    new UserInput()
                    {
                        InputType = UserInputType.MouseClick,
                        TimeStamp = timeStamp,
                        IsPressed = buttonControl.magnitude == 0,
                        MousePosition = currentPosition,
                        PrevMousePosition = prevPosition,
                        MouseButtonName = buttonControl.name,
                        MouseButton = buttonControl.name switch
                        {
                            "leftButton" => 0,
                            "middleButton" => 1,
                            "rightButton" => 2,
                            "forwardButton" => 3,
                            "backButton" => 4,
                            _ => throw new NotImplementedException("Unknow mouse button: " + buttonControl.shortDisplayName)
                        }
                    }
                );
            }

            // Movement of the mouse? Onestly not sure how this works
            // I hope i can just ignore the deltas, and use the position x and y
            if (change is AxisControl axisControl)
            {

                //axisControl.
                Debug.Log("AxisControl: " + axisControl.path + ": " + axisControl.magnitude);
            }

            //if (change is DeltaControl deltaControl)
            //{
            //    Debug.Log("DeltaControl");
            //}
        }

        _savedMousePosition = currentPosition;
    }
}