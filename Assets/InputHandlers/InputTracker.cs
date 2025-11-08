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
    public List<UserInput> InputQueue;

    public void KeyInputChange(InputAction.CallbackContext context)
    {
        //if (!context.performed)
        //    Debug.Log("Was not performed?");
        //if (context.performed)
        //{
        //    Debug.Log("Something happend: " + context.time);
        //}
        //Debug.Log(context.)
        Debug.Log(context);
        Debug.Log(context.valueType);
        //Debug.Log(context.action.)
    }

    public void SpacebarIGuess()
    {
        Debug.Log("Spacebar pressed");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //History = new List<UserInput>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
