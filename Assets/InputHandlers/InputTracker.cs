using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum UserInputType
{
    Keypress,
    MouseClick,
    //MouseMovement
}

public class UserInput
{
    public UserInputType InputType;
    public double TimeStamp;

    public KeyCode KeyCode;
    public Vector2 MouseClickPosition;
    //public int
}

// We get all the inputs so that the game logic can run through all the events since the last fixedframe such that we dont drop any inputs
// When the next fixedframe starts, the game logic can get the current history and let a new one accumulate for next fixedframe
public class InputTracker : MonoBehaviour
{
    // Accumulates each frame for game logic to use on each fixed update
    public List<UserInput> InputQueue;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InputQueue = new List<UserInput>();
        
        var a = InputSystem.actions;

        //a.FindAction("Attack").performed += InputTracker_performed;

        //InputAction action = new InputAction();

        Debug.Log(InputActionMap.ToJson(InputSystem.actions.actionMaps));

    }

    // Update is called once per frame
    void Update()
    {

    }
}


















//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.InputSystem;
//using UnityEngine.InputSystem.LowLevel;

//public enum UserInputType
//{
//    Keypress,
//    MouseClick,
//    //MouseMovement
//}

//public class UserInput
//{
//    public UserInputType InputType;
//    public double TimeStamp;

//    public KeyCode KeyCode;
//    public Vector2 MouseClickPosition;
//    //public int
//}

//// We get all the inputs so that the game logic can run through all the events since the last fixedframe such that we dont drop any inputs
//// When the next fixedframe starts, the game logic can get the current history and let a new one accumulate for next fixedframe
//public class InputTracker : MonoBehaviour
//{
//    public List<UserInput> InputQueue;
//    public GameObject InputTrackerObj; // Contains the Player Input Actions


//    public void KeyInputChange(InputAction.CallbackContext context)
//    {
//        //InputSystem.

//        //InputSettings.UpdateMode.ProcessEventsManually
//        //if (!context.performed)
//        //    Debug.Log("Was not performed?");
//        //if (context.performed)
//        //{
//        //    Debug.Log("Something happend: " + context.time);
//        //}
//        //Debug.Log(context.)
//        //Debug.Log(context);
//        //Debug.Log(context.valueType);
//        //Debug.Log(context.control);
//        //Debug.Log(context.ReadValueAsObject());
//    }


//    InputRecorder recorder = null;
//    public void SpacebarIGuess(InputAction.CallbackContext context)
//    {
//        //if (!context.performed)
//        //    return;

//        //string filePath = "C:\\Users\\ceniu\\MyRTSGame\\TempLogs\\input_logs.txt";

//        //// Set up a recorder with such that it automatically grows in size as needed.
//        //if (recorder == null)
//        //{
//        //    recorder = new InputRecorder();
//        //    //recorder.   
//        //    //recorder.Enable();
//        //    Debug.Log("Tracing");
//        //}
//        //else
//        //{
//        //    //recorder.WriteTo(filePath);


//        //    recorder.Disable();
//        //    recorder = null;
//        //    Debug.Log("Stopped tracing");
//        //}

//        //// Load trace from same file.
//        ////var loadedTrace = InputEventTrace.LoadFrom("mytrace.inputtrace");
//    }

//    // Start is called once before the first execution of Update after the MonoBehaviour is created
//    void Start()
//    {
//        //History = new List<UserInput>();
//        //InputSystem.onActionChange += InputSystem_onActionChange;

//        InputTrackerObj = GameObject.Find("InputTracker");

//        var a = InputSystem.actions;

//        a.FindAction("Attack").performed += InputTracker_performed;

//        InputAction action = new InputAction();

//        //Debug.Log(InputActionMap.ToJson(InputSystem.actions.actionMaps));

//    }

//    private void InputTracker_performed(InputAction.CallbackContext obj)
//    {
//        Debug.Log("Somebody attacked");
//    }

//    private void InputSystem_onActionChange(object obj, InputActionChange change)
//    {
//        // obj can be either an InputAction or an InputActionMap
//        // depending on the specific change.

//        switch (change)
//        {
//            case InputActionChange.ActionStarted:
//            case InputActionChange.ActionPerformed:
//            case InputActionChange.ActionCanceled:
//                Debug.Log($"{((InputAction)obj)} {change}");
//                break;
//        }
//    }

//    // Update is called once per frame
//    void Update()
//    {

//    }
//}