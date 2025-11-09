using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

public enum UserInputType
{
    KeyChange,
    MouseClick,
    //MouseMovement
}

public class UserInput
{
    public UserInputType InputType;
    public double TimeStamp;

    // Keyboard
    public Key KeyCode;
    public bool IsPressed;

    // Mouse
    public Vector2 MouseClickPosition;
    //public int

    public override string ToString()
    {
        if (InputType == UserInputType.KeyChange)
            return $"[{TimeStamp:F3}] Key {KeyCode} {(IsPressed ? "Pressed" : "Released")}";
        else
            return $"[{TimeStamp:F3}] Mouse Click at {MouseClickPosition}";
    }
}

// We get all the inputs so that the game logic can run through all the events since the last fixedframe such that we dont drop any inputs
// When the next fixedframe starts, the game logic can get the current history and let a new one accumulate for next fixedframe
public class InputTracker : MonoBehaviour
{
    // A collection of the userinputs from last frame. The history sorted from 0->first to N->last
    public UserInput[] LastFrameInputsHistory;

    // Accumulates each frame for game logic to use on each fixed update
    private List<UserInput> _inputList;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LastFrameInputsHistory = Array.Empty<UserInput>();

        _inputList = new List<UserInput>();

        InputSystem.onEvent.ForDevice(Keyboard.current).Call(HandleKeyboardRawEvent);
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

    // Remove inputsystem onevent listiner when program closes
    private void OnApplicationQuit()
    {
        //InputSystem.onEvent.
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
                    IsPressed= keyControl.magnitude == 0, // 0==down, 1==up
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
}